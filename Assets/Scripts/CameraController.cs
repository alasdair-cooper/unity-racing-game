using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Vector3 Offset;
    public Vector2 Sensitivity;
    public float ReOrientateSpeed;
    public float SecondsUntilReOrientation;

    private InputAction lookAction;
    private Camera Camera;
    public Component Player;

    private float timeWithoutInput;

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

            var offset = Camera.transform.position - Player.transform.position;

            if (offset != InitialOffset() && timeWithoutInput > SecondsUntilReOrientation)
            {
                var rotation = Quaternion.RotateTowards(Quaternion.identity, Quaternion.FromToRotation(offset, InitialOffset()), ReOrientateSpeed * Time.deltaTime);

                Camera.transform.position = Player.transform.position + rotation * offset;
                Camera.transform.LookAt(Player.transform.position);

                timeWithoutInput += Time.deltaTime;
            }

            return;
        }

        {
            var offset = Camera.transform.position - Player.transform.position;
            var horizontalAxis = Vector3.Cross(Camera.transform.up, offset);
            var rotation = Quaternion.AngleAxis(delta.x * Sensitivity.x, Player.transform.up) * Quaternion.AngleAxis(delta.y * Sensitivity.y, horizontalAxis);

            Camera.transform.position = Player.transform.position + rotation * offset;
            Camera.transform.LookAt(Player.transform.position);

            timeWithoutInput = 0;
        }

    }
}