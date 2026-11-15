// using System.Collections;
// using System.Collections.Generic;
// using JetBrains.Annotations;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.UIElements;

// public class Interactable : MonoBehaviour
// {
//     private Outline outline;

//     public UnityEvent onInteraction;
//     public UnityEvent onStopInteraction;

//     public float minDistance = 1f;
//     public float maxDistance = 10f;

//     public bool isSelected = false;
//     public bool runInitialScript = false;
//     public bool runEndingScript = false;

//     public bool canPickUp = false;
//     public bool currentlyPickedUp = false;
//     // Start is called before the first frame update

//     void Awake()
//     {
//         // if (!gameObject.GetComponent<Outline>()) outline = gameObject.AddComponent<Outline>();
//         // gameObject.tag = "Interactable";
//         gameObject.layer = LayerMask.NameToLayer("InteractableLayer");
//     }


//     private void Start()
//     {
//         TryAttachOutline();
//         DisableOutline();
//     }


//     private bool CanSafelyAddOutline()
//     {
//         bool hasAtLeastOneValidMesh = false;

//         // 1. Check standard MeshFilters
//         foreach (var mf in GetComponentsInChildren<MeshFilter>(true))
//         {
//             if (mf.sharedMesh == null)
//                 return false; // Mesh not generated/assigned yet

//             if (!mf.sharedMesh.isReadable)
//             {
//                 Debug.LogWarning($"[Interactable] '{name}' skipped Outline: '{mf.sharedMesh.name}' needs Read/Write enabled.", this);
//                 return false;
//             }

//             hasAtLeastOneValidMesh = true;
//         }

//         // 2. Check SkinnedMeshRenderers (for rigged/animated objects)
//         foreach (var smr in GetComponentsInChildren<SkinnedMeshRenderer>(true))
//         {
//             if (smr.sharedMesh == null)
//                 return false;

//             if (!smr.sharedMesh.isReadable)
//             {
//                 Debug.LogWarning($"[Interactable] '{name}' skipped Outline: skinned mesh '{smr.sharedMesh.name}' needs Read/Write enabled.", this);
//                 return false;
//             }

//             hasAtLeastOneValidMesh = true;
//         }

//         return hasAtLeastOneValidMesh;
//     }

//     private void TryAttachOutline()
//     {
//         if (outline == null && !TryGetComponent(out outline) && CanSafelyAddOutline())
//         {
//             outline = gameObject.AddComponent<Outline>();
//         }
//     }

//     public void Interact()
//     {
//         onInteraction.Invoke();
//         isSelected = true;
//     }

//     public void StopInteract()
//     {
//         onStopInteraction.Invoke();
//     }

//     public void DisableOutline()
//     {
//         if (outline != null)
//             outline.enabled = false;
//     }

//     public void EnableOutline()
//     {
//         TryAttachOutline();

//         if (outline != null)
//             outline.enabled = true;
//     }

//     public void UpdateCanPickUp(bool newState)
//     {
//         canPickUp = newState;
//     }

//     public void UpdatePickUpState(bool newState)
//     {
//         currentlyPickedUp = newState;
//         // GetComponent<BoxCollider>().enabled = !newState;
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("InteractableLayer");
    }

    private void Start()
    {
        TryAttachOutline();
        DisableOutline();
    }

    // private void TryAttachOutline()
    // {
    //     // Check whether THIS GameObject already has an Outline.
    //     if (outline == null)
    //     {
    //         outline = GetComponent<Outline>();
    //     }

    //     // Only add an Outline to THIS GameObject if one does not already exist.
    //     if (outline == null && CanSafelyAddOutline())
    //     {
    //         outline = gameObject.AddComponent<Outline>();
    //     }
    // }

    private void TryAttachOutline()
    {
        if (outline != null)
            return;

        outline = GetComponent<Outline>();

        if (outline != null)
            return;

        outline = gameObject.AddComponent<Outline>();

        Debug.Log($"[Interactable] Outline added to '{name}'.", this);
    }

    private bool CanSafelyAddOutline()
    {
        bool hasAtLeastOneValidMesh = false;

        foreach (var mf in GetComponentsInChildren<MeshFilter>(true))
        {
            if (mf.sharedMesh == null)
                return false;

            if (!mf.sharedMesh.isReadable)
            {
                Debug.LogWarning($"[Interactable] '{name}' skipped Outline: '{mf.sharedMesh.name}' needs Read/Write enabled.", this);
                return false;
            }

            hasAtLeastOneValidMesh = true;
        }

        foreach (var smr in GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            if (smr.sharedMesh == null)
                return false;

            if (!smr.sharedMesh.isReadable)
            {
                Debug.LogWarning($"[Interactable] '{name}' skipped Outline: skinned mesh '{smr.sharedMesh.name}' needs Read/Write enabled.", this);
                return false;
            }

            hasAtLeastOneValidMesh = true;
        }

        return hasAtLeastOneValidMesh;
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
        if (outline != null)
            outline.enabled = false;
    }

    public void EnableOutline()
    {
        TryAttachOutline();

        if (outline != null)
            outline.enabled = true;
    }

    public void UpdateCanPickUp(bool newState)
    {
        canPickUp = newState;
    }

    public void UpdatePickUpState(bool newState)
    {
        currentlyPickedUp = newState;
    }
}