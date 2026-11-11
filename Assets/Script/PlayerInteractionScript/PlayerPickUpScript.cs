using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class PlayerPickUpScript : MonoBehaviour
{

    public static PlayerPickUpScript Instance;



    [Header("Hold Position")]
    public Transform holdPosition;
    private Vector3 originalHeldPosition; 
    private Transform heldObjectTransform; //rigidbody of object we pick up
    private Quaternion originalHeldRotation;
    private bool currentlyHolding = false;

    public float rotationSensitivity;
    public Interactable heldObject; //object which we pick up
    private Vector3 XaxisVector;
    private Vector3 YaxisVector;

    [Header("Pick Up Animation")]
    public bool inAnimation = false;
    public float smoothTime = 0.3f;
    public float smoothTimeRotation = 0.5f;
    private Vector3 currentVelocity;
    private float stoppingDistance = 0.1f;
    private Vector3 animationEndPosition;

    [Header("Zooming in")]
    public float scrollSpeed = 2.0f;
    private float currentDistance = 5.0f;
    private float minDistance = 100.0f;
    private float maxDistance = 2000.0f;

    [Header("Player Interaction Connection Variables")]
    private PlayerInteraction playerInteraction;

    public bool somethingElseClicked = false;
    public Interactable anotherInteractableClicked;

    [Header("Interactable COnnection Variables")]
    public bool sameObjectClicked = false;
    public bool wrongObjectClicked = false;
    public bool skipUpdate = false;


    private void Awake()
    {
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
        playerInteraction = GetComponent<PlayerInteraction>();
    }


    void Update()
    {
        if (inAnimation)
        {
            // print("IN ANIMATINO");
            ObjectAnimation(animationEndPosition);
        }
        else if (!heldObject) {
            heldObjectTransform = null;
            return;
        }
        if (skipUpdate) 
        { 
            // print("SKIP UPDATE + " + skipUpdate);  
            return;
        }
        // print("WrongObjectClicked +  " + wrongObjectClicked + "       sameObjectClicked  + " + sameObjectClicked);

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0) ZoomObject(scrollInput);
    }

    void LateUpdate()
    {
        // print("LATE UPDATE WrongObjectClicked +  " + wrongObjectClicked + "       sameObjectClicked  + " + sameObjectClicked);
        if (somethingElseClicked && wrongObjectClicked)
        {
            // print("SOMETHING ELSE CLICKED + " + somethingElseClicked);
            playerInteraction.mouseClick = true;
            playerInteraction.currentInteractable = anotherInteractableClicked;
            anotherInteractableClicked = null;
            somethingElseClicked = false;
            playerInteraction.setByPlayerPickUpScript = true;
            PutDownObject();
            wrongObjectClicked = false;
        }
    }

    public void ObjectAnimation(Vector3 endPosition)
    {
        // print("HELD OBJECT TRANSFORM + " + heldObjectTransform);
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
        // print("distance = " + distance + "    stoppingdistance = " + stoppingDistance + "   rotation = " + angle);
        if (distance <= stoppingDistance && angle <= 0.5f)
        {
            // Snap to exact target position
            heldObjectTransform.position = endPosition;
            heldObjectTransform.rotation = originalHeldRotation;
            
            // Reset velocity and stop processing
            currentVelocity = Vector3.zero;
            inAnimation = false;
        }
    }


    public void PickUpObject(Interactable pickUpObj)
        {
            currentlyHolding = true;
            heldObject = pickUpObj; //assign heldObj to the object that was hit by the raycast (no longer == null)
            heldObjectTransform = pickUpObj.GetComponent<Transform>(); //assign Rigidbody
            
            heldObject.isSelected = true;
            heldObject.runInitialScript = true;


            originalHeldPosition = heldObjectTransform.position;
            originalHeldRotation = heldObjectTransform.rotation;
            inAnimation = true;
            animationEndPosition = holdPosition.position;
            heldObject.UpdatePickUpState(true);

            minDistance = heldObject.minDistance;
            maxDistance = heldObject.maxDistance;

            wrongObjectClicked = false;
        }

    public void PutDownObject()
    {
        currentlyHolding = false;
        if (heldObject == null)
        {
            return;
        }
        // print("PUTTING DOWN OBJECT");
        heldObject.UpdatePickUpState(false);
        heldObject.isSelected = false;
        heldObject.runEndingScript = true;
        animationEndPosition = originalHeldPosition;
        heldObject = null;
        inAnimation = true;
        wrongObjectClicked = false;
    }

    public void RotateObject()
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


    public void ClickSameObject()
    {
        sameObjectClicked = true;
    }

    public void ZoomObject(float scrollInput)
    {
        currentDistance -= scrollInput * scrollSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        // Update position
        Transform camTransform = Camera.main.transform;
        heldObject.transform.position = camTransform.position + (camTransform.forward * currentDistance);
    }

    public void UpdateEndPosition(Transform newPosition)
    {
        animationEndPosition = newPosition.position;
        inAnimation = true;
    }

    public void UpdateEndPosition(Vector3 newPosition)
    {
        animationEndPosition = newPosition;
        inAnimation = true;
    }

    public void UpdateEndRotation(Quaternion newRotation)
    {
        originalHeldRotation = newRotation;
    }


    public void PutAnotherInteractableDown()
    {
        somethingElseClicked = false;
        sameObjectClicked = false;
        wrongObjectClicked = false;
        anotherInteractableClicked = null;

    }
}