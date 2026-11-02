
using UnityEngine;

public class PlugBoardPlugScript : MonoBehaviour
{
    [Header("Plug Designs")]
    public GameObject[] plugDesigns;

    [Header("Plug Board Plug Interact")]
    public Interactable interactable;

    [Header("Plug Board Information")]
    public char plugBoardChar;
    public bool plugBoardSet = false;

    [Header("Socket Information")]
    public PlugBoardPlugSocketScript plugSocket;

    private PlugBoardPlugSocketScript currentSocket;
    private PlugBoardPlugSocketScript targetSocket;

    [Header("Dragging")]
    [SerializeField] private float draggingSmoothTime = 0.04f;

    private bool isSelected = false;
    private bool dragging = false;

    private Camera mainCamera;
    private Plane draggingPlane;

    private Vector3 mouseOffset;
    private Vector3 dragVelocity;

    private bool releasePending = false;

    [Header("Animation")]
    public bool inAnimation = false;
    private Vector3 endPosition;
    private Vector3 currentVelocity;

    [SerializeField] private float stoppingDistance = 0.01f;
    [SerializeField] private float smoothTime = 0.5f;


    // --------------------------------------------------
    // INITIALISATION
    // --------------------------------------------------

    private void Start()
    {
        mainCamera = Camera.main;

        RandomisePlugShape();

        currentSocket = plugSocket;
        
        interactable = GetComponent<Interactable>();
    }

    private void RandomisePlugShape()
    {
        foreach (GameObject plug in plugDesigns)
        {
            if (plug != null)
            {
                plug.SetActive(false);
            }
        }

        if (plugDesigns.Length > 0)
        {
            int randomIndex = Random.Range(0, plugDesigns.Length);

            plugDesigns[randomIndex].SetActive(true);
        }
    }


    // --------------------------------------------------
    // UPDATE
    // --------------------------------------------------

private void Update()
{
    if (inAnimation)
    {
        if (dragging && Input.GetMouseButtonUp(0))
        {
            releasePending = true;
        }

        PlugBoardPlugAnimation();
        return;
    }

    if (releasePending)
    {
        releasePending = false;
        StopDragging();
        return;
    }

    HandleDragging();
}

    private void HandleDragging()
    {
        if (Input.GetMouseButtonDown(0) && interactable.isSelected)
        {
            StartDragging();
        }

        if (dragging && Input.GetMouseButton(0))
        {
            UpdateDragging();
        }

        if (dragging && Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }
    }


    // --------------------------------------------------
    // DRAGGING
    // --------------------------------------------------

    private void StartDragging()
    {
        dragging = true;

        dragVelocity = Vector3.zero;

        // Create a plane parallel to the camera
        // at the plug's current depth.
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
            // the mouse and the centre of the plug.
            mouseOffset = transform.position - mouseWorldPosition;
        }

        // Disconnect from the original socket.
        if (currentSocket != null)
        {
            currentSocket.RemovePlug(true);
        }

        currentSocket = null;
        plugSocket = null;

        plugBoardChar = ' ';
        plugBoardSet = false;

        targetSocket = null;
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

            // Smoothly follow the mouse.
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref dragVelocity,
                draggingSmoothTime
            );
        }

        // Update the socket the plug is hovering over.
        UpdateTargetSocket();
    }

   private void StopDragging()
    {
        Debug.Log(
            $"STOP DRAGGING: targetSocket = " +
            $"{(targetSocket != null ? targetSocket.name : "NULL")}"
        );

        dragging = false;
        dragVelocity = Vector3.zero;

        if (targetSocket != null &&
            targetSocket.CanAcceptPlug(this))
        {
            Debug.Log($"REINSERTING INTO: {targetSocket.name}");

            targetSocket.SetPlug(this);
            targetSocket = null;

            interactable.isSelected = false;
            return;
        }

        if (targetSocket != null)
        {
            targetSocket.DisableOutline();
        }

        targetSocket = null;
        interactable.isSelected = false;
    }


    private void RecalculateMouseOffset()
    {
        Ray mouseRay = mainCamera.ScreenPointToRay(
            Input.mousePosition
        );

        if (draggingPlane.Raycast(mouseRay, out float distance))
        {
            Vector3 mouseWorldPosition = mouseRay.GetPoint(distance);

            mouseOffset = transform.position - mouseWorldPosition;
        }
    }

    // --------------------------------------------------
    // SOCKET DETECTION
    // --------------------------------------------------

    private void UpdateTargetSocket()
    {
        PlugBoardPlugSocketScript detectedSocket =
            PlugBoardPlugSocketScript.FindSocketForPlug(this);

        if (detectedSocket == targetSocket)
        {
            return;
        }

        Debug.Log($"TARGET SOCKET ASSIGNED: {name}");

        // Remove outline from the previous socket.
        if (targetSocket != null)
        {
            targetSocket.DisableOutline();
        }

        targetSocket = detectedSocket;

        // Highlight the new socket.
        if (targetSocket != null)
        {
            targetSocket.EnableOutline();
        }
    }

    public void SetTargetSocket(
        PlugBoardPlugSocketScript socket)
    {
        if (!dragging || socket == null)
        {
            return;
        }

        if (!socket.CanAcceptPlug(this))
        {
            return;
        }

        if (targetSocket != null && targetSocket != socket)
        {
            targetSocket.DisableOutline();
        }

        targetSocket = socket;

        targetSocket.EnableOutline();
    }

    public void RemoveTargetSocket(
        PlugBoardPlugSocketScript socket)
    {
        if (targetSocket == socket)
        {
            targetSocket.DisableOutline();

            targetSocket = null;
        }
    }


    // --------------------------------------------------
    // SELECTION
    // --------------------------------------------------

    public void UpdateIsSelected()
    {
        isSelected = !isSelected;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
    }

    public bool IsDragging()
    {
        return dragging;
    }


    // --------------------------------------------------
    // ANIMATION
    // --------------------------------------------------

    private void PlugBoardPlugAnimation()
    {
        print($"endPosition = {endPosition}");
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

            inAnimation = false;

            // Recalculate the offset after the plug-out animation.
            // This prevents UpdateDragging from moving the plug
            // back towards its previous plugged-in position.
            if (dragging && Input.GetMouseButton(0))
            {
                RecalculateMouseOffset();
            }

            if (currentSocket != null)
            {
                interactable.isSelected = false;
            }
        }
    }

    public void StartAnimation(Transform newEndPosition)
    {
        if (newEndPosition == null) return;

        Debug.Log(
            $"START ANIMATION (Transform): {newEndPosition.position}\n" +
            System.Environment.StackTrace
        );

        endPosition = newEndPosition.position;
        currentVelocity = Vector3.zero;
        inAnimation = true;
    }

    public void StartAnimation(Vector3 newEndPosition)
    {
        Debug.Log(
            $"START ANIMATION (Vector3): {newEndPosition}\n" +
            System.Environment.StackTrace
        );

        endPosition = newEndPosition;
        currentVelocity = Vector3.zero;
        inAnimation = true;
    }

    // --------------------------------------------------
    // PLUG BOARD INFORMATION
    // --------------------------------------------------

    public void SetPlugBoard(
        PlugBoardPlugSocketScript socket,
        char newChar)
    {
        plugSocket = socket;

        currentSocket = socket;

        plugBoardChar = newChar;

        plugBoardSet = socket != null;
    }
}