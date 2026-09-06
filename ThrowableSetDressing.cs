using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E53 RID: 3667
[RequireComponent(typeof(NetworkView))]
public class ThrowableSetDressing : TransferrableObject
{
	// Token: 0x17000885 RID: 2181
	// (get) Token: 0x0600598D RID: 22925 RVA: 0x001D1057 File Offset: 0x001CF257
	// (set) Token: 0x0600598E RID: 22926 RVA: 0x001D105F File Offset: 0x001CF25F
	public bool inInitialPose { get; private set; } = true;

	// Token: 0x0600598F RID: 22927 RVA: 0x001D1068 File Offset: 0x001CF268
	public override bool ShouldBeKinematic()
	{
		return this.inInitialPose || base.ShouldBeKinematic();
	}

	// Token: 0x06005990 RID: 22928 RVA: 0x001D107A File Offset: 0x001CF27A
	protected override void Awake()
	{
		base.Awake();
		this.netView = base.GetComponent<NetworkView>();
	}

	// Token: 0x06005991 RID: 22929 RVA: 0x001D108E File Offset: 0x001CF28E
	protected override void Start()
	{
		base.Start();
		this.respawnAtPos = base.transform.position;
		this.respawnAtRot = base.transform.rotation;
		this.currentState = TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06005992 RID: 22930 RVA: 0x001D10C3 File Offset: 0x001CF2C3
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		this.inInitialPose = false;
		this.StopRespawnTimer();
	}

	// Token: 0x06005993 RID: 22931 RVA: 0x001D10DA File Offset: 0x001CF2DA
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.StartRespawnTimer(-1f);
		return true;
	}

	// Token: 0x06005994 RID: 22932 RVA: 0x001D10F4 File Offset: 0x001CF2F4
	public override void DropItem()
	{
		base.DropItem();
		this.StartRespawnTimer(-1f);
	}

	// Token: 0x06005995 RID: 22933 RVA: 0x001D1107 File Offset: 0x001CF307
	private void StopRespawnTimer()
	{
		if (this.respawnTimer != null)
		{
			base.StopCoroutine(this.respawnTimer);
			this.respawnTimer = null;
		}
	}

	// Token: 0x06005996 RID: 22934 RVA: 0x001D1124 File Offset: 0x001CF324
	public void SetWillTeleport()
	{
		this.worldShareableInstance.SetWillTeleport();
	}

	// Token: 0x06005997 RID: 22935 RVA: 0x001D1134 File Offset: 0x001CF334
	public void StartRespawnTimer(float overrideTimer = -1f)
	{
		float num = ((overrideTimer != -1f) ? overrideTimer : this.respawnTimerDuration);
		this.StopRespawnTimer();
		if (this.respawnTimerDuration != 0f && (!this.netView.IsValid || this.netView.IsMine))
		{
			this.respawnTimer = base.StartCoroutine(this.RespawnTimerCoroutine(num));
		}
	}

	// Token: 0x06005998 RID: 22936 RVA: 0x001D1193 File Offset: 0x001CF393
	private IEnumerator RespawnTimerCoroutine(float timerDuration)
	{
		yield return new WaitForSeconds(timerDuration);
		if (base.InHand())
		{
			yield break;
		}
		this.SetWillTeleport();
		base.transform.position = this.respawnAtPos;
		base.transform.rotation = this.respawnAtRot;
		this.inInitialPose = true;
		this.rigidbodyInstance.isKinematic = true;
		yield break;
	}

	// Token: 0x040069D3 RID: 27091
	public float respawnTimerDuration;

	// Token: 0x040069D5 RID: 27093
	[Tooltip("set this only if this set dressing is using as an ingredient for the magic cauldron - Halloween")]
	public MagicIngredientType IngredientTypeSO;

	// Token: 0x040069D6 RID: 27094
	private float _respawnTimestamp;

	// Token: 0x040069D7 RID: 27095
	[SerializeField]
	private CapsuleCollider capsuleCollider;

	// Token: 0x040069D8 RID: 27096
	private NetworkView netView;

	// Token: 0x040069D9 RID: 27097
	private Vector3 respawnAtPos;

	// Token: 0x040069DA RID: 27098
	private Quaternion respawnAtRot;

	// Token: 0x040069DB RID: 27099
	private Coroutine respawnTimer;
}
