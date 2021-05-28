using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //�����Đ�����I�u�W�F�N�g�̃v���n�u
    [SerializeField] GameObject audioprefab;
    static GameObject audioset;

    //���f�[�^�\����
    [System.Serializable]
    public struct K_Audio
    {
        public string name;
        public AudioClip audioclip;
        public float loopstart;//�ݒ肵�Ȃ��ꍇ��0
        public float loopend;//�ݒ肵�Ȃ��ꍇ��0
    }

    //���f�[�^������z��
    [SerializeField] K_Audio[] AudioData;
    static K_Audio[] AudioDataSet;

    private void Awake()
    {
        audioset = audioprefab;
        AudioDataSet = AudioData;
    }

    //���Đ�
    //name = �z��ɐݒ肵�����̖��O
    //loop = ���[�v����
    //fade = �t�F�[�h����
    //fadetime = �t�F�[�h����
    static public AudioController PlayAudio(string name, bool loop, bool fade, int fadeflame = 60)
    {
        GameObject obj = Instantiate(audioset);//���쐬
        AudioController audiocon = obj.GetComponent<AudioController>();

        //���T��
        foreach (var clip in AudioDataSet)
        {
            if (clip.name == name)//�������O�T��
            {
                obj.name = "Audio_" + clip.name;
                audiocon.PlayAudio(clip.audioclip, fade, loop, fadeflame, clip.loopstart, clip.loopend);
                break;
            }
        }

        return audiocon;
    }
}
