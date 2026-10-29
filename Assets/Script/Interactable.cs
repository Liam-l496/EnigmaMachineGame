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

    public bool canPickUp = false;
    public bool currentlyPickedUp = false;
    // Start is called before the first frame update
    void Start()
    {
        outline = gameObject.AddComponent<Outline>();
        gameObject.tag = "Interactable";
        DisableOutline();
    }

    public void Interact()
    {
        onInteraction.Invoke();
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
