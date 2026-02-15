using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance;
    [SerializeField] Image backGround;
    [SerializeField] private float fadeSpeed = 1f;
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Time_Controll.Instance.PauseTimer();
        StartCoroutine(FadeBackground());
    }


    public void KillSession()
    {
        Destroy(gameObject);
    }


    private void SetBackgroundAlpha(float alpha)
    {
        Color c = backGround.color;
        c.a = Mathf.Clamp01(alpha);
        backGround.color = c;
    }

    private IEnumerator FadeBackground()
    {
        float alpha = 1f;

        while (alpha > 0f)
        {
            alpha -= fadeSpeed * Time.deltaTime;
            SetBackgroundAlpha(alpha);
            yield return null;
        }

        SetBackgroundAlpha(0f);
        backGround.gameObject.SetActive(false);
        Time_Controll.Instance.UnpauseTimer();
    }


}