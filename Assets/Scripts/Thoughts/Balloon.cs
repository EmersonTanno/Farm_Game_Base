using System;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public static event Action OnBalloonUp;
    public static event Action OnBalloonDown;

    public void BalloonUp()
    {
        OnBalloonUp?.Invoke();
    }

    public void BalloonDown()
    {
        OnBalloonDown?.Invoke();
    }
}
