using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Vector3 Offset;
    public Vector2 Sensitivity;
    public float ReOrientateSpeed;
    public float SecondsUntilReOrientation;
    public int MinVerticalPitch;
    public int MaxVerticalPitch;

    private InputAction lookAction;
    private Camera Camera;
    public Component Player;

    private float timeWithoutInput;
    private Coroutine returnToOffsetCoroutine;


    private Vector3 InitialOffset() => Offset;

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

        var horizontalAxis = Vector3.Cross(Vector3.up, offset.normalized);

        var rotation = Quaternion.AngleAxis(delta.x * Sensitivity.x, Vector3.up) * Quaternion.AngleAxis(delta.y * Sensitivity.y, horizontalAxis);

        Camera.transform.position = Player.transform.position + rotation * offset;
        Camera.transform.LookAt(Player.transform);

        timeWithoutInput = 0;

        return;
    }

    private void ReturnToOffset()
    {
        returnToOffsetCoroutine = StartCoroutine(ReturnToOffsetCoroutine());
    }

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

            Camera.transform.position = Player.transform.position + currentOffset;
            Camera.transform.LookAt(Player.transform.position);

            yield return null;
        }

        Camera.transform.position = Player.transform.position + targetOffset;
        Camera.transform.LookAt(Player.transform.position);

        returnToOffsetCoroutine = null;
    }
}