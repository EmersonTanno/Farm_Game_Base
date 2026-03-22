using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;

public class DebtController : MonoBehaviour
{
    public static DebtController Instance;
    private List<DebtData> actualDebtList = new List<DebtData>();
    private List<DebtData> historyDebtList = new List<DebtData>();


    private bool debtListActive = false;
    [SerializeField] private GameObject debtListGroup;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private List<GameObject> debtCards = new List<GameObject>();
    [SerializeField] private GameObject debtCardArea;
    [SerializeField] private GameObject debtCardPrefab;
    [SerializeField] private TextMeshProUGUI debtCardCanvaTitle;

    public static event Action<DebtData> OnDebtCreation;

    void Awake()
    {
        Instance = this;
    }

    #region Actions
    void OnEnable()
    {
        Calendar_Controller.OnDayChange += PassDay;
        GameLanguageManager.OnLanguageChange += SetDebtCards;
    }

    void OnDisable()
    {
        Calendar_Controller.OnDayChange -= PassDay;
        GameLanguageManager.OnLanguageChange -= SetDebtCards;
    }
    #endregion

    private void Update() {
        if(Input.GetKeyDown(KeyCode.L))
        {
            SetDebtList();
        }
    }

    #region Create Debt
    public bool CreateNewDebt(DebtTypeEnum type, int extraPercentageToPay, int quantityMarksTaken, int daysQuantityToPay, int interestPercentage, int maxDaysOver, int creditorNpcId = -1)
    {   
        DebtData newDebt = new DebtData
        {
            id =  $"DEBT_{type}_{Guid.NewGuid():N}",
            debtType = type,
            creditorNpcId = creditorNpcId,
            extraPercentageToPay = extraPercentageToPay,

            quantityMarksTaken = quantityMarksTaken,
            debtMarksToPay = quantityMarksTaken + Mathf.RoundToInt(quantityMarksTaken * (extraPercentageToPay/100f)),

            startDay = Calendar_Controller.Instance.day,
            startMonth = Calendar_Controller.Instance.month,
            startYear = Calendar_Controller.Instance.year,

            daysQuantityToPay = daysQuantityToPay,

            interestPercentage = interestPercentage,
            
            maxDaysOver = maxDaysOver
        };

        if(!CheckIfCanCreateDebt(newDebt))
        {
            Debug.LogWarning($"Cannot Create Debt: Debt of Type {newDebt.debtType} / NPC {newDebt.creditorNpcId}");
            return false;
        }
        actualDebtList.Add(newDebt);
        OnDebtCreation?.Invoke(newDebt);
        return true;
    }

    public void CreateNewCityDebt(int debtMarksToPay, int daysQuantityToPay, int interestPercentage, int maxDaysOver)
    {
        DebtData newDebt = new DebtData
        {
            id =  $"DEBT_{DebtTypeEnum.CITY}_{Guid.NewGuid():N}",
            debtType = DebtTypeEnum.CITY,

            debtMarksToPay = debtMarksToPay,

            startDay = Calendar_Controller.Instance.day,
            startMonth = Calendar_Controller.Instance.month,
            startYear = Calendar_Controller.Instance.year,

            daysQuantityToPay = daysQuantityToPay,

            interestPercentage = interestPercentage,
            
            maxDaysOver = maxDaysOver
        };

        if(!CheckIfCanCreateDebt(newDebt))
            return;

        actualDebtList.Add(newDebt);
        OnDebtCreation?.Invoke(newDebt);
    }

    public bool CheckExistingDebtType(DebtTypeEnum type)
    {
        return actualDebtList.Exists(i => i.debtType == type);
    }

    public bool CheckExistingSharkDebtWithNPC(int npcId)
    {
        return actualDebtList.Exists(i => i.debtType == DebtTypeEnum.SHARK && i.creditorNpcId == npcId);
    }

    public bool CheckIfCanCreateDebt(DebtData newDebt) 
    {
        if(newDebt.debtType == DebtTypeEnum.CITY) 
            return true;
        
        if(newDebt.debtType == DebtTypeEnum.BANK && CheckExistingDebtType(newDebt.debtType))
            return false;
        
        if(newDebt.debtType == DebtTypeEnum.SHARK && newDebt.creditorNpcId != -1 && CheckExistingSharkDebtWithNPC(newDebt.creditorNpcId))
            return false;

        return true;
    }
    #endregion

    #region Get Debts
    public IReadOnlyList<DebtData> GetAllActiveDebts()
    {
        return actualDebtList;
    }

    public List<DebtData> GetAllDefeatedDebts()
    {
        return actualDebtList.FindAll(d => d.state == DebtStateEnum.Defeated);
    }

    public List<DebtData> GetAllHistoryDebts()
    {
        return historyDebtList;
    }

    private DebtData GetDebt(string debtId)
    {
        return actualDebtList.Find(i => i.id == debtId);
    }
    
    public bool HasActiveDebtOfType(DebtTypeEnum type)
    {
        return actualDebtList.Any(i => i.debtType == type);
    }
    #endregion

    #region Move Debt
    private void MoveDebt(string debtId, List<DebtData> targetList)
    {
        DebtData debt = actualDebtList.Find(i => i.id == debtId);

        if (debt == null)
            return;

        targetList.Add(debt);
        actualDebtList.Remove(debt);
    }
    #endregion

    #region Pay Debt
    public void PayDebt(string debtId)
    {
        DebtData debt = actualDebtList.Find(i => i.id == debtId);

        if (debt == null)
        {
            Debug.LogWarning($"Debt {debtId} not found.");
            return;
        }

        Status_Controller status = Status_Controller.Instance;

        if(status.gold < debt.debtMarksToPay)
        {
            Debug.LogWarning($"Marks insufficient to pay ID:{debtId} /// O - {debt.debtMarksToPay}.");
            return;
        }

        status.RemoveGold(debt.debtMarksToPay);

        debt.state = DebtStateEnum.Paid;

        MoveDebt(debtId, historyDebtList);
        SetDebtCards();
    }
    #endregion

    #region Pass Day
    private void PassDay()
    {
        if (actualDebtList.Count == 0)
            return;

        for (int i = actualDebtList.Count - 1; i >= 0; i--)
        {
            DebtData debt = actualDebtList[i];
            debt.passedDays++;

            if (IsPastDue(debt) && debt.state == DebtStateEnum.Active)
            {
                debt.state = DebtStateEnum.Defeated;
            }

            if(debt.state == DebtStateEnum.Defeated)
            {
                debt.daysOverdue++;
                if (debt.daysOverdue == 1 || debt.daysOverdue % 7 == 0)
                {
                    debt.debtMarksToPay += Mathf.RoundToInt(debt.debtMarksToPay * (debt.interestPercentage/100f));
                }

                if(debt.daysOverdue > debt.maxDaysOver)
                {
                    Debug.Log("Game Over");
                }
            }
        }
    }

    private bool IsPastDue(DebtData debt)
    {
        if(debt.passedDays > debt.daysQuantityToPay)
        {
            return true;
        }

        return false;
    }
    #endregion

    #region Debt Cards
    public void SetDebtList()
    {
        GameSession gameSession = GameSession.Instance;
        if(gameSession.gameState == GameState.Paused || gameSession.gameState == GameState.PausedCutscene || gameSession.gameState == GameState.Dialogue || gameSession.gameState == GameState.PausedDialogue || gameSession.gameState == GameState.Cutscene || (Time_Controll.Instance.timerPaused && !debtListActive))
        {
            return;
        }
        
        if(!debtListActive)
        {
            ActivateDebtList();
        }
        else
        {
            DeactivateDebtList();
        }
    }

    public void ActivateDebtList()
    {
        if(debtListActive)
        {
            return;
        }
        Time_Controll.Instance.PauseTimer();
        debtListActive = true;
        debtListGroup.SetActive(true);
        SetDebtCards();
    }

    public void DeactivateDebtList()
    {
        if(!debtListActive)
        {
            return;
        }
        Time_Controll.Instance.UnpauseTimer();
        debtListActive = false;
        debtListGroup.SetActive(false);
    }

    private void ResetDebtCards()
    {
        for(int i = 0; i < debtCards.Count; i++)
        {
            debtCards[i].SetActive(false);
        }
    }

    private void SetDebtCards()
    {
        debtCardCanvaTitle.text = GameLanguageManager.Instance.GetDebtItemName("debt");

        ResetDebtCards();

        for(int i = 0; i < actualDebtList.Count; i++)
        {
            DebtCard debtCard;

            if(i >= debtCards.Count)
            {
                GameObject newCard = Instantiate(debtCardPrefab, debtCardArea.transform);
                debtCards.Add(newCard);
                debtCard = newCard.GetComponent<DebtCard>();
            }
            else
            {
                debtCard = debtCards[i].GetComponent<DebtCard>();
            }

            debtCards[i].SetActive(true);
            debtCard.SetDebtCard(actualDebtList[i]);
        }

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void ReloadDebtCards()
    {
        if(!debtListActive) return;

        SetDebtCards();
    }
    #endregion

    #region Save & Load
    public void Save(ref DebtSaveData data)
    {
        data.actualDebtList = actualDebtList;
        data.historyDebtList = historyDebtList;
    }

    public void Load(DebtSaveData data)
    {
        actualDebtList = data.actualDebtList;
        historyDebtList = data.historyDebtList;
    }
    #endregion
}