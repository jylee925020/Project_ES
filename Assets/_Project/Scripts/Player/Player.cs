using UnityEngine;

/// <summary>
/// 플레이어 내부 컴포넌트들의 중재자.
/// 외부에서 플레이어 전체에 대한 명령을 받을 때 진입점 역할을 한다.
/// </summary>
public class Player : MonoBehaviour
{
    private PlayerState playerState;
    private PlayerHealth playerHealth;
    private PlayerHitReaction playerHitReaction;
    private PlayerPhysics playerPhysics;

    private void Awake()
    {
        playerState = GetComponent<PlayerState>();
        playerHealth = GetComponent<PlayerHealth>();
        playerHitReaction = GetComponent<PlayerHitReaction>();
        playerPhysics = GetComponent<PlayerPhysics>();
    }

    public void Stop()
    {
        playerPhysics.Stop();
    }

    public void ResetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void Revive()
    {
        playerHealth.RestoreFullHealth();
        playerHitReaction.CancelHitStun();
        playerState.Revive();
    }
}