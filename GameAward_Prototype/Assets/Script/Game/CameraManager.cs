using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float Length;//����


    private GameObject player;//�v���C���[
    private Player playerplayer;

    Vector3 playerposi;

    // Start is called before the first frame update
    void Start()
    {
        //�v���C���[�I�u�W�F�N�g�Ⴄ
        GameObject PlayerObj = GameObject.Find("Player").gameObject;//�v���C���[�I�u�W�F
        player = PlayerObj.transform.Find("PlayerModel").gameObject;//�v���C���[���f��
        playerplayer = PlayerObj.GetComponent<Player>();
        

        //�v���C���[�̍��W���L�^����
        playerposi = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerplayer.ClearFlag == 0)
        {
            playerposi.x = player.transform.position.x;
            playerposi.z = player.transform.position.z;

            playerposi.y = player.transform.position.y;

            Vector3 zikuposi = Vector3.zero;
            Vector3 kijun = Vector3.zero;

            if (playerposi.y < 2.0f)
            {
                zikuposi = new Vector3(playerposi.x, 2.0f, playerposi.z);

                kijun = new Vector3(0.0f, 2.0f, 0.0f);
            }
            else
            {
                zikuposi = new Vector3(playerposi.x, player.transform.position.y, playerposi.z);

                kijun = new Vector3(0.0f, player.transform.position.y, 0.0f);
            }



            //�x�N�g��
            Vector3 vec = zikuposi - kijun;
            Vector3.Normalize(vec);//���K��

            //���W�ݒ�
            this.transform.position = kijun + (vec * Length);

            //���|�W������
            this.transform.LookAt(zikuposi);
        }
    }
}
