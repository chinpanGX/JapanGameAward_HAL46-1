using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        //ˆê”ÔÅ‰İ’è
        this.GetComponent<Create>().SetPlayer(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
