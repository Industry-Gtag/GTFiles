using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200058C RID: 1420
public class BubbleGumEvents : MonoBehaviour
{
	// Token: 0x06002407 RID: 9223 RVA: 0x000C202D File Offset: 0x000C022D
	private void OnEnable()
	{
		this._edible.onBiteWorld.AddListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._edible.onBiteView.AddListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x06002408 RID: 9224 RVA: 0x000C2067 File Offset: 0x000C0267
	private void OnDisable()
	{
		this._edible.onBiteWorld.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._edible.onBiteView.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x06002409 RID: 9225 RVA: 0x000C20A1 File Offset: 0x000C02A1
	public void OnBiteView(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, true);
	}

	// Token: 0x0600240A RID: 9226 RVA: 0x000C20AC File Offset: 0x000C02AC
	public void OnBiteWorld(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, false);
	}

	// Token: 0x0600240B RID: 9227 RVA: 0x000C20B8 File Offset: 0x000C02B8
	public void OnBite(VRRig rig, int nextState, bool isViewRig)
	{
		GorillaTagger instance = GorillaTagger.Instance;
		GameObject gameObject = null;
		if (isViewRig && instance != null)
		{
			gameObject = instance.gameObject;
		}
		else if (!isViewRig)
		{
			gameObject = rig.gameObject;
		}
		if (!BubbleGumEvents.gTargetCache.TryGetValue(gameObject, out this._bubble))
		{
			this._bubble = gameObject.GetComponentsInChildren<GumBubble>(true).FirstOrDefault((GumBubble g) => g.transform.parent.name == "$gum");
			if (isViewRig)
			{
				this._bubble.audioSource = instance.offlineVRRig.tagSound;
				this._bubble.targetScale = Vector3.one * 1.36f;
			}
			else
			{
				this._bubble.audioSource = rig.tagSound;
				this._bubble.targetScale = Vector3.one * 2f;
			}
			BubbleGumEvents.gTargetCache.Add(gameObject, this._bubble);
		}
		GumBubble bubble = this._bubble;
		if (bubble != null)
		{
			bubble.transform.parent.gameObject.SetActive(true);
		}
		GumBubble bubble2 = this._bubble;
		if (bubble2 == null)
		{
			return;
		}
		bubble2.InflateDelayed();
	}

	// Token: 0x04002F4A RID: 12106
	[SerializeField]
	private EdibleHoldable _edible;

	// Token: 0x04002F4B RID: 12107
	[SerializeField]
	private GumBubble _bubble;

	// Token: 0x04002F4C RID: 12108
	private static Dictionary<GameObject, GumBubble> gTargetCache = new Dictionary<GameObject, GumBubble>(16);

	// Token: 0x0200058D RID: 1421
	public enum EdibleState
	{
		// Token: 0x04002F4E RID: 12110
		A = 1,
		// Token: 0x04002F4F RID: 12111
		B,
		// Token: 0x04002F50 RID: 12112
		C = 4,
		// Token: 0x04002F51 RID: 12113
		D = 8
	}
}
