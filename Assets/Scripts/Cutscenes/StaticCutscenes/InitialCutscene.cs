using UnityEngine;

public class InitialCutscene : MonoBehaviour
{
    void Start()
    {
        CutsceneController.Instance.StartCutscene(1);
        Destroy(gameObject);
    }
}