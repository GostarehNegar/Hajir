# Product Scraper

A Python script that scrapes multiple pages of a website and extracts product information.

## Features

- **Multi-page scraping**: Scrapes up to 4 pages (configurable)
- **Flexible extraction**: Extracts product names, prices, descriptions, images, and URLs
- **Multiple output formats**: Saves data to JSON and CSV
- **Configurable**: Pre-configured for popular e-commerce sites
- **Error handling**: Robust error handling and logging
- **Polite scraping**: Respectful delays between requests

## Installation

1. **Clone or download the project files**
   ```bash
   # Files needed:
   # - product_scraper.py (main scraper)
   # - config.py (website configurations)
   # - requirements.txt (dependencies)
   # - example_usage.py (usage examples)
   ```

2. **Install dependencies**
   ```bash
   pip install -r requirements.txt
   ```

## Quick Start

### Basic Usage

```python
from product_scraper import ProductScraper

# Initialize scraper with your target website
scraper = ProductScraper(base_url="https://example.com/products", delay=1.0)

# Scrape 4 pages
products = scraper.scrape_pages(num_pages=4)

# Save results
scraper.save_to_json("products.json")
scraper.save_to_csv("products.csv")

# Print summary
scraper.print_summary()
```

### Using Configuration

```python
from product_scraper import ProductScraper
from config import get_config

# Get configuration for a predefined website
config = get_config(website_name="example")

scraper = ProductScraper(
    base_url=config["base_url"],
    delay=config["delay"]
)

products = scraper.scrape_pages(num_pages=config["pages"])
```

## File Structure

```
product_scraper/
├── product_scraper.py    # Main scraper class
├── config.py             # Website configurations
├── example_usage.py      # Usage examples
├── requirements.txt      # Python dependencies
└── README.md            # This file
```

## Output Format

The scraper extracts products with the following fields:

- `name`: Product name
- `price`: Product price
- `description`: Product description
- `image_url`: Product image URL
- `product_url`: Product page URL
- `scraped_at`: Timestamp of when the product was scraped

### Example JSON Output
```json
[
  {
    "name": "Example Product",
    "price": "$29.99",
    "description": "This is a sample product description",
    "image_url": "https://example.com/images/product1.jpg",
    "product_url": "https://example.com/products/1",
    "scraped_at": "2024-01-15 10:30:45"
  }
]
```

## Pre-configured Websites

The script includes configurations for:

- **Example**: Generic e-commerce site (`example.com`)
- **Amazon**: Amazon product search
- **eBay**: eBay product listings

### Adding Custom Configurations

Edit [`config.py`](config.py) to add new website configurations:

```python
"my_website": {
    "base_url": "https://my-website.com/products",
    "pagination_pattern": "?page={page}",
    "product_container": ".product-item",
    "name_selector": ".product-title",
    "price_selector": ".product-price",
    "description_selector": ".product-description",
    "image_selector": ".product-image img",
    "link_selector": ".product-link a"
}
```

## Command Line Usage

Run the examples:

```bash
# Run all examples
python example_usage.py

# Run the main scraper directly
python product_scraper.py
```

## Customization

### Modifying Selectors

The script uses multiple CSS selectors to find product information. You can modify these in the [`ProductScraper`](product_scraper.py) class:

```python
# Common CSS selectors for product containers
product_selectors = [
    '.product', '.item', '.card', '.product-item',
    '[data-product]', '.product-card', '.grid-item'
]
```

### Adjusting Request Settings

```python
# Increase delay to be more polite
scraper = ProductScraper(base_url=url, delay=2.0)

# Change user agent
scraper.session.headers.update({
    'User-Agent': 'Your Custom User Agent'
})
```

## Error Handling

The script includes comprehensive error handling:

- **Network errors**: Continues with next page if one fails
- **Missing elements**: Skips products with missing data
- **Invalid URLs**: Handles malformed URLs gracefully
- **Timeouts**: Configurable request timeouts

## Legal Considerations

⚠️ **Important**: Always check:
- `robots.txt` of the target website
- Terms of Service
- Rate limiting policies
- Legal requirements in your jurisdiction

## Troubleshooting

### Common Issues

1. **No products found**
   - Check if the website blocks scraping
   - Verify CSS selectors match the website structure
   - Inspect the webpage HTML to find correct selectors

2. **Connection errors**
   - Check internet connection
   - Verify the URL is correct
   - Try increasing the timeout

3. **Missing data**
   - Some fields might be empty if selectors don't match
   - Check the website's HTML structure

### Debug Mode

Enable debug logging to see more details:

```python
import logging
logging.basicConfig(level=logging.DEBUG)
```

## Dependencies

- [`requests`](https://pypi.org/project/requests/) - HTTP requests
- [`beautifulsoup4`](https://pypi.org/project/beautifulsoup4/) - HTML parsing
- [`lxml`](https://pypi.org/project/lxml/) - XML/HTML parser (faster alternative)

## License

This project is for educational purposes. Use responsibly and in compliance with website terms of service.