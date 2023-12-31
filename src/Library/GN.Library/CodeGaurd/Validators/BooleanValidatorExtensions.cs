using GN.CodeGuard.Internals;

namespace GN.CodeGuard
{
    public static class BooleanValidatorExtensions
    {
        public static ArgBase<bool> IsTrue(this ArgBase<bool> arg)
        {
            if (!arg.Value)
            {
                arg.ThrowNotEqual(true);
            }

            return arg;
        }

        public static ArgBase<bool> IsFalse(this ArgBase<bool> arg)
        {
            if (arg.Value)
            {
                arg.ThrowNotEqual(false);
            }

            return arg;
        }
    }
}