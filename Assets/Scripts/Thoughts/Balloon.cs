using System;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public static event Action<ThoughtBubbleController> OnBalloonUp;
    public static event Action<ThoughtBubbleController> OnBalloonDown;

    [SerializeField] private ThoughtBubbleController owner;

    public void BalloonUp()
    {
        OnBalloonUp?.Invoke(owner);
    }

    public void BalloonDown()
    {
        OnBalloonDown?.Invoke(owner);
    }

}
