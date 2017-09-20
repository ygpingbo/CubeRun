using UnityEngine;
using System.Collections;

/// <summary>
/// UI管理器
/// </summary>
public class UIManager : MonoBehaviour {

    private GameObject m_StartUI;
    private GameObject m_GameUI;

    private UILabel m_ScoreLabel;
    private UILabel m_GemLabel;
    private UILabel m_GameScoreLabel;
    private UILabel m_GameGemLabel;
    private PlayerController m_Player;

    private GameObject m_PlayButton;
    private GameObject m_PauseButton;
    private GameObject m_ContinueButton;
    private GameObject m_Left;
    private GameObject m_Right;

	// Use this for initialization
	void Start () {
        m_StartUI = GameObject.Find("Start_UI");
        m_GameUI = GameObject.Find("Game_UI ");
        //startUI
        m_ScoreLabel = GameObject.Find("label_score").GetComponent<UILabel>();
        m_GemLabel = GameObject.Find("label_gem").GetComponent<UILabel>();
        //gameUI
        m_GameScoreLabel = GameObject.Find("label_game_score").GetComponent<UILabel>();
        m_GameGemLabel = GameObject.Find("label_geme_gem").GetComponent<UILabel>();
        m_Player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        //按键
        m_PlayButton = GameObject.Find("btn_play");
        UIEventListener.Get(m_PlayButton).onClick = PlayButtonClick;    //委托
        m_PauseButton = GameObject.Find("btn_pause");
        UIEventListener.Get(m_PauseButton).onClick = PauseButtonClick;
        m_ContinueButton = GameObject.Find("btn_continue");
        UIEventListener.Get(m_ContinueButton).onClick = ContinueButtonClick;
        m_Left = GameObject.Find("btn_left");
        UIEventListener.Get(m_Left).onClick = Left;
        m_Right = GameObject.Find("btn_right");
        UIEventListener.Get(m_Right).onClick = Right;
        
        //设置UI初始显示
        m_StartUI.SetActive(true);
        m_GameUI.SetActive(false);
        m_PauseButton.SetActive(true);
        m_ContinueButton.SetActive(false);
        
        Init();
	}

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        //startUI
        m_ScoreLabel.text = PlayerPrefs.GetInt("score", 0).ToString();
        m_GemLabel.text = PlayerPrefs.GetInt("gem", 0).ToString();
        //gameUI
        m_GameScoreLabel.text = "0";
        m_GameGemLabel.text = PlayerPrefs.GetInt("gem", 0).ToString();
        
    }

    /// <summary>
    /// 点击btn_play按钮
    /// </summary>
    private void PlayButtonClick(GameObject go)
    {
        m_StartUI.SetActive(false);
        m_GameUI.SetActive(true);
        m_Player.StartGame();
    }

    /// <summary>
    /// 更新得分UI
    /// </summary>
    /// <param name="score"></param>
    /// <param name="gem"></param>
    public void UpdateData(int score, int gem)
    {
        //start
        m_ScoreLabel.text = PlayerPrefs.GetInt("score", 0).ToString();
        m_GemLabel.text = gem.ToString();
        //game
        m_GameScoreLabel.text = score.ToString();
        m_GameGemLabel.text = gem.ToString();
    }

    /// <summary>
    /// 重置UI
    /// </summary>
    public void ResetUI()
    {
        m_StartUI.SetActive(true);
        m_GameUI.SetActive(false);
    }

    /// <summary>
    /// 暂停
    /// </summary>
    private void PauseButtonClick(GameObject go)
    {
        Debug.Log("暂停");
        Time.timeScale = 0;
        m_PauseButton.SetActive(false);
        m_ContinueButton.SetActive(true);
    }

    /// <summary>
    /// 继续
    /// </summary>
    /// <param name="go"></param>
    private void ContinueButtonClick(GameObject go)
    {
        Debug.Log("继续");
        Time.timeScale = 1;
        m_PauseButton.SetActive(true);
        m_ContinueButton.SetActive(false);
    }

    private void Left(GameObject go)
    {
        m_Player.Left();
    }

    private void Right(GameObject go)
    {
        m_Player.Right();
    }
}
