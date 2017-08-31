using UnityEngine;
using System.Collections;

public class EnemySpawn : MonoBehaviour {

	private GameObject m_enemyObj;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setEnemyObject(GameObject obj) {
		m_enemyObj = obj;
	}

	public void onAnimationComplete() {
		m_enemyObj.SetActive (true);
		Destroy (gameObject);
	}
}
