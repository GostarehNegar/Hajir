from app import app
from scraper import scraper
from models import AddKbDocumetRequest
from models import SppechRecognitionRequest
import logging
import speech
api = app.api

@app.api.get("/api/scrape/getpage")
async def get_page(url: str = "", type: str = ""):

    result = ""
    if type == 'text':
        result = scraper.get_page_text(url)
    else:
        result = scraper.get_page(url)

    return result
@app.api.get("/api/ping")
async def ping():

    return "pong"
    pass
@app.api.post("/api/kb/add")
async def add_document(doc:AddKbDocumetRequest):
    logging.info(f"Adding {doc.source}")
    return "ok"




@app.api.get("/api/scrape/pagetext")
async def get_page(url: str = ""):

    result = scraper.get_page_text(url)

    return result

        