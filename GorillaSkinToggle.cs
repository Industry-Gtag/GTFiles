using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020002DA RID: 730
public class GorillaSkinToggle : MonoBehaviour, ISpawnable
{
	// Token: 0x170001D8 RID: 472
	// (get) Token: 0x060012AF RID: 4783 RVA: 0x00063B4D File Offset: 0x00061D4D
	public bool applied
	{
		get
		{
			return this._applied;
		}
	}

	// Token: 0x170001D9 RID: 473
	// (get) Token: 0x060012B0 RID: 4784 RVA: 0x00063B55 File Offset: 0x00061D55
	// (set) Token: 0x060012B1 RID: 4785 RVA: 0x00063B5D File Offset: 0x00061D5D
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170001DA RID: 474
	// (get) Token: 0x060012B2 RID: 4786 RVA: 0x00063B66 File Offset: 0x00061D66
	// (set) Token: 0x060012B3 RID: 4787 RVA: 0x00063B6E File Offset: 0x00061D6E
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060012B4 RID: 4788 RVA: 0x00063B78 File Offset: 0x00061D78
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this._rig = base.GetComponentInParent<VRRig>(true);
		if (this.coloringRules.Length != 0)
		{
			this._activeSkin = GorillaSkin.CopyWithInstancedMaterials(this._skin);
			for (int i = 0; i < this.coloringRules.Length; i++)
			{
				this.coloringRules[i].Init();
			}
			return;
		}
		this._activeSkin = this._skin;
	}

	// Token: 0x060012B5 RID: 4789 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060012B6 RID: 4790 RVA: 0x00063BE0 File Offset: 0x00061DE0
	private void OnPlayerColorChanged(Color playerColor)
	{
		foreach (GorillaSkinToggle.ColoringRule coloringRule in this.coloringRules)
		{
			coloringRule.Apply(this._activeSkin, playerColor);
		}
	}

	// Token: 0x060012B7 RID: 4791 RVA: 0x00063C18 File Offset: 0x00061E18
	private void OnEnable()
	{
		if (this.coloringRules.Length != 0)
		{
			this._rig.OnColorChanged += this.OnPlayerColorChanged;
			this.OnPlayerColorChanged(this._rig.playerColor);
		}
		this.Apply();
	}

	// Token: 0x060012B8 RID: 4792 RVA: 0x00063C51 File Offset: 0x00061E51
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.Remove();
		if (this.coloringRules.Length != 0)
		{
			this._rig.OnColorChanged -= this.OnPlayerColorChanged;
		}
	}

	// Token: 0x060012B9 RID: 4793 RVA: 0x00063C81 File Offset: 0x00061E81
	public void Apply()
	{
		GorillaSkin.ApplyToRig(this._rig, this._activeSkin, GorillaSkin.SkinType.cosmetic);
		this._applied = true;
	}

	// Token: 0x060012BA RID: 4794 RVA: 0x00063C9C File Offset: 0x00061E9C
	public void ApplyToMannequin(GameObject mannequin, bool swapMesh = false)
	{
		if (this._skin.IsNull())
		{
			Debug.LogError("No skin set on GorillaSkinToggle");
			return;
		}
		if (mannequin.IsNull())
		{
			Debug.LogError("No mannequin set on GorillaSkinToggle");
			return;
		}
		this._skin.ApplySkinToMannequin(mannequin, swapMesh);
	}

	// Token: 0x060012BB RID: 4795 RVA: 0x00063CD8 File Offset: 0x00061ED8
	public void Remove()
	{
		GorillaSkin.ApplyToRig(this._rig, null, GorillaSkin.SkinType.cosmetic);
		float @float = PlayerPrefs.GetFloat("redValue", 0f);
		float float2 = PlayerPrefs.GetFloat("greenValue", 0f);
		float float3 = PlayerPrefs.GetFloat("blueValue", 0f);
		GorillaTagger.Instance.UpdateColor(@float, float2, float3);
		this._applied = false;
	}

	// Token: 0x040016C0 RID: 5824
	private VRRig _rig;

	// Token: 0x040016C1 RID: 5825
	[SerializeField]
	private GorillaSkin _skin;

	// Token: 0x040016C2 RID: 5826
	private GorillaSkin _activeSkin;

	// Token: 0x040016C3 RID: 5827
	[SerializeField]
	private GorillaSkinToggle.ColoringRule[] coloringRules;

	// Token: 0x040016C4 RID: 5828
	[Space]
	[SerializeField]
	private bool _applied;

	// Token: 0x020002DB RID: 731
	[Serializable]
	private struct ColoringRule
	{
		// Token: 0x060012BD RID: 4797 RVA: 0x00063D36 File Offset: 0x00061F36
		public void Init()
		{
			if (string.IsNullOrEmpty(this.shaderColorProperty))
			{
				this.shaderColorProperty = "_BaseColor";
			}
			this.shaderHashId = new ShaderHashId(this.shaderColorProperty);
		}

		// Token: 0x060012BE RID: 4798 RVA: 0x00063D64 File Offset: 0x00061F64
		public void Apply(GorillaSkin skin, Color color)
		{
			if (this.colorMaterials.HasFlag(GorillaSkinMaterials.Body))
			{
				skin.bodyMaterial.SetColor(this.shaderHashId, color);
			}
			if (this.colorMaterials.HasFlag(GorillaSkinMaterials.Chest))
			{
				skin.chestMaterial.SetColor(this.shaderHashId, color);
			}
			if (this.colorMaterials.HasFlag(GorillaSkinMaterials.Scoreboard))
			{
				skin.scoreboardMaterial.SetColor(this.shaderHashId, color);
			}
		}

		// Token: 0x040016C7 RID: 5831
		public GorillaSkinMaterials colorMaterials;

		// Token: 0x040016C8 RID: 5832
		public string shaderColorProperty;

		// Token: 0x040016C9 RID: 5833
		private ShaderHashId shaderHashId;
	}
}
