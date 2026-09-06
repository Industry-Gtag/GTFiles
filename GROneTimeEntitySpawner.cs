using System;
using UnityEngine;

// Token: 0x020007CA RID: 1994
public class GROneTimeEntitySpawner : MonoBehaviour
{
	// Token: 0x060032CF RID: 13007 RVA: 0x001165EF File Offset: 0x001147EF
	private void Start()
	{
		if (this.EntityPrefab == null)
		{
			Debug.Log("Can't  spawn null entity", this);
		}
		base.Invoke("TrySpawn", this.SpawnDelay);
	}

	// Token: 0x060032D0 RID: 13008 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Update()
	{
	}

	// Token: 0x060032D1 RID: 13009 RVA: 0x0011661C File Offset: 0x0011481C
	private void TrySpawn()
	{
		if (!this.bHasSpawned && this.EntityPrefab != null)
		{
			Debug.Log("trying to spawn entity" + this.EntityPrefab.name, this);
			GameEntityManager gameEntityManager = this.reactor.grManager.gameEntityManager;
			if (gameEntityManager.IsAuthority())
			{
				if (!gameEntityManager.IsZoneActive())
				{
					Debug.Log("delaying spawn attempt because zone not active", this);
					base.Invoke("TrySpawn", 0.2f);
					return;
				}
				Debug.Log("trying to spawn entity", this);
				gameEntityManager.RequestCreateItem(this.EntityPrefab.name.GetStaticHash(), base.transform.position + new Vector3(0f, 0f, 0f), base.transform.rotation, 0L);
				this.bHasSpawned = true;
			}
		}
	}

	// Token: 0x040041DB RID: 16859
	public GhostReactor reactor;

	// Token: 0x040041DC RID: 16860
	public GameEntity EntityPrefab;

	// Token: 0x040041DD RID: 16861
	private bool bHasSpawned;

	// Token: 0x040041DE RID: 16862
	private float SpawnDelay = 3f;
}
