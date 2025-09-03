using UnityEngine;

public class CheckpointInstance : MonoBehaviour
{
    void OnTriggerEnter(Collider other) => EventController.Instance.CheckpointEvents.OnCheckpointPassed(name);
}