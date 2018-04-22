using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GameController : MonoBehaviour {

    public static float Z_OffsetLevel3D = 10;
    public static GameController GameCtrl { get; set; }
    public static PlayerController PlayerCtrl { get; set; }
    public static WaveSpawner Spawner { get; set; }
    public static GameObject Level3D { get; set; }
    public static Transform Player3DTransform { get; set; }
    public static Camera MainCamera { get; set; }

    public NavMeshSurface WalkingSurface;

    public NavMeshSurface FireProofSurface;
    public NavMeshSurface FlyingSurface;

    void Start()
    {
        GameCtrl = this;
        PlayerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Spawner = GetComponent<WaveSpawner>();
        Level3D = GameObject.FindGameObjectWithTag("Level3D");
        Player3DTransform = GameObject.FindGameObjectWithTag("Player3DPosition").transform;
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        StartCoroutine(SetupMap());
    }

    private IEnumerator SetupMap()
    {
        yield return new WaitForFixedUpdate();
        Level3D.GetComponent<MapGenerator>().BakeNavMeshes();

        yield return Spawner.Setup();

        yield return new WaitForFixedUpdate();
        Player3DTransform.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
    }

    public void RebuildNavMeshes()
    {
        StartCoroutine(Co_RebuildNavMeshes());
    }

    private IEnumerator Co_RebuildNavMeshes()
    {
        yield return new WaitForFixedUpdate();

        //TODO this may get expensive with a lot of enemies, if it is noticeable then make a 
        //static bool on GameController that Mirror checks every update
        //if it's true, then don't update the enemy2D that frame
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var item in enemies)
        {
            item.GetComponent<Mirror>().DontMirror();
        }

        FireProofSurface.RemoveData();
        yield return new WaitForFixedUpdate();
        FireProofSurface.BuildNavMesh();

        WalkingSurface.RemoveData();
        yield return new WaitForFixedUpdate();
        WalkingSurface.BuildNavMesh();

        foreach (var item in enemies)
        {
            item.GetComponent<Mirror>().Mirror3DObject();
        }
    }

    public static void Win()
    {
        Debug.LogError("Do something to show that you won!");
    }
}
