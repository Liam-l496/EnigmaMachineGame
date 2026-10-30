using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.AI;
using System.IO.Pipes;
using System;
using Cinemachine;
using UnityEditorInternal;

public class RotorMasterScript : MonoBehaviour
{

    [Header("Other Components")]
    public Interactable interactable;
    public RotorScript rotorScript;
    public RotorSwappingScript rotorSwappingScript;
    public RotorEncryptionScript rotorEncryptionScript;


    // [Header("Interacting Variables")]
    // Might need IsSelected update function
    public bool IsSelected { get; set; } = false;
    public bool InAnimation { get;  set; } = false;
    public bool BlackFlapUp { get;  set; } = false;
    public bool ReflectorHolding { get; set; } = true;
    public bool WindowFlapUp { get; set; } = false;
    public bool Transitioning { get; set; } = false;
    public int rotorPosition;


    // For Animation
    [Header("Animation")]
    public Vector3 currentVelocity;
    public Vector3 endPosition; 
    public float stoppingDistance = 0.01f;
    public float smoothTime = 0.5f;
    

    void Start()
    {
        interactable = GetComponent<Interactable>();
        rotorScript = GetComponent<RotorScript>();
        rotorSwappingScript = GetComponent<RotorSwappingScript>();
        rotorEncryptionScript  = GetComponent<RotorEncryptionScript>();
        // = GetComponent<>();
    }

    void LateUpdate()
    {
        if (IsInAnimation())
        {
            RotorAnimation(endPosition);
            return;
        }
    }

    /// <summary>
    /// Encrypt the text 
    /// </summary>
    public int GetPlainToCipher(int plainChar)
    {
        return rotorEncryptionScript.PlainToCipher(plainChar, rotorScript.rotorValue);
    }

    public int GetCipherToPlain(int cipherChar)
    {
        return rotorEncryptionScript.CipherToPlain(cipherChar, rotorScript.rotorValue);
    }



    /// <summary>
    /// Animation Function
    /// </summary> 
    public bool IsInAnimation()
    {
        if (InAnimation && transform.position != endPosition)
        {
            return true;
        }
        if (transform.position == endPosition)
        {
            InAnimation = false;
        }
        return false;
    }

    private void RotorAnimation(Vector3 endPosition)
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            endPosition,
            ref currentVelocity,
            smoothTime
        );

        float distance = Vector3.Distance(transform.position, endPosition);
        if (distance <= stoppingDistance)
        {
            // Snap to exact target position
            transform.position = endPosition;
            
            // Reset velocity and stop processing
            currentVelocity = Vector3.zero;
            InAnimation = false;
        }
    }

    public void UpdateEndPosition(Transform newEndPositionTransform)
    {
        endPosition = newEndPositionTransform.position;
    }    

    /// <summary>
    /// Updating States
    /// </summary>

    public void UpdateFlapUpState(bool flapUp)
    {
        BlackFlapUp = flapUp;
        // rotorScript.enabled = !flapUp;
        if (flapUp)
        {
            rotorScript.enabled = false;
            interactable.UpdateCanPickUp(true);
        }
        else
        {
            rotorScript.enabled = true;
            interactable.UpdateCanPickUp(false);
        }
    }

    public void UpdateReflectorHoldingState(bool reflectorHolding)
    {
        ReflectorHolding = reflectorHolding;
        interactable.UpdateCanPickUp(reflectorHolding);
        if (!reflectorHolding)
        {
            rotorSwappingScript.enabled = true;
            rotorSwappingScript.RotorSwappingAnimationState(reflectorHolding);
        }
        else
        {
            rotorSwappingScript.RotorSwappingAnimationState(reflectorHolding);
            rotorSwappingScript.enabled = false;
        }
    }

    public void UpdateWindowFlapUpState(bool windowFlapUp)
    {
        WindowFlapUp = windowFlapUp;
        if (windowFlapUp)
        {
            rotorScript.enabled = true;
            interactable.UpdateCanPickUp(false);
        }
        else
        {
            rotorScript.enabled = false;
        }
    }

    public void InTransitionAnimation(bool transitioning)
    {
        Transitioning = transitioning;
        if (Transitioning)
        {
            print("Transition now");
        } 
        else
        {
            print("Transition stopped now");
        }
    }


    private void OnEnable()
    {
        AnimationHandlerScript.BlackCoverFlapUp += UpdateFlapUpState;
        AnimationHandlerScript.ReflectorHolding += UpdateReflectorHoldingState;
        AnimationHandlerScript.WindowFlapUp += UpdateWindowFlapUpState;
        AnimationHandlerScript.OnBlendStateChanged -= InTransitionAnimation;
    }

    private void OnDisable()
    {
        AnimationHandlerScript.BlackCoverFlapUp -= UpdateFlapUpState;
        AnimationHandlerScript.ReflectorHolding -= UpdateReflectorHoldingState;
        AnimationHandlerScript.WindowFlapUp -= UpdateWindowFlapUpState;
        AnimationHandlerScript.OnBlendStateChanged -= InTransitionAnimation;
    }
}