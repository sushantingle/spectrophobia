using UnityEngine;
 using System.Collections;
 
 public class ProgressBar : MonoBehaviour {

    private float m_value = 0;
    private float m_maxValue = 1.0f;

    public RectTransform m_foreground;

    public enum ScrollType {
        SCROLL_VERTICAL_UP_DOWN,
        SCROLL_VERTICAL_DOWN_UP,
        SCROLL_HORIZONTAL_LEFT_RIGHT,
        SCROLL_HORIZONTAL_RIGHT_LEFT,
    }

    public ScrollType m_type = ScrollType.SCROLL_HORIZONTAL_RIGHT_LEFT;

    private void Start()
    {
        switch (m_type)
        {
            case ScrollType.SCROLL_HORIZONTAL_LEFT_RIGHT:
                m_maxValue = m_foreground.localScale.x;
                m_value = 0.0f;
                break;
            case ScrollType.SCROLL_HORIZONTAL_RIGHT_LEFT:
                m_maxValue = m_foreground.localScale.x;
                m_value = m_maxValue;
                break;
            case ScrollType.SCROLL_VERTICAL_DOWN_UP:
                m_maxValue = m_foreground.localScale.y;
                m_value = 0.0f;
                break;
            case ScrollType.SCROLL_VERTICAL_UP_DOWN:
                m_maxValue = m_foreground.localScale.y;
                m_value = m_maxValue;
                break;
        }
    }

    private void Update()
    {
        Vector3 scale = m_foreground.localScale;

        if (m_type == ScrollType.SCROLL_HORIZONTAL_LEFT_RIGHT || m_type == ScrollType.SCROLL_HORIZONTAL_RIGHT_LEFT)
            scale.x = Mathf.Lerp(scale.x, m_value, Time.deltaTime * 10.0f);
        else if(m_type == ScrollType.SCROLL_VERTICAL_DOWN_UP || m_type == ScrollType.SCROLL_VERTICAL_UP_DOWN)
            scale.y = Mathf.Lerp(scale.y, m_value, Time.deltaTime * 10.0f);

        m_foreground.localScale = scale;
    }

    public void setValue(float val)
    {
        m_value = m_maxValue * val;
        CustomDebug.Log("Bar value :" + m_value + "   max value : " + m_maxValue);
    }
}