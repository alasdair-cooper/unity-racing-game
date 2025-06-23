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

    private InputAction accelerateAction;
    private InputAction brakeAction;
    private InputAction moveAction;

    private Transform playerTransform;

    private float currentSpeed = 0;

    private float AirResistanceDeceleration => 1 / AirResistanceMultiplier * currentSpeed / EngineAcceleration;

    private float SteeringAngleDegrees => moveAction.ReadValue<Vector2>().x * 45;

    void Start()
    {
        accelerateAction = InputSystem.actions.FindAction("accelerate");
        brakeAction = InputSystem.actions.FindAction("brake");
        moveAction = InputSystem.actions.FindAction("move");

        playerTransform = GetComponent<Transform>();
    }

    void Update()
    {
        var currentAcceleration = EngineAcceleration * accelerateAction.ReadValue<float>() - (BrakeDeceleration * brakeAction.ReadValue<float>()) - FrictionDeceleration - AirResistanceDeceleration;

        currentSpeed = Mathf.Max(0, currentSpeed + currentAcceleration);

        playerTransform.Rotate(0, SteeringAngleDegrees * Time.deltaTime * TurningSpeed, 0);

        playerTransform.position += currentSpeed * Time.deltaTime * playerTransform.forward;
    }
}
