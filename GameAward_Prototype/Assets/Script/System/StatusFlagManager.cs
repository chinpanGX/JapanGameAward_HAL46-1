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
    static public readonly int GAME_PLAY = 4;
    static public readonly int GAME_RESULT = 5;
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

    static public int StageMaxNum = 0;

    static public int MissCount = 0;

    private void Awake()
    {
        GameStatusFlag = GAME_START;

        //�f�o�b�O�p�ݒ�
        SceneFlag = SCENE_GAME;

        //���ݑI��ł�X�e�[�W(�f�o�b�O�̂��)
        SelectStageID = stagecon.NowStage;

        //�ő�X�e�[�W��
        StageMaxNum = stagecon.stage.Length;
    }
}
