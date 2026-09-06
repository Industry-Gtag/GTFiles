using System;
using UnityEngine;

// Token: 0x02000E3A RID: 3642
public class GrowUntilCollision : MonoBehaviour
{
	// Token: 0x06005904 RID: 22788 RVA: 0x001CEF00 File Offset: 0x001CD100
	private void Start()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		if (this.audioSource != null)
		{
			this.maxVolume = this.audioSource.volume;
			this.maxPitch = this.audioSource.pitch;
		}
		this.zero();
	}

	// Token: 0x06005905 RID: 22789 RVA: 0x001CEF50 File Offset: 0x001CD150
	private void zero()
	{
		base.transform.localScale = Vector3.one * this.initialRadius;
		if (this.audioSource != null)
		{
			this.audioSource.volume = 0f;
			this.audioSource.pitch = 1f;
		}
		this.timeSinceTrigger = 0f;
	}

	// Token: 0x06005906 RID: 22790 RVA: 0x001CEFB1 File Offset: 0x001CD1B1
	private void OnTriggerEnter(Collider other)
	{
		this.tryToTrigger(base.transform.position, other.transform.position);
	}

	// Token: 0x06005907 RID: 22791 RVA: 0x001CEFB1 File Offset: 0x001CD1B1
	private void OnTriggerExit(Collider other)
	{
		this.tryToTrigger(base.transform.position, other.transform.position);
	}

	// Token: 0x06005908 RID: 22792 RVA: 0x001CEFD0 File Offset: 0x001CD1D0
	private void OnCollisionEnter(Collision collision)
	{
		this.tryToTrigger(base.transform.position, collision.GetContact(0).point);
	}

	// Token: 0x06005909 RID: 22793 RVA: 0x001CF000 File Offset: 0x001CD200
	private void OnCollisionExit(Collision collision)
	{
		this.tryToTrigger(base.transform.position, collision.GetContact(0).point);
	}

	// Token: 0x0600590A RID: 22794 RVA: 0x001CF02D File Offset: 0x001CD22D
	private void tryToTrigger(Vector3 p1, Vector3 p2)
	{
		if (this.timeSinceTrigger > this.minRetriggerTime)
		{
			if (this.colliderFound != null)
			{
				this.colliderFound.Invoke(p1, p2);
			}
			this.zero();
		}
	}

	// Token: 0x0600590B RID: 22795 RVA: 0x001CF058 File Offset: 0x001CD258
	private void Update()
	{
		float num = Mathf.Max(new float[]
		{
			base.transform.lossyScale.x,
			base.transform.lossyScale.y,
			base.transform.lossyScale.z
		});
		if (base.transform.localScale.x < this.maxSize * num)
		{
			base.transform.localScale += Vector3.one * Time.deltaTime * num;
			if (this.audioSource != null)
			{
				this.audioSource.volume = this.maxVolume * (base.transform.localScale.x / this.maxSize);
				this.audioSource.pitch = 1f + this.maxPitch * (base.transform.localScale.x / this.maxSize);
			}
		}
		this.timeSinceTrigger += Time.deltaTime;
	}

	// Token: 0x04006935 RID: 26933
	[SerializeField]
	private float maxSize = 10f;

	// Token: 0x04006936 RID: 26934
	[SerializeField]
	private float initialRadius = 1f;

	// Token: 0x04006937 RID: 26935
	[SerializeField]
	private float minRetriggerTime = 1f;

	// Token: 0x04006938 RID: 26936
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x04006939 RID: 26937
	private AudioSource audioSource;

	// Token: 0x0400693A RID: 26938
	private float maxVolume;

	// Token: 0x0400693B RID: 26939
	private float maxPitch;

	// Token: 0x0400693C RID: 26940
	private float timeSinceTrigger;
}
