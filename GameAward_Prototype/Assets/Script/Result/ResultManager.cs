using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    private GameObject camera;


    private const int RESULT_NONE = 0;
    private const int RESULT_START = 1;
    private int result;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (StatusFlagManager.SceneFlag != StatusFlagManager.SCENE_RESULT)
        {
            return;
        }

        if (result == RESULT_NONE)
        {

            Vector3 vec = camera.transform.position - player.transform.position;
            vec = Vector3.Normalize(vec);

            camera.GetComponent<BlockMove>().StartMove(player.transform.position + vec + new Vector3(-0.75f, 0.5f, 0.7f));
            result = RESULT_START;
        }
    }
}
