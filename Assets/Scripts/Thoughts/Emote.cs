using System;
using UnityEngine;

public class Emote : MonoBehaviour
{
    public static event Action OnEmoteUp;
    public static event Action OnEmoteDown;

    public void EmoteUp()
    {
        OnEmoteUp?.Invoke();
    }

    public void EmoteDown()
    {
        OnEmoteDown?.Invoke();
    }
}
