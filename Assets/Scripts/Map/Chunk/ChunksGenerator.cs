using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class ChunksGenerator : MonoBehaviour
{
    public int chunkSize = 17;
    public int chunksX = 5;
    public int chunksZ = 5;
    public SerializedDictionary<Vector3Int,PositionChunk> positionsChunks = new SerializedDictionary<Vector3Int, PositionChunk>();
    public GameObject[] characters;
    GameObject chunkPrefab;
    private void Start()
    {
        StartCoroutine(GenerateChunks());
    }
    public IEnumerator GenerateChunks()
    {
        #region CreateGrid
        positionsChunks = GenerateMap(chunksX, chunksZ, out Vector3Int initalRoomPos, out Vector3Int finalRoomPos);
        chunkPrefab = Resources.Load<GameObject>("Prefabs/Map/Chunk");
        Vector2 offset = new Vector2(chunksX * chunkSize / 2f - (chunkSize / 2f), chunksZ * chunkSize / 2f - (chunkSize / 2f));
        for (int c = 0; c < positionsChunks.Count; c++)
        {
            InstanceChunk(offset, positionsChunks.ElementAt(c).Key);
        }

        positionsChunks[initalRoomPos].managementChunk.typeChunk = Chunk.TypeChunk.Initial;
        positionsChunks[finalRoomPos].managementChunk.typeChunk = Chunk.TypeChunk.Final;

        InstanceOtherRooms(Chunk.TypeChunk.RoomKey, out Vector3Int positionRoomKey);
        Vector3Int positionRoomKeyOut = positionRoomKey + Vector3Int.up * 2;
        positionsChunks.Add(positionRoomKeyOut, new PositionChunk());
        InstanceChunk(offset, positionRoomKeyOut);
        positionsChunks[positionRoomKeyOut].managementChunk.typeChunk = Chunk.TypeChunk.RoomKeyOut;
        // InstanceChunkWhitPortal(positionRoomKey, Chunk.TypeChunk.RoomKeyOut);
        // InstanceOtherRooms(offset, initalRoomPos, finalRoomPos, Chunk.TypeChunk.Special);

        // SetOtherRoom(initalRoomPos, finalRoomPos, Chunk.TypeChunk.Traps);
        // SetOtherRoom(initalRoomPos, finalRoomPos, Chunk.TypeChunk.Store);
        GameManager.Instance.openCloseScene.AdjustLoading(10);
        #endregion
        yield return null;
        #region GetRoom
        foreach (var potitionChunk in positionsChunks)
        {
            potitionChunk.Value.room = potitionChunk.Value.managementChunk.GetRandomRoom();
        }
        GameManager.Instance.openCloseScene.AdjustLoading(20);
        yield return null;
        #endregion
        #region InstanceUniqRoom
        Dictionary<string, RoomDrawer> instanciedRooms = new Dictionary<string, RoomDrawer>();
        for (int i = 0; i < positionsChunks.Count; i++)
        {
            if (!instanciedRooms.ContainsKey(positionsChunks.ElementAt(i).Value.room.name))
            {
                RoomDrawer room = Instantiate(positionsChunks.ElementAt(i).Value.room).GetComponent<RoomDrawer>();
                room.size = chunkSize;
                instanciedRooms.Add(positionsChunks.ElementAt(i).Value.room.name, room);
            }
        }
        GameManager.Instance.openCloseScene.AdjustLoading(30);
        yield return new WaitForSeconds(0.5f);
        #endregion
        #region DrawBaseRoom
        foreach (var room in instanciedRooms.Values)
        {
            room.gameObject.SetActive(true);
            room.DrawMap();
            yield return null;
            room.gameObject.SetActive(false);
        }
        GameManager.Instance.openCloseScene.AdjustLoading(40);
        yield return null;
        #endregion
        #region DuplicateRooms
        foreach (var positionChunks in positionsChunks)
        {
            if (instanciedRooms.TryGetValue(positionChunks.Value.room.name, out RoomDrawer roomDrawer))
            {
                positionChunks.Value.managementChunk.drawerMap = Instantiate
                (
                    roomDrawer.gameObject,
                    transform.position,
                    Quaternion.identity,
                    positionChunks.Value.managementChunk.transform
                ).GetComponent<RoomDrawer>();
                positionChunks.Value.managementChunk.drawerMap.gameObject.transform.localPosition = Vector3.zero;
            }
        }

        #region SetPortals
        positionsChunks[positionRoomKey].managementChunk.drawerMap.portal.otherPortal = positionsChunks[positionRoomKeyOut].managementChunk.drawerMap.portal;
        positionsChunks[positionRoomKeyOut].managementChunk.drawerMap.portal.otherPortal = positionsChunks[positionRoomKey].managementChunk.drawerMap.portal;
        #endregion

        foreach (var positionChunks in positionsChunks)
        {
            positionChunks.Value.managementChunk.drawerMap.chunkPos = positionChunks.Value.managementChunk.chunkId;
        }
        
        GameManager.Instance.openCloseScene.AdjustLoading(50);
        yield return new WaitForSeconds(0.5f);

        foreach (var room in instanciedRooms.Values)
        {
            Destroy(room.gameObject);
            yield return null;
        }
        instanciedRooms.Clear();

        yield return new WaitForSeconds(0.5f);
        #endregion
        #region ActiveRooms
        foreach (var positionChunk in positionsChunks)
        {
            positionChunk.Value.managementChunk.drawerMap.gameObject.SetActive(true);
        }
        GameManager.Instance.openCloseScene.AdjustLoading(60);
        yield return null;
        #endregion
        #region ActiveBridges
        ActiveBridges();
        GameManager.Instance.openCloseScene.AdjustLoading(70);
        yield return new WaitForSeconds(0.5f);
        #endregion
        #region DrawRoom
        foreach (var positionChunk in positionsChunks)
        {
            positionChunk.Value.managementChunk.drawerMap.DrawMap();
        }
        GameManager.Instance.openCloseScene.AdjustLoading(80);
        yield return new WaitForSeconds(0.5f);
        #endregion
        foreach (var chunk in positionsChunks)
        {
            chunk.Value.managementChunk.CombineBlocks();
        }
        GameManager.Instance.openCloseScene.AdjustLoading(90);
        yield return null;
        characters = GameObject.FindGameObjectsWithTag("Player");
        Vector3 spawnPosition = positionsChunks[initalRoomPos].chunkInstance.transform.position;
        foreach (var character in characters)
        {
            character.transform.position = new Vector3(spawnPosition.x, 1, spawnPosition.z);
            character.TryGetComponent<Character>(out Character component);
            component.characterHud.characterUi.mapUi.sizeMap = new Vector2(chunksX, chunksZ);
            component.characterHud.CreateMap();
            component.characterHud.characterUi.mapUi.currentRoom = new Vector2(initalRoomPos.x, initalRoomPos.z);
            component.characterHud.UpdateMap(component.characterHud.characterUi.mapUi.currentRoom);
        }
        GameManager.Instance.openCloseScene.AdjustLoading(100);
    }
    public void SetOtherRoom(Vector3Int initialRoomPos, Vector3Int finalRoomPos, Chunk.TypeChunk typeChunk)
    {
        while (true)
        {
            Vector3Int selected = new Vector3Int(Random.Range(0, chunksX), 0, Random.Range(0, chunksZ));
            if (positionsChunks.ContainsKey(selected) &&
                positionsChunks[selected].managementChunk.typeChunk != Chunk.TypeChunk.Initial &&
                positionsChunks[selected].managementChunk.typeChunk != Chunk.TypeChunk.Final &&
                positionsChunks[selected].managementChunk.typeChunk != Chunk.TypeChunk.RoomKey &&
                positionsChunks[selected].managementChunk.typeChunk != Chunk.TypeChunk.Special &&
                positionsChunks[selected].managementChunk.typeChunk != Chunk.TypeChunk.Traps &&
                positionsChunks[selected].managementChunk.typeChunk != Chunk.TypeChunk.Store)
            {
                positionsChunks[selected].managementChunk.typeChunk = typeChunk;
                break;
            }
        }
    }
    public void InstanceOtherRooms(Chunk.TypeChunk typeChunk, out Vector3Int position)
    {
        while (true)
        {
            Vector3Int selected = new Vector3Int(Random.Range(0, chunksX), 0, Random.Range(0, chunksZ));
            if (positionsChunks.ContainsKey(selected) &&
                positionsChunks[selected].managementChunk.typeChunk != Chunk.TypeChunk.Initial &&
                positionsChunks[selected].managementChunk.typeChunk != Chunk.TypeChunk.Final &&
                positionsChunks[selected].managementChunk.typeChunk != Chunk.TypeChunk.RoomKey &&
                positionsChunks[selected].managementChunk.typeChunk != Chunk.TypeChunk.Special &&
                positionsChunks[selected].managementChunk.typeChunk != Chunk.TypeChunk.Traps &&
                positionsChunks[selected].managementChunk.typeChunk != Chunk.TypeChunk.Store)
            {
                position = selected;
                positionsChunks[selected].managementChunk.typeChunk = typeChunk;
                break;
            }
        }
    }
    public void ActiveBridges()
    {
        for (int i = 0; i < positionsChunks.Count; i++)
        {
            if (ValidateBridge(positionsChunks.ElementAt(i).Key + Vector3Int.forward))
            {
                ActiveBridges(positionsChunks.ElementAt(i).Value.managementChunk, RoomDrawer.DirectionBridges.Forward);
            }
            if (ValidateBridge(positionsChunks.ElementAt(i).Key + Vector3Int.back))
            {
                ActiveBridges(positionsChunks.ElementAt(i).Value.managementChunk, RoomDrawer.DirectionBridges.Back);
            }
            if (ValidateBridge(positionsChunks.ElementAt(i).Key + Vector3Int.left))
            {
                ActiveBridges(positionsChunks.ElementAt(i).Value.managementChunk, RoomDrawer.DirectionBridges.Left);
            }
            if (ValidateBridge(positionsChunks.ElementAt(i).Key + Vector3Int.right))
            {
                ActiveBridges(positionsChunks.ElementAt(i).Value.managementChunk, RoomDrawer.DirectionBridges.Rigth);
            }
        }
    }
    public void ActiveBridges(Chunk chunk, RoomDrawer.DirectionBridges directionBridge)
    {
        chunk.drawerMap.ActiveBridges(directionBridge);
    }
    public bool ValidateBridge(Vector3Int pos)
    {
        bool contains = false;
        foreach (var chunk in positionsChunks)
        {
            if (chunk.Key == pos)
            {
                contains = true;
                break;
            }
        }
        return contains;
    }

    public void InstanceChunk(Vector2 offset, Vector3Int position)
    {
        Vector3 chunkPosition = new Vector3(position.x * chunkSize - offset.x, position.y * chunkSize, position.z * chunkSize - offset.y);

        GameObject chunkGO = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity);
        chunkGO.name = $"Chunk_{position.x}_{position.z}";
        chunkGO.transform.parent = transform;

        Chunk chunkComponent = chunkGO.GetComponent<Chunk>();

        BoxCollider collider = chunkGO.GetComponent<BoxCollider>();

        chunkComponent.chunkSize = chunkSize;
        collider.size = chunkSize * Vector3.one;

        positionsChunks[position].chunkInstance = chunkGO;
        positionsChunks[position].managementChunk = chunkComponent;
        chunkComponent.chunkId = new Vector2(position.x, position.z);
    }
    SerializedDictionary<Vector3Int, PositionChunk> GenerateMap(int ancho, int alto, out Vector3Int initalRoomPos, out Vector3Int finalRoomPos)
    {
        Vector3Int initPos = new Vector3Int(Random.Range(0, ancho), 0, 0);
        initalRoomPos = initPos;
        Vector3Int finalPos = new Vector3Int(Random.Range(0, ancho), 0, alto - 1);
        while (initPos == finalPos)
        {
            finalRoomPos = new Vector3Int(Random.Range(0, ancho), 0, alto - 1);
        }
        finalRoomPos = finalPos;

        SerializedDictionary<Vector3Int, PositionChunk> camino = new SerializedDictionary<Vector3Int, PositionChunk>
        {
            { initPos, new PositionChunk() }
        };

        HashSet<Vector3Int> visitados = new HashSet<Vector3Int> { initPos };
        Vector3Int actual = initPos;

        while (actual != finalPos)
        {
            List<Vector3Int> movimientos = new List<Vector3Int>
        {
            new Vector3Int(actual.x - 1, 0, actual.z),
            new Vector3Int(actual.x + 1, 0, actual.z),
            new Vector3Int(actual.x, 0, actual.z - 1),
            new Vector3Int(actual.x, 0, actual.z + 1)
        };

            movimientos = movimientos
                .Where(mov => mov.x >= 0 && mov.x < ancho && mov.z >= 0 && mov.z < alto && !visitados.Contains(mov))
                .OrderBy(_ => Random.value)
                .ToList();

            if (movimientos.Count == 0)
            {
                break;
            }

            movimientos = movimientos.OrderBy(mov => Vector3Int.Distance(mov, finalPos) + Random.Range(-0.5f, 0.5f)).ToList();
            actual = movimientos[0];

            visitados.Add(actual);
            camino.Add(actual, new PositionChunk());
        }

        int cantidadRamas = Random.Range(3, 6);
        for (int i = 0; i < cantidadRamas; i++)
        {
            int indiceCamino = Random.Range(1, camino.Count - 1);
            Vector3Int puntoRama = camino.ElementAt(indiceCamino).Key;
            int longitudRama = Random.Range(2, 5);

            Vector3Int ramaActual = puntoRama;
            for (int j = 0; j < longitudRama; j++)
            {
                List<Vector3Int> movimientosRama = new List<Vector3Int>
            {
                new Vector3Int(ramaActual.x - 1, 0, ramaActual.z),
                new Vector3Int(ramaActual.x + 1, 0, ramaActual.z),
                new Vector3Int(ramaActual.x, 0, ramaActual.z - 1),
                new Vector3Int(ramaActual.x, 0, ramaActual.z + 1)
            };

                movimientosRama = movimientosRama
                    .Where(mov => mov.x >= 0 && mov.x < ancho && mov.z >= 0 && mov.z < alto && !visitados.Contains(mov))
                    .OrderBy(_ => Random.value)
                    .ToList();

                if (movimientosRama.Count == 0) break;

                ramaActual = movimientosRama[0];
                visitados.Add(ramaActual);
                camino.Add(ramaActual, new PositionChunk());
            }
        }
        return camino;
    }

    [System.Serializable] public class DirectionChunk
    {
        public int pos = 0;
        public ValidateDirectionChunk validateDirectionChunk;
    }
    [System.Serializable] public class PositionChunk
    {
        public Chunk managementChunk;
        public GameObject room;
        public GameObject chunkInstance;
    }
    public enum ValidateDirectionChunk
    {
        Forward = 0,
        Back = 1,
        Left = 2,
        Rigth = 3
    }
}