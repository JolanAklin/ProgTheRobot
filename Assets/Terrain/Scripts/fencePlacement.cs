using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fencePlacement : MonoBehaviour
{
    public Vector2Int pos;
    public int rot; // 90 multiplied by this number

    public GameObject fence;
    public TerrainManager.FencePostion fencePos;
}
