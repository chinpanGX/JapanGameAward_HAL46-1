using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Invoke("ChangeScene", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static  void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

}
