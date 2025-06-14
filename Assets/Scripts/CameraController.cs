using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Vector3 Offset;
    public Vector2 Sensitivity;
    public float ReOrientateSpeed;
    public float SecondsUntilReOrientation;
    public int MinPitch;
    public int MaxPitch;
    public bool InvertPitch;
    public int MaxYaw;
    public int MinYaw;

    private InputAction lookAction;
    private Camera Camera;
    public Component Player;

    private float timeWithoutInput;
    private Coroutine returnToOffsetCoroutine;

    private List<Vector3> gizmoLines = new();

    private Vector3 InitialOffset() => Offset;

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

        var offset = Camera.transform.position - Player.transform.position;

        var rotation = Quaternion.AngleAxis(delta.x * Sensitivity.x, VerticalAxis) * Quaternion.AngleAxis(delta.y * Sensitivity.y * (InvertPitch ? -1 : 1), HorizontalAxis);

        offset = rotation * offset;

        var verticalAngle = Vector3.SignedAngle(Vector3.ProjectOnPlane(offset, VerticalAxis), offset, HorizontalAxis);

        if (verticalAngle > MaxPitch)
        {
            offset = ClampToPitch(MaxPitch);
        }
        else if (verticalAngle < MinPitch)
        {
            offset = ClampToPitch(MinPitch);
        }

        var horizontalAngle = Vector3.SignedAngle(Vector3.ProjectOnPlane(offset, Player.transform.up), -Player.transform.forward, VerticalAxis);

        if (horizontalAngle > MaxYaw)
        {
            offset = ClampToYaw(MaxYaw);
        }
        else if (horizontalAngle < MinYaw)
        {
            offset = ClampToYaw(MinYaw);
        }

        MoveCamera(offset);

        timeWithoutInput = 0;

        return;

        Vector3 ClampToPitch(int pitch)
        {
            var clampedRotation = Quaternion.AngleAxis(pitch, HorizontalAxis);
            var correctOffsetDirection = Vector3.ProjectOnPlane(offset, VerticalAxis);
            // Normalize & multiply by the original magnitude as not preserved by projection
            return clampedRotation * correctOffsetDirection.normalized * offset.magnitude;
        }

        Vector3 ClampToYaw(int yaw)
        {
            var clampedRotation = Quaternion.AngleAxis(180 - yaw, VerticalAxis);
            var correctOffsetDirection = Vector3.ProjectOnPlane(offset, Player.transform.right);
            return clampedRotation * correctOffsetDirection.normalized * offset.magnitude;
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

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Player.transform.position, Player.transform.forward);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(Player.transform.position, HorizontalAxis * 50);

            Gizmos.color = Color.blue;
            foreach (var line in gizmoLines)
            {
                Gizmos.DrawLine(Player.transform.position, line);
            }
        }
    }
}