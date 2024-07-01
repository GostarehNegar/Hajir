import pydantic
from . import BaseModel

_sys_prefix: str = "sys."
_domain_prefix: str = "dom.%DOM%."
_pub_prefix: str = "pub."
_mail_box = "box.%BOX%."

class constants:
    class subjects:
        foo: str = "foo"
        make_reply: str = "makereply"

        class mailbox:
            query_service = _mail_box+"query-service"

        class system:
            heartbeat: str = _sys_prefix+"hearbeat"

        class domain:
            make_reply: str = _domain_prefix + "make-reply"

            def get_app_subject(sub: str, app: str):
                sub.replace("%APP%", app)

    class AgentNames:
        Conversational = "conversational"
