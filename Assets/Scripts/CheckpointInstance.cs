using System;
using UnityEngine;

public class CheckpointInstance : MonoBehaviour
{
    private GameController gameController;

    public event EventHandler<CheckpointPassedEventArgs> OnPassed;

    void Start() => gameController = GetComponentInParent<GameController>();

    void OnTriggerEnter(Collider other) => OnPassed.Invoke(this, new CheckpointPassedEventArgs(name));
}

public class CheckpointPassedEventArgs {

    public CheckpointPassedEventArgs(string name) => Name = name;

    public string Name { get; }
}