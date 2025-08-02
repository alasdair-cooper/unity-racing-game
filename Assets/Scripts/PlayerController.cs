using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public float EngineAcceleration;
    public float BrakeDeceleration;
    public float FrictionDeceleration;
    public float AirResistanceMultiplier;
    public float TurningSpeed;
    public float FullLockAngle;

    [Header("Wheels")]
    public Component FrontLeftWheel;
    public Component FrontRightWheel;
    public Component RearLeftWheel;
    public Component RearRightWheel;

    private InputAction accelerateAction;
    private InputAction brakeAction;
    private InputAction moveAction;

    private float currentSpeed = 0;
    private float currentLockAngle = 0;

    private float AirResistanceDeceleration => 1 / AirResistanceMultiplier * currentSpeed / EngineAcceleration;

    private float Wheelbase => (FrontLeftWheel.transform.position - RearLeftWheel.transform.position).magnitude;

    void Start()
    {
        accelerateAction = InputSystem.actions.FindAction("accelerate");
        brakeAction = InputSystem.actions.FindAction("brake");
        moveAction = InputSystem.actions.FindAction("move");

        GizmosController.Instance.RegisterGizmo(() =>
        {
            var r = CalculateBackWheelTurningRadius(-currentLockAngle);

            return new LineGizmoInfo(Color.cyan, RearLeftWheel.transform.position, RearLeftWheel.transform.position - RearLeftWheel.transform.right * r, currentLockAngle < 0);
        });

        GizmosController.Instance.RegisterGizmo(() =>
        {
            var r = CalculateFrontWheelTurningRadius(-currentLockAngle);

            return new LineGizmoInfo(Color.cyan, FrontLeftWheel.transform.position, FrontLeftWheel.transform.position - FrontLeftWheel.transform.right * r, currentLockAngle < 0);
        });

        GizmosController.Instance.RegisterGizmo(() =>
       {
           var r = CalculateBackWheelTurningRadius(currentLockAngle);

           return new LineGizmoInfo(Color.cyan, RearRightWheel.transform.position, RearRightWheel.transform.position + RearRightWheel.transform.right * r, currentLockAngle > 0);
       });

        GizmosController.Instance.RegisterGizmo(() =>
        {
            var r = CalculateFrontWheelTurningRadius(currentLockAngle);

            return new LineGizmoInfo(Color.cyan, FrontRightWheel.transform.position, FrontRightWheel.transform.position + FrontRightWheel.transform.right * r, currentLockAngle > 0);
        });
    }

    void Update()
    {
        var currentAcceleration = EngineAcceleration * accelerateAction.ReadValue<float>() - (BrakeDeceleration * brakeAction.ReadValue<float>()) - FrictionDeceleration - AirResistanceDeceleration;

        currentSpeed = Mathf.Max(0, currentSpeed + currentAcceleration);

        var moveInputValue = moveAction.ReadValue<Vector2>().x;

        if (moveInputValue != 0)
        {
            currentLockAngle = Mathf.Clamp(currentLockAngle + moveAction.ReadValue<Vector2>().x * Time.deltaTime * TurningSpeed, -FullLockAngle, FullLockAngle);
        }
        else if (currentLockAngle > 0)
        {
            currentLockAngle = Mathf.Clamp(currentLockAngle - Time.deltaTime * TurningSpeed, 0, FullLockAngle);
        }
        else if (currentLockAngle < 0)
        {
            currentLockAngle = Mathf.Clamp(currentLockAngle + Time.deltaTime * TurningSpeed, -FullLockAngle, 0);
        }

        if (currentLockAngle > 0)
        {
            var turningRadius = CalculateBackWheelTurningRadius(currentLockAngle);
            var turnCentre = RearRightWheel.transform.position + RearRightWheel.transform.right * turningRadius;

            var turningCircumference = turningRadius * 2 * Mathf.PI;

            // Calculate the percentage of a full rotation to turn (in degrees)
            var rotationAngle = currentSpeed * Time.deltaTime / turningCircumference * 360;

            transform.RotateAround(turnCentre, transform.up, rotationAngle);
        }
        else if (currentLockAngle < 0)
        {
            var turningRadius = CalculateBackWheelTurningRadius(-currentLockAngle);
            var turnCentre = RearLeftWheel.transform.position - RearLeftWheel.transform.right * turningRadius;

            var turningCircumference = turningRadius * 2 * Mathf.PI;

            // Calculate the percentage of a full rotation to turn (in degrees)
            var rotationAngle = currentSpeed * Time.deltaTime / turningCircumference * 360;

            transform.RotateAround(turnCentre, transform.up, -rotationAngle);
        }
        else
        {
            transform.position += currentSpeed * Time.deltaTime * transform.forward;
        }

        FrontLeftWheel.transform.localRotation = Quaternion.AngleAxis(currentLockAngle, FrontLeftWheel.transform.up);
        FrontRightWheel.transform.localRotation = Quaternion.AngleAxis(currentLockAngle, FrontRightWheel.transform.up);
    }

    float CalculateFrontWheelTurningRadius(float angle) => Wheelbase / Mathf.Sin(Mathf.Deg2Rad * angle);

    float CalculateBackWheelTurningRadius(float angle) => Wheelbase / Mathf.Tan(Mathf.Deg2Rad * angle);
}
