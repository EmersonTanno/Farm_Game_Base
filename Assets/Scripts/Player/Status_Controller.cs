using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Status_Controller : MonoBehaviour
{
    public static Status_Controller Instance;

    #region Variables

    //Gold
    public int gold = 0;
    private int goldT;
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] GameObject playerUiGroup;
 

    //Lucky
    private int lucky = 0;
    private int dayLucky = 0;

    //Energy
    private int maxEnergy = 200;
    public int energy;
    public bool isFainted = false;
    private bool faintInProgress;
    private bool firstEnergyAdd = false;
    private bool secondEnergyAdd = false;
    private bool thirdAdd = false;

    public static event Action OnFaint;

    #endregion

    #region Core
    void Awake()
    {
        Instance = this;
        goldT = gold;
        energy = maxEnergy;
        UpdateGoldCanva();
    }

    void OnEnable()
    {
        Calendar_Controller.OnDayChange += GetLuckyForDay;
        Calendar_Controller.OnDayChange += ResetEnergy;
    }

    void OnDisable()
    {
        Calendar_Controller.OnDayChange -= GetLuckyForDay;
        Calendar_Controller.OnDayChange -= ResetEnergy;
    }
    #endregion

    #region Gold
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
            for (int i = goldT; i <= gold; i++)
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
            for (int i = goldT; i >= gold; i--)
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
    #endregion

    #region Lucky
    public int GetLucky()
    {
        return lucky;
    }

    private void GetLuckyForDay()
    {
        lucky -= dayLucky;
        if (lucky < 0)
        {
            lucky = 0;
        }
        dayLucky = UnityEngine.Random.Range(1, 11);
        lucky += dayLucky;
    }
    #endregion

    #region Energy
    public bool UseEnergy(int usedEnergy)
    {
        if (isFainted)
        {
            return false;
        }

        energy -= usedEnergy;

        CheckEnergyWarning();
        CheckEnergyLevel();
        return true;
    }

    private void CheckEnergyWarning()
    {
        if (energy <= maxEnergy / 2 && !firstEnergyAdd)
        {
            firstEnergyAdd = true;
            Player_Controller.Instance.ShowReaction(ThoughtEmoteEnum.Sweat);
            return;
        }

        if (energy <= maxEnergy / 4 && !secondEnergyAdd)
        {
            secondEnergyAdd = true;
            Player_Controller.Instance.ShowReaction(ThoughtEmoteEnum.Sweat);
            return;
        }

        if (energy <= 10 && !thirdAdd)
        {
            thirdAdd = true;
            Player_Controller.Instance.ShowReaction(ThoughtEmoteEnum.Sweat);
        }
    }

    public void CheckEnergyLevel()
    {
        if (energy > 0) return;
        if (faintInProgress) return;

        faintInProgress = true;
        isFainted = true;
        
        Player_Controller.Instance.ShowReaction(ThoughtEmoteEnum.Sleep);

        StartCoroutine(FaintRoutine());
    }

    private IEnumerator FaintRoutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        WarpController.OnWarpEnd += OnWarpFinished;
        TpToHouse();
    }

    private void OnWarpFinished()
    {
        WarpController.OnWarpEnd -= OnWarpFinished;

        OnFaint?.Invoke();

        faintInProgress = false;
    }

    private void TpToHouse()
    {
        WarpTile warp = new WarpTile
        {
            scene = "House",
            x = 5,
            y = 1,
            transitionType = TransitionType.Instant
        };

        WarpController.Instance.ExecuteWarp(warp);
    }

    private void ResetEnergy()
    {
        energy = maxEnergy;
        firstEnergyAdd = false;
        secondEnergyAdd = false;
        thirdAdd = false;
        isFainted = false;
    }
    #endregion

    #region Ui
    public void SetControllPlayerUiGroup(bool setActive)
    {
        playerUiGroup.SetActive(setActive);
    }
    #endregion

    #region Save / Load
    public void Save(ref PlayerSaveData data)
    {
        data.playerData.gold = gold;
    }

    public void Load(PlayerSaveData data)
    {
        gold = data.playerData.gold;
        goldText.text = gold.ToString();
    }
    #endregion
}
