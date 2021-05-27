using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 音效枚举 (根据文本添加的)
/// </summary>
public enum AudioEffectSet {
    Hit,                
    Bonus, 
    GameOver, 
    Click,
}

/// <summary>
/// 背景音乐枚举
/// </summary>
public enum BGMSet
{

    ChillMusic,

}

public class AudioManager : Singleton<AudioManager>
{
    //音效文本路径
    private static string audioTextPathPrefix = Application.dataPath + "/Resources/";
    private const string audioTextPathMiddle = "Audio/AudioList";
    private const string audioTextPathPostfix = ".txt";

    //背景音乐文本路径
    private static string BGMTextPathPrefix = Application.dataPath + "/Resources/";
    private const string BGMTextPathMiddle = "BGM/BGMList";
    private const string BGMTextPathPostfix = ".txt";
    private AudioSource BGMAudioSource;
    /// <summary>
    /// 音效文本文件全路径
    /// </summary>
    public static string AudioTextPath
    {
        get
        {
            return audioTextPathPrefix + audioTextPathMiddle + audioTextPathPostfix;
        }
    }

    /// <summary>
    /// 背景音乐文本文件全路径
    /// </summary>
    public static string BGMTextPath
    {
        get
        {
            return BGMTextPathPrefix + BGMTextPathMiddle + BGMTextPathPostfix;
        }
    }

    // 音效字典
    private Dictionary<string, AudioClip> audioClipDict = new Dictionary<string, AudioClip>();
    // 背景音乐字典
    private Dictionary<string, AudioClip> BGMClipDict = new Dictionary<string, AudioClip>();

    private bool isMute = false;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AudioManager()
    {
        LoadBGMAndAudioClip();
    }



    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        
    }

    /// <summary>
    /// 是否静音
    /// </summary>
    public bool IsMute { get => isMute;
        set { isMute = value;


            // 如果没有 音频源
            if (BGMAudioSource == null)
            {
                BGMAudioSource = Camera.main.gameObject.AddComponent<AudioSource>();

            }

            if (isMute == true)
            {
               
                // 暂停音乐
                BGMAudioSource.Pause();
            }
            else {

                // 播放音乐
                BGMAudioSource.Play();
            }
        }
    }

    private void LoadBGMAndAudioClip() {
        audioClipDict = LoadAudioClip(audioTextPathMiddle);
        BGMClipDict = LoadAudioClip(BGMTextPathMiddle);
    }

    /// <summary>
    /// 从文本中加载音频数据
    /// </summary>
    private Dictionary<string, AudioClip> LoadAudioClip(string path)
    {
        Dictionary<string, AudioClip>  audioClipDict = new Dictionary<string, AudioClip>();
        // 加载文本数据
        TextAsset ta = Resources.Load<TextAsset>(path);

        // 解析文本数据内容，并添加到字典中
        string[] lines = ta.text.Split('\n');
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line)) continue;
            string[] keyvalue = line.Split(',');
            string key = keyvalue[0];
            AudioClip value = Resources.Load<AudioClip>(keyvalue[1]);
            audioClipDict.Add(key, value);
        }

        return audioClipDict;
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    /// <param name="volume">音量</param>
    /// <param name="isLoop">循环与否</param>
    public void PlayBGM(BGMSet name, float volume = 2.0f, bool isLoop = true) {

        // 如果静音，则不播放声音
        if (IsMute) return;

        if (BGMAudioSource == null) {
            BGMAudioSource = Camera.main.gameObject.AddComponent<AudioSource>();
           
        }

        // 设置背景音乐音量和循环与否
        BGMAudioSource.Stop();
        BGMAudioSource.volume = volume;
        BGMAudioSource.loop = isLoop;


        // 从字典中获取对应音效进行播放
        AudioClip ac;
        BGMClipDict.TryGetValue(name.ToString(), out ac);
        if (ac != null)
        {
            BGMAudioSource.clip = (ac);
            BGMAudioSource.Play();
        }

        

    }


    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name">音效名称</param>
    public void PlayAudioEffect(AudioEffectSet name)
    {
        PlayAudioEffect(name, Vector3.zero);
    }

    /// <summary>
    /// 带位置播放音效
    /// </summary>
    /// <param name="name">音效名称</param>
    /// <param name="position">位置</param>
    public void PlayAudioEffect(AudioEffectSet name, Vector3 position)
    {
        // 如果静音，则不播放声音
        if (IsMute) return;

        // 从字典中获取对应音效进行播放
        AudioClip ac;
        audioClipDict.TryGetValue(name.ToString(), out ac);
        if (ac != null)
        {
            AudioSource.PlayClipAtPoint(ac, position);
        }
    }
}
