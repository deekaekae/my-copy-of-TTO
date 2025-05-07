using UnityEngine;

public class CameraRigController : MonoBehaviour
{
    public Transform announcerPosition;
    public Transform playerPosition;
    public Transform aiPosition;

    public Transform announcerLookTarget;
    public Transform playerLookTarget;
    public Transform aiLookTarget;

    public float moveSpeed = 3f;
    public float lookSpeed = 3f;

    private Transform currentRigPosition;
    private Transform currentLookTarget;
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
        MoveToAnnouncer();
    }

    private void Update()
    {
        if (currentRigPosition != null)
        {
            transform.position = Vector3.Lerp(transform.position, currentRigPosition.position, Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, currentRigPosition.rotation, Time.deltaTime * moveSpeed);
        }

        if (mainCam != null && currentLookTarget != null)
        {
            Vector3 direction = currentLookTarget.position - mainCam.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
        }
    }

    public void MoveToAnnouncer()
    {
        currentRigPosition = announcerPosition;
        currentLookTarget = announcerLookTarget;
    }

    public void MoveToPlayer()
    {
        currentRigPosition = playerPosition;
        currentLookTarget = playerLookTarget;
    }

    public void MoveToAI()
    {
        currentRigPosition = aiPosition;
        currentLookTarget = aiLookTarget;
    }

    public void SetPlayerLookTarget(Transform target)
    {
        playerLookTarget = target;
    }

    public void SetAILookTarget(Transform target)
    {
        aiLookTarget = target;
    }

}
