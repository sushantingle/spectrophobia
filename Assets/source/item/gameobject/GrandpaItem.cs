using UnityEngine;
using System.Collections;

public class GrandpaItem : ItemBase {
    
	// Use this for initialization
	protected override void Start () {
        base.Start();
        m_type = ItemManager.ITEM_TYPE.ITEM_D;
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

	protected override void OnTriggerEnter2D(Collider2D col) {
        base.OnTriggerEnter2D(col);
    }
}
