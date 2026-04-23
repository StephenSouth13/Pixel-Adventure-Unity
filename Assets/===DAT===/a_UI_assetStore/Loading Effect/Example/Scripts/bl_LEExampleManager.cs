using UnityEngine;
using System.Collections.Generic;

public class bl_LEExampleManager : MonoBehaviour {

    public List<GameObject> AllPrefabs = new List<GameObject>();
    public Transform Panel;
    public GameObject CurrentObj;

    private int current = 0;

    public void Change(bool forward)
    {
        if(CurrentObj != null)
        {
            Destroy(CurrentObj);
            CurrentObj = null;
        }

        if (forward)
        {
            if (current < AllPrefabs.Count - 1)
            {
                current++;
            }
            else { current = 0; }
        }
        else
        {
            if(current > 0)
            {
                current--;
            }
            else
            {
                current = AllPrefabs.Count - 1;
            }
        }

        CurrentObj = Instantiate(AllPrefabs[current]) as GameObject;
        CurrentObj.transform.SetParent(Panel, false);
        CurrentObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

}