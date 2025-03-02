using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Common
{
    public class KeyValueItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Attachment { get; set; }
        public int Category { get; set; }
    }
    public interface IKeyValueStore
    {
        Task<KeyValueItem> Get(string key, int? category = null);
        Task<KeyValueItem> Store(string key, string value, string attachment = null, int category = 0);
    }
}
