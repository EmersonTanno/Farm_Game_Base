using UnityEngine;

public class ThoughtBubbleController : MonoBehaviour
{
    [SerializeField] GameObject reactionBalloon;
    [SerializeField] GameObject emoteObject;
    private Animator balloonAnimator;
    private Animator emoteAnimator;
    private bool canShowBalloon = true;
    private ThoughtEmoteEnum emoteType;

    void Awake()
    {
        balloonAnimator = reactionBalloon.GetComponent<Animator>();
        emoteAnimator = emoteObject.GetComponent<Animator>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            ShowBalloon(ThoughtEmoteEnum.Sweat);
        }
    }

    void OnEnable()
    {
        Balloon.OnBalloonUp += CanShowEmote;
        Balloon.OnBalloonDown += DeactivateReaction;
        Emote.OnEmoteDown += DeactivateBalloon;
    }

    void OnDisable()
    {
        Balloon.OnBalloonUp -= CanShowEmote;
        Balloon.OnBalloonDown -= DeactivateReaction;
        Emote.OnEmoteDown -= DeactivateBalloon;
    }



    public void ShowBalloon(ThoughtEmoteEnum emote)
    {
        if(!canShowBalloon) return;

        canShowBalloon = false;
        emoteType = emote;
        reactionBalloon.SetActive(true);
        balloonAnimator.SetBool("Active", true);
    }

    private void DeactivateBalloon(ThoughtBubbleController owner)
    {
        if (owner != this) return;

        emoteObject.SetActive(false);
        balloonAnimator.SetBool("Active", false);
    }

    private void CanShowEmote(ThoughtBubbleController owner)
    {
        if (owner != this) return;

        emoteObject.SetActive(true);
        emoteAnimator.SetTrigger(ToTrigger(emoteType));
    }

    private void DeactivateReaction(ThoughtBubbleController owner)
    {
        if (owner != this) return;

        reactionBalloon.SetActive(false);
        emoteType = ThoughtEmoteEnum.None;
        canShowBalloon = true;
    }

    public static string ToTrigger(ThoughtEmoteEnum emote)
    {
        return emote.ToString();
    }



    //Posteriormente aplicar para npcs
}
