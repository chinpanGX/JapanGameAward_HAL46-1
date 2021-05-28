using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //音を再生するオブジェクトのプレハブ
    [SerializeField] GameObject audioprefab;
    static GameObject audioset;

    public enum AudioType
    {
        BGM,
        SE,
    }

    //音データ構造体
    [System.Serializable]
    public struct K_Audio
    {
        public string name;
        public AudioClip audioclip;
        public AudioType audiotype;
        public float loopstart;//設定しない場合は0
        public float loopend;//設定しない場合は0
    }

    //音データを入れる配列
    [SerializeField] K_Audio[] AudioData;
    static K_Audio[] AudioDataSet;


    static private List<AudioController> audiolist;

    private void Awake()
    {
        audioset = audioprefab;
        AudioDataSet = AudioData;

        audiolist = new List<AudioController>();
    }

    private void Update()
    {

        for (int i = 0; i < audiolist.Count; i++)
        {
            if (!audiolist[i].gameObject.activeSelf)
            {
                Destroy(audiolist[i].gameObject);
                audiolist.RemoveAt(i);
            }
        }
    }


    //音再生
    //name = 配列に設定した音の名前
    //loop = ループ処理
    //fade = フェード処理
    //fadetime = フェード時間
    static public AudioController PlayAudio(string name, bool loop, bool fade, int fadeflame = 60)
    {
        GameObject obj = Instantiate(audioset);//音作成
        AudioController audiocon = obj.GetComponent<AudioController>();

        //音探し
        foreach (var clip in AudioDataSet)
        {
            if (clip.name == name)//同じ名前探し
            {
                obj.name = "Audio_" + clip.name;
                audiocon.PlayAudio(clip.audioclip, fade, loop, fadeflame, clip.loopstart, clip.loopend);
                break;
            }
        }
        audiolist.Add(audiocon);
        return audiocon;
    }

    static public void AllFadeOutAudio()
    {
        foreach (var faudio in audiolist)
        {
            faudio.FadeOutStart();
        }

        audiolist.Clear();
    }

    static public void AllStopAudio()
    {
        foreach (var saudio in audiolist)
        {
            saudio.Stop();
        }

        audiolist.Clear();
    }
    
}
