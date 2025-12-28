using System.Collections;
using UnityEngine;

public class ThoughtBubbleController : MonoBehaviour
{
    [SerializeField] GameObject reactionBalloon;
    private Animator balloonAnimator;

    void Awake()
    {
        balloonAnimator = reactionBalloon.GetComponent<Animator>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            ShowBalloon();
        }
    }


    public void ShowBalloon()
    {
        reactionBalloon.SetActive(true);
        balloonAnimator.SetBool("Active", true);
        StartCoroutine(DeactivateBalloon());
    }

    private IEnumerator DeactivateBalloon()
    {
        yield return new WaitForSeconds(2f);
        balloonAnimator.SetBool("Active", false);
        yield return new WaitForSeconds(1f);
        reactionBalloon.SetActive(false);
    }

    //ajustar para ele apenas poder ativar um balão por vez
}
