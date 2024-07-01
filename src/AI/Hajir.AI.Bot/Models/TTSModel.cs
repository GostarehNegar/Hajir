using System;
using System.Collections.Generic;
using System.Text;

namespace GostarehNegarBot.Contracts
{
    public class TTSRequestModel
    {
        public string text { get; set; }
    }
    public class TTSResponseModel
    {
        public int status { get; set; }
        public string content { get; set; }
    }
}
