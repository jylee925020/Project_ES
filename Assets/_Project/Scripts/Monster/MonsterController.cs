using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [SerializeField] private Transform body;

    private MonsterMovement movement;
    private MonsterAI ai;

    private float previousDirection = 1f;

    private void Awake()
    {
        movement = GetComponent<MonsterMovement>();
        ai = GetComponent<MonsterAI>();
    }

    private void FixedUpdate()
    {
        if (!ai.PatrolEnabled)
        {
            movement.Stop();
            return;
        }
        float direction = ai.MoveDirection;

        movement.Move(direction);

        if (direction != previousDirection)
        {
            FlipBody(direction);
            previousDirection = direction;
        }
    }

    private void FlipBody(float direction)
    {
        Vector3 scale = body.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction);
        body.localScale = scale;
    }
}