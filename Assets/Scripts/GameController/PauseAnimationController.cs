using UnityEngine;

public class PauseAnimationController : MonoBehaviour
{
    public void OnStampHit()
    {
        PauseController.Instance.SetPauseCanvas(true);
    }

    public void CanSelectOptions()
    {
        PauseController.Instance.SetCanSelect(true);
    }

    public void OnSettingsSwitchToPauseEnd()
    {
        PauseController.Instance.SetCanSelect(true);
        PauseController.Instance.SetSettingCanvas(false);
    }
}