using UnityEngine;
using TMPro;
using System.Collections;

public class AIController : MonoBehaviour
{
    
    private CoinFlipper coinFlipper;
    private bool expectedResult;
    private bool isTurnActive = false;
    private GameObject currentCoin;
    [SerializeField] private UIManager uiManager;


    public void StartTurn(bool expected){
        
        expectedResult = expected;
        isTurnActive = true;

        Debug.Log("AI Turn. Target: " + (expected ? "HEADS" : "TAILS"));

        StartCoroutine(AIDelayFlip());
    }

    private IEnumerator AIDelayFlip(){
        yield return new WaitForSeconds(1.5f); // Give camera time

        // Check if round is still valid
        if (!isTurnActive || GameManager.Instance == null || currentCoin == null){
            Debug.LogWarning("AI flip aborted: Turn ended or context lost.");
            yield break;
        }

        Debug.Log("AI is flipping...");
        coinFlipper.Flip();
    }


    private void HandleFlipResult(bool result){
        if (!isTurnActive) return;
        isTurnActive = false;

        if (uiManager != null)
            uiManager.ShowAIResult("He flips: " + (result ? "HEADS" : "TAILS"));

        bool success = result == expectedResult;

        if (success){
            RewardManager.Instance.UpdateAIRewards();
        }

        GameManager.Instance.OnAIFinishedTurn(success);
    }

    

    public void SetCurrentCoin(GameObject coin){
        if (coinFlipper != null)
            coinFlipper.OnFlipComplete -= HandleFlipResult;

        currentCoin = coin;
        coinFlipper = currentCoin.GetComponent<CoinFlipper>();

        if (coinFlipper != null){
            coinFlipper.OnFlipComplete += HandleFlipResult;
            Debug.Log("AIController: Subscribed to new coin flipper");
        }
        else{
            Debug.LogWarning("AIController: No CoinFlipper found on assigned coin");
        }
    }

}
