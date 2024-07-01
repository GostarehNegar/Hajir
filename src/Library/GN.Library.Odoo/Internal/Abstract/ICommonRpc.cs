﻿using System;
using CookComputing.XmlRpc;

namespace GN.Library.Odoo.Internal.Abstract
{
    [XmlRpcUrl("common")]
    public interface ICommonRpc : IXmlRpcProxy
    {
        [XmlRpcMethod("login")]
        int login(String dbName, string dbUser, string dbPwd );


        [XmlRpcMethod("authenticate")]
        int authenticate(string dbName, string dbUser, string dbPwd, params object[] user_agent_env);
    }
}
