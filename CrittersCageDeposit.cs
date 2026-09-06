using System;
using System.Collections;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200005B RID: 91
public class CrittersCageDeposit : CrittersActorDeposit
{
	// Token: 0x14000005 RID: 5
	// (add) Token: 0x060001C8 RID: 456 RVA: 0x0000ACB4 File Offset: 0x00008EB4
	// (remove) Token: 0x060001C9 RID: 457 RVA: 0x0000ACEC File Offset: 0x00008EEC
	public event Action<Menagerie.CritterData, int> OnDepositCritter;

	// Token: 0x060001CA RID: 458 RVA: 0x0000AD21 File Offset: 0x00008F21
	private void Awake()
	{
		this.attachPoint.OnGrabbedChild += this.StartProcessCage;
	}

	// Token: 0x060001CB RID: 459 RVA: 0x0000AD3A File Offset: 0x00008F3A
	protected override bool CanDeposit(CrittersActor depositActor)
	{
		return base.CanDeposit(depositActor) && !this.isHandlingDeposit;
	}

	// Token: 0x060001CC RID: 460 RVA: 0x0000AD50 File Offset: 0x00008F50
	private void StartProcessCage(CrittersActor depositedActor)
	{
		this.currentCage = depositedActor;
		base.StartCoroutine(this.ProcessCage());
	}

	// Token: 0x060001CD RID: 461 RVA: 0x0000AD66 File Offset: 0x00008F66
	private IEnumerator ProcessCage()
	{
		this.isHandlingDeposit = true;
		bool isLocalDeposit = this.currentCage.lastGrabbedPlayer == PhotonNetwork.LocalPlayer.ActorNumber;
		this.depositAudio.GTPlayOneShot(this.depositStartSound, isLocalDeposit ? 1f : 0.5f);
		float transition = 0f;
		CrittersPawn crittersPawn = this.currentCage.GetComponentInChildren<CrittersPawn>();
		int lastGrabbedPlayer = this.currentCage.lastGrabbedPlayer;
		Menagerie.CritterData critterData;
		if (crittersPawn.IsNotNull())
		{
			critterData = new Menagerie.CritterData(crittersPawn.visuals);
		}
		else
		{
			critterData = new Menagerie.CritterData();
		}
		while (transition < this.submitDuration)
		{
			transition += Time.deltaTime;
			this.attachPoint.transform.localPosition = Vector3.Lerp(this.depositStartLocation, this.depositEndLocation, Mathf.Min(transition / this.submitDuration, 1f));
			yield return null;
		}
		if (crittersPawn.IsNotNull())
		{
			Action<Menagerie.CritterData, int> onDepositCritter = this.OnDepositCritter;
			if (onDepositCritter != null)
			{
				onDepositCritter(critterData, lastGrabbedPlayer);
			}
			CrittersActor crittersActor = crittersPawn;
			bool flag = false;
			Vector3 zero = Vector3.zero;
			crittersActor.Released(flag, default(Quaternion), zero, default(Vector3), default(Vector3));
			crittersPawn.gameObject.SetActive(false);
			this.depositAudio.GTPlayOneShot(this.depositCritterSound, isLocalDeposit ? 1f : 0.5f);
		}
		else
		{
			this.depositAudio.GTPlayOneShot(this.depositEmptySound, isLocalDeposit ? 1f : 0.5f);
		}
		this.currentCage.transform.position = Vector3.zero;
		this.currentCage.gameObject.SetActive(false);
		this.currentCage = null;
		transition = 0f;
		while (transition < this.returnDuration)
		{
			transition += Time.deltaTime;
			this.attachPoint.transform.localPosition = Vector3.Lerp(this.depositEndLocation, this.depositStartLocation, Mathf.Min(transition / this.returnDuration, 1f));
			yield return null;
		}
		this.isHandlingDeposit = false;
		yield break;
	}

	// Token: 0x060001CE RID: 462 RVA: 0x0000AD78 File Offset: 0x00008F78
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.TransformPoint(this.depositStartLocation), 0.1f);
		Gizmos.DrawLine(base.transform.TransformPoint(this.depositStartLocation), base.transform.TransformPoint(this.depositEndLocation));
		Gizmos.DrawWireSphere(base.transform.TransformPoint(this.depositEndLocation), 0.1f);
	}

	// Token: 0x040001FA RID: 506
	private bool isHandlingDeposit;

	// Token: 0x040001FB RID: 507
	public Vector3 depositStartLocation;

	// Token: 0x040001FC RID: 508
	public Vector3 depositEndLocation;

	// Token: 0x040001FD RID: 509
	public float submitDuration = 0.5f;

	// Token: 0x040001FE RID: 510
	public float returnDuration = 1f;

	// Token: 0x040001FF RID: 511
	public AudioSource depositAudio;

	// Token: 0x04000200 RID: 512
	public AudioClip depositStartSound;

	// Token: 0x04000201 RID: 513
	public AudioClip depositEmptySound;

	// Token: 0x04000202 RID: 514
	public AudioClip depositCritterSound;

	// Token: 0x04000203 RID: 515
	private CrittersActor currentCage;
}
