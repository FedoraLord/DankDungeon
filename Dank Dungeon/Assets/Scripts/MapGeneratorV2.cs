using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class MapGeneratorV2 : MonoBehaviour {

    [Header("Tilemaps")]
    public GridLayout tileMapGrid;
    public Tilemap staticObstacles;
    public Tilemap dynamicObstacles;
    public Tilemap floorTiles;

    [Header("Parents")]
    public Transform staticParent;
    public Transform dynamicParent;
    public Transform floorParent;

    [Header("Prefabs")]
    public GameObject staticPrefab;

    void Start () {
        CompressTileMaps();
        ResizeFloor();
        
        GenerateSomeShit(staticObstacles, staticPrefab, staticParent, (x => x != null));
    }

    public void BakeNavMeshes()
    {
        NavMeshSurface[] surfaces = floorParent.GetComponents<NavMeshSurface>();
        foreach (var surface in surfaces)
        {
            surface.BuildNavMesh();
        }
    }
    
    public IEnumerator UpdateNavMeshes()
    {
        NavMeshSurface[] surfaces = floorParent.GetComponents<NavMeshSurface>();
        foreach (var surface in surfaces)
        {
            yield return surface.UpdateNavMesh();
        }
    }

    private void CompressTileMaps()
    {
        floorTiles.CompressBounds();
        staticObstacles.CompressBounds();
    }

    private void ResizeFloor()
    {
        BoundsInt bounds = floorTiles.cellBounds;
        floorParent.localPosition = new Vector3((float)bounds.size.x / 2 + bounds.position.x, (float)bounds.size.y / 2 + bounds.position.y, 0);
        floorParent.localScale = new Vector3(bounds.size.x, 1, bounds.size.y);
    }

    private void GenerateSomeShit(Tilemap mapLayer, GameObject prefab3D, Transform parent, Func<TileBase, bool> tileCheck)
    {
        BoundsInt bounds = mapLayer.cellBounds;
        TileBase[] allTiles = mapLayer.GetTilesBlock(bounds);
        int continuousTiles = 0;

        for (int y = 0; y < bounds.size.y; y++)
        {
            for (int x = 0; x < bounds.size.x; x++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                bool isTileAffected = tileCheck(tile);

                if (isTileAffected)
                {
                    continuousTiles++;
                }

                int trueX = x;
                if (x == bounds.size.x - 1)
                    trueX++;

                //if end of continuous tile set OR last x-index of the array
                if (continuousTiles > 0 && (!isTileAffected || trueX == x + 1))
                {
                    Vector3Int mapPosition = new Vector3Int(trueX - continuousTiles + bounds.x, y + bounds.y, 0);
                    Vector3 position = tileMapGrid.CellToWorld(mapPosition);
                    Vector3 scale = new Vector3(continuousTiles, 1, 1);
                    position += new Vector3((float)continuousTiles / 2, 0.5f, -1);

                    GenerateNavObject(prefab3D, parent, position, scale);

                    continuousTiles = 0;
                }
            }
        }
    }
    
    public void GenerateNavObject(GameObject prefab, Transform parent, Vector3 localPosition, Vector3 localScale)
    {
        GameObject obj = Instantiate(prefab, parent);
        obj.transform.localPosition = localPosition;
        obj.transform.localScale = localScale;
    }
}
