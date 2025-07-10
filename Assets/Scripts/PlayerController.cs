using System;
using System.Runtime.CompilerServices;
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

    [Header("Attachments")]
    public Component FrontLeftWheel;
    public Component FrontRightWheel;
    public Component RearLeftWheel;
    public Component RearRightWheel;
    public GizmosController Gizmos;

    private InputAction accelerateAction;
    private InputAction brakeAction;
    private InputAction moveAction;

    private float currentSpeed = 0;
    private float currentLockAngle = 0;

    private float AirResistanceDeceleration => 1 / AirResistanceMultiplier * currentSpeed / EngineAcceleration;

    public float SteeringAngle => currentLockAngle;

    private float Wheelbase => (FrontLeftWheel.transform.position - RearLeftWheel.transform.position).magnitude;

    void Start()
    {
        accelerateAction = InputSystem.actions.FindAction("accelerate");
        brakeAction = InputSystem.actions.FindAction("brake");
        moveAction = InputSystem.actions.FindAction("move");

        Gizmos.RegisterGizmo(() =>
        {
            var r = CalculateBackWheelTurningRadius(-SteeringAngle);

            return new LineGizmoInfo(Color.cyan, RearLeftWheel.transform.position, RearLeftWheel.transform.position - RearLeftWheel.transform.right * r, SteeringAngle < 0);
        });

        Gizmos.RegisterGizmo(() =>
        {
            var r = CalculateFrontWheelTurningRadius(-SteeringAngle);

            return new LineGizmoInfo(Color.cyan, FrontLeftWheel.transform.position, FrontLeftWheel.transform.position - FrontLeftWheel.transform.right * r, SteeringAngle < 0);
        });

        Gizmos.RegisterGizmo(() =>
       {
           var r = CalculateBackWheelTurningRadius(SteeringAngle);

           return new LineGizmoInfo(Color.cyan, RearRightWheel.transform.position, RearRightWheel.transform.position + RearRightWheel.transform.right * r, SteeringAngle > 0);
       });

        Gizmos.RegisterGizmo(() =>
        {
            var r = CalculateFrontWheelTurningRadius(SteeringAngle);

            return new LineGizmoInfo(Color.cyan, FrontRightWheel.transform.position, FrontRightWheel.transform.position + FrontRightWheel.transform.right * r, SteeringAngle > 0);
        });
    }

    void Update()
    {
        var currentAcceleration = EngineAcceleration * accelerateAction.ReadValue<float>() - (BrakeDeceleration * brakeAction.ReadValue<float>()) - FrictionDeceleration - AirResistanceDeceleration;

        currentSpeed = Mathf.Max(0, currentSpeed + currentAcceleration);

        var moveInputValue = moveAction.ReadValue<Vector2>().x;

        if (moveInputValue != 0)
        {
            currentLockAngle = Mathf.Clamp(SteeringAngle + moveAction.ReadValue<Vector2>().x * Time.deltaTime * TurningSpeed, -FullLockAngle, FullLockAngle);
        }
        else if (SteeringAngle > 0)
        {
            currentLockAngle = Mathf.Clamp(SteeringAngle - Time.deltaTime * TurningSpeed, 0, FullLockAngle);
        }
        else if (SteeringAngle < 0)
        {
            currentLockAngle = Mathf.Clamp(SteeringAngle + Time.deltaTime * TurningSpeed, -FullLockAngle, 0);
        }

        if (currentLockAngle > 0)
        {
            // turn left
        }
        else if (currentLockAngle < 0)
        {
            // turn right
        }
        else
        {
            transform.position += currentSpeed * Time.deltaTime * transform.forward;
        }

        FrontLeftWheel.transform.rotation = Quaternion.AngleAxis(SteeringAngle, FrontLeftWheel.transform.up);
        FrontRightWheel.transform.rotation = Quaternion.AngleAxis(SteeringAngle, FrontRightWheel.transform.up);
    }

    float CalculateFrontWheelTurningRadius(float angle) => Wheelbase / Mathf.Sin(Mathf.Deg2Rad * angle);

    float CalculateBackWheelTurningRadius(float angle) => Wheelbase / Mathf.Tan(Mathf.Deg2Rad * angle);
}
