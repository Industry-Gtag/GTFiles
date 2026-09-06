using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020000CB RID: 203
public class ShadeJumpscare : MonoBehaviour
{
	// Token: 0x060004EA RID: 1258 RVA: 0x0001B6D3 File Offset: 0x000198D3
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x0001B6E1 File Offset: 0x000198E1
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.startAngle = Random.value * 360f;
		this.audioSource.clip = this.audioClips.GetRandomItem<AudioClip>();
		this.audioSource.GTPlay();
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x0001B720 File Offset: 0x00019920
	private void Update()
	{
		float num = Time.time - this.startTime;
		float num2 = num / this.animationTime;
		this.shadeTransform.SetPositionAndRotation(base.transform.position + new Vector3(0f, this.shadeHeightFunction.Evaluate(num2), 0f), Quaternion.Euler(0f, this.startAngle + num * this.shadeRotationSpeed, 0f));
		float num3 = this.shadeScaleFunction.Evaluate(num2);
		this.shadeTransform.localScale = new Vector3(num3, num3 * this.shadeYScaleMultFunction.Evaluate(num2), num3);
		this.audioSource.volume = this.soundVolumeFunction.Evaluate(num2);
	}

	// Token: 0x0400057F RID: 1407
	[SerializeField]
	private Transform shadeTransform;

	// Token: 0x04000580 RID: 1408
	[SerializeField]
	private float animationTime;

	// Token: 0x04000581 RID: 1409
	[SerializeField]
	private float shadeRotationSpeed = 1f;

	// Token: 0x04000582 RID: 1410
	[SerializeField]
	private AnimationCurve shadeHeightFunction;

	// Token: 0x04000583 RID: 1411
	[SerializeField]
	private AnimationCurve shadeScaleFunction;

	// Token: 0x04000584 RID: 1412
	[SerializeField]
	private AnimationCurve shadeYScaleMultFunction;

	// Token: 0x04000585 RID: 1413
	[SerializeField]
	private AnimationCurve soundVolumeFunction;

	// Token: 0x04000586 RID: 1414
	[SerializeField]
	private AudioClip[] audioClips;

	// Token: 0x04000587 RID: 1415
	private AudioSource audioSource;

	// Token: 0x04000588 RID: 1416
	private float startTime;

	// Token: 0x04000589 RID: 1417
	private float startAngle;
}
