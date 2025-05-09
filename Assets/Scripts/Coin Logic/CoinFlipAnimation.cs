using UnityEngine;

public class CoinFlip : MonoBehaviour
{
    [Header("Flip Settings")]
    public float maxHeight = 5f;
    public float baseVerticalVelocity = 10f;
    public float gravity = -20f;
    public float baseRotationSpeed = 2000f;

    [Tooltip("Set to true to land on heads, false for tails.")]
    public bool isHeads = true;

    [Header("Control Flags")]
    public bool flip = false;

    private float verticalVelocity = 0f;
    private float currentRotationSpeed = 0f;
    private bool hasFlipped = false;
    private Vector3 startPos;
    private Quaternion targetRotation;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (flip){
            if (!hasFlipped){
                verticalVelocity = baseVerticalVelocity;
                hasFlipped = true;
            }

            // Calculate current height ratio
            float height = transform.position.y - startPos.y;
            float heightRatio = Mathf.Clamp01(height / maxHeight);

            // Apply slow motion effect near the top of the arc
            float slowMoFactor = Mathf.SmoothStep(1f, 0.3f, Mathf.Sin(heightRatio * Mathf.PI));
            Time.timeScale = slowMoFactor;

            // Rotation and vertical tension slow down near apex
            float tensionFactor = Mathf.Cos(heightRatio * Mathf.PI);
            float tensionMultiplier = Mathf.Abs(tensionFactor);

            currentRotationSpeed = baseRotationSpeed * tensionMultiplier;
            float verticalSpeed = verticalVelocity * tensionMultiplier;

            // Rotate the coin
            transform.Rotate(Vector3.forward * currentRotationSpeed * Time.deltaTime);

            // Move the coin up/down
            transform.position += Vector3.up * verticalSpeed * Time.deltaTime;

            // Apply gravity
            verticalVelocity += gravity * Time.deltaTime;

            // Landing check
            if (transform.position.y <= startPos.y){
                transform.position = startPos;
                flip = false;
                verticalVelocity = 0f;

                // Reset time to normal
                Time.timeScale = 1f;

                // Set final rotation based on inspector toggle
                if (isHeads)
                    targetRotation = Quaternion.Euler(0f, 0f, 0f);
                else
                    targetRotation = Quaternion.Euler(180f, 0f, 0f);

                transform.rotation = targetRotation;
                hasFlipped = false;
            }
        }
    }
}
