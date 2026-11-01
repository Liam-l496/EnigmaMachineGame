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
    private RotorMasterScript rotorMasterScript;

    private float rotateSpeed;
    [SerializeField] float rotateSpeedModifier = 7;
    [SerializeField] Vector3 rotationDirection = new Vector3(-1, 0, 0);

    public int rotorValue = 1;

    public float maxYCoordinate;

    private float highestYCoordinate;
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

    public int rotorPlace;


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
        rotorMasterScript = GetComponent<RotorMasterScript>();
        print("rotorMasterScript + " + rotorMasterScript);
    }

    void LateUpdate()
    { 
        if (rotorMasterScript.Transitioning || rotorMasterScript.InAnimation)
        {
            return;
        }

        if (correctingAngle)
        // If the rotor is between the numbers
        // Used also incrementing the rotor
        {
            // print("Correcting angle");
            CorrectAngle();
        }

        if (!rotorMasterScript.IsSelected)
        {
            // Not looking at rotor
            // Not selected, do not take any input/anything
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            // print("MOUSE BUTTON UP");
            // if mouse over rotor and mouse released
            // print("MOUSE BUTTON UP isSelected " + isSelected + " transform " + transform.position);
            rotorMasterScript.IsSelected = false;
            isDragging = false;
            correctingAngle = true;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // print("MOUSE BUTTON DOwn");
            // if mouse over rotor and mouse clicked
            // UpdateTextObjectState(false);
            // print("isSelected" + isSelected + "Clicked " + transform.position);
            startMousePosition = Input.mousePosition;
            isDragging = true;
        }
        if (Input.GetMouseButton(0) && isDragging)
        {

            // print("MOUSE BUTTON DRAGGING");
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

    // public void UpdateRotorValue()
    // {
    //     Transform ahead = numberLocationArray[(rotorValue) % 26];
    //     Transform current = numberLocationArray[(rotorValue - 1) % 26];
    //     Transform behind = numberLocationArray[(rotorValue - 2) < 0 ? 25 : rotorValue - 2];

    //     if (ahead.position.y >= highestYCoordinate)
    //     {
    //         highestYCoordinate = ahead.position.y;
    //         rotorValue = (rotorValue % 26) + 1;
    //     }
    //     else if (behind.position.y >= highestYCoordinate)
    //     {
    //         highestYCoordinate = behind.position.y;
    //         rotorValue = (rotorValue - 2) < 0 ? 26 : rotorValue - 1;
    //     }
    //     else
    //     {
    //         highestYCoordinate = current.position.y;
    //     }
    // }

    public void UpdateRotorValue()
    {
        // Map 1-based rotorValue to 0-based array indices cleanly
        Transform ahead = numberLocationArray[SafeMod(rotorValue, 26)];
        Transform current = numberLocationArray[SafeMod(rotorValue - 1, 26)];
        Transform behind = numberLocationArray[SafeMod(rotorValue - 2, 26)];

        if (ahead.position.y >= highestYCoordinate)
        {
            highestYCoordinate = ahead.position.y;
            rotorValue = SafeMod(rotorValue, 26) + 1; // Always wraps within 1..26
        }
        else if (behind.position.y >= highestYCoordinate)
        {
            highestYCoordinate = behind.position.y;
            rotorValue = SafeMod(rotorValue - 2, 26) + 1; // Always wraps within 1..26
        }
        else
        {
            highestYCoordinate = current.position.y;
        }
    }

    private int SafeMod(int n, int m)
    {
        return (n % m + m) % m;
    }

    public int IncrementRotorValue()
    {
        rotorValue = rotorValue+1;
        if (rotorValue > 26 || rotorValue <= 0)
        {
            rotorValue = 1;
        }
        return rotorValue;
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
}
