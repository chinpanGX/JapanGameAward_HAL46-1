using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSave : MonoBehaviour
{
    //セーブデータ
    public const int SD_TIME = 1 << 0;
    public const int SD_ACTION = 1 << 1;
    public const int SD_MISS = 1 << 2;
    public static int[] SavaData = null;


    private void Start()
    {
        SavaData = new int[StatusFlagManager.StageMaxNum];

        //０埋め
        for (int i = 0; i < SavaData.Length; i++)
        {
            SavaData[i] = 0;
        }

        //セーブデータ読み込み
        ScoreLoad();
    }

    //タイムのデータをセット
    public static void SetTime(int stageid)
    {
        SavaData[stageid] |= SD_TIME;
    }

    //アクションのデータをセット
    public static void SetAction(int stageid)
    {
        SavaData[stageid] |= SD_ACTION;
    }

    //ミスのデータをセット
    public static void SetMiss(int stageid)
    {
        SavaData[stageid] |= SD_MISS;
    }

    //セーブデータロード
    public static void ScoreLoad()
    {
        //セーブデータがない場合処理しない
        if (!IsSavaData())
        {
            return;
        }

        //スコア取得
        string data = PlayerPrefs.GetString("GameData");

        //読み込んだデータをカンマで配列に分解
        string[] dataArray = data.Split(',');

        //セーブデータに放り込む
        for (int i = 0; i < StatusFlagManager.StageMaxNum; i++)
        {
            SavaData[i] = int.Parse(dataArray[i]);
        }
    }

    //データをセーブ
    public static void ScoreSava()
    {
        //文字列データ準備
        string data = null;

        //データをコンマで区切ってセット
        for (int i = 0; i < SavaData.Length; i++)
        {
            data = data + SavaData[i].ToString() + ",";
        }

        PlayerPrefs.SetString("GameData", data);
        PlayerPrefs.Save();
    }

    //セーブデータ削除
    public static void SavaDataDelete()
    {
        PlayerPrefs.DeleteKey("GameDeta");
    }

    public static void SavaDataClear()
    {
        //０埋め
        for (int i = 0; i < SavaData.Length; i++)
        {
            SavaData[i] = 0;
        }
    }

    //セーブデータの有無
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
