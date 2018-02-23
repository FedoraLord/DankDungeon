using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour {

    public GridLayout tileMapGrid;

    public Tilemap staticObstacles;
    public Tilemap dynamicObstacles;
    public Tilemap floorTiles;

    public Transform parentStatic;
    public Transform parentDynamic;
    public Transform navFloor;

    public GameObject prefabStatic;

    void Start () {
        CompressTileMaps();
        ResizeFloor();
        GenerateStaticObstacles();
        BakeNavMesh();
    }

    private void BakeNavMesh()
    {
        NavMeshSurface surface = navFloor.GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }

    private void CompressTileMaps()
    {
        floorTiles.CompressBounds();
        staticObstacles.CompressBounds();
    }

    private void ResizeFloor()
    {
        BoundsInt bounds = floorTiles.cellBounds;
        navFloor.localPosition = new Vector3((float)bounds.size.x / 2 + bounds.position.x, (float)bounds.size.y / 2 + bounds.position.y, 0);
        navFloor.localScale = new Vector3(bounds.size.x, 1, bounds.size.y);
    }

    private void GenerateStaticObstacles()
    {
        BoundsInt bounds = staticObstacles.cellBounds;
        TileBase[] allTiles = staticObstacles.GetTilesBlock(bounds);
        int continuousTiles = 0;

        for (int y = 0; y < bounds.size.y; y++)
        {
            for (int x = 0; x < bounds.size.x; x++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                
                if (tile != null)
                {
                    continuousTiles++;
                }
                else if (continuousTiles > 0)
                {
                    Vector3Int mapPosition = new Vector3Int(x - continuousTiles + bounds.x, y + bounds.y, 0);
                    CreateStaticObstacle(mapPosition, continuousTiles);
                    continuousTiles = 0;
                }
            }

            if (continuousTiles > 0)
            {
                Vector3Int mapPosition = new Vector3Int(bounds.size.x + bounds.x - continuousTiles, y + bounds.y, 0);
                CreateStaticObstacle(mapPosition, continuousTiles);
                continuousTiles = 0;
            }
        }
    }

    private void CreateStaticObstacle(Vector3Int mapPosition, int numTiles)
    {
        Vector3 scale = new Vector3(numTiles, 1, 1);
        Vector3 position = tileMapGrid.CellToWorld(mapPosition);
        position += new Vector3((float)numTiles / 2, 0.5f, -1);
        GenerateNavObject(prefabStatic, parentStatic, position, scale);
    }
    
    public void GenerateNavObject(GameObject prefab, Transform parent, Vector3 localPosition, Vector3 localScale)
    {
        GameObject obj = Instantiate(prefab, parent);
        obj.transform.localPosition = localPosition;
        obj.transform.localScale = localScale;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
