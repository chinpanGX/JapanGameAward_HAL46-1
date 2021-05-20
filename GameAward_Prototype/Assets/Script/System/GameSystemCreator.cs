using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystemCreator : MonoBehaviour
{
    private static bool Loaded { get; set; }

    [SerializeField]
    GameObject[] GameSystemPrefabs = null;

    private void Awake()
    {
        if (Loaded) return;
        Loaded = true;
        foreach (var prefab in GameSystemPrefabs)
        {
            GameObject go = Instantiate(prefab);
            DontDestroyOnLoad(prefab);
        }
    }
}
