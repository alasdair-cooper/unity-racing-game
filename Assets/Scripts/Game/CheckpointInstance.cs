using UnityEngine;

public class CheckpointInstance : MonoBehaviour
{
    void OnTriggerEnter(Collider other) => EventController.Instance.OnCheckpointPassed(name);
}