from models import SppechRecognitionRequest
import speech_recognition as sr 
import base64
import os
import io

r = sr.Recognizer()

def recognize(req: SppechRecognitionRequest):
    
    audio_data = base64.b64decode(req.audio)
    
    audio_file = io.BytesIO(audio_data)
    
    with sr.AudioFile(audio_file) as source:
        audio_listened = r.record(source)
        text = r.recognize_google(audio_listened, language="fa-IR")
        
    os.remove("audio_file_path")
        
    return text