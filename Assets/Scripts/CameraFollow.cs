using UnityEngine;
using System.Collections;

/// <summary>
/// 摄像机跟随角色移动
/// </summary>
public class CameraFollow : MonoBehaviour {


    private Transform m_Transform;
    private Transform player_Transform;

    private bool startFollow = false;

    private Vector3 normalPos;  //初始位置

	// Use this for initialization
	void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        normalPos = m_Transform.position;
        player_Transform = GameObject.FindWithTag("Player").GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        CameraMove();
	}

    /// <summary>
    /// 摄像机移动
    /// </summary>
    void CameraMove()
    {
        if (startFollow == true)
        {
            //y轴上有偏移量
            Vector3 nextPos = new Vector3(m_Transform.position.x, player_Transform.position.y + 1.5f,
                player_Transform.position.z);
            m_Transform.position = Vector3.Lerp(m_Transform.position, nextPos, Time.deltaTime);
        }
    }

    /// <summary>
    /// 开启摄像机跟随
    /// </summary>
    /// <param name="followStatus"></param>
    public void SetCameraFollow(bool followStatus)
    {
        startFollow = followStatus;
    }

    /// <summary>
    /// 重置摄像机
    /// </summary>
    public void ResetCamera()
    {
        m_Transform.position = normalPos;
    }
}
