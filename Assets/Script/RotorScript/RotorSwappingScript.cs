
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotorSwappingScript : MonoBehaviour
{
    public bool selectedForSwapping = false;
    private Vector2 mousePositionSwappingBefore;
    private Vector2 mousePositionSwappingAfter;

    public RotorMasterScript rotorMasterScript;

    private Interactable interactable;

    [Header("Rotor Swapping Variables")]
    public Transform RotorPositionLifted;
    public Transform RotorPositionSwappingTop;
    public Transform RotorPositionStationary;
    public Transform RotorBarrierRight;
    public Transform RotorBarrierLeft;
    public BoxCollider rotorSwappingCollider;

    [Header("Moving Speed")]
    [SerializeField] float transitionSpeed = 0.5f;

    [Header("Dragging")]
    [SerializeField] private float draggingSmoothTime = 0.04f;

    private Camera mainCamera;
    private Plane draggingPlane;

    private Vector3 mouseOffset;
    private Vector3 dragVelocity;

    private bool draggingInitialised = false;

    void Start()
    {
        rotorMasterScript = GetComponent<RotorMasterScript>();
        print("rotorMasterScript swapping" + rotorMasterScript);
        interactable = GetComponent<Interactable>();

        mainCamera = Camera.main;

        DisableRotorSwappingCollider();
    }

    void LateUpdate()
    {
        if (rotorMasterScript.Transitioning || rotorMasterScript.InAnimation)
        {
            return;
        }

        if (!rotorMasterScript.ReflectorHolding)
        {
            if (Input.GetMouseButtonDown(0) && rotorMasterScript.IsSelected)
            {
                UpdateSelectedForSwapping(true);

                draggingInitialised = false;
            }

            if (Input.GetMouseButton(0) && selectedForSwapping)
            {
                // Initialise dragging after the lifting animation finishes.
                if (!draggingInitialised)
                {
                    StartDragging();
                }

                UpdateDragging();
            }

            if (!Input.GetMouseButton(0) && selectedForSwapping)
            {
                rotorMasterScript.IsSelected = false;
                interactable.isSelected = false;

                draggingInitialised = false;
                dragVelocity = Vector3.zero;

                UpdateSelectedForSwapping(false);
            }

            return;
        }
    }

    // --------------------------------------------------
    // DRAGGING
    // --------------------------------------------------

    private void StartDragging()
    {
        draggingInitialised = true;

        dragVelocity = Vector3.zero;

        // Create a plane parallel to the camera
        // at the rotor's current depth.
        draggingPlane = new Plane(
            mainCamera.transform.forward,
            transform.position
        );

        // Get the mouse position in world space.
        Ray mouseRay = mainCamera.ScreenPointToRay(
            Input.mousePosition
        );

        if (draggingPlane.Raycast(mouseRay, out float distance))
        {
            Vector3 mouseWorldPosition = mouseRay.GetPoint(distance);

            // Maintain the original distance between
            // the mouse and the centre of the rotor.
            mouseOffset = transform.position - mouseWorldPosition;
        }
    }

    private void UpdateDragging()
    {
        Ray mouseRay = mainCamera.ScreenPointToRay(
            Input.mousePosition
        );

        if (draggingPlane.Raycast(mouseRay, out float distance))
        {
            Vector3 mouseWorldPosition = mouseRay.GetPoint(distance);

            Vector3 targetPosition = mouseWorldPosition + mouseOffset;

            // Restrict movement to the X-axis.
            float minX = Mathf.Min(
                RotorBarrierLeft.position.x,
                RotorBarrierRight.position.x
            );

            float maxX = Mathf.Max(
                RotorBarrierLeft.position.x,
                RotorBarrierRight.position.x
            );

            targetPosition.x = Mathf.Clamp(
                targetPosition.x,
                minX,
                maxX
            );

            // Preserve the rotor's current Y and Z positions.
            targetPosition.y = transform.position.y;
            targetPosition.z = transform.position.z;

            // Smoothly follow the mouse.
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref dragVelocity,
                draggingSmoothTime
            );
        }
    }

    public void UpdateSelectedForSwapping(bool newState)
    {
        if (rotorMasterScript.ReflectorHolding)
        {
            selectedForSwapping = false;
            return;
        }

        selectedForSwapping = newState;

        if (newState)
        {
            EnableRotorSwappingCollider();
            RotorSwappablePosition();
        }
        else
        {
            DisableRotorSwappingCollider();
            RotorStationaryPosition();
        }
    }

    public void RotorSwappablePosition()
    {
        rotorMasterScript.InAnimation = true;
        rotorMasterScript.UpdateEndPosition(RotorPositionSwappingTop);
    }

    public void RotorStationaryPosition()
    {
        rotorMasterScript.InAnimation = true;
        rotorMasterScript.UpdateEndPosition(RotorPositionLifted);
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     if (!rotorMasterScript.IsSelected) return;
    //     Debug.Log("Collided with a trigger object");
    //     if(other.CompareTag("RotorSwappingPoint"))
    //     {
    //         print("COllided with rotor swapping point");
    //         Transform swappingPoint = other.GetComponent<Transform>();
    //         RotorSwappingScript swappingRotor = swappingPoint.parent.gameObject.GetComponent<RotorSwappingScript>();
    //         if (rotorMasterScript.rotorScript != swappingRotor) GameManagerScript.Instance.SwapPosition(this, swappingRotor);
    //     }
    // }

    public void RotorSwappingAnimationState(bool reflectorHolding)
    {
        if (!reflectorHolding)
        {
            print("LIFTING Up");
            rotorMasterScript.UpdateEndPosition(RotorPositionLifted);
            // rotorMasterScript.endAngle = rotorMasterScript.liftUpAngle;
        }
        else
        {
            print("LIFTING Down");
            rotorMasterScript.UpdateEndPosition(RotorPositionStationary);
            // rotorMasterScript.endAngle = rotorMasterScript.originalAngle;
        }

        rotorMasterScript.InAnimation = true;
    }

    public void DisableRotorSwappingCollider()
    {
        rotorSwappingCollider.enabled = false;
    }

    public void EnableRotorSwappingCollider()
    {
        rotorSwappingCollider.enabled = true;
    }
}
