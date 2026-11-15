using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class AnimationHandlerScript : MonoBehaviour
{

    public static AnimationHandlerScript Instance;

    [Header("Cinemachine")]
    [SerializeField] CinemachineBrain brain;
    public static bool IsBlending { get; private set; }
    public static event Action<bool> OnBlendStateChanged;

    [Header("Virtual Camera Script")]
    [SerializeField] VirtualCameraScript virtualCamera;

    [Header("Box Wooden")]
    [SerializeField] Animator animatorBoxWooden;
    [SerializeField] BoxCollider colliderBoxWoodenLid;
    [SerializeField] BoxCollider colliderPlugBoardFlap;
    private int openWoodenBoxHash;
    private int plugBoardLidOpenHash;

    [Header("Black Cover Flap")]
    [SerializeField] Animator animatorBlackFlapCover;
    [SerializeField] BoxCollider colliderBlackFlapCover;
    [SerializeField] BoxCollider colliderPowerLever;
    [SerializeField] BoxCollider colliderWindowFlap;
    private int hashBlackFlapCover;
    private int hashPowerLever;
    private int hashWindowFlap;

    public static event Action<bool> WindowFlapUp;
    public static event Action<bool> BlackCoverFlapUp;

    [Header("Reflector")]
    [SerializeField] Animator animatorReflector;
    [SerializeField] BoxCollider colliderReflector;
    private int hashReflectorHolding;
    public static event Action<bool> ReflectorHolding;

    [Header("BatteryBox")]
    [SerializeField] Animator animatorBatteryBox;
    [SerializeField] BoxCollider colliderBatteryBox;
    private int hashBatteryLidOpen;

    [Header("KeyBoardHolder")]
    // [SerializeField] Animator animatorActuatorBar;
    [SerializeField] BoxCollider colliderKeyBoardHolder;
    // private int hashActuatorBar;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        openWoodenBoxHash = Animator.StringToHash("boxOpen");
        plugBoardLidOpenHash = Animator.StringToHash("plugBoardLidOpen");
        animatorBoxWooden.speed = 0f;

        hashBlackFlapCover = Animator.StringToHash("isCoverLifting");
        hashPowerLever = Animator.StringToHash("isPowerOn");
        hashWindowFlap = Animator.StringToHash("isWindowLifting");
        
        hashReflectorHolding = Animator.StringToHash("isHolding");

        hashBatteryLidOpen = Animator.StringToHash("BatteryLidOpen");

        // hashActuatorBar = Animator.StringToHash("");
    }

    void Update()
    {
        UpdateColliderWoodenBox();
        UpdateColliderBlackCoverFlap();
        UpdateColliderReflector();
        UpdateColliderBatteryBox();

        if (brain == null) return;

        bool currentlyBlending = brain.IsBlending;

        // Check if the state changed this frame
        if (currentlyBlending != IsBlending)
        {
            IsBlending = currentlyBlending;
            OnBlendStateChanged?.Invoke(IsBlending);
        }
    }

    /// <summary>
    ///  Wooden Box Functions
    /// </summary>
    public void UpdateWoodenLidState()
    {
        animatorBoxWooden.speed = 1f;
        animatorBoxWooden.SetBool(openWoodenBoxHash, !animatorBoxWooden.GetBool(openWoodenBoxHash));
        if (animatorBoxWooden.GetBool(openWoodenBoxHash))
        {
            virtualCamera.ActivateCameraOverall();
        }
        else
        {
            virtualCamera.ActivateCameraStarting();
        }
    }

    public void UpdatePlugBoardState()
    {
        // plugBoardOpenState = !plugBoardOpenState;
        animatorBoxWooden.SetBool(plugBoardLidOpenHash, !animatorBoxWooden.GetBool(plugBoardLidOpenHash));
        if (animatorBoxWooden.GetBool(plugBoardLidOpenHash))
        {
            virtualCamera.ActivateCameraPlugBoard();
        }
        else{
            virtualCamera.ActivateCameraOverall();
        }
    }

    private void UpdateColliderWoodenBox()
    {
        colliderPlugBoardFlap.enabled = animatorBoxWooden.GetBool(openWoodenBoxHash);
        colliderBoxWoodenLid.enabled = !animatorBoxWooden.GetBool(plugBoardLidOpenHash) && !animatorBlackFlapCover.GetBool(hashBlackFlapCover);
    }

    /// <summary>
    /// Black Flap Cover Functions
    /// </summary>
    public void UpdateBlackCoverFlapState()
    {
        animatorBlackFlapCover.SetBool(hashBlackFlapCover, !animatorBlackFlapCover.GetBool(hashBlackFlapCover));
        if (animatorBlackFlapCover.GetBool(hashBlackFlapCover))
        {
            BlackCoverFlapUp?.Invoke(true);
            virtualCamera.ActivateCameraBlackCoverFlap();
        }
        else
        {
            BlackCoverFlapUp?.Invoke(false);
            virtualCamera.ActivateCameraOverall();
        }
    }

    public bool ReturnBlackCoverFlapState()
    {
        return animatorBlackFlapCover.GetBool(hashBlackFlapCover);
    }

    public void UpdatePowerOnState()
    {
        animatorBlackFlapCover.SetBool(hashPowerLever, !animatorBlackFlapCover.GetBool(hashPowerLever));
        GameManagerScript.Instance.powerOn = animatorBlackFlapCover.GetBool(hashPowerLever);
    }

    public void UpdateWindowFlapState()
    {
        animatorBlackFlapCover.SetBool(hashWindowFlap, ! animatorBlackFlapCover.GetBool(hashWindowFlap));
        if (animatorBlackFlapCover.GetBool(hashWindowFlap))
        {
            virtualCamera.ActivateCameraRotorWindow();
            WindowFlapUp?.Invoke(true);
        }
        else
        {
            virtualCamera.ActivateCameraOverall();
            WindowFlapUp?.Invoke(false);
        }
    }
    private void UpdateColliderBlackCoverFlap()
    {
        colliderBlackFlapCover.enabled = !(animatorBlackFlapCover.GetBool(hashPowerLever) || animatorBlackFlapCover.GetBool(hashWindowFlap))
                                        && animatorReflector.GetBool(hashReflectorHolding)
                                        && !animatorBatteryBox.GetBool(hashBatteryLidOpen);
        colliderPowerLever.enabled = !animatorBlackFlapCover.GetBool(hashBlackFlapCover);
        colliderWindowFlap.enabled = !animatorBlackFlapCover.GetBool(hashBlackFlapCover);
    }

    /// <summary>
    /// Reflector Functions
    /// </summary>

    public void UpdateReflectorState()
    {
        animatorReflector.SetBool(hashReflectorHolding, !animatorReflector.GetBool(hashReflectorHolding));
        if (!animatorReflector.GetBool(hashReflectorHolding))
        {
            virtualCamera.ActivateCameraRotor();
            ReflectorHolding?.Invoke(false);
        }
        else
        {
            virtualCamera.ActivateCameraBlackCoverFlap();
            ReflectorHolding?.Invoke(true);
        }
        GameManagerScript.Instance.ClearRotorSelected();
    }
    public void UpdateColliderReflector()
    {
        colliderReflector.enabled = animatorBlackFlapCover.GetBool(hashBlackFlapCover);
    }

    /// <summary>
    /// Battery Box Function
    /// </summary>
    public void UpdateBatteryBoxState()
    {
        animatorBatteryBox.SetBool(hashBatteryLidOpen, !animatorBatteryBox.GetBool(hashBatteryLidOpen));
    }
    public void UpdateColliderBatteryBox()
    {
        colliderBatteryBox.enabled = animatorBlackFlapCover.GetBool(hashBlackFlapCover);
    }

    /// <summary>
    /// KeyBoardHolder
    /// </summary>
    public void UpdateKeyBoardHolderState()
    {
        if (virtualCamera.cameraKeyBoard.Priority <= virtualCamera.cameraOverall.Priority)
        {
            virtualCamera.ActivateCameraKeyBoard();
        }
        else
        {
            virtualCamera.ActivateCameraOverall();
        }
    }

    /// <summary>
    /// Lamp Board 
    /// </summary>
    public void UpdateLampBoardState()
    {
        if (virtualCamera.cameraLampBoard.Priority <= virtualCamera.cameraBlackCoverFlap.Priority)
        {
            virtualCamera.ActivateCameraLampBoard();
        }
        else
        {
            virtualCamera.ActivateCameraBlackCoverFlap();
        }
    }
}
