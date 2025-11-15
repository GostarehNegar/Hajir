#!/usr/bin/env python3
"""
Product Scraper - Scrapes multiple pages of a website and extracts product information
"""

import requests
from bs4 import BeautifulSoup
import json
import time
import csv
from urllib.parse import urljoin, urlparse
import logging
from typing import List, Dict, Optional

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

class ProductScraper:
    def __init__(self, base_url: str, delay: float = 1.0):
        """
        Initialize the scraper with base URL and request delay
        
        Args:
            base_url: The base URL of the website to scrape
            delay: Delay between requests in seconds (to be polite)
        """
        self.base_url = base_url
        self.delay = delay
        self.session = requests.Session()
        self.session.headers.update({
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36'
        })
        self.products = []
    
    def fetch_page(self, url: str) -> Optional[BeautifulSoup]:
        """
        Fetch a webpage and return BeautifulSoup object
        
        Args:
            url: URL to fetch
            
        Returns:
            BeautifulSoup object or None if request fails
        """
        try:
            logger.info(f"Fetching: {url}")
            response = self.session.get(url, timeout=10)
            response.raise_for_status()
            
            # Add delay to be polite
            time.sleep(self.delay)
            
            return BeautifulSoup(response.content, 'html.parser')
        except requests.RequestException as e:
            logger.error(f"Error fetching {url}: {e}")
            return None
    
    def extract_products_from_page(self, soup: BeautifulSoup, page_url: str) -> List[Dict]:
        """
        Extract product information from a page
        
        Args:
            soup: BeautifulSoup object of the page
            page_url: URL of the current page
            
        Returns:
            List of product dictionaries
        """
        products = []
        
        # Common CSS selectors for product containers
        product_selectors = [
            '.product', '.item', '.card', '.product-item',
            '[data-product]', '.product-card', '.grid-item'
        ]
        
        product_elements = []
        for selector in product_selectors:
            elements = soup.select(selector)
            if elements:
                product_elements = elements
                logger.info(f"Found {len(elements)} products using selector: {selector}")
                break
        
        if not product_elements:
            logger.warning("No product elements found on page")
            return products
        
        for element in product_elements:
            try:
                product = self.extract_product_details(element, page_url)
                if product:
                    products.append(product)
            except Exception as e:
                logger.error(f"Error extracting product: {e}")
                continue
        
        return products
    
    def extract_product_details(self, element, page_url: str) -> Optional[Dict]:
        """
        Extract individual product details from a product element
        
        Args:
            element: BeautifulSoup element containing product info
            page_url: URL of the current page
            
        Returns:
            Dictionary with product details or None if extraction fails
        """
        try:
            product = {}
            
            # Extract name
            name_selectors = ['.product-name', '.title', 'h1', 'h2', 'h3', '[itemprop="name"]']
            product['name'] = self._extract_text(element, name_selectors)
            
            # Extract price
            price_selectors = ['.price', '.product-price', '[itemprop="price"]', '.amount']
            product['price'] = self._extract_text(element, price_selectors)
            
            # Extract description
            desc_selectors = ['.description', '[itemprop="description"]', '.product-desc']
            product['description'] = self._extract_text(element, desc_selectors)
            
            # Extract image URL
            img_selectors = ['img[src]', '[itemprop="image"]', '.product-image img']
            product['image_url'] = self._extract_attribute(element, img_selectors, 'src')
            if product['image_url'] and not product['image_url'].startswith(('http:', 'https:')):
                product['image_url'] = urljoin(page_url, product['image_url'])
            
            # Extract product URL
            link_selectors = ['a[href]', '.product-link']
            product['product_url'] = self._extract_attribute(element, link_selectors, 'href')
            if product['product_url'] and not product['product_url'].startswith(('http:', 'https:')):
                product['product_url'] = urljoin(page_url, product['product_url'])
            
            # Add timestamp
            product['scraped_at'] = time.strftime('%Y-%m-%d %H:%M:%S')
            
            # Only include products that have at least a name or price
            if product['name'] or product['price']:
                return product
            
        except Exception as e:
            logger.error(f"Error in product detail extraction: {e}")
        
        return None
    
    def _extract_text(self, element, selectors: List[str]) -> str:
        """Extract text using multiple selectors"""
        for selector in selectors:
            found = element.select_one(selector)
            if found and found.get_text(strip=True):
                return found.get_text(strip=True)
        return ""
    
    def _extract_attribute(self, element, selectors: List[str], attr: str) -> str:
        """Extract attribute using multiple selectors"""
        for selector in selectors:
            found = element.select_one(selector)
            if found and found.get(attr):
                return found.get(attr)
        return ""
    
    def scrape_pages(self, num_pages: int = 4) -> List[Dict]:
        """
        Scrape multiple pages from the website
        
        Args:
            num_pages: Number of pages to scrape
            
        Returns:
            List of all scraped products
        """
        self.products = []
        
        for page_num in range(1, num_pages + 1):
            page_url = self._construct_page_url(page_num)
            if not page_url:
                logger.error(f"Could not construct URL for page {page_num}")
                continue
            
            soup = self.fetch_page(page_url)
            if not soup:
                logger.error(f"Failed to fetch page {page_num}")
                continue
            
            page_products = self.extract_products_from_page(soup, page_url)
            self.products.extend(page_products)
            logger.info(f"Page {page_num}: Found {len(page_products)} products")
        
        logger.info(f"Total products scraped: {len(self.products)}")
        return self.products
    
    def _construct_page_url(self, page_num: int) -> str:
        """
        Construct URL for a specific page number
        
        Args:
            page_num: Page number to construct URL for
            
        Returns:
            Constructed URL string
        """
        # Common pagination patterns
        pagination_patterns = [
            f"{self.base_url}?page={page_num}",
            f"{self.base_url}&page={page_num}",
            f"{self.base_url}/page/{page_num}",
            f"{self.base_url}/products?page={page_num}",
        ]
        
        # Try the first pattern that results in a valid URL
        for pattern in pagination_patterns:
            try:
                parsed = urlparse(pattern)
                if parsed.scheme and parsed.netloc:
                    return pattern
            except Exception:
                continue
        
        # If no pattern works, return the base URL for first page only
        if page_num == 1:
            return self.base_url
        
        return ""
    
    def save_to_json(self, filename: str = "products.json"):
        """Save scraped products to JSON file"""
        with open(filename, 'w', encoding='utf-8') as f:
            json.dump(self.products, f, indent=2, ensure_ascii=False)
        logger.info(f"Products saved to {filename}")
    
    def save_to_csv(self, filename: str = "products.csv"):
        """Save scraped products to CSV file"""
        if not self.products:
            logger.warning("No products to save")
            return
        
        fieldnames = set()
        for product in self.products:
            fieldnames.update(product.keys())
        
        with open(filename, 'w', newline='', encoding='utf-8') as f:
            writer = csv.DictWriter(f, fieldnames=list(fieldnames))
            writer.writeheader()
            writer.writerows(self.products)
        logger.info(f"Products saved to {filename}")
    
    def print_summary(self):
        """Print summary of scraped products"""
        print(f"\n=== SCRAPING SUMMARY ===")
        print(f"Total products: {len(self.products)}")
        if self.products:
            print(f"Sample product fields: {list(self.products[0].keys())}")
            for i, product in enumerate(self.products[:3], 1):
                print(f"\nSample product {i}:")
                for key, value in product.items():
                    if value:
                        print(f"  {key}: {value[:100]}{'...' if len(str(value)) > 100 else ''}")

def main():
    """
    Main function to demonstrate the scraper
    Replace the example URL with your target website
    """
    # Example URL - Replace this with the actual website you want to scrape
    target_url = "https://example.com/products"
    
    # Initialize scraper
    scraper = ProductScraper(base_url=target_url, delay=1.0)
    
    # Scrape 4 pages
    products = scraper.scrape_pages(num_pages=4)
    
    # Save results
    scraper.save_to_json("scraped_products.json")
    scraper.save_to_csv("scraped_products.csv")
    
    # Print summary
    scraper.print_summary()

if __name__ == "__main__":
    main()