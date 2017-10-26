using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimDictionary : DictionaryTemplate<AnimationManager.AnimType, GameObject> { }

public class AnimationManager : MonoBehaviour {

    public enum AnimType {
        ANIM_BLAST,
        ANIM_HIT,
        ANIM_DEATH,
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
            case AnimType.ANIM_DEATH:
                {
                    BlastAnimData data = (BlastAnimData)animData;
                    GameObject animObj = (GameObject)Instantiate(getAnimPrefab(type), data.m_position, Quaternion.identity);
                    BlastAnim animation = animObj.GetComponent<BlastAnim>();
                    animation.m_targetScale = data.m_targetScale;
                    animation.m_duration = data.m_duration;
                }
                break;
            case AnimType.ANIM_HIT:
                {
                    GameObject animObj = (GameObject)Instantiate(getAnimPrefab(type), animData.m_position, Quaternion.identity);
                    HitAnim animation = animObj.GetComponent<HitAnim>();
                    animation.m_duration = animData.m_duration;
                }
                break;
        }
    }
}
