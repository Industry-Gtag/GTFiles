using System;
using System.Collections;
using Fusion;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x020004AE RID: 1198
[NetworkBehaviourWeaved(1)]
public class HitTargetNetworkState : NetworkComponent
{
	// Token: 0x06001D2F RID: 7471 RVA: 0x0009E53C File Offset: 0x0009C73C
	protected override void Awake()
	{
		base.Awake();
		this.audioPlayer = base.GetComponent<AudioSource>();
		SlingshotProjectileHitNotifier component = base.GetComponent<SlingshotProjectileHitNotifier>();
		if (component != null)
		{
			component.OnProjectileHit += this.ProjectileHitReciever;
			component.OnProjectileCollisionStay += this.ProjectileHitReciever;
			return;
		}
		Debug.LogError("Needs SlingshotProjectileHitNotifier added to this GameObject to increment score");
	}

	// Token: 0x06001D30 RID: 7472 RVA: 0x0009E59A File Offset: 0x0009C79A
	protected override void Start()
	{
		base.Start();
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
	}

	// Token: 0x06001D31 RID: 7473 RVA: 0x0009E5BD File Offset: 0x0009C7BD
	private void SetInitialState()
	{
		this.networkedScore.Value = 0;
		this.nextHittableTimestamp = 0f;
		this.audioPlayer.GTStop();
	}

	// Token: 0x06001D32 RID: 7474 RVA: 0x0009E5E1 File Offset: 0x0009C7E1
	public void OnLeftRoom()
	{
		this.SetInitialState();
	}

	// Token: 0x06001D33 RID: 7475 RVA: 0x0009E5E9 File Offset: 0x0009C7E9
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		if (Application.isEditor)
		{
			base.StartCoroutine(this.TestPressCheck());
		}
		this.SetInitialState();
	}

	// Token: 0x06001D34 RID: 7476 RVA: 0x0009E611 File Offset: 0x0009C811
	private IEnumerator TestPressCheck()
	{
		for (;;)
		{
			if (this.testPress)
			{
				this.testPress = false;
				this.TargetHit(Vector3.zero, Vector3.one);
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x06001D35 RID: 7477 RVA: 0x0009E620 File Offset: 0x0009C820
	private void ProjectileHitReciever(SlingshotProjectile projectile, Collision collision)
	{
		this.TargetHit(projectile.launchPosition, collision.contacts[0].point);
	}

	// Token: 0x06001D36 RID: 7478 RVA: 0x0009E640 File Offset: 0x0009C840
	public void TargetHit(Vector3 launchPoint, Vector3 impactPoint)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		if (Time.time <= this.nextHittableTimestamp)
		{
			return;
		}
		int num = this.networkedScore.Value;
		if (this.scoreIsDistance)
		{
			int num2 = Mathf.RoundToInt((launchPoint - impactPoint).magnitude * 3.28f);
			if (num2 <= num)
			{
				return;
			}
			num = num2;
		}
		else
		{
			num++;
			if (num >= 1000)
			{
				num = 0;
			}
		}
		if (this.resetAfterDuration > 0f && this.resetCoroutine == null)
		{
			this.resetAtTimestamp = Time.time + this.resetAfterDuration;
			this.resetCoroutine = base.StartCoroutine(this.ResetCo());
		}
		this.PlayAudio(this.networkedScore.Value, num);
		this.networkedScore.Value = num;
		this.nextHittableTimestamp = Time.time + (float)this.hitCooldownTime;
	}

	// Token: 0x17000318 RID: 792
	// (get) Token: 0x06001D37 RID: 7479 RVA: 0x0009E718 File Offset: 0x0009C918
	// (set) Token: 0x06001D38 RID: 7480 RVA: 0x0009E73E File Offset: 0x0009C93E
	[Networked]
	[NetworkedWeaved(0, 1)]
	public unsafe int Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HitTargetNetworkState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return this.Ptr[0];
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HitTargetNetworkState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			this.Ptr[0] = value;
		}
	}

	// Token: 0x06001D39 RID: 7481 RVA: 0x0009E765 File Offset: 0x0009C965
	public override void WriteDataFusion()
	{
		this.Data = this.networkedScore.Value;
	}

	// Token: 0x06001D3A RID: 7482 RVA: 0x0009E778 File Offset: 0x0009C978
	public override void ReadDataFusion()
	{
		int data = this.Data;
		if (data != this.networkedScore.Value)
		{
			this.PlayAudio(this.networkedScore.Value, data);
		}
		this.networkedScore.Value = data;
	}

	// Token: 0x06001D3B RID: 7483 RVA: 0x0009E7B8 File Offset: 0x0009C9B8
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.networkedScore.Value);
	}

	// Token: 0x06001D3C RID: 7484 RVA: 0x0009E7E0 File Offset: 0x0009C9E0
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		if (num != this.networkedScore.Value)
		{
			this.PlayAudio(this.networkedScore.Value, num);
		}
		this.networkedScore.Value = num;
	}

	// Token: 0x06001D3D RID: 7485 RVA: 0x0009E833 File Offset: 0x0009CA33
	public void PlayAudio(int oldScore, int newScore)
	{
		if (oldScore > newScore && !this.scoreIsDistance)
		{
			this.audioPlayer.GTPlayOneShot(this.audioClips[1], 1f);
			return;
		}
		this.audioPlayer.GTPlayOneShot(this.audioClips[0], 1f);
	}

	// Token: 0x06001D3E RID: 7486 RVA: 0x0009E872 File Offset: 0x0009CA72
	private IEnumerator ResetCo()
	{
		while (Time.time < this.resetAtTimestamp)
		{
			yield return new WaitForSeconds(this.resetAtTimestamp - Time.time);
		}
		this.networkedScore.Value = 0;
		this.PlayAudio(this.networkedScore.Value, 0);
		this.resetCoroutine = null;
		yield break;
	}

	// Token: 0x06001D40 RID: 7488 RVA: 0x0009E890 File Offset: 0x0009CA90
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06001D41 RID: 7489 RVA: 0x0009E8A8 File Offset: 0x0009CAA8
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04002774 RID: 10100
	[SerializeField]
	private WatchableIntSO networkedScore;

	// Token: 0x04002775 RID: 10101
	[SerializeField]
	private int hitCooldownTime = 1;

	// Token: 0x04002776 RID: 10102
	[SerializeField]
	private bool testPress;

	// Token: 0x04002777 RID: 10103
	[SerializeField]
	private AudioClip[] audioClips;

	// Token: 0x04002778 RID: 10104
	[SerializeField]
	private bool scoreIsDistance;

	// Token: 0x04002779 RID: 10105
	[SerializeField]
	private float resetAfterDuration;

	// Token: 0x0400277A RID: 10106
	private AudioSource audioPlayer;

	// Token: 0x0400277B RID: 10107
	private float nextHittableTimestamp;

	// Token: 0x0400277C RID: 10108
	private float resetAtTimestamp;

	// Token: 0x0400277D RID: 10109
	private Coroutine resetCoroutine;

	// Token: 0x0400277E RID: 10110
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private int _Data;
}
