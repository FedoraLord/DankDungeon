using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour {
    
    public int maxSpawnPoints;
    public Transform enemyParent;
    public List<Wave> waves;

    private int waveIndex;
    private NavMeshAgent playerAgent;
    private List<Transform> currentSpawnPoints = new List<Transform>();
    private List<Transform> validSpawnPoints = new List<Transform>();
    private List<Transform> invalidSpawnPoints = new List<Transform>();

    private bool roomsInitialized;
    private bool wavesStarted;
    private WaveTimer countdown = new WaveTimer();
    private List<Enemy> aliveEnemies = new List<Enemy>();
    public Text waveCounter;
    private string timeLeft;

	void Start () {
        playerAgent = GameObject.FindGameObjectWithTag("Player3DPosition").GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Co-routine method that calls InitializeValidRooms after allowing for the NavMesh to be baked.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Setup()
    {
        //wait for navmesh to be baked
        yield return new WaitForEndOfFrame();
        yield return InitializeValidRooms();
    }
    
    private void Update()
    {
        if (countdown.isActive)
        {
            if (countdown.stopTime > Time.time)
            {
                //TODO Update the timer on screen
                timeLeft = ((Time.time - countdown.stopTime)*(-1)).ToString("0");
                waveCounter.text = ("NEXT WAVE: " + timeLeft);
            }
            else
            {
                countdown.Stop();
                StartCoroutine(ExecuteWaveCommands(waves[waveIndex]));
            }
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

        //TODO give a minimum spawn distance .Where(x => x.distance > something) but make sure it has at least minSpawnPoints
        pairs = pairs.OrderBy(x => x.distance).ToList();
        currentSpawnPoints = new List<Transform>();

        for (int i = 0; i < pairs.Count && i < maxSpawnPoints; i++)
        {
            currentSpawnPoints.Add(pairs[i].point);
        }

        if (!wavesStarted)
        {
            wavesStarted = true;
            StartWave();
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

    private struct WaveTimer
    {
        public bool isActive;
        public float stopTime;

        public void Start(float timeLength) {
            isActive = true;
            stopTime = Time.time + timeLength;
        }

        public void Stop()
        {
            isActive = false;
        }
    }

    public void StartWave()
    {
        if (waveIndex < waves.Count)
        {
            if (waves[waveIndex].timeBeforeWave > 0)
                countdown.Start(waves[waveIndex].timeBeforeWave);
            else
                StartCoroutine(ExecuteWaveCommands(waves[waveIndex]));
        }
    }

    private IEnumerator ExecuteWaveCommands(Wave currentWave)
    {
        int firstSpawn = 0;
        for (int i = 0; i < currentWave.commandQueue.Count; i++)
        {
            if (currentWave.commandQueue[i].type == WaveCommand.CommandType.Spawning)
            {
                firstSpawn = i;
                break;
            }
        }

        int currentCommand = firstSpawn;

        while (currentCommand < currentWave.commandQueue.Count)
        {
            List<WaveCommand> spawnCommands = new List<WaveCommand>();
            for (; currentCommand < currentWave.commandQueue.Count; currentCommand++)
            {
                if (currentWave.commandQueue[currentCommand].type == WaveCommand.CommandType.Spawning)
                    spawnCommands.Add(currentWave.commandQueue[currentCommand]);
                else
                    break;
            }

            //TODO if there is only one point spawning like 12 more, maybe distribute to the other points so it spawns 4 mobs of 1 command simultaneously. 
            //However, this will make the wave progress much faster which you may not want
            //HOWTO make a prior loop that builds a shuffled queue of the monsters, and let the second loop spaw them at each point ever x seconds
            int maxSpawned = spawnCommands.OrderByDescending(x => x.N).Select(x => x.N).First();
            for (int n = 0; n < maxSpawned; n++)
            {
                for (int i = 0; i < spawnCommands.Count; i++)
                {
                    if (spawnCommands[i].N > n)
                    {
                        Spawn(currentWave.Enemies[spawnCommands[i].enemyIndex], currentSpawnPoints[i % currentSpawnPoints.Count].position);
                    }
                }
                yield return new WaitForSeconds(currentWave.spawnRate);
            }

            List<WaveCommand> utilityCommands = new List<WaveCommand>();
            for (; currentCommand < currentWave.commandQueue.Count; currentCommand++)
            {
                if (currentWave.commandQueue[currentCommand].type == WaveCommand.CommandType.Utility)
                    utilityCommands.Add(currentWave.commandQueue[currentCommand]);
                else
                    break;
            }

            for (int i = 0; i < utilityCommands.Count; i++)
            {
                switch (utilityCommands[i].utility)
                {
                    case WaveCommand.UtilityCommand.WaitNSeconds:
                        yield return new WaitForSeconds(utilityCommands[i].N);
                        break;
                    case WaveCommand.UtilityCommand.WaitUntilNAlive:
                        while (aliveEnemies.Count > utilityCommands[i].N)
                        {
                            yield return new WaitForEndOfFrame();
                        }
                        break;
                }
            }
        }

        waveIndex++;
        StartWave();
    }

    private void Spawn(Enemy enemy, Vector3 position)
    {
        GameObject spawned = Instantiate(enemy.gameObject, position, Quaternion.identity, enemyParent);
        aliveEnemies.Add(spawned.GetComponent<Enemy>());
    }

    public void EnemyDied(Enemy dead)
    {
        aliveEnemies.Remove(dead);
    }
}
