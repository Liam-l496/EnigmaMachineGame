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
    public BoxCollider rotorSwappingCollider;

    [Header("Moving Speed")]
    [SerializeField] float transitionSpeed = 0.5f;

    void Start()
    {
        rotorMasterScript = GetComponent<RotorMasterScript>();
        print("rotorMasterScript swapping" + rotorMasterScript);
        interactable = GetComponent<Interactable>();
    }


    void LateUpdate()
    {
        if (rotorMasterScript.Transitioning || rotorMasterScript.InAnimation)
        {
            return;
        }
        EnableRotorSwappingCollider();
        if(!rotorMasterScript.ReflectorHolding)
        {
            if (Input.GetMouseButtonDown(0) && rotorMasterScript.IsSelected)
            {
                // print("MOUSE DOWN");
                // First click on Rotor
                UpdateSelectedForSwapping(true);
                mousePositionSwappingBefore = Input.mousePosition;
            }
            if (Input.GetMouseButton(0) && selectedForSwapping)
            {
                // print("MOUSE HODLING");
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
                // print("MPUSE UP");
                // Rotor is let go
                // print("ROTOR LET GO    ROTOR NUMBER" + rotorNumber );
                rotorMasterScript.IsSelected = false;
                UpdateSelectedForSwapping(false);
            }
            return;
        }
        // print("REFLECTOR HOLDING");
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
            RotorSwappablePosition();
        }
        else
        {
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

    void OnTriggerEnter(Collider other)
    {
        if (!rotorMasterScript.IsSelected) return;
        Debug.Log("Collided with a trigger object");
        if(other.CompareTag("RotorSwappingPoint"))
        {
            print("COllided with rotor swapping point");
            Transform swappingPoint = other.GetComponent<Transform>();
            RotorSwappingScript swappingRotor = swappingPoint.parent.gameObject.GetComponent<RotorSwappingScript>();
            if (rotorMasterScript.rotorScript != swappingRotor) GameManagerScript.Instance.SwapPosition(this, swappingRotor);
        }
    }

    public void RotorSwappingAnimationState(bool reflectorHolding)
    {
        if (!reflectorHolding)
        {
            rotorMasterScript.UpdateEndPosition(RotorPositionLifted);
        }
        else
        {
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
