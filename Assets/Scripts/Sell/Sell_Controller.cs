using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Sell_Controller : MonoBehaviour
{
    public static Sell_Controller Instance { get; private set; }

    #region Variables
    [Header("UI")]
    [SerializeField] GameObject sellUi;
    [SerializeField] TextMeshProUGUI totalText;
    [SerializeField] TextMeshProUGUI taxText;
    [SerializeField] TextMeshProUGUI gainText;
    [SerializeField] TextMeshProUGUI actualTaxText;
    [SerializeField] TextMeshProUGUI taxPaidText;
    [SerializeField] TextMeshProUGUI nextYearTaxText;
    [SerializeField] TextMeshProUGUI debtText;
    [SerializeField] TextMeshProUGUI jokeText;
    [SerializeField] GameObject sellContentContainer;
    [SerializeField] GameObject sellContentSlot;

    private List<string> jokeList = new List<string>();

    private Dictionary<Item, int> sellControlDict = new Dictionary<Item, int>();

    public bool active = false;

    #endregion

    #region Core
    void Awake()
    {
        Instance = this;

        jokeList = new List<string>()
        {
            "O governo agradece a sua contribuição involuntária.",
            "Seu lucro é nosso lucro também.",
            "Não se preocupe, a taxa volta em benefício… para alguém.",
            "Sorria: você está sendo tributado.",
            "Impostos: porque nem no mundo pixelado dá pra fugir.",
            "Contribuir é lindo… sobreviver também seria.",
            "Obrigado por financiar o progresso. De alguém.",
            "Taxas: porque até no mundo 2D existe sofrimento.",
            "Parabéns! Você vendeu… e o governo também ganhou.",
            "A vida é feita de escolhas. Menos os impostos.",
            "Sua produtividade é inspiradora. Para o governo.",
            "Aproveite seu lucro! Ao menos o que sobrou dele.",
            "Contribuição obrigatória: a mecânica mais realista do jogo."
        };
    }
    #endregion

    #region Sell
    public void SellItems(List<Item> sellItemsList)
    {
        int gainedValue = 0;
        int taxedValue = 0;
        int totalValue = 0;

        sellControlDict.Clear();

        foreach (var item in sellItemsList)
        {
            totalValue += item.sellValue;

            int tax = Tax_System.Instance.ApplySellTaxes(item.sellValue);
            taxedValue += tax;
            gainedValue += item.sellValue - tax;

            AddItem(item);
        }

        Tax_System.Instance.UpdateTaxPaidDuringYear(taxedValue);
        Tax_System.Instance.AddSellItemsValueToAnualSells(totalValue);

        ActivateDeactivateUi();
        
        RefreshSellContentUI();
        SetInfos(taxedValue);

        gainText.text = $"Recebido: ${gainedValue}";
        taxText.text = $"Taxas: -${taxedValue}";
        totalText.text = $"Total: ${totalValue}";

        Status_Controller.Instance.AddGold(gainedValue);
    }
    #endregion

    #region UI
    private void RefreshSellContentUI()
    {
        foreach (Transform child in sellContentContainer.transform)
            Destroy(child.gameObject);

        if (sellControlDict.Count == 0) return;

        foreach (var kvp in sellControlDict)
        {
            Item item = kvp.Key;
            int quantity = kvp.Value;

            GameObject newSlot = Instantiate(
                sellContentSlot,
                sellContentContainer.transform
            );

            Sell_Content_Slot slot = newSlot.GetComponent<Sell_Content_Slot>();
            slot.SetInfo(item, quantity);
        }
    }


    public void ActivateDeactivateUi()
    {
        if(active == true)
        {
            active = false;
            Time_Controll.Instance.UnpauseTime();
        } else
        {
            active = true;
        }
        
        sellUi.SetActive(active);
    }

    private void SetInfos(int taxedValue)
    {
        actualTaxText.text = $"Imposto atual: {Tax_System.Instance.GetTax()}%";
        taxPaidText.text = $"Total de impostos pagos: ${Tax_System.Instance.GetTaxPaidDuringYear()}";
        nextYearTaxText.text = $"Previsão do imposto anual: ${Tax_System.Instance.CalculateAnualTax()}";
        debtText.text = $"Débito: $0";
        
        if(taxedValue > 0)
        {
            jokeText.text = GetRandomJoke();
        } else
        {
            jokeText.text = "Pouco lucro hoje… bom para você, ruim para o governo.";
        }
    }

    private string GetRandomJoke()
    {
        if (jokeList.Count == 0) return "";
        int index = Random.Range(0, jokeList.Count);
        return jokeList[index];
    }
    #endregion

    #region Add / Remove Dictionary
    public void AddItem(Item newItem)
    {
        if (!newItem) return;

        if (sellControlDict.ContainsKey(newItem))
            sellControlDict[newItem]++;
        else
            sellControlDict[newItem] = 1;
    }

    public void RemoveItem(Item item)
    {
        if (!sellControlDict.ContainsKey(item)) return;

        sellControlDict[item]--;

        if (sellControlDict[item] <= 0)
            sellControlDict.Remove(item);
    }
    #endregion
}
