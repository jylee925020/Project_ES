using UnityEngine;

/// <summary>
/// 플레이어의 상태에 따라 애니메이션을 제어하는 클래스
/// 앞으로도 다른 클래스에서 여기를 의존하지 않게 유지할 것
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private PlayerMovement movement;
    private PlayerPhysics physics;
    private PlayerAttack attack;
    private PlayerState state;


    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
    private static readonly int AttackHash = Animator.StringToHash("Attack");


    #region life cycle
    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        physics = GetComponent<PlayerPhysics>();
        attack = GetComponent<PlayerAttack>();
        state = GetComponent<PlayerState>();
    }
    private void OnEnable()
    {
        movement.OnJumped += PlayJump;
        attack.OnAttacked += PlayAttack;
    }

    private void OnDisable()
    {
        movement.OnJumped -= PlayJump;
        attack.OnAttacked -= PlayAttack;
    }

    private void Update()
    {
        UpdateState();
    }
    #endregion

    private void UpdateState()
    {
        animator.SetBool(IsGroundedHash, state.IsGrounded);
        animator.SetBool(IsRunningHash, Mathf.Abs(physics.CurrentVelocityX) > 0.01f);
    }

    private void PlayJump()
    {
        animator.SetTrigger(JumpHash);
    }


    // 무기로부터 공격 모션 이름을 받아서 애니메이션을 재생함.
    private void PlayAttack(string motionName)
    {
        string stateName = state.IsGrounded
            ? $"Ground_{motionName}"
            : $"Air_{motionName}";

        animator.Play(stateName);
    }
}