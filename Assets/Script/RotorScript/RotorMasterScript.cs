using UnityEngine;
using TMPro;

public class RotorMasterScript : MonoBehaviour
{

    [Header("Other Components")]
    public Interactable interactable;
    public RotorScript rotorScript;
    public RotorSwappingScript rotorSwappingScript;
    public RotorEncryptionScript rotorEncryptionScript;
    public RotorInteractScript rotorInteractScript;
    public GameObject cables;
    public RotorCableAssignerScript rotorCableAssignerScript;
    public TextMeshPro rotorNameText;


    // [Header("Interacting Variables")]
    // Might need IsSelected update function
    public bool IsSelected { get; set; } = false;
    public bool InAnimation { get;  set; } = false;
    public bool BlackFlapUp { get;  set; } = false;
    public bool ReflectorHolding { get; set; } = true;
    public bool WindowFlapUp { get; set; } = false;
    public bool Transitioning { get; set; } = false;
    public int rotorPosition;
    public string rotorName;


    // For Animation
    [Header("Animation")]
    public Vector3 currentVelocity;
    public Vector3 endPosition; 
    public Quaternion liftUpAngle;
    public Quaternion originalAngle;
    public Quaternion endAngle;
    public float stoppingDistance = 0.01f;
    public float smoothTime = 0.5f;
    public float smoothTimeRotation = 0.5f;
    

    void Start()
    {
        interactable = GetComponent<Interactable>();
        rotorScript = GetComponent<RotorScript>();
        rotorSwappingScript = GetComponent<RotorSwappingScript>();
        rotorEncryptionScript  = GetComponent<RotorEncryptionScript>();
        rotorInteractScript= GetComponent<RotorInteractScript>();
        rotorCableAssignerScript = GetComponent<RotorCableAssignerScript>();
        // = GetComponent<>();
        print("DISABLING ");
        DisableCables();
        print("DISABLED");
        
        rotorNameText.enabled = false;
        rotorNameText.text = rotorName;

        // originalAngle = transform.rotation;
        // liftUpAngle = Quaternion.Euler(336.8394f, 180f, 180f);
        // endAngle = originalAngle;
    }

    void Update()
    {
        
    }

    void LateUpdate()
    {
        if (IsInAnimation())
        {
            RotorAnimation(endPosition);
            return;
        }
    }

    /// <summary>
    /// Encrypt the text 
    /// </summary>
    public int GetPlainToCipher(int plainChar)
    {
        return rotorEncryptionScript.PlainToCipher(plainChar, rotorScript.rotorValue);
    }

    public int GetCipherToPlain(int cipherChar)
    {
        return rotorEncryptionScript.CipherToPlain(cipherChar, rotorScript.rotorValue);
    }



    /// <summary>
    /// Animation Function
    /// </summary> 
    public bool IsInAnimation()
    {
        if (InAnimation && transform.position != endPosition)
        {
            return true;
        }
        if (transform.position == endPosition)
        {
            InAnimation = false;
        }
        return false;
    }

    private void RotorAnimation(Vector3 endPosition)
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            endPosition,
            ref currentVelocity,
            smoothTime
        );

        // transform.rotation = Quaternion.Slerp(
        //     transform.rotation,
        //     endAngle,
        //     Time.deltaTime * smoothTimeRotation
        // );

        float distance = Vector3.Distance(transform.position, endPosition);
        if (distance <= stoppingDistance)
        {
            // Snap to exact target position
            transform.position = endPosition;
            
            // Reset velocity and stop processing
            currentVelocity = Vector3.zero;
            InAnimation = false;
        }
    }

    public void UpdateEndPosition(Transform newEndPositionTransform)
    {
        endPosition = newEndPositionTransform.position;
        InAnimation = true;
    }    

    /// <summary>
    /// Updating States
    /// </summary>

    public void UpdateFlapUpState(bool flapUp)
    {
        BlackFlapUp = flapUp;
        // rotorScript.enabled = !flapUp;
        if (flapUp)
        {
            interactable.UpdateCanPickUp(true);
            rotorScript.enabled = true;
        }
        else
        {
            interactable.UpdateCanPickUp(false);
            rotorScript.enabled = false;
        }
    }

    public void UpdateReflectorHoldingState(bool reflectorHolding)
    {
        ReflectorHolding = reflectorHolding;
        interactable.UpdateCanPickUp(reflectorHolding);
        print("IN UPDATE REFLECTOR HOLDINGS STATE");
        if (!reflectorHolding)
        {
            rotorInteractScript.enabled = false;
            rotorSwappingScript.enabled = true;
            rotorSwappingScript.RotorSwappingAnimationState(reflectorHolding);
            rotorNameText.enabled = true;
            rotorScript.enabled = false;

        }
        else
        {
            rotorSwappingScript.RotorSwappingAnimationState(reflectorHolding);
            rotorSwappingScript.enabled = false;
            rotorInteractScript.enabled = true;
            rotorNameText.enabled = false;
            rotorScript.enabled = true;

        }
    }

    public void UpdateWindowFlapUpState(bool windowFlapUp)
    {
        WindowFlapUp = windowFlapUp;
        if (windowFlapUp)
        {
            rotorScript.enabled = true;
            interactable.UpdateCanPickUp(false);
        }
        else
        {
            rotorScript.enabled = false;
        }
    }

    public void InTransitionAnimation(bool transitioning)
    {
        Transitioning = transitioning;
        if (Transitioning)
        {
            print("Transition now");
        } 
        else
        {
            print("Transition stopped now");
        }
    }

    public void ActivateCables()
    {
        cables.SetActive(true);
    }

    public void DisableCables()
    {
        cables.SetActive(false);
    }


    private void OnEnable()
    {
        AnimationHandlerScript.BlackCoverFlapUp += UpdateFlapUpState;
        AnimationHandlerScript.ReflectorHolding += UpdateReflectorHoldingState;
        AnimationHandlerScript.WindowFlapUp += UpdateWindowFlapUpState;
        AnimationHandlerScript.OnBlendStateChanged -= InTransitionAnimation;
    }

    private void OnDisable()
    {
        AnimationHandlerScript.BlackCoverFlapUp -= UpdateFlapUpState;
        AnimationHandlerScript.ReflectorHolding -= UpdateReflectorHoldingState;
        AnimationHandlerScript.WindowFlapUp -= UpdateWindowFlapUpState;
        AnimationHandlerScript.OnBlendStateChanged -= InTransitionAnimation;
    }
}