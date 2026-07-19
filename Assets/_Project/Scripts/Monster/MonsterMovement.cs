using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [SerializeField] private Rigidbody2D rb;

    public float HorizontalVelocity => rb.linearVelocity.x;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    public void Move(float direction)
    {
        direction = Mathf.Clamp(direction, -1f, 1f);

        rb.linearVelocity = new Vector2(
            direction * moveSpeed,
            rb.linearVelocity.y
        );
    }

    public void Stop()
    {
        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );
    }
}