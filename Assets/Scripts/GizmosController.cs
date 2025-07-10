using System;
using System.Collections.Generic;
using UnityEngine;

public class GizmosController : MonoBehaviour
{
    private readonly List<Func<GizmoInfo>> _gizmoGenerators = new();

    public void RegisterGizmo(Func<GizmoInfo> gizmoGenerator)
    {
        _gizmoGenerators.Add(gizmoGenerator);
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            foreach (var generator in _gizmoGenerators)
            {
                generator().Draw();
            }
        }
    }
}

public abstract class GizmoInfo
{
    public GizmoInfo(Color color) => Color = color;

    public Color Color { get; }

    public abstract void Draw();
}

public class LineGizmoInfo : GizmoInfo
{
    public LineGizmoInfo(Color color, Vector3 start, Vector3 end) : base(color)
    {
        Start = start;
        End = end;
    }

    public Vector3 Start { get; }

    public Vector3 End { get; }

    public override void Draw()
    {
        var prevColor = Gizmos.color;
        Gizmos.color = Color;
        Gizmos.DrawLine(Start, End);
        Gizmos.color = prevColor;
    }
}

public class CircleGizmoInfo : GizmoInfo
{
    public CircleGizmoInfo(Color color, Vector3 origin, float radius) : base(color)
    {
        Origin = origin;
        Radius = radius;
    }

    public Vector3 Origin { get; }

    public float Radius { get; }

    public override void Draw()
    {
        var prevColor = Gizmos.color;
        Gizmos.color = Color;
        Gizmos.DrawSphere(Origin, Radius);
        Gizmos.color = prevColor;
    }
}