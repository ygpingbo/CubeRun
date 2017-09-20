using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 地图管理器
/// </summary>
public class MapManager : MonoBehaviour {

    //资源
    private GameObject m_prefab_tile;   //地板
    private GameObject m_prefab_wall;   //墙壁
    private GameObject m_prefab_spikes; //地面陷阱
    private GameObject m_prefab_sky_spikes; //天空陷阱
    private GameObject m_prefab_gem;    //宝石

    //概率
    private int pr_floor = 0;
    private int pr_hole = 0;
    private int pr_spikes = 0;
    private int pr_sky_spikes = 0;
    private int pr_gam = 2;

    //列表
    public List<GameObject[]> mapList = new List<GameObject[]>();  //地图存储列表

    //组件
    private Transform m_Transform;

    //常量
    private float floorGap;    //地板间隙
    private int floorList = 15;     //底板行数
    private int index = 0;  //记录地面塌陷层数
   
    //地板颜色
    private Color colorOne = new Color(124 / 255f, 155 / 255f, 230 / 255f);
    private Color colorTwo = new Color(125 / 255f, 169 / 255f, 233 / 255f);
    private Color colorWall = new Color(87 / 255f, 93 / 255f, 169 / 255f); 

	// Use this for initialization
	void Start () {
        //资源动态加载
        m_prefab_tile = Resources.Load("tile_white") as GameObject;
        m_prefab_wall = Resources.Load("wall2") as GameObject;
        m_prefab_spikes = Resources.Load("moving_spikes") as GameObject;
        m_prefab_sky_spikes = Resources.Load("smashing_spikes") as GameObject;
        m_prefab_gem = Resources.Load("gem 2") as GameObject;

        //获取组件
        m_Transform = gameObject.GetComponent<Transform>();

        //间隙计算
        floorGap = Mathf.Sqrt(2) * 0.254f;
        //地图生成
        CreateMapItem(0);
	}
	
	// Update is called once per frame
	void Update () {
        //设置地板名字为坐标号
        PrintTestName();
	}

    /// <summary>
    /// 创建地图元素（段）
    /// </summary>
    public void CreateMapItem(long order)
    {
        //偏移量
        float offsetZ = order * floorGap * floorList;
        for (int i = 0; i < floorList; i++)
        {
            //单排
            GameObject[] item1 = new GameObject[6];
            for (int j = 0; j < 6; j++)
            { 
                Vector3 pos = new Vector3(j * floorGap, 0, i * floorGap + offsetZ);
                Vector3 rot = new Vector3(-90, 45, 0);
                GameObject go = null;
                if (j == 0 || j == 5)
                {
                    go = GameObject.Instantiate(m_prefab_wall, pos, Quaternion.Euler(rot)) as GameObject;//有旋转
                    //颜色
                    go.GetComponent<MeshRenderer>().material.color = colorWall;
                }
                else
                {
                    int pr = CalcPR();
                    switch (pr)
                    {
                        default: break;
                        case 0: //地板
                            go = GameObject.Instantiate(m_prefab_tile, pos, Quaternion.Euler(rot)) as GameObject;//有旋转
                            //颜色
                            go.GetComponent<MeshRenderer>().material.color = colorOne;
                            go.GetComponent<Transform>().FindChild("normal_a2").GetComponent<MeshRenderer>().material.color = colorOne;
                            int gemPr = CalGemPR();
                            //生成宝石
                            if (gemPr == 1 && i >= 5)
                            {
                                GameObject gem = GameObject.Instantiate(m_prefab_gem, go.GetComponent<Transform>().position + new Vector3(0, 0.06f, 0), Quaternion.identity) as GameObject;
                                gem.GetComponent<Transform>().SetParent(go.GetComponent<Transform>());
                            }
                            break;
                        case 1: //坑洞
                            go = new GameObject();
                            go.GetComponent<Transform>().position = pos;
                            go.GetComponent<Transform>().rotation = Quaternion.Euler(rot);
                            break;
                        case 2: //地面陷阱
                            go = GameObject.Instantiate(m_prefab_spikes, pos, Quaternion.Euler(rot)) as GameObject;//有旋转
                            break;
                        case 3: //天空陷阱
                             go = GameObject.Instantiate(m_prefab_sky_spikes, pos, Quaternion.Euler(rot)) as GameObject;//有旋转
                            break;
                    }
                    
                  
                }
                //建立父子关系
                go.GetComponent<Transform>().SetParent(m_Transform);
                item1[j] = go;
            }
            mapList.Add(item1);  //将一排地板（墙壁）存入列表

            //双排
            GameObject[] item2 = new GameObject[5];
            for (int j = 0; j < 5; j++)
            {
                Vector3 pos = new Vector3(j * floorGap, 0, i * floorGap + offsetZ);
                pos += new Vector3(floorGap / 2, 0, floorGap / 2);
                Vector3 rot = new Vector3(-90, 45, 0);
                GameObject go = null;
                int pr = CalcPR();
                switch (pr)
                {
                    default:
                        break;
                    case 0:
                        go = GameObject.Instantiate(m_prefab_tile, pos, Quaternion.Euler(rot)) as GameObject;//有旋转
                        //颜色
                        go.GetComponent<MeshRenderer>().material.color = colorTwo;
                        go.GetComponent<Transform>().FindChild("normal_a2").GetComponent<MeshRenderer>().material.color = colorTwo;
                        int gemPr = CalGemPR();
                        //生成宝石
                        if (gemPr == 1 && i >= 5)
                        {
                            GameObject gem = GameObject.Instantiate(m_prefab_gem, go.GetComponent<Transform>().position + new Vector3(0, 0.06f, 0), Quaternion.identity) as GameObject;
                            gem.GetComponent<Transform>().SetParent(go.GetComponent<Transform>());
                        }
                        break;
                    case 1: //坑洞
                        go = new GameObject();
                        go.GetComponent<Transform>().position = pos;
                        go.GetComponent<Transform>().rotation = Quaternion.Euler(rot);
                        break;
                    case 2: //地面陷阱
                        go = GameObject.Instantiate(m_prefab_spikes, pos, Quaternion.Euler(rot)) as GameObject;//有旋转
                        break;
                    case 3: //天空陷阱
                        go = GameObject.Instantiate(m_prefab_sky_spikes, pos, Quaternion.Euler(rot)) as GameObject;//有旋转
                        break;
                }
                //建立父子关系
                go.GetComponent<Transform>().SetParent(m_Transform);
                item2[j] = go;
            }
            mapList.Add(item2);  //将一排地板（墙壁）存入列表
        }
    }

    /// <summary>
    /// 用于测试，设置地板名字为坐标号!!!!!!!!!!!按空格启用
    /// </summary>
    private void PrintTestName()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < mapList.Count; i++)
            {
                for (int j = 0; j < mapList[i].Length; j++)
                {
                    mapList[i][j].name = "(" + i + ", " + j + ")";
                }
            }
        }
    }

    /// <summary>
    /// 开启地面塌陷协成
    /// </summary>
    public void StartTileDown()
    {
        StartCoroutine("TileDown");
    }

    /// <summary>
    /// 关闭地面塌陷协成
    /// </summary>
    public void StopTileDown()
    {
        StopCoroutine("TileDown");
    }

    /// <summary>
    /// 地面塌陷
    /// </summary>
    /// <returns></returns>
    private IEnumerator TileDown()
    {
        yield return new WaitForSeconds(0.8f);
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < mapList[index].Length; i++)
            {
                Rigidbody rb = mapList[index][i].AddComponent<Rigidbody>();
                rb.angularVelocity = new Vector3(Random.Range(0.0f, 1.0f),
                    Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)) * Random.Range(1, 10);
                GameObject.Destroy(mapList[index][i], 1.0f);
            }
            index++;
        }
    }

    /// <summary>
    /// 获取掉落层数
    /// </summary>
    /// <returns></returns>
    public int GetIndex()
    {
        return index;
    }

    /// <summary>
    /// 计算概率
    /// 0：瓷砖
    /// 1：坑洞
    /// 2：地面陷阱
    /// 3：天空陷阱
    /// </summary>
    /// <returns></returns>
    private int CalcPR()
    {
        int pr = Random.Range(1, 100 + pr_floor);

        if (pr <= pr_hole)
        {
            return 1;
        }
        else if (31 < pr && pr < pr_spikes + 30)
        {
            return 2;
        }
        else if (61 < pr && pr < pr_sky_spikes + 60)
        {
            return 3;
        }

        return 0;
    }

    /// <summary>
    /// 计算宝石生成概率
    /// </summary>
    /// <returns>0：不生成，1：生成</returns>
    private int CalGemPR()
    {
        int pr = Random.Range(1, 100);

        if (pr <= pr_gam)
        {
            return 1;
        }
        return 0;
    }

    /// <summary>
    /// 增加概率值
    /// </summary>
    public void AddPR()
    {
        if (pr_hole >= 10)
        {
            pr_floor += 2;
        }
        if (pr_hole < 30)
        {
            pr_hole += 2;
        }
        if (pr_spikes < 30 && pr_hole >= 10)
        {
            pr_spikes += 2;
        }
        if (pr_sky_spikes < 30 && pr_hole >= 10)
        {
            pr_sky_spikes += 2;
        }
    }

    /// <summary>
    /// 重置地图
    /// </summary>
    public void ResetGameMap()
    {
        //销毁地图
        Transform[] sonTransform = m_Transform.GetComponentsInChildren<Transform>();
        for (int i = 1; i < sonTransform.Length; i++)
        {
            GameObject.Destroy(sonTransform[i].gameObject);
        }

        //重置概率
        pr_hole = 0;
        pr_spikes = 0;
        pr_sky_spikes = 0;

        //重置塌陷层数
        index = 0;

        //重置地图列表
        mapList.Clear();
        CreateMapItem(0);
    }
}
