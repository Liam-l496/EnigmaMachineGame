using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInteraction : MonoBehaviour
{

    public static PlayerInteraction Instance;

    public float playerReach = 1000f;
    private Camera mainCamera;
    public Interactable previousInteractable;
    public Interactable currentInteractable;
    public bool setByPlayerPickUpScript = false;
    public bool mouseClick = false;

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
        // If Player Pick Up Script did not say anything
        if (!setByPlayerPickUpScript) {
            currentInteractable = CurrentInteractableSelected();
            CheckSameInteractable();
            previousInteractable = currentInteractable;
            mouseClick = Input.GetMouseButtonDown(0);
        }
        else
        {
            setByPlayerPickUpScript = false;
        }

        // If mouse held down and object picked up, then rotate
        if (Input.GetMouseButton(0) && playerPickUpScript.heldObject != null)
        {
            playerPickUpScript.RotateObject();
        }

        // No mouse input, then return
        if (!mouseClick) return;

        // If nothing looked at, then put down object and return
        if (currentInteractable == null)
        {
            if (playerPickUpScript.heldObject) playerPickUpScript.PutDownObject();
            return;
        }
        // If no object held and object can be picked up, then pick up
        if (!playerPickUpScript.heldObject && currentInteractable.canPickUp)
        {
            playerPickUpScript.PickUpObject(currentInteractable);
        }

        // If same object clicked
        else if (currentInteractable == playerPickUpScript.heldObject)
        {
            playerPickUpScript.ClickSameObject();
        }

        // If something else interactable clicked, then let another script decide outcome
        else if (currentInteractable != playerPickUpScript.heldObject && playerPickUpScript.heldObject)
        {
            playerPickUpScript.somethingElseClicked = true;
            playerPickUpScript.anotherInteractableClicked = currentInteractable;
        }

        // If something cannot be picked up but interacted
        else if (!currentInteractable.canPickUp)
        {
            print("INTERACTING");
            currentInteractable.Interact();
        }
    }

    private void CheckSameInteractable()
    {
        if (previousInteractable == currentInteractable)
        {
            return;   
        }
        if (previousInteractable) previousInteractable.DisableOutline();
        if (currentInteractable) currentInteractable.EnableOutline();
    }

    private Interactable GetParent(Interactable currentlyInteracting)
    {
        if (currentlyInteracting != null && currentlyInteracting.transform.parent != null)
        {
            return currentlyInteracting.transform.parent.GetComponent<Interactable>();
        }
        return null;
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
}