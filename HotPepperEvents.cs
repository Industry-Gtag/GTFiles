using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000594 RID: 1428
public class HotPepperEvents : MonoBehaviour
{
	// Token: 0x0600242D RID: 9261 RVA: 0x000C2716 File Offset: 0x000C0916
	private void OnEnable()
	{
		this._pepper.onBiteWorld.AddListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._pepper.onBiteView.AddListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x0600242E RID: 9262 RVA: 0x000C2750 File Offset: 0x000C0950
	private void OnDisable()
	{
		this._pepper.onBiteWorld.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._pepper.onBiteView.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x0600242F RID: 9263 RVA: 0x000C278A File Offset: 0x000C098A
	public void OnBiteView(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, true);
	}

	// Token: 0x06002430 RID: 9264 RVA: 0x000C2795 File Offset: 0x000C0995
	public void OnBiteWorld(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, false);
	}

	// Token: 0x06002431 RID: 9265 RVA: 0x000C27A0 File Offset: 0x000C09A0
	public void OnBite(VRRig rig, int nextState, bool isViewRig)
	{
		if (nextState != 8)
		{
			return;
		}
		GameObject gameObject = rig.cosmeticReferences.Get(this.m_targetEffectID);
		if (gameObject.IsNull())
		{
			return;
		}
		HotPepperFace component = gameObject.GetComponent<HotPepperFace>();
		if (component.IsNull())
		{
			return;
		}
		component.PlayFX(1f);
	}

	// Token: 0x04002F6D RID: 12141
	[SerializeField]
	private EdibleHoldable _pepper;

	// Token: 0x04002F6E RID: 12142
	[SerializeField]
	private CosmeticRefID m_targetEffectID = CosmeticRefID.HotPepperFaceEffect;

	// Token: 0x02000595 RID: 1429
	public enum EdibleState
	{
		// Token: 0x04002F70 RID: 12144
		A = 1,
		// Token: 0x04002F71 RID: 12145
		B,
		// Token: 0x04002F72 RID: 12146
		C = 4,
		// Token: 0x04002F73 RID: 12147
		D = 8
	}
}
