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

    void Start()
    {
        rotorMasterScript = GetComponent<RotorMasterScript>();
        print("rotorMasterScript swapping" + rotorMasterScript);
        interactable = GetComponent<Interactable>();
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
                mousePositionSwappingBefore = Input.mousePosition;
            }

            if (Input.GetMouseButton(0) && selectedForSwapping)
            {
                mousePositionSwappingAfter = Input.mousePosition;

                float mouseDeltaX = (mousePositionSwappingAfter.x - mousePositionSwappingBefore.x) * 4f;

                float targetX = transform.position.x - mouseDeltaX;

                float minX = Mathf.Min(RotorBarrierLeft.position.x, RotorBarrierRight.position.x);
                float maxX = Mathf.Max(RotorBarrierLeft.position.x, RotorBarrierRight.position.x);

                float clampedX = Mathf.Clamp(targetX, minX, maxX);

                Vector3 newPosition = new Vector3(clampedX, transform.position.y, transform.position.z);

                transform.position = Vector3.Lerp(
                    transform.position,
                    newPosition,
                    transitionSpeed * Time.deltaTime
                );

                mousePositionSwappingBefore = mousePositionSwappingAfter;
            }

            if (!Input.GetMouseButton(0) && selectedForSwapping)
            {
                rotorMasterScript.IsSelected = false;
                UpdateSelectedForSwapping(false);
            }

            return;
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
        }
        else
        {
            print("LIFTING Down");
            rotorMasterScript.UpdateEndPosition(RotorPositionStationary);
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
