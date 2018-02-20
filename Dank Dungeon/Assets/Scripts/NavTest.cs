using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavTest : MonoBehaviour {

    public float agentClimb;
    public float agentHeight;
    public float agentRadius;
    public float agentSlope;
    public int agentTypeID;
    public float minRegionArea;
    public bool overrideTileSize;
    public bool overrideVoxelSize;
    public int tileSize;
    public float voxelSize;

    // Use this for initialization
    void Start () {
        //NavMeshBuildSettings buildSettings = new NavMeshBuildSettings();
        //{
        //    agentClimb = agentClimb,
        //    agentHeight = agentHeight,
        //    agentRadius = agentRadius,
        //    agentSlope = agentSlope, 
        //    agentTypeID = agentTypeID,
        //    minRegionArea = minRegionArea,
        //    overrideTileSize = overrideTileSize,
        //    overrideVoxelSize = overrideVoxelSize,
        //    tileSize = tileSize,
        //    voxelSize = voxelSize
        //};

        //NavMeshBuildSource asdf = new NavMeshBuildSource();
        //{
        //    area = 0,
        //    component = 0,
        //    shape = 0,
        //    size = 0,
        //    sourceObject = 0,
        //    transform = 0
        //}
        //var aaaa = NavMeshBuilder.BuildNavMeshData(buildSettings, new List<NavMeshBuildSource>() { asdf }, new Bounds(), new Vector3(), Quaternion.identity);


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
