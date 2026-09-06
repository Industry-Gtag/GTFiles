using System;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000EF RID: 239
public class SIGadgetWing : SIGadget
{
	// Token: 0x060005A7 RID: 1447 RVA: 0x00020840 File Offset: 0x0001EA40
	private void Awake()
	{
		if (this.m_buttonActivatable == null)
		{
			this.m_buttonActivatable = base.GetComponent<GameButtonActivatable>();
		}
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.OnSnapped));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.OnReleased));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.OnUnsnapped));
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x00020903 File Offset: 0x0001EB03
	private void OnGrabbed()
	{
		this._lastWingPos = this.m_wingCenter.transform.position;
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x00020903 File Offset: 0x0001EB03
	private void OnSnapped()
	{
		this._lastWingPos = this.m_wingCenter.transform.position;
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnReleased()
	{
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnUnsnapped()
	{
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x0002091C File Offset: 0x0001EB1C
	protected override void OnUpdateAuthority(float dt)
	{
		Vector3 position = this.m_wingCenter.transform.position;
		SIGadgetWing_EState state = this._state;
		this._state = (this.m_buttonActivatable.CheckInput(0.25f) ? SIGadgetWing_EState.TriggerPressed : SIGadgetWing_EState.Idle);
		if (state != this._state)
		{
			this.gameEntity.RequestState(this.gameEntity.id, (long)this._state);
			this._lastWingPos = position;
		}
		if (this._state != SIGadgetWing_EState.TriggerPressed)
		{
			return;
		}
		Vector3 vector = this._lastWingPos - position;
		Vector3 up = this.m_wingCenter.transform.up;
		float num = Mathf.Max(Vector3.Dot(vector, up), 0f);
		double num2 = PhotonNetwork.Time - (double)GTPlayer.Instance.LastTouchedGroundAtNetworkTime;
		float num3 = Mathf.Lerp(this.m_flapStrength, this.m_flapDecayedStrength, (float)num2 / this.m_decayDuration);
		if (base.IsBlocked(SIExclusionType.AffectsLocalMovement))
		{
			return;
		}
		Vector3 vector2 = up * (num * num3);
		GTPlayer.Instance.AddForce(vector2, ForceMode.Impulse);
		this._lastWingPos = position;
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void OnUpdateRemote(float dt)
	{
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x00020A18 File Offset: 0x0001EC18
	public override void OnEntityStateChange(long prevState, long newState)
	{
		if (newState == prevState || newState < 0L || newState >= 2L)
		{
			return;
		}
		this.m_gtAnimator.SetState(newState);
	}

	// Token: 0x040006D5 RID: 1749
	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	// Token: 0x040006D6 RID: 1750
	[SerializeField]
	private float m_flapStrength;

	// Token: 0x040006D7 RID: 1751
	[SerializeField]
	private float m_flapDecayedStrength;

	// Token: 0x040006D8 RID: 1752
	[SerializeField]
	private float m_decayDuration;

	// Token: 0x040006D9 RID: 1753
	[SerializeField]
	private float m_liftStrength;

	// Token: 0x040006DA RID: 1754
	[SerializeField]
	private float m_liftCap;

	// Token: 0x040006DB RID: 1755
	[SerializeField]
	private Transform m_wingCenter;

	// Token: 0x040006DC RID: 1756
	[SerializeField]
	private GTAnimator m_gtAnimator;

	// Token: 0x040006DD RID: 1757
	private Vector3 _lastWingPos;

	// Token: 0x040006DE RID: 1758
	private SIGadgetWing_EState _state;
}
