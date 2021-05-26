using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//�X�e�[�^�X��t���O���Ǘ�����}�l�[�W���[
public class StatusFlagManager : MonoBehaviour
{
    static public readonly int GAME_NONE = 0;
    static public readonly int GAME_START = 1;
    static public readonly int GAME_OVER = 2;
    static public readonly int GAME_CLEAR = 3;
    static public int GameStatusFlag = GAME_NONE;//�Q�[���̌��݂̏��

    static public readonly int SCENE_NONE = 0;
    static public readonly int SCENE_TITLE = 1;
    static public readonly int SCENE_STAGESELECT = 2;
    static public readonly int SCENE_PAUSE = 3;
    static public readonly int SCENE_RESULT = 4;
    static public readonly int SCENE_GAME = 5;
    static public int SceneFlag = SCENE_NONE;//�V�[���̌��݂̏��

    [SerializeField] StageManager stagecon;//�X�e�[�W�}�l�[�W���[
    static public int SelectStageID = 0;//���ݑI��ł�X�e�[�WID

    private void Awake()
    {
        //��ԍŏ��̃V�[��
        SceneFlag = SCENE_GAME;

        //���ݑI��ł�X�e�[�W(�f�o�b�O�̂��)
        SelectStageID = stagecon.NowStage;
    }
}
