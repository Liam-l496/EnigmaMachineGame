

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlugBoardPlugSocketScript : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshPro textObject;
    [SerializeField] private char plugBoardText;

    [Header("Plug Positions")]
    [SerializeField] private Transform plugInPosition;
    public Vector3 plugOutPosition;

    [Header("Plug Information")]
    public PlugBoardPlugScript plugObject;

    [Header("Collider")]
    [SerializeField] private BoxCollider socketCollider;

    [Header("Object references")]
    private Outline socketOutline;
    private Animator animatorPlugSocket;
    private int isPluggedHash;

    // --------------------------------------------------
    // SOCKET REGISTRY
    // --------------------------------------------------

    private static readonly List<PlugBoardPlugSocketScript> allSockets =
        new List<PlugBoardPlugSocketScript>();

    private readonly Dictionary<PlugBoardPlugScript, HashSet<Collider>>
        overlappingPlugs =
            new Dictionary<PlugBoardPlugScript, HashSet<Collider>>();


    // --------------------------------------------------
    // INITIALISATION
    // --------------------------------------------------

    private void Awake()
    {
        if (socketCollider == null)
        {
            socketCollider = GetComponent<BoxCollider>();
        }

        if (socketCollider != null)
        {
            socketCollider.isTrigger = true;
        }

        socketOutline = GetComponent<Outline>();

        if (socketOutline == null)
        {
            socketOutline = gameObject.AddComponent<Outline>();
        }

        socketOutline.enabled = false;

        animatorPlugSocket = GetComponent<Animator>();

        isPluggedHash = Animator.StringToHash("isPlugged");
    }

    private void OnEnable()
    {
        if (!allSockets.Contains(this))
        {
            allSockets.Add(this);
        }
    }

    private void OnDisable()
    {
        allSockets.Remove(this);

        overlappingPlugs.Clear();

        DisableOutline();
    }

    private void Start()
    {
        if (plugBoardText == '\0')
        {
            plugBoardText = ' ';
        }

        if (textObject != null)
        {
            textObject.text = plugBoardText.ToString();
        }
    }


    // --------------------------------------------------
    // COLLIDER DETECTION
    // --------------------------------------------------

    private void OnTriggerEnter(Collider collision)
    {
        PlugBoardPlugScript plug =
            collision.GetComponentInParent<PlugBoardPlugScript>();

        if (plug == null)
        {
            return;
        }

        if (!overlappingPlugs.ContainsKey(plug))
        {
            overlappingPlugs.Add(
                plug,
                new HashSet<Collider>()
            );
        }

        overlappingPlugs[plug].Add(collision);

        if (plug.IsDragging())
        {
            plug.SetTargetSocket(this);
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        PlugBoardPlugScript plug =
            collision.GetComponentInParent<PlugBoardPlugScript>();

        if (plug != null && plug.IsDragging())
        {
            plug.SetTargetSocket(this);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        PlugBoardPlugScript plug =
            collision.GetComponentInParent<PlugBoardPlugScript>();

        if (plug == null)
        {
            return;
        }

        if (!overlappingPlugs.ContainsKey(plug))
        {
            return;
        }

        overlappingPlugs[plug].Remove(collision);

        // Only remove the plug once all its colliders
        // have exited the socket collider.
        if (overlappingPlugs[plug].Count == 0)
        {
            overlappingPlugs.Remove(plug);

            plug.RemoveTargetSocket(this);
        }
    }


    // --------------------------------------------------
    // FIND SOCKET
    // --------------------------------------------------

    public static PlugBoardPlugSocketScript FindSocketForPlug(
        PlugBoardPlugScript plug)
    {
        PlugBoardPlugSocketScript closestSocket = null;

        float closestDistance = float.MaxValue;

        foreach (PlugBoardPlugSocketScript socket in allSockets)
        {
            if (socket == null)
            {
                continue;
            }

            if (!socket.CanAcceptPlug(plug))
            {
                continue;
            }

            if (!socket.overlappingPlugs.ContainsKey(plug))
            {
                continue;
            }

            float distance = Vector3.Distance(
                plug.transform.position,
                socket.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;

                closestSocket = socket;
            }
        }

        return closestSocket;
    }


    // --------------------------------------------------
    // SOCKET MANAGEMENT
    // --------------------------------------------------

    public bool CanAcceptPlug(PlugBoardPlugScript plug)
    {
        return plug != null &&
               (plugObject == null || plugObject == plug);
    }

    public void SetPlug(PlugBoardPlugScript plug)
    {
        if (!CanAcceptPlug(plug))
        {
            return;
        }

        // Debug.Log(
        //     $"[{Time.time:F2}s] Setting plug into socket {plugBoardText}"
        // );

        plugObject = plug;

        plugOutPosition = plug.transform.position;

        // Update the plug's information.
        plug.SetPlugBoard(this, plugBoardText);

        DisableOutline();

        // Animate the plug into the socket.
        plug.StartAnimation(plugInPosition);
        animatorPlugSocket.SetBool(isPluggedHash, true);
    }

    public void RemovePlug(bool animate = true)
    {
        if (plugObject == null)
        {
            return;
        }

        PlugBoardPlugScript oldPlug = plugObject;

        Debug.Log(
            $"Removing plug. Animate = {animate}, Position = {plugOutPosition}"
        );

        plugObject = null;

        DisableOutline();

        oldPlug.SetPlugBoard(null, ' ');

        if (animate)
        {
            oldPlug.StartAnimation(plugOutPosition);
            animatorPlugSocket.SetBool(isPluggedHash, false);
        }
    }


    // --------------------------------------------------
    // OUTLINE
    // --------------------------------------------------

    public void EnableOutline()
    {
        if (socketOutline != null && plugObject == null)
        {
            socketOutline.enabled = true;
        }
    }

    public void DisableOutline()
    {
        if (socketOutline != null)
        {
            socketOutline.enabled = false;
        }
    }
}