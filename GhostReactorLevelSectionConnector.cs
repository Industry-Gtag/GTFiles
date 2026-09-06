using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000718 RID: 1816
public class GhostReactorLevelSectionConnector : MonoBehaviour
{
	// Token: 0x06002DBF RID: 11711 RVA: 0x000F88E8 File Offset: 0x000F6AE8
	private void Awake()
	{
		this.prePlacedGameEntities = new List<GameEntity>(128);
		base.GetComponentsInChildren<GameEntity>(this.prePlacedGameEntities);
		for (int i = 0; i < this.prePlacedGameEntities.Count; i++)
		{
			this.prePlacedGameEntities[i].gameObject.SetActive(false);
		}
		this.renderers = new List<Renderer>(512);
		this.hidden = false;
		base.GetComponentsInChildren<Renderer>(this.renderers);
		if (this.boundingCollider == null)
		{
			Debug.LogWarningFormat("Missing Bounding Collider for section {0}", new object[] { base.gameObject.name });
		}
	}

	// Token: 0x06002DC0 RID: 11712 RVA: 0x000F8990 File Offset: 0x000F6B90
	public void Init(GhostReactorManager grManager)
	{
		if (grManager.IsAuthority())
		{
			if (this.gateEntity != null)
			{
				grManager.gameEntityManager.RequestCreateItem(this.gateEntity.name.GetStaticHash(), this.gateSpawnPoint.position, this.gateSpawnPoint.rotation, 0L);
			}
			for (int i = 0; i < this.prePlacedGameEntities.Count; i++)
			{
				if (!this.prePlacedGameEntities[i].isBuiltIn)
				{
					int staticHash = this.prePlacedGameEntities[i].gameObject.name.GetStaticHash();
					if (!grManager.gameEntityManager.FactoryHasEntity(staticHash))
					{
						Debug.LogErrorFormat("Cannot Find Entity in Factory {0} {1}", new object[]
						{
							this.prePlacedGameEntities[i].gameObject.name,
							staticHash
						});
					}
					else
					{
						GameEntityCreateData gameEntityCreateData = new GameEntityCreateData
						{
							entityTypeId = staticHash,
							position = this.prePlacedGameEntities[i].transform.position,
							rotation = this.prePlacedGameEntities[i].transform.rotation,
							createData = 0L,
							createdByEntityId = -1,
							slotIndex = -1
						};
						GhostReactorLevelSection.tempCreateEntitiesList.Add(gameEntityCreateData);
					}
				}
			}
			grManager.gameEntityManager.RequestCreateItems(GhostReactorLevelSection.tempCreateEntitiesList);
			GhostReactorLevelSection.tempCreateEntitiesList.Clear();
		}
	}

	// Token: 0x06002DC1 RID: 11713 RVA: 0x000F8B04 File Offset: 0x000F6D04
	public void Hide(bool hide)
	{
		for (int i = 0; i < this.renderers.Count; i++)
		{
			if (!(this.renderers[i] == null))
			{
				this.renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x06002DC2 RID: 11714 RVA: 0x000F8B50 File Offset: 0x000F6D50
	public void UpdateDisable(Vector3 playerPos)
	{
		if (this.boundingCollider == null)
		{
			return;
		}
		float sqrMagnitude = (this.boundingCollider.ClosestPoint(playerPos) - playerPos).sqrMagnitude;
		float num = 324f;
		float num2 = 484f;
		if (this.hidden && sqrMagnitude < num)
		{
			this.hidden = false;
			this.Hide(false);
			return;
		}
		if (!this.hidden && sqrMagnitude > num2)
		{
			this.hidden = true;
			this.Hide(true);
		}
	}

	// Token: 0x04003A94 RID: 14996
	public Transform hubAnchor;

	// Token: 0x04003A95 RID: 14997
	public Transform sectionAnchor;

	// Token: 0x04003A96 RID: 14998
	public Transform gateSpawnPoint;

	// Token: 0x04003A97 RID: 14999
	public GameEntity gateEntity;

	// Token: 0x04003A98 RID: 15000
	public GhostReactorLevelSectionConnector.Direction direction;

	// Token: 0x04003A99 RID: 15001
	public BoxCollider boundingCollider;

	// Token: 0x04003A9A RID: 15002
	public List<Transform> pathNodes;

	// Token: 0x04003A9B RID: 15003
	private const float SHOW_DIST = 18f;

	// Token: 0x04003A9C RID: 15004
	private const float HIDE_DIST = 22f;

	// Token: 0x04003A9D RID: 15005
	private List<GameEntity> prePlacedGameEntities;

	// Token: 0x04003A9E RID: 15006
	private List<Renderer> renderers;

	// Token: 0x04003A9F RID: 15007
	private bool hidden;

	// Token: 0x02000719 RID: 1817
	public enum Direction
	{
		// Token: 0x04003AA1 RID: 15009
		Down = -1,
		// Token: 0x04003AA2 RID: 15010
		Forward,
		// Token: 0x04003AA3 RID: 15011
		Up
	}
}
