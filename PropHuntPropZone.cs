using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000281 RID: 641
public class PropHuntPropZone : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x06001158 RID: 4440 RVA: 0x0005D4A4 File Offset: 0x0005B6A4
	private void Awake()
	{
		this.hasBoxCollider = base.TryGetComponent<BoxCollider>(out this.boxCollider);
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x0005D4C3 File Offset: 0x0005B6C3
	private void OnEnable()
	{
		GorillaPropHuntGameManager.RegisterPropZone(this);
	}

	// Token: 0x0600115A RID: 4442 RVA: 0x0005D4CB File Offset: 0x0005B6CB
	private void OnDisable()
	{
		this.DestroyDecoys();
		GorillaPropHuntGameManager.UnregisterPropZone(this);
	}

	// Token: 0x0600115B RID: 4443 RVA: 0x0005D4DC File Offset: 0x0005B6DC
	public void DestroyDecoys()
	{
		foreach (PropPlacementRB propPlacementRB in this.propPlacementRBs)
		{
			if (propPlacementRB != null)
			{
				PropHuntPools.ReturnDecoyProp(propPlacementRB);
			}
		}
		this.propPlacementRBs.Clear();
	}

	// Token: 0x0600115C RID: 4444 RVA: 0x0005D544 File Offset: 0x0005B744
	public void OnRoundStart()
	{
		if (!PropHuntPools.IsReady)
		{
			Debug.LogError("ERROR!!!  PropHuntPropZone: (this should never happen) props not ready to be spawned so aborting. you should only be calling this if `PropHuntPools.IsReady` is true or from the callback `PropHuntPools.OnReady`.");
		}
		this.CreateDecoys(GorillaPropHuntGameManager.instance.GetSeed());
	}

	// Token: 0x0600115D RID: 4445 RVA: 0x0005D568 File Offset: 0x0005B768
	public void CreateDecoys(int seed)
	{
		this.DestroyDecoys();
		SRand srand = new SRand(seed + this.seedOffset);
		for (int i = 0; i < this.numProps; i++)
		{
			PropPlacementRB propPlacementRB;
			if (!PropHuntPools.TryGetDecoyProp(GorillaPropHuntGameManager.instance.GetCosmeticId(srand.NextUInt()), out propPlacementRB))
			{
				return;
			}
			Vector3 vector2;
			if (this.hasBoxCollider)
			{
				Vector3 vector = new Vector3(srand.NextFloat(-this.boxCollider.size.x, this.boxCollider.size.x) / 2f, srand.NextFloat(-this.boxCollider.size.y, this.boxCollider.size.y) / 2f, srand.NextFloat(-this.boxCollider.size.z, this.boxCollider.size.z) / 2f);
				vector2 = base.transform.TransformPoint(vector);
			}
			else
			{
				vector2 = base.transform.position + srand.NextPointInsideSphere(this.radius);
			}
			propPlacementRB.gameObject.SetActive(false);
			propPlacementRB.transform.SetParent(null, false);
			propPlacementRB.transform.position = vector2;
			propPlacementRB.transform.rotation = Quaternion.Euler(srand.NextFloat(360f), srand.NextFloat(360f), srand.NextFloat(360f));
			propPlacementRB._placingProp.SetActive(false);
			propPlacementRB._placingProp.transform.SetParent(null, false);
			this.propPlacementRBs.Add(propPlacementRB);
		}
		for (int j = 0; j < this.propPlacementRBs.Count; j++)
		{
			this.propPlacementRBs[j].gameObject.SetActive(true);
		}
		GTDelayedExec.Add(this, this.m_simDurationBeforeFreeze, 0);
	}

	// Token: 0x0600115E RID: 4446 RVA: 0x0005D74C File Offset: 0x0005B94C
	public void OnDelayedAction(int contextId)
	{
		for (int i = 0; i < this.propPlacementRBs.Count; i++)
		{
			PropPlacementRB propPlacementRB = this.propPlacementRBs[i];
			propPlacementRB.gameObject.SetActive(false);
			Transform transform = propPlacementRB.transform;
			GameObject placingProp = propPlacementRB._placingProp;
			placingProp.transform.SetPositionAndRotation(transform.position, transform.rotation);
			placingProp.SetActive(true);
		}
	}

	// Token: 0x0600115F RID: 4447 RVA: 0x0005D7B0 File Offset: 0x0005B9B0
	private PropPlacementRB _GetOrCreatePropPlacementObj_NoPool()
	{
		PropPlacementRB propPlacementRB;
		if (this.nextUnusedPropPlacement < this.propPlacementRBs.Count)
		{
			propPlacementRB = this.propPlacementRBs[this.nextUnusedPropPlacement];
		}
		else
		{
			propPlacementRB = Object.Instantiate<PropPlacementRB>(this.propPlacementPrefab, base.transform);
			this.propPlacementRBs.Add(propPlacementRB);
		}
		this.nextUnusedPropPlacement++;
		return propPlacementRB;
	}

	// Token: 0x06001160 RID: 4448 RVA: 0x0005D811 File Offset: 0x0005BA11
	private void SpawnProp_NoPool(GTAssetRef<GameObject> item, Vector3 pos, Quaternion rot, CosmeticSO debugCosmeticSO)
	{
		this._GetOrCreatePropPlacementObj_NoPool().PlaceProp_NoPool(this, item, pos, rot, debugCosmeticSO);
	}

	// Token: 0x040014AE RID: 5294
	private const string preLog = "PropHuntPropZone: ";

	// Token: 0x040014AF RID: 5295
	private const string preLogEd = "(editor only log) PropHuntPropZone: ";

	// Token: 0x040014B0 RID: 5296
	private const string preLogBeta = "(beta only log) PropHuntPropZone: ";

	// Token: 0x040014B1 RID: 5297
	private const string preErr = "ERROR!!!  PropHuntPropZone: ";

	// Token: 0x040014B2 RID: 5298
	private const string preErrEd = "ERROR!!!  (editor only log) PropHuntPropZone: ";

	// Token: 0x040014B3 RID: 5299
	private const string preErrBeta = "ERROR!!!  (beta only log) PropHuntPropZone: ";

	// Token: 0x040014B4 RID: 5300
	private const bool _k__GT_PROP_HUNT__USE_POOLING__ = true;

	// Token: 0x040014B5 RID: 5301
	[SerializeField]
	private PropPlacementRB propPlacementPrefab;

	// Token: 0x040014B6 RID: 5302
	[SerializeField]
	private int seedOffset;

	// Token: 0x040014B7 RID: 5303
	[SerializeField]
	private float radius = 1f;

	// Token: 0x040014B8 RID: 5304
	[SerializeField]
	private int numProps = 10;

	// Token: 0x040014B9 RID: 5305
	[SerializeField]
	private float m_simDurationBeforeFreeze = 2f;

	// Token: 0x040014BA RID: 5306
	private BoxCollider boxCollider;

	// Token: 0x040014BB RID: 5307
	private bool hasBoxCollider;

	// Token: 0x040014BC RID: 5308
	private int nextUnusedPropPlacement;

	// Token: 0x040014BD RID: 5309
	private readonly List<PropPlacementRB> propPlacementRBs = new List<PropPlacementRB>(64);
}
