using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Pool;

// Token: 0x0200059D RID: 1437
public class PlayerColoredCosmetic : MonoBehaviour
{
	// Token: 0x06002471 RID: 9329 RVA: 0x000C3A40 File Offset: 0x000C1C40
	public void Awake()
	{
		for (int i = 0; i < this.coloringRules.Length; i++)
		{
			this.coloringRules[i].Init(this.dontCreateMaterialInstance);
		}
	}

	// Token: 0x06002472 RID: 9330 RVA: 0x000C3A78 File Offset: 0x000C1C78
	private void InitIfNeeded()
	{
		if (!this.didInit)
		{
			this.didInit = true;
			this.rig = base.GetComponentInParent<VRRig>();
			if (this.rig == null && GorillaTagger.Instance != null)
			{
				this.rig = GorillaTagger.Instance.offlineVRRig;
			}
			this.particleMains = new ParticleSystem.MainModule[this.particleSystems.Length];
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				this.particleMains[i] = this.particleSystems[i].main;
			}
		}
	}

	// Token: 0x06002473 RID: 9331 RVA: 0x000C3B0A File Offset: 0x000C1D0A
	private void OnEnable()
	{
		this.InitIfNeeded();
		if (this.rig != null)
		{
			this.rig.OnColorChanged += this.UpdateColor;
			this.UpdateColor(this.rig.playerColor);
		}
	}

	// Token: 0x06002474 RID: 9332 RVA: 0x000C3B48 File Offset: 0x000C1D48
	private void OnDisable()
	{
		if (this.rig != null)
		{
			this.rig.OnColorChanged -= this.UpdateColor;
		}
	}

	// Token: 0x06002475 RID: 9333 RVA: 0x000C3B70 File Offset: 0x000C1D70
	public void UpdateColor(Color color)
	{
		this.InitIfNeeded();
		Color color2 = Color.Lerp(color, this.lerpToColor, this.lerpStrength);
		for (int i = 0; i < this.coloringRules.Length; i++)
		{
			this.coloringRules[i].Apply(color2, this.dontCreateMaterialInstance);
		}
		for (int j = 0; j < this.particleSystems.Length; j++)
		{
			this.particleMains[j].startColor = color2;
		}
	}

	// Token: 0x04002FC6 RID: 12230
	private const string preLog = "[GT/PlayerColoredCosmetic]  ";

	// Token: 0x04002FC7 RID: 12231
	private const string preErr = "ERROR!!!  ";

	// Token: 0x04002FC8 RID: 12232
	private bool didInit;

	// Token: 0x04002FC9 RID: 12233
	private VRRig rig;

	// Token: 0x04002FCA RID: 12234
	[SerializeField]
	private Color lerpToColor = Color.white;

	// Token: 0x04002FCB RID: 12235
	[SerializeField]
	[Range(0f, 1f)]
	private float lerpStrength;

	// Token: 0x04002FCC RID: 12236
	[SerializeField]
	private bool dontCreateMaterialInstance;

	// Token: 0x04002FCD RID: 12237
	[SerializeField]
	private PlayerColoredCosmetic.ColoringRule[] coloringRules;

	// Token: 0x04002FCE RID: 12238
	[SerializeField]
	private ParticleSystem[] particleSystems;

	// Token: 0x04002FCF RID: 12239
	private ParticleSystem.MainModule[] particleMains;

	// Token: 0x0200059E RID: 1438
	[Serializable]
	private struct ColoringRule
	{
		// Token: 0x06002477 RID: 9335 RVA: 0x000C3C00 File Offset: 0x000C1E00
		public void Init(bool dontCreateMaterialInstance)
		{
			this.hashId = Shader.PropertyToID(this.shaderColorProperty);
			if (this.meshRenderer == null)
			{
				Debug.LogError("ERROR!!!  ColoringRule.Init: Default meshRenderer cannot be null! Path=" + this.meshRenderer.transform.GetPathQ());
			}
			List<Material> list;
			using (CollectionPool<List<Material>, Material>.Get(out list))
			{
				this.meshRenderer.GetSharedMaterials(list);
				if (this.materialIndex < 0 || this.materialIndex >= list.Count)
				{
					Debug.LogError("ERROR!!!  " + string.Format("ColoringRule.Init: Material index {0} is out of range! Path=", this.materialIndex) + this.meshRenderer.transform.GetPathQ(), this.meshRenderer);
				}
				this.defaultMaterial = list[this.materialIndex];
				if (this.defaultMaterial == null)
				{
					Debug.LogError("ERROR!!!  ColoringRule.Init: Default material cannot be null! Path=" + this.meshRenderer.transform.GetPathQ(), this.meshRenderer);
				}
				if (dontCreateMaterialInstance)
				{
					this.instancedMaterial = list[this.materialIndex];
				}
				else
				{
					this.instancedMaterial = new Material(list[this.materialIndex]);
					list[this.materialIndex] = this.instancedMaterial;
					this.meshRenderer.SetSharedMaterials(list);
				}
			}
		}

		// Token: 0x06002478 RID: 9336 RVA: 0x000C3D64 File Offset: 0x000C1F64
		public void Apply(Color color, bool dontCreateMaterialInstance)
		{
			if (dontCreateMaterialInstance)
			{
				List<Material> list;
				using (CollectionPool<List<Material>, Material>.Get(out list))
				{
					this.meshRenderer.GetSharedMaterials(list);
					if (this.materialIndex >= 0 && this.materialIndex < list.Count && list[this.materialIndex] != null)
					{
						list[this.materialIndex].SetColor(this.hashId, color);
					}
					return;
				}
			}
			this.instancedMaterial.SetColor(this.hashId, color);
		}

		// Token: 0x04002FD0 RID: 12240
		[SerializeField]
		private string shaderColorProperty;

		// Token: 0x04002FD1 RID: 12241
		private int hashId;

		// Token: 0x04002FD2 RID: 12242
		[SerializeField]
		private Renderer meshRenderer;

		// Token: 0x04002FD3 RID: 12243
		[SerializeField]
		private int materialIndex;

		// Token: 0x04002FD4 RID: 12244
		private Material instancedMaterial;

		// Token: 0x04002FD5 RID: 12245
		private Material defaultMaterial;
	}
}
