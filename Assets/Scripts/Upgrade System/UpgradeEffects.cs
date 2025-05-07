
using UnityEngine;

public static class UpgradeEffects
{
    public static void ApplyTiltHeads(ref bool result)
    {
        if (!Has(Upgrade.UpgradeType.Tilt)) return;

        float biasChance = 0.7f; // 70% chance to force heads
        result = UnityEngine.Random.value < biasChance ? true : false;
        Debug.Log("[UpgradeEffects] TiltHeads applied. Result is now: " + result);
    }

    public static void ApplyTiltTails(ref bool result)
    {
        if (!Has(Upgrade.UpgradeType.Tilt)) return;

        float biasChance = 0.7f; // 70% chance to force tails
        result = UnityEngine.Random.value < biasChance ? false : true;
        Debug.Log("[UpgradeEffects] TiltTails applied. Result is now: " + result);
    }

    public static void ApplyDoubleReward(ref int reward)
    {
        if (Has(Upgrade.UpgradeType.DoubleOrNothing))
        {
            reward *= 2;
            Debug.Log("[UpgradeEffects] Double or Nothing applied. New reward: " + reward);
            UpgradeManager.Instance.UseOneTimeUpgrade(Upgrade.UpgradeType.DoubleOrNothing);
        }
    }

    public static void ApplyStreakBoost(ref int multiplier)
    {
        if (Has(Upgrade.UpgradeType.Streaks))
        {
            multiplier += 1;
            Debug.Log("[UpgradeEffects] Streaks bonus applied. Multiplier boosted to: " + multiplier);
        }
    }

    public static bool ShouldStunOpponent()
    {
        if (Has(Upgrade.UpgradeType.Stun))
        {
            UpgradeManager.Instance.UseOneTimeUpgrade(Upgrade.UpgradeType.Stun);
            Debug.Log("[UpgradeEffects] Stun activated!");
            return true;
        }
        return false;
    }

    public static int GetMercyBonus()
    {
        return UpgradeManager.Instance.GetUpgradeCount(Upgrade.UpgradeType.Mercy);
    }

    private static bool Has(Upgrade.UpgradeType type)
    {
        return UpgradeManager.Instance != null && UpgradeManager.Instance.HasUpgrade(type);
    }

    public static float GetChanceToLandHeads()
    {
        return Has(Upgrade.UpgradeType.Tilt) ? 70f : 50f;
    }

    public static float GetChanceToLandTails()
    {
        return Has(Upgrade.UpgradeType.Tilt) ? 30f : 50f;
    }

    public static float GetChanceToMatchExpected()
    {
        return Has(Upgrade.UpgradeType.Tilt) ? 65f : 50f;
    }
}
