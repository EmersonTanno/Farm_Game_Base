using UnityEngine;

public class InitialCutscene : MonoBehaviour
{
    [SerializeField] int cutsceneId = 2;
    void Start()
    {
        CutsceneController.Instance.StartCutscene(cutsceneId);
        Destroy(gameObject);
    }
}