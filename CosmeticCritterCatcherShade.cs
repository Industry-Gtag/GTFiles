using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000C5 RID: 197
public class CosmeticCritterCatcherShade : CosmeticCritterCatcher
{
	// Token: 0x1700005D RID: 93
	// (get) Token: 0x060004CF RID: 1231 RVA: 0x0001ADA0 File Offset: 0x00018FA0
	// (set) Token: 0x060004D0 RID: 1232 RVA: 0x0001ADA8 File Offset: 0x00018FA8
	public Vector3 LastTargetPosition { get; private set; }

	// Token: 0x060004D1 RID: 1233 RVA: 0x0001ADB1 File Offset: 0x00018FB1
	public float GetActionTimeFrac()
	{
		return this.targetHoldTime / this.maxHoldTime;
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x0001ADC0 File Offset: 0x00018FC0
	protected override CallLimiter CreateCallLimiter()
	{
		return new CallLimiter(10, 0.25f, 0.5f);
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x0001ADD4 File Offset: 0x00018FD4
	public override CosmeticCritterAction GetLocalCatchAction(CosmeticCritter critter)
	{
		if (this.heartbeatCooldown > 0.5f || (this.currentTarget != null && this.currentTarget != critter))
		{
			return CosmeticCritterAction.None;
		}
		if (critter is CosmeticCritterShadeFleeing && this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.LOCKED, 0f))
		{
			if (this.targetHoldTime >= this.minSecondsLockedToCatch && (critter.transform.position - this.catchOrigin.position).sqrMagnitude <= this.catchRadius * this.catchRadius)
			{
				return CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn;
			}
			return CosmeticCritterAction.RPC | CosmeticCritterAction.ShadeHeartbeat;
		}
		else
		{
			if (!(critter is CosmeticCritterShadeHidden) || !this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.TRACKING, 0f))
			{
				return CosmeticCritterAction.None;
			}
			if (this.targetHoldTime >= this.secondsToReveal)
			{
				return CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn | CosmeticCritterAction.SpawnLinked;
			}
			return CosmeticCritterAction.RPC | CosmeticCritterAction.ShadeHeartbeat;
		}
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x0001AEA0 File Offset: 0x000190A0
	public override bool ValidateRemoteCatchAction(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		if (!base.ValidateRemoteCatchAction(critter, catchAction, serverTime))
		{
			return false;
		}
		if (critter is CosmeticCritterShadeFleeing)
		{
			if ((catchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None && (critter.transform.position - this.catchOrigin.position).sqrMagnitude <= this.catchRadius * this.catchRadius + 1f && this.targetHoldTime >= this.minSecondsLockedToCatch * 0.8f)
			{
				return true;
			}
			if ((catchAction & CosmeticCritterAction.ShadeHeartbeat) != CosmeticCritterAction.None && this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.LOCKED, 2f))
			{
				return true;
			}
		}
		else if (critter is CosmeticCritterShadeHidden)
		{
			if ((catchAction & (CosmeticCritterAction.Despawn | CosmeticCritterAction.SpawnLinked)) != CosmeticCritterAction.None && this.targetHoldTime >= this.secondsToReveal * 0.8f)
			{
				return true;
			}
			if ((catchAction & CosmeticCritterAction.ShadeHeartbeat) != CosmeticCritterAction.None && this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.TRACKING, 2f))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x0001AF74 File Offset: 0x00019174
	public override void OnCatch(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		this.currentTarget = critter;
		float num = (PhotonNetwork.InRoom ? ((float)(PhotonNetwork.Time - serverTime)) : 0f);
		this.heartbeatCooldown = 1f + num;
		this.targetHoldTime += num;
		if (!(critter is CosmeticCritterShadeFleeing))
		{
			if (critter is CosmeticCritterShadeHidden)
			{
				this.maxHoldTime = this.secondsToReveal;
				if ((catchAction & (CosmeticCritterAction.Despawn | CosmeticCritterAction.SpawnLinked)) != CosmeticCritterAction.None)
				{
					(this.optionalLinkedSpawner as CosmeticCritterSpawnerShadeFleeing).SetSpawnPosition(critter.transform.position);
					this.currentTarget = null;
					this.targetHoldTime = 0f;
				}
			}
			return;
		}
		this.maxHoldTime = this.minSecondsLockedToCatch;
		if ((catchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None)
		{
			this.shadeRevealer.ShadeCaught();
			this.currentTarget = null;
			this.targetHoldTime = 0f;
			return;
		}
		CosmeticCritterAction cosmeticCritterAction = catchAction & CosmeticCritterAction.ShadeHeartbeat;
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x0001B03E File Offset: 0x0001923E
	protected override void Awake()
	{
		base.Awake();
		this.shadeRevealer = this.transferrableObject as ShadeRevealer;
		this.maxHoldTime = Mathf.Max(this.secondsToReveal, this.minSecondsLockedToCatch);
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x0001B070 File Offset: 0x00019270
	protected void LateUpdate()
	{
		if (this.heartbeatCooldown > 0f)
		{
			this.heartbeatCooldown -= Time.deltaTime;
			if (this.heartbeatCooldown < 0f)
			{
				this.heartbeatCooldown = 0f;
				this.currentTarget = null;
				return;
			}
			this.targetHoldTime = Mathf.Min(this.targetHoldTime + Time.deltaTime, this.maxHoldTime);
			if (this.currentTarget is CosmeticCritterShadeFleeing)
			{
				if (!base.IsLocal || this.heartbeatCooldown > 0.4f)
				{
					this.shadeRevealer.SetBestBeamState(ShadeRevealer.State.LOCKED);
				}
				Vector3 normalized = (this.catchOrigin.position - this.currentTarget.transform.position).normalized;
				(this.currentTarget as CosmeticCritterShadeFleeing).pullVector += this.vacuumSpeed * Time.deltaTime * normalized;
				return;
			}
			if (this.currentTarget is CosmeticCritterShadeHidden && (!base.IsLocal || this.heartbeatCooldown > 0.4f))
			{
				this.shadeRevealer.SetBestBeamState(ShadeRevealer.State.TRACKING);
				return;
			}
		}
		else if (this.targetHoldTime > 0f)
		{
			this.targetHoldTime = Mathf.Max(this.targetHoldTime - Time.deltaTime, 0f);
		}
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x0001B1B9 File Offset: 0x000193B9
	protected override void OnEnable()
	{
		base.OnEnable();
		this.currentTarget = null;
		this.targetHoldTime = 0f;
		this.heartbeatCooldown = 1f;
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x0001B1DE File Offset: 0x000193DE
	protected override void OnDisable()
	{
		base.OnDisable();
		this.currentTarget = null;
		this.targetHoldTime = 0f;
		this.heartbeatCooldown = 1f;
	}

	// Token: 0x04000554 RID: 1364
	[SerializeField]
	private float secondsToReveal = 1f;

	// Token: 0x04000555 RID: 1365
	[SerializeField]
	private float minSecondsLockedToCatch = 1f;

	// Token: 0x04000556 RID: 1366
	[SerializeField]
	private Transform catchOrigin;

	// Token: 0x04000557 RID: 1367
	[SerializeField]
	private float catchRadius = 1f;

	// Token: 0x04000558 RID: 1368
	[SerializeField]
	private float vacuumSpeed = 3f;

	// Token: 0x04000559 RID: 1369
	private ShadeRevealer shadeRevealer;

	// Token: 0x0400055A RID: 1370
	private CosmeticCritter currentTarget;

	// Token: 0x0400055B RID: 1371
	private float targetHoldTime;

	// Token: 0x0400055C RID: 1372
	private float maxHoldTime;

	// Token: 0x0400055E RID: 1374
	private const float HEARTBEAT_DELAY = 1f;

	// Token: 0x0400055F RID: 1375
	private float heartbeatCooldown;
}
