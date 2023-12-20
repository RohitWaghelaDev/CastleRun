using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="GameLevels",menuName ="ScriptableObjects/Levels")]
public class GameLevels : ScriptableObject
{
    [SerializeField]
    private List<GameObject> levels = new List<GameObject>();

    public GameObject GetLevel(int level)
    {
        if (level>= levels.Count)
        {
            return levels[levels.Count - 1];
        }
        return levels[level];
    }
}
