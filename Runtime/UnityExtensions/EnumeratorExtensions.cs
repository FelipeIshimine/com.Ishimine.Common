using System;
using System.Collections;
using System.Collections.Generic;

public static class EnumeratorExtensions
{
    public static ExtendedCoroutine<T> AsExtended<T>(this IEnumerator<T> enumerator, Action callback = null) => new(enumerator, callback);
    public static ExtendedCoroutine AsExtended(this IEnumerator enumerator, Action callback = null) => new(enumerator, callback);
}
