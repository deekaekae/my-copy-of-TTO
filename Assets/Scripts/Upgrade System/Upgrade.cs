using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade System/Upgrade")]
public class Upgrade : ScriptableObject
{
    public string upgradeName;
    public string description;
    public int cost;
    public UpgradeType type;
    public bool isOneTimeUse;

    public enum UpgradeType
    {
        DoubleOrNothing,
        Tilt,
        Streaks,
        Thief,
        Stun,
        Mercy
    }
}
