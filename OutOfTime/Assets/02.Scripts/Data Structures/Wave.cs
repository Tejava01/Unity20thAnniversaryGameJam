using UnityEngine;
using System;

// [Serializable] flag means it can be assigned in the inspector.
// in BattleManager, add a field: [SerializeField] List<Wave> waves;
[Serializable]
public class Wave
{
    public GameObject enemy_prefab;
    public int count;
}
