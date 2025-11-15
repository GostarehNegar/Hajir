#!/usr/bin/env python3
"""
Example usage of the product scraper
"""

import sys
import os

# Add current directory to path to import our modules
sys.path.append(os.path.dirname(os.path.abspath(__file__)))

from product_scraper import ProductScraper
from config import get_config

def example_basic_usage():
    """Basic usage example with a custom URL"""
    print("=== BASIC USAGE EXAMPLE ===")
    
    # Replace with your target website
    target_url = "https://artaelectric.ir/brand/faran/"
    
    scraper = ProductScraper(base_url=target_url, delay=1.0)
    products = scraper.scrape_pages(num_pages=4)
    
    # Save results
    scraper.save_to_json("basic_scraped_products.json")
    scraper.save_to_csv("basic_scraped_products.csv")
    
    scraper.print_summary()

def example_with_config():
    """Example using predefined website configurations"""
    print("\n=== CONFIGURATION-BASED USAGE ===")
    
    # Get configuration for a predefined website
    config = get_config(website_name="example")
    
    scraper = ProductScraper(
        base_url=config["base_url"],
        delay=config["delay"]
    )
    
    products = scraper.scrape_pages(num_pages=config["pages"])
    scraper.save_to_json("config_scraped_products.json")
    scraper.print_summary()

def example_custom_website():
    """Example with a custom website URL"""
    print("\n=== CUSTOM WEBSITE EXAMPLE ===")
    
    # Replace this with any website you want to scrape
    custom_url = "https://artaelectric.ir/brand/faran/"
    
    config = get_config(custom_url=custom_url)
    
    scraper = ProductScraper(
        base_url=config["base_url"],
        delay=config["delay"]
    )
    
    products = scraper.scrape_pages(num_pages=3)
    scraper.save_to_json("custom_scraped_products.json")
    scraper.print_summary()

def example_error_handling():
    """Example demonstrating error handling"""
    print("\n=== ERROR HANDLING EXAMPLE ===")
    
    # This URL might not exist or might block scraping
    problematic_url = "https://nonexistent-website-12345.com/products"
    
    scraper = ProductScraper(base_url=problematic_url, delay=1.0)
    
    try:
        products = scraper.scrape_pages(num_pages=2)
        if products:
            scraper.print_summary()
        else:
            print("No products were scraped (expected for problematic URL)")
    except Exception as e:
        print(f"Scraping failed as expected: {e}")

if __name__ == "__main__":
    print("Product Scraper Usage Examples")
    print("=" * 50)
    
    # Run examples
    example_basic_usage()
    #example_with_config()
    #example_custom_website()
    #example_error_handling()
    
    print("\n" + "=" * 50)
    print("To use the scraper with your own website:")
    print("1. Edit the target_url in example_usage.py")
    print("2. Run: python example_usage.py")
    print("3. Check the generated JSON and CSV files")