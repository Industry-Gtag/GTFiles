using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000764 RID: 1892
public class GRBreakable : MonoBehaviour, IGameHittable
{
	// Token: 0x17000483 RID: 1155
	// (get) Token: 0x06002FF6 RID: 12278 RVA: 0x00104E90 File Offset: 0x00103090
	public bool BrokenLocal
	{
		get
		{
			return this.brokenLocal;
		}
	}

	// Token: 0x06002FF7 RID: 12279 RVA: 0x00104E98 File Offset: 0x00103098
	private void OnEnable()
	{
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x06002FF8 RID: 12280 RVA: 0x00104EB1 File Offset: 0x001030B1
	private void OnDisable()
	{
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged -= this.OnEntityStateChanged;
		}
	}

	// Token: 0x06002FF9 RID: 12281 RVA: 0x00104ED8 File Offset: 0x001030D8
	private void OnEntityStateChanged(long prevState, long nextState)
	{
		GRBreakable.BreakableState breakableState = (GRBreakable.BreakableState)nextState;
		if (breakableState == GRBreakable.BreakableState.Broken)
		{
			this.BreakLocal();
			return;
		}
		if (breakableState == GRBreakable.BreakableState.Unbroken)
		{
			this.RestoreLocal();
		}
	}

	// Token: 0x06002FFA RID: 12282 RVA: 0x00104EFC File Offset: 0x001030FC
	public void BreakLocal()
	{
		if (!this.brokenLocal)
		{
			this.brokenLocal = true;
			if (this.breakableCollider != null)
			{
				this.breakableCollider.enabled = false;
			}
			for (int i = 0; i < this.disableWhenBroken.Count; i++)
			{
				this.disableWhenBroken[i].gameObject.SetActive(false);
			}
			for (int j = 0; j < this.enableWhenBroken.Count; j++)
			{
				this.enableWhenBroken[j].gameObject.SetActive(true);
			}
			if (this.audioSource != null)
			{
				this.audioSource.PlayOneShot(this.breakSound, this.breakSoundVolume);
			}
			GameEntity gameEntity;
			if (this.gameEntity.IsAuthority() && this.holdsRandomItem && this.itemSpawnProbability.TryForRandomItem(this.gameEntity, out gameEntity, 0))
			{
				this.gameEntity.manager.RequestCreateItem(gameEntity.gameObject.name.GetStaticHash(), this.itemSpawnLocation.position, this.itemSpawnLocation.rotation, 0L);
			}
		}
	}

	// Token: 0x06002FFB RID: 12283 RVA: 0x00105018 File Offset: 0x00103218
	public void RestoreLocal()
	{
		if (this.brokenLocal)
		{
			this.brokenLocal = false;
			if (this.breakableCollider != null)
			{
				this.breakableCollider.enabled = true;
			}
			for (int i = 0; i < this.disableWhenBroken.Count; i++)
			{
				this.disableWhenBroken[i].gameObject.SetActive(true);
			}
			for (int j = 0; j < this.enableWhenBroken.Count; j++)
			{
				this.enableWhenBroken[j].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06002FFC RID: 12284 RVA: 0x001050A8 File Offset: 0x001032A8
	public bool IsHitValid(GameHitData hit)
	{
		return !this.brokenLocal && hit.hitTypeId == 0;
	}

	// Token: 0x06002FFD RID: 12285 RVA: 0x001050C0 File Offset: 0x001032C0
	public void OnHit(GameHitData hit)
	{
		if (hit.hitTypeId == 0 && (int)this.gameEntity.GetState() != 1)
		{
			this.gameEntity.RequestState(this.gameEntity.id, 1L);
			GameEntity gameEntity = this.gameEntity.manager.GetGameEntity(hit.hitByEntityId);
			if (gameEntity != null && gameEntity.IsHeldByLocalPlayer())
			{
				PlayerGameEvents.MiscEvent("GRSmashBreakable", 1);
			}
		}
	}

	// Token: 0x04003D7B RID: 15739
	public GameEntity gameEntity;

	// Token: 0x04003D7C RID: 15740
	public List<Transform> enableWhenBroken;

	// Token: 0x04003D7D RID: 15741
	public List<Transform> disableWhenBroken;

	// Token: 0x04003D7E RID: 15742
	public Collider breakableCollider;

	// Token: 0x04003D7F RID: 15743
	public bool holdsRandomItem = true;

	// Token: 0x04003D80 RID: 15744
	public Transform itemSpawnLocation;

	// Token: 0x04003D81 RID: 15745
	public GRBreakableItemSpawnConfig itemSpawnProbability;

	// Token: 0x04003D82 RID: 15746
	public AudioSource audioSource;

	// Token: 0x04003D83 RID: 15747
	public AudioClip breakSound;

	// Token: 0x04003D84 RID: 15748
	public float breakSoundVolume;

	// Token: 0x04003D85 RID: 15749
	private bool brokenLocal;

	// Token: 0x02000765 RID: 1893
	public enum BreakableState
	{
		// Token: 0x04003D87 RID: 15751
		Unbroken,
		// Token: 0x04003D88 RID: 15752
		Broken
	}
}
