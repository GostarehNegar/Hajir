#!/usr/bin/env python3
"""
Configuration for the product scraper
"""

# Example configurations for different websites
WEBSITE_CONFIGS = {
    "example": {
        "base_url": "https://artaelectric.ir/brand/faran/",
        "pagination_pattern": "?page={page}",
        "product_container": ".product",
        "name_selector": ".product-name",
        "price_selector": ".price",
        "description_selector": ".description",
        "image_selector": ".product-image img",
        "link_selector": ".product-link"
    },
    "amazon": {
        "base_url": "https://www.amazon.com/s",
        "pagination_pattern": "&page={page}",
        "product_container": "[data-component-type='s-search-result']",
        "name_selector": "h2 a span",
        "price_selector": ".a-price-whole",
        "description_selector": ".a-text-normal",
        "image_selector": ".s-image",
        "link_selector": "h2 a"
    },
    "ebay": {
        "base_url": "https://www.ebay.com/sch/i.html",
        "pagination_pattern": "&_pgn={page}",
        "product_container": ".s-item",
        "name_selector": ".s-item__title",
        "price_selector": ".s-item__price",
        "description_selector": ".s-item__subtitle",
        "image_selector": ".s-item__image-img",
        "link_selector": ".s-item__link"
    }
}

# Default configuration
DEFAULT_CONFIG = {
    "base_url": "",
    "pagination_pattern": "?page={page}",
    "product_container": ".product, .item, .card, .product-item, [data-product], .product-card, .grid-item",
    "name_selector": ".product-name, .title, h1, h2, h3, [itemprop='name']",
    "price_selector": ".price, .product-price, [itemprop='price'], .amount",
    "description_selector": ".description, [itemprop='description'], .product-desc",
    "image_selector": "img[src], [itemprop='image'], .product-image img",
    "link_selector": "a[href], .product-link",
    "delay": 1.0,
    "pages": 4
}

def get_config(website_name=None, custom_url=None):
    """
    Get configuration for a specific website
    
    Args:
        website_name: Name of predefined website config
        custom_url: Custom URL to use
    
    Returns:
        Dictionary with configuration
    """
    config = DEFAULT_CONFIG.copy()
    
    if website_name and website_name in WEBSITE_CONFIGS:
        config.update(WEBSITE_CONFIGS[website_name])
    
    if custom_url:
        config["base_url"] = custom_url
    
    return config