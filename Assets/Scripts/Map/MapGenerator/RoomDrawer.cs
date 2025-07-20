using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomDrawer : MonoBehaviour
{
    [SerializeField] NavMeshSurface navMeshSurface;
    public GameObject blocksContainer;
    public List<MapBlock> mapBlocks = new List<MapBlock>();
    public List<MapDecoration> decorationsBlocks = new List<MapDecoration>();
    public SerializedDictionary<DirectionBridges, BridgesInfo> bridges;
    public MapBlock[] mapBlocksForSpawn;
    public Vector3[] spawnPos;
    public bool autoInit;
    public int size;
    public Vector2 chunkPos;
    void Start()
    {
        if (autoInit) DrawMap();
    }
    [NaughtyAttributes.Button]
    public void DrawMap()
    {
        StartCoroutine(DrawRoom());
    }
    IEnumerator DrawRoom()
    {
        DrawBlocks();
        yield return new WaitForSeconds(0.1f);
        DrawBridges();
        yield return new WaitForSeconds(0.1f);
        BuildNavMesh();
    }
    void DrawBlocks()
    {
        for (int i = 0; i < mapBlocks.Count; i++)
        {
            mapBlocks[i].DrawBlock();
        }
        for (int i = 0; i < decorationsBlocks.Count; i++)
        {
            decorationsBlocks[i].DrawBlock();
        }
    }
    public void ActiveBridges(DirectionBridges directionBridges)
    {
        if (bridges.TryGetValue(directionBridges, out BridgesInfo bridgesInfo))
        {
            bridgesInfo.bridge.SetActive(true);
        }
    }
    public void DrawBridges()
    {
        List<DirectionBridges> toRemove = new List<DirectionBridges>();

        foreach (var kvp in bridges)
        {
            DirectionBridges direction = kvp.Key;
            var bridgeData = kvp.Value;

            if (bridgeData.bridge.activeSelf)
            {
                foreach (var block in bridgeData.blocks)
                {
                    block.DrawBlock();
                }
            }
            else
            {
                Destroy(bridgeData.bridge);
                toRemove.Add(direction);
            }
        }
        foreach (var dir in toRemove)
        {
            bridges.Remove(dir);
        }
    }
    void BuildNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
    void UpdateNavMesh()
    {
        //navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }
    [NaughtyAttributes.Button]
    public void GetSpawnPos()
    {
        List<Vector3> newSpawnPos = new List<Vector3>();
        foreach (MapBlock mapBlock in mapBlocksForSpawn)
        {
            newSpawnPos.Add(mapBlock.transform.position + Vector3.up);
        }
        spawnPos = newSpawnPos.ToArray();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Character>().characterHud.characterUi.mapUi.currentRoom = chunkPos;
        }
    }
    [Serializable] public class BridgesInfo
    {
        public GameObject bridge;
        public MapBlock[] blocks;
    }
    public enum DirectionBridges
    {
        Forward = 0,
        Back = 1,
        Left = 2,
        Rigth = 3
    }
}
