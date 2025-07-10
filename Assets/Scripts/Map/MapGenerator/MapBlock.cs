using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class MapBlock : MonoBehaviour
{
    public bool isWalkable = false;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;
    public MeshFilter meshFilter;
    public BlockInfo blockInfo;
    private Vector3 boxSize = new Vector3(0.25f, 0.25f, 0.25f);
    private Vector3 offsetPos = new Vector3(0, 0.5f, 0);
    [SerializeField] List<TypeDirections> directionsBlocks = new List<TypeDirections>();
    public bool detectUp = false;
    public bool detectDown = false;
    public bool detectLeft = false;
    public bool detectRight = false;
    public bool detectForward = false;
    public bool detectBack = false;
    public bool detectUpLeft = false;
    public bool detectUpRight = false;
    public bool detectDownLeft = false;
    public bool detectDownRight = false;
    public bool detectUpForward = false;
    public bool detectUpBack = false;
    public bool detectDownForward = false;
    public bool detectDownBack = false;
    public bool detectForwardLeft = false;
    public bool detectForwardRigth = false;
    public bool detectBackLeft = false;
    public bool detectBackRigth = false;
    private Vector3 up = new Vector3(0, 1, 0);
    private Vector3 down = new Vector3(0, -1, 0);
    private Vector3 left = new Vector3( -1, 0, 0);
    private Vector3 right = new Vector3(1, 0, 0);
    private Vector3 forward = new Vector3( 0, 0, 1);
    private Vector3 back = new Vector3( 0, 0, -1);
    public bool drawGizmos;
    public int blockSelectedIndex;
    public NewRuleInfo newRuleInfo;
    [NaughtyAttributes.Button]
    public void DrawBlock()
    {
        DetectInSpecifiedDirections();
    }
    private int GetBitmask()
    {
        int mask = 0;
        foreach (var dir in directionsBlocks)
        {
            mask |= 1 << (int)dir;
        }
        return mask;
    }
    public void DetectInSpecifiedDirections()
    {
        isWalkable = false;
        DetectsDirections();
        SetTextureFromAtlas(GetTextureByDirection(out Sprite spriteSelected), spriteSelected, out Vector2[] uvsForRule, out Sprite spriteForRule);
        newRuleInfo = new NewRuleInfo
        {
            bitMask = GetBitmask(),
            spriteRule = spriteForRule,
            uvsRule = uvsForRule
        };
    }
    [NaughtyAttributes.Button]
    void DetectsDirections()
    {
        directionsBlocks.Clear();
        if (detectUp) CheckDirection(up, TypeDirections.Up);
        if (!directionsBlocks.Contains(TypeDirections.Up))
        {
            if (detectDown) CheckDirection(down, TypeDirections.Down);
            if (detectLeft) CheckDirection(left, TypeDirections.Left);
            if (detectRight) CheckDirection(right, TypeDirections.Rigth);
            if (detectForward) CheckDirection(forward, TypeDirections.Forward);
            if (detectBack) CheckDirection(back, TypeDirections.Back);
            if (detectUpLeft) CheckDirection(up + left, TypeDirections.UpLeft);
            if (detectUpRight) CheckDirection(up + Vector3.right, TypeDirections.UpRigth);
            if (detectDownLeft) CheckDirection(down + left, TypeDirections.DownLeft);
            if (detectDownRight) CheckDirection(down + right, TypeDirections.DownRigth);
            if (detectUpForward) CheckDirection(up + forward, TypeDirections.UpForward);
            if (detectUpBack) CheckDirection(up + back, TypeDirections.UpBack);
            if (detectDownForward) CheckDirection(down + forward, TypeDirections.DownForward);
            if (detectDownBack) CheckDirection(down + back, TypeDirections.DownBack);
            if (detectForwardLeft) CheckDirection(forward + left, TypeDirections.ForwardLeft);
            if (detectForwardRigth) CheckDirection(forward + right, TypeDirections.ForwardRight);
            if (detectBackLeft) CheckDirection(back + left, TypeDirections.BackLeft);
            if (detectBackRigth) CheckDirection(back + right, TypeDirections.BackRight);
        }
    }
    [NaughtyAttributes.Button]
    void AddBitMaskRule()
    {
        blockInfo.tileRules.Add(GetBitmask(), new TexturesInfo(null, null));
        directionsBlocks.Clear();
        newRuleInfo = new NewRuleInfo();
    }
    [NaughtyAttributes.Button]
    void AddRuleByInfo()
    {
        if (blockInfo.tileRules.TryGetValue(GetBitmask(), out TexturesInfo texturesInfo))
        {
            blockInfo.tileRules[GetBitmask()].uvsPos = newRuleInfo.uvsRule;
        }
        directionsBlocks.Clear();
        newRuleInfo = new NewRuleInfo();
    }
    void CheckDirection(Vector3 direction, TypeDirections directionBlock)
    {
        if (DetectManagementMapBlock(GetAdjustedDirection(direction), out GameObject detectedObject))
        {
            if (detectedObject.GetComponent<MapBlock>().blockInfo.typeBlock == blockInfo.typeBlock)
            {
                directionsBlocks.Add(directionBlock);
            }
        }
    }
    Vector3 GetAdjustedDirection(Vector3 direction)
    {
        return transform.rotation * direction;
    }
    public Sprite GetTextureByDirection(out Sprite spriteSelected)
    {        
        if (blockInfo.tileRules.TryGetValue(GetBitmask(), out TexturesInfo textureFinded)){
            if (blockInfo.variationTextures.TryGetValue(textureFinded.tileTexture, out Sprite[] sprites))
            {
                spriteSelected = textureFinded.tileTexture;
                return sprites[Random.Range(0, sprites.Length - 1)];
            }
            else
            {
                spriteSelected = textureFinded.tileTexture;
                return textureFinded.tileTexture;
            }
        }
        spriteSelected = blockInfo.tileRules.ElementAt(0).Value.tileTexture;
        return blockInfo.tileRules.ElementAt(0).Value.tileTexture;
    }
    public Mesh GetMeshByTexture(Sprite mainTexture)
    {
        if (blockInfo.tileMeshes.TryGetValue(mainTexture, out TileMeshes tileMesh))
        {
            Mesh copia = new Mesh();
            copia.vertices = tileMesh.mesh.vertices;
            copia.triangles = tileMesh.mesh.triangles;
            copia.uv = tileMesh.mesh.uv;
            copia.normals = tileMesh.mesh.normals;
            copia.colors = tileMesh.mesh.colors;
            copia.tangents = tileMesh.mesh.tangents;
            copia.bounds = tileMesh.mesh.bounds;
            copia.boneWeights = tileMesh.mesh.boneWeights;
            copia.bindposes = tileMesh.mesh.bindposes;
            return copia;
        }
        return null;
    }
    public void SetTextureFromAtlas(Sprite variationSprite, Sprite spriteSelected, out Vector2[] uvsForRule, out Sprite ruleSprite)
    {
        Mesh newMesh = GetMeshByTexture(spriteSelected);
        if (newMesh != null)
        {
            meshFilter.mesh = newMesh;
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = newMesh;
            }
        }
        Vector2[] uvs = newMesh.uv;
        Texture2D texture = variationSprite.texture;
        Rect spriteRect = variationSprite.textureRect;
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i].x = Mathf.Lerp(spriteRect.x / texture.width, (spriteRect.x + spriteRect.width) / texture.width, uvs[i].x);
            uvs[i].y = Mathf.Lerp(spriteRect.y / texture.height, (spriteRect.y + spriteRect.height) / texture.height, uvs[i].y);
        }
        newMesh.uv = uvs;
        uvsForRule = uvs;
        ruleSprite = spriteSelected;
    }

    public bool EqualsDirections(TypeDirections[] directions1, TypeDirections[] directions2)
    {
        return new HashSet<TypeDirections>(directions1).SetEquals(directions2);
    }
    public bool DetectManagementMapBlock(Vector3 direction, out GameObject detectedObject)
    {
        detectedObject = null;
        Vector3 targetPosition = transform.position + offsetPos + direction;
        Collider[] hitColliders = Physics.OverlapBox(targetPosition, boxSize, Quaternion.identity);

        foreach (Collider collider in hitColliders)
        {
            if (collider.GetComponent<MapBlock>() != null)
            {
                detectedObject = collider.gameObject;
                return true;
            }
        }
        return false;
    }
    // private void OnDrawGizmos()
    // {
    //     if (drawGizmos)
    //     {
    //         Gizmos.color = Color.green;
    //         Vector3 center = transform.position;

    //         if (detectUp) DrawGizmo(center, up);
    //         if (detectDown) DrawGizmo(center, down);
    //         if (detectLeft) DrawGizmo(center, left);
    //         if (detectRight) DrawGizmo(center, right);
    //         if (detectForward) DrawGizmo(center, forward);
    //         if (detectBack) DrawGizmo(center, back);
    //         if (detectUpLeft) DrawGizmo(center, up + left);
    //         if (detectUpRight) DrawGizmo(center, up + right);
    //         if (detectDownLeft) DrawGizmo(center, down + left);
    //         if (detectDownRight) DrawGizmo(center, down + right);
    //         if (detectUpForward) DrawGizmo(center, up + forward);
    //         if (detectUpBack) DrawGizmo(center, up + back);
    //         if (detectDownForward) DrawGizmo(center, down + forward);
    //         if (detectDownBack) DrawGizmo(center, down + back);
    //         if (detectForwardLeft) DrawGizmo(center, forward + left);
    //         if (detectForwardRigth) DrawGizmo(center, forward + right);
    //         if (detectBackLeft) DrawGizmo(center, back + left);
    //         if (detectBackRigth) DrawGizmo(center, back + right);
    //     }
    // }
    private void DrawGizmo(Vector3 center, Vector3 direction)
    {
        Vector3 destination = center + offsetPos + direction;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(destination, boxSize);
    }
    [System.Serializable]
    public class BlockInfo
    {
        public Material atlasMaterial;
        public Sprite keyForVariations;
        public TypeBlock typeBlock;
        public SerializedDictionary<int, TexturesInfo> tileRules;
        public SerializedDictionary<Sprite, TileMeshes> tileMeshes;
        public SerializedDictionary<Sprite, Sprite[]> variationTextures;
    }
    [System.Serializable]   public class TexturesInfo
    {
        public Sprite tileTexture;
        public Vector2[] uvsPos;

        public TexturesInfo(Sprite tileTexture, Vector2[] uvsPos){
            this.tileTexture = tileTexture;
            this.uvsPos = uvsPos;
        }
    }
    [System.Serializable]   public class Variations
    {
        public float probability;
        public Sprite sprite;
        public bool needInstance;
        public GameObject instance;
        public bool needInstanceTexture;
        public Sprite instanceSpriteFromAtlas;
        public bool isWalkable = false;
    }
    [System.Serializable]   public class TileMeshes
    {        
        public Mesh mesh;

        public TileMeshes(Mesh mesh)
        {
            this.mesh = mesh;
        }
    }
    [System.Serializable] public class NewRuleInfo
    {
        public int bitMask;
        public Sprite spriteRule;
        public Vector2[] uvsRule;
    }
    public enum TypeDirections
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Rigth = 4,
        Forward = 5,
        Back = 6,
        UpLeft = 7,
        UpRigth = 8,
        DownLeft = 9,
        DownRigth = 10,
        UpForward = 11,
        UpBack = 12,
        DownForward = 13,
        DownBack = 14,
        ForwardLeft = 15,
        ForwardRight = 16,
        BackLeft = 17,
        BackRight = 18,
    }
    public enum TypeBlock
    {
        Void = 0,
        Block = 1,
        Stairs = 2
    }
}
