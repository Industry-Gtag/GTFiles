using System;
using UnityEngine;

// Token: 0x02000865 RID: 2149
public class GorillaCaveCrystal : Tappable
{
	// Token: 0x060037B9 RID: 14265 RVA: 0x00131508 File Offset: 0x0012F708
	private void Awake()
	{
		if (this.tapScript == null)
		{
			this.tapScript = base.GetComponent<TapInnerGlow>();
		}
	}

	// Token: 0x060037BA RID: 14266 RVA: 0x00131524 File Offset: 0x0012F724
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		this._tapStrength = tapStrength;
		this.AnimateCrystal();
	}

	// Token: 0x060037BB RID: 14267 RVA: 0x00131533 File Offset: 0x0012F733
	private void AnimateCrystal()
	{
		if (this.tapScript)
		{
			this.tapScript.Tap();
		}
	}

	// Token: 0x040047DD RID: 18397
	public bool overrideSoundAndMaterial;

	// Token: 0x040047DE RID: 18398
	public CrystalOctave octave;

	// Token: 0x040047DF RID: 18399
	public CrystalNote note;

	// Token: 0x040047E0 RID: 18400
	[SerializeField]
	private MeshRenderer _crystalRenderer;

	// Token: 0x040047E1 RID: 18401
	public TapInnerGlow tapScript;

	// Token: 0x040047E2 RID: 18402
	[HideInInspector]
	public GorillaCaveCrystalVisuals visuals;

	// Token: 0x040047E3 RID: 18403
	[HideInInspector]
	[SerializeField]
	private AnimationCurve _lerpInCurve = AnimationCurve.Constant(0f, 1f, 1f);

	// Token: 0x040047E4 RID: 18404
	[HideInInspector]
	[SerializeField]
	private AnimationCurve _lerpOutCurve = AnimationCurve.Constant(0f, 1f, 1f);

	// Token: 0x040047E5 RID: 18405
	[HideInInspector]
	[SerializeField]
	private bool _animating;

	// Token: 0x040047E6 RID: 18406
	[HideInInspector]
	[SerializeField]
	[Range(0f, 1f)]
	private float _tapStrength = 1f;

	// Token: 0x040047E7 RID: 18407
	[NonSerialized]
	private TimeSince _timeSinceLastTap;
}
