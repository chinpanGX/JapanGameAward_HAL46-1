using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AudioManagerWindowEditor : EditorWindow
{
    // 音频列表文本的保存路径
    private static string AUDIO_TEXT_PATH = Application.dataPath + "/Resources/Audio/AudioList.txt";

    // 菜单栏显示
    [MenuItem("Manager/AudioManager")]
    static void CreateWindow()
    {
        // 显示编辑扩展器面板
        AudioManagerWindowEditor window = EditorWindow.GetWindow<AudioManagerWindowEditor>("音效管理");
        window.Show();
    }

    // 音频名称，音频路径，音频字典
    private string saveFileName;
    private string audioName;
    private string audioPath;
    private Dictionary<string, string> audioDict = new Dictionary<string, string>();

    void Awake()
    {
        // 面板打开加载数据
        LoadAudioList();
    }

    /// <summary>
    /// 面板绘制
    /// </summary>
    void OnGUI()
    {

        // 音频列表数据标题栏
        GUILayout.BeginHorizontal();
        GUILayout.Label("音效名称");
        GUILayout.Label("音效路径");
        GUILayout.Label("操作");
        GUILayout.EndHorizontal();

        // 通过音频列表数据，逐行显示在面板中
        foreach (string key in audioDict.Keys)
        {
            string value;
            audioDict.TryGetValue(key, out value);
            GUILayout.BeginHorizontal();
            GUILayout.Label(key);
            GUILayout.Label(value);
            if (GUILayout.Button("删除"))
            {
                // 删除音频字典数据指定值，并保存文本
                audioDict.Remove(key);
                SaveAudioList();
                return;
            }
            GUILayout.EndHorizontal();
        }

        // 输入添加音频数据
        audioName = EditorGUILayout.TextField("音效名字", audioName);
        audioPath = EditorGUILayout.TextField("音效路径", audioPath);

        string[] strs = Selection.assetGUIDs;


        string path = AssetDatabase.GUIDToAssetPath(strs[0]);
        Debug.Log(path + " ");

        if (GUILayout.Button("添加音效"))
        {
            object o = Resources.Load(audioPath);
            if (o == null)
            {
                Debug.LogWarning("音效不存在于" + audioPath + " 添加不成功");
                audioPath = "";
            }
            else
            {
                if (audioDict.ContainsKey(audioName))
                {
                    Debug.LogWarning("名字已经存在，请修改");
                }
                else
                {
                    // 添加到音频字典中，并保存数据到文本中
                    audioDict.Add(audioName, audioPath);
                    SaveAudioList();
                   
                }
            }
        }
    }

    /// <summary>
    /// 实时更新数据
    /// </summary>
    void OnInspectorUpdate()
    {

        LoadAudioList();
    }


    /// <summary>
    /// 保存新的音频列表数据
    /// </summary>
    private void SaveAudioList()
    {
        StringBuilder sb = new StringBuilder();
        //从音频字典中获取数据
        foreach (string key in audioDict.Keys)
        {
            string value;
            audioDict.TryGetValue(key, out value);
            sb.Append(key + "," + value + "\n");
        }
        //写入数据
        File.WriteAllText(AUDIO_TEXT_PATH, sb.ToString());
        // 刷新 Asset 文件夹数据
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 加载保存的音频列表数据
    /// </summary>
    private void LoadAudioList()
    {
        audioDict = new Dictionary<string, string>();
        // 如果没有音频列表数据，则返回
        if (File.Exists(AUDIO_TEXT_PATH) == false) return;

        //
        string[] lines = File.ReadAllLines(AUDIO_TEXT_PATH);
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line)) continue;
            string[] keyvalue = line.Split(',');
            audioDict.Add(keyvalue[0], keyvalue[1]);
        }
    }
}
