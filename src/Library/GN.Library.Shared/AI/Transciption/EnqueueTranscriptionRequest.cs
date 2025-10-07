using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.AI.Transciption
{
    public class EnqueueTranscriptionRequest
    {
        public string File {  get; set; }
        public long Length { get; set; }
        public string Id { get; set; }
    }
    public class EnqueueTranscriptionResponse
    {
        public string Id { get; set; }
        public string File { get; set; }
        public bool Failed { get; set; }
    }
    public class TranscriptionStarted
    {
        public string Id { get; set; }
        public DateTime Time { get; set; }
    }
    public class TranscriptionFinished
    {
        public string Id { get; set; }
        public bool Failed { get; set; }
        public string Transcription { get; set; }
        public string Error { get; set; }
    }
}
