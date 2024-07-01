from langchain.chains import RetrievalQA

from langchain.agents import Tool
from langchain.tools import BaseTool
from brainpy.knowledge import Knowledge
PRICE_LIST="""
لیست قیمت محصولات:
فارسی ساز مایکروسافت پراجکت 32000000 ریال
فارسی ساز مایکروسافت شیرپوینت 72000000 ریال
فارسی ساز مایکروسافت داینامیکس 80000000 ریال
فارسی ساز مایکروسافت پراجکت سرور 85000000 ریال
فارسی ساز پریماورا 42000000 ریال
فارسی ساز آفیس: رایگان



توجه داشته باشید که:
فارسی ساز سی.آر.ام همان فارسی ساز داینامیکس است.
پرنیان همان فارسی ساز مایکروسافت پراجکت است که با نام تجاری پرنیان شناخته میشود.
پرنیان به صورت تحت شبکه هم ارائه میشود. در این حالت قیمت آن بر اساس تعداد کاربر متفاوت است:
1 کاربره 32000000 ریال
سه کاربره 62000000 ریال
5 کاربره 75000000 ریال
15 کاربره 165000000 ریال
نامحدود 250000000 ریال

این قیمتها فقط برای پرنیان تحت شبکه است. شیرپوینت، پراجکت سرور و فارسی ساز داینامیکس محدودتی تعداد کابر ندارد و قیمتش همان است که در لیست قیمت آمده است.
نینتکس به صورت رایگان و به همراه فارسی ساز شیرپوینت ارائه میشود.


"""


class PriceListTool(BaseTool):
        name='Price List'
        description="use this tool when you need a know about product price. "
        def _run(self, input:str):
             return PRICE_LIST
        def _arun(self, input: str):
            raise NotImplementedError("This tool does not support async")
        
             




def pricelist_tool(llm, kb: Knowledge):
    return PriceListTool()
