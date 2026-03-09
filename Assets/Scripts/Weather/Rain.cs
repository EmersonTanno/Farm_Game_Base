using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Rain : MonoBehaviour
{
    private Transform followTarget;
    private ParticleSystem rainPS;

    private Vector3 offset;
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
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("Rain: Player não encontrado.");
            return;
        }

        Debug.Log("Player Found");

        followTarget = player.transform;

        offset = transform.position - followTarget.position;
        initialized = true;

        rainPS.Play();
    }

    void LateUpdate()
    {
        if (!initialized || followTarget == null)
        {
            Debug.Log("Player NOT FOUND");
          return;  
        } 

        transform.position = followTarget.position + offset;
    }
}
