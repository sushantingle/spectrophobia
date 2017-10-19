using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimDictionary : DictionaryTemplate<AnimationManager.AnimType, GameObject> { }

public class AnimationManager : MonoBehaviour {

    public enum AnimType {
        ANIM_BLAST,
    }

    private static AnimationManager m_instance = null;
    public List<AnimDictionary> m_animList;

    public static AnimationManager getInstance()
    {
        return m_instance;
    }

    private void Awake()
    {
        m_instance = this;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private GameObject getAnimPrefab(AnimType type)
    {
        var obj = m_animList.Find(anim => anim._key == type);
        if (obj != null)
            return obj._value;
        return null;
    }


    public void startAnim(AnimType type, AnimData animData)
    {
        switch(type)
        {
            case AnimType.ANIM_BLAST:
                {
                    BlastAnimData data = (BlastAnimData)animData;
                    GameObject animObj = (GameObject)Instantiate(getAnimPrefab(type), data.m_position, Quaternion.identity);
                    BlastAnim animation = animObj.GetComponent<BlastAnim>();
                    animation.m_targetScale = data.m_targetScale;
                    animation.m_speed = data.m_duration;
                }
                break;
        }
    }
}
