using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200062F RID: 1583
public class BuilderDispenserShelf : MonoBehaviour
{
	// Token: 0x06002773 RID: 10099 RVA: 0x000D0C6B File Offset: 0x000CEE6B
	private void BuildDispenserPool()
	{
		this.dispenserPool = new List<BuilderDispenser>(12);
		this.activeDispensers = new List<BuilderDispenser>(6);
		this.AddToDispenserPool(6);
	}

	// Token: 0x06002774 RID: 10100 RVA: 0x000D0C90 File Offset: 0x000CEE90
	private void AddToDispenserPool(int count)
	{
		if (this.dispenserPrefab == null)
		{
			return;
		}
		for (int i = 0; i < count; i++)
		{
			BuilderDispenser builderDispenser = Object.Instantiate<BuilderDispenser>(this.dispenserPrefab, this.shelfCenter);
			builderDispenser.gameObject.SetActive(false);
			builderDispenser.table = this.table;
			builderDispenser.shelfID = this.shelfID;
			this.dispenserPool.Add(builderDispenser);
		}
	}

	// Token: 0x06002775 RID: 10101 RVA: 0x000D0CFC File Offset: 0x000CEEFC
	private void ActivateDispensers()
	{
		this.piecesInSet.Clear();
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in this.currentGroup.pieceSubsets)
		{
			if (this._includedCategories.Contains(builderPieceSubset.pieceCategory))
			{
				this.piecesInSet.AddRange(builderPieceSubset.pieceInfos);
			}
		}
		if (this.piecesInSet.Count <= 0)
		{
			return;
		}
		int count = this.piecesInSet.Count;
		if (this.dispenserPool.Count < count)
		{
			this.AddToDispenserPool(count - this.dispenserPool.Count);
		}
		this.activeDispensers.Clear();
		for (int i = 0; i < this.dispenserPool.Count; i++)
		{
			if (i < count)
			{
				BuilderDispenser builderDispenser = this.dispenserPool[i];
				builderDispenser.gameObject.SetActive(true);
				float num = this.shelfWidth / -2f + this.shelfWidth / (float)(count * 2) + this.shelfWidth / (float)count * (float)i;
				builderDispenser.transform.localPosition = new Vector3(num, 0f, 0f);
				builderDispenser.AssignPieceType(this.piecesInSet[i], this.currentGroup.defaultMaterial.GetHashCode());
				this.activeDispensers.Add(builderDispenser);
			}
			else
			{
				this.dispenserPool[i].ClearDispenser();
				this.dispenserPool[i].gameObject.SetActive(false);
			}
		}
		this.dispenserToUpdate = 0;
	}

	// Token: 0x06002776 RID: 10102 RVA: 0x000D0EA8 File Offset: 0x000CF0A8
	public void Setup()
	{
		this.InitIfNeeded();
		foreach (BuilderDispenser builderDispenser in this.dispenserPool)
		{
			builderDispenser.table = this.table;
			builderDispenser.shelfID = this.shelfID;
		}
	}

	// Token: 0x06002777 RID: 10103 RVA: 0x000D0F10 File Offset: 0x000CF110
	private void InitIfNeeded()
	{
		if (this.initialized)
		{
			return;
		}
		this.setSelector.Setup(this._includedCategories);
		this.currentGroup = this.setSelector.GetSelectedGroup();
		this.setSelector.OnSelectedGroup.AddListener(new UnityAction<int>(this.OnSelectedSetChange));
		this.BuildDispenserPool();
		this.ActivateDispensers();
		this.initialized = true;
	}

	// Token: 0x06002778 RID: 10104 RVA: 0x000D0F77 File Offset: 0x000CF177
	private void OnDestroy()
	{
		if (this.setSelector != null)
		{
			this.setSelector.OnSelectedGroup.RemoveListener(new UnityAction<int>(this.OnSelectedSetChange));
		}
	}

	// Token: 0x06002779 RID: 10105 RVA: 0x000D0FA3 File Offset: 0x000CF1A3
	public void OnSelectedSetChange(int displayGroupID)
	{
		if (this.table.GetTableState() != BuilderTable.TableState.Ready)
		{
			return;
		}
		this.table.RequestShelfSelection(this.shelfID, displayGroupID, false);
	}

	// Token: 0x0600277A RID: 10106 RVA: 0x000D0FC8 File Offset: 0x000CF1C8
	public void SetSelection(int displayGroupID)
	{
		this.setSelector.SetSelection(displayGroupID);
		BuilderPieceSet.BuilderDisplayGroup selectedGroup = this.setSelector.GetSelectedGroup();
		if ((this.initialized && this.currentGroup == null) || selectedGroup.displayName != this.currentGroup.displayName)
		{
			this.currentGroup = selectedGroup;
			if (this.table.GetTableState() == BuilderTable.TableState.Ready)
			{
				if (!this.animatingShelf)
				{
					this.StartShelfSwap();
					return;
				}
			}
			else
			{
				this.animatingShelf = false;
				this.ImmediateShelfSwap();
			}
		}
	}

	// Token: 0x0600277B RID: 10107 RVA: 0x000D1046 File Offset: 0x000CF246
	public int GetSelectedDisplayGroupID()
	{
		return this.setSelector.GetSelectedGroup().GetDisplayGroupIdentifier();
	}

	// Token: 0x0600277C RID: 10108 RVA: 0x000D1058 File Offset: 0x000CF258
	private void ImmediateShelfSwap()
	{
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ClearDispenser();
		}
		this.ActivateDispensers();
	}

	// Token: 0x0600277D RID: 10109 RVA: 0x000D10B0 File Offset: 0x000CF2B0
	private void StartShelfSwap()
	{
		this.dispenserToClear = 0;
		this.timeToClearShelf = (double)(Time.time + 0.15f);
		this.resetAnimation.Rewind();
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ParentPieceToShelf(this.resetAnimation.transform);
		}
		this.resetAnimation.Play();
		this.animatingShelf = true;
	}

	// Token: 0x0600277E RID: 10110 RVA: 0x000D1144 File Offset: 0x000CF344
	public void UpdateShelf()
	{
		if (this.animatingShelf && (double)Time.time > this.timeToClearShelf)
		{
			if (this.dispenserToClear < this.activeDispensers.Count)
			{
				if (this.dispenserToClear == 0)
				{
					this.resetSoundBank.Play();
				}
				this.activeDispensers[this.dispenserToClear].ClearDispenser();
				this.dispenserToClear++;
				return;
			}
			if (!this.resetAnimation.isPlaying)
			{
				this.playSpawnSetSound = true;
				this.ActivateDispensers();
				this.animatingShelf = false;
			}
		}
	}

	// Token: 0x0600277F RID: 10111 RVA: 0x000D11D4 File Offset: 0x000CF3D4
	public void UpdateShelfSliced()
	{
		if (!PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			return;
		}
		if (!this.initialized)
		{
			return;
		}
		if (this.animatingShelf)
		{
			return;
		}
		if (this.shouldVerifySetSelection)
		{
			BuilderPieceSet.BuilderDisplayGroup selectedGroup = this.setSelector.GetSelectedGroup();
			if (selectedGroup == null || !BuilderSetManager.instance.DoesAnyPlayerInRoomOwnPieceSet(selectedGroup.setID))
			{
				int defaultGroupID = this.setSelector.GetDefaultGroupID();
				if (defaultGroupID != -1)
				{
					this.OnSelectedSetChange(defaultGroupID);
				}
			}
			this.shouldVerifySetSelection = false;
		}
		if (this.activeDispensers.Count > 0)
		{
			this.activeDispensers[this.dispenserToUpdate].UpdateDispenser();
			this.dispenserToUpdate = (this.dispenserToUpdate + 1) % this.activeDispensers.Count;
		}
	}

	// Token: 0x06002780 RID: 10112 RVA: 0x000D1287 File Offset: 0x000CF487
	public void VerifySetSelection()
	{
		this.shouldVerifySetSelection = true;
	}

	// Token: 0x06002781 RID: 10113 RVA: 0x000D1290 File Offset: 0x000CF490
	public void OnShelfPieceCreated(BuilderPiece piece, bool playfx)
	{
		if (this.playSpawnSetSound && playfx)
		{
			this.audioSource.GTPlayOneShot(this.spawnNewSetSound, 1f);
			this.playSpawnSetSound = false;
		}
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ShelfPieceCreated(piece, playfx);
		}
	}

	// Token: 0x06002782 RID: 10114 RVA: 0x000D130C File Offset: 0x000CF50C
	public void OnShelfPieceRecycled(BuilderPiece piece)
	{
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ShelfPieceRecycled(piece);
		}
	}

	// Token: 0x06002783 RID: 10115 RVA: 0x000D1360 File Offset: 0x000CF560
	public void OnClearTable()
	{
		if (!this.initialized)
		{
			return;
		}
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.OnClearTable();
		}
		base.StopAllCoroutines();
		if (this.animatingShelf)
		{
			this.resetAnimation.Rewind();
			this.animatingShelf = false;
		}
	}

	// Token: 0x06002784 RID: 10116 RVA: 0x000D13DC File Offset: 0x000CF5DC
	public void ClearShelf()
	{
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ClearDispenser();
		}
	}

	// Token: 0x04003317 RID: 13079
	[Header("Set Selection")]
	[SerializeField]
	private BuilderSetSelector setSelector;

	// Token: 0x04003318 RID: 13080
	public List<BuilderPieceSet.BuilderPieceCategory> _includedCategories;

	// Token: 0x04003319 RID: 13081
	[Header("Dispenser Shelf Properties")]
	public Transform shelfCenter;

	// Token: 0x0400331A RID: 13082
	public float shelfWidth = 1.4f;

	// Token: 0x0400331B RID: 13083
	public Animation resetAnimation;

	// Token: 0x0400331C RID: 13084
	[SerializeField]
	private SoundBankPlayer resetSoundBank;

	// Token: 0x0400331D RID: 13085
	[SerializeField]
	private AudioClip spawnNewSetSound;

	// Token: 0x0400331E RID: 13086
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400331F RID: 13087
	private bool playSpawnSetSound;

	// Token: 0x04003320 RID: 13088
	[HideInInspector]
	public BuilderTable table;

	// Token: 0x04003321 RID: 13089
	public int shelfID = -1;

	// Token: 0x04003322 RID: 13090
	private BuilderPieceSet.BuilderDisplayGroup currentGroup;

	// Token: 0x04003323 RID: 13091
	private bool initialized;

	// Token: 0x04003324 RID: 13092
	public BuilderDispenser dispenserPrefab;

	// Token: 0x04003325 RID: 13093
	private List<BuilderDispenser> dispenserPool;

	// Token: 0x04003326 RID: 13094
	private List<BuilderDispenser> activeDispensers;

	// Token: 0x04003327 RID: 13095
	private List<BuilderPieceSet.PieceInfo> piecesInSet = new List<BuilderPieceSet.PieceInfo>(10);

	// Token: 0x04003328 RID: 13096
	private bool animatingShelf;

	// Token: 0x04003329 RID: 13097
	private double timeToClearShelf = double.MaxValue;

	// Token: 0x0400332A RID: 13098
	private int dispenserToClear;

	// Token: 0x0400332B RID: 13099
	private int dispenserToUpdate;

	// Token: 0x0400332C RID: 13100
	private bool shouldVerifySetSelection;
}
