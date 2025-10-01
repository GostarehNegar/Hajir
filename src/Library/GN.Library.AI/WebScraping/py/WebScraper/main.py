#!/usr/bin/env python3
"""
Website Scraper for ChromaDB RAG Operations

This script crawls an entire website, extracts text content,
chunks it appropriately, and stores it in ChromaDB for RAG operations.

Requirements:
pip install requests beautifulsoup4 chromadb sentence-transformers lxml python-dotenv

Usage:
python website_scraper.py
"""

import os
import re
import time
import hashlib
import logging
from urllib.parse import urljoin, urlparse, urlunparse
from urllib.robotparser import RobotFileParser
from typing import Set, List, Dict, Optional, Tuple
from dataclasses import dataclass
from concurrent.futures import ThreadPoolExecutor, as_completed
import json

import requests
from bs4 import BeautifulSoup, Comment
import chromadb
from chromadb.config import Settings
from sentence_transformers import SentenceTransformer

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s',
    handlers=[
        logging.FileHandler('scraper.log'),
        logging.StreamHandler()
    ]
)
logger = logging.getLogger(__name__)

@dataclass
class ScrapedPage:
    """Data class to hold scraped page information."""
    url: str
    title: str
    content: str
    links: List[str]
    metadata: Dict
    
class RobotChecker:
    """Check robots.txt compliance."""
    
    def __init__(self, base_url: str, user_agent: str = "*"):
        self.base_url = base_url
        self.user_agent = user_agent
        self.rp = RobotFileParser()
        robots_url = urljoin(base_url, "/robots.txt")
        self.rp.set_url(robots_url)
        try:
            self.rp.read()
            logger.info(f"Loaded robots.txt from {robots_url}")
        except Exception as e:
            logger.warning(f"Could not load robots.txt: {e}")
    
    def can_fetch(self, url: str) -> bool:
        """Check if URL can be fetched according to robots.txt."""
        try:
            return self.rp.can_fetch(self.user_agent, url)
        except Exception:
            return True  # If robots.txt can't be parsed, allow fetching

class WebsiteScraper:
    """Main website scraper class."""
    
    def __init__(self, 
                 base_url: str,
                 max_pages: int = 1000,
                 delay: float = 1.0,
                 max_workers: int = 5,
                 chunk_size: int = 1000,
                 chunk_overlap: int = 200,
                 respect_robots: bool = True,
                 allowed_domains: Optional[List[str]] = None,
                 exclude_patterns: Optional[List[str]] = None):
        """
        Initialize the website scraper.
        
        Args:
            base_url: The starting URL to scrape
            max_pages: Maximum number of pages to scrape
            delay: Delay between requests in seconds
            max_workers: Maximum number of concurrent workers
            chunk_size: Size of text chunks for embeddings
            chunk_overlap: Overlap between chunks
            respect_robots: Whether to respect robots.txt
            allowed_domains: List of allowed domains (None for same domain only)
            exclude_patterns: List of URL patterns to exclude
        """
        self.base_url = base_url.rstrip('/')
        self.domain = urlparse(base_url).netloc
        self.max_pages = max_pages
        self.delay = delay
        self.max_workers = max_workers
        self.chunk_size = chunk_size
        self.chunk_overlap = chunk_overlap
        
        # URL management
        self.visited_urls: Set[str] = set()
        self.failed_urls: Set[str] = set()
        self.to_visit: Set[str] = {self.base_url}
        
        # Domain filtering
        if allowed_domains is None:
            self.allowed_domains = {self.domain}
        else:
            self.allowed_domains = set(allowed_domains)
        
        # URL exclusion patterns
        self.exclude_patterns = exclude_patterns or []
        self.exclude_patterns.extend([
            r'\.pdf$', r'\.jpg$', r'\.jpeg$', r'\.png$', r'\.gif$',
            r'\.css$', r'\.js$', r'\.zip$', r'\.tar$', r'\.gz$',
            r'/api/', r'/admin/', r'/login', r'/signup',
            r'#', r'\?print=', r'/feed/', r'\.xml$'
        ])
        
        # Robots.txt checker
        self.robot_checker = RobotChecker(base_url) if respect_robots else None
        
        # HTTP session with retry strategy
        self.session = requests.Session()
        self.session.headers.update({
            'User-Agent': 'Mozilla/5.0 (compatible; WebScraper/1.0)',
            'Accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8',
            'Accept-Language': 'en-US,en;q=0.5',
            'Accept-Encoding': 'gzip, deflate',
            'Connection': 'keep-alive',
        })
        
        # Initialize ChromaDB and embedding model
        self.setup_chromadb()
        #self.embedding_model = SentenceTransformer('all-MiniLM-L6-v2')
        
    def setup_chromadb(self):
        """Initialize ChromaDB client and collection."""
        self.chroma_client = chromadb.PersistentClient(
            path="./chroma_db",
            settings=Settings(anonymized_telemetry=False)
        )
        
        # Create or get collection
        collection_name = f"website_{self._sanitize_collection_name(self.domain)}"
        try:
            self.collection = self.chroma_client.get_or_create_collection(collection_name,
                metadata={"description": f"Scraped content from {self.base_url}"})

            logger.info(f"Using existing collection: {collection_name}")
        except ValueError:
            self.collection = self.chroma_client.create_collection(
                name=collection_name,
                metadata={"description": f"Scraped content from {self.base_url}"}
            )
            logger.info(f"Created new collection: {collection_name}")
    
    def _sanitize_collection_name(self, name: str) -> str:
        """Sanitize collection name for ChromaDB."""
        return re.sub(r'[^a-zA-Z0-9_-]', '_', name)
    
    def is_valid_url(self, url: str) -> bool:
        """Check if URL should be scraped."""
        parsed = urlparse(url)
        
        # Check domain
        if parsed.netloc not in self.allowed_domains:
            return False
        
        # Check exclusion patterns
        for pattern in self.exclude_patterns:
            if re.search(pattern, url, re.IGNORECASE):
                return False
        
        # Check robots.txt
        if self.robot_checker and not self.robot_checker.can_fetch(url):
            return False
        
        return True
    
    def normalize_url(self, url: str) -> str:
        """Normalize URL by removing fragments and sorting query parameters."""
        parsed = urlparse(url)
        # Remove fragment and normalize
        normalized = urlunparse((
            parsed.scheme, parsed.netloc, parsed.path,
            parsed.params, parsed.query, ''
        ))
        return normalized
    
    def extract_links(self, soup: BeautifulSoup, base_url: str) -> List[str]:
        """Extract all valid links from a page."""
        links = []
        for tag in soup.find_all(['a', 'link'], href=True):
            href = tag['href']
            full_url = urljoin(base_url, href)
            normalized_url = self.normalize_url(full_url)
            
            if self.is_valid_url(normalized_url):
                links.append(normalized_url)
        
        return list(set(links))  # Remove duplicates
    
    def clean_text(self, soup: BeautifulSoup) -> str:
        """Extract and clean text content from HTML."""
        # Remove script and style elements
        for element in soup(["script", "style", "nav", "footer", "header", "aside"]):
            element.decompose()
        
        # Remove comments
        for comment in soup.find_all(string=lambda text: isinstance(text, Comment)):
            comment.extract()
        
        # Get text content
        text = soup.get_text()
        
        # Clean up whitespace
        lines = (line.strip() for line in text.splitlines())
        chunks = (phrase.strip() for line in lines for phrase in line.split("  "))
        text = ' '.join(chunk for chunk in chunks if chunk)
        
        return text
    
    def scrape_page(self, url: str) -> Optional[ScrapedPage]:
        """Scrape a single page."""
        try:
            logger.info(f"Scraping: {url}")
            
            response = self.session.get(url, timeout=30)
            response.raise_for_status()
            
            # Check content type
            content_type = response.headers.get('content-type', '').lower()
            if 'text/html' not in content_type:
                logger.warning(f"Skipping non-HTML content: {url}")
                return None
            
            soup = BeautifulSoup(response.content, 'html.parser')
            
            # Extract page information
            title = soup.title.string.strip() if soup.title else "No Title"
            content = self.clean_text(soup)
            links = self.extract_links(soup, url)
            
            # Create metadata
            metadata = {
                'url': url,
                'title': title,
                'scraped_at': time.time(),
                'content_length': len(content),
                'domain': urlparse(url).netloc,
                'path': urlparse(url).path
            }
            
            return ScrapedPage(
                url=url,
                title=title,
                content=content,
                links=links,
                metadata=metadata
            )
            
        except Exception as e:
            logger.error(f"Failed to scrape {url}: {e}")
            self.failed_urls.add(url)
            return None
    
    def chunk_text(self, text: str) -> List[str]:
        """Split text into overlapping chunks."""
        if len(text) <= self.chunk_size:
            return [text]
        
        chunks = []
        start = 0
        
        while start < len(text):
            end = start + self.chunk_size
            
            # Try to break at sentence boundary
            if end < len(text):
                # Look for sentence endings
                for i in range(end, max(start + self.chunk_size // 2, end - 200), -1):
                    if text[i:i+1] in '.!?':
                        end = i + 1
                        break
            
            chunk = text[start:end].strip()
            if chunk:
                chunks.append(chunk)
            
            start = end - self.chunk_overlap
            if start >= len(text):
                break
        
        return chunks
    
    def store_in_chromadb(self, page: ScrapedPage):
        """Store page content in ChromaDB."""
        chunks = self.chunk_text(page.content)
        
        for i, chunk in enumerate(chunks):
            if not chunk.strip():
                continue
            
            # Create unique ID for this chunk
            chunk_id = hashlib.md5(f"{page.url}_{i}_{chunk[:100]}".encode()).hexdigest()
            
            # Prepare metadata for this chunk
            chunk_metadata = page.metadata.copy()
            chunk_metadata.update({
                'chunk_index': i,
                'total_chunks': len(chunks),
                'chunk_size': len(chunk)
            })
            
            try:
                self.collection.add(
                    ids=[chunk_id],
                    documents=[chunk],
                    metadatas=[chunk_metadata]
                )
            except Exception as e:
                logger.error(f"Failed to store chunk in ChromaDB: {e}")
    
    def crawl_website(self):
        """Main crawling function."""
        logger.info(f"Starting to crawl {self.base_url}")
        logger.info(f"Max pages: {self.max_pages}, Delay: {self.delay}s, Workers: {self.max_workers}")
        
        pages_scraped = 0
        
        while self.to_visit and pages_scraped < self.max_pages:
            # Get next batch of URLs to process
            current_batch = list(self.to_visit)[:self.max_workers]
            self.to_visit -= set(current_batch)
            
            # Process batch with threading
            with ThreadPoolExecutor(max_workers=self.max_workers) as executor:
                future_to_url = {
                    executor.submit(self.scrape_page, url): url 
                    for url in current_batch
                }
                
                for future in as_completed(future_to_url):
                    url = future_to_url[future]
                    self.visited_urls.add(url)
                    
                    try:
                        page = future.result()
                        if page:
                            # Store in ChromaDB
                            self.store_in_chromadb(page)
                            
                            # Add new links to visit
                            new_links = set(page.links) - self.visited_urls - self.failed_urls
                            self.to_visit.update(new_links)
                            
                            pages_scraped += 1
                            logger.info(f"Processed {pages_scraped}/{self.max_pages} pages. Queue: {len(self.to_visit)}")
                            
                            if pages_scraped >= self.max_pages:
                                break
                                
                    except Exception as e:
                        logger.error(f"Error processing {url}: {e}")
                    
                    # Rate limiting
                    time.sleep(self.delay)
        
        logger.info(f"Crawling completed. Pages scraped: {pages_scraped}")
        logger.info(f"Failed URLs: {len(self.failed_urls)}")
        
        # Save statistics
        stats = {
            'total_pages_scraped': pages_scraped,
            'failed_urls': len(self.failed_urls),
            'total_chunks': self.collection.count(),
            'base_url': self.base_url,
            'scraping_completed_at': time.time()
        }
        
        with open('scraping_stats.json', 'w') as f:
            json.dump(stats, f, indent=2)
        
        return stats

def main():
    """Main function with example usage."""
    # Configuration
    BASE_URL = "https://gnco.ir"  # Change this to your target website
    MAX_PAGES = 100  # Adjust based on website size
    
    # Create scraper
    scraper = WebsiteScraper(
        base_url=BASE_URL,
        max_pages=MAX_PAGES,
        delay=1.0,  # Be respectful with delays
        max_workers=3,  # Don't overwhelm the server
        chunk_size=1000,
        chunk_overlap=200,
        respect_robots=True
    )
    
    # Start scraping
    try:
        stats = scraper.crawl_website()
        print("\n=== Scraping Complete ===")
        print(f"Pages scraped: {stats['total_pages_scraped']}")
        print(f"Total chunks stored: {stats['total_chunks']}")
        print(f"ChromaDB collection: {scraper.collection.name}")
        
        # Example query
        print("\n=== Example Query ===")
        results = scraper.collection.query(
            query_texts=["Python functions and classes"],
            n_results=3
        )
        
        for i, (doc, metadata) in enumerate(zip(results['documents'][0], results['metadatas'][0])):
            print(f"\nResult {i+1}:")
            print(f"URL: {metadata['url']}")
            print(f"Title: {metadata['title']}")
            print(f"Content preview: {doc[:200]}...")
            
    except KeyboardInterrupt:
        logger.info("Scraping interrupted by user")
    except Exception as e:
        logger.error(f"Scraping failed: {e}")

if __name__ == "__main__":
    main()