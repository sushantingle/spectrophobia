using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleItemData : BaseItemData {

    public void init(ItemManager.ITEM_TYPE _type, float _startTime, float _duration)
    {
        base.init(_type, _duration);
    }

    public override void update()
    {
        base.update();
    }
}
