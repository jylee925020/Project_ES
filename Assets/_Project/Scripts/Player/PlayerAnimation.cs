using UnityEngine;

/// <summary>
/// 플레이어의 상태에 따라 애니메이션을 제어하는 클래스
/// 앞으로도 다른 클래스에서 여기를 의존하지 않게 유지할 것
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerPhysics physics;

    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");

    private void OnEnable()
    {
        movement.OnJumped += PlayJump;
    }

    private void OnDisable()
    {
        movement.OnJumped -= PlayJump;
    }

    private void Update()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        animator.SetBool(IsGroundedHash, physics.IsGrounded);
        animator.SetBool(IsRunningHash, Mathf.Abs(physics.CurrentVelocityX) > 0.01f);
    }

    private void PlayJump()
    {
        animator.SetTrigger(JumpHash);
    }
}