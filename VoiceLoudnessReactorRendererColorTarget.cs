using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E81 RID: 3713
[Serializable]
public class VoiceLoudnessReactorRendererColorTarget
{
	// Token: 0x06005A49 RID: 23113 RVA: 0x001D54C0 File Offset: 0x001D36C0
	public void Inititialize()
	{
		if (this._materials == null)
		{
			this._materials = new List<Material>(this.renderer.materials);
			this._materials[this.materialIndex].EnableKeyword(this.colorProperty);
			this.renderer.SetMaterials(this._materials);
			this.UpdateMaterialColor(0f);
		}
	}

	// Token: 0x06005A4A RID: 23114 RVA: 0x001D5524 File Offset: 0x001D3724
	public void UpdateMaterialColor(float level)
	{
		Color color = this.gradient.Evaluate(level);
		if (this._lastColor == color)
		{
			return;
		}
		this._materials[this.materialIndex].SetColor(this.colorProperty, color);
		this._lastColor = color;
	}

	// Token: 0x04006B53 RID: 27475
	[SerializeField]
	private string colorProperty = "_BaseColor";

	// Token: 0x04006B54 RID: 27476
	public Renderer renderer;

	// Token: 0x04006B55 RID: 27477
	public int materialIndex;

	// Token: 0x04006B56 RID: 27478
	public Gradient gradient;

	// Token: 0x04006B57 RID: 27479
	public bool useSmoothedLoudness;

	// Token: 0x04006B58 RID: 27480
	public float scale = 1f;

	// Token: 0x04006B59 RID: 27481
	private List<Material> _materials;

	// Token: 0x04006B5A RID: 27482
	private Color _lastColor = Color.white;
}
