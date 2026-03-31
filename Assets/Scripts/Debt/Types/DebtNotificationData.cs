public class DebtNotificationData
{
    public DebtData debt;
    public bool isPayment;

    public DebtNotificationData(DebtData debt, bool isPayment)
    {
        this.debt = debt;
        this.isPayment = isPayment;
    }
}