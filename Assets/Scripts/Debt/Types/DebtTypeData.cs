using UnityEngine;

[CreateAssetMenu]
public class DebtTypeData : ScriptableObject
{
    public DebtTypeEnum type;
    public string displayName;
    public int defaultInterest;
    public int defaultDuration;
    public int amount;
    public int compoundInterest;
    public int maxDaysOver;
    public int quantityDaysToPay;
    public int creditorNpcId = -1;
}