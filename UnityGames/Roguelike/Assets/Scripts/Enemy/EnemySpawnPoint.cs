using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Maybe instead of making several scriptable that contain lists, you might as well just make several prefabs
//that's what it would boil down to anyways, each prefab would have it's own list
//but instead of separating those into a scriptable list and an identical prefab whose only difference is the list, just make the list part of the prefab

public class EnemySpawnPoint : MonoBehaviour
{
    bool test;
    public int EnemiesToSpawn = 1;
    public float SpawnOffset;
    public List<Enemy> Enemies;
    public Room CurrentRoom;


    // Start is called before the first frame update
    void Start()
    {
        CurrentRoom = GetComponentInParent<Room>();
        CurrentRoom.AddSpawnPoint(this);
    }


    public void SpawnEnemy()
    {
        for (int i = 0; i < EnemiesToSpawn; i++)
        {
            float rand = Random.Range(-SpawnOffset, SpawnOffset);
            int enemyRand = Random.Range(0, Enemies.Count);
            Vector3 spawnPos = transform.position + new Vector3(rand, rand, 0);
            CurrentRoom.SpawnEnemyAtLocation(spawnPos, Enemies[enemyRand]);

        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, SpawnOffset);
    }
#endif
}
