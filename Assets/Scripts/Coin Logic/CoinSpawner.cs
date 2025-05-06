using UnityEngine;
using System.Collections.Generic;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Transform spawnCenter; // New center point for row layout
    

    private List<GameObject> activeCoins = new List<GameObject>();

    public void ClearCoins()
    {
        foreach (var coin in activeCoins)
        {
            Destroy(coin);
        }
        activeCoins.Clear();
    }

    public void MoveCoinAside(GameObject coin)
    {
        int usedIndex = activeCoins.IndexOf(coin);
        float offset = (usedIndex - 0.5f) * .75f;
        coin.transform.position = spawnCenter.position + new Vector3(offset, 0f, 2f);
    }

    public GameObject SpawnSingleCoin()
    {
        // Spread evenly around center
        Vector3 position = spawnCenter.position;
        GameObject coin = Instantiate(coinPrefab, position, Quaternion.identity);
        activeCoins.Add(coin);
        return coin;
    }
}
