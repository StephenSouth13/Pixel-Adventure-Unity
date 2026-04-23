using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class bl_LoadingEffect : MonoBehaviour {

    [Header("Settings")]
    [SerializeField]public int ID;//Unique id of loading component for get reference for another script easy.
    [SerializeField]private bool Loading = false;//if loading? for get the public value use isLoading instead.
    public float FadeSpeed = 4;
    [Header("References")]
    [SerializeField]private LoadingUIInfo[] LoadingUI;//all UI component.

    [HideInInspector]public bool ShowList = true;//use for editor only
    private CanvasGroup m_CanvasGroup;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        //Get canvas group or add it.
        if (GetComponent<CanvasGroup>() != null)
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
        }
        else
        {
            m_CanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        //if not loading set alpha to 0 for avoid render on start
        m_CanvasGroup.alpha = (Loading) ? 1 : 0;
    }

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        //setup all loadings components in start
        for (int i = 0; i < LoadingUI.Length; i++)
        {
            LoadingUI[i].Init();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (LoadingUI == null || LoadingUI.Length <= 0)
            return;

        if (Loading)
        {
            OnLoading();
        }
        else
        {
            OnUnLoading();
        }
    }

    /// <summary>
    /// When is loading
    /// </summary>
    void OnLoading()
    {
        for (int i = 0; i < LoadingUI.Length; i++)
        {
            if (LoadingUI[i].UI != null)
            {
                //Delay 
                if (LoadingUI[i].NextDelay > Time.time)
                {
                    return;
                }
                //for rotate loading effect
                if (LoadingUI[i].m_Type == LoadingEffectType.Rotate)
                {
                    //rotate the transform of UI in the desired axis.
                    LoadingUI[i].UI.Rotate(((LoadingUI[i].Axis * (LoadingUI[i].Speed * 10)) * Time.deltaTime), Space.World);
                }//for filled image loading effect
                else if (LoadingUI[i].m_Type == LoadingEffectType.Filled)
                {
                    //pingpong for forward and reverse
                    if (LoadingUI[i].PingPong)// If PingPong
                    {
                        if (LoadingUI[i].Forward)
                        {
                            LoadingUI[i].Value += Time.deltaTime * (LoadingUI[i].Speed / 10);
                            LoadingUI[i].Image.fillAmount = LoadingUI[i].Curve.Evaluate(LoadingUI[i].Value);
                            if (LoadingUI[i].Image.fillAmount >= 1)
                            {
                                LoadingUI[i].Forward = !LoadingUI[i].Forward;
                            }
                        }
                        else
                        {
                            LoadingUI[i].Value -= Time.deltaTime * (LoadingUI[i].Speed / 10);
                            LoadingUI[i].Image.fillAmount = LoadingUI[i].Curve.Evaluate(LoadingUI[i].Value);
                            if (LoadingUI[i].Image.fillAmount <= 0)
                            {
                                LoadingUI[i].Forward = !LoadingUI[i].Forward;
                            }
                        }
                    }
                    else //If not pingpong restart filled amount and change the direction
                    {
                        if (LoadingUI[i].Forward)
                        {
                            LoadingUI[i].Value += Time.deltaTime * (LoadingUI[i].Speed / 10);
                            LoadingUI[i].Image.fillAmount = LoadingUI[i].Curve.Evaluate(LoadingUI[i].Value);
                            if (LoadingUI[i].Image.fillAmount >= 1)
                            {
                                LoadingUI[i].Forward = !LoadingUI[i].Forward;
                                LoadingUI[i].Image.fillClockwise = !LoadingUI[i].Image.fillClockwise;
                                if (LoadingUI[i].Image.fillMethod == Image.FillMethod.Horizontal || LoadingUI[i].Image.fillMethod == Image.FillMethod.Vertical)
                                {
                                    if (LoadingUI[i].Image.fillOrigin == (int)Image.OriginHorizontal.Left) { LoadingUI[i].Image.fillOrigin = (int)Image.OriginHorizontal.Right; }
                                    else
                                    {
                                        LoadingUI[i].Image.fillOrigin = (int)Image.OriginHorizontal.Left;
                                    }
                                }
                            }
                        }
                        else
                        {
                            LoadingUI[i].Value -= Time.deltaTime * (LoadingUI[i].Speed / 10);
                            LoadingUI[i].Image.fillAmount = LoadingUI[i].Curve.Evaluate(LoadingUI[i].Value);
                            if (LoadingUI[i].Image.fillAmount <= 0)
                            {
                                LoadingUI[i].Forward = !LoadingUI[i].Forward;
                                LoadingUI[i].Image.fillClockwise = !LoadingUI[i].Image.fillClockwise;
                                if (LoadingUI[i].Image.fillMethod == Image.FillMethod.Horizontal || LoadingUI[i].Image.fillMethod == Image.FillMethod.Vertical)
                                {
                                    if (LoadingUI[i].Image.fillOrigin == (int)Image.OriginHorizontal.Left) { LoadingUI[i].Image.fillOrigin = (int)Image.OriginHorizontal.Right; }
                                    else
                                    {
                                        LoadingUI[i].Image.fillOrigin = (int)Image.OriginHorizontal.Left;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //set alpha to 1
        if (m_CanvasGroup.alpha < 1)
        {
            m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, 1, Time.deltaTime * FadeSpeed);
        }
    }

    /// <summary>
    /// When is not loading
    /// </summary>
    void OnUnLoading()
    {
        //set alpha to 0
        if (m_CanvasGroup.alpha > 0)
        {
            m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, 0, Time.deltaTime * FadeSpeed);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool isLoading
    {
        get
        {
            return Loading;
        }
        set
        {
            Loading = value;
        }
    }
}