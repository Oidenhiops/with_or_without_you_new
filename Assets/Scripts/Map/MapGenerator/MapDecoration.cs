using UnityEngine;

public class MapDecoration : MonoBehaviour
{
    public Sprite[] spriteKeys;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public Mesh originalDecorationMesh;
    public MeshCollider meshCollider;
    [NaughtyAttributes.Button]
    public void DrawBlock()
    {
        if (spriteKeys.Length > 0) SetTextureFromAtlas();
    }
    void SetTextureFromAtlas()
    {
        Mesh newMesh = GetMeshByTexture();
        if (newMesh != null)
        {
            meshFilter.mesh = newMesh;
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = newMesh;
            }
        }
        Vector2[] uvs = newMesh.uv;
        Texture2D texture = spriteKeys[Random.Range(0, spriteKeys.Length)].texture;
        Rect spriteRect = spriteKeys[Random.Range(0, spriteKeys.Length)].textureRect;
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i].x = Mathf.Lerp(spriteRect.x / texture.width, (spriteRect.x + spriteRect.width) / texture.width, uvs[i].x);
            uvs[i].y = Mathf.Lerp(spriteRect.y / texture.height, (spriteRect.y + spriteRect.height) / texture.height, uvs[i].y);
        }
        newMesh.uv = uvs;
    }
    public Mesh GetMeshByTexture()
    {
        Mesh copia = new Mesh();
        copia.vertices = originalDecorationMesh.vertices;
        copia.triangles = originalDecorationMesh.triangles;
        copia.uv = originalDecorationMesh.uv;
        copia.normals = originalDecorationMesh.normals;
        copia.colors = originalDecorationMesh.colors;
        copia.tangents = originalDecorationMesh.tangents;
        copia.bounds = originalDecorationMesh.bounds;
        copia.boneWeights = originalDecorationMesh.boneWeights;
        copia.bindposes = originalDecorationMesh.bindposes;
        return copia;
    }
}
