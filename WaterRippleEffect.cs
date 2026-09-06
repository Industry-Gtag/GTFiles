using System;
using GorillaLocomotion.Swimming;
using UnityEngine;

// Token: 0x020003B5 RID: 949
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class WaterRippleEffect : MonoBehaviour
{
	// Token: 0x060016E8 RID: 5864 RVA: 0x00085411 File Offset: 0x00083611
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.renderer = base.GetComponent<SpriteRenderer>();
		this.ripplePlaybackSpeedHash = Animator.StringToHash(this.ripplePlaybackSpeedName);
	}

	// Token: 0x060016E9 RID: 5865 RVA: 0x0008543C File Offset: 0x0008363C
	public void Destroy()
	{
		this.waterVolume = null;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x060016EA RID: 5866 RVA: 0x00085458 File Offset: 0x00083658
	public void PlayEffect(WaterVolume volume = null)
	{
		this.waterVolume = volume;
		this.rippleStartTime = Time.time;
		this.animator.SetFloat(this.ripplePlaybackSpeedHash, this.ripplePlaybackSpeed);
		if (this.waterVolume != null && this.waterVolume.Parameters != null)
		{
			this.renderer.color = this.waterVolume.Parameters.rippleSpriteColor;
		}
		Color color = this.renderer.color;
		color.a = 1f;
		this.renderer.color = color;
	}

	// Token: 0x060016EB RID: 5867 RVA: 0x000854F0 File Offset: 0x000836F0
	private void Update()
	{
		if (this.waterVolume != null && !this.waterVolume.isStationary && this.waterVolume.surfacePlane != null)
		{
			Vector3 vector = Vector3.Dot(base.transform.position - this.waterVolume.surfacePlane.position, this.waterVolume.surfacePlane.up) * this.waterVolume.surfacePlane.up;
			base.transform.position = base.transform.position - vector;
		}
		float num = Mathf.Clamp01((Time.time - this.rippleStartTime - this.fadeOutDelay) / this.fadeOutTime);
		Color color = this.renderer.color;
		color.a = 1f - num;
		this.renderer.color = color;
		if (num >= 1f - Mathf.Epsilon)
		{
			this.Destroy();
			return;
		}
	}

	// Token: 0x040021E8 RID: 8680
	[SerializeField]
	private float ripplePlaybackSpeed = 1f;

	// Token: 0x040021E9 RID: 8681
	[SerializeField]
	private float fadeOutDelay = 0.5f;

	// Token: 0x040021EA RID: 8682
	[SerializeField]
	private float fadeOutTime = 1f;

	// Token: 0x040021EB RID: 8683
	private string ripplePlaybackSpeedName = "RipplePlaybackSpeed";

	// Token: 0x040021EC RID: 8684
	private int ripplePlaybackSpeedHash;

	// Token: 0x040021ED RID: 8685
	private float rippleStartTime = -1f;

	// Token: 0x040021EE RID: 8686
	private Animator animator;

	// Token: 0x040021EF RID: 8687
	private SpriteRenderer renderer;

	// Token: 0x040021F0 RID: 8688
	private WaterVolume waterVolume;
}
