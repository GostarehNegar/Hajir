from pydantic import BaseModel


class AddKbDocumetRequest(BaseModel):
    id: str
    source: str
    category: str = 'kb'
    content: str | None = None

class SppechRecognitionRequest(BaseModel):
    audio: str
    type: str
