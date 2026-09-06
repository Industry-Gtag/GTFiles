using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020004BD RID: 1213
public class SlingshotLifeIndicator : MonoBehaviour, IGorillaSliceableSimple, ISpawnable
{
	// Token: 0x17000325 RID: 805
	// (get) Token: 0x06001D9C RID: 7580 RVA: 0x000A02BF File Offset: 0x0009E4BF
	// (set) Token: 0x06001D9D RID: 7581 RVA: 0x000A02C7 File Offset: 0x0009E4C7
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000326 RID: 806
	// (get) Token: 0x06001D9E RID: 7582 RVA: 0x000A02D0 File Offset: 0x0009E4D0
	// (set) Token: 0x06001D9F RID: 7583 RVA: 0x000A02D8 File Offset: 0x0009E4D8
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001DA0 RID: 7584 RVA: 0x000A02E1 File Offset: 0x0009E4E1
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06001DA1 RID: 7585 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001DA2 RID: 7586 RVA: 0x000A02EA File Offset: 0x0009E4EA
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
	}

	// Token: 0x06001DA3 RID: 7587 RVA: 0x000A030E File Offset: 0x0009E50E
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.Reset();
		RoomSystem.LeftRoomEvent -= new Action(this.OnLeftRoom);
	}

	// Token: 0x06001DA4 RID: 7588 RVA: 0x000A0339 File Offset: 0x0009E539
	private void SetActive(GameObject obj, bool active)
	{
		if (!obj.activeSelf && active)
		{
			obj.SetActive(true);
		}
		if (obj.activeSelf && !active)
		{
			obj.SetActive(false);
		}
	}

	// Token: 0x06001DA5 RID: 7589 RVA: 0x000A0364 File Offset: 0x0009E564
	public void SliceUpdate()
	{
		if (!NetworkSystem.Instance.InRoom || (this.checkedBattle && !this.inBattle))
		{
			if (this.indicator1.activeSelf)
			{
				this.indicator1.SetActive(false);
			}
			if (this.indicator2.activeSelf)
			{
				this.indicator2.SetActive(false);
			}
			if (this.indicator3.activeSelf)
			{
				this.indicator3.SetActive(false);
			}
			return;
		}
		if (this.bMgr == null)
		{
			this.checkedBattle = true;
			this.inBattle = true;
			if (GorillaGameManager.instance == null)
			{
				return;
			}
			this.bMgr = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
			if (this.bMgr == null)
			{
				this.inBattle = false;
				return;
			}
		}
		VRRig vrrig = this.myRig;
		if (((vrrig != null) ? vrrig.creator : null) == null)
		{
			return;
		}
		int playerLives = this.bMgr.GetPlayerLives(this.myRig.creator);
		this.SetActive(this.indicator1, playerLives >= 1);
		this.SetActive(this.indicator2, playerLives >= 2);
		this.SetActive(this.indicator3, playerLives >= 3);
	}

	// Token: 0x06001DA6 RID: 7590 RVA: 0x000A0499 File Offset: 0x0009E699
	public void OnLeftRoom()
	{
		this.Reset();
	}

	// Token: 0x06001DA7 RID: 7591 RVA: 0x000A04A1 File Offset: 0x0009E6A1
	public void Reset()
	{
		this.bMgr = null;
		this.inBattle = false;
		this.checkedBattle = false;
	}

	// Token: 0x040027F9 RID: 10233
	private VRRig myRig;

	// Token: 0x040027FA RID: 10234
	public GorillaPaintbrawlManager bMgr;

	// Token: 0x040027FB RID: 10235
	public bool checkedBattle;

	// Token: 0x040027FC RID: 10236
	public bool inBattle;

	// Token: 0x040027FD RID: 10237
	public GameObject indicator1;

	// Token: 0x040027FE RID: 10238
	public GameObject indicator2;

	// Token: 0x040027FF RID: 10239
	public GameObject indicator3;
}
