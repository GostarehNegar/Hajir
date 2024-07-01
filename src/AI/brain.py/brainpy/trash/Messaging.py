import json
from nats.aio.msg import Msg
class Message():

    def __init__(
        self,
        msg: Msg):
        try:
            self.Msg = msg
            self.data = json.loads(msg.data.decode())
        finally:
            pass
        return

    def id(self):
        return self.data['id']
    
    def payload(self):
        ret = None
        try:
            ret = json.loads(self.data['Payload'])
            pass
        except:
            pass

        return ret

        return self.data['Payload']
    


