using System.Collections.Generic;
using UnityEngine;

public static class UpgradeEffects
{
    public static void ApplyAllEffects(ref bool expected, int flipIndex)
    {
        foreach (var upgrade in UpgradeManager.Instance.GetPlayerUpgrades())
        {
            switch (upgrade.effectType)
            {
                case UpgradeEffectType.Tilt:
                    ApplyTilt(ref expected, upgrade, flipIndex);
                    break;

                case UpgradeEffectType.Reward:
                    ApplyRewardEffect(upgrade);
                    break;

                case UpgradeEffectType.Mercy:
                    ApplyMercyEffect(upgrade);
                    break;

                case UpgradeEffectType.Sequence:
                    ApplySequenceEffect(upgrade);
                    break;
            }
        }
    }

    private static void ApplyTilt(ref bool expected, Upgrade upgrade, int flipIndex)
    {
        if (upgrade.category != UpgradeCategory.Passive) return;
        if (upgrade.firstFlipOnly && flipIndex != 0) return;

        bool biasApplies =
            (expected && upgrade.affectsHeads) ||
            (!expected && upgrade.affectsTails);

        if (!biasApplies) return;

        float chance = upgrade.effectStrength;
        expected = Random.value < chance ? expected : !expected;

        Debug.Log($"[UpgradeEffects] Tilt effect applied: {upgrade.name}");
    }

    private static void ApplyRewardEffect(Upgrade upgrade)
    {
        if (upgrade.category != UpgradeCategory.Passive) return;

        switch (upgrade.upgradeName)
        {
            case "Multiplier Boost":
                int bonus = Mathf.RoundToInt(upgrade.effectStrength);
                RewardManager.Instance.ApplyMultiplierBonus(bonus);
                Debug.Log("[UpgradeEffects] Multiplier Boost applied");
                break;
        }
    }

    private static void ApplyMercyEffect(Upgrade upgrade)
    {
        if (upgrade.category != UpgradeCategory.Passive) return;

        switch (upgrade.upgradeName)
        {
            case "Extra Attempt":
                GameManager.Instance.AddExtraAttempt(); // This method must exist in GameManager
                Debug.Log("[UpgradeEffects] Extra Attempt granted");
                break;
        }
    }

    private static void ApplySequenceEffect(Upgrade upgrade)
    {
        Debug.Log($"[UpgradeEffects] Sequence effect triggered for: {upgrade.name}");
    }

    // Optional for runtime effects
    public static void ApplyOneTimeUseUpgrade(Upgrade upgrade)
    {
        Debug.Log($"[UpgradeEffects] One-time upgrade used: {upgrade.name}");
    }

    // Optional helper if you want passive upgrades to influence calculations
    
    public static float GetTotalMultiplierBonus()
    {
        float total = 0f;
        foreach (var upgrade in UpgradeManager.Instance.GetPlayerUpgrades())
        {
            if (upgrade.effectType == UpgradeEffectType.Reward &&
                upgrade.upgradeName == "Multiplier Boost" &&
                upgrade.category == UpgradeCategory.Passive)
            {
                total += upgrade.effectStrength;
            }
        }
        return total;
    }

    public static float GetChanceToLandHeads()
    {
        float baseChance = 50f;
        float modifier = 0f;

        foreach (var upgrade in UpgradeManager.Instance.GetPlayerUpgrades())
        {
            if (upgrade.effectType == UpgradeEffectType.Tilt && upgrade.affectsHeads)
            {
                modifier += upgrade.effectStrength * 100f;
            }
        }

        return Mathf.Clamp(baseChance + modifier, 0f, 100f);
    }

    public static float GetChanceToLandTails()
    {
        float baseChance = 50f;
        float modifier = 0f;

        foreach (var upgrade in UpgradeManager.Instance.GetPlayerUpgrades())
        {
            if (upgrade.effectType == UpgradeEffectType.Tilt && upgrade.affectsTails)
            {
                modifier += upgrade.effectStrength * 100f;
            }
        }

        return Mathf.Clamp(baseChance + modifier, 0f, 100f);
    }

    public static float GetChanceToMatchExpected()
    {
        float baseChance = 50f; // assuming neutral
        float modifier = 0f;

        foreach (var upgrade in UpgradeManager.Instance.GetPlayerUpgrades())
        {
            if (upgrade.effectType == UpgradeEffectType.Tilt && upgrade.affectsMatchExpected)
            {
                modifier += upgrade.effectStrength * 100f;
            }
        }

        return Mathf.Clamp(baseChance + modifier, 0f, 100f);
    }

}