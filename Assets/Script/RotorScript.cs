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

public class RotorScript : MonoBehaviour
{

    [SerializeField] float transitionSpeed = 0.5f;
    private Interactable interactable;

    [Header("TextObject")]
    [SerializeField] TextMeshPro  textObject;

    private float rotateSpeed;
    [SerializeField] float rotateSpeedModifier = 7;
    [SerializeField] Vector3 rotationDirection = new Vector3(-1, 0, 0);

    public int rotorValue = 1;
    public int notchPosition = 10;

    public float maxYCoordinate;
    private float maxZCoordinate;

    private float highestYCoordinate;

    private bool isSelected;
    private Vector2 startMousePosition;
    private bool isDragging = false;
    private bool correctingAngle = false;

    public Transform[] numberLocationArray;

    /// <summary>
    /// Rotor Stopping for clicking
    /// </summary>
    public float divider = 50f;

    public float oneRotateSpeed = 300f;

    private bool? overShot;

    private bool blackFlapUp = false;

    void Start()
    {
        List<Transform> matchingList = new List<Transform>();

        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Number"))
            {
                matchingList.Add(child.gameObject.GetComponent<Transform>());
            }
        }

        numberLocationArray = matchingList.ToArray();
        highestYCoordinate = numberLocationArray[0].position.y;
        // maxYCoordinate = numberLocationArray[0].position.y;
        maxYCoordinate = 11.43f;
        maxZCoordinate = numberLocationArray[0].position.z;
        interactable = GetComponent<Interactable>();
        UpdateText();
        PopulateDictionary();
        notch = notchChar - 'A';
    }

    void LateUpdate()
    {
        // print("isSelected + " + isSelected + " GAMEOBJECT" + transform.position);
        
        if (IsInAnimation())
        {
            return;
        }

        if (correctingAngle)
        // If the rotor is between the numbers
        // Used also incrementing the rotor
        {
            CorrectAngle();
            UpdateText();
        }

        if (!isSelected)
        {
            // Not looking at rotor
            // print("IN !SELECTED STATEMENT isSelected : " + isSelected + " transform: " + transform.position);
            // Not selected, do not take any input/anything
            return;
        }

        if(!reflectorHolding)
        {
            if (Input.GetMouseButtonDown(0) && isSelected)
            {
                // First click on Rotor
                UpdateSelectedForSwapping(true);
                mousePositionSwappingBefore = Input.mousePosition;
            }
            if (Input.GetMouseButton(0) && selectedForSwapping)
            {
                // When rotor is being dragged around
                mousePositionSwappingAfter = Input.mousePosition;
                Vector3 newPosition = new Vector3(transform.position.x - ((mousePositionSwappingAfter.x - mousePositionSwappingBefore.x)*4f), transform.position.y, transform.position.z);
                transform.position = Vector3.Lerp(
                    transform.position,
                    newPosition,
                    transitionSpeed * Time.deltaTime
                );
                mousePositionSwappingBefore = mousePositionSwappingAfter;
                // print("HOLDING isSelected: " + isSelected + "     selectedForSwapping" + selectedForSwapping + "    ROtor NUMBEr: " + rotorNumber);
            }
            if (!Input.GetMouseButton(0) && selectedForSwapping)
            {
                // Rotor is let go
                // print("ROTOR LET GO    ROTOR NUMBER" + rotorNumber );
                isSelected = false;
                UpdateSelectedForSwapping(false);
            }
            return;
        }
        // print("PAST !SELECTED STATEMENT isSelected : " + isSelected + " transform: " + transform.position);


        if (Input.GetMouseButtonUp(0))
        {
            // if mouse over rotor and mouse released
            // print("MOUSE BUTTON UP isSelected " + isSelected + " transform " + transform.position);
            isSelected = false;
            isDragging = false;
            correctingAngle = true;
            UpdateTextObjectState(true);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // if mouse over rotor and mouse clicked
            // UpdateTextObjectState(false);
            // print("isSelected" + isSelected + "Clicked " + transform.position);
            startMousePosition = Input.mousePosition;
            isDragging = true;
        }
        if (Input.GetMouseButton(0) && isDragging)
        {
            // if left click held and dragging mode, then rotate rotor
            // print("isSelected" + isSelected + "now Dragging " + transform.position);
            rotateSpeed = (Input.mousePosition.y - startMousePosition.y)/rotateSpeedModifier;
            RotateRotor();
            UpdateRotorValue();
        }

        startMousePosition = Input.mousePosition;
    }   

    public bool RotorFixedPosition()
    {
        Transform current = numberLocationArray[(rotorValue - 1) % 26];
        float roundedYCoordinate = (Mathf.Round(current.position.y * divider)/divider);
        float roundedYCoordinateMax = (Mathf.Round(maxYCoordinate * divider)/divider);
        // print(" Current Y " +  roundedYCoordinate + " Max Y " + roundedYCoordinateMax);
        return roundedYCoordinate >= roundedYCoordinateMax;
    }

    public void RotateRotor()
    {
        transform.Rotate(rotateSpeed * rotationDirection * Time.deltaTime);
    }

    public void IsSelectedTrue()
    {
        // print("isSelected is True");
        isSelected = true;
    }

    public void UpdateIsSelected()
    {
        isSelected = !isSelected;
    }

    public void UpdateRotorValue()
    {
        Transform ahead = numberLocationArray[(rotorValue) % 26];
        Transform current = numberLocationArray[(rotorValue - 1) % 26];
        Transform behind = numberLocationArray[(rotorValue - 2) < 0 ? 25 : rotorValue - 2];

        if (ahead.position.y >= highestYCoordinate)
        {
            highestYCoordinate = ahead.position.y;
            rotorValue = (rotorValue % 26) + 1;
            notchPosition = (notchPosition % 26) + 1;
        }
        else if (behind.position.y >= highestYCoordinate)
        {
            highestYCoordinate = behind.position.y;
            rotorValue = (rotorValue - 2) < 0 ? 26 : rotorValue - 1;
            notchPosition = (notchPosition - 2) < 0 ? 26 : notchPosition - 1;
        }
        else
        {
            highestYCoordinate = current.position.y;
        }
    }

    public int IncrementRotorValue()
    {
        rotorValue = rotorValue+1;
        if (rotorValue > 26 || rotorValue <= 0)
        {
            rotorValue = 1;
        }
        notchPosition = notchPosition + 1;
        if (notchPosition > 26 || notchPosition <= 0)
        {
            notchPosition = 1;
        }
        return rotorValue;
    }

    public void UpdateText()
    {
        textObject.text = rotorValue.ToString();
        if (rotorValue < 10)
        {
            textObject.text = "0" + rotorValue.ToString();
        }
    }

    public void SpinRotorOnce()
    {
        IncrementRotorValue();
        correctingAngle = true;
    }
    
    public void CorrectAngle()
    {
        Transform current = numberLocationArray[(rotorValue - 1) % 26];
        Transform opposite = numberLocationArray[(rotorValue + 12) % 26];
        if (RotorFixedPosition() && current.position.z < opposite.position.z)
        {
            overShot = null;
            correctingAngle = false;
            return;
        }

        // print("Index: " + ((rotorValue - 1) % 26) + "current.position" + current.position + "INDEX: " + ((rotorValue + 12) % 26) + "   opposite.position.z " + opposite.position);

        // current is on the left 
        if (current.position.z < opposite.position.z)
        {
            if (overShot == true)
            {
                rotateSpeed = -oneRotateSpeed/4;
            }
            else {
                rotateSpeed = -oneRotateSpeed;
            }
            overShot = false;
            // Rotate the rotor in the correct way
        }
        // opposite is on the left
        else
        {
            if (overShot == false)
            {
                rotateSpeed = oneRotateSpeed/4;
            }
            else
            {
                rotateSpeed = oneRotateSpeed;
            }

            overShot = true;
            // Rotate the rotor in the opposite way
        }
        RotateRotor();
    }

    public void UpdateTextObjectState(bool newState)
    {
        if (blackFlapUp)
        {
            textObject.enabled = false;
            return;
        }
        textObject.enabled = newState;
    }

    public void UpdateFlapUpState(bool flapUp)
    {
        blackFlapUp = flapUp;  
        UpdateTextObjectState(!flapUp); 
        if (interactable)
        {
            interactable.UpdateCanPickUp(flapUp);
        }
    }

    public void UpdateReflectorHoldingState(bool reflectorHoldingState)
    {
        reflectorHolding = reflectorHoldingState;
        if (interactable)
        {
            interactable.UpdateCanPickUp(reflectorHoldingState);
        }
        if (!reflectorHolding)
        {
            UpdateEndPosition(RotorPositionLifted);
        }
        else
        {
            UpdateEndPosition(RotorPositionStationary);
        }
        inAnimation = true;
    }

    private void OnEnable()
    {
        AnimationHandlerScript.BlackCoverFlapUp += UpdateFlapUpState;
        AnimationHandlerScript.ReflectorHolding += UpdateReflectorHoldingState;
    }

    private void OnDisable()
    {
        AnimationHandlerScript.BlackCoverFlapUp -= UpdateFlapUpState;
        AnimationHandlerScript.ReflectorHolding -= UpdateReflectorHoldingState;
    }


    // ROTOR ANIMATINO
    [Header("Rotor Swapping Variables")]
    public Transform RotorPositionLifted;
    public Transform RotorPositionSwappingTop;
    public Transform RotorPositionStationary;


    public int rotorPlace;
    private Transform[] transformOrder = new Transform[] {};
    private bool inAnimation = false;
    private Vector3 endPosition;
    private Vector3 currentVelocity;
    private float stoppingDistance = 0.01f;
    public float smoothTime = 0.5f;
    private void SwapRotorAnimation(Vector3 endPosition)
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
            inAnimation = false;
        }
    }

    public bool IsInAnimation()
    {
        if (inAnimation && transform.position != endPosition)
        {
            SwapRotorAnimation(endPosition);
            return true;
        }
        if (transform.position == endPosition)
        {
            inAnimation = false;
        }
        return false;
    }

    public void UpdateEndPosition(Transform newEndPositionTransform)
    {
        endPosition = newEndPositionTransform.position;
    }    

    // ROTOR SELECTING

    private bool reflectorHolding = true;
    public bool selectedForSwapping = false;
    private Vector2 mousePositionSwappingBefore;
    private Vector2 mousePositionSwappingAfter;

    public void UpdateSelectedForSwapping(bool newState)
    {
        if (reflectorHolding)
        {
            selectedForSwapping = false;
            return;
        }
        selectedForSwapping = newState;
        if (newState)
        {
            RotorSwappablePosition();
        }
        else
        {
            RotorStationaryPosition();
        }
        // GameManagerScript.Instance.RotorSelected(this);
    }

    public void RotorSwappablePosition()
    {
        inAnimation = true;
        UpdateEndPosition(RotorPositionSwappingTop);
    }

    public void RotorStationaryPosition()
    {
        inAnimation = true;
        UpdateEndPosition(RotorPositionLifted);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isSelected) return;
        Debug.Log("Collided");
        if(other.CompareTag("RotorSwappingPoint"))
        {
            Transform swappingPoint = other.GetComponent<Transform>();
            RotorScript swappingRotor = swappingPoint.parent.gameObject.GetComponent<RotorScript>();
            if (this != swappingRotor) GameManagerScript.Instance.SwapPosition(this, swappingRotor);
        }
    }

    public void MoveRotor(Transform endPoint)
    {
        transform.position = endPoint.position;
    }


    ///////////////////////////////
    /// 
    /// ROTOR ACTUAL JOB
    /// 

    [Header("Rotor Mechanisms")]
    [SerializeField] string wiring;
    public char notchChar;
    public int notch;

    public Dictionary<int, int> dictionaryEntry = new Dictionary<int, int>();
    public Dictionary<int, int> dictionaryExit = new Dictionary<int, int>();

    public void PopulateDictionary()
    {
        int normalLetter = 0;
        foreach (char c in wiring)
        {
            int exit = c - 'A';
            dictionaryEntry.Add(normalLetter, exit);
            dictionaryExit.Add(exit, normalLetter);
            normalLetter += 1;
        }
    }

    public int PlainToCipher(int plainValue)
    {
        int shift = rotorValue - 1;
        
        int pinIn = (plainValue + shift) % 26;
        
        int pinOut = dictionaryEntry[pinIn];
        
        int cipherValue = (pinOut - shift + 26) % 26;
        
        return cipherValue;
    } 

    public int CipherToPlain(int cipherValue)
    {
        int shift = rotorValue - 1;
        
        int pinIn = (cipherValue + shift) % 26;
        
        int pinOut = dictionaryExit[pinIn];
        
        int plainValue = (pinOut - shift + 26) % 26;
        
        return plainValue;
    }



}
