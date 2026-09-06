using System;
using UnityEngine;

// Token: 0x020002C1 RID: 705
public class DJDeckEqualizer : MonoBehaviour
{
	// Token: 0x06001239 RID: 4665 RVA: 0x00061C19 File Offset: 0x0005FE19
	private void Start()
	{
		this.inputColorHash = this.inputColorProperty;
		this.material = this.display.material;
	}

	// Token: 0x0600123A RID: 4666 RVA: 0x00061C40 File Offset: 0x0005FE40
	private void Update()
	{
		Color color = default(Color);
		color.r = 0.25f;
		color.g = 0.25f;
		color.b = 0.5f;
		for (int i = 0; i < this.redTracks.Length; i++)
		{
			AudioSource audioSource = this.redTracks[i];
			if (audioSource.isPlaying)
			{
				color.r = Mathf.Lerp(0.25f, 1f, this.redTrackCurves[i].Evaluate(audioSource.time));
				break;
			}
		}
		for (int j = 0; j < this.greenTracks.Length; j++)
		{
			AudioSource audioSource2 = this.greenTracks[j];
			if (audioSource2.isPlaying)
			{
				color.g = Mathf.Lerp(0.25f, 1f, this.greenTrackCurves[j].Evaluate(audioSource2.time));
				break;
			}
		}
		this.material.SetColor(this.inputColorHash, color);
	}

	// Token: 0x04001602 RID: 5634
	[SerializeField]
	private MeshRenderer display;

	// Token: 0x04001603 RID: 5635
	[SerializeField]
	private AnimationCurve[] redTrackCurves;

	// Token: 0x04001604 RID: 5636
	[SerializeField]
	private AnimationCurve[] greenTrackCurves;

	// Token: 0x04001605 RID: 5637
	[SerializeField]
	private AudioSource[] redTracks;

	// Token: 0x04001606 RID: 5638
	[SerializeField]
	private AudioSource[] greenTracks;

	// Token: 0x04001607 RID: 5639
	private Material material;

	// Token: 0x04001608 RID: 5640
	[SerializeField]
	private string inputColorProperty;

	// Token: 0x04001609 RID: 5641
	private ShaderHashId inputColorHash;
}
