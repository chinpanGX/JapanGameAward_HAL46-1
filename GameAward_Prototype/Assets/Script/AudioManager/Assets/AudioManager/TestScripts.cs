using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScripts : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            AudioManager.Instance.PlayAudioEffect(AudioEffectSet.Hit);
            Debug.Log("播放音效");//効果音Play
        }

        if (Input.GetMouseButtonDown(1))
        {
            AudioManager.Instance.PlayAudioEffect(AudioEffectSet.Click, Vector3.one * 10);
            Debug.Log("播放带位置音效");//
        }

        if (Input.GetMouseButtonDown(2))
        {
            AudioManager.Instance.PlayBGM(BGMSet.ChillMusic);
            Debug.Log("播放背景音乐");//BGM再生
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.Instance.IsMute = !AudioManager.Instance.IsMute;
            Debug.Log(AudioManager.Instance.IsMute ? "静音" : "不静音");//ミュート解除
        }
    }
}
