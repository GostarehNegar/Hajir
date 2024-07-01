using System;
using System.Collections.Generic;
using System.Text;

namespace GostarehNegarBot.Lib
{
    public class LibConstants
    {

        public class Subjects
        {
            public static string _SYS_PREFIX = "sys.";
            public static string _APP_PPEFIX = "app.%APP%.";
            public static string _PUB_PPEFIX = "pub.";
            public class System
            {
                public static string HeartBeat = _SYS_PREFIX + "heartbeat";
            }
            public class App
            {
                public static string Make_Reply = _APP_PPEFIX + "make-reply";
            }
        }
    }
}
