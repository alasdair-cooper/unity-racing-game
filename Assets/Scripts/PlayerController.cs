using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float EngineAcceleration;
    public float BrakeDeceleration;
    public float FrictionDeceleration;
    public float AirResistanceMultiplier;

    public float TurningSpeed;
    public float FullLockAngle;

    private InputAction accelerateAction;
    private InputAction brakeAction;
    private InputAction moveAction;

    private float currentSpeed = 0;
    private float currentLockAngle = 0;

    private float AirResistanceDeceleration => 1 / AirResistanceMultiplier * currentSpeed / EngineAcceleration;

    public float SteeringAngle => currentLockAngle;

    void Start()
    {
        accelerateAction = InputSystem.actions.FindAction("accelerate");
        brakeAction = InputSystem.actions.FindAction("brake");
        moveAction = InputSystem.actions.FindAction("move");
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

        // playerTransform.Rotate(0, SteeringAngleDegrees * Time.deltaTime * TurningSpeed, 0);

        transform.position += currentSpeed * Time.deltaTime * transform.forward;
    }
}
