using System;
using UnityEngine;

// Token: 0x020005FB RID: 1531
public class GameBall : MonoBehaviour
{
	// Token: 0x17000402 RID: 1026
	// (get) Token: 0x06002613 RID: 9747 RVA: 0x000C9683 File Offset: 0x000C7883
	public bool IsLaunched
	{
		get
		{
			return this._launched;
		}
	}

	// Token: 0x06002614 RID: 9748 RVA: 0x000C968C File Offset: 0x000C788C
	private void Awake()
	{
		this.id = GameBallId.Invalid;
		if (this.rigidBody == null)
		{
			this.rigidBody = base.GetComponent<Rigidbody>();
		}
		if (this.collider == null)
		{
			this.collider = base.GetComponent<Collider>();
		}
		if (this.disc && this.rigidBody != null)
		{
			this.rigidBody.maxAngularVelocity = 28f;
		}
		this.heldByActorNumber = -1;
		this.lastHeldByTeamId = -1;
		this.onlyGrabTeamId = -1;
		this._monkeBall = base.GetComponent<MonkeBall>();
	}

	// Token: 0x06002615 RID: 9749 RVA: 0x000C9720 File Offset: 0x000C7920
	private void FixedUpdate()
	{
		if (this.rigidBody == null)
		{
			return;
		}
		if (this._launched)
		{
			this._launchedTimer += Time.fixedDeltaTime;
			if (this.collider.isTrigger && this._launchedTimer > 1f && this.rigidBody.linearVelocity.y <= 0f)
			{
				this._launched = false;
				this.collider.isTrigger = false;
			}
		}
		Vector3 vector = -Physics.gravity * (1f - this.gravityMult);
		this.rigidBody.AddForce(vector * this.rigidBody.mass, ForceMode.Force);
		this._catchSoundDecay -= Time.deltaTime;
	}

	// Token: 0x06002616 RID: 9750 RVA: 0x000C97E5 File Offset: 0x000C79E5
	public void WasLaunched()
	{
		this._launched = true;
		this.collider.isTrigger = true;
		this._launchedTimer = 0f;
	}

	// Token: 0x06002617 RID: 9751 RVA: 0x000C9805 File Offset: 0x000C7A05
	public Vector3 GetVelocity()
	{
		if (this.rigidBody == null)
		{
			return Vector3.zero;
		}
		return this.rigidBody.linearVelocity;
	}

	// Token: 0x06002618 RID: 9752 RVA: 0x000C9826 File Offset: 0x000C7A26
	public void SetVelocity(Vector3 velocity)
	{
		this.rigidBody.linearVelocity = velocity;
	}

	// Token: 0x06002619 RID: 9753 RVA: 0x000C9834 File Offset: 0x000C7A34
	public void PlayCatchFx()
	{
		if (this.audioSource != null && this._catchSoundDecay <= 0f && this.audioSource.isActiveAndEnabled)
		{
			this.audioSource.clip = this.catchSound;
			this.audioSource.volume = this.catchSoundVolume;
			this.audioSource.GTPlay();
			this._catchSoundDecay = 0.1f;
		}
	}

	// Token: 0x0600261A RID: 9754 RVA: 0x000C98A4 File Offset: 0x000C7AA4
	public void PlayThrowFx()
	{
		if (this.audioSource != null && this.audioSource.isActiveAndEnabled)
		{
			this.audioSource.clip = this.throwSound;
			this.audioSource.volume = this.throwSoundVolume;
			this.audioSource.GTPlay();
		}
	}

	// Token: 0x0600261B RID: 9755 RVA: 0x000C98FC File Offset: 0x000C7AFC
	public void PlayBounceFX()
	{
		if (this.audioSource != null && this.audioSource.isActiveAndEnabled)
		{
			this.audioSource.clip = this.groundSound;
			this.audioSource.volume = this.groundSoundVolume;
			this.audioSource.GTPlay();
		}
	}

	// Token: 0x0600261C RID: 9756 RVA: 0x000C9951 File Offset: 0x000C7B51
	public void SetHeldByTeamId(int teamId)
	{
		this.lastHeldByTeamId = teamId;
	}

	// Token: 0x0600261D RID: 9757 RVA: 0x000C995A File Offset: 0x000C7B5A
	private bool IsGamePlayer(Collider collider)
	{
		return GameBallPlayer.GetGamePlayer(collider, false) != null;
	}

	// Token: 0x0600261E RID: 9758 RVA: 0x000C9969 File Offset: 0x000C7B69
	public void SetVisualOffset(bool detach)
	{
		if (this._monkeBall != null)
		{
			this._monkeBall.SetVisualOffset(detach);
		}
	}

	// Token: 0x040031A1 RID: 12705
	public GameBallId id;

	// Token: 0x040031A2 RID: 12706
	public float gravityMult = 1f;

	// Token: 0x040031A3 RID: 12707
	public bool disc;

	// Token: 0x040031A4 RID: 12708
	public Vector3 localDiscUp;

	// Token: 0x040031A5 RID: 12709
	public AudioSource audioSource;

	// Token: 0x040031A6 RID: 12710
	public AudioClip catchSound;

	// Token: 0x040031A7 RID: 12711
	public float catchSoundVolume;

	// Token: 0x040031A8 RID: 12712
	private float _catchSoundDecay;

	// Token: 0x040031A9 RID: 12713
	public AudioClip throwSound;

	// Token: 0x040031AA RID: 12714
	public float throwSoundVolume;

	// Token: 0x040031AB RID: 12715
	public AudioClip groundSound;

	// Token: 0x040031AC RID: 12716
	public float groundSoundVolume;

	// Token: 0x040031AD RID: 12717
	[SerializeField]
	private Rigidbody rigidBody;

	// Token: 0x040031AE RID: 12718
	[SerializeField]
	private Collider collider;

	// Token: 0x040031AF RID: 12719
	public int heldByActorNumber;

	// Token: 0x040031B0 RID: 12720
	public int lastHeldByActorNumber;

	// Token: 0x040031B1 RID: 12721
	public int lastHeldByTeamId;

	// Token: 0x040031B2 RID: 12722
	public int onlyGrabTeamId;

	// Token: 0x040031B3 RID: 12723
	private bool _launched;

	// Token: 0x040031B4 RID: 12724
	private float _launchedTimer;

	// Token: 0x040031B5 RID: 12725
	public MonkeBall _monkeBall;
}
