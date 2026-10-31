using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Interactable : MonoBehaviour
{
    private Outline outline;

    public UnityEvent onInteraction;
    public UnityEvent onStopInteraction;

    public float minDistance = 1f;
    public float maxDistance = 10f;

    public bool isSelected = false;
    public bool runInitialScript = false;
    public bool runEndingScript = false;

    public bool canPickUp = false;
    public bool currentlyPickedUp = false;
    // Start is called before the first frame update

    void Awake()
    {
        outline = gameObject.AddComponent<Outline>();
        gameObject.tag = "Interactable";
        DisableOutline();   
    }
    void Start()
    {
    }

    public void Interact()
    {
        onInteraction.Invoke();
        isSelected = true;
    }

    public void StopInteract()
    {
        onStopInteraction.Invoke();
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void UpdateCanPickUp(bool newState)
    {
        canPickUp = newState;
    }

    public void UpdatePickUpState(bool newState)
    {
        currentlyPickedUp = newState;
        // GetComponent<BoxCollider>().enabled = !newState;
    }
}
