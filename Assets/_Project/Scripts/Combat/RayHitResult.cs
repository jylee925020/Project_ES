using UnityEngine;

public readonly struct RayHitResult
{
    public Vector2 Origin { get; }
    public Vector2 EndPoint { get; }
    public RaycastHit2D Hit { get; }

    public bool HasHit => Hit.collider != null;

    public RayHitResult(
        Vector2 origin,
        Vector2 endPoint,
        RaycastHit2D hit)
    {
        Origin = origin;
        EndPoint = endPoint;
        Hit = hit;
    }
}