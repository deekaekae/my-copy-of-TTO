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

    [Header("Eye Control")]
    public Transform[] eyeTransforms; //for announcer eyes
    public float eyeLookSpeed = 5f;
    public float maxEyeAngle = 30f;


    private void Start()
    {
        mainCam = Camera.main;
        MoveToAnnouncer();
    }

    private void Update()
    {
        if (currentRigPosition != null){
            transform.position = Vector3.Lerp(transform.position, currentRigPosition.position, Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, currentRigPosition.rotation, Time.deltaTime * moveSpeed);
        }

        if (mainCam != null && currentLookTarget != null){
            Vector3 direction = currentLookTarget.position - mainCam.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
        }

        if (currentLookTarget != null && eyeTransforms != null){
            foreach (var eye in eyeTransforms){
                if (eye == null) continue;

                Vector3 toTarget = currentLookTarget.position - eye.position;
                Quaternion lookRotation = Quaternion.LookRotation(toTarget);

                // Convert to local rotation relative to head
                Quaternion localRotation = Quaternion.Inverse(eye.parent.rotation) * lookRotation;

                // Limit rotation
                localRotation = Quaternion.RotateTowards(
                    Quaternion.identity,  // base eye forward
                    localRotation,
                    maxEyeAngle
                );

                eye.localRotation = Quaternion.Slerp(
                    eye.localRotation,
                    localRotation,
                    Time.deltaTime * eyeLookSpeed
                );
            }
        }

    }

    public void MoveToAnnouncer(){
        currentRigPosition = announcerPosition;
        currentLookTarget = announcerLookTarget;
    }

    public void MoveToPlayer(){
        currentRigPosition = playerPosition;
        currentLookTarget = playerLookTarget;
    }

    public void MoveToAI(){
        currentRigPosition = aiPosition;
        currentLookTarget = aiLookTarget;
    }

    public void SetPlayerLookTarget(Transform target){
        playerLookTarget = target;
    }

    public void SetAILookTarget(Transform target){
        aiLookTarget = target;
    }

    
}
