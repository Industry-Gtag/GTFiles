using System;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200020C RID: 524
public class PartyHornTransferableObject : TransferrableObject
{
	// Token: 0x06000DDF RID: 3551 RVA: 0x0004C1FA File Offset: 0x0004A3FA
	internal override void OnEnable()
	{
		base.OnEnable();
		this.InitToDefault();
	}

	// Token: 0x06000DE0 RID: 3552 RVA: 0x0004C208 File Offset: 0x0004A408
	internal override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x0004C210 File Offset: 0x0004A410
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x0004C220 File Offset: 0x0004A420
	protected Vector3 CalcMouthPiecePos()
	{
		if (!this.mouthPiece)
		{
			return base.transform.position + this.mouthPieceZOffset * base.transform.forward;
		}
		return this.mouthPiece.position;
	}

	// Token: 0x06000DE3 RID: 3555 RVA: 0x0004C26C File Offset: 0x0004A46C
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!base.InHand())
		{
			return;
		}
		if (this.itemState != TransferrableObject.ItemStates.State0)
		{
			return;
		}
		if (!GorillaParent.hasInstance)
		{
			return;
		}
		Transform transform = base.transform;
		Vector3 vector = this.CalcMouthPiecePos();
		float num = this.mouthPieceRadius * this.mouthPieceRadius * GTPlayer.Instance.scale * GTPlayer.Instance.scale;
		bool flag = (GorillaTagger.Instance.offlineVRRig.GetMouthPosition() - vector).sqrMagnitude < num;
		if (this.soundActivated && PhotonNetwork.InRoom)
		{
			bool flag2;
			if (flag)
			{
				GorillaTagger instance = GorillaTagger.Instance;
				if (instance == null)
				{
					flag2 = false;
				}
				else
				{
					Recorder myRecorder = instance.myRecorder;
					bool? flag3 = ((myRecorder != null) ? new bool?(myRecorder.IsCurrentlyTransmitting) : null);
					bool flag4 = true;
					flag2 = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
				}
			}
			else
			{
				flag2 = false;
			}
			flag = flag2;
		}
		for (int i = 0; i < VRRigCache.ActiveRigContainers.Count; i++)
		{
			VRRig rig = VRRigCache.ActiveRigContainers[i].Rig;
			if (flag)
			{
				break;
			}
			flag = (rig.GetMouthPosition() - vector).sqrMagnitude < num;
			if (this.soundActivated)
			{
				bool flag5;
				if (flag)
				{
					RigContainer rigContainer = rig.rigContainer;
					if (rigContainer == null)
					{
						flag5 = false;
					}
					else
					{
						PhotonVoiceView voice = rigContainer.Voice;
						bool? flag3 = ((voice != null) ? new bool?(voice.IsSpeaking) : null);
						bool flag4 = true;
						flag5 = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
					}
				}
				else
				{
					flag5 = false;
				}
				flag = flag5;
			}
		}
		this.itemState = (flag ? TransferrableObject.ItemStates.State1 : this.itemState);
	}

	// Token: 0x06000DE4 RID: 3556 RVA: 0x0004C400 File Offset: 0x0004A600
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (TransferrableObject.ItemStates.State1 != this.itemState)
		{
			return;
		}
		if (!this.localWasActivated)
		{
			if (this.effectsGameObject)
			{
				this.effectsGameObject.SetActive(true);
			}
			this.cooldownRemaining = this.cooldown;
			this.localWasActivated = true;
			UnityEvent onCooldownStart = this.OnCooldownStart;
			if (onCooldownStart != null)
			{
				onCooldownStart.Invoke();
			}
		}
		this.cooldownRemaining -= Time.deltaTime;
		if (this.cooldownRemaining <= 0f)
		{
			this.InitToDefault();
		}
	}

	// Token: 0x06000DE5 RID: 3557 RVA: 0x0004C488 File Offset: 0x0004A688
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.effectsGameObject)
		{
			this.effectsGameObject.SetActive(false);
		}
		this.cooldownRemaining = this.cooldown;
		this.localWasActivated = false;
		UnityEvent onCooldownReset = this.OnCooldownReset;
		if (onCooldownReset == null)
		{
			return;
		}
		onCooldownReset.Invoke();
	}

	// Token: 0x04001082 RID: 4226
	[Tooltip("This GameObject will activate when held to any gorilla's mouth.")]
	public GameObject effectsGameObject;

	// Token: 0x04001083 RID: 4227
	public float cooldown = 2f;

	// Token: 0x04001084 RID: 4228
	public float mouthPieceZOffset = -0.18f;

	// Token: 0x04001085 RID: 4229
	public float mouthPieceRadius = 0.05f;

	// Token: 0x04001086 RID: 4230
	public Transform mouthPiece;

	// Token: 0x04001087 RID: 4231
	public bool soundActivated;

	// Token: 0x04001088 RID: 4232
	public UnityEvent OnCooldownStart;

	// Token: 0x04001089 RID: 4233
	public UnityEvent OnCooldownReset;

	// Token: 0x0400108A RID: 4234
	private float cooldownRemaining;

	// Token: 0x0400108B RID: 4235
	private PartyHornTransferableObject.PartyHornState partyHornStateLastFrame;

	// Token: 0x0400108C RID: 4236
	private bool localWasActivated;

	// Token: 0x0200020D RID: 525
	private enum PartyHornState
	{
		// Token: 0x0400108E RID: 4238
		None = 1,
		// Token: 0x0400108F RID: 4239
		CoolingDown
	}
}
