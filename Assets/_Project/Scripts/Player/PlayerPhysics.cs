using UnityEngine;

/// <summary>
/// 플레이어 Rigidbody2D의 속도를 읽고 제어한다.
/// </summary>
public class PlayerPhysics : MonoBehaviour
{
    private Rigidbody2D rb;

    public float CurrentVelocityX => rb.linearVelocity.x;
    public float CurrentVelocityY => rb.linearVelocity.y;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    #region Velocity Control

    public void SetVelocityX(float velocityX)
    {
        rb.linearVelocity = new Vector2(
            velocityX,
            rb.linearVelocity.y
        );
    }

    public void SetVelocityY(float velocityY)
    {
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            velocityY
        );
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        rb.linearVelocity = newVelocity;
    }

    public void MoveVelocityX(
        float targetVelocityX,
        float changeSpeed
    )
    {
        float newVelocityX = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetVelocityX,
            changeSpeed * Time.deltaTime
        );

        SetVelocityX(newVelocityX);
    }

    #endregion
}