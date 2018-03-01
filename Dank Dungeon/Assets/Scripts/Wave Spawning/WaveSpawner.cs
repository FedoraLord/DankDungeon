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

    private bool roomsInitialized;
    
	void Start () {
        playerAgent = GameObject.FindGameObjectWithTag("Player3DPosition").GetComponent<NavMeshAgent>();
        StartCoroutine(Setup());
    }

    /// <summary>
    /// Co-routine method that calls InitializeValidRooms after allowing for the NavMesh to be baked.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Setup()
    {
        //wait for navmesh to be baked
        yield return new WaitForEndOfFrame();
        yield return InitializeValidRooms();
    }
    
    private void Update()
    {
        for (int i = 0; i < currentSpawnPoints.Count; i++)
        {
            test[i].transform.position = currentSpawnPoints[i].position;
        }
    }

    /// <summary>
    /// A Co-routine method to dynamically get all the rooms in the scene and initialize lists for the WaveSpawner instance.
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitializeValidRooms()
    {
        GameObject[] allRooms = GameObject.FindGameObjectsWithTag("Room");
        for (int i = 0; i < allRooms.Length; i++)
        {
            Vector3 dest = allRooms[i].transform.position;
            dest.z = GameController.Z_OffsetLevel3D;
            playerAgent.SetDestination(dest);

            //give the agent time to calculate the path
            yield return new WaitForEndOfFrame();

            if (playerAgent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                validSpawnPoints.Add(allRooms[i].transform);
            }
            else
            {
                invalidSpawnPoints.Add(allRooms[i].transform);
            }
        }

        roomsInitialized = true;
        UpdateSpawnPoints();
    }
	
    /// <summary>
    /// Updates where the enemies spawn from. Chooses valid rooms closest to the player. Call this when the player changes rooms.
    /// </summary>
    public void UpdateSpawnPoints()
    {
        if (roomsInitialized)
            StartCoroutine(Co_UpdateSpawnPoints());
    }

    private struct PointDistancePair
    {
        public Transform point;
        public float distance;
    }
    
    private IEnumerator Co_UpdateSpawnPoints()
    {
        List<PointDistancePair> pairs = new List<PointDistancePair>();

        for (int i = 0; i < validSpawnPoints.Count; i++)
        {
            if (validSpawnPoints[i] == PlayerController.lastRoom)
                continue;

            Vector3 destination = validSpawnPoints[i].position;
            destination.z = GameController.Z_OffsetLevel3D;
            playerAgent.SetDestination(destination);

            yield return new WaitForEndOfFrame();
            
            pairs.Add(new PointDistancePair()
            {
                point = validSpawnPoints[i],
                distance = CalculatePathDistance(playerAgent.path.corners)
            });
        }

        pairs = pairs.OrderBy(x => x.distance).ToList();
        currentSpawnPoints = new List<Transform>();

        for (int i = 0; i < pairs.Count && i < maxSpawnPoints; i++)
        {
            currentSpawnPoints.Add(pairs[i].point);
        }
    }

    /// <summary>
    /// Updates the possible rooms enemies can spawn from. Call this when new rooms and pathways become available.
    /// </summary>
    public void UpdateValidRooms()
    {
        StartCoroutine(Co_UpdateValidRooms());
    }

    private IEnumerator Co_UpdateValidRooms()
    {
        List<Transform> newRooms = new List<Transform>();

        for (int i = 0; i < invalidSpawnPoints.Count; i++)
        {
            Vector3 dest = invalidSpawnPoints[i].transform.position;
            dest.z = GameController.Z_OffsetLevel3D;
            playerAgent.SetDestination(dest);
            
            yield return new WaitForEndOfFrame();

            if (playerAgent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                newRooms.Add(invalidSpawnPoints[i]);
            }
        }

        foreach (Transform t in newRooms)
        {
            validSpawnPoints.Add(t);
            invalidSpawnPoints.Remove(t);
        }

        UpdateSpawnPoints();
    }

    /// <summary>
    /// Calculates the walking distance of the corners on a NavMeshPath.
    /// </summary>
    /// <param name="corners">The corners of the computed NavMeshPath</param>
    /// <returns>The total distance along the NavMeshPath</returns>
    private float CalculatePathDistance(Vector3[] corners)
    {
        float distance = 0;
        for (int i = 0; i < corners.Length - 1; i++)
        {
            distance += Vector3.Distance(corners[i], corners[i + 1]);
        }
        return distance;
    }
}
