using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002C8 RID: 712
public class EqualizerAnim : MonoBehaviour
{
	// Token: 0x06001262 RID: 4706 RVA: 0x0006283D File Offset: 0x00060A3D
	private void Start()
	{
		this.inputColorHash = Shader.PropertyToID(this.inputColorProperty);
	}

	// Token: 0x06001263 RID: 4707 RVA: 0x00062850 File Offset: 0x00060A50
	private void Update()
	{
		if (EqualizerAnim.thisFrame == Time.frameCount)
		{
			if (EqualizerAnim.materialsUpdatedThisFrame.Contains(this.material))
			{
				return;
			}
		}
		else
		{
			EqualizerAnim.thisFrame = Time.frameCount;
			EqualizerAnim.materialsUpdatedThisFrame.Clear();
		}
		float num = Time.time % this.loopDuration;
		this.material.SetColor(this.inputColorHash, new Color(this.redCurve.Evaluate(num), this.greenCurve.Evaluate(num), this.blueCurve.Evaluate(num)));
		EqualizerAnim.materialsUpdatedThisFrame.Add(this.material);
	}

	// Token: 0x04001642 RID: 5698
	[SerializeField]
	private AnimationCurve redCurve;

	// Token: 0x04001643 RID: 5699
	[SerializeField]
	private AnimationCurve greenCurve;

	// Token: 0x04001644 RID: 5700
	[SerializeField]
	private AnimationCurve blueCurve;

	// Token: 0x04001645 RID: 5701
	[SerializeField]
	private float loopDuration;

	// Token: 0x04001646 RID: 5702
	[SerializeField]
	private Material material;

	// Token: 0x04001647 RID: 5703
	[SerializeField]
	private string inputColorProperty;

	// Token: 0x04001648 RID: 5704
	private int inputColorHash;

	// Token: 0x04001649 RID: 5705
	private static int thisFrame;

	// Token: 0x0400164A RID: 5706
	private static HashSet<Material> materialsUpdatedThisFrame = new HashSet<Material>();
}
