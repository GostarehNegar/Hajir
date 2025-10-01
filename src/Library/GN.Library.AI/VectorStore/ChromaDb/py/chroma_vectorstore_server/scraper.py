import argparse
import requests
import os
from urllib.parse import urlparse
from collections import defaultdict
from bs4 import BeautifulSoup
import json
from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.firefox.service import Service
from selenium.webdriver.firefox.options import Options as FireFoxOptions
from selenium.webdriver.remote.webdriver import WebDriver as RemoteWebDriver
import time
import logging
import options
# options = Options()
# options.add_argument("-headless")  # Ensure GUI is off
# chrome = webdriver.Chrome(options=options)


def cleanUrl(url: str):
    return url.replace("https://", "").replace("/", "-").replace(".", "_").replace("=", "_").replace("&", "_").replace("?", "_")


Folder = options.Documents_FOLDER


class Scraper():
    _browser: RemoteWebDriver = None

    def __init__(self):
        self._browser = None
        self._folder = options.Documents_FOLDER

    def browser(self, type: str = "chrome", refresh: bool = False):
        if self._browser == None or refresh:
            if self._browser != None:
                self._browser.quit()
            if (type == "firefox"):
                pass
            else:
                options = Options()
                options.add_argument("-headless")  # Ensure GUI is off
                self._browser = webdriver.Chrome(options=options)
        return self._browser

    def download(self, url: str, sleep=5):
        _b = self.browser()
        page = _b.get(url)
        time.sleep(sleep)
        result = _b.page_source
        return result

    def get_file_name(self, url: str):

        return os.path.join(self._folder, cleanUrl(url)+".html")

    def get_page_text(self, url: str, sleep=5) -> str:
        html = self.get_page(url=url, sleep=sleep)
        soup = BeautifulSoup(html, "html.parser")
        return soup.text

    def get_page(self, url: str, sleep=5) -> str:
        file_name = self.get_file_name(url)
        result = ""
        if os.path.isfile(file_name):
            logging.info(f"File already exist we will just load it.")
            with open(file_name, "r", encoding='utf-8') as text_file:
                return text_file.read()
        logging.info(f"Downloading Page: {url}")
        result = self.download(url, sleep=sleep)
        logging.info(f"Page Successfully Downloaded : {url}")
        with open(file_name, "w", encoding='utf-8') as f:
            f.write(result)
        return result


scraper = Scraper()
