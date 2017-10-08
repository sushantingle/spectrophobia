using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCBulletMMaterialDictionary : DictionaryTemplate<CardDataBase.NPC_TYPE, Material> { }

[CreateAssetMenu(fileName = "Bullet Material", menuName = "BulletMaterial")]
public class BulletColorScriptablePObj : ScriptableObject {

    public List<NPCBulletMMaterialDictionary> m_bulletMaterials;

    public Material getMaterialOf(CardDataBase.NPC_TYPE type)
    {
        var obj = m_bulletMaterials.Find(item => item._key == type);
        if (obj != null)
            return obj._value;
        return null;
    }
}
