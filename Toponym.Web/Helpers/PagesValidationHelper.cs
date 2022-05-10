namespace Toponym.Web.Pages;

using System.Diagnostics;
using System.Runtime.CompilerServices;

public static class PagesValidationHelper
{
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
