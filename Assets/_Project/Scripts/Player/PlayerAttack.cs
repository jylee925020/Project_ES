using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어의 공격 실행과 공격 타이밍을 관리한다.
/// 공격은 선딜, 공격 판정 생성, 후딜 순서로 진행되며
/// 착지해도 남은 후딜은 유지된다.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    private PlayerState state;

    [Header("Temporary Attack")]        // 추후 무기가 보유하게 할 데이터들. 임시로 여기서 지정함.
    [SerializeField] private AttackObject attackPrefab;          // 공격 프리팹
    [SerializeField] private Transform attackSpawnPoint;    // 공격 프리팹 생성 위치 

    [SerializeField, Min(0f)] private float startupTime = 0.12f;
    [SerializeField, Min(0f)] private float activeTime = 0.05f;
    [SerializeField, Min(0f)] private float recoveryTime = 0.28f;

    [SerializeField] private int attackDamage = 1;          // 공격력


    [Header("Temporary Animation")]
    [SerializeField] private AnimationData groundAttackAnimation;
    [SerializeField] private AnimationData airAttackAnimation;

    public event Action<AnimationData> OnAttackStarted;

    private Coroutine attackRoutine;     // 현재 공격 코루틴
    private AttackObject currentAttackObject;       // 이번 공격에 생성한 공격 오브젝트

    private bool startedInAir;           // 공격 시작 시 공중이었는지

    private enum AttackPhase
    {
        None,       // 공격 중이지 않음.
        Startup,    // 공격 선딜 중
        Active,     // 공격 중
        Recovery    // 공격 후딜 중
    }

    private AttackPhase currentPhase;


    #region lifecycle
    private void Awake()
    {
        state = GetComponent<PlayerState>();
    }
    private void OnEnable()
    {
        state.OnActionChanged += HandleActionChanged;
        state.OnLanded += HandleLanded;
    }

    private void OnDisable()
    {
        state.OnActionChanged -= HandleActionChanged;
        state.OnLanded -= HandleLanded;
    }
    #endregion

    // 공격 실행 시도
    public bool TryAttack()
    {
        if (!state.CanAttack)
            return false;

        state.BeginAction(PlayerActionType.Attack);

        startedInAir = !state.IsGrounded;
        attackRoutine = StartCoroutine(AttackRoutine());

        return true;
    }


    private bool IsAttackRunning =>
        currentPhase != AttackPhase.None ||
        attackRoutine != null;

    // 외부에서 호출하는 강제 공격 종료
    public void ForceInterrupt()
    {
        if (!IsAttackRunning)
            return;

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }

        FinishAttack();
    }

    // 선딜 후에 공격 오브젝트 생성, 활성 시간 후에 공격 오브젝트 판정 끄기, 이후 후딜 진행
    private IEnumerator AttackRoutine()
    {
        currentPhase = AttackPhase.Startup;

        RequestAttackAnimation();

        yield return new WaitForSeconds(startupTime);

        currentPhase = AttackPhase.Active;
        SpawnAttack();

        yield return new WaitForSeconds(activeTime);

        DisableCurrentAttackObject();

        currentPhase = AttackPhase.Recovery;

        yield return new WaitForSeconds(recoveryTime);

        FinishAttack();
    }

    private void RequestAttackAnimation()
    {
        AnimationData animationData = state.IsGrounded
            ? groundAttackAnimation
            : airAttackAnimation;

        if (animationData == null)
            return;

        OnAttackStarted?.Invoke(animationData);
    }

    private void DisableCurrentAttackObject()
    {
        if (currentAttackObject == null)
            return;

        currentAttackObject.DisableDamage();
        currentAttackObject = null;
    }

    // 공격 종료. 변수 초기화.
    private void FinishAttack()
    {
        DisableCurrentAttackObject();

        state.EndAction(PlayerActionType.Attack);

        startedInAir = false;
        currentPhase = AttackPhase.None;
        attackRoutine = null;
    }


    // 공격 프리팹 생성 (추후 무기별로 다른 공격 프리팹을 생성하도록 수정할 예정)
    private void SpawnAttack()
    {
        currentAttackObject = AttackObject.SpawnAsChild(
            attackPrefab,
            attackSpawnPoint,
            attackDamage,
            AttackFaction.Player
        );
    }

    // 액션 변경 이벤트에 호출됨
    private void HandleActionChanged(PlayerActionType action)
    {
        if (action == PlayerActionType.Dead)
        {
            ForceInterrupt();
        }
    }

    // 착지 이벤트에 호출
    private void HandleLanded()
    {
        if (!IsAttackRunning)
            return;

        if (!startedInAir)
            return;

        startedInAir = false;

        if (currentPhase == AttackPhase.Startup)
        {
            RestartAsGroundAttack();
        }
    }

    // 선딜 중 착지 시 지상 공격으로 전환
    private void RestartAsGroundAttack()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }

        attackRoutine = StartCoroutine(AttackRoutine());
    }
}