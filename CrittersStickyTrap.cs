using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200007B RID: 123
public class CrittersStickyTrap : CrittersToolThrowable
{
	// Token: 0x060002FA RID: 762 RVA: 0x00011B7B File Offset: 0x0000FD7B
	public override void Initialize()
	{
		base.Initialize();
		this.TogglePhysics(!this.isStuck);
	}

	// Token: 0x060002FB RID: 763 RVA: 0x00011B92 File Offset: 0x0000FD92
	public override void OnDisable()
	{
		base.OnDisable();
		this.isStuck = false;
	}

	// Token: 0x060002FC RID: 764 RVA: 0x00011BA4 File Offset: 0x0000FDA4
	public override void SetImpulse()
	{
		if (this.isOnPlayer || this.isSceneActor)
		{
			return;
		}
		this.localLastImpulse = this.lastImpulseTime;
		base.MoveActor(this.lastImpulsePosition, this.lastImpulseQuaternion, this.parentActorId >= 0, false, true);
		this.TogglePhysics(this.usesRB && this.parentActorId == -1 && !this.isStuck);
		if (!this.rb.isKinematic)
		{
			this.rb.linearVelocity = this.lastImpulseVelocity;
			this.rb.angularVelocity = this.lastImpulseAngularVelocity;
		}
	}

	// Token: 0x060002FD RID: 765 RVA: 0x00011C40 File Offset: 0x0000FE40
	protected override void OnImpact(Vector3 hitPosition, Vector3 hitNormal)
	{
		if (CrittersManager.instance.LocalAuthority())
		{
			if (this.stickOnImpact)
			{
				this.rb.isKinematic = true;
				this.isStuck = true;
				this.updatedSinceLastFrame = true;
				base.UpdateImpulses(false, true);
			}
			CrittersStickyGoo crittersStickyGoo = (CrittersStickyGoo)CrittersManager.instance.SpawnActor(CrittersActor.CrittersActorType.StickyGoo, this.subStickyGooIndex);
			if (crittersStickyGoo == null)
			{
				return;
			}
			CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StickyDeployed, this.actorId, base.transform.position, Quaternion.LookRotation(hitNormal));
			Vector3 vector = base.transform.forward;
			vector -= hitNormal * Vector3.Dot(hitNormal, vector);
			crittersStickyGoo.MoveActor(hitPosition, Quaternion.LookRotation(vector, hitNormal), false, true, true);
			crittersStickyGoo.SetImpulseVelocity(Vector3.zero, Vector3.zero);
			base.UpdateImpulses(true, false);
		}
	}

	// Token: 0x060002FE RID: 766 RVA: 0x0000EA5B File Offset: 0x0000CC5B
	protected override void OnImpactCritter(CrittersPawn impactedCritter)
	{
		this.OnImpact(impactedCritter.transform.position, impactedCritter.transform.up);
	}

	// Token: 0x060002FF RID: 767 RVA: 0x00011D19 File Offset: 0x0000FF19
	protected override void OnPickedUp()
	{
		if (this.isStuck)
		{
			this.isStuck = false;
			this.updatedSinceLastFrame = true;
		}
	}

	// Token: 0x06000300 RID: 768 RVA: 0x00011D31 File Offset: 0x0000FF31
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(this.isStuck);
	}

	// Token: 0x06000301 RID: 769 RVA: 0x00011D4C File Offset: 0x0000FF4C
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		bool flag;
		if (!(base.UpdateSpecificActor(stream) & CrittersManager.ValidateDataType<bool>(stream.ReceiveNext(), out flag)))
		{
			return false;
		}
		this.isStuck = flag;
		this.TogglePhysics(!this.isStuck);
		return true;
	}

	// Token: 0x06000302 RID: 770 RVA: 0x00011D89 File Offset: 0x0000FF89
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(this.isStuck);
		return this.TotalActorDataLength();
	}

	// Token: 0x06000303 RID: 771 RVA: 0x0000A230 File Offset: 0x00008430
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 1;
	}

	// Token: 0x06000304 RID: 772 RVA: 0x00011DAC File Offset: 0x0000FFAC
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		bool flag;
		if (!CrittersManager.ValidateDataType<bool>(data[startingIndex], out flag))
		{
			return this.TotalActorDataLength();
		}
		this.isStuck = flag;
		this.TogglePhysics(!this.isStuck);
		return this.TotalActorDataLength();
	}

	// Token: 0x04000365 RID: 869
	[Header("Sticky Trap")]
	public bool stickOnImpact = true;

	// Token: 0x04000366 RID: 870
	public int subStickyGooIndex = -1;

	// Token: 0x04000367 RID: 871
	private bool isStuck;
}
