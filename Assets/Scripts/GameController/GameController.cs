using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    public static GameController Instance;
    [SerializeField] private GameObject canva;
    [SerializeField] private GameObject npcs;
    private bool canSave = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(canva);
        DontDestroyOnLoad(npcs);
    }

    void OnEnable()
    {
        Calendar_Controller.OnDayChange += SaveGameData;
        Sell_Controller.OnSellFinish += CanSave;
    }

    void OnDisable()
    {
        Calendar_Controller.OnDayChange -= SaveGameData;
        Sell_Controller.OnSellFinish -= CanSave;
    }

    private void CanSave()
    {
        canSave = true;
    }

    private void SaveGameData()
    {
        StartCoroutine(SaveData());
    }

    private IEnumerator SaveData()
    {
        while(!canSave)
        {
            yield return null;
        }
        
        SaveSystem.Save();
        canSave = false;
    }
}
