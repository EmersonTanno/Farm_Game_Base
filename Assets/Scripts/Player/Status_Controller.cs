using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Status_Controller : MonoBehaviour
{
    public static Status_Controller Instance;
    int gold = 0;
    int goldT = 0;
    [SerializeField] TextMeshProUGUI goldText;

    void Awake()
    {
        Instance = this;
    }
    public void AddGold(int quantity)
    {
        gold += quantity;
        StartCoroutine(GoldAnimation());
    }

    public void RemoveGold(int quantity)
    {
        gold -= quantity;
        StartCoroutine(GoldAnimation());
    }

    private void UpdateGoldCanva()
    {
        goldText.text = goldT.ToString();
    }

    private IEnumerator GoldAnimation()
    {
        Vector3 originalPosition = goldText.transform.position;
        float time = 0.01f;

        float diff = Mathf.Abs(gold - goldT);

        if (diff >= 500)
            time = 0.001f;

        float shakeIntensity = 2f;
        float shakeSpeed = 30f;

        if (gold > goldT)
        {
            for (int i = goldT; i < gold; i++)
            {
                goldT = i;
                UpdateGoldCanva();

                float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
                float offsetY = Mathf.Cos(Time.time * shakeSpeed * 1.3f) * shakeIntensity;
                goldText.transform.position = originalPosition + new Vector3(offsetX, offsetY, 0f);

                shakeIntensity = Mathf.Lerp(2f, 0f, (float)(i - goldT) / diff);

                yield return new WaitForSeconds(time);
            }
        }
        else
        {
            for (int i = goldT; i > gold; i--)
            {
                goldT = i;
                UpdateGoldCanva();

                float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
                float offsetY = Mathf.Cos(Time.time * shakeSpeed * 1.3f) * shakeIntensity;
                goldText.transform.position = originalPosition + new Vector3(offsetX, offsetY, 0f);

                shakeIntensity = Mathf.Lerp(2f, 0f, (float)(i - goldT) / diff);

                yield return new WaitForSeconds(time);
            }
        }

        goldText.transform.position = originalPosition;
    }

}
