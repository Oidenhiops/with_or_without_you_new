using System;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public RoomDrawer roomDrawer;
    public GameObject blockWalls;
    int _amountEnemies;
    public Action<int> OnAllEnemiesDie;
    public GameObject characterPrefab;
    public int amountSpawns;
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
        blockWalls.SetActive(true);
        SpawnEnemies();
    }
    public void ValidateAllEnemiesDie(int currentEnemies)
    {
        if (amountSpawns == 0)
        {
            FinishBattle();
        }
        else
        {
            SpawnEnemies();
        }
    }
    public void SpawnEnemies()
    {
        amountSpawns--;
        // amountEnemies = UnityEngine.Random.Range(4, 10);
        amountEnemies = 1;
        for (int i = 0; i < amountEnemies; i++)
        {
            Character character = Instantiate(characterPrefab, transform.position, Quaternion.identity, transform).GetComponent<Character>();
            character.transform.localPosition = GetSpawnPosition();
            _ = character.InitializeCharacter();
        }
    }
    public Vector3 GetSpawnPosition()
    {
        return roomDrawer.spawnPos[UnityEngine.Random.Range(0, roomDrawer.spawnPos.Length - 1)];
    }
    void FinishBattle()
    {
        blockWalls.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(GetComponent<BoxCollider>());
            InitializeFigth();
        }
    }
}
