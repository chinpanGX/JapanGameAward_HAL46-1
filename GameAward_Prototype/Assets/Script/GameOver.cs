using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    private Water m_Water;
    public bool m_Flag;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        m_Water = GetComponent<Water>();
    }

    // Update is called once per frame
    void Update()
    {
        m_Flag = m_Water.GetGameOverFlag();
        if (m_Flag == true)
        {
           
        }
        gameObject.SetActive(true);
    }
}
