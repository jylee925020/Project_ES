using UnityEngine;
/// <summary>
/// 주요 게임 오브젝트의 참조를 보관하고 제공하는 싱글톤 매니저 클래스입니다.
/// </summary>
public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance { get; private set; }

    public Player Player { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Player = FindFirstObjectByType<Player>();
    }
}