using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200005A RID: 90
public class CrittersCage : CrittersActor
{
	// Token: 0x17000023 RID: 35
	// (get) Token: 0x060001B7 RID: 439 RVA: 0x0000AA40 File Offset: 0x00008C40
	public Vector3 critterScale
	{
		get
		{
			if (this.subObjectIndex < this.critterScales.Length && this.subObjectIndex >= 0)
			{
				return this.critterScales[this.subObjectIndex];
			}
			return Vector3.one;
		}
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x060001B8 RID: 440 RVA: 0x0000AA72 File Offset: 0x00008C72
	public bool CanCatch
	{
		get
		{
			return this.heldByPlayer && !this.hasCritter && !this.inReleasingPosition && this._releaseCooldownEnd <= Time.time;
		}
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x0000AA9E File Offset: 0x00008C9E
	public void SetHasCritter(bool value)
	{
		if (this.hasCritter != value && !value)
		{
			this._releaseCooldownEnd = Time.time + this.releaseCooldown;
		}
		this.hasCritter = value;
		this.UpdateCageVisuals();
	}

	// Token: 0x060001BA RID: 442 RVA: 0x0000AACB File Offset: 0x00008CCB
	public override void Initialize()
	{
		base.Initialize();
		this.hasCritter = false;
		this.heldByPlayer = false;
		this.inReleasingPosition = false;
		this.SetLidActive(true, false);
	}

	// Token: 0x060001BB RID: 443 RVA: 0x0000AAF0 File Offset: 0x00008CF0
	private void UpdateCageVisuals()
	{
		this.SetLidActive(!this.heldByPlayer || this.hasCritter, true);
	}

	// Token: 0x060001BC RID: 444 RVA: 0x0000AB0C File Offset: 0x00008D0C
	private void SetLidActive(bool active, bool playAudio = true)
	{
		if (active != this._lidActive && playAudio)
		{
			this.sound.GTPlayOneShot(active ? this.openSound : this.closeSound, 1f);
		}
		this.lid.SetActive(active);
		this._lidActive = active;
	}

	// Token: 0x060001BD RID: 445 RVA: 0x0000AB5D File Offset: 0x00008D5D
	protected override void RemoteGrabbedBy(CrittersActor grabbingActor)
	{
		base.RemoteGrabbedBy(grabbingActor);
		this.heldByPlayer = grabbingActor.isOnPlayer;
		this.UpdateCageVisuals();
	}

	// Token: 0x060001BE RID: 446 RVA: 0x0000AB78 File Offset: 0x00008D78
	public override void GrabbedBy(CrittersActor grabbingActor, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
	{
		base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
		this.heldByPlayer = grabbingActor.isOnPlayer;
		this.UpdateCageVisuals();
	}

	// Token: 0x060001BF RID: 447 RVA: 0x0000AB99 File Offset: 0x00008D99
	public override void Released(bool keepWorldPosition, Quaternion rotation = default(Quaternion), Vector3 position = default(Vector3), Vector3 impulseVelocity = default(Vector3), Vector3 impulseAngularVelocity = default(Vector3))
	{
		base.Released(keepWorldPosition, rotation, position, impulseVelocity, impulseAngularVelocity);
		this.heldByPlayer = false;
		this.UpdateCageVisuals();
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x0000ABB5 File Offset: 0x00008DB5
	protected override void HandleRemoteReleased()
	{
		base.HandleRemoteReleased();
		this.heldByPlayer = false;
		this.UpdateCageVisuals();
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x0000ABCA File Offset: 0x00008DCA
	public override bool ShouldDespawn()
	{
		return base.ShouldDespawn() && !this.hasCritter;
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x0000ABDF File Offset: 0x00008DDF
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(this.hasCritter);
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0000ABFC File Offset: 0x00008DFC
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		if (!base.UpdateSpecificActor(stream))
		{
			return false;
		}
		bool flag;
		if (!CrittersManager.ValidateDataType<bool>(stream.ReceiveNext(), out flag))
		{
			return false;
		}
		this.SetHasCritter(flag);
		return true;
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x0000AC2D File Offset: 0x00008E2D
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(this.hasCritter);
		return this.TotalActorDataLength();
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x0000A230 File Offset: 0x00008430
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 1;
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x0000AC50 File Offset: 0x00008E50
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		bool flag;
		if (!CrittersManager.ValidateDataType<bool>(data[startingIndex], out flag))
		{
			return this.TotalActorDataLength();
		}
		this.SetHasCritter(flag);
		return this.TotalActorDataLength();
	}

	// Token: 0x040001EC RID: 492
	public Transform grabPosition;

	// Token: 0x040001ED RID: 493
	public Transform cagePosition;

	// Token: 0x040001EE RID: 494
	public float grabDistance;

	// Token: 0x040001EF RID: 495
	[SerializeField]
	private Vector3[] critterScales = new Vector3[] { Vector3.one };

	// Token: 0x040001F0 RID: 496
	[SerializeField]
	private float releaseCooldown = 0.25f;

	// Token: 0x040001F1 RID: 497
	[SerializeField]
	private AudioSource sound;

	// Token: 0x040001F2 RID: 498
	[SerializeField]
	private AudioClip openSound;

	// Token: 0x040001F3 RID: 499
	[SerializeField]
	private AudioClip closeSound;

	// Token: 0x040001F4 RID: 500
	public GameObject lid;

	// Token: 0x040001F5 RID: 501
	[NonSerialized]
	public bool heldByPlayer;

	// Token: 0x040001F6 RID: 502
	[NonSerialized]
	private bool hasCritter;

	// Token: 0x040001F7 RID: 503
	[NonSerialized]
	public bool inReleasingPosition;

	// Token: 0x040001F8 RID: 504
	private float _releaseCooldownEnd;

	// Token: 0x040001F9 RID: 505
	private bool _lidActive;
}
