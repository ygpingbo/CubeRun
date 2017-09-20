using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    private Transform m_Transform;
    private MapManager m_MapManager;
    private UIManager m_UIManager;

    //蜗牛痕迹
    private Color ColorOne = new Color(122 / 255f, 85 / 255f, 179 / 255f);
    private Color ColorTwo = new Color(126 / 255f, 93 / 255f, 183 / 255f);

    //角色坐标
    private int z = 3;
    private int x = 2;
    private long order = 0;  //地板的偏移量
    private bool getKey = true; //能否接受按键
    private bool life = false;
    private bool isDrop = false;    //是否正在掉落
    private CameraFollow m_CameraFollow;    //摄像机跟随

    //得分
    private int gemCount = 0;   //宝石数
    private int scoreCount = 0; //移动得分

	void Start () {
        gemCount = PlayerPrefs.GetInt("gem", 0);    //从注册表取出键值，默认为0

        m_Transform = gameObject.GetComponent<Transform>();

        m_MapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        m_CameraFollow = GameObject.Find("Camera").GetComponent<CameraFollow>();
        m_UIManager = GameObject.Find("UI Root").GetComponent<UIManager>();
    }

	void Update () {
        if (life)
        {
            PlayerMove();
        }
        IsDown();
	}

    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGame()
    {
        life = true;
        m_CameraFollow.SetCameraFollow(true);
        m_MapManager.StartTileDown();
        SetPlayerPos();
    }

    /// <summary>
    /// 判断角色是否掉落
    /// </summary>
    private void IsDown()
    {
        if (z < m_MapManager.GetIndex() && isDrop == false) //防止重复添加刚体组件
        {
            isDrop = true;
            m_MapManager.StopTileDown();
            gameObject.AddComponent<Rigidbody>();
            life = false;
            Invoke("GameOver", 0.5f);
        }
    }

    /// <summary>
    /// 设置角色位置，蜗牛痕迹，踩到空洞
    /// </summary>
    private void SetPlayerPos()
    {
        Transform playerPos = m_MapManager.mapList[z][x].GetComponent<Transform>();
        m_Transform.position = playerPos.position + new Vector3(0, 0.254f / 2, 0);
        m_Transform.rotation = Quaternion.Euler(new Vector3(0, 45, 0));
        MeshRenderer floor = null; 
        //蜗牛痕迹（普通地面和陷阱）
        if (m_MapManager.mapList[z][x].tag == "floor")
        {
            floor =  playerPos.FindChild("normal_a2").GetComponent<MeshRenderer>();
            SetColor(floor);
        }
        if (m_MapManager.mapList[z][x].tag == "Spikes")
        {
            floor = playerPos.FindChild("moving_spikes_a2").GetComponent<MeshRenderer>();
            SetColor(floor);
        }
        if (m_MapManager.mapList[z][x].tag == "Sky Spikes")
        {
            floor = playerPos.FindChild("smashing_spikes_a2").GetComponent<MeshRenderer>();
            SetColor(floor);
        }

        //踩到空洞
        if (floor == null && isDrop == false)
        {
            isDrop = true;
            m_MapManager.StopTileDown();
            gameObject.AddComponent<Rigidbody>();
            life = false;
            Invoke("GameOver", 0.5f);
        }

        //动态生成地图
        CalcPosition();
    }

    /// <summary>
    /// 蜗牛痕迹上色
    /// </summary>
    private void SetColor(MeshRenderer floor)
    {
        if (z % 2 == 0)
        {
            floor.material.color = ColorOne;
        }
        else
        {
            floor.material.color = ColorTwo;
        }
    }

    /// <summary>
    /// 角色操作
    /// </summary>
    private void PlayerMove()
    {
                //left
        if (Input.GetKey(KeyCode.A) && getKey == true)
        {
            Left();
        }
        //right
        if (Input.GetKey(KeyCode.D) && getKey == true)
        {
            Right();
        }
    }

    public void Left()
    {
        if (x != 0)
        {
            getKey = false;
            z++;
            if (z % 2 == 1)
            {
                x--;
            }
            SetPlayerPos();
            AddScoreCount();
            Invoke("OpenKeyDown", 0.12f);    //按键间隔
        }//边界判断
    }

    public void Right()
    {
        if (x != 4 || z % 2 == 0)
        {
            getKey = false;
            z++;
            if (z % 2 == 0)
            {
                x++;
            }
            SetPlayerPos();
            AddScoreCount();
            Invoke("OpenKeyDown", 0.12f);    //按键间隔
        } //边界判断   
    }

    /// <summary>
    /// 获取按键的间隔开关
    /// </summary>
    private void OpenKeyDown()
    {
        getKey = true;
    }

    /// <summary>
    /// 动态生成地图
    /// </summary>
    private void CalcPosition()
    {
        //地图的行数-向前移动数
        if (m_MapManager.mapList.Count - z <= 15)
        {
            order++;
            m_MapManager.CreateMapItem(order);
            m_MapManager.AddPR();
        }
    }

    /// <summary>
    /// 眩晕
    /// </summary>
    private void Dizzy()
    {
        life = false;
        Invoke("Recover", 1.5f);
    }

    private void Recover()
    {
        life = true;
    }

    /// <summary>
    /// 触发器
    /// </summary>
    /// <param name="coll"></param>
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Spikes Attack")
        {
            Dizzy();
        }
        if (coll.tag == "Sky Spikes Attack")
        {
            Dizzy();
        }
        if (coll.tag == "Gem")
        {
            GameObject.Destroy(coll.GetComponent<Transform>().parent.gameObject);
            AddGemCount();
        }
    }

    /// <summary>
    /// 游戏状态
    /// </summary>
    /// <returns>true:run,false:end</returns>
    public bool GameStatus()
    {
        return life;
    }

    /// <summary>
    /// 宝石数累加
    /// </summary>
    private void AddGemCount()
    {
        gemCount++;
        m_UIManager.UpdateData(scoreCount, gemCount);   //更新UI
    }

    /// <summary>
    /// 移动得分累加
    /// </summary>
    private void AddScoreCount()
    {
        scoreCount++;
        m_UIManager.UpdateData(scoreCount, gemCount);   //更新UI
    }

    /// <summary>
    /// 存储数据
    /// </summary>
    private void SaveData()
    {
        PlayerPrefs.SetInt("gem", gemCount);
        if (scoreCount > PlayerPrefs.GetInt("score", 0))
        {
            PlayerPrefs.SetInt("score", scoreCount);
        }
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    private void GameOver()
    {
        life = false;
        m_CameraFollow.SetCameraFollow(false);  //摄像机停止跟随
        SaveData();
        //UI
        StartCoroutine("ResetGame");
    }

    private void ResetPlayer()
    {
        //重置位置
        z = 3;
        x = 2;
        //移除刚体
        GameObject.Destroy(gameObject.GetComponent<Rigidbody>()); 
        //重置分数
        scoreCount = 0;
        m_UIManager.UpdateData(scoreCount, gemCount);
        //重置地图偏移
        order = 0;
        //重置掉落状态
        isDrop = false;
    }

    private IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(2.0f);
        ResetPlayer();
        m_UIManager.ResetUI();
        m_MapManager.ResetGameMap();
        m_CameraFollow.ResetCamera();
    }
}
