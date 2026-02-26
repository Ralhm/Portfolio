using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "EnemySpawnData.asset")]
public class EnemySpawnData : ScriptableObject
{
    [SerializeField]
    public List<Enemy> Enemies;
}
