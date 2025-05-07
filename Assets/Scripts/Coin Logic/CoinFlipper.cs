using System.Collections;
using UnityEngine;

using System;
public class CoinFlipper : MonoBehaviour
{
    public event Action<bool> OnFlipComplete; // true = heads, false = tails
    private bool isFlipping = false;

    public void ResetState()
    {
        isFlipping = false;
        Debug.Log($"{gameObject.name}: CoinFlipper.ResetState() called.");
    }

    public void Flip(bool? overrideResult = null)
    {
        if (isFlipping)
        {
            Debug.LogWarning($"{gameObject.name} is already flipping, skipping duplicate call.");
            return;
        }

        isFlipping = true;

        bool result = overrideResult ?? UnityEngine.Random.value < 0.5f;
        Debug.Log($"{gameObject.name} Flip result: {(result ? "HEADS" : "TAILS")}");

        // Trigger CoinFlip animation
        CoinFlip anim = GetComponent<CoinFlip>();
        if (anim != null)
        {
            anim.isHeads = result;
            anim.flip = true;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} has no CoinFlip component attached.");
        }

        StartCoroutine(FinishFlipAfterAnimation(result));
    }

    private IEnumerator FinishFlipAfterAnimation(bool result)
    {
        yield return new WaitForSeconds(3.5f); // Adjust if animation timing changes
        OnFlipComplete?.Invoke(result);
        isFlipping = false;
    }
}

