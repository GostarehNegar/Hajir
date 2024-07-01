using System.Collections;

namespace GN.Library.Odoo.Internal.Concrete
{
    public class RpcFilterValue : ArrayList
    {
        public RpcFilterValue AddValue(object value)
        {
            Add(value);
            return this;
        }
    }
}