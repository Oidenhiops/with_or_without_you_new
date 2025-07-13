using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunksGenerator : MonoBehaviour
{
    public int chunkSize = 17;
    public int chunksX = 5;
    public int chunksZ = 5;
    public List<PositionChunk> positionsChunks = new List<PositionChunk>();
    public GameObject[] characters;
    private void Start()
    {
        StartCoroutine(GenerateChunks());
    }
    public IEnumerator GenerateChunks()
    {
        #region CreateGrid
        positionsChunks = GenerateMap(chunksX, chunksZ, out Vector3Int initalRoomPos, out Vector3Int finalRoomPos);
        GameObject chunkPrefab = Resources.Load<GameObject>("Prefabs/Map/Chunk");
        float offsetX = chunksX * chunkSize / 2f - (chunkSize / 2f);
        float offsetZ = chunksZ * chunkSize / 2f - (chunkSize / 2f);

        for (int c = 0; c < positionsChunks.Count; c++)
        {
            Vector3Int chunkCoord = positionsChunks[c].positionChunk;
            Vector3 chunkPosition = new Vector3(chunkCoord.x * chunkSize - offsetX, 0, chunkCoord.z * chunkSize - offsetZ);

            GameObject chunkGO = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity);
            chunkGO.name = $"Chunk_{chunkCoord.x}_{chunkCoord.z}";
            chunkGO.transform.parent = transform;

            Chunk chunkComponent = chunkGO.GetComponent<Chunk>();
            BoxCollider collider = chunkGO.GetComponent<BoxCollider>();

            chunkComponent.chunkSize = chunkSize;
            collider.size = chunkSize * Vector3.one;

            positionsChunks[c].chunkInstance = chunkGO;
            positionsChunks[c].managementChunk = chunkComponent;
        }
        GameManager.Instance.openCloseScene.AdjustLoading(10);
        yield return null;
        #endregion
        #region GetRoom
        for (int i = 0; i < positionsChunks.Count; i++)
        {
            PositionChunk chunkData = positionsChunks[i];
            bool isInitial = chunkData.positionChunk == initalRoomPos;
            bool isFinal = chunkData.positionChunk == finalRoomPos;
            chunkData.room = chunkData.managementChunk.GetRandomRoom(isInitial, isFinal);
        }
        GameManager.Instance.openCloseScene.AdjustLoading(20);
        yield return null;
        #endregion
        #region InstanceUniqRoom
        Dictionary<string, RoomDrawer> instanciedRooms = new Dictionary<string, RoomDrawer>();
        for (int i = 0; i < positionsChunks.Count; i++)
        {
            if (!instanciedRooms.ContainsKey(positionsChunks[i].room.name))
            {
                RoomDrawer room = Instantiate(positionsChunks[i].room).GetComponent<RoomDrawer>();
                instanciedRooms.Add(positionsChunks[i].room.name, room);

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
        foreach (PositionChunk positionChunks in positionsChunks)
        {
            if (instanciedRooms.TryGetValue(positionChunks.room.name, out RoomDrawer roomDrawer))
            {
                positionChunks.managementChunk.drawerMap = Instantiate
                (
                    roomDrawer.gameObject,
                    transform.position,
                    Quaternion.identity,
                    positionChunks.managementChunk.transform
                ).GetComponent<RoomDrawer>();
                positionChunks.managementChunk.drawerMap.gameObject.transform.localPosition = Vector3.zero;
            }
        }
        GameManager.Instance.openCloseScene.AdjustLoading(50);
        yield return new WaitForSeconds(0.5f);
        #endregion
        #region ActiveRooms
        foreach (PositionChunk positionChunk in positionsChunks)
        {
            positionChunk.managementChunk.drawerMap.gameObject.SetActive(true);
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
        foreach (PositionChunk positionChunk in positionsChunks)
        {
            positionChunk.managementChunk.drawerMap.DrawMap();
        }
        GameManager.Instance.openCloseScene.AdjustLoading(80);
        yield return new WaitForSeconds(0.5f);
        #endregion
        foreach (var chunk in positionsChunks)
        {
            chunk.managementChunk.CombineBlocks();
        }
        GameManager.Instance.openCloseScene.AdjustLoading(90);
        yield return null;
        characters = GameObject.FindGameObjectsWithTag("Player");
        Vector3 spawnPosition = FindSpawnPosition(initalRoomPos);
        foreach (var character in characters)
        {
            character.transform.position = new Vector3(spawnPosition.x, 1, spawnPosition.z);
        }
        GameManager.Instance.openCloseScene.AdjustLoading(100);
    }
    public Vector3 FindSpawnPosition(Vector3 initalRoomPos)
    {
        foreach (PositionChunk chunk in positionsChunks)
        {
            if (chunk.positionChunk == initalRoomPos)
            {
                return chunk.chunkInstance.transform.position;
            }
        }
        return Vector3.zero;
    }
    public void ActiveBridges()
    {
        for (int i = 0; i < positionsChunks.Count; i++)
        {
            if (ValidateBridge(positionsChunks[i].positionChunk + Vector3Int.forward))
            {
                ActiveBridges(positionsChunks[i].managementChunk, RoomDrawer.DirectionBridges.Forward);
            }
            if (ValidateBridge(positionsChunks[i].positionChunk + Vector3Int.back))
            {
                ActiveBridges(positionsChunks[i].managementChunk, RoomDrawer.DirectionBridges.Back);
            }
            if (ValidateBridge(positionsChunks[i].positionChunk + Vector3Int.left))
            {
                ActiveBridges(positionsChunks[i].managementChunk, RoomDrawer.DirectionBridges.Left);
            }
            if (ValidateBridge(positionsChunks[i].positionChunk + Vector3Int.right))
            {
                ActiveBridges(positionsChunks[i].managementChunk, RoomDrawer.DirectionBridges.Rigth);
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
            if (chunk.positionChunk == pos)
            {
                contains = true;
                break;
            }
        }
        return contains;
    }
    List<PositionChunk> GenerateMap(int ancho, int alto, out Vector3Int initalRoomPos, out Vector3Int finalRoomPos)
    {
        Vector3Int initPos = new Vector3Int(Random.Range(0, ancho), 0, 0);
        initalRoomPos = initPos;
        Vector3Int finalPos = new Vector3Int(Random.Range(0, ancho), 0, alto - 1);
        while (initPos == finalPos)
        {
            finalRoomPos = new Vector3Int(Random.Range(0, ancho), 0, alto - 1);
        }
        finalRoomPos = finalPos;

        List<PositionChunk> camino = new List<PositionChunk>
        {
            new PositionChunk { positionChunk = initPos }
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
            camino.Add(new PositionChunk { positionChunk = actual });
        }

        int cantidadRamas = Random.Range(3, 6);
        for (int i = 0; i < cantidadRamas; i++)
        {
            int indiceCamino = Random.Range(1, camino.Count - 1);
            Vector3Int puntoRama = camino[indiceCamino].positionChunk;
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
                camino.Add(new PositionChunk { positionChunk = ramaActual });
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
        public Vector3Int positionChunk = new Vector3Int();
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