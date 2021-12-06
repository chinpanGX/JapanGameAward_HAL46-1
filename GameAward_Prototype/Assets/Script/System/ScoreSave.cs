using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSave : MonoBehaviour
{
    //�Z�[�u�f�[�^
    public const int SD_TIME = 1 << 0;
    public const int SD_ACTION = 1 << 1;
    public const int SD_MISS = 1 << 2;
    public static int[] SavaData = null;


    private void Start()
    {
        SavaData = new int[StatusFlagManager.StageMaxNum];

        //�O����
        for (int i = 0; i < SavaData.Length; i++)
        {
            SavaData[i] = 0;
        }

        //�Z�[�u�f�[�^�ǂݍ���
        ScoreLoad();
    }

    //�^�C���̃f�[�^���Z�b�g
    public static void SetTime(int stageid)
    {
        SavaData[stageid] |= SD_TIME;
    }

    //�A�N�V�����̃f�[�^���Z�b�g
    public static void SetAction(int stageid)
    {
        SavaData[stageid] |= SD_ACTION;
    }

    //�~�X�̃f�[�^���Z�b�g
    public static void SetMiss(int stageid)
    {
        SavaData[stageid] |= SD_MISS;
    }

    //�Z�[�u�f�[�^���[�h
    public static void ScoreLoad()
    {
        //�Z�[�u�f�[�^���Ȃ��ꍇ�������Ȃ�
        if (!IsSavaData())
        {
            return;
        }

        //�X�R�A�擾
        string data = PlayerPrefs.GetString("GameData");

        //�ǂݍ��񂾃f�[�^���J���}�Ŕz��ɕ���
        string[] dataArray = data.Split(',');

        //�Z�[�u�f�[�^�ɕ��荞��
        for (int i = 0; i < StatusFlagManager.StageMaxNum; i++)
        {
            SavaData[i] = int.Parse(dataArray[i]);
        }
    }

    //�f�[�^���Z�[�u
    public static void ScoreSava()
    {
        //������f�[�^����
        string data = null;

        //�f�[�^���R���}�ŋ�؂��ăZ�b�g
        for (int i = 0; i < SavaData.Length; i++)
        {
            data = data + SavaData[i].ToString() + ",";
        }

        PlayerPrefs.SetString("GameData", data);
        PlayerPrefs.Save();
    }

    //�Z�[�u�f�[�^�폜
    public static void SavaDataDelete()
    {
        PlayerPrefs.DeleteKey("GameDeta");
    }

    public static void SavaDataClear()
    {
        //�O����
        for (int i = 0; i < SavaData.Length; i++)
        {
            SavaData[i] = 0;
        }
    }

    //�Z�[�u�f�[�^�̗L��
    public static bool IsSavaData()
    {
        if (SavaData == null)
        {
            return false;
        }

        foreach (var item in SavaData)
        {
            if (item != 0)
            {
                return true;
            }
        }
        return false;
    }
}
