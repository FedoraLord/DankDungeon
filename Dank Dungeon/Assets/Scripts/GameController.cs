﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GameController : MonoBehaviour {

    public static float Z_OffsetLevel3D = 10;
    public static GameController GameCtrl { get; set; }
    public static WaveSpawner Spawner { get; set; }
    public static GameObject Level3D { get; set; }
    public static Transform Player3DTransform { get; set; }

    public NavMeshSurface WalkingSurface;
    public NavMeshSurface FireProofSurface;
    public NavMeshSurface FlyingSurface;

    void Start()
    {
        GameCtrl = this;
        Spawner = GetComponent<WaveSpawner>();
        Level3D = GameObject.FindGameObjectWithTag("Level3D");
        Player3DTransform = GameObject.FindGameObjectWithTag("Player3DPosition").transform;
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
            item.GetComponent<Mirror>().independentMover = Mirror.MirrorMode.DontMirror;
        }

        FireProofSurface.RemoveData();
        yield return new WaitForFixedUpdate();
        FireProofSurface.BuildNavMesh();

        WalkingSurface.RemoveData();
        yield return new WaitForFixedUpdate();
        WalkingSurface.BuildNavMesh();

        foreach (var item in enemies)
        {
            item.GetComponent<Mirror>().independentMover = Mirror.MirrorMode.Object3D;
        }
    }
}
