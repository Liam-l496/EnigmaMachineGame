using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotorInnerCasingScript : MonoBehaviour
{


    // Inner Rotor Casing Script

    [Header("Inner Rotor Casing Script")]
    private bool innerRotorInteracting = false;
    private GameObject parent;
    private BoxCollider parentBoxCollider;

    private bool isRotating = false;

    public Vector2 startMousePosition;
    public float rotateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject;
        parentBoxCollider = parent.GetComponent<BoxCollider>();
    }


    public void InnerRotorInteractable()
    {
        print("INNER CLICKED, innerRotorInteracting " + innerRotorInteracting );
        if (parentBoxCollider.enabled)
        {
            print("INNER CLICKED PARENT ENABLED, innerRotorInteracting " + innerRotorInteracting );
            innerRotorInteracting = false;
            return;
        }
        print("INNER CLICKED, innerRotorInteracting NO PARENT" + innerRotorInteracting );
        innerRotorInteracting = !innerRotorInteracting;
    }

// Adjust this in the Inspector to tweak drag responsiveness
    [SerializeField] private float rotationSensitivity = 0.5f;

    private Vector3 lastMousePosition;

    public void LateUpdate() 
    {
        print("INNER CLICKED LastUpdate, innerRotorInteracting " + innerRotorInteracting );

        if (!innerRotorInteracting) {

            return;
        }

        // 1. Mouse Press Started
        if (Input.GetMouseButtonDown(0))
        {
            print("INNER CLICKED FIRST CLICK, innerRotorInteracting " + innerRotorInteracting );

            lastMousePosition = Input.mousePosition;
            isRotating = true;
        }

        // 2. Mouse Held Down (Dragging)
        if (Input.GetMouseButton(0) && isRotating)
        {
            print("INNER CLICKED HELD DOWN, innerRotorInteracting " + innerRotorInteracting );

            // Calculate mouse distance moved since the last frame
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

            // Use horizontal (X) or vertical (Y) drag depending on your setup.
            // Using mouseDelta.x allows side-to-side dragging to rotate the object.
            rotateSpeed = mouseDelta.x * rotationSensitivity;

            RotateInnerRotor();

            // Store current position for the next frame's comparison
            lastMousePosition = Input.mousePosition;
        }

        // 3. Mouse Released
        if (Input.GetMouseButtonUp(0))
        {
            print("INNER CLICKED LET GO, innerRotorInteracting " + innerRotorInteracting );

            innerRotorInteracting = false;
            isRotating = false;
            rotateSpeed = 0f;
        }
    }

    private void RotateInnerRotor()
    {
        // Rotates around the Y-axis (Up). Change Vector3.up to Vector3.forward or Vector3.right 
        // depending on which axis your cylinder needs to spin around.
        transform.Rotate(Vector3.right, -rotateSpeed, Space.Self);
    }
}
