using UnityEngine;

public class PauseAnimationController : MonoBehaviour
{
    public void OnStampHit()
    {
        PauseController.Instance.SetPauseCanvas(true);
    }

    public void OnStampLeft()
    {
        PauseController.Instance.SetCanSelect(true);
    }
}