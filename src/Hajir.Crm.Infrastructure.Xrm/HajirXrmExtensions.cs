﻿using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm
{
    internal static class HajirXrmExtensions
    {
        public static string GetFormattedValue(this Entity entity, string key)
        {
            return entity.FormattedValues.TryGetValue(key, out var res) ? res : null;
        }

        internal static string RemoveArabic(this string str)
        {
            return str == null ? str : str.Replace("ك", "ک").Replace("ي", "ی");

        }
    }
}
