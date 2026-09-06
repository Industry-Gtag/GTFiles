using System;
using System.Collections;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000AE1 RID: 2785
public class HalloweenWatcherEyes : MonoBehaviour
{
	// Token: 0x0600476F RID: 18287 RVA: 0x0018144C File Offset: 0x0017F64C
	private void Start()
	{
		this.playersViewCenterCosAngle = Mathf.Cos(this.playersViewCenterAngle * 0.017453292f);
		this.watchMinCosAngle = Mathf.Cos(this.watchMaxAngle * 0.017453292f);
		base.StartCoroutine(this.CheckIfNearPlayer(Random.Range(0f, this.timeBetweenUpdates)));
		base.enabled = false;
	}

	// Token: 0x06004770 RID: 18288 RVA: 0x001814AB File Offset: 0x0017F6AB
	private IEnumerator CheckIfNearPlayer(float initialSleep)
	{
		yield return new WaitForSeconds(initialSleep);
		for (;;)
		{
			base.enabled = (base.transform.position - GTPlayer.Instance.transform.position).sqrMagnitude < this.watchRange * this.watchRange;
			if (!base.enabled)
			{
				this.LookNormal();
			}
			yield return new WaitForSeconds(this.timeBetweenUpdates);
		}
		yield break;
	}

	// Token: 0x06004771 RID: 18289 RVA: 0x001814C4 File Offset: 0x0017F6C4
	private void Update()
	{
		Vector3 normalized = (GTPlayer.Instance.headCollider.transform.position - base.transform.position).normalized;
		if (Vector3.Dot(GTPlayer.Instance.headCollider.transform.forward, -normalized) > this.playersViewCenterCosAngle)
		{
			this.LookNormal();
			this.pretendingToBeNormalUntilTimestamp = Time.time + this.durationToBeNormalWhenPlayerLooks;
		}
		if (this.pretendingToBeNormalUntilTimestamp > Time.time)
		{
			return;
		}
		if (Vector3.Dot(base.transform.forward, normalized) < this.watchMinCosAngle)
		{
			this.LookNormal();
			return;
		}
		Quaternion quaternion = Quaternion.LookRotation(normalized, base.transform.up);
		Quaternion quaternion2 = Quaternion.Lerp(base.transform.rotation, quaternion, this.lerpValue);
		this.leftEye.transform.rotation = quaternion2;
		this.rightEye.transform.rotation = quaternion2;
		if (this.lerpDuration > 0f)
		{
			this.lerpValue = Mathf.MoveTowards(this.lerpValue, 1f, Time.deltaTime / this.lerpDuration);
			return;
		}
		this.lerpValue = 1f;
	}

	// Token: 0x06004772 RID: 18290 RVA: 0x001815F2 File Offset: 0x0017F7F2
	private void LookNormal()
	{
		this.leftEye.transform.localRotation = Quaternion.identity;
		this.rightEye.transform.localRotation = Quaternion.identity;
		this.lerpValue = 0f;
	}

	// Token: 0x040059F9 RID: 23033
	public float timeBetweenUpdates = 5f;

	// Token: 0x040059FA RID: 23034
	public float watchRange;

	// Token: 0x040059FB RID: 23035
	public float watchMaxAngle;

	// Token: 0x040059FC RID: 23036
	public float lerpDuration = 1f;

	// Token: 0x040059FD RID: 23037
	public float playersViewCenterAngle = 30f;

	// Token: 0x040059FE RID: 23038
	public float durationToBeNormalWhenPlayerLooks = 3f;

	// Token: 0x040059FF RID: 23039
	public GameObject leftEye;

	// Token: 0x04005A00 RID: 23040
	public GameObject rightEye;

	// Token: 0x04005A01 RID: 23041
	private float playersViewCenterCosAngle;

	// Token: 0x04005A02 RID: 23042
	private float watchMinCosAngle;

	// Token: 0x04005A03 RID: 23043
	private float pretendingToBeNormalUntilTimestamp;

	// Token: 0x04005A04 RID: 23044
	private float lerpValue;
}
