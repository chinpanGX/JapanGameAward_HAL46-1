using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    AudioSource setaudio = null;


    private bool fadein;
    public bool FadeIn { get { return fadein; } }

    private bool fadeout;
    public bool FadeOut { get { return fadeout; } }


    private float fadespeed;
    private float loopstart;
    private float loopend;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (setaudio.loop && loopend != 0.0f)//ループ状態で終わりが設定されている場合
        {
            if (setaudio.time >= loopend)//再生位置が指定した場所にたどり着いたら
            {
                setaudio.time = loopstart;
            }
        }

        //音が流し終わった場合ストップ処理をする
        if (!setaudio.isPlaying)
        {
            Stop();
        }
    }

    private void FixedUpdate()
    {
        if (fadein)
        {
            setaudio.volume += fadespeed;

            if (setaudio.volume >= 1.0f)//音量最大になったら
            {
                setaudio.volume = 1.0f;
                fadein = false;
            }
        }
        else if (fadeout)
        {
            setaudio.volume -= fadespeed;

            if (setaudio.volume <= 0.0f)//音量が０になったら
            {
                Stop();
            }
        }
    }

    public void PlayAudio(AudioClip audioClip, bool fadein, bool loop, int fadeflame = 60, float audioloopstart = 0.0f, float audioloopend = 0.0f)
    {
        setaudio = this.GetComponent<AudioSource>();//音再生機能追加
        setaudio.playOnAwake = false;//最初から再生しないようにする
        setaudio.clip = audioClip;//音声データをセット
        setaudio.loop = loop;//ループ処理
        loopstart = audioloopstart;
        loopend = audioloopend;

        

        if (fadein)//フェード分岐
        {
            this.fadein = true;
            fadeout = false;
            setaudio.volume = 0.0f;//音量
            fadespeed = 1.0f / (float)fadeflame;
        }
        else
        {
            this.fadein = false;
            fadeout = false;
            setaudio.volume = 1.0f;//音量
            fadespeed = 0.0f;
        }
        
        //再生
        setaudio.Play();
    }

    //音のフェードアウト
    public void FadeOutStart(int fadeflame = 60)
    {
        fadeout = true;
        fadein = false;

        fadespeed = 1.0f / (float)fadeflame;
    }

    //一時停止
    public void Pause()
    {
        setaudio.Pause();
    }

    //一時停止解除
    public void UnPause()
    {
        setaudio.UnPause();
    }

    //ストップ
    //注意　オブジェクト事消すのでまた最初から再生する際はAudioManagerから
    public void Stop()
    {
        setaudio.Stop();
        this.gameObject.SetActive(false);
    }
}
