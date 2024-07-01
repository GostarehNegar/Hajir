from langchain.chains import RetrievalQA

from langchain.agents import Tool

from brainpy.knowledge import Knowledge
import asyncio
from typing import TYPE_CHECKING, Any, Coroutine, TypeVar
from brainpy import utils

_PRODUCT_LIST="""
فهرست محصولات هزیر صنعت عبارتند از:
1. UPS modular
یو پی اس ماژولار (منبع تغذیه اضطراری) نوعی یو پی اس است که دارای رک بوده که در ان چندین ...

2. یو پی اس مدل Uranus
یو پی اس آنلاین - Switch Mode مدل: Uranus قدرت خروجی: 10KVA - 200KVA ورودی سه فاز / خروجی سه ...

3. یو پی اس مدل Homa

یو پی اس اداری - مجهز به باتری داخلی مدل: Homa قدرت خروجی: 1000VA مجهز به استابلایزر مجهز به باتری ...

4. یو پی اس رکمونت مدل Classic-RMI

یو پی اس اداری رکمونت - مجهز به باتری داخلی مدل: Classic-RMI قدرت خروجی: 1000VA, 1500VA تکنولوژی لاین اینتراکتیو شکل ...

5. یو پی اس مدل Classic-I

یو پی اس اداری - مجهز به باتری داخلی مدل: Classic-I قدرت خروجی: 1000VA, 1500VA تکنولوژی لاین اینتراکتیو شکل موج ...

6. یو پی اس مدل Classic-E

یو پی اس اداری مدل: Classic-E قدرت خروجی: 1.5KVA, 2KVA, 3KVA, 6KVA تکنولوژی لاین اینتراکتیو شکل موج سیسنوسی مجهز ...

7. یو پی اس مدل Genesis 2Plus RM

یو پی اس آنلاین رکمونت مدل: Genesis 2Plus RM قدرت خروجی: 6KVA, 10KVA ورودی تک فاز / خروجی تک فاز ...

8. یو پی اس مدل Genesis-A
یو پی اس مدل Genesis-A
یو پی اس آنلاین - Switch Mode مدل: Genesis-A قدرت خروجی: 10KVA, 20KVA ورودی سه فاز / خروجی تک فاز ...
ادامه مطلب
9.یو پی اس مدل Genesis B Plus
یو پی اس مدل Genesis B Plus
یو پی اس آنلاین - Switch Mode مدل: Genesis B Plus قدرت خروجی: 6KVA, 10KVA ورودی تک فاز / خروجی ...
ادامه مطلب
10. یو پی اس رکمونت مدل Genesis RM
یو پی اس رکمونت مدل Genesis RM
یو پی اس آنلاین رکمونت مدل: Genesis RM قدرت خروجی: 1KVA, 2KVA, 3KVA ورودی تک فاز / خروجی تک فاز ...
ادامه مطلب
11. یو پی اس مدل Genesis
یو پی اس مدل Genesis
یو پی اس آنلاین - Switch Mode مدل: Genesis قدرت خروجی: 1KVA, 2KVA, 3KVA ورودی تک فاز / خروجی تک ...
ادامه مطلب
12. یو پی اس مدل Spider Net
یو پی اس مدل Spider Net
یو پی اس آنلاین صنعتی - Transformer-Based مدل: Spider Net قدرت خروجی: 10KVA - 200KVA ورودی سه فاز / خروجی ...
ادامه مطلب
13. یو پی اس مدل Super Nova
یو پی اس مدل Super Nova
یو پی اس آنلاین صنعتی - Transformer-Based مدل: Super Nova قدرت خروجی: 10KVA, 20KVA, 30KVA ورودی سه فاز / خروجی ...
ادامه مطلب
14. یو پی اس مدل Eternal
یو پی اس مدل Eternal
15. یو پی اس آنلاین صنعتی - Transformer-Based مدل: Eternal قدرت خروجی: 6KVA, 10KVA ورودی تک فاز / خروجی تک فاز ...
"""
async def __func():
    await asyncio.sleep(1)
    return _PRODUCT_LIST
def _func(p:str):
    return utils.run_async_deprecated(__func())

def product_list_tool(llm, kb:Knowledge):
    return  Tool(
                name='Product List',
                func=_func,
                description=(
                    'use this tool when you need a know the product list. '
                    ''
                )
            )
