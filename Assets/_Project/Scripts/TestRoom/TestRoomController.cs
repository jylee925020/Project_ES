using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 테스트 룸 환경을 설정하는 클래스로, 각 메서드는 UI버튼으로 호출됨.
/// </summary>
public class TestRoomController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Player player;
    [SerializeField] private Transform playerSpawnPoint;

    [Header("Monster")]
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private Transform monsterSpawnPoint;

    private readonly List<GameObject> spawnedMonsters = new();


    public void ResetRoom()
    {
        ClearMonsters();

        player.Revive();
        player.ResetPosition(playerSpawnPoint.position);
        player.Stop();
    }

    public void SpawnMonster()
    {
        if (monsterPrefab == null || monsterSpawnPoint == null)
            return;

        GameObject monster = Instantiate(
            monsterPrefab,
            monsterSpawnPoint.position,
            monsterSpawnPoint.rotation
        );

        MonsterAI ai = monster.GetComponent<MonsterAI>();
        if (ai != null)
            ai.enabled = monsterAIEnabled;
        MonsterAttack attack = monster.GetComponent<MonsterAttack>();

        if (attack != null)
            attack.SetAttackEnabled(monsterAttackEnabled);

        spawnedMonsters.Add(monster);
    }

    public void ClearMonsters()
    {
        for (int i = spawnedMonsters.Count - 1; i >= 0; i--)
        {
            if (spawnedMonsters[i] != null)
                Destroy(spawnedMonsters[i]);
        }

        spawnedMonsters.Clear();
    }

    private bool monsterAIEnabled = true;
    private bool monsterAttackEnabled = true;

    public void SetMonsterAI(bool enabled)
    {
        monsterAIEnabled = enabled;

        foreach (GameObject monster in spawnedMonsters)
        {
            if (monster == null)
                continue;

            MonsterAI ai = monster.GetComponent<MonsterAI>();

            if (ai != null)
                ai.enabled = enabled;
        }
    }

    public void SetMonsterAttack(bool enabled)
    {
        monsterAttackEnabled = enabled;

        foreach (GameObject monster in spawnedMonsters)
        {
            if (monster == null)
                continue;

            MonsterAttack attack = monster.GetComponent<MonsterAttack>();

            if (attack != null)
                attack.SetAttackEnabled(enabled);
        }
    }

    public void RevivePlayer()
    {
        if (player == null)
            return;

        player.Revive();
    }
}