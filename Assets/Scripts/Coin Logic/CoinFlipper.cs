using System.Collections;
using UnityEngine;

using System;
public class CoinFlipper : MonoBehaviour
{
    public event Action<bool> OnFlipComplete; // true = heads, false = tails

    // DONT FORGET TO UN Comment, use other flip() for testing

    /*
    public void Flip()
    {
        
        bool result = UnityEngine.Random.value < 0.5f;
        Debug.Log("Coin flipped: " + (result ? "HEADS" : "TAILS"));

        // Optional: Trigger animation or camera here later
        // StartCoroutine(PlayFlipAnimation(result));

        // Immediately return result (can delay if animated later)
        OnFlipComplete?.Invoke(result);
    }

    // Optional animation stub for future use
    /*
    private IEnumerator PlayFlipAnimation(bool result)
    {
        // TODO: Play animation or trigger camera
        yield return new WaitForSeconds(1f);
        OnFlipComplete?.Invoke(result);
    }
    */
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

        bool result;
        if (overrideResult.HasValue)
        {
            result = overrideResult.Value;
            Debug.Log("DEBUG: Forced " + (result ? "HEADS" : "TAILS"));
        }
        else
        {
            result = UnityEngine.Random.value < 0.5f;
            Debug.Log("Coin flipped: " + (result ? "HEADS" : "TAILS"));
        }

        if (OnFlipComplete == null)
        {
            Debug.LogWarning($"{gameObject.name}: No flip listeners found. This flip will not trigger any logic.");
        }
        else
        {
            OnFlipComplete.Invoke(result);
        }

        Debug.Log("Flip() called with override: " + (overrideResult.HasValue ? overrideResult.ToString() : "none"));
        Debug.Log($"{gameObject.name} was flipped by {(overrideResult.HasValue ? "DEBUG KEY" : "RUNTIME")} at frame {Time.frameCount}");

        StartCoroutine(ResetFlipLock());
    }


    private IEnumerator ResetFlipLock()
    {
        yield return new WaitForSeconds(0.1f);
        isFlipping = false;
    }

}

