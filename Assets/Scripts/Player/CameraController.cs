using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public float FollowDistance;
    public float FollowPitch;
    [Tooltip("Speed at which the camera returns to the default offset.")]
    public float ReOrientateSpeed;
    [Tooltip("Number of seconds for camera controls to be idle before it returns to the default offset.")]
    public float SecondsUntilReOrientation;
    public int MinPitch;
    public int MaxPitch;
    public int MaxYaw;
    public int MinYaw;

    [Header("Controls")]
    public Vector2 Sensitivity;
    public bool InvertPitch;

    [Header("Attachments")]
    public Component Player;

    private InputAction lookAction;
    private Camera Camera;

    private float timeWithoutInput;
    private Coroutine returnToOffsetCoroutine;

    private Vector3 InitialOffset() => Quaternion.AngleAxis(FollowPitch, Player.transform.right) * ((-Player.transform.forward).normalized * FollowDistance);

    private Vector3 VerticalAxis => Player.transform.up;

    /// <summary>
    /// Get the horizontal axis as it pertains to the camera orientation.
    /// </summary>
    /// <remarks>
    /// This will always point in the direction of the positive x axis (right) relative to the camera.
    /// </remarks>
    private Vector3 HorizontalAxis => Vector3.Cross((Camera.transform.position - Player.transform.position).normalized, Vector3.up);

    private void Start()
    {
        lookAction = InputSystem.actions.FindAction("look");
        Camera = GetComponent<Camera>();
        Camera.transform.position = InitialOffset();

        GizmosController.Instance.RegisterGizmo(() => new LineGizmoInfo(Color.green, Player.transform.position, Player.transform.position + Player.transform.forward));
        GizmosController.Instance.RegisterGizmo(() => new LineGizmoInfo(Color.green, Player.transform.position, Player.transform.position + Player.transform.right));
    }

    private void Update()
    {
        var delta = lookAction.ReadValue<Vector2>();

        if (Mathf.Approximately(delta.magnitude, 0))
        {
            timeWithoutInput += Time.deltaTime;

            if (timeWithoutInput > SecondsUntilReOrientation && returnToOffsetCoroutine == null)
            {
                ReturnToOffset();
            }

            return;
        }

        CancelReturnToOffset();

        var rotation = Quaternion.AngleAxis(delta.x * Sensitivity.x, VerticalAxis) * Quaternion.AngleAxis(delta.y * Sensitivity.y * (InvertPitch ? -1 : 1), HorizontalAxis);
        var newPosition = ClampRotations(rotation * (Camera.transform.position - Player.transform.position));

        MoveCamera(newPosition);

        timeWithoutInput = 0;
    }

    private Vector3 ClampRotations(Vector3 position)
    {
        var verticalAngle = Vector3.SignedAngle(Vector3.ProjectOnPlane(position, VerticalAxis), position, HorizontalAxis);

        if (verticalAngle > MaxPitch)
        {
            position = ClampToPitch(MaxPitch);
        }
        else if (verticalAngle < MinPitch)
        {
            position = ClampToPitch(MinPitch);
        }

        var horizontalAngle = Vector3.SignedAngle(Vector3.ProjectOnPlane(position, VerticalAxis), -Player.transform.forward, VerticalAxis);

        if (horizontalAngle > MaxYaw)
        {
            position = ClampToYaw(MaxYaw);
        }
        else if (horizontalAngle < MinYaw)
        {
            position = ClampToYaw(MinYaw);
        }

        return position;

        Vector3 ClampToPitch(int pitch)
        {
            var clampedRotation = Quaternion.AngleAxis(pitch, HorizontalAxis);
            var correctOffsetDirection = Vector3.ProjectOnPlane(position, VerticalAxis);
            // Normalize & multiply by the original magnitude as not preserved by projection
            return clampedRotation * correctOffsetDirection.normalized * position.magnitude;
        }

        Vector3 ClampToYaw(int yaw)
        {
            var clampedRotation = Quaternion.AngleAxis(180 - yaw, VerticalAxis);
            var correctOffsetDirection = Vector3.ProjectOnPlane(position, Player.transform.right);
            return clampedRotation * correctOffsetDirection.normalized * position.magnitude;
        }
    }

    /// <summary>
    /// Move the camera to a new offset and look at the player.
    /// </summary>
    /// <param name="position">The offset to move to.</param>
    private void MoveCamera(Vector3 position)
    {
        Camera.transform.position = Player.transform.position + position;
        Camera.transform.LookAt(Player.transform);
    }

    /// <summary>
    /// Begin the repositioning back to the original offset.
    /// </summary>
    private void ReturnToOffset() => returnToOffsetCoroutine = StartCoroutine(ReturnToOffsetCoroutine());

    /// <summary>
    /// Abort the repositioning back to offset.
    /// </summary>
    private void CancelReturnToOffset()
    {
        if (returnToOffsetCoroutine is not null)
        {
            StopCoroutine(returnToOffsetCoroutine);
        }
        returnToOffsetCoroutine = null;
    }

    private IEnumerator ReturnToOffsetCoroutine()
    {
        var startOffset = Camera.transform.position - Player.transform.position;
        var targetOffset = InitialOffset();

        float duration = Vector3.Angle(startOffset, targetOffset) / ReOrientateSpeed;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            var currentOffset = Vector3.Slerp(startOffset, targetOffset, Mathf.SmoothStep(0f, 1f, t));

            MoveCamera(currentOffset);

            yield return null;
        }

        MoveCamera(targetOffset);

        returnToOffsetCoroutine = null;
    }
}