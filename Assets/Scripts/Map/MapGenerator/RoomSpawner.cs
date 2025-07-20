using System;
using NaughtyAttributes;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class RoomSpawner : MonoBehaviour
{
    public RoomDrawer roomDrawer;
    public NavMeshSurface surface;
    public GameObject blockWalls;
    [ReadOnly]
    public int _amountEnemies;
    public Action<int> OnAllEnemiesDie;
    public GameObject characterPrefab;
    public int amountSpawns;
    public bool initializeSpawn;
    public Character player;
    public int amountEnemies
    {
        get => _amountEnemies;
        set
        {
            if (_amountEnemies != value)
            {
                _amountEnemies = value;
                if (_amountEnemies == 0)
                {
                    OnAllEnemiesDie?.Invoke(_amountEnemies);
                }
            }
        }
    }
    public void InitializeFigth()
    {
        amountSpawns = UnityEngine.Random.Range(2, 5);
        OnAllEnemiesDie += ValidateAllEnemiesDie;
        blockWalls.SetActive(true);
        SpawnEnemies();
    }
    public void ValidateAllEnemiesDie(int currentEnemies)
    {
        if (amountSpawns <= 0)
        {
            FinishBattle();
        }
        else if (amountEnemies <= 0)
        {
            SpawnEnemies();
        }
    }
    public void SpawnEnemies()
    {
        amountSpawns--;
        amountEnemies = UnityEngine.Random.Range(4, 10);
        for (int i = 0; i < amountEnemies; i++)
        {
            Character character = Instantiate(characterPrefab, transform.position, Quaternion.identity).GetComponent<Character>();
            character.OnIsActiveChange += CharacterDead;
            character.transform.localPosition = GetRandomPointInTransformArea();
            _ = character.InitializeCharacter();
            character.characterMove.SetTarget(player.transform);
        }
    }
    void CharacterDead(bool isLive)
    {
        if (!isLive)
        {
            amountEnemies--;
        }
    }
    public Vector3 GetRandomPointInTransformArea()
    {
        Vector3 center = transform.position;
        Vector3 halfExtents = Vector3.one * roomDrawer.size / 2f;

        for (int i = 0; i < 30; i++)
        {
            Vector3 randomOffset = new Vector3(
                UnityEngine.Random.Range(-halfExtents.x, halfExtents.x),
                0f, // Suponemos área en XZ (plano)
                UnityEngine.Random.Range(-halfExtents.z, halfExtents.z)
            );

            Vector3 randomPoint = center + randomOffset;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        Debug.LogWarning("No se encontró un punto válido en el NavMesh dentro del área transform.");
        return center;
    }
    void FinishBattle()
    {
        OnAllEnemiesDie = null;
        blockWalls.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!initializeSpawn)
            {
                initializeSpawn = true;
                player = other.GetComponent<Character>();
                InitializeFigth();
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, Vector3.one * roomDrawer.size + Vector3.one);
    }
}
