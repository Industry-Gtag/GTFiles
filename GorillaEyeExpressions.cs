using System;
using UnityEngine;

// Token: 0x0200086E RID: 2158
public class GorillaEyeExpressions : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060037DF RID: 14303 RVA: 0x00131F39 File Offset: 0x00130139
	private void Awake()
	{
		this.loudness = base.GetComponent<ISpeakerLoudness>();
	}

	// Token: 0x060037E0 RID: 14304 RVA: 0x00131F47 File Offset: 0x00130147
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.timeLastUpdated = Time.time;
		this.deltaTime = Time.deltaTime;
	}

	// Token: 0x060037E1 RID: 14305 RVA: 0x00012134 File Offset: 0x00010334
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060037E2 RID: 14306 RVA: 0x00131F66 File Offset: 0x00130166
	public void SliceUpdate()
	{
		this.deltaTime = Time.time - this.timeLastUpdated;
		this.timeLastUpdated = Time.time;
		this.CheckEyeEffects();
		this.UpdateEyeExpression();
	}

	// Token: 0x060037E3 RID: 14307 RVA: 0x00131F94 File Offset: 0x00130194
	private void CheckEyeEffects()
	{
		if (this.loudness == null)
		{
			this.loudness = base.GetComponent<ISpeakerLoudness>();
		}
		if (this.loudness.IsSpeaking && this.loudness.Loudness > this.screamVolume)
		{
			this.overrideDuration = this.screamDuration;
			this.overrideUV = this.ScreamUV;
			return;
		}
		if (this.overrideDuration > 0f)
		{
			this.overrideDuration -= this.deltaTime;
			if (this.overrideDuration <= 0f)
			{
				this.overrideUV = this.BaseUV;
			}
		}
	}

	// Token: 0x060037E4 RID: 14308 RVA: 0x00132028 File Offset: 0x00130228
	private void UpdateEyeExpression()
	{
		this.targetFace.GetComponent<Renderer>().material.SetVector(this._BaseMap_ST, new Vector4(0.5f, 1f, this.overrideUV.x, this.overrideUV.y));
	}

	// Token: 0x04004818 RID: 18456
	public GameObject targetFace;

	// Token: 0x04004819 RID: 18457
	[Space]
	[SerializeField]
	private float screamVolume = 0.2f;

	// Token: 0x0400481A RID: 18458
	[SerializeField]
	private float screamDuration = 0.5f;

	// Token: 0x0400481B RID: 18459
	[SerializeField]
	private Vector2 ScreamUV = new Vector2(0.8f, 0f);

	// Token: 0x0400481C RID: 18460
	private Vector2 BaseUV = Vector3.zero;

	// Token: 0x0400481D RID: 18461
	private ISpeakerLoudness loudness;

	// Token: 0x0400481E RID: 18462
	private float overrideDuration;

	// Token: 0x0400481F RID: 18463
	private Vector2 overrideUV;

	// Token: 0x04004820 RID: 18464
	private float timeLastUpdated;

	// Token: 0x04004821 RID: 18465
	private float deltaTime;

	// Token: 0x04004822 RID: 18466
	private ShaderHashId _BaseMap_ST = "_BaseMap_ST";
}
