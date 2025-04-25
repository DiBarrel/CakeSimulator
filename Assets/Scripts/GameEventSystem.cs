using System;
using UnityEngine;

public static class GameEventSystem
{
    public static event Action OnScreenResolutionChanged;

    // [TEMPLATE] Other events
    // public static event Action OnA;
    // public static event Action<int> OnB;

    public static void NotifyResolutionChanged()
    {
        OnScreenResolutionChanged?.Invoke();
    }
}
