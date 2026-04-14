using UnityEngine;

public class bl_LEExampleUsage : MonoBehaviour {

    public int ID;

    /// <summary>
    /// When we want show loading effect
    /// </summary>
	public void Show()
    {
        bl_LoadingUtils.GetLoading(ID).isLoading = true;
    }

    /// <summary>
    /// When we want hide loading effect
    /// </summary>
    public void Hide()
    {
        bl_LoadingUtils.GetLoading(ID).isLoading = false;
    }
}