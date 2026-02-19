using System.Collections.Generic;
using UnityEngine;

public class DynamicGrid : MonoBehaviour
{
    public GameObject prefab;
    public Transform player;
    
    [Header("Number of cells")]
    public int viewDistanceForward = 5; 
    public int viewDistanceBackward = 5; 
    public int viewDistanceSideways = 5;
    public bool limitXDistance = true;
    public int limitDisance = 5;


    private float cellSize;
    private Dictionary<Vector2Int, GameObject> gridObjects = new Dictionary<Vector2Int, GameObject>();

    int _xOrgin;

    void Start()
    {
        // Calculate size based on the prefab's bounds
        cellSize = prefab.GetComponent<Renderer>().bounds.size.x;
        UpdateGrid();

        _xOrgin = Mathf.FloorToInt(player.position.x);

        if (limitXDistance) viewDistanceSideways = limitDisance;
    }

    void Update()
    {
        UpdateGrid();
    }

    void UpdateGrid()
    {
        // Get player's current grid cell
        int pX = limitXDistance ? _xOrgin : Mathf.FloorToInt(player.position.x / cellSize);
        int pZ = Mathf.FloorToInt(player.position.z / cellSize);


        for (int x = -viewDistanceBackward; x <= viewDistanceForward; x++)
        {
            for (int z = -viewDistanceSideways; z <= viewDistanceSideways; z++)
            {
                Vector2Int cellCoord = new Vector2Int(pX + x, pZ + z);

                if (!gridObjects.ContainsKey(cellCoord))
                {
                    Vector3 spawnPos = new Vector3(cellCoord.x * cellSize, 0, cellCoord.y * cellSize);
                    GameObject newObj = Instantiate(prefab, spawnPos, Quaternion.identity);
                    gridObjects.Add(cellCoord, newObj);
                }
            }
        }
    }
}