using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade System/Upgrade")]
public class Upgrade : ScriptableObject
{
    [Header("Basic Info")]
    public string upgradeName;
    [TextArea] public string description;
    public int cost;

    [Header("Classification")]
    public UpgradeCategory category;         // Passive or OneTimeUse
    public UpgradeEffectType effectType;     // Tilt, Reward, Mercy, etc.

    [Header("Effect Data")]
    [Range(-1f, 1f)]
    public float effectStrength = 0.1f;      // Ex: +0.1 = +10% odds
    public bool affectsMatchExpected =false; 
    public bool affectsHeads = false;        // Optional: used by Tilt-type
    public bool affectsTails = false;        // Optional: used by Tilt-type
    public bool firstFlipOnly = false;       // Optional: "first flip each round" targeting
    
    [Header("Reward Effect Settings")]
    public bool rewardModifiesSuccess = false;
    public bool affectsMultiplier = false;
    public float successCashMultiplier = 1f; // 1x means no change, 2x doubles
    public bool rewardModifiesFailure = false;
    public float failCashPenaltyPercent = 0f; // 0.3 = lose 30%
    public int flatCashAmount = 0; // still used for passive income
    public bool affectsCash = false;


}

public enum UpgradeCategory
{
    Passive,
    OneTimeUse
}

public enum UpgradeEffectType
{
    Tilt,
    Reward,
    Mercy,
    Sequence
}
