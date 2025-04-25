
using UnityEngine;

public class GameEventTracker : MonoBehaviour
{
    private int lastWidth, lastHeight;

    private void Start()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;
    }

    private void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;

            GameEventSystem.NotifyResolutionChanged();
        }
    }
}
