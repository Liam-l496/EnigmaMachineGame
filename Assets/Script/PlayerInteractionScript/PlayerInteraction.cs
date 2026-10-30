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

    [Header("PlayerPickUpScripts")]
    [SerializeField] PlayerPickUpScript playerPickUpScript;
    
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
        playerPickUpScript = GetComponent<PlayerPickUpScript>();
    }

    void Update()
    {
        if (AnimationHandlerScript.IsBlending || playerPickUpScript.inAnimation)
        {
            // print("IN ANIMATION");
            return;
        }

        if (currentInteractable == null || currentInteractable != null && !currentInteractable.currentlyPickedUp) {
            // Holding nothings
            // Interacting but not been picked up
            print("CHECKING INTERACTION");
            // Updates what is currently highlighted and not
            CheckInteraction();
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
                // print("STATE 1");
                playerPickUpScript.PickUpObject(currentInteractable);
            }
            else if (currentInteractable.canPickUp && currentInteractable.currentlyPickedUp)
            {
                // print("STATE 2");
                // Put down the object
                // Already picked up
                Interactable currentlyInteracting = CurrentInteractableSelected();

                if (currentlyInteracting != playerPickUpScript.heldObject)
                {
                    // print("STATE 2.1");
                    // Only put down when the object is not looked at and clicked
                    playerPickUpScript.PutDownObject();
                }
                else
                {
                    // print("STATE 2.2");
                    // Logic for interacting with same object
                    playerPickUpScript.ClickSameObject();
                }
            }
            else {
                // Object cannot be picked up but just interacted
                // print("INTERACTING STATE 3");
                currentInteractable.Interact();
            }
        }
        else if (Input.GetMouseButton(0) && playerPickUpScript.heldObject != null)
        {
            // print("STATE 4");
            playerPickUpScript.RotateObject();
        }
        else if (currentInteractable != null)
        {
            // print("STATE 5");
            currentInteractable.StopInteract();
        }
    }

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

}