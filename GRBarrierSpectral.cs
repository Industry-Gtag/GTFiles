using System;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x0200075E RID: 1886
public class GRBarrierSpectral : MonoBehaviour, IGameEntityComponent, IGameHittable
{
	// Token: 0x06002FD6 RID: 12246 RVA: 0x001042B8 File Offset: 0x001024B8
	public void Awake()
	{
		this.hitFx.SetActive(false);
		this.destroyedFx.SetActive(false);
	}

	// Token: 0x06002FD7 RID: 12247 RVA: 0x001042D4 File Offset: 0x001024D4
	public void OnEntityInit()
	{
		this.entity.SetState((long)this.health);
		Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(this.entity.createData);
		base.transform.localScale = vector;
	}

	// Token: 0x06002FD8 RID: 12248 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002FD9 RID: 12249 RVA: 0x00104310 File Offset: 0x00102510
	public void OnEntityStateChange(long prevState, long newState)
	{
		int num = (int)newState;
		this.ChangeHealth(num);
	}

	// Token: 0x06002FDA RID: 12250 RVA: 0x00104328 File Offset: 0x00102528
	public void OnImpact(GameHitType hitType)
	{
		if (hitType == GameHitType.Flash)
		{
			int num = Mathf.Max(this.health - 1, 0);
			this.ChangeHealth(num);
			if (this.entity.IsAuthority())
			{
				this.entity.RequestState(this.entity.id, (long)this.health);
			}
		}
	}

	// Token: 0x06002FDB RID: 12251 RVA: 0x0010437C File Offset: 0x0010257C
	private void ChangeHealth(int nextHealth)
	{
		if (this.health != nextHealth)
		{
			this.health = nextHealth;
			if (this.health == 0)
			{
				this.collider.enabled = false;
				this.visualMesh.enabled = false;
				this.audioSource.PlayOneShot(this.onDestroyedClip, this.onDestroyedVolume);
				this.destroyedFx.SetActive(false);
				this.destroyedFx.SetActive(true);
			}
			else
			{
				this.audioSource.PlayOneShot(this.onDamageClip, this.onDamageVolume);
				this.hitFx.SetActive(false);
				this.hitFx.SetActive(true);
			}
			this.RefreshVisuals();
		}
	}

	// Token: 0x06002FDC RID: 12252 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x06002FDD RID: 12253 RVA: 0x00104424 File Offset: 0x00102624
	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		if (this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId) != null)
		{
			this.OnImpact(hitTypeId);
		}
	}

	// Token: 0x06002FDE RID: 12254 RVA: 0x00104460 File Offset: 0x00102660
	public void RefreshVisuals()
	{
		if (this.lastVisualUpdateHealth != this.health)
		{
			this.lastVisualUpdateHealth = this.health;
			Color color = this.visualMesh.material.GetColor("_BaseColor");
			color.a = (float)this.health / (float)this.maxHealth;
			this.visualMesh.material.SetColor("_BaseColor", color);
		}
	}

	// Token: 0x04003D49 RID: 15689
	public GameEntity entity;

	// Token: 0x04003D4A RID: 15690
	public MeshRenderer visualMesh;

	// Token: 0x04003D4B RID: 15691
	public Collider collider;

	// Token: 0x04003D4C RID: 15692
	public AudioSource audioSource;

	// Token: 0x04003D4D RID: 15693
	public AudioClip onDamageClip;

	// Token: 0x04003D4E RID: 15694
	public float onDamageVolume;

	// Token: 0x04003D4F RID: 15695
	public AudioClip onDestroyedClip;

	// Token: 0x04003D50 RID: 15696
	public float onDestroyedVolume;

	// Token: 0x04003D51 RID: 15697
	[SerializeField]
	private GameObject hitFx;

	// Token: 0x04003D52 RID: 15698
	[SerializeField]
	private GameObject destroyedFx;

	// Token: 0x04003D53 RID: 15699
	public int maxHealth = 3;

	// Token: 0x04003D54 RID: 15700
	[ReadOnly]
	public int health = 3;

	// Token: 0x04003D55 RID: 15701
	private int lastVisualUpdateHealth = -1;
}
