using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMove : MonoBehaviour
{
    [SerializeField]int MoveFlame;

    public bool moveflag { get; set; }
    private Vector3 target;
    private Vector3 speed;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        if (moveflag)
        {
            //‹——£‚ª‹ß‚­‚È‚Á‚½‚ç
            float distance = Vector3.Distance(this.transform.position, target);
            if (0.01f >= distance)
            {
                this.transform.position = target;
                moveflag = false;
                return;
            }
            //ˆÚ“®
            this.transform.position += speed;
        }
    }

    public void StartMove(Vector3 target, Vector3 start)
    {
        moveflag = true;
        this.target = target;
        this.transform.position = start;

        //ˆÚ“®•ûŒü‚Æ‘¬“xŒvŽZ
        speed = target - start;
        speed /= MoveFlame;
    }

    public void StartMove(Vector3 target)
    {
        StartMove(target, this.transform.position);
    }
}
