using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private float _globalRespawnTimer;
    public float globalRespawnTimer = 10;

    public Action OnGlobalRespawn;

    public List<SpawnPoint> spawnPoints;

    public bool useGlMaxEntitesAlive;
    public bool useGlRespawnTime;

    private void Awake()
    {
        spawnPoints = new List<SpawnPoint>(GetComponentsInChildren<SpawnPoint>());
    }

    // Update is called once per frame
    //void Update () {

    //       if(useGlRespawnTime) {

    //           _globalRespawnTimer -= Time.deltaTime;

    //           if(_globalRespawnTimer <= 0) {

    //               AttemptGlobalRespawn();
    //               ResetRespawnTimer();
    //           }
    //       }
    //}

    //void ResetRespawnTimer() {

    //    _globalRespawnTimer = globalRespawnTimer;
    //}

    //void AttemptGlobalRespawn() {

    //    for(int i = 0; i < spawnPoints.Count; i++) {

    //        SpawnPoint sp = spawnPoints[i];
    //    }
    //}
}