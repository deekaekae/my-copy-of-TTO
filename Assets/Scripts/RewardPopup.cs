using UnityEngine;
using TMPro;

public class RewardPopup : MonoBehaviour
{
    public TextMeshProUGUI popupText;
    public float lifetime = 3f;
    public float floatSpeed = 20f;

    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);
        
        if (canvasGroup != null)
            canvasGroup.alpha -= Time.deltaTime / lifetime;
    }

    public void SetText(string message){
        if (popupText != null)
            popupText.text = message;
        else
            Debug.LogWarning("PopupText is null");
    }
}
