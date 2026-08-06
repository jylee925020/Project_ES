using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [SerializeField] private Transform body;

    private MonsterMovement movement;
    private MonsterAI ai;
    private MonsterHealth health;

    private float previousDirection = 1f;

    private void Awake()
    {
        movement = GetComponent<MonsterMovement>();
        ai = GetComponent<MonsterAI>();
        health = GetComponent<MonsterHealth>();
    }

    private void FixedUpdate()
    {
        if (health.IsDead)
        {
            movement.Stop();
            ai.enabled = false;
            enabled = false;
            return;
        }
        float direction = ai.MoveDirection;

        UpdateFacing(direction);

        if (ai.ShouldMove)
        {
            movement.Move(direction);
        }
        else
        {
            movement.Stop();
        }
    }

    private void UpdateFacing(float direction)
    {
        if (Mathf.Approximately(direction, previousDirection))
            return;

        FlipBody(direction);
        previousDirection = direction;
    }

    private void FlipBody(float direction)
    {
        Vector3 scale = body.localScale;

        scale.x =
            Mathf.Abs(scale.x) * Mathf.Sign(direction);

        body.localScale = scale;
    }
}