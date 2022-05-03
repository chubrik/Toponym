using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Toponym.Web
{
    public static class ValidationHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Check(bool condition, string? message = null)
        {
            Debug.Assert(condition);
            if (!condition) throw new InvalidOperationException(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T NotNull<T>(T? value) where T : class
        {
            Debug.Assert(value != null);
            return value ?? throw new ArgumentNullException(nameof(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T NotNull<T>(T? value) where T : struct
        {
            Debug.Assert(value != null);
            return value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
