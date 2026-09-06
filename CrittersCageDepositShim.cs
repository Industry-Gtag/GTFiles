using System;
using UnityEngine;

// Token: 0x0200005D RID: 93
public class CrittersCageDepositShim : MonoBehaviour
{
	// Token: 0x060001D6 RID: 470 RVA: 0x0000B0DC File Offset: 0x000092DC
	[ContextMenu("Copy Deposit Data To Shim")]
	private CrittersCageDeposit CopySpawnerDataInPrefab()
	{
		CrittersCageDeposit component = base.gameObject.GetComponent<CrittersCageDeposit>();
		this.cageBoxCollider = (BoxCollider)component.gameObject.GetComponent<Collider>();
		this.type = component.actorType;
		this.disableGrabOnAttach = component.disableGrabOnAttach;
		this.allowMultiAttach = component.allowMultiAttach;
		this.snapOnAttach = component.snapOnAttach;
		this.startLocation = component.depositStartLocation;
		this.endLocation = component.depositEndLocation;
		this.submitDuration = component.submitDuration;
		this.returnDuration = component.returnDuration;
		this.depositAudio = component.depositAudio;
		this.depositStartSound = component.depositStartSound;
		this.depositEmptySound = component.depositEmptySound;
		this.depositCritterSound = component.depositCritterSound;
		this.attachPointTransform = component.GetComponentInChildren<CrittersActor>().transform;
		this.visiblePlatformTransform = this.attachPointTransform.transform.GetChild(0).transform;
		return component;
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x0000B1CC File Offset: 0x000093CC
	[ContextMenu("Replace Deposit With Shim")]
	private void ReplaceSpawnerWithShim()
	{
		CrittersCageDeposit crittersCageDeposit = this.CopySpawnerDataInPrefab();
		if (crittersCageDeposit.attachPoint.GetComponent<Rigidbody>() != null)
		{
			Object.DestroyImmediate(crittersCageDeposit.attachPoint.GetComponent<Rigidbody>());
		}
		Object.DestroyImmediate(crittersCageDeposit.attachPoint);
		Object.DestroyImmediate(crittersCageDeposit);
	}

	// Token: 0x0400020D RID: 525
	public BoxCollider cageBoxCollider;

	// Token: 0x0400020E RID: 526
	public CrittersActor.CrittersActorType type;

	// Token: 0x0400020F RID: 527
	public bool disableGrabOnAttach;

	// Token: 0x04000210 RID: 528
	public bool allowMultiAttach;

	// Token: 0x04000211 RID: 529
	public bool snapOnAttach;

	// Token: 0x04000212 RID: 530
	public Vector3 startLocation;

	// Token: 0x04000213 RID: 531
	public Vector3 endLocation;

	// Token: 0x04000214 RID: 532
	public float submitDuration;

	// Token: 0x04000215 RID: 533
	public float returnDuration;

	// Token: 0x04000216 RID: 534
	public AudioSource depositAudio;

	// Token: 0x04000217 RID: 535
	public AudioClip depositStartSound;

	// Token: 0x04000218 RID: 536
	public AudioClip depositEmptySound;

	// Token: 0x04000219 RID: 537
	public AudioClip depositCritterSound;

	// Token: 0x0400021A RID: 538
	public Transform attachPointTransform;

	// Token: 0x0400021B RID: 539
	public Transform visiblePlatformTransform;
}
