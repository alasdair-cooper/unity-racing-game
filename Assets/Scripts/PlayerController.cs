using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float EngineAcceleration;
    public float BrakeDeceleration;
    public float FrictionDeceleration;
    public float AirResistanceMultiplier;

    private InputAction accelerateAction;
    private InputAction decelerateAction;

    private Transform playerTransform;

    private float currentSpeed = 0;

    public float AirResistanceDeceleration => (1 / AirResistanceMultiplier) * currentSpeed / EngineAcceleration;

    void Start()
    {
        accelerateAction = InputSystem.actions.FindAction("accelerate");
        decelerateAction = InputSystem.actions.FindAction("decelerate");

        playerTransform = GetComponent<Transform>();
    }

    void Update()
    {
        var currentAcceleration = EngineAcceleration * accelerateAction.ReadValue<float>() - (BrakeDeceleration * decelerateAction.ReadValue<float>()) - FrictionDeceleration - AirResistanceDeceleration;

        currentSpeed = Mathf.Max(0, currentSpeed + currentAcceleration);

        playerTransform.position += currentSpeed * Time.deltaTime * playerTransform.forward;
    }
}
