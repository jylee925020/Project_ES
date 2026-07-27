using UnityEngine;

/// <summary>
/// 플레이어의 입력을 받아 이동, 점프, 공격 등을 제어하는 클래스
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerPhysics physics;
    [SerializeField] private PlayerAttack attack;
    [SerializeField] private PlayerState state;

    private float lastPressedDirection;     // 마지막으로 눌린 방향키를 저장하여 양쪽 키를 동시에 누를 때 이동 방향을 결정

    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleAttack();
    }

    #region Movement

    private void HandleMovement()
    {
        // 왼쪽, 오른쪽 키 입력 상태를 확인
        // 한쪽을 누른 상태에서 다른 쪽을 누르면 이미 누른 방향을 유지하고 이동만 반대로 함.
        bool leftHeld = Input.GetKey(KeyCode.LeftArrow);

        bool rightHeld = Input.GetKey(KeyCode.RightArrow);

        bool leftPressed = Input.GetKeyDown(KeyCode.LeftArrow);

        bool rightPressed = Input.GetKeyDown(KeyCode.RightArrow);

        if (leftPressed)
        {
            lastPressedDirection = -1f;
        }

        if (rightPressed)
        {
            lastPressedDirection = 1f;
        }

        float moveX = ResolveMoveInput(leftHeld, rightHeld);
        bool bothDirectionsHeld = leftHeld && rightHeld;

        // 공격 중엔 지상이라면 이동을 제한
        if (attack.IsAttacking)
        {
            HandleAttackingMovement(moveX);
            return;
        }

        // 양쪽 키를 동시에 누르면 이동만 나중에 누른 방향으로 하고 바라보는 방향은 유지
        movement.Move(moveX, !bothDirectionsHeld);
    }

    // 둘다 누른 상태라면 마지막에 눌린 방향 반환.
    // 그 외엔 왼쪽은 -1, 오른쪽은 1, 아무것도 안누르면 0 반환
    private float ResolveMoveInput(bool leftHeld, bool rightHeld)
    {
        if (leftHeld && rightHeld)
        {
            return lastPressedDirection;
        }

        if (leftHeld)
        {
            return -1f;
        }

        if (rightHeld)
        {
            return 1f;
        }

        return 0f;
    }

    private void HandleAttackingMovement(float moveX)
    {
        if (state.IsGrounded)
        {
            movement.StopImmediately();
            return;
        }

        movement.Move(moveX, false);
    }

    #endregion

    #region Jump

    private void HandleJump()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            movement.CutJump();
        }

        // 공격 중엔 점프 막힘.
        if (attack.IsAttacking)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            movement.Jump();
        }
    }

    #endregion

    #region Attack

    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            attack.Attack();
        }
    }

    #endregion
}