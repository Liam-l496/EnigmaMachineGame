
using UnityEngine;

public class GlobalCameraMovement : MonoBehaviour
{
    public static GlobalCameraMovement Instance { get; private set; }

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.08f;
    public float horizontalLimit = 3f;
    public float verticalLimit = 2f;
    public float rotationSmoothTime = 0.15f;

    [Header("Camera Parallax")]
    public float horizontalParallax = 0.025f;
    public float verticalParallax = 0.02f;

    [Header("Idle Sway")]
    public float swaySpeed = 1.2f;
    public float swayAmount = 0.15f;
    public float swayPositionAmount = 0.003f;

    public Vector2 LookAngles { get; set; }
    public Vector2 SmoothedAngles { get; set; }

    public Vector3 PositionOffset { get; private set; }
    public Vector3 RotationOffset { get; private set; }

    private Vector2 angleVelocity;
    private float idleTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        HandleMouseInput();
        HandleSway();
    }

    private void HandleMouseInput()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        Vector2 angles = LookAngles;

        angles.x += mouseX * mouseSensitivity;
        angles.y -= mouseY * mouseSensitivity;

        angles.x = Mathf.Clamp(
            angles.x, -horizontalLimit, horizontalLimit
        );

        angles.y = Mathf.Clamp(
            angles.y, -verticalLimit, verticalLimit
        );

        LookAngles = angles;

        Vector2 smoothedAngles = SmoothedAngles;

        smoothedAngles.x = Mathf.SmoothDamp(
            smoothedAngles.x,
            LookAngles.x,
            ref angleVelocity.x,
            rotationSmoothTime
        );

        smoothedAngles.y = Mathf.SmoothDamp(
            smoothedAngles.y,
            LookAngles.y,
            ref angleVelocity.y,
            rotationSmoothTime
        );

        SmoothedAngles = smoothedAngles;
    }

    private void HandleSway()
    {
        idleTime += Time.deltaTime;

        float swayX = Mathf.Sin(idleTime * swaySpeed);
        float swayY = Mathf.Cos(idleTime * swaySpeed * 0.7f);

        RotationOffset = new Vector3(
            swayY * swayAmount,
            swayX * swayAmount,
            swayX * swayAmount * 0.15f
        );

        PositionOffset = new Vector3(
            (SmoothedAngles.x / horizontalLimit) * horizontalParallax,
            (-SmoothedAngles.y / verticalLimit) * verticalParallax +
            swayY * swayPositionAmount,
            0f
        );
    }

    public void ResetMovement()
    {
        LookAngles = Vector2.zero;
    }
}
