using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    Animator animator;
    int isPressingHash;

    [Header("Key Details")]
    [SerializeField] string keyPress;
    private bool mousePressingKey = false;

    private bool spinRotorBool = false;

    [Header("TextBox")]
    [SerializeField] TextMeshPro keyText;

    [Header("Lamp Bulb")]
    [SerializeField] TextMeshPro LightBulbText;
    public Material blackMaterial;
    public Material lightMaterial;
    [SerializeField] GameObject lampBulbPlane;
    private Renderer lampBulbRenderer;
    private bool lightOn = false;
    public bool lightOnGameManager = false;
    private bool blackFlapUp = false;
    [SerializeField] GameObject lampBulbObject;
    private Renderer lampBulbObjectRenderer;
    public Material glassMaterial;




    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isPressingHash = Animator.StringToHash("isPressing");
        keyText.text = keyPress.ToUpper();
        LightBulbText.text = keyPress.ToUpper();
        lampBulbRenderer = lampBulbPlane.GetComponent<Renderer>();
        if(lampBulbObject == null) return;
        lampBulbObjectRenderer = lampBulbObject.GetComponent<Renderer>();
    }   

    // Update is called once per frame
    void Update()
    {
        // If the mouse left hold was let go, then let boolean be false
        if (mousePressingKey && Input.GetMouseButtonUp(0))
        {
            MouseButtonUp();
        }
    }


    void LateUpdate()
    {
        bool keyInput = Input.GetKey(keyPress.ToLower());
        if (keyInput || mousePressingKey)
        {
            Pressing();
            if (!spinRotorBool)
            {
                GameManagerScript.Instance.KeyPress(keyPress);
                spinRotorBool = true;
            }
        }
        else
        {
            Releasing();
            if (spinRotorBool)
            {
                GameManagerScript.Instance.KeyRelease();
            }
            spinRotorBool = false;
        }
    }

    public void Pressing()
    {
        animator.SetBool(isPressingHash, true);
    }

    public void Releasing()
    {
        animator.SetBool(isPressingHash, false);
    }

    public void MouseButtonDown()
    {
        mousePressingKey = true;
    }
    public void MouseButtonUp()
    {
        mousePressingKey = false;
    }

    // public void LampSwitch(bool keyPress)
    // {
    //     if (!GameManagerScript.Instance.powerOn)
    //     // If power off
    //     {
    //         if (lightOn)
    //         // If light is on when power off, then turn it off
    //         {
    //             LampOffPlane();
    //         }
    //         return;
    //     }
    //     else if (keyPress || mousePressingKey)
    //     {
    //         // if key pressed while power on
    //         if (!lightOn)
    //         {
    //             // if black flap is down, then light up plane
    //             if (!blackFlapUp) LampOnPlane();
    //             // LampOnBulb();
    //         }
    //     }
    //     else
    //     {
    //         // if nothing pressed
    //         if (lightOn)
    //         {
    //             // If light still on, then turn it off
    //             LampOffPlane();
    //             // LampOffBulb();
    //         }
    //     }
    // }


    public void LampOn()
    {   if (!GameManagerScript.Instance.powerOn) return;
        LampOnPlane();
    }


    public void LampOff()
    {
        LampOffPlane();
    }


    public void LampSwitch()
    {
        if (!GameManagerScript.Instance.powerOn)
        // If power off
        {
            if (lightOn)
            // If light is on when power off, then turn it off
            {
                LampOffPlane();
            }
            return;
        }
            // if key pressed while power on
        if (!lightOn)
        {
            // if black flap is down, then light up plane
            if (!blackFlapUp) LampOnPlane();
            // LampOnBulb();
        }
        // if nothing pressed
        if (lightOn)
        {
            // If light still on, then turn it off
            LampOffPlane();
            // LampOffBulb();
        }
    }

    public void LampOnPlane()
    {
        lampBulbRenderer.material = lightMaterial;
        lightOn = true;
    }

    public void LampOffPlane()
    {
        lampBulbRenderer.material = blackMaterial;
        lightOn = false;
    }
    
    public void LampOnBulb()
    {
        if(lampBulbObjectRenderer == null) return;
        lampBulbObjectRenderer.material = lightMaterial;
        lightOn = true;
    }
    public void LampOffBulb()
    {
        if(lampBulbObjectRenderer == null) return;
        lampBulbObjectRenderer.material = glassMaterial;
        lightOn = false;
    }

    public void UpdateFlapUpState(bool flapUp)
    {
        blackFlapUp = flapUp;
        // lampBulbPlane.SetActive(!flapUp);
    }


    private void OnEnable()
    {
        AnimationHandlerScript.BlackCoverFlapUp += UpdateFlapUpState;
    }

    private void OnDisable()
    {
        AnimationHandlerScript.BlackCoverFlapUp -= UpdateFlapUpState;
    }
}
