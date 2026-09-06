using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002C6 RID: 710
public class ElfLauncherElf : MonoBehaviour
{
	// Token: 0x06001257 RID: 4695 RVA: 0x00062744 File Offset: 0x00060944
	private void OnEnable()
	{
		base.StartCoroutine(this.ReturnToPoolAfterDelayCo());
	}

	// Token: 0x06001258 RID: 4696 RVA: 0x00062753 File Offset: 0x00060953
	private IEnumerator ReturnToPoolAfterDelayCo()
	{
		yield return new WaitForSeconds(this.destroyAfterDuration);
		ObjectPools.instance.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06001259 RID: 4697 RVA: 0x00062762 File Offset: 0x00060962
	private void OnCollisionEnter(Collision collision)
	{
		if (this.bounceAudioCoolingDownUntilTimestamp > Time.time)
		{
			return;
		}
		this.bounceAudio.Play();
		this.bounceAudioCoolingDownUntilTimestamp = Time.time + this.bounceAudioCooldownDuration;
	}

	// Token: 0x0600125A RID: 4698 RVA: 0x0006278F File Offset: 0x0006098F
	private void FixedUpdate()
	{
		this.rb.AddForce(base.transform.lossyScale.x * Physics.gravity * this.rb.mass, ForceMode.Force);
	}

	// Token: 0x0400163A RID: 5690
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x0400163B RID: 5691
	[SerializeField]
	private SoundBankPlayer bounceAudio;

	// Token: 0x0400163C RID: 5692
	[SerializeField]
	private float bounceAudioCooldownDuration;

	// Token: 0x0400163D RID: 5693
	[SerializeField]
	private float destroyAfterDuration;

	// Token: 0x0400163E RID: 5694
	private float bounceAudioCoolingDownUntilTimestamp;
}
