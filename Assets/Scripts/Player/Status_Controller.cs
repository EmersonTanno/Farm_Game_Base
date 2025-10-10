using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Status_Controller : MonoBehaviour
{
    public static Status_Controller Instance;
    int gold = 0;
    [SerializeField] TextMeshProUGUI goldText;

    void Awake()
    {
        Instance = this;
    }
    public void AddGold(int quantity)
    {
        gold += quantity;
        UpdateGoldCanva();
    }

    private void UpdateGoldCanva()
    {
        goldText.text = gold.ToString();
    }
}
