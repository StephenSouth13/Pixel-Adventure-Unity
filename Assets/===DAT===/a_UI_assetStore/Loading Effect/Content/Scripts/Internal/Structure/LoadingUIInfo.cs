using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class LoadingUIInfo  {

    public RectTransform UI;
    public LoadingEffectType m_Type = LoadingEffectType.Rotate;
    [Range(1,100)]public float Speed = 10;
    public Vector3 Axis = -Vector3.forward;

    public bool PingPong = false;
    public AnimationCurve Curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1,1) });
    public float Delay = 0;

    [HideInInspector]public float Value;
    [HideInInspector]public bool Forward = true;
    [HideInInspector]public float NextDelay;

    private Image m_image = null;
    public Image Image
    {
        get
        {
            if(m_image == null)
            {
                m_image = UI.GetComponent<Image>();
            }
            return m_image;
        }
    }

    public void Init()
    {
        NextDelay = Time.time + Delay;
        if(m_Type == LoadingEffectType.Filled)
        {
            Value = Image.fillAmount;
        }
    }

}