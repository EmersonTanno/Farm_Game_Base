using System;
using UnityEngine;

public class Emote : MonoBehaviour
{
    public static event Action<ThoughtBubbleController> OnEmoteUp;
    public static event Action<ThoughtBubbleController> OnEmoteDown;

    [SerializeField] private ThoughtBubbleController owner;

    public void EmoteUp()
    {
        OnEmoteUp?.Invoke(owner);
    }

    public void EmoteDown()
    {
        OnEmoteDown?.Invoke(owner);
    }

}
