using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //TextMeshPro for UI

public class Announcer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI announcementText;

    private bool currentCall; // true = heads, false = tails

    public bool CurrentCall => currentCall;

    public void Announce(bool coinCall)
    {
        currentCall = coinCall;

        string callString = currentCall ? "HEADS" : "TAILS";
        Debug.Log("Announcer says: " + callString);

        if (announcementText != null)
            announcementText.text = $"Next Coin: {callString}";
    }

    // Optional helper: Randomly announce a new call
    public bool GenerateAndAnnounce()
    {
        bool coinCall = Random.value < 0.5f;
        Announce(coinCall);
        return coinCall;
    }
}
