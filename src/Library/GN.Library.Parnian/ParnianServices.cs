using GN.Parnian.Library.EasyHook;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GN.Library.Parnian
{
    public interface IParnianServices
    {
        LocalHook Hook(MethodInfo method, MethodInfo hookMethod, bool activate = true);
        
        
    }
    internal class ParnianServices : IParnianServices
    {
        internal static List<LocalHook> Hooks = new List<LocalHook>();
        public static IParnianServices Instance = new ParnianServices();

        public LocalHook Hook(MethodInfo method, MethodInfo hookMethod, bool activate = true)
        {
            System.Runtime.CompilerServices.RuntimeHelpers.PrepareMethod(method.MethodHandle);
            System.Runtime.CompilerServices.RuntimeHelpers.PrepareMethod(hookMethod.MethodHandle);

            var hook = LocalHook
                .CreateUnmanaged(method.MethodHandle.GetFunctionPointer(), hookMethod.MethodHandle.GetFunctionPointer(), IntPtr.Zero);
            if (activate)
            {
                hook.ThreadACL.SetExclusiveACL(new int[] { -1 });
            }
            Hooks.Add(hook);
            return hook;
        }

        public LocalHook Hook(Action action, Action hookAction)
        {
            return Hook(action.Method, hookAction.Method);
        }
    }
}
