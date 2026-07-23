using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어의 공격 실행과 공격 타이밍을 관리하는 클래스
/// 공격을 누르면 공격 코루틴이 실행되어 선딜, 공격 프리팹 생성, 후딜 순으로 진행됨.
/// 선딜 중에 착지하면 지상 공격으로 전환되어 나감.
/// 후딜 중에 착지하면 후딜이 캔슬되어 바로 공격 종료됨.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private PlayerPhysics physics;

    [Header("Temporary Attack")]        // 추후 무기가 보유하게 할 데이터들. 임시로 여기서 지정함.
    [SerializeField] private AttackObject attackPrefab;          // 공격 프리팹
    [SerializeField] private Transform attackSpawnPoint;    // 공격 프리팹 생성 위치 
    [SerializeField] private float startupTime = 0.12f;     // 선딜
    [SerializeField] private float recoveryTime = 0.28f;    // 후딜
    [SerializeField] private int attackDamage = 1;          // 공격력


    public event Action<string> OnAttacked;

    private bool isAttacking;
    public bool IsAttacking => isAttacking;

    private Coroutine attackRoutine;     // 현재 공격 코루틴

    private bool startedInAir;           // 공격 시작 시 공중이었는지

    // 공중에서 공격 중일 때 선딜 중에 착지하면 지상 공격으로 전환됨.
    [SerializeField] private float landingStartupTime = 0.05f;   // 선딜 중 착지 시 지상 공격으로 전환되는 시간
    private bool isInStartup;            // 현재 선딜 중인지
    private bool isInRecovery;           // 현재 후딜 중인지


    #region lifecycle
    private void OnEnable()
    {
        physics.OnLanded += HandleLanded;
    }

    private void OnDisable()
    {
        physics.OnLanded -= HandleLanded;
    }
    #endregion

    // 공격 실행
    public void Attack()
    {
        if (isAttacking)
            return;

        startedInAir = !physics.IsGrounded;
        attackRoutine = StartCoroutine(AttackRoutine(startupTime));
    }

    // 공격 코루틴, 선딜을 설정할 수 있음.
    private IEnumerator AttackRoutine(float currentStartupTime)
    {
        isAttacking = true;
        isInStartup = true;
        isInRecovery = false;

        OnAttacked?.Invoke("Swing_1");  // 공격 이벤트 호출 (애니메이션 , 사운드 등 에서 사용)
        // 추후 무기별로 다른 이벤트를 호출하도록 수정할 예정

        yield return new WaitForSeconds(currentStartupTime); // 선딜 대기
        isInStartup = false;    // 선딜 종료

        SpawnAttack();          // 공격 프리팹 생성

        isInRecovery = true;    // 후딜 시작

        yield return new WaitForSeconds(recoveryTime); // 후딜 대기

        // 후딜 중에 착지 이벤트가 발생하면 이 코루틴이 중단되고 FinishAttack()가 호출됨. (HandleLanded()에서)

        FinishAttack();     // 공격 종료
    }

    // 착지 이벤트에서 호출되는 함수. 공격 중에 착지 시를 처리함.
    private void HandleLanded()
    {
        if (!isAttacking || !startedInAir)
            return;

        // 선딜 중에 착지하면 지상 공격으로 전환
        if (isInStartup)
        {
            ConvertToGroundAttack();
            return;
        }

        // 후딜 중에 착지하면 후딜만 캔슬
        if (isInRecovery)
        {
            CancelAttackRecovery();
        }
    }

    // 지상 공격으로 전환
    private void ConvertToGroundAttack()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }

        startedInAir = false;
        isInStartup = false;
        isInRecovery = false;
        attackRoutine = null;

        attackRoutine = StartCoroutine(
            AttackRoutine(landingStartupTime)
        );
    }

    // 후딜 캔슬
    private void CancelAttackRecovery()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }

        FinishAttack();
    }

    // 공격 종료. 변수 초기화.
    private void FinishAttack()
    {
        isAttacking = false;
        startedInAir = false;
        isInStartup = false;
        isInRecovery = false;
        attackRoutine = null;
    }

    // 공격 프리팹 생성 (추후 무기별로 다른 공격 프리팹을 생성하도록 수정할 예정)
    private void SpawnAttack()
    {
        AttackObject.SpawnAsChild(
            attackPrefab,
            attackSpawnPoint,
            attackDamage,
            AttackFaction.Player
        );
    }
}