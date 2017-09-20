using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour {

    private Transform m_Transform;
    private Transform m_gem;
    private PlayerController m_player;

	// Use this for initialization
	void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        m_gem = m_Transform.FindChild("gem 3");
        m_player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (m_player.GameStatus())
        {
            m_gem.Rotate(Vector3.up);
        }
	}
}
