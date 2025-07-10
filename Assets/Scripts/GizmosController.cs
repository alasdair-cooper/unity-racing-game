using System;
using System.Collections.Generic;
using System.Linq;
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
            foreach (var gizmo in _gizmoGenerators.Select(x => x()).Where(x => x.ShouldDraw))
            {
                gizmo.Draw();
            }
        }
    }
}

public abstract class GizmoInfo
{
    public GizmoInfo(Color color, bool shouldDraw)
    {
        Color = color;
        ShouldDraw = shouldDraw;
    }

    public Color Color { get; }

    public bool ShouldDraw { get; }

    public abstract void Draw();
}

public class LineGizmoInfo : GizmoInfo
{
    public LineGizmoInfo(Color color, Vector3 start, Vector3 end, bool shouldDraw = true) : base(color, shouldDraw)
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
    public CircleGizmoInfo(Color color, Vector3 origin, float radius, bool shouldDraw = true) : base(color, shouldDraw)
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