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

    public void OnSettingsSwitchEnd()
    {
        PauseController.Instance.SetCanSelect(true);
    }

    public void OnSettingsSwitchToPauseEnd()
    {
        PauseController.Instance.SetCanSelect(true);
        PauseController.Instance.SetSettingCanvas(false);
    }
}