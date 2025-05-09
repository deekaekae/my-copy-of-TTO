using UnityEngine;

public class CoinClickTarget : MonoBehaviour
{
    public bool isActive = false;

    public void SetActive(bool state){
        isActive = state;

        // Optional: Visual feedback
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = state ? Color.white : Color.gray;
    }
}
