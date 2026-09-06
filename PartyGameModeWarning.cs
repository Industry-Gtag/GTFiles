using System;
using System.Collections;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x02000360 RID: 864
public class PartyGameModeWarning : MonoBehaviour
{
	// Token: 0x17000218 RID: 536
	// (get) Token: 0x06001527 RID: 5415 RVA: 0x00070AB8 File Offset: 0x0006ECB8
	public bool ShouldShowWarning
	{
		get
		{
			return FriendshipGroupDetection.Instance.IsInParty && FriendshipGroupDetection.Instance.AnyPartyMembersOutsideFriendCollider();
		}
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x00070AD4 File Offset: 0x0006ECD4
	private void Awake()
	{
		GameObject[] array = this.showParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
	}

	// Token: 0x06001529 RID: 5417 RVA: 0x00070AFF File Offset: 0x0006ECFF
	public void Show()
	{
		this.visibleUntilTimestamp = Time.time + this.visibleDuration;
		if (this.hideCoroutine == null)
		{
			this.hideCoroutine = base.StartCoroutine(this.HideCo());
		}
	}

	// Token: 0x0600152A RID: 5418 RVA: 0x00070B2D File Offset: 0x0006ED2D
	private IEnumerator HideCo()
	{
		GameObject[] array = this.showParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		array = this.hideParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		float lastVisible;
		do
		{
			lastVisible = this.visibleUntilTimestamp;
			yield return new WaitForSeconds(this.visibleUntilTimestamp - Time.time);
		}
		while (lastVisible != this.visibleUntilTimestamp);
		array = this.showParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		array = this.hideParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		this.hideCoroutine = null;
		yield break;
	}

	// Token: 0x04001A0C RID: 6668
	[SerializeField]
	private GameObject[] showParts;

	// Token: 0x04001A0D RID: 6669
	[SerializeField]
	private GameObject[] hideParts;

	// Token: 0x04001A0E RID: 6670
	[SerializeField]
	private float visibleDuration;

	// Token: 0x04001A0F RID: 6671
	private float visibleUntilTimestamp;

	// Token: 0x04001A10 RID: 6672
	private Coroutine hideCoroutine;
}
