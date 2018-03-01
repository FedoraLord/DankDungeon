using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class WaveSpawner : MonoBehaviour {

    public static List<string> alternateWaveCommands = new List<string>()
    {
        "Wait N Seconds",
        "Wait Until N Alive"
    };

    public int maxSpawnPoints;
    public Transform enemyParent;
    public List<Wave> waves;

    private int currentWave;
    private NavMeshAgent playerAgent;
    public List<Transform> currentSpawnPoints;
    public List<Transform> validSpawnPoints = new List<Transform>();
    public List<Transform> invalidSpawnPoints = new List<Transform>();
    public List<GameObject> test = new List<GameObject>();
    
	void Start () {
        playerAgent = GameObject.FindGameObjectWithTag("Player3DPosition").GetComponent<NavMeshAgent>();
        Invoke("Setup", 1);
    }

    private void Setup()
    {   
        InitializeValidRooms();
        UpdateSpawnPoints();
    }

    private void Update()
    {
        for (int i = 0; i < currentSpawnPoints.Count; i++)
        {
            test[i].transform.position = currentSpawnPoints[i].position;
        }
    }

    public void InitializeValidRooms()
    {
        GameObject[] allRooms = GameObject.FindGameObjectsWithTag("Room");
        for (int i = 0; i < allRooms.Length; i++)
        {
            if (allRooms[i].transform == PlayerController.lastRoom)
                continue;
            
            Vector3 destination = allRooms[i].transform.position;
            destination.z = GameController.Z_OffsetLevel3D;

            if (IsValidDestination(destination))
            {
                validSpawnPoints.Add(allRooms[i].transform);
            }
            else
            {
                invalidSpawnPoints.Add(allRooms[i].transform);
            }
        }
    }

    private bool IsValidDestination(Vector3 destination)
    {
        bool valid = playerAgent.SetDestination(destination);
        playerAgent.isStopped = true;
        return valid;
    }
	
	public void UpdateValidRooms()
    {
        for (int i = 0; i < invalidSpawnPoints.Count; i++)
        {
            Vector3 destination = invalidSpawnPoints[i].transform.position;
            destination.z = GameController.Z_OffsetLevel3D;
            
            if (IsValidDestination(destination))
            {
                validSpawnPoints.Add(invalidSpawnPoints[i]);
                invalidSpawnPoints.RemoveAt(i);
            }
        }
    }

    public void UpdateSpawnPoints()
    {
        currentSpawnPoints = new List<Transform>();
        List<Transform> closestPoints = validSpawnPoints.OrderBy(x => CalculateWalkingDistance(x)).ToList();

        for (int i = 0; i < maxSpawnPoints && i < closestPoints.Count; i++)
        {
            currentSpawnPoints.Add(closestPoints[i]);
        }
    }

    //please work
    private float CalculateWalkingDistance(Transform t)
    {
        NavMeshPath path = new NavMeshPath();
        Vector3 destination = t.position;
        destination.z = GameController.Z_OffsetLevel3D;
        playerAgent.CalculatePath(destination, path);
        float distance = 0;

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            distance += Vector2.Distance(path.corners[i], path.corners[i + 1]);
        }

        return distance;
    }
}
