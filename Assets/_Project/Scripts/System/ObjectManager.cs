using UnityEngine;

/// <summary>
/// 주요 게임 오브젝트의 참조를 보관하고 제공하는 싱글톤 매니저 클래스입니다.
/// TODO : 참조 등록보다 검색이 빠른 경우에 대비할 것
/// </summary>
public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance { get; private set; }

    public Player Player { get; private set; }
    public PlayerHealth PlayerHealth { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Player = FindFirstObjectByType<Player>();
        PlayerHealth = Player.GetComponent<PlayerHealth>();
    }
}