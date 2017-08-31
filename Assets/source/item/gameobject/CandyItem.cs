using UnityEngine;
using System.Collections;

public class CandyItem : ItemBase {

	public int m_count = 1; // check if we can make it private

	// Use this for initialization
	protected override void Start () {
        base.Start();
        m_type = ItemManager.ITEM_TYPE.ITEM_CANDY;
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
    }

	protected override void OnTriggerEnter2D(Collider2D col) {
        base.OnTriggerEnter2D(col);
	}
}
