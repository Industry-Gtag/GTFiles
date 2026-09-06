using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000DFF RID: 3583
[RequireComponent(typeof(CompositeTriggerEvents))]
public class VRRigCollection : MonoBehaviour
{
	// Token: 0x17000851 RID: 2129
	// (get) Token: 0x060057D0 RID: 22480 RVA: 0x001C9D2A File Offset: 0x001C7F2A
	public List<RigContainer> Rigs
	{
		get
		{
			return this.containedRigs;
		}
	}

	// Token: 0x060057D1 RID: 22481 RVA: 0x001C9D32 File Offset: 0x001C7F32
	private void OnEnable()
	{
		this.collisionTriggerEvents.CompositeTriggerEnter += this.OnRigTriggerEnter;
		this.collisionTriggerEvents.CompositeTriggerExit += this.OnRigTriggerExit;
	}

	// Token: 0x060057D2 RID: 22482 RVA: 0x001C9D64 File Offset: 0x001C7F64
	private void OnDisable()
	{
		for (int i = this.containedRigs.Count - 1; i >= 0; i--)
		{
			this.RigDisabled(this.containedRigs[i]);
		}
		this.collisionTriggerEvents.CompositeTriggerEnter -= this.OnRigTriggerEnter;
		this.collisionTriggerEvents.CompositeTriggerExit -= this.OnRigTriggerExit;
	}

	// Token: 0x060057D3 RID: 22483 RVA: 0x001C9DCC File Offset: 0x001C7FCC
	private void OnRigTriggerEnter(Collider other)
	{
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		RigContainer rigContainer;
		if (attachedRigidbody == null || !attachedRigidbody.TryGetComponent<RigContainer>(out rigContainer) || other != rigContainer.HeadCollider || this.containedRigs.Contains(rigContainer))
		{
			return;
		}
		rigContainer.RigEvents.disableEvent += this.RigDisabled;
		this.containedRigs.Add(rigContainer);
		Action<RigContainer> action = this.playerEnteredCollection;
		if (action == null)
		{
			return;
		}
		action(rigContainer);
	}

	// Token: 0x060057D4 RID: 22484 RVA: 0x001C9E50 File Offset: 0x001C8050
	private void OnRigTriggerExit(Collider other)
	{
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		RigContainer rigContainer;
		if (attachedRigidbody == null || !attachedRigidbody.TryGetComponent<RigContainer>(out rigContainer) || other != rigContainer.HeadCollider || !this.containedRigs.Contains(rigContainer))
		{
			return;
		}
		rigContainer.RigEvents.disableEvent -= this.RigDisabled;
		this.containedRigs.Remove(rigContainer);
		Action<RigContainer> action = this.playerLeftCollection;
		if (action == null)
		{
			return;
		}
		action(rigContainer);
	}

	// Token: 0x060057D5 RID: 22485 RVA: 0x001C9ED4 File Offset: 0x001C80D4
	private void RigDisabled(RigContainer rig)
	{
		this.collisionTriggerEvents.ResetColliderMask(rig.HeadCollider);
		this.collisionTriggerEvents.ResetColliderMask(rig.BodyCollider);
	}

	// Token: 0x060057D6 RID: 22486 RVA: 0x001C9EF8 File Offset: 0x001C80F8
	private bool HasRig(VRRig rig)
	{
		for (int i = 0; i < this.containedRigs.Count; i++)
		{
			if (this.containedRigs[i].Rig == rig)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060057D7 RID: 22487 RVA: 0x001C9F38 File Offset: 0x001C8138
	private bool HasRig(NetPlayer player)
	{
		for (int i = 0; i < this.containedRigs.Count; i++)
		{
			if (this.containedRigs[i].Creator == player)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04006830 RID: 26672
	public readonly List<RigContainer> containedRigs = new List<RigContainer>(20);

	// Token: 0x04006831 RID: 26673
	[SerializeField]
	private CompositeTriggerEvents collisionTriggerEvents;

	// Token: 0x04006832 RID: 26674
	public Action<RigContainer> playerEnteredCollection;

	// Token: 0x04006833 RID: 26675
	public Action<RigContainer> playerLeftCollection;
}
