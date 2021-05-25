/*------------------------------------------------------------
 
    [Fade.cs]
    Author : 出合翔太

    fadeの処理

    FadeOut➞画面が暗くなり、引数NextSceneNameに遷移したいシーンの名前を入れる
    FadeIN->暗くなった画面を徐々に明るくする遷移先のシーンでstart時に呼ぶ
 
 -------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    //フェード用のCanvasとImage
    private static Canvas m_fadeCanvas;
    private static Image m_fadeImage;

    //フェード用Imageの透明度
    private static float m_alpha = 0.0f;

    //フェードインアウトのフラグ
    public static bool m_isFadeIn = false;
    public static bool m_isFadeOut = false;

    //フェードしたい時間（単位は秒）
    private static float m_fadeTime = 3.0f;

    //遷移先のシーン番号
    private static string nextScene;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        //フラグ有効なら毎フレームフェードイン/アウト処理
        if (m_isFadeIn)
        {
            //経過時間から透明度計算
            m_alpha -= Time.deltaTime / m_fadeTime;

            //フェードイン終了判定
            if (m_alpha <= 0.0f)
            {
                m_isFadeIn = false;
                m_alpha = 0.0f;
                m_fadeCanvas.enabled = false;
            }

            //フェード用Imageの色・透明度設定
            m_fadeImage.color = new Color(0.0f, 0.0f, 0.0f, m_alpha);
        }
        else if (m_isFadeOut)
        {
            //経過時間から透明度計算
            m_alpha += Time.deltaTime / m_fadeTime;

            //フェードアウト終了判定
            if (m_alpha >= 1.0f)
            {
                m_isFadeOut = false;
                m_alpha = 1.0f;

                //次のシーンへ遷移
                Scene.ChangeScene(nextScene);
            }

            //フェード用Imageの色・透明度設定
            m_fadeImage.color = new Color(0.0f, 0.0f, 0.0f, m_alpha);
        }
    }


    static void Init()
    {
        //フェード用のCanvas生成
        GameObject FadeCanvasObject = new GameObject("CanvasFade");
        m_fadeCanvas = FadeCanvasObject.AddComponent<Canvas>();
        FadeCanvasObject.AddComponent<GraphicRaycaster>();
        m_fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        FadeCanvasObject.AddComponent<Fade>();

        //最前面になるよう適当なソートオーダー設定
        m_fadeCanvas.sortingOrder = 100;

        //フェード用のImage生成
        m_fadeImage = new GameObject("ImageFade").AddComponent<Image>();
        m_fadeImage.transform.SetParent(m_fadeCanvas.transform, false);
        m_fadeImage.rectTransform.anchoredPosition = Vector3.zero;

        //Imageサイズは適当に大きく設定
        m_fadeImage.rectTransform.sizeDelta = new Vector2(9999, 9999);
    }

    //フェードイン開始
    public static void FadeIn()
    {
        if (m_fadeImage == null)
        {
            Init();
        }
        m_fadeImage.color = Color.black;
        m_isFadeIn = true;
    }

    //フェードアウト開始->paramname "NextSceneName" = 遷移したいシーンの名前を設定する
    public static void FadeOut(string NextSceneName)
    {
        if (m_fadeImage == null)
        {
            Init();
        }
        nextScene = NextSceneName;
        m_fadeImage.color = Color.clear;
        m_fadeCanvas.enabled = true;
        m_isFadeOut = true;
    }
}
