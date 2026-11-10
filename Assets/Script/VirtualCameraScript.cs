using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UnityEditor.AssetImporters;

public class VirtualCameraScript : MonoBehaviour
{
    [Header("Camera Object")]
    public CinemachineVirtualCamera cameraStarting;
    public CinemachineVirtualCamera cameraOverall;
    public CinemachineVirtualCamera cameraRotorWindow;
    public CinemachineVirtualCamera cameraBlackCoverFlap;
    public CinemachineVirtualCamera cameraPlugBoard;
    public CinemachineVirtualCamera cameraKeyBoard;
    public CinemachineVirtualCamera cameraRotor;
    public CinemachineVirtualCamera cameraLampBoard;




    public const int CAMERA_ACTIVE = 20;
    public const int CAMERA_DEACTIVE = 10;

    // public static event Action BlackCoverFlapUp;
    // public static bool BlackCoverFlapUpEnabled = false;



    void Start()
    {
        ActivateCameraStarting();
    }

    private void DeactiveAllCamera()
    {
        cameraStarting.Priority = CAMERA_DEACTIVE;
        cameraOverall.Priority = CAMERA_DEACTIVE;
        cameraRotorWindow.Priority = CAMERA_DEACTIVE;
        cameraBlackCoverFlap.Priority = CAMERA_DEACTIVE;
        cameraPlugBoard.Priority = CAMERA_DEACTIVE;
        cameraKeyBoard.Priority = CAMERA_DEACTIVE;
        cameraRotor.Priority = CAMERA_DEACTIVE;
        cameraLampBoard.Priority = CAMERA_DEACTIVE;
        // DeactivateAllActions();
    }

    // private void DeactivateAllActions()
    // {
    //     if(BlackCoverFlapUpEnabled)
    //     {
    //         BlackCoverFlapUp?.Invoke();
    //         BlackCoverFlapUpEnabled = false;
    //     }
    // }

    public void ActivateCameraStarting()
    {
        DeactiveAllCamera();
        cameraStarting.Priority = CAMERA_ACTIVE;
    }

    public void ActivateCameraOverall()
    {
        DeactiveAllCamera();
        cameraOverall.Priority = CAMERA_ACTIVE;
    }

    public void ActivateCameraRotorWindow()
    {
        DeactiveAllCamera();
        cameraRotorWindow.Priority = CAMERA_ACTIVE;
    }

    public void ActivateCameraBlackCoverFlap()
    {
        DeactiveAllCamera();
        cameraBlackCoverFlap.Priority = CAMERA_ACTIVE;
        // BlackCoverFlapUp?.Invoke();
        // BlackCoverFlapUpEnabled = true;
    }

    public void ActivateCameraPlugBoard()
    {
        DeactiveAllCamera();
        cameraPlugBoard.Priority = CAMERA_ACTIVE;
    }
    
    public void ActivateCameraKeyBoard()
    {
        DeactiveAllCamera();
        cameraKeyBoard.Priority = CAMERA_ACTIVE;
    }    
    public void ActivateCameraRotor()
    {
        DeactiveAllCamera();
        cameraRotor.Priority = CAMERA_ACTIVE;
    }    

    public void ActivateCameraLampBoard()
    {
        DeactiveAllCamera();
        cameraLampBoard.Priority = CAMERA_ACTIVE;
    }
}
