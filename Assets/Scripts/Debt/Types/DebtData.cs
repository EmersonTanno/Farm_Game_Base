[System.Serializable]
public class DebtData
{
    public string id;
    public DebtTypeEnum debtType;
    public bool paid = false;
    public float extraPercentageToPay;

    //valor pego e valor a pagar
    public int quantityMarksTaken;
    public int debtMarksToPay;

    //adquirida em
    public int startDay;
    public Season startSeason;
    public int startYear;

    //data final para pagar
    public int finalDay;
    public Season finalSeason;
    public int finalYear;

    //porcentagem de juros por dia passado
    public float interestPercentage;
    
    //quantidade máxima de dias que pode passar do dia máximo
    public int maxDaysOver;
}