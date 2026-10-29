using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugBoardPlugScript : MonoBehaviour
{
    [Header("Plug Board Information")]
    public char plugBoardChar;
    public PlugBoardPlugSocketScript plugSocket;
    public bool plugBoardSet = false;


    [Header("For selecting")]
    private bool isSelected = false;
    private bool dragging = false;
    private Vector2 mousePositionSwappingBefore;
    private Vector2 mousePositionSwappingAfter;
    public float draggingSpeed = 4f;



    [Header("Animation")]
    private bool inAnimation = false;
    private Vector3 endPosition;
    private Vector3 currentVelocity;
    private float stoppingDistance = 0.01f;
    public float smoothTime = 0.5f;
    // Start is called before the first frame update

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (inAnimation)
        {
            PlugBoardPlugAnimation(endPosition);
        }
    }


    void LateUpdate()
    {
        if (inAnimation)
        {
            // Plug going in or out
            // No input be taken
            // If plug taken out, then track where the mouse is so plug does not fly
            mousePositionSwappingBefore = Input.mousePosition;
            return;
        }

        PlugOutDragging();
        PlugInDragging();

    }

    public void PlugInDragging()
    {
        if (!plugSocket)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0) && isSelected)
        {
            // Plug in and clicked for removingn
            dragging = true;
            plugSocket.RemovePlug();
            // plugSocket.SetPlug(this);
            inAnimation = true;
            mousePositionSwappingAfter = Input.mousePosition;
            return;
        }
        if (Input.GetMouseButton(0) && dragging)
        {
            // When plug is being dragged around
            mousePositionSwappingAfter = Input.mousePosition;
            // Vector3 newPosition = new Vector3(mousePositionSwappingAfter.x, mousePositionSwappingAfter.y, transform.position.z);
            Vector3 newPosition = new Vector3(transform.position.x - ((mousePositionSwappingAfter.x - mousePositionSwappingBefore.x)*draggingSpeed),
                                             transform.position.y + (mousePositionSwappingAfter.y - mousePositionSwappingBefore.y)*draggingSpeed, 
                                             transform.position.z);
            transform.position = Vector3.Lerp(
                transform.position,
                newPosition,
                0.5f * Time.deltaTime
            );

            mousePositionSwappingBefore = mousePositionSwappingAfter;
            // print("HOLDING isSelected: " + isSelected + "     selectedForSwapping" + selectedForSwapping + "    ROtor NUMBEr: " + rotorNumber);
        }
        if (!Input.GetMouseButton(0) && dragging)
        {
            Debug.Log($"[{Time.time:F2}s] Mouse up plug");
            // Plug is let go
            // print("ROTOR LET GO    ROTOR NUMBER" + rotorNumber );
            isSelected = false;
            dragging = false;
            inAnimation = true;
            if (plugSocket)
            {
                Debug.Log($"[{Time.time:F2}s] mouse up and plug decided plug");
                // If touching a socket, then plug in animation
                plugSocket.DisableOutline();
            }
            // UpdateSelectedForSwapping(false);
        }

    }

    public void PlugOutDragging()
    {
        if (plugSocket && !Input.GetMouseButton(0) && dragging)
        {
            plugSocket.DisableOutline();
            isSelected = false;
            dragging = false;
            inAnimation = true;
            return;
        }
        if (Input.GetMouseButtonDown(0) && isSelected)
        {
            // Debug.Log($"[{Time.time:F2}s] Mouse button down plug");
            mousePositionSwappingBefore = Input.mousePosition;
            dragging = true;
        }
        if (Input.GetMouseButton(0) && dragging)
        {
            // When plug is being dragged around
            mousePositionSwappingAfter = Input.mousePosition;
           Vector3 newPosition = new Vector3(transform.position.x - ((mousePositionSwappingAfter.x - mousePositionSwappingBefore.x)*draggingSpeed),
                                             transform.position.y + (mousePositionSwappingAfter.y - mousePositionSwappingBefore.y)*draggingSpeed, 
                                             transform.position.z);
            transform.position = Vector3.Lerp(
                transform.position,
                newPosition,
                0.5f * Time.deltaTime
            );

            mousePositionSwappingBefore = mousePositionSwappingAfter;

            // print("HOLDING isSelected: " + isSelected + "     selectedForSwapping" + selectedForSwapping + "    ROtor NUMBEr: " + rotorNumber);
        }
        return;

    } 


    public void UpdateIsSelected()
    {
        isSelected = !isSelected;
    }

    private void PlugBoardPlugAnimation(Vector3 endPosition)
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
            if (plugSocket)
            {
                isSelected = false;
            }
        }
    }

    public void StartAnimation(Transform newEndPosition)
    {
            Debug.Log($"[{Time.time:F2}s] Setting new position plug");
        endPosition = newEndPosition.position;
    }

    public void SetPlugBoard(PlugBoardPlugSocketScript socket, char newChar)
    {
            Debug.Log($"[{Time.time:F2}s] Setting new plug position plug");
        plugSocket = socket;
        plugBoardChar = newChar;
    }
}



// void LateUpdate()
//     {
//         if (inAnimation)
//         {
//             // Plug going in or out
//             // No input be taken
//             // If plug taken out, then track where the mouse is so plug does not fly
//             mousePositionSwappingBefore = Input.mousePosition;
//             return;
//         }
//         if (Input.GetMouseButtonDown(0) && isSelected)
//         {
//             Debug.Log($"[{Time.time:F2}s] Mouse button down plug");
//             // If plug got clicked
//             if (plugSocket)
//             {
//                 Debug.Log($"[{Time.time:F2}s] plugged in so removing plug"); 
//                 // If plug in, then take it out, start animation
//                 plugSocket.RemovePlug();
//                 inAnimation = true;
//                 // Continue so plug can move immediately
//             }
//             mousePositionSwappingBefore = Input.mousePosition;
//             dragging = true;
//         }
//         if (Input.GetMouseButton(0) && dragging)
//         {
//             // When plug is being dragged around
//             mousePositionSwappingAfter = Input.mousePosition;
//             // Vector3 newPosition = new Vector3(mousePositionSwappingAfter.x, mousePositionSwappingAfter.y, transform.position.z);
//             Vector3 newPosition = new Vector3(transform.position.x - ((mousePositionSwappingAfter.x - mousePositionSwappingBefore.x)*draggingSpeed),
//                                              transform.position.y + (mousePositionSwappingAfter.y - mousePositionSwappingBefore.y)*draggingSpeed, 
//                                              transform.position.z);
//             transform.position = Vector3.Lerp(
//                 transform.position,
//                 newPosition,
//                 0.5f * Time.deltaTime
//             );
//             mousePositionSwappingBefore = mousePositionSwappingAfter;
//             // print("HOLDING isSelected: " + isSelected + "     selectedForSwapping" + selectedForSwapping + "    ROtor NUMBEr: " + rotorNumber);
//         }
//         if (!Input.GetMouseButton(0) && dragging)
//         {
//             Debug.Log($"[{Time.time:F2}s] Mouse up plug");
//             // Plug is let go
//             // print("ROTOR LET GO    ROTOR NUMBER" + rotorNumber );
//             isSelected = false;
//             dragging = false;
//             inAnimation = true;
//             if (plugSocket)
//             {
//                 Debug.Log($"[{Time.time:F2}s] mouse up and plug decided plug");
//                 // If touching a socket, then plug in animation
//                 plugSocket.DisableOutline();
//             }
//             // UpdateSelectedForSwapping(false);
//         }
//         return;
//     }