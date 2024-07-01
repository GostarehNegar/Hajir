from pydantic import BaseModel
from brainpy.library.bus import BusSettings
import pydantic

class ChromaSettings(BaseModel):
    host: str = "localhost"
    port: str = "8009"
    persist_directory: str = "db"

    def is_http_client(self)->bool:
        return self.host and not self.host.isspace() and self.port and not self.port.isspace()


class Config(BaseModel):
    memory: str = "ConversationBufferMemory"
    dataPath: str = "../db"
    node_name: str = "kk"
    chroma: ChromaSettings = ChromaSettings()
    bus:BusSettings = BusSettings()

    def save(self):
        with open('config.json', 'w') as f:
            # f.write(json.dumps(self,indent=2))
            # print(json.dumps(self,indent=2))
            f.write(self.json(indent=2))


def _read_config() -> Config:
    result: Config = None
    try:
        result = pydantic.parse_file_as(Config, path="config.json")
        pass
    except:
        pass
    if result == None:
        result = Config()
    try:
        result.save()
    except:
        pass

    result.json()
    return result


config = _read_config()
