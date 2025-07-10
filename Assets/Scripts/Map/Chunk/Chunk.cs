using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public int chunkSize;
    public RoomDrawer drawerMap;
    public bool autoDrawChunk = false;
    public bool autoCombineChunk = false;
    public bool dontDisableMesh = false;
    void Start()
    {
        if (autoDrawChunk) DrawRoom();
        if (autoCombineChunk) CombineBlocks();
    }
    public void DrawRoom()
    {
        drawerMap.DrawMap();
    }
    [NaughtyAttributes.Button] public void CombineBlocks()
    {
        StartCoroutine(CombineMeshes());
    }
    public GameObject GetRandomRoom(bool isInitialRoom, bool isFinalRoom)
    {
        string path = isInitialRoom || isFinalRoom ? isInitialRoom ? "Prefabs/Map/AncientWall/Rooms/Initial" : "Prefabs/Map/AncientWall/Rooms/Final" : "Prefabs/Map/AncientWall/Rooms/General";
        GameObject[] rooms = Resources.LoadAll<GameObject>(path);
        GameObject room = rooms[Random.Range(0, rooms.Length)];
        return room;
    }
    public IEnumerator CombineMeshes()
    {
        yield return new WaitForSeconds(0.2f);
        Dictionary<Texture2D, List<GameObject>> materialToCombineInstances = new Dictionary<Texture2D, List<GameObject>>();
        for (int i = 0; i < drawerMap.mapBlocks.Count; i++)
        {
            Texture2D material = (Texture2D)drawerMap.mapBlocks[i].meshRenderer.sharedMaterial.GetTexture("_BaseMap");
            if (!materialToCombineInstances.ContainsKey(material))
            {
                materialToCombineInstances[material] = new List<GameObject>();
            }
            materialToCombineInstances[material].Add(drawerMap.mapBlocks[i].gameObject);
        }
        for (int i = 0; i < drawerMap.bridges.Count; i++)
        {
            if (drawerMap.bridges.TryGetValue(drawerMap.bridges.ElementAt(i).Key, out RoomDrawer.BridgesInfo bridges))
            {
                for (int b = 0; b < bridges.blocks.Length; b++)
                {
                    Texture2D material = (Texture2D)bridges.blocks[b].meshRenderer.sharedMaterial.GetTexture("_BaseMap");
                    if (!materialToCombineInstances.ContainsKey(material))
                    {
                        materialToCombineInstances[material] = new List<GameObject>();
                    }
                    materialToCombineInstances[material].Add(bridges.blocks[b].gameObject);
                }
            }
        }
        for (int i = 0; i < drawerMap.decorationsBlocks.Count; i++)
        {
            Texture2D material = (Texture2D)drawerMap.decorationsBlocks[i].meshRenderer.sharedMaterial.GetTexture("_BaseMap");
            if (!materialToCombineInstances.ContainsKey(material))
            {
                materialToCombineInstances[material] = new List<GameObject>();
            }
            materialToCombineInstances[material].Add(drawerMap.decorationsBlocks[i].gameObject);
        }
        foreach (var entry in materialToCombineInstances)
        {
            List<GameObject> bloks = entry.Value;
            CombineInstance[] combineInstances = new CombineInstance[bloks.Count];
            for (int i = 0; i < bloks.Count; i++)
            {
                MeshFilter meshFilter = bloks[i].GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    combineInstances[i].mesh = meshFilter.sharedMesh;
                    combineInstances[i].transform = bloks[i].transform.localToWorldMatrix;
                }
            }
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combineInstances, true, true);
            GameObject combinedObject = new GameObject();
            combinedObject.layer = LayerMask.NameToLayer("Map");
            combinedObject.transform.SetParent(transform);
            MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
            combinedMeshFilter.mesh = combinedMesh;
            MeshRenderer combinedMeshRenderer = combinedObject.AddComponent<MeshRenderer>();
            combinedObject.AddComponent<MeshCollider>();
            combinedMeshRenderer.material = bloks[0].GetComponent<MeshRenderer>().material;
            combinedObject.name = $"CombinedMesh {bloks[0].GetComponent<MeshRenderer>().material.name}";
            combinedObject.isStatic = true;
            foreach (GameObject obj in bloks)
            {
                Destroy(obj);
            }
        }
        if(!dontDisableMesh) DisableCombinedMesh();
    }
    public void DisableCombinedMesh()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer)){
                meshRenderer.enabled = false;
            }
            else{
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    public void EnabledCombinedMesh()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer)){
                meshRenderer.enabled = true;
            }
            else{
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, chunkSize * Vector3.one);
    }
}
