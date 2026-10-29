

    // void Update()
    // {
    //     if (currentInteractable == null || currentInteractable != null && !currentInteractable.currentlyPickedUp) {
    //         // Interacting with objects that cannot be picked up
    //         CheckInteraction();
    //     }
    //     if (inPickUpAnimation)
    //     {
    //         PickUpObjectAnimation(animationEndPosition);
    //         return;
    //     }
    //     if (Input.GetMouseButtonDown(0) && currentInteractable != null)
    //     // Click on what is currently highlighted
    //     {
    //         if (currentInteractable.canPickUp && !currentInteractable.currentlyPickedUp)
    //         // Logic for picking things up
    //         // Need to be on pickUp boolean + currently not picked up
    //         // Need a state for putting it down
    //             // When clicking elsewhere
    //         {
    //             PickUpObject(currentInteractable);
    //         }
    //         else if (currentInteractable.canPickUp && currentInteractable.currentlyPickedUp)
    //         {
    //             // Put down the object
    //             // Already picked up
    //             Interactable currentlyInteracting = CurrentInteractableSelected();

    //             if (currentlyInteracting != heldObject)
    //             {
    //                 // Only put down when the object is not looked at and clicked
    //                 PutDownObject();
    //             }
    //             else
    //             {
    //                 // Logic for interacting with same object
    //                 print("Clicking on the same object");
    //             }
    //         }
    //         else {
    //             // Object cannot be picked up but just interacted
    //             print("INTERACTING");
    //             currentInteractable.Interact();
    //         }
    //     }
    //     else if (Input.GetMouseButton(0) && heldObject != null)
    //     {
    //         RotateObject();
    //     }
    //     else if (currentInteractable != null)
    //     {
    //         currentInteractable.StopInteract();
    //     }
    // }





////////////////////////////////////////



using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInteraction : MonoBehaviour
{

    public static PlayerInteraction Instance;

    public float playerReach = 1000f;
    private Camera mainCamera;
    public Interactable currentInteractable;

    [Header("Hold Position")]
    [SerializeField] Transform holdPosition;
    
    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Pick UP script
        LayerNumber = LayerMask.NameToLayer("HoldLayer");
    }

    void Update()
    {
        if (currentInteractable == null || currentInteractable != null && !currentInteractable.currentlyPickedUp) {
            // Interacting with objects that cannot be picked up
            CheckInteraction();
        }
        if (inPickUpAnimation)
        {
            PickUpObjectAnimation(animationEndPosition);
            return;
        }
        if (Input.GetMouseButtonDown(0) && currentInteractable != null)
        // Click on what is currently highlighted
        {
            if (currentInteractable.canPickUp && !currentInteractable.currentlyPickedUp)
            // Logic for picking things up
            // Need to be on pickUp boolean + currently not picked up
            // Need a state for putting it down
                // When clicking elsewhere
            {
                PickUpObject(currentInteractable);
            }
            else if (currentInteractable.canPickUp && currentInteractable.currentlyPickedUp)
            {
                // Put down the object
                // Already picked up
                Interactable currentlyInteracting = CurrentInteractableSelected();

                if (currentlyInteracting != heldObject)
                {
                    // Only put down when the object is not looked at and clicked
                    PutDownObject();
                }
                else
                {
                    // Logic for interacting with same object
                    print("Clicking on the same object");
                }
            }
            else {
                // Object cannot be picked up but just interacted
                print("INTERACTING");
                currentInteractable.Interact();
            }
        }
        else if (Input.GetMouseButton(0) && heldObject != null)
        {
            RotateObject();
        }
        else if (currentInteractable != null)
        {
            currentInteractable.StopInteract();
        }
    }

    
    // Update is called once per frame
    // void Update()
    // {
    //     if (currentInteractable == null || currentInteractable != null && !currentInteractable.currentlyPickedUp) {
    //         // Interacting with objects that cannot be picked up
    //         CheckInteraction();
    //     }
    //     if (inPickUpAnimation)
    //     {
    //         PickUpObjectAnimation(animationEndPosition);
    //         return;
    //     }
    //     if (Input.GetMouseButtonDown(0) && currentInteractable != null)
    //     // Click on what is currently highlighted
    //     {
    //         if (currentInteractable.canPickUp && !currentInteractable.currentlyPickedUp)
    //         // Logic for picking things up
    //         // Need to be on pickUp boolean + currently not picked up
    //         // Need a state for putting it down
    //             // When clicking elsewhere
    //         {
    //             PickUpObject(currentInteractable);
    //         }
    //         else if (currentInteractable.canPickUp && currentInteractable.currentlyPickedUp)
    //         {
    //             // Put down the object
    //             // Already picked up
    //             Interactable currentlyInteracting = CurrentInteractableSelected();
    //             Interactable parent = GetParent(currentlyInteracting);
                
                
    //             if (parent == heldObject)
    //             {
    //                 print("Clicking on a child object");
    //                 currentlyInteracting.Interact();
    //             }
    //             else if (currentlyInteracting != heldObject)
    //             {
    //                 print(currentInteractable);
    //                 // Only put down when the object is not looked at and clicked
    //                 PutDownObject();
    //             }
    //             else
    //             {
    //                 // Logic for interacting with same object
    //                 print("Clicking on the same object");
    //             }
    //         }
    //         else {
    //             // Object cannot be picked up but just interacted
    //             print("INTERACTING");
    //             currentInteractable.Interact();
    //         }
    //     }
    //     else if (Input.GetMouseButton(0) && heldObject != null)
    //     {
    //         Interactable currentlyInteracting = CurrentInteractableSelected();
    //         Interactable parent = GetParent(currentlyInteracting);
    //         if (parent != heldObject)
    //         {
    //             RotateObject(); 
    //         }
    //     }
    //     else if (currentInteractable != null)
    //     {
    //         currentInteractable.StopInteract();
    //     }
    // }

    private Interactable GetParent(Interactable currentlyInteracting)
    {
        if (currentlyInteracting != null && currentlyInteracting.transform.parent != null)
        {
            return currentlyInteracting.transform.parent.GetComponent<Interactable>();
        }
        return null;
    }

    private void CheckInteraction()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Draws a 10-unit line in the Scene view while running
        Debug.DrawRay(ray.origin, ray.direction * playerReach, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, playerReach))
        {
            // Debug.Log($"Ray hit object: {hit.collider.gameObject.name} with Tag: {hit.collider.tag}");

            if (hit.collider.CompareTag("Interactable"))
            {
                Interactable newInteractable = hit.collider.GetComponent<Interactable>();

                if (newInteractable != null && newInteractable.enabled)
                {
                    if (currentInteractable != newInteractable)
                    {
                        DisableCurrentInteractable();
                        SetNewCurrentInteractable(newInteractable);
                    }
                    return;
                }
                else
                {
                    // Debug.LogWarning("Hit object with 'Interactable' tag, but missing the Interactable script!");
                }
            }
        }
        DisableCurrentInteractable();
    }

    private Interactable CurrentInteractableSelected()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Draws a 10-unit line in the Scene view while running
        Debug.DrawRay(ray.origin, ray.direction * playerReach, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, playerReach))
        {
            // Debug.Log($"Ray hit object: {hit.collider.gameObject.name} with Tag: {hit.collider.tag}");

            if (hit.collider.CompareTag("Interactable"))
            {
                Interactable newInteractable = hit.collider.GetComponent<Interactable>();
                return newInteractable;
            }
        }
        return null;
    } 

    void SetNewCurrentInteractable(Interactable newInteractable)
    {
        currentInteractable = newInteractable;
        currentInteractable.EnableOutline();
    }

    void DisableCurrentInteractable()
    {
        if (currentInteractable)
        {
            currentInteractable.DisableOutline();
            currentInteractable = null;
        }
    }



    ///
    /// Pick UP Script
    /// 
    public float rotationSensitivity;
    private Interactable heldObject; //object which we pick up
    private Transform heldObjectTransform; //rigidbody of object we pick up
    private Vector3 originalHeldPosition;
    private Quaternion originalHeldRotation;

    public Vector3 XaxisVector;
    public Vector3 YaxisVector;

    private int LayerNumber; //layer index

    /// <summary>
    ///  Pick Up Animation
    /// </summary>
    public bool inPickUpAnimation = false;
    public float smoothTime = 0.3f;
    public float smoothTimeRotation = 0.5f;
    private Vector3 currentVelocity;
    private float stoppingDistance = 0.1f;
    private Vector3 animationEndPosition;


    private void PickUpObjectAnimation(Vector3 endPosition)
    {
        heldObjectTransform.position = Vector3.SmoothDamp(
            heldObjectTransform.position,
            endPosition,
            ref currentVelocity,
            smoothTime
        );

        heldObjectTransform.rotation = Quaternion.Slerp(
            heldObjectTransform.rotation,
            originalHeldRotation,
            Time.deltaTime * smoothTimeRotation
        );

        float distance = Vector3.Distance(heldObjectTransform.position, endPosition);
        float angle = Quaternion.Angle(heldObjectTransform.rotation, originalHeldRotation);
        if (distance <= stoppingDistance && angle <= 0.5f)
        {
            // Snap to exact target position
            heldObjectTransform.position = endPosition;
            heldObjectTransform.rotation = originalHeldRotation;
            
            // Reset velocity and stop processing
            currentVelocity = Vector3.zero;
            inPickUpAnimation = false;
        }
        if (!inPickUpAnimation && endPosition != holdPosition.position)
        {
            heldObject = null;
            heldObjectTransform = null;
        }
    }

    void PickUpObject(Interactable pickUpObj)
        {
            heldObject = pickUpObj; //assign heldObj to the object that was hit by the raycast (no longer == null)
            heldObjectTransform = pickUpObj.GetComponent<Transform>(); //assign Rigidbody
            
            originalHeldPosition = heldObjectTransform.position;
            originalHeldRotation = heldObjectTransform.rotation;
            inPickUpAnimation = true;
            animationEndPosition = holdPosition.position;
            // heldObjectTransform.position = holdPosition.position; //parent object to holdposition
            heldObject.gameObject.layer = LayerNumber; //change the object layer to the holdLayer
            heldObject.UpdatePickUpState(true);
        }

    void PutDownObject()
    {
        if (heldObject == null)
        {
            return;
        }
        heldObject.UpdatePickUpState(false);
        animationEndPosition = originalHeldPosition;
        // heldObjectTransform.position = originalHeldPosition;
        // heldObjectTransform.rotation = originalHeldRotation;
        // heldObject = null;
        // heldObjectTransform = null;
        inPickUpAnimation = true;
    }

    void RotateObject()
    {
        if (heldObject == null)
        {
            return;
        }
        float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
        float YaxisRotation = -Input.GetAxis("Mouse Y") * rotationSensitivity;
        //rotate the object depending on mouse X-Y Axis
        heldObjectTransform.Rotate(Vector3.forward, XaxisRotation, Space.World);
        heldObjectTransform.Rotate(Vector3.right, YaxisRotation, Space.World);
    }





}




/////////////////// Unedited code

// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEngine;
// using UnityEngine.EventSystems;

// public class PlayerInteraction : MonoBehaviour
// {
//     public float playerReach = 1000f;
//     private Camera mainCamera;
//     Interactable currentInteractable;
    
//     private void Awake()
//     {
//         mainCamera = Camera.main;
//         if (mainCamera == null)
//         {
//             mainCamera = FindFirstObjectByType<Camera>();
//         }
//     }
    
    
//         // Update is called once per frame
//     void Update()
//     {
//         CheckInteraction();
//         if (Input.GetMouseButtonDown(0) && currentInteractable != null)
//         {
//             currentInteractable.Interact();
//         }
//         else if (currentInteractable != null)
//         {
//             currentInteractable.StopInteract();
//         }
//     }
//     private void CheckInteraction()
//     {
//         Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

//         // Draws a 10-unit line in the Scene view while running
//         Debug.DrawRay(ray.origin, ray.direction * playerReach, Color.red);

//         if (Physics.Raycast(ray, out RaycastHit hit, playerReach))
//         {
//             // Debug.Log($"Ray hit object: {hit.collider.gameObject.name} with Tag: {hit.collider.tag}");

//             if (hit.collider.CompareTag("Interactable"))
//             {
//                 Interactable newInteractable = hit.collider.GetComponent<Interactable>();

//                 if (newInteractable != null && newInteractable.enabled)
//                 {
//                     if (currentInteractable != newInteractable)
//                     {
//                         DisableCurrentInteractable();
//                         SetNewCurrentInteractable(newInteractable);
//                     }
//                     return;
//                 }
//                 else
//                 {
//                     // Debug.LogWarning("Hit object with 'Interactable' tag, but missing the Interactable script!");
//                 }
//             }
//         }

//         DisableCurrentInteractable();
//     }



//     void SetNewCurrentInteractable(Interactable newInteractable)
//     {
//         currentInteractable = newInteractable;
//         currentInteractable.EnableOutline();
//     }

//     void DisableCurrentInteractable()
//     {
//         if (currentInteractable)
//         {
//             currentInteractable.DisableOutline();
//             currentInteractable = null;
//         }
//     }

// }
