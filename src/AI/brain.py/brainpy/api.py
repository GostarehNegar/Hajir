from fastapi import FastAPI
import brainpy.models.BrainModels as BrainModels
# app = FastAPI()
# app.mount(path="", app=StaticFiles(directory="static"))


def initialize(app: FastAPI):
    @app.get("/ping")
    async def hello():
        return {"Hello world!"}
    
   

        

   


    @app.post("/conversation")
    async def Insert(c: BrainModels.ConversationModel):
        id = c.Id
        pass
