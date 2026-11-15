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
    [Header("For Actuator Animation")]
    private int previousRotorValue = 1;
    private bool actuatorAnimatorSet = false;

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

    [Header("Left Side")]
    [SerializeField] Transform leftSideTransform;


    void Start()
    {
        List<Transform> matchingList = new List<Transform>();

        foreach (Transform child in leftSideTransform)
        {
            if (child.name.StartsWith("Number"))
            {
                matchingList.Add(child.gameObject.GetComponent<Transform>());
            }
        }

        numberLocationArray = matchingList.ToArray();
        highestYCoordinate = numberLocationArray[0].position.y;

        maxYCoordinate = 11.43f;

        rotorMasterScript = GetComponent<RotorMasterScript>();
        print("rotorMasterScript + " + rotorMasterScript);
    }

    void Update()
    {
        if (rotorValue != previousRotorValue)
        {
            GameManagerScript.Instance.AnimateActuatorBarPressing(rotorPlace == 2, rotorPlace == 3);
            previousRotorValue = rotorValue;
            actuatorAnimatorSet = true;
        }
    }

    void LateUpdate()
    {
        if (actuatorAnimatorSet)
        {
            GameManagerScript.Instance.AnimateActuatorBarReleasing();
            actuatorAnimatorSet = false;
        }

        if (rotorMasterScript.Transitioning || rotorMasterScript.InAnimation)
        {
            return;
        }

        if (correctingAngle)
        {
            CorrectAngle();
        }

        if (!rotorMasterScript.IsSelected)
        {
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            rotorMasterScript.IsSelected = false;
            isDragging = false;
            correctingAngle = true;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            startMousePosition = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            rotateSpeed = (Input.mousePosition.y - startMousePosition.y) / rotateSpeedModifier;

            RotateRotor();
            UpdateRotorValue();
        }

        startMousePosition = Input.mousePosition;
    }


    public bool RotorFixedPosition()
    {
        Transform current = numberLocationArray[(rotorValue - 1) % 26];

        float roundedYCoordinate = (Mathf.Round(current.position.y * divider) / divider);
        float roundedYCoordinateMax = (Mathf.Round(maxYCoordinate * divider) / divider);

        return roundedYCoordinate >= roundedYCoordinateMax;
    }


    public void RotateRotor()
    {
        transform.Rotate(rotateSpeed * rotationDirection * Time.deltaTime);
    }


    public void UpdateRotorValue()
    {
        Transform ahead = numberLocationArray[SafeMod(rotorValue, 26)];
        Transform current = numberLocationArray[SafeMod(rotorValue - 1, 26)];
        Transform behind = numberLocationArray[SafeMod(rotorValue - 2, 26)];

        if (ahead.position.y >= highestYCoordinate)
        {
            highestYCoordinate = ahead.position.y;
            rotorValue = SafeMod(rotorValue, 26) + 1;
        }
        else if (behind.position.y >= highestYCoordinate)
        {
            highestYCoordinate = behind.position.y;
            rotorValue = SafeMod(rotorValue - 2, 26) + 1;
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
        rotorValue = rotorValue + 1;

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

        // Calculate the rotor's current position relative to its rotation axis.
        Vector3 rotationAxis = transform.TransformDirection(rotationDirection.normalized);

        Vector3 currentDirection = Vector3.ProjectOnPlane(
            current.position - transform.position,
            rotationAxis
        );

        // The desired position is the highest point on the rotor.
        Vector3 targetDirection = Vector3.ProjectOnPlane(
            Vector3.up,
            rotationAxis
        );

        // Calculate the remaining angle required to reach the correct position.
        float remainingAngle = Vector3.Angle(
            currentDirection,
            targetDirection
        );

        // If the rotor is sufficiently close, finish the correction.
        if (remainingAngle <= 0.15f)
        {
            overShot = null;
            correctingAngle = false;
            rotateSpeed = 0f;
            return;
        }

        // Determine the correction direction using the original rotor logic.
        bool rotateNegative = current.position.z < opposite.position.z;

        // Preserve the overshoot direction tracking.
        if (rotateNegative)
        {
            overShot = false;
        }
        else
        {
            overShot = true;
        }

        // Calculate a smooth deceleration based on the remaining angle.
        // The rotor slows down as it approaches its destination.
        float decelerationDistance = 30f;

        float speedMultiplier = Mathf.SmoothStep(
            0f,
            1f,
            Mathf.Clamp01(remainingAngle / decelerationDistance)
        );

        // Maintain a small minimum speed to avoid the rotor getting stuck.
        float minimumSpeed = 35f;

        float currentSpeed = Mathf.Lerp(
            minimumSpeed,
            oneRotateSpeed,
            speedMultiplier
        );

        // Determine the rotation direction.
        rotateSpeed = rotateNegative ? -currentSpeed : currentSpeed;

        // Calculate the rotation for this frame.
        float rotationStep = currentSpeed * Time.deltaTime;

        // Never allow the rotor to rotate beyond its target.
        rotationStep = Mathf.Min(rotationStep, remainingAngle);

        // Apply the rotation using the existing rotation direction.
        transform.Rotate(
            Mathf.Sign(rotateSpeed) * rotationStep * rotationDirection.normalized,
            Space.Self
        );

        // If the rotor has reached its destination, finish the animation.
        if (rotationStep >= remainingAngle)
        {
            overShot = null;
            correctingAngle = false;
            rotateSpeed = 0f;
        }
    }
}