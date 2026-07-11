using UnityEngine;
/// <summary>
/// 플레이어의 입력을 받아 이동, 점프 등을 제어하는 클래스
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        movement.Move(moveX);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            movement.Jump();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            movement.CutJump();
        }
    }
}