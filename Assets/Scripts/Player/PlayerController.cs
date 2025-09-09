using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public float EngineAcceleration;
    [Tooltip("The curve to use when calculating acceleration while the throttle is continually pressed.")]
    public AnimationCurve EngineAccelerationCurve;
    [Tooltip("The number of seconds until the acceleration reaches the max value defined above.")]
    public int SecondsToMaxAcceleration;
    [Tooltip("Deceleration due to friction.")]
    public float FrictionDeceleration;
    [Tooltip("Deceleration while applying the brake.")]
    public float BrakeDeceleration;
    public float MaxSpeed;
    public float TurningSpeed;
    [Tooltip("Maximum wheel angle (in either direction).")]
    public float FullLockAngle;
    [Tooltip("Reduce the turning speed as the speed approaches the max speed. A value of 0.5 would result in a 50% decrease in turning speed at max speed.")]
    public float SpeedTurningSpeedReduction;

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

    private bool isControlDisabled = true;

    private float Wheelbase => (FrontLeftWheel.transform.position - RearLeftWheel.transform.position).magnitude;

    public float CurrentSpeed => currentSpeed;

    private float sustainedAccelerationTime;

    void Start()
    {
        EventController.Instance.RaceEvents.RaceStarted += (_, _) => isControlDisabled = false;

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
        if (isControlDisabled)
        {
            return;
        }

        var throttleInputValue = accelerateAction.ReadValue<float>();
        sustainedAccelerationTime = throttleInputValue > 0 ? sustainedAccelerationTime + Time.deltaTime : 0;

        var accelerationModifier = EngineAccelerationCurve.Evaluate(Math.Min(sustainedAccelerationTime, SecondsToMaxAcceleration) / SecondsToMaxAcceleration);

        var currentAcceleration = throttleInputValue != 0 ? EngineAcceleration * throttleInputValue * accelerationModifier - (BrakeDeceleration * brakeAction.ReadValue<float>()) : -FrictionDeceleration;

        currentSpeed = Mathf.Clamp(currentSpeed + currentAcceleration * Time.deltaTime, 0, MaxSpeed);

        var moveInputValue = moveAction.ReadValue<Vector2>().x;

        var turningSpeedModifier = 1 - (currentSpeed / MaxSpeed * SpeedTurningSpeedReduction);

        Debug.Log(turningSpeedModifier);

        if (moveInputValue != 0)
        {
            currentLockAngle = Mathf.Clamp(currentLockAngle + moveInputValue * Time.deltaTime * TurningSpeed * turningSpeedModifier, -FullLockAngle, FullLockAngle);
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
