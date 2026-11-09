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

public class RotorInteractScript: MonoBehaviour
{

    [Header("Left Side")]
    public GameObject sideLeft;
    private Transform transformLeft;
    private BoxCollider colliderLeft;
    public Transform OpenPositionLeft;
    public Transform ClosePositionLeft;

    [Header("Right Side")]
    public GameObject sideRight;
    private Transform transformRight;
    private BoxCollider colliderRight;
    public Transform OpenPositionRight;
    public Transform ClosePositionRight;

    [Header("Inner Tube")]
    public GameObject innerTube;
    private CapsuleCollider colliderInnerTube;

    [Header("Original Box Collider")]
    [SerializeField] BoxCollider colliderOriginal;
    [SerializeField] BoxCollider colliderSwapping;

    [Header("Cables")]
    [SerializeField] GameObject cables;


    [Header("Animation")]
    public bool inAnimationLeft = false;
    public bool inAnimationRight = false;

    private Vector3 endPositionLeft;
    private Vector3 endPositionRight;

    private Vector3 currentVelocityLeft;
    private Vector3 currentVelocityRight;

    [SerializeField] private float stoppingDistance = 0.01f;
    [SerializeField] private float smoothTime = 0.5f;

    [Header("Control Variables")]
    public bool open = false;

    [Header("Rotating Variables")]
    private Vector2? mouseClickPosition;
    private float maxDragDistance = 5f;
    private bool clickPermitted = false;



    public Interactable interactable;

    void Awake()
    {
        transformLeft = sideLeft.transform;
        colliderLeft = sideLeft.GetComponent<BoxCollider>();
        
        transformRight = sideRight.transform;
        colliderRight = sideRight.GetComponent<BoxCollider>();
        
        colliderInnerTube = innerTube.GetComponent<CapsuleCollider>();
        interactable = GetComponent<Interactable>();
    }

    void Start()
    {
        colliderLeft.enabled = false;
        colliderRight.enabled = false;
        colliderInnerTube.enabled = false;
    }

    void Update()
    {
        // EnableCables();
        if (inAnimationLeft || inAnimationRight)
        {
            SetPosition();
            if (inAnimationLeft) inAnimationLeft = Animation(transformLeft, endPositionLeft, currentVelocityLeft);
            if (inAnimationRight) inAnimationRight = Animation(transformRight, endPositionRight, currentVelocityRight);
            return;
        }

        if (!interactable.isSelected) return;

        if (Input.GetMouseButtonUp(0) && mouseClickPosition != null)
        {
            Vector2 currentMousePos = Input.mousePosition;
            // Only allow interaction when the mouse is not too far away
            if (Vector2.Distance(mouseClickPosition.Value, currentMousePos) < maxDragDistance)
            {
                clickPermitted = true;
            }
            mouseClickPosition = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            mouseClickPosition = Input.mousePosition;
        }

        if (PlayerPickUpScript.Instance.sameObjectClicked)
        {
            UpdateOpenState();
            PlayerPickUpScript.Instance.sameObjectClicked = false;
        }

        else if (PlayerPickUpScript.Instance.anotherInteractableClicked)
        {
            bool checkParent = CheckParent( PlayerPickUpScript.Instance.anotherInteractableClicked);
            // RotorInteractScript rotorInteractScript = CheckAnotherObject();
            if (!checkParent) 
            {
                PlayerPickUpScript.Instance.wrongObjectClicked = true;
                return;
            }
            if (clickPermitted) {
                UpdateOpenState();
                PlayerPickUpScript.Instance.anotherInteractableClicked = null;
                clickPermitted = false;
            }
        }   
    }

    void LateUpdate()
    {
        if (interactable.runInitialScript)
        {
            interactable.runInitialScript = false;
            colliderLeft.enabled = true;
            colliderRight.enabled = true;
            colliderInnerTube.enabled = true;
            colliderOriginal.enabled = false;
            colliderSwapping.enabled = false;
            cables.SetActive(true);
        }
        else if (interactable.runEndingScript)
        {
            print($"RUNNING ENDING SCRIPT + {open}");
            interactable.runEndingScript = false;
            if (open) {
                UpdateOpenState();
            }
            colliderLeft.enabled = false;
            colliderRight.enabled = false;
            colliderInnerTube.enabled = false;
            colliderOriginal.enabled = true;
            colliderSwapping.enabled = true;
            cables.SetActive(false);
        }
    }

    private RotorInteractScript CheckAnotherObject()
    {
        RotorInteractScript rotorInteractScript = PlayerPickUpScript.Instance.anotherInteractableClicked.GetComponent<RotorInteractScript>();
        if (!rotorInteractScript)
        {
            print("WRONG OBJECT NOT ROTOR INTERACT SCRIPT");
            // PlayerPickUpScript.Instance.wrongObjectClicked = true;
        }
        return rotorInteractScript;
    }

    private bool Animation(Transform transform, Vector3 endPosition, Vector3 currentVelocity)
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            endPosition,
            ref currentVelocity,
            smoothTime
        );

        float distance = Vector3.Distance(
            transform.position,
            endPosition
        );

        if (distance <= stoppingDistance)
        {
            transform.position = endPosition;

            currentVelocity = Vector3.zero;

            return false;
        }
        return true;
    }

    public void UpdateOpenState()
    {
        open = !open;
        SetPosition();
        inAnimationLeft = true;
        inAnimationRight = true;
    }

    public bool InAnimation()
    {
        return inAnimationLeft || inAnimationRight;
    }

    public void SetPosition()
    {
        if (open)
        {
            endPositionLeft = OpenPositionLeft.position;
            endPositionRight = OpenPositionRight.position;
        }
        else
        {
            endPositionLeft = ClosePositionLeft.position;
            endPositionRight = ClosePositionRight.position;
        }
    }

    public bool CheckParent(Interactable differentInteractable)
    {
        if (differentInteractable == null || differentInteractable.transform.parent == null)
            return false;

        if (differentInteractable.transform.parent == this.transform)
        {
            print("CLICKED ON CHILD");
        }
        return differentInteractable.transform.parent == this.transform;
    }

    public void EnableCables()
    {
        // if (!open && !InAnimation())
        // {
        //     cables.SetActive(true);
        // }
        // else if (open && !cables.activeSelf)
        // {
        //     cables.SetActive(true);
        // }
    }

}