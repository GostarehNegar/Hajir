using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GostarehNegarBot.Internals
{
    public static class Locator
    {
        static IServiceProvider serviceProvider;
        public static void Initailize(IServiceProvider sp)
        {
            serviceProvider = sp;
        }

        public static IHost Init(this IHost host)
        {
            serviceProvider = host.Services;

            return host;
        }


    }
}
