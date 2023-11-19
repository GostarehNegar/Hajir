using GN.Parnian.Library.EasyHook;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GN.Library.Parnian
{
    public static partial class ParnianExtensions
    {

        public static IParnianServices HookingServices = ParnianServices.Instance;
        public static LocalHook Hook(this IParnianServices parnian, Action action, Action hookAction, bool activate = true)=>
                parnian.Hook(action.Method, hookAction.Method, activate);
        
        public static LocalHook Hook<T>(IParnianServices parnian, Action<T> action, Action<T> hookAction, bool activate = true)
        {
            return parnian.Hook(action.Method, hookAction.Method, activate);
        }
        public static LocalHook Hook<T>(this IParnianServices parnian, Func<T> action, Func<T> hookAction, bool activate = true)
        {
            return parnian.Hook(action.Method, hookAction.Method, activate);
        }
    }
}
