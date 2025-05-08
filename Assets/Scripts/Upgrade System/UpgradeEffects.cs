using System.Collections.Generic;
using UnityEngine;
using System.Linq;

    public static class UpgradeEffects
    {
        private static List<Upgrade> queuedOneTimeUpgrades = new();
        public static void ApplyAllEffects(ref bool expected, int flipIndex)
        {
            //Apply One-Time Use Upgrades First
            foreach (var upgrade in queuedOneTimeUpgrades)
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

                Debug.Log($"[UpgradeEffects] Applied OTU: {upgrade.upgradeName}");
            }

            if (queuedOneTimeUpgrades.Count > 0)
            {
                Debug.Log($"[UpgradeEffects] Cleared {queuedOneTimeUpgrades.Count} OTU upgrades after use.");
                queuedOneTimeUpgrades.Clear();
            }

            //Apply Passive Upgrades
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
    public static List<Upgrade> GetQueuedOTUs() => queuedOneTimeUpgrades;

    private static void ApplyTilt(ref bool expected, Upgrade upgrade, int flipIndex)
    {
        if (upgrade.category != UpgradeCategory.Passive &&
            !queuedOneTimeUpgrades.Contains(upgrade))
            return;

        if (upgrade.firstFlipOnly && flipIndex != 0)
            return;

        bool biasApplies =
            (expected && upgrade.affectsHeads) ||
            (!expected && upgrade.affectsTails);

        if (!biasApplies)
            return;

        float chance = upgrade.effectStrength;
        expected = Random.value < Mathf.Clamp01(0.5f + chance) ? expected : !expected;

        Debug.Log($"[UpgradeEffects] Tilt effect applied: {upgrade.upgradeName}");
    }


    private static void ApplyRewardEffect(Upgrade upgrade)
    {
        if (!upgrade.affectsCash)
            return;

        int currentCash = RewardManager.Instance.GetCurrentCash();

        // Flat passive income
        if (upgrade.flatCashAmount > 0 && upgrade.category == UpgradeCategory.Passive)
        {
            if (GameManager.Instance.IsPlayerTurn()) // ✅ Only apply if it's the player's turn
            {
                RewardManager.Instance.AddCash(upgrade.flatCashAmount);
                Debug.Log($"[UpgradeEffects] Passive Income: +${upgrade.flatCashAmount}");
            }
            return;
        }

        // Percent-based success/fail logic
        if (upgrade.rewardModifiesSuccess || upgrade.rewardModifiesFailure)
        {
            bool matched = GameManager.Instance.LastFlipMatched();

            if (matched && upgrade.rewardModifiesSuccess)
            {
                int bonus = Mathf.RoundToInt(currentCash * (upgrade.successCashMultiplier - 1f));
                RewardManager.Instance.AddCash(bonus);
                Debug.Log($"[UpgradeEffects] Success Reward: +${bonus} from {upgrade.upgradeName}");
            }

            if (!matched && upgrade.rewardModifiesFailure)
            {
                int loss = Mathf.RoundToInt(currentCash * upgrade.failCashPenaltyPercent);
                RewardManager.Instance.SpendCash(loss);
                Debug.Log($"[UpgradeEffects] Failure Penalty: -${loss} from {upgrade.upgradeName}");
            }
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
        queuedOneTimeUpgrades.Add(upgrade);
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

        var allUpgrades = UpgradeManager.Instance.GetPlayerUpgrades().Concat(queuedOneTimeUpgrades);

        foreach (var upgrade in allUpgrades)
        {
            if (upgrade.effectType == UpgradeEffectType.Tilt)
            {
                if (upgrade.affectsHeads)
                    modifier += upgrade.effectStrength * 100f;
                if (upgrade.affectsTails)
                    modifier -= upgrade.effectStrength * 100f; // inverse effect
            }
        }

        return Mathf.Clamp(baseChance + modifier, 0f, 100f);
    }


    public static float GetChanceToLandTails()
    {
        float baseChance = 50f;
        float modifier = 0f;

        var allUpgrades = UpgradeManager.Instance.GetPlayerUpgrades().Concat(queuedOneTimeUpgrades);

        foreach (var upgrade in allUpgrades)
        {
            if (upgrade.effectType == UpgradeEffectType.Tilt)
            {
                if (upgrade.affectsTails)
                    modifier += upgrade.effectStrength * 100f;
                if (upgrade.affectsHeads)
                    modifier -= upgrade.effectStrength * 100f;
            }
        }

        return Mathf.Clamp(baseChance + modifier, 0f, 100f);
    }


    public static float GetChanceToMatchExpected()
    {
        float baseChance = 50f; // assuming neutral
        float modifier = 0f;

        var allUpgrades = UpgradeManager.Instance.GetPlayerUpgrades().Concat(queuedOneTimeUpgrades);

        foreach (var upgrade in allUpgrades)
        {
            if (upgrade.effectType == UpgradeEffectType.Tilt && upgrade.affectsMatchExpected)
            {
                modifier += upgrade.effectStrength * 100f;
            }
        }

        return Mathf.Clamp(baseChance + modifier, 0f, 100f);
    }

}