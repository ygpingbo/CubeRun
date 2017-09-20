using UnityEngine;
using System.Collections;

/// <summary>
/// 地面陷阱
/// </summary>
public class Spikes : MonoBehaviour {

    private Transform m_Transform;
    private Transform son_Transform;

    private Vector3 normalPos;  //原始点
    private Vector3 targetPos;  //目标点

	// Use this for initialization
	void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        son_Transform = m_Transform.FindChild("moving_spikes_b").GetComponent<Transform>();

        normalPos = son_Transform.position;
        targetPos = son_Transform.position + new Vector3(0, 0.15f, 0);

        //钉子开始运动
        StartCoroutine("UpAndDown");
	}

    /// <summary>
    /// 钉子向上运动
    /// </summary>
    /// <returns></returns>
    private IEnumerator Up()
    {
        while (true)
        {
            son_Transform.position = Vector3.Lerp(son_Transform.position, targetPos, Time.deltaTime * 30);
            yield return null;
        }
    }

    /// <summary>
    /// 钉子向下运动
    /// </summary>
    /// <returns></returns>
    private IEnumerator Down()
    {
        while (true)
        {
            son_Transform.position = Vector3.Lerp(son_Transform.position, normalPos, Time.deltaTime * 15);
            yield return null;
        }
    }

    /// <summary>
    /// 钉子开启运动
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpAndDown()
    {
        while (true)
        {
            StopCoroutine("Down");
            StartCoroutine("Up");
            yield return new WaitForSeconds(2.0f);
            StopCoroutine("Up");
            StartCoroutine("Down");
            yield return new WaitForSeconds(2.0f);
        }
    }
}
