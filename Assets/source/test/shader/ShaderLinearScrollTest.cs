using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderLinearScrollTest : MonoBehaviour {
    public float m_speedX = 0.0f;
    public float m_speedY = 0.0f;
    float offsetX = 0.0f;
    float offsetY = 0.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        offsetX += Time.deltaTime * m_speedX;
        if (offsetX > 1.0f)
            offsetX = 0.0f;
        else if (offsetX < 0.0f)
            offsetX = 1.0f;

        offsetY += Time.deltaTime * m_speedY;
        if (offsetY > 1.0f)
            offsetY = 0.0f;
        else if (offsetY < 0.0f)
            offsetY = 1.0f;

        GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
	}
}
