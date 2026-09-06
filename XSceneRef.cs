using System;
using UnityEngine;

// Token: 0x020003C6 RID: 966
[Serializable]
public struct XSceneRef
{
	// Token: 0x06001734 RID: 5940 RVA: 0x00086798 File Offset: 0x00084998
	public bool TryResolve(out XSceneRefTarget result)
	{
		if (this.TargetID == 0)
		{
			result = null;
			return true;
		}
		if (this.didCache && this.cached != null)
		{
			result = this.cached;
			return true;
		}
		XSceneRefTarget xsceneRefTarget;
		if (!XSceneRefGlobalHub.TryResolve(this.TargetScene, this.TargetID, out xsceneRefTarget))
		{
			result = null;
			return false;
		}
		this.cached = xsceneRefTarget;
		this.didCache = true;
		result = xsceneRefTarget;
		return true;
	}

	// Token: 0x06001735 RID: 5941 RVA: 0x00086800 File Offset: 0x00084A00
	public bool TryResolve(out GameObject result)
	{
		XSceneRefTarget xsceneRefTarget;
		if (this.TryResolve(out xsceneRefTarget))
		{
			result = ((xsceneRefTarget == null) ? null : xsceneRefTarget.gameObject);
			return true;
		}
		result = null;
		return false;
	}

	// Token: 0x06001736 RID: 5942 RVA: 0x00086834 File Offset: 0x00084A34
	public bool TryResolve<T>(out T result) where T : Component
	{
		XSceneRefTarget xsceneRefTarget;
		if (this.TryResolve(out xsceneRefTarget))
		{
			result = ((xsceneRefTarget == null) ? default(T) : xsceneRefTarget.GetComponent<T>());
			return true;
		}
		result = default(T);
		return false;
	}

	// Token: 0x06001737 RID: 5943 RVA: 0x00086875 File Offset: 0x00084A75
	public void AddCallbackOnLoad(Action callback)
	{
		this.TargetScene.AddCallbackOnSceneLoad(callback);
	}

	// Token: 0x06001738 RID: 5944 RVA: 0x00086883 File Offset: 0x00084A83
	public void RemoveCallbackOnLoad(Action callback)
	{
		this.TargetScene.RemoveCallbackOnSceneLoad(callback);
	}

	// Token: 0x06001739 RID: 5945 RVA: 0x00086891 File Offset: 0x00084A91
	public void AddCallbackOnUnload(Action callback)
	{
		this.TargetScene.AddCallbackOnSceneUnload(callback);
	}

	// Token: 0x0600173A RID: 5946 RVA: 0x0008689F File Offset: 0x00084A9F
	public void RemoveCallbackOnUnload(Action callback)
	{
		this.TargetScene.RemoveCallbackOnSceneUnload(callback);
	}

	// Token: 0x04002277 RID: 8823
	public SceneIndex TargetScene;

	// Token: 0x04002278 RID: 8824
	public int TargetID;

	// Token: 0x04002279 RID: 8825
	private XSceneRefTarget cached;

	// Token: 0x0400227A RID: 8826
	private bool didCache;
}
