using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Rain : MonoBehaviour
{
    private Transform followTarget;
    private ParticleSystem rainPS;

    private Vector3 offset = new Vector3(0, 17);
    private bool initialized;

    void Awake()
    {
        rainPS = GetComponent<ParticleSystem>();
        rainPS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void OnEnable()
    {
        FindTargetAndCalculateOffset();
    }

    void FindTargetAndCalculateOffset()
    {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");

        if (camera == null)
        {
            Debug.LogWarning("Rain: MainCamera não encontrado.");
            return;
        }

        followTarget = camera.transform;

        transform.position = followTarget.position + offset;
        initialized = true;
    }

    void LateUpdate()
    {
        if (!initialized || followTarget == null) 
        {
            FindTargetAndCalculateOffset();
            return;
        }  
        
        transform.position = followTarget.position + offset;
    }
}
