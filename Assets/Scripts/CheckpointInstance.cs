using UnityEngine;

public class CheckpointInstance : MonoBehaviour
{
    private GameController gameController;

    void Start() => gameController = GetComponentInParent<GameController>();

    void OnTriggerEnter(Collider other) => gameController.OnCheckpointPassed(name);
}