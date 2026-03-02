[System.Serializable]
public class DebtData
{
    public string id;
    public DebtTypeEnum debtType;
    public DebtStateEnum state = DebtStateEnum.Active;
    public int creditorNpcId;
    public int extraPercentageToPay;

    //valor pego e valor a pagar
    public int quantityMarksTaken;
    public int debtMarksToPay;

    //adquirida em
    public int startDay;
    public int startMonth;
    public int startYear;

    //data final para pagar
    public int daysQuantityToPay;
    public int passedDays = 0;

    //porcentagem de juros por dia passado
    public int interestPercentage;
    
    //quantidade máxima de dias que pode passar do dia máximo
    public int maxDaysOver;
    public int daysOverdue = 0;
}