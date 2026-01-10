using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfigurations : MonoBehaviour
{
    public static GameConfigurations Instance;

    public LanguageEnum gameLanguage = LanguageEnum.Potugues;
    void Awake()
    {
        if(Instance && Instance != this)
        {
            Destroy(this);
        }

        Instance = this;
    }
}
