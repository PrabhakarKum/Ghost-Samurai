using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public PlayerManager playerManager;
    public Camera cameraObject;
    [SerializeField] Transform cameraPivotTransform;

    [Header("Camera Settings")]
    private Vector3 cameraVelocity;
    [SerializeField] private float leftAndRightRotationSpeed = 100;
    [SerializeField] private float upAndDownRotationSpeed = 100;
    [SerializeField] private float minimumPivot = -30f; //THE LOWEST POINT YOU ARE ABLE TO LOOK DOWN
    [SerializeField] private float maximumPivot = 60f;  //THE HIGHEST POINT YOU ARE ABLE TO LOOK UP
    [SerializeField] private float cameraCollisionRadius = 0.2f;
    [SerializeField] private LayerMask collideWithLayer;

    [Header("Camera values")]
    [SerializeField] private float leftAndRightLookAngle;
    [SerializeField] private float upAndDownLookAngle;
    [SerializeField] private float cameraSmoothSpeed = 1;
    private Vector3 cameraObjectPosition; // USED FOR CAMERA COLLISION ( MOVES THE CAMERA OBJECT TO THIS POSITION UPON COLLIDING)
    private float cameraZPosition;  //VALUES USED FOR CAMERA COLLISION
    private float targetCameraZPosition;   //VALUES USED FOR CAMERA COLLISION
    
    [Header("Lock On")]
    [SerializeField] private float lockOnRadius = 20f;
    [SerializeField] private float minimumViewableAngle = -50f;
    [SerializeField] private float maximumViewableAngle = 50f;
    [SerializeField] private float lockOnTargetFollowSpeed = 0.2f;
    [SerializeField] private float setCameraHeightSpeed = 1f;
    [SerializeField] private float unlockedCameraHeight = 1.5f;
    [SerializeField] private float unlockedZoomIn = 0f;
    [SerializeField] private float lockedCameraHeight = 3f;
    [SerializeField] private float lockedZoomOut = 1.5f;
    
    private Coroutine cameraLockOnHeigthCoroutine;
    private List<CharacterManager> availableTargets = new List<CharacterManager>();
    public CharacterManager  nearestLockOnTarget;
    public CharacterManager  leftLockOnTarget;
    public CharacterManager  rightLockOnTarget;



    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleCameraActions()
    {
        if(playerManager !=  null)
        {
            HandleFollowTarget();
            HandleRotation();
            HandleCollisions();

        }
    }
    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, playerManager.transform.position, ref cameraVelocity, cameraSmoothSpeed);
        transform.position = targetCameraPosition;
    }

    private void HandleRotation()
    {
        // IF LOCKED ON, FORCE ROTATION TOWARDS TARGET
        if (playerManager.isLockedOn)
        {
            // THIS ROTATES THIS GAME OBJECT (LEFT AND RIGHT)
            Vector3 rotationDirection = playerManager._playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - transform.position;
            rotationDirection.Normalize();
            rotationDirection.y = 0;
            
            quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed *Time.deltaTime);
            
            // THIS ROTATES PIVOT OBJECT(UP AND DOWN)
            rotationDirection = playerManager._playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - cameraPivotTransform.position;
            rotationDirection.Normalize();
            
            targetRotation = Quaternion.LookRotation(rotationDirection);
            cameraPivotTransform.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed *Time.deltaTime);
            
            // SAVE OUR ROTATION TO OUR LOOK ANGLE, SO WHEN WE UNLOCK IT DOES NOT SNAP FAR AWAY
            leftAndRightLookAngle = transform.rotation.eulerAngles.y;
            upAndDownLookAngle = transform.rotation.eulerAngles.x;
            
        }
        else // ELSE ROTATE REGULARLY
        {
            // NORMAL ROTATION
            // ROTATE LEFT AND RIGHT BASED ON HORIZONTAL ON THE MOUSE
            leftAndRightLookAngle += (PlayerInputManager._instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;

            // ROTATE UP AND DOWN BASED ON VERTICAL ON THE MOUSE
            upAndDownLookAngle -= (PlayerInputManager._instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;

            // CLAMP THE UP AND DOWN LOOK ANGLER BETWEEN MIN AND MAX
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);


            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;

            // ROTATE THIS GAME OBJECT LEFT AND RIGHT
            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation; 


            // ROTATE THE PIVOT GAME OBJECT UP AND DOWN
            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }
    }

    private void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition;
        RaycastHit hit;

        // DIRECTION FOR THE COLLISION TO CHECK 
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        // WE CHECK IF THERE IS ANY OBJECT IN FRONT OF OUR DESIRED DIRECTION * 
        if(Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayer))
        {
            //IF THERE IS ANY OBJECT , WE GET OUR DISTANCE FROM IT
            Debug.DrawRay(cameraPivotTransform.position, direction * Mathf.Abs(targetCameraZPosition), Color.green);
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);

            // WE THEN EQUATE OUR DISTANCE FROM IT
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }
        
        // IF OUR TARGET POSITION IS LESS THAN OUR COLLISION RADIUS, WE SUBSTRACT OUR COLLISION RADIUS
        if(Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        // WE THEN APPLY OUR FINAL POSITION USING A LERP OVER A TIME OF 0.2F SECONDS
        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;
        
    }

    void OnDrawGizmos()
    {
        // Draw a wireframe sphere to visualize the SphereCast
        Gizmos.color = Color.red; // Set the color to red
        Gizmos.DrawWireSphere(cameraPivotTransform.position, cameraCollisionRadius); // Draw the sphere
    }

    public void HandleLocatingLockOnTargets()
    {
        float shortestDistance = Mathf.Infinity; //WILL BE USED TO DETERMINE THE TARGET CLOSEST TO US
        float shortestDistanceOfRightTarget = Mathf.Infinity; // WILL BE USED TO DETERMINE SHORTEST DISTANCE ON ONE AXIS TO THE RIGHT OF CURRENT TARGET( closest target to the right of the current target)
        float shortestDistanceOfLeftTarget = -Mathf.Infinity; // WILL BE USED TO DETERMINE SHORTEST DISTANCE ON ONE AXIS TO THE LEFT OF CURRENT TARGET
        
        // to do
        Collider[] colliders = Physics.OverlapSphere(playerManager.transform.position, lockOnRadius, WorldUtiityManagers.Instance.GetCharacterLayers());

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();
            if (lockOnTarget != null)
            {
                // CHECK IF THEY ARE WITHIN OUR FOV
                Vector3 lockOnTargetDirection = lockOnTarget.transform.position - playerManager.transform.position;
                float lockOnTargetDistance = Vector3.Distance(playerManager.transform.position, lockOnTarget.transform.position);
                float viewableAngle = Vector3.Angle(lockOnTargetDirection, cameraObject.transform.forward);

                // IF TARGET IS DEAD, CHECK THE NEXT POTENTIAL TARGET 
                if (lockOnTarget.isDead)
                    continue;
                
                // IF TARGET IS US, CHECK THE POTENTIAL TARGET
                if (lockOnTarget.transform.root == playerManager.transform.root)
                    continue;
                
                //IF THE TARGET IS FOV OR BLOCKED BY ENVIRONMENT, CHECK NEXT POTENTIAL TARGET
                if (viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)
                {
                    RaycastHit hit;
                    //TODO ADD LAYER-MASK FOR ENVIRONMENT LAYER
                    if (Physics.Linecast(playerManager._playerCombatManager.lockOnTransform.position, lockOnTarget.characterCombatManager.lockOnTransform.position, out hit, WorldUtiityManagers.Instance.GetEnvironmentLayers()))
                    {
                        // WE HIT SOMETHING, WE CANNOT SEE OUR LOCK ON TARGET
                        continue;
                    }
                    else
                    {
                        // OTHERWISE, ADD THEM TO OUR POTENTIAL TARGET
                        availableTargets.Add(lockOnTarget);
                    }
                }
            }
        }
        
        // WE NOW SWAP THROUGH OUR POTENTIAL TARGETS TO SEE WHICH ONE WE LOCK ONTO FIRST
        for (int k = 0; k < availableTargets.Count; k++)
        {
            if (availableTargets[k] != null)
            {
                float distanceFromTarget = Vector3.Distance(playerManager.transform.position, availableTargets[k].transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[k];
                }

                if (playerManager.isLockedOn)
                {
                    Vector3 relativeEnemyPosition = playerManager.transform.InverseTransformPoint(availableTargets[k].transform.position);
                    var distanceFromLeftTarget = relativeEnemyPosition.x;
                    var distanceFromRightTarget = relativeEnemyPosition.x;
                    
                    if(availableTargets[k] != playerManager._playerCombatManager.currentTarget)
                        continue;

                    // CHECK THE LEFT SIDE FOR TARGETS
                    if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockOnTarget = availableTargets[k];
                    }
                    // CHECK THE RIGHT SIDE FOR TARGETS
                    else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        rightLockOnTarget = availableTargets[k];
                    }
                }
            }
            else
            {
                ClearLockedOnTargets();
                playerManager.isLockedOn = false;
            }
        }
    }

    public void SetLockCameraHeight()
    {
        if (cameraLockOnHeigthCoroutine != null)
        {
            StopCoroutine(cameraLockOnHeigthCoroutine);
        }

        cameraLockOnHeigthCoroutine = StartCoroutine(SetCameraHeight());

    }

    public void ClearLockedOnTargets()
    {
        nearestLockOnTarget = null;
        leftLockOnTarget = null;
        rightLockOnTarget = null;
        availableTargets.Clear();
    }

    public IEnumerator WaitThenFindNewTarget()
    {
        while (playerManager.isPerformingAction)
        {
            yield return null;
        }
        ClearLockedOnTargets();
        HandleLocatingLockOnTargets();

        if (nearestLockOnTarget != null)
        {
            playerManager._playerCombatManager.SetTarget(nearestLockOnTarget);
            playerManager.isLockedOn = true;
        }
        
        yield return null;
    }

    private IEnumerator SetCameraHeight()
    {
        float duration = 1;
        float timer = 0;
        
        Vector3 velocity = Vector3.zero;
        Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight, lockedZoomOut);
        Vector3 newUnLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, unlockedCameraHeight, unlockedZoomIn);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            if (playerManager != null)
            {
                if (playerManager._playerCombatManager.currentTarget != null)
                {
                    cameraPivotTransform.transform.localPosition = 
                        Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedCameraHeight, ref velocity, setCameraHeightSpeed);

                    cameraPivotTransform.transform.localRotation = 
                        Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
                }
                else
                {
                    cameraPivotTransform.transform.localPosition = 
                        Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnLockedCameraHeight, ref velocity, setCameraHeightSpeed);
                }
            }
            yield return null;
        }

        if (playerManager != null)
        {
            if (playerManager._playerCombatManager.currentTarget != null)
            {
                cameraPivotTransform.transform.localPosition = newLockedCameraHeight;
                cameraPivotTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                cameraPivotTransform.transform.localPosition = newUnLockedCameraHeight;
            }
        }
        
        yield return null;
    }
}
