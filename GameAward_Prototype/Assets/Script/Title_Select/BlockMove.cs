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
            //距離が近くなったら
            float distance = Vector3.Distance(this.transform.position, target);
            if (0.01f >= distance)
            {
                this.transform.position = target;
                moveflag = false;
                return;
            }
            //移動
            this.transform.position += speed;
        }
    }

    public void StartMove(Vector3 target, Vector3 start)
    {
        moveflag = true;
        this.target = target;
        this.transform.position = start;

        //移動方向と速度計算
        speed = target - start;
        speed /= MoveFlame;
    }

    public void StartMove(Vector3 target)
    {
        StartMove(target, this.transform.position);
    }
}
