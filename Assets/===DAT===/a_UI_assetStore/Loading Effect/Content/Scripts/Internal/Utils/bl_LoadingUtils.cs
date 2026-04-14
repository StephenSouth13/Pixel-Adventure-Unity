using UnityEngine;

public static class bl_LoadingUtils
{

    public static bl_LoadingEffect GetLoading(int id)
    {
        bl_LoadingEffect[] all = GameObject.FindObjectsOfType<bl_LoadingEffect>();
        foreach(bl_LoadingEffect l in all)
        {
            if(l.ID == id)
            {
                return l;
            }
        }
        Debug.LogWarning(string.Format("Loading with ID: {0} doesn't exist in this scene.", id));
        return null;
    }
}