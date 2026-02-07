using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Sell_Controller : MonoBehaviour
{
    public static Sell_Controller Instance { get; private set; }

    #region Variables
    [Header("UI")]
    [SerializeField] GameObject sellUi;
    [SerializeField] TextMeshProUGUI titleSellText;
    [SerializeField] TextMeshProUGUI infosText;
    [SerializeField] TextMeshProUGUI totalText;
    [SerializeField] TextMeshProUGUI continueButtonText;
    [SerializeField] TextMeshProUGUI taxText;
    [SerializeField] TextMeshProUGUI gainText;
    [SerializeField] TextMeshProUGUI actualTaxText;
    [SerializeField] TextMeshProUGUI taxPaidText;
    [SerializeField] TextMeshProUGUI nextYearTaxText;
    [SerializeField] TextMeshProUGUI debtText;
    [SerializeField] TextMeshProUGUI jokeText;
    [SerializeField] GameObject sellContentContainer;
    [SerializeField] GameObject sellContentSlot;
    [SerializeField] GameObject darkBackground;

    private List<string> jokeListLow = new List<string>();
    private List<string> jokeListMedium = new List<string>();
    private List<string> jokeListHigh = new List<string>();
    private List<string> jokeListNoProfit = new List<string>();

    private Dictionary<Item, int> sellControlDict = new Dictionary<Item, int>();

    private List<Item> sellItemsList = new List<Item>();

    public bool active = false;

    public static Action OnSellFinish;

    #endregion

    #region Core
    void Awake()
    {
        Instance = this;

        jokeListLow = new List<string>()
        {
            "A taxa de {tax}% hoje foi boazinha… quase dá para agradecer.",
            "Relaxa, só tiraram um pedacinho. Bem pequeno. Micro. Invisível até.",
            "Seu lucro foi de ${profit}. O governo pegou só um cafezinho.",
            "Hoje o governo foi moderado. Isso deve ser bug.",
            "Parabéns pela venda de ${total}! E parabéns ao governo pelo 'desconto'.",
            "Taxa baixa? Aproveita, é evento raro.",
            "Até o governo ficou com pena dessa taxa.",
            "Com uma mordida tão pequena, dá até para trabalhar feliz.",
            "Sua venda de {quantity} itens quase passou despercebida pelo governo.",
            "Se toda taxa fosse assim, até dava vontade de pagar."
        };

        jokeListMedium = new List<string>()
        {
            "Você vendeu ${total}. O governo achou justo pegar sua parte… e um pouco da parte da sua parte.",
            "Taxa de {tax}% aplicada com sucesso. Sucesso para quem?",
            "O governo analisou seus ganhos e pensou: ‘posso pegar mais’.",
            "${taxed} de imposto? Isso é o famoso 'toma aqui que é nosso'.",
            "Seu lucro foi de ${profit}. O do governo foi melhor.",
            "Parabéns! Hoje você trabalhou por duas pessoas: por você e pelo governo.",
            "Imposto médio, dor média, tristeza média.",
            "Seu esforço foi médio. A taxa também. Todo mundo perdeu um pouco.",
            "O governo insiste que isso é 'contribuição'. Você insiste que isso dói.",
            "Nem alto demais, nem baixo demais. A taxa perfeita para atrapalhar."
        };

        jokeListHigh = new List<string>()
        {
            "O governo viu sua venda de ${total} e imediatamente ficou animado. Muito animado.",
            "Sua taxa foi de {tax}%. A partir desse valor, já é assalto sentimental.",
            "${taxed} em impostos? O governo chama isso de ‘lanche da tarde’.",
            "Quando virou dívida? Não virou. Só parece.",
            "A economia agradece. Você não.",
            "Você trabalha, o governo vibra. Grande parceria.",
            "${profit} para você e ${taxed} para o governo. Parece justo? Para o governo, sim.",
            "Isso não é taxa. Isso é DLC obrigatória.",
            "Houve lucro? Sim. Para alguém.",
            "Taxa alta detectada. Seus sentimentos foram automaticamente tributados."
        };

        jokeListNoProfit = new List<string>()
        {
            "Pouco lucro hoje… bom para você, ruim para o governo.",
            "Nem o governo achou interessante te tributar hoje. Parabéns?",
            "A venda foi tão baixa que o governo nem se deu ao trabalho.",
            "Sem imposto hoje. O governo entrou em modo ‘leave me alone’.",
            "Você ganhou pouco, mas pelo menos ninguém tentou tirar de você."
        };
    }
    #endregion

    #region Events
    void OnEnable()
    {
        Calendar_Controller.OnDayChange += SellItems;
    }

    void OnDisable()
    {
        Calendar_Controller.OnDayChange -= SellItems;
    }
    #endregion

    #region Sell
    public void SellItems()
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

        sellItemsList.Clear();

        Tax_System.Instance.UpdateTaxPaidDuringYear(taxedValue);
        Tax_System.Instance.AddSellItemsValueToAnualSells(totalValue);

        ActivateDeactivateUi();
        
        RefreshSellContentUI();
        SetInfos(taxedValue, totalValue, gainedValue);

        gainText.text = $"{GameLanguageManager.Instance.GetSellMenuItemName("total")}${gainedValue}";
        taxText.text = $"{GameLanguageManager.Instance.GetSellMenuItemName("tax")}-${taxedValue}";
        totalText.text = $"{GameLanguageManager.Instance.GetSellMenuItemName("received")}${totalValue}";

        Status_Controller.Instance.AddGold(gainedValue);

        OnSellFinish?.Invoke();
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
        
        darkBackground.SetActive(active);
        sellUi.SetActive(active);
        InventoryManager.Instance.ControllInventoryGroup(!active);
        Calendar_Controller.Instance.ControllTimeGroup(!active);
        Status_Controller.Instance.SetControllPlayerUiGroup(!active);
    }

    private void SetInfos(int taxedValue, int totalValue, int gainedValue)
    {
        titleSellText.text = GameLanguageManager.Instance.GetSellMenuItemName("sell");
        infosText.text = GameLanguageManager.Instance.GetSellMenuItemName("infos");
        continueButtonText.text = GameLanguageManager.Instance.GetSellMenuItemName("continue");
        actualTaxText.text = $"{GameLanguageManager.Instance.GetSellMenuItemName("actualTax")}{Tax_System.Instance.GetTax()}%";
        taxPaidText.text = $"{GameLanguageManager.Instance.GetSellMenuItemName("totalTax")}${Tax_System.Instance.GetTaxPaidDuringYear()}";
        nextYearTaxText.text = $"{GameLanguageManager.Instance.GetSellMenuItemName("yearTaxPrevision")}${Tax_System.Instance.CalculateAnualTax()}";
        debtText.text = $"{GameLanguageManager.Instance.GetSellMenuItemName("debt")}$0";
        
        jokeText.text = ReplaceVars(GetRandomJoke(taxedValue), taxedValue, totalValue, gainedValue, sellControlDict.Count);
    }

    private string ReplaceVars(string text, int taxedValue, int total, int profit, int quantity)
    {
        return text
            .Replace("{tax}", Tax_System.Instance.GetTax().ToString())
            .Replace("{total}", total.ToString())
            .Replace("{taxed}", taxedValue.ToString())
            .Replace("{profit}", profit.ToString())
            .Replace("{quantity}", quantity.ToString())
            .Replace("{yearTax}", Tax_System.Instance.CalculateAnualTax().ToString());
    }


    private string GetRandomJoke(int taxedValue)
    {
        int index = 0;
        if (taxedValue == 0)
        {
            if (jokeListNoProfit.Count == 0) return "";
            index = UnityEngine.Random.Range(0, jokeListNoProfit.Count);
            return jokeListNoProfit[index];
        }
        if(taxedValue < 500)
        {
            if (jokeListLow.Count == 0) return "";
            index = UnityEngine.Random.Range(0, jokeListLow.Count);
            return jokeListLow[index];
        } else if(taxedValue > 1000)
        {
            if (jokeListMedium.Count == 0) return "";
            index = UnityEngine.Random.Range(0, jokeListMedium.Count);
            return jokeListMedium[index];
        } else
        {
            if (jokeListHigh.Count == 0) return "";
            index = UnityEngine.Random.Range(0, jokeListHigh.Count);
            return jokeListHigh[index];
        }
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


    public void AddItemToList(Item newItem)
    {
        if (!newItem) return;

        sellItemsList.Add(newItem);
    }

    public void RemoveItemFromList(Item item)
    {
        if (!sellControlDict.ContainsKey(item)) return;

        sellItemsList.Remove(item);
    }
    #endregion
}
