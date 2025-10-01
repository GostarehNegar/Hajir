using GN.Library.Nats.Internals;
using System;

namespace GN.Library.Nats
{
    public class NatsOptions
    {
        public string Url { get; set; }
        public string ConnectionString { get; set; }
        public NatsOptions SetConnectionString(string value)
        {
            this.ConnectionString = value;
            try
            {
                var _params = new ConnectionStringParser(value);
                this.Url = _params.GetValue("url");
            }
            catch (Exception err)
            {
                Console.WriteLine(
                    $"An error occured while trying to parse nats connection string. Err:{err.Message}");
            }
            return this;
        }

        public NatsOptions Validate()
        {
            return this;
        }
        public override string ToString()
        {
            return $"Url:'{this.Url}'";
        }
    }
}