using System;
using UnityEngine;

// Token: 0x02000896 RID: 2198
public class GorillaMouthFlap : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06003946 RID: 14662 RVA: 0x00138470 File Offset: 0x00136670
	private void Start()
	{
		this.speaker = base.GetComponent<ISpeakerLoudness>();
		this.targetFaceRenderer = this.targetFace.GetComponent<Renderer>();
		this.facePropBlock = new MaterialPropertyBlock();
		this.hasDefaultMouthAtlas = false;
		if (this.targetFaceRenderer != null)
		{
			this.SetDefaultMouthAtlas(this.targetFaceRenderer.material);
		}
	}

	// Token: 0x06003947 RID: 14663 RVA: 0x001384CB File Offset: 0x001366CB
	public void EnableLeafBlower()
	{
		this.leafBlowerActiveUntilTimestamp = Time.time + 0.1f;
	}

	// Token: 0x06003948 RID: 14664 RVA: 0x001384DE File Offset: 0x001366DE
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.lastTimeUpdated = Time.time;
		this.deltaTime = Time.deltaTime;
	}

	// Token: 0x06003949 RID: 14665 RVA: 0x00012134 File Offset: 0x00010334
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600394A RID: 14666 RVA: 0x00138500 File Offset: 0x00136700
	public void SliceUpdate()
	{
		this.deltaTime = Time.time - this.lastTimeUpdated;
		this.lastTimeUpdated = Time.time;
		if (this.speaker == null)
		{
			this.speaker = base.GetComponent<ISpeakerLoudness>();
			return;
		}
		float num = 0f;
		if (this.speaker.IsSpeaking)
		{
			num = this.speaker.Loudness;
		}
		this.CheckMouthflapChange(this.speaker.IsMicEnabled, num);
		MouthFlapLevel mouthFlapLevel = this.noMicFace;
		if (this.leafBlowerActiveUntilTimestamp > Time.time)
		{
			mouthFlapLevel = this.leafBlowerFace;
		}
		else if (this.useMicEnabled)
		{
			mouthFlapLevel = this.mouthFlapLevels[this.activeFlipbookIndex];
		}
		this.UpdateMouthFlapFlipbook(mouthFlapLevel);
	}

	// Token: 0x0600394B RID: 14667 RVA: 0x001385B0 File Offset: 0x001367B0
	private void CheckMouthflapChange(bool isMicEnabled, float currentLoudness)
	{
		if (isMicEnabled)
		{
			this.useMicEnabled = true;
			int i = this.mouthFlapLevels.Length - 1;
			while (i >= 0)
			{
				if (currentLoudness >= this.mouthFlapLevels[i].maxRequiredVolume)
				{
					return;
				}
				if (currentLoudness > this.mouthFlapLevels[i].minRequiredVolume)
				{
					if (this.activeFlipbookIndex != i)
					{
						this.activeFlipbookIndex = i;
						this.activeFlipbookPlayTime = 0f;
						return;
					}
					return;
				}
				else
				{
					i--;
				}
			}
			return;
		}
		if (this.useMicEnabled)
		{
			this.useMicEnabled = false;
			this.activeFlipbookPlayTime = 0f;
		}
	}

	// Token: 0x0600394C RID: 14668 RVA: 0x0013863C File Offset: 0x0013683C
	private void UpdateMouthFlapFlipbook(MouthFlapLevel mouthFlap)
	{
		Material material = this.targetFaceRenderer.material;
		this.activeFlipbookPlayTime += this.deltaTime;
		this.activeFlipbookPlayTime %= mouthFlap.cycleDuration;
		int num = Mathf.FloorToInt(this.activeFlipbookPlayTime * (float)mouthFlap.faces.Length / mouthFlap.cycleDuration);
		material.SetTextureOffset(this._MouthMap, mouthFlap.faces[num]);
	}

	// Token: 0x0600394D RID: 14669 RVA: 0x001386B4 File Offset: 0x001368B4
	public void SetMouthTextureReplacement(Texture2D replacementMouthAtlas)
	{
		Material material = this.targetFaceRenderer.material;
		this.SetDefaultMouthAtlas(material);
		material.SetTexture(this._MouthMap, replacementMouthAtlas);
	}

	// Token: 0x0600394E RID: 14670 RVA: 0x001386E6 File Offset: 0x001368E6
	public void ClearMouthTextureReplacement()
	{
		this.targetFaceRenderer.material.SetTexture(this._MouthMap, this.defaultMouthAtlas);
	}

	// Token: 0x0600394F RID: 14671 RVA: 0x0013870C File Offset: 0x0013690C
	public Material SetFaceMaterialReplacement(Material replacementFaceMaterial)
	{
		if (!this.hasDefaultFaceMaterial)
		{
			this.defaultFaceMaterial = this.targetFaceRenderer.material;
			this.hasDefaultFaceMaterial = true;
		}
		this.targetFaceRenderer.material = replacementFaceMaterial;
		if (this.hasDefaultMouthAtlas && this.defaultMouthAtlas != null)
		{
			this.targetFaceRenderer.material.SetTexture(this._MouthMap, this.defaultMouthAtlas);
		}
		return this.targetFaceRenderer.material;
	}

	// Token: 0x06003950 RID: 14672 RVA: 0x00138787 File Offset: 0x00136987
	public void ClearFaceMaterialReplacement()
	{
		if (this.hasDefaultFaceMaterial)
		{
			this.targetFaceRenderer.material = this.defaultFaceMaterial;
		}
	}

	// Token: 0x06003951 RID: 14673 RVA: 0x001387A2 File Offset: 0x001369A2
	private void SetDefaultMouthAtlas(Material face)
	{
		if (!this.hasDefaultMouthAtlas)
		{
			this.defaultMouthAtlas = face.GetTexture(this._MouthMap);
			this.hasDefaultMouthAtlas = true;
		}
	}

	// Token: 0x04004960 RID: 18784
	public GameObject targetFace;

	// Token: 0x04004961 RID: 18785
	public MouthFlapLevel[] mouthFlapLevels;

	// Token: 0x04004962 RID: 18786
	public MouthFlapLevel noMicFace;

	// Token: 0x04004963 RID: 18787
	public MouthFlapLevel leafBlowerFace;

	// Token: 0x04004964 RID: 18788
	private bool useMicEnabled;

	// Token: 0x04004965 RID: 18789
	private float leafBlowerActiveUntilTimestamp;

	// Token: 0x04004966 RID: 18790
	private int activeFlipbookIndex;

	// Token: 0x04004967 RID: 18791
	private float activeFlipbookPlayTime;

	// Token: 0x04004968 RID: 18792
	private ISpeakerLoudness speaker;

	// Token: 0x04004969 RID: 18793
	private float lastTimeUpdated;

	// Token: 0x0400496A RID: 18794
	private float deltaTime;

	// Token: 0x0400496B RID: 18795
	private Renderer targetFaceRenderer;

	// Token: 0x0400496C RID: 18796
	private MaterialPropertyBlock facePropBlock;

	// Token: 0x0400496D RID: 18797
	private Texture defaultMouthAtlas;

	// Token: 0x0400496E RID: 18798
	private Material defaultFaceMaterial;

	// Token: 0x0400496F RID: 18799
	private bool hasDefaultMouthAtlas;

	// Token: 0x04004970 RID: 18800
	private bool hasDefaultFaceMaterial;

	// Token: 0x04004971 RID: 18801
	private ShaderHashId _MouthMap = "_MouthMap";

	// Token: 0x04004972 RID: 18802
	private ShaderHashId _BaseMap = "_BaseMap";
}
