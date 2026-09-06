using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200067C RID: 1660
public class ChestHeartbeat : MonoBehaviour
{
	// Token: 0x06002989 RID: 10633 RVA: 0x000E0D9C File Offset: 0x000DEF9C
	public void Update()
	{
		if (PhotonNetwork.InRoom)
		{
			if ((PhotonNetwork.ServerTimestamp > this.lastShot + this.millisMin || Mathf.Abs(PhotonNetwork.ServerTimestamp - this.lastShot) > 10000) && PhotonNetwork.ServerTimestamp % 1500 <= 10)
			{
				this.lastShot = PhotonNetwork.ServerTimestamp;
				this.audioSource.GTPlayOneShot(this.audioSource.clip, 1f);
				base.StartCoroutine(this.HeartBeat());
				return;
			}
		}
		else if ((Time.time * 1000f > (float)(this.lastShot + this.millisMin) || Mathf.Abs(Time.time * 1000f - (float)this.lastShot) > 10000f) && Time.time * 1000f % 1500f <= 10f)
		{
			this.lastShot = PhotonNetwork.ServerTimestamp;
			this.audioSource.GTPlayOneShot(this.audioSource.clip, 1f);
			base.StartCoroutine(this.HeartBeat());
		}
	}

	// Token: 0x0600298A RID: 10634 RVA: 0x000E0EAA File Offset: 0x000DF0AA
	private IEnumerator HeartBeat()
	{
		float startTime = Time.time;
		while (Time.time < startTime + this.endtime)
		{
			if (Time.time < startTime + this.minTime)
			{
				this.deltaTime = Time.time - startTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * this.heartMinSize, this.deltaTime / this.minTime);
			}
			else if (Time.time < startTime + this.maxTime)
			{
				this.deltaTime = Time.time - startTime - this.minTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one * this.heartMinSize, Vector3.one * this.heartMaxSize, this.deltaTime / (this.maxTime - this.minTime));
			}
			else if (Time.time < startTime + this.endtime)
			{
				this.deltaTime = Time.time - startTime - this.maxTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one * this.heartMaxSize, Vector3.one, this.deltaTime / (this.endtime - this.maxTime));
			}
			yield return new WaitForFixedUpdate();
		}
		yield break;
	}

	// Token: 0x04003623 RID: 13859
	public int millisToWait;

	// Token: 0x04003624 RID: 13860
	public int millisMin = 300;

	// Token: 0x04003625 RID: 13861
	public int lastShot;

	// Token: 0x04003626 RID: 13862
	public AudioSource audioSource;

	// Token: 0x04003627 RID: 13863
	public Transform scaleTransform;

	// Token: 0x04003628 RID: 13864
	private float deltaTime;

	// Token: 0x04003629 RID: 13865
	private float heartMinSize = 0.9f;

	// Token: 0x0400362A RID: 13866
	private float heartMaxSize = 1.2f;

	// Token: 0x0400362B RID: 13867
	private float minTime = 0.05f;

	// Token: 0x0400362C RID: 13868
	private float maxTime = 0.1f;

	// Token: 0x0400362D RID: 13869
	private float endtime = 0.25f;

	// Token: 0x0400362E RID: 13870
	private float currentTime;
}
