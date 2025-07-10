using UnityEngine;

public class GenerateMapMenu : MonoBehaviour
{
    public MapBlock[] blocks;
    void Start()
    {
        foreach(var block in blocks)
        {
            block.DrawBlock();
        }
    }
}
