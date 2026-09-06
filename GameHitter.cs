using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020006E8 RID: 1768
public class GameHitter : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x06002C79 RID: 11385 RVA: 0x000F032A File Offset: 0x000EE52A
	private void Awake()
	{
		this.components = new List<IGameHitter>(1);
		base.GetComponentsInChildren<IGameHitter>(this.components);
		this.attributes = base.GetComponent<GRAttributes>();
	}

	// Token: 0x06002C7A RID: 11386 RVA: 0x000F0350 File Offset: 0x000EE550
	public void OnEntityInit()
	{
		GRTool component = base.GetComponent<GRTool>();
		if (component != null)
		{
			component.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(component);
		}
	}

	// Token: 0x06002C7B RID: 11387 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002C7C RID: 11388 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002C7D RID: 11389 RVA: 0x000F0386 File Offset: 0x000EE586
	private void OnToolUpgraded(GRTool tool)
	{
		if (this.attributes.HasValueForAttribute(GRAttributeType.KnockbackMultiplier))
		{
			this.knockbackMultiplier = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.KnockbackMultiplier);
		}
	}

	// Token: 0x06002C7E RID: 11390 RVA: 0x000F03AC File Offset: 0x000EE5AC
	public void ApplyHit(GameHitData hitData)
	{
		if (this.hitFx.hitSound != null)
		{
			this.hitFx.hitSound.Play(null);
		}
		if (this.hitFx.hitEffect != null)
		{
			this.hitFx.hitEffect.Stop();
			this.hitFx.hitEffect.Play();
		}
		for (int i = 0; i < this.components.Count; i++)
		{
			this.components[i].OnSuccessfulHit(hitData);
		}
		if (this.gameEntity.IsHeldByLocalPlayer())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, 0.2f);
			GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
			if (gamePlayer != null)
			{
				int num = gamePlayer.FindHandIndex(this.gameEntity.id);
				if (num != -1)
				{
					GTPlayer.Instance.TempFreezeHand(GamePlayer.IsLeftHand(num), 0.15f);
				}
			}
		}
		if (GRNoiseEventManager.instance != null)
		{
			GRNoiseEventManager.instance.AddNoiseEvent(hitData.hitPosition, 1f, 1f);
		}
	}

	// Token: 0x06002C7F RID: 11391 RVA: 0x000F04C0 File Offset: 0x000EE6C0
	public void ApplyHitToPlayer(GRPlayer player, Vector3 hitPosition)
	{
		this.hitFx.hitSound.Play(null);
		if (this.hitFx.hitEffect != null)
		{
			this.hitFx.hitEffect.Play();
		}
		for (int i = 0; i < this.components.Count; i++)
		{
			this.components[i].OnSuccessfulHitPlayer(player, hitPosition);
		}
	}

	// Token: 0x06002C80 RID: 11392 RVA: 0x000F052C File Offset: 0x000EE72C
	private void PlayVibration(float strength, float duration)
	{
		if (!this.gameEntity.IsHeldByLocalPlayer())
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
		if (gamePlayer == null)
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), strength, duration);
	}

	// Token: 0x06002C81 RID: 11393 RVA: 0x000F058C File Offset: 0x000EE78C
	private T GetParentEnemy<T>(Collider collider) where T : MonoBehaviour
	{
		Transform transform = collider.transform;
		while (transform != null)
		{
			T component = transform.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return default(T);
	}

	// Token: 0x06002C82 RID: 11394 RVA: 0x000F05D4 File Offset: 0x000EE7D4
	public int CalcHitAmount(GameHitType hitType, GameHittable hittable, GameEntity hitByEntity)
	{
		int num = 0;
		if (hitByEntity != null)
		{
			GRAttributes component = hitByEntity.GetComponent<GRAttributes>();
			if (component != null)
			{
				switch (hitType)
				{
				case GameHitType.Club:
					num = component.CalculateFinalValueForAttribute(this.damageAttribute);
					break;
				case GameHitType.Flash:
					num = component.CalculateFinalValueForAttribute(this.flashDamageAttribute);
					break;
				case GameHitType.Shield:
					num = component.CalculateFinalValueForAttribute(this.shieldDamageAttribute);
					break;
				}
			}
		}
		return num;
	}

	// Token: 0x06002C83 RID: 11395 RVA: 0x000F063C File Offset: 0x000EE83C
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.hitOnCollision)
		{
			return;
		}
		float num = this.gameEntity.GetVelocity().sqrMagnitude;
		if (this.gameEntity.lastHeldByActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
		{
			return;
		}
		bool flag = false;
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
		if (gamePlayer != null)
		{
			float handSpeed = GamePlayerLocal.instance.GetHandSpeed(gamePlayer.FindHandIndex(this.gameEntity.id));
			num = handSpeed * handSpeed;
		}
		if (num < this.minSwingSpeed * this.minSwingSpeed)
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		if (timeAsDouble < this.hitCooldownEnd)
		{
			return;
		}
		Collider collider = collision.collider;
		GameHittable parentEnemy = this.GetParentEnemy<GameHittable>(collider);
		if (parentEnemy != null && parentEnemy.IsColliderValid(collision.collider))
		{
			Vector3 vector = parentEnemy.transform.position - base.transform.position;
			vector.Normalize();
			if (!flag && gamePlayer != null)
			{
				vector = GamePlayerLocal.instance.GetHandVelocity(gamePlayer.FindHandIndex(this.gameEntity.id)).normalized;
			}
			float num2 = Mathf.Sqrt(num);
			num2 = Mathf.Min(num2, this.maxImpulseSpeed);
			vector *= num2;
			Vector3 position = parentEnemy.transform.position;
			GameHitData gameHitData = new GameHitData
			{
				hitTypeId = (int)this.hitType,
				hitEntityId = parentEnemy.gameEntity.id,
				hitByEntityId = this.gameEntity.id,
				hitEntityPosition = position,
				hitImpulse = vector * this.knockbackMultiplier,
				hitPosition = collision.GetContact(0).point,
				hitAmount = this.CalcHitAmount(this.hitType, parentEnemy, this.gameEntity),
				hittablePoint = parentEnemy.FindHittablePoint(collider)
			};
			if (parentEnemy.IsHitValid(gameHitData))
			{
				parentEnemy.RequestHit(gameHitData);
				this.hitCooldownEnd = timeAsDouble + 0.25;
			}
		}
	}

	// Token: 0x040038F5 RID: 14581
	public GameEntity gameEntity;

	// Token: 0x040038F6 RID: 14582
	public GameHitType hitType;

	// Token: 0x040038F7 RID: 14583
	public GRAttributeType damageAttribute = GRAttributeType.BatonDamage;

	// Token: 0x040038F8 RID: 14584
	public GRAttributeType flashDamageAttribute = GRAttributeType.FlashDamage;

	// Token: 0x040038F9 RID: 14585
	public GRAttributeType shieldDamageAttribute = GRAttributeType.BatonDamage;

	// Token: 0x040038FA RID: 14586
	public float minSwingSpeed = 1.5f;

	// Token: 0x040038FB RID: 14587
	public GameHitFx hitFx;

	// Token: 0x040038FC RID: 14588
	private GRAttributes attributes;

	// Token: 0x040038FD RID: 14589
	public float knockbackMultiplier = 1f;

	// Token: 0x040038FE RID: 14590
	public float maxImpulseSpeed = 4.5f;

	// Token: 0x040038FF RID: 14591
	private List<IGameHitter> components;

	// Token: 0x04003900 RID: 14592
	private double hitCooldownEnd;

	// Token: 0x04003901 RID: 14593
	public bool hitOnCollision = true;
}
