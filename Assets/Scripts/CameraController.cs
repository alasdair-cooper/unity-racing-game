using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public int Offset = 5;
    public Vector2 Sensitivity = Vector2.one;
    public float ReOrientateSpeed = 0.5f;
    public float SecondsUntilReOrientation = 2;

    private InputAction lookAction;
    private InputAction jumpAction;
    private Camera Camera;
    public Component Player;

    private float timeWithoutInput;

    private void Start()
    {
        lookAction = InputSystem.actions.FindAction("look");
        jumpAction = InputSystem.actions.FindAction("jump");
        Camera = GetComponent<Camera>();
        Camera.transform.position = Vector3.back * Offset;
    }

    private void Update()
    {
        var delta = lookAction.ReadValue<Vector2>();

        var horizontalAxis = Vector3.Cross(Camera.transform.up, Camera.transform.position - Player.transform.position);

        // Translate to origin
        // Rotate 
        // Calculate new position based on forward + follow distance

        if (Mathf.Approximately(delta.magnitude, 0))
        {
            // if (timeWithoutInput > SecondsUntilReOrientation && !Mathf.Approximately(Vector3.Dot(Player.transform.up, horizontalAxis), 0))
            // {
            //     var rot = Quaternion.RotateTowards(Camera.transform.rotation, Quaternion.FromToRotation(Player.transform.position, Camera.transform.position), ReOrientateSpeed);

            //     Camera.transform.rotation = rot;
            // }

            // timeWithoutInput += Time.deltaTime;
        }
        else
        {
            var rotation = Camera.transform.rotation * Quaternion.AngleAxis(delta.x * Sensitivity.x, Player.transform.up) * Quaternion.AngleAxis(delta.y * Sensitivity.y, horizontalAxis);

            Camera.transform.SetPositionAndRotation(Vector3.zero, rotation);
            Camera.transform.position = Camera.transform.forward * -Offset;

            // Camera.transform.RotateAround(Player.transform.position, Player.transform.up, delta.x * Sensitivity.x);
            // Camera.transform.RotateAround(Player.transform.position, horizontalAxis, delta.y * Sensitivity.y);

            // Camera.transform.LookAt(Player.transform.position);

            timeWithoutInput = 0;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Vector3.up * -100, Vector3.up * 100);

        var horizontalAxis = Vector3.Cross(Camera.transform.up, Camera.transform.position - Player.transform.position);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(horizontalAxis * -100, horizontalAxis * 100);
    }
}