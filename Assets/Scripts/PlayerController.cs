using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private UIManager uiManager; 

    private CoinFlipper coinFlipper;
    private GameObject currentCoin;

    private bool expectedResult;
    private bool isTurnActive = false;
    private bool? debugOverrideFlip = null;

    public void StartTurn(bool expected)
    {
        expectedResult = expected;
        isTurnActive = true;

        if (uiManager != null)
        {
            uiManager.ShowAttempts(GameManager.Instance.GetPlayerAttemptsRemaining());
            uiManager.ShowPlayerResult("Your turn. Match: " + (expected ? "HEADS" : "TAILS"));
        }

        Debug.Log("Your turn. Match: " + (expectedResult ? "HEADS" : "TAILS"));
        Debug.Log("Player Turn Started, waiting for input...");
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.H)) debugOverrideFlip = true;
        else if (Input.GetKey(KeyCode.T)) debugOverrideFlip = false;
        else debugOverrideFlip = null;

        if (!isTurnActive || GameManager.Instance.GetPlayerAttemptsRemaining() <= 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == currentCoin)
            {
                CoinClickTarget clickTarget = currentCoin.GetComponent<CoinClickTarget>();
                if (clickTarget != null && clickTarget.isActive)
                {
                    coinFlipper.Flip(debugOverrideFlip);
                }
            }
        }
    }

    private void HandleFlipResult(bool result)
    {
        if (!isTurnActive)
        {
            Debug.LogWarning("Ignoring flip result: Turn not active.");
            return;
        }

        uiManager.ShowPlayerResult("You flipped: " + (result ? "HEADS" : "TAILS"));
        bool success = result == expectedResult;
        Debug.Log("Player flipped: " + (result ? "HEADS" : "TAILS") + (success ? " ✅" : " ❌"));

        if (success)
        {
            isTurnActive = false;
            RewardManager.Instance.UpdatePlayerRewards(); 
            StartCoroutine(DelayEndPlayerTurn());
        }
        else
        {
            GameManager.Instance.UsePlayerAttempt();  // ✅ Only reduce attempt on failure
            RewardManager.Instance.OnPlayerMissed();

            if (GameManager.Instance.GetPlayerAttemptsRemaining() > 0)
            {
                Debug.Log("Try again...");
                isTurnActive = true;
            }
            else
            {
                Debug.Log("Out of attempts. Ending game.");
                isTurnActive = false;
                GameManager.Instance.EndGameDueToPlayerFailure();
            }
        }
    }

    private void EndTurn(bool success)
    {
        isTurnActive = false;
        GameManager.Instance.OnPlayerFinishedTurn(success);
    }

    public void SetCurrentCoin(GameObject coin)
    {
        if (coinFlipper != null)
        {
            coinFlipper.OnFlipComplete -= HandleFlipResult;
            Debug.Log("Unsubscribed from previous coin flipper.");
        }

        currentCoin = coin;
        coinFlipper = currentCoin.GetComponent<CoinFlipper>();

        if (coinFlipper == null)
        {
            Debug.LogWarning("CoinFlipper not found on coin.");
            return;
        }

        coinFlipper.ResetState();
        coinFlipper.OnFlipComplete += HandleFlipResult;
        Debug.Log("Subscribed to new coin flipper.");
    }

    private IEnumerator DelayEndPlayerTurn()
    {
        yield return new WaitForSeconds(3.5f);  // Adjust timing for drama or animation
        EndTurn(true);  // Trigger AI’s turn after delay
    }

}
