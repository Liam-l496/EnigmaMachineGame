
using UnityEngine;
using Cinemachine;

public class CinemachineCameraMovement : CinemachineExtension
{
    [Header("Movement Settings")]
    public bool enableMouseLook = true;
    public bool enableParallax = true;
    public bool enableIdleSway = true;

    [Header("Individual Camera Multipliers")]
    [Range(0f, 2f)]
    public float movementMultiplier = 1f;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        // Only apply the movement once per camera update.
        if (stage != CinemachineCore.Stage.Finalize)
            return;

        if (GlobalCameraMovement.Instance == null)
            return;

        GlobalCameraMovement movement =
            GlobalCameraMovement.Instance;

        // Mouse-controlled rotation
        if (enableMouseLook)
        {
            Vector2 angles = movement.SmoothedAngles;

            Quaternion mouseRotation = Quaternion.Euler(
                angles.y,
                angles.x,
                0f
            );

            state.OrientationCorrection =
                state.OrientationCorrection *
                Quaternion.Slerp(
                    Quaternion.identity,
                    mouseRotation,
                    movementMultiplier
                );
        }

        // Positional parallax
        if (enableParallax)
        {
            Vector3 offset = movement.PositionOffset;

            state.PositionCorrection +=
                state.RawOrientation * (offset * movementMultiplier);
        }

        // Idle sway
        if (enableIdleSway)
        {
            Vector3 sway = movement.RotationOffset;

            Quaternion swayRotation = Quaternion.Euler(sway);

            state.OrientationCorrection =
                state.OrientationCorrection *
                Quaternion.Slerp(
                    Quaternion.identity,
                    swayRotation,
                    movementMultiplier
                );
        }
    }
}
