using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000648 RID: 1608
public class BuilderPiecePrivatePlot : MonoBehaviour
{
	// Token: 0x0600283F RID: 10303 RVA: 0x000D7B5A File Offset: 0x000D5D5A
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x06002840 RID: 10304 RVA: 0x000D7B64 File Offset: 0x000D5D64
	private void Init()
	{
		if (this.initDone)
		{
			return;
		}
		this.materialProps = new MaterialPropertyBlock();
		this.usedResources = new int[3];
		for (int i = 0; i < this.usedResources.Length; i++)
		{
			this.usedResources[i] = 0;
		}
		this.tempResourceCount = new int[3];
		this.piece = base.GetComponent<BuilderPiece>();
		this.SetPlotState(BuilderPiecePrivatePlot.PlotState.Vacant);
		this.piecesToCount = new Queue<BuilderPiece>(1024);
		this.initDone = true;
		this.privatePlotIndex = -1;
	}

	// Token: 0x06002841 RID: 10305 RVA: 0x000D7BEC File Offset: 0x000D5DEC
	private void Start()
	{
		if (this.piece != null && this.piece.GetTable() != null)
		{
			BuilderTable table = this.piece.GetTable();
			this.doesLocalPlayerOwnAPlot = table.DoesPlayerOwnPlot(PhotonNetwork.LocalPlayer.ActorNumber);
			table.OnLocalPlayerClaimedPlot.AddListener(new UnityAction<bool>(this.OnLocalPlayerClaimedPlot));
			this.UpdateVisuals();
			foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
			{
				builderResourceMeter.table = this.piece.GetTable();
			}
		}
		this.buildArea.gameObject.SetActive(true);
		this.buildArea.enabled = true;
		this.buildAreaBounds = this.buildArea.bounds;
		this.buildArea.gameObject.SetActive(false);
		this.buildArea.enabled = false;
		this.zoneRenderers.Clear();
		this.zoneRenderers.Add(this.tmpLabel.GetComponent<Renderer>());
		foreach (BuilderResourceMeter builderResourceMeter2 in this.resourceMeters)
		{
			this.zoneRenderers.AddRange(builderResourceMeter2.GetComponentsInChildren<Renderer>());
		}
		this.zoneRenderers.AddRange(this.borderMeshes);
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.inBuilderZone = true;
		this.OnZoneChanged();
	}

	// Token: 0x06002842 RID: 10306 RVA: 0x000D7DA8 File Offset: 0x000D5FA8
	private void OnDestroy()
	{
		if (this.piece != null && this.piece.GetTable() != null)
		{
			this.piece.GetTable().OnLocalPlayerClaimedPlot.RemoveListener(new UnityAction<bool>(this.OnLocalPlayerClaimedPlot));
		}
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x06002843 RID: 10307 RVA: 0x000D7E2C File Offset: 0x000D602C
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(this.piece.GetTable().tableZone);
		this.inBuilderZone = flag;
	}

	// Token: 0x06002844 RID: 10308 RVA: 0x000D7E5B File Offset: 0x000D605B
	private void OnLocalPlayerClaimedPlot(bool claim)
	{
		this.doesLocalPlayerOwnAPlot = claim;
		this.UpdateVisuals();
	}

	// Token: 0x06002845 RID: 10309 RVA: 0x000D7E6C File Offset: 0x000D606C
	public void UpdatePlot()
	{
		if (BuilderPieceInteractor.instance == null || BuilderPieceInteractor.instance.heldChainLength == null || BuilderPieceInteractor.instance.heldChainLength.Length < 2)
		{
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (!this.initDone)
		{
			this.Init();
		}
		if ((this.plotState == BuilderPiecePrivatePlot.PlotState.Occupied && this.owningPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber) || (this.plotState == BuilderPiecePrivatePlot.PlotState.Vacant && !this.doesLocalPlayerOwnAPlot))
		{
			BuilderPiece parentPiece = BuilderPieceInteractor.instance.prevPotentialPlacement[0].parentPiece;
			BuilderPiece parentPiece2 = BuilderPieceInteractor.instance.prevPotentialPlacement[1].parentPiece;
			bool flag = false;
			if (parentPiece == null && this.leftPotentialParent != null)
			{
				this.isLeftOverPlot = false;
				this.leftPotentialParent = null;
				flag = true;
			}
			else if ((this.leftPotentialParent == null && parentPiece != null) || (parentPiece != null && !parentPiece.Equals(this.leftPotentialParent)))
			{
				BuilderPiece attachedBuiltInPiece = parentPiece.GetAttachedBuiltInPiece();
				this.isLeftOverPlot = attachedBuiltInPiece != null && attachedBuiltInPiece.Equals(this.piece);
				this.leftPotentialParent = parentPiece;
				flag = true;
			}
			if (parentPiece2 == null && this.rightPotentialParent != null)
			{
				this.isRightOverPlot = false;
				this.rightPotentialParent = null;
				flag = true;
			}
			else if ((this.rightPotentialParent == null && parentPiece2 != null) || (parentPiece2 != null && !parentPiece2.Equals(this.rightPotentialParent)))
			{
				BuilderPiece attachedBuiltInPiece2 = parentPiece2.GetAttachedBuiltInPiece();
				this.isRightOverPlot = attachedBuiltInPiece2 != null && attachedBuiltInPiece2.Equals(this.piece);
				this.rightPotentialParent = parentPiece2;
				flag = true;
			}
			if (flag)
			{
				this.UpdateVisuals();
			}
		}
		else if (this.isRightOverPlot || this.isLeftOverPlot)
		{
			this.isRightOverPlot = false;
			this.isLeftOverPlot = false;
			this.UpdateVisuals();
		}
		foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
		{
			builderResourceMeter.UpdateMeterFill();
		}
	}

	// Token: 0x06002846 RID: 10310 RVA: 0x000D80A4 File Offset: 0x000D62A4
	public void RecountPlotCost()
	{
		this.Init();
		this.piece.GetChainCost(this.usedResources);
		this.UpdateVisuals();
	}

	// Token: 0x06002847 RID: 10311 RVA: 0x000D80C3 File Offset: 0x000D62C3
	public void OnPieceAttachedToPlot(BuilderPiece attachPiece)
	{
		this.AddChainResourcesToCount(attachPiece, true);
		this.UpdateVisuals();
	}

	// Token: 0x06002848 RID: 10312 RVA: 0x000D80D3 File Offset: 0x000D62D3
	public void OnPieceDetachedFromPlot(BuilderPiece detachPiece)
	{
		this.AddChainResourcesToCount(detachPiece, false);
		this.UpdateVisuals();
	}

	// Token: 0x06002849 RID: 10313 RVA: 0x000D80E3 File Offset: 0x000D62E3
	public void ChangeAttachedPieceCount(int delta)
	{
		this.attachedPieceCount += delta;
		this.UpdateVisuals();
	}

	// Token: 0x0600284A RID: 10314 RVA: 0x000D80FC File Offset: 0x000D62FC
	public void AddChainResourcesToCount(BuilderPiece chain, bool attach)
	{
		if (chain == null)
		{
			return;
		}
		this.piecesToCount.Clear();
		for (int i = 0; i < this.tempResourceCount.Length; i++)
		{
			this.tempResourceCount[i] = 0;
		}
		this.piecesToCount.Enqueue(chain);
		this.AddPieceCostToArray(chain, this.tempResourceCount);
		bool flag = false;
		while (this.piecesToCount.Count > 0 && !flag)
		{
			BuilderPiece builderPiece = this.piecesToCount.Dequeue().firstChildPiece;
			while (builderPiece != null)
			{
				this.piecesToCount.Enqueue(builderPiece);
				if (!this.AddPieceCostToArray(builderPiece, this.tempResourceCount))
				{
					Debug.LogWarning("Builder plot placing pieces over limits");
					flag = true;
					break;
				}
				builderPiece = builderPiece.nextSiblingPiece;
			}
		}
		for (int j = 0; j < this.usedResources.Length; j++)
		{
			if (attach)
			{
				this.usedResources[j] += this.tempResourceCount[j];
			}
			else
			{
				this.usedResources[j] -= this.tempResourceCount[j];
			}
		}
	}

	// Token: 0x0600284B RID: 10315 RVA: 0x000D81FD File Offset: 0x000D63FD
	public void ClaimPlotForPlayerNumber(int player)
	{
		this.owningPlayerActorNumber = player;
		this.SetPlotState(BuilderPiecePrivatePlot.PlotState.Occupied);
	}

	// Token: 0x0600284C RID: 10316 RVA: 0x000D820D File Offset: 0x000D640D
	public int GetOwnerActorNumber()
	{
		return this.owningPlayerActorNumber;
	}

	// Token: 0x0600284D RID: 10317 RVA: 0x000D8218 File Offset: 0x000D6418
	public void ClearPlot()
	{
		this.Init();
		this.attachedPieceCount = 0;
		for (int i = 0; i < this.usedResources.Length; i++)
		{
			this.usedResources[i] = 0;
		}
		this.SetPlotState(BuilderPiecePrivatePlot.PlotState.Vacant);
	}

	// Token: 0x0600284E RID: 10318 RVA: 0x000D8255 File Offset: 0x000D6455
	public void FreePlot()
	{
		this.SetPlotState(BuilderPiecePrivatePlot.PlotState.Vacant);
	}

	// Token: 0x0600284F RID: 10319 RVA: 0x000D825E File Offset: 0x000D645E
	public bool IsPlotClaimed()
	{
		return this.plotState > BuilderPiecePrivatePlot.PlotState.Vacant;
	}

	// Token: 0x06002850 RID: 10320 RVA: 0x000D826C File Offset: 0x000D646C
	public bool IsChainUnderCapacity(BuilderPiece chain)
	{
		if (chain == null)
		{
			return true;
		}
		this.piecesToCount.Clear();
		for (int i = 0; i < this.tempResourceCount.Length; i++)
		{
			this.tempResourceCount[i] = this.usedResources[i];
		}
		this.piecesToCount.Enqueue(chain);
		if (!this.AddPieceCostToArray(chain, this.tempResourceCount))
		{
			return false;
		}
		while (this.piecesToCount.Count > 0)
		{
			BuilderPiece builderPiece = this.piecesToCount.Dequeue().firstChildPiece;
			while (builderPiece != null)
			{
				this.piecesToCount.Enqueue(builderPiece);
				if (!this.AddPieceCostToArray(builderPiece, this.tempResourceCount))
				{
					return false;
				}
				builderPiece = builderPiece.nextSiblingPiece;
			}
		}
		return true;
	}

	// Token: 0x06002851 RID: 10321 RVA: 0x000D8320 File Offset: 0x000D6520
	public bool AddPieceCostToArray(BuilderPiece addedPiece, int[] array)
	{
		if (addedPiece == null)
		{
			return true;
		}
		if (addedPiece.cost != null)
		{
			foreach (BuilderResourceQuantity builderResourceQuantity in addedPiece.cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					array[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
					if (array[(int)builderResourceQuantity.type] > this.piece.GetTable().GetPrivateResourceLimitForType((int)builderResourceQuantity.type))
					{
						return false;
					}
				}
			}
			return true;
		}
		return true;
	}

	// Token: 0x06002852 RID: 10322 RVA: 0x000D83DC File Offset: 0x000D65DC
	public bool CanPlayerAttachToPlot(int actorNumber)
	{
		return (this.plotState == BuilderPiecePrivatePlot.PlotState.Occupied && this.owningPlayerActorNumber == actorNumber) || (this.plotState == BuilderPiecePrivatePlot.PlotState.Vacant && !this.piece.GetTable().DoesPlayerOwnPlot(actorNumber));
	}

	// Token: 0x06002853 RID: 10323 RVA: 0x000D8410 File Offset: 0x000D6610
	public bool CanPlayerGrabFromPlot(int actorNumber, Vector3 worldPosition)
	{
		if (this.owningPlayerActorNumber == actorNumber || this.plotState == BuilderPiecePrivatePlot.PlotState.Vacant)
		{
			return true;
		}
		int num;
		if (this.piece.GetTable().plotOwners.TryGetValue(actorNumber, out num))
		{
			BuilderPiece builderPiece = this.piece.GetTable().GetPiece(num);
			BuilderPiecePrivatePlot builderPiecePrivatePlot;
			if (builderPiece != null && builderPiece.TryGetPlotComponent(out builderPiecePrivatePlot))
			{
				return builderPiecePrivatePlot.IsLocationWithinPlotExtents(worldPosition);
			}
		}
		return false;
	}

	// Token: 0x06002854 RID: 10324 RVA: 0x000D8478 File Offset: 0x000D6678
	private void SetPlotState(BuilderPiecePrivatePlot.PlotState newState)
	{
		this.plotState = newState;
		BuilderPiecePrivatePlot.PlotState plotState = this.plotState;
		if (plotState != BuilderPiecePrivatePlot.PlotState.Vacant)
		{
			if (plotState == BuilderPiecePrivatePlot.PlotState.Occupied)
			{
				if (this.tmpLabel != null && NetworkSystem.Instance != null)
				{
					string text = string.Empty;
					NetPlayer player = NetworkSystem.Instance.GetPlayer(this.owningPlayerActorNumber);
					RigContainer rigContainer;
					if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
					{
						text = rigContainer.Rig.playerNameVisible;
					}
					if (string.IsNullOrEmpty(text) && !this.tmpLabel.text.Equals("OCCUPIED"))
					{
						this.tmpLabel.text = "OCCUPIED";
					}
					else if (!this.tmpLabel.text.Equals(text))
					{
						this.tmpLabel.text = text;
					}
				}
				else if (this.tmpLabel != null && !this.tmpLabel.text.Equals("OCCUPIED"))
				{
					this.tmpLabel.text = "OCCUPIED";
				}
			}
		}
		else
		{
			this.owningPlayerActorNumber = -1;
			if (this.tmpLabel != null && !this.tmpLabel.text.Equals(string.Empty))
			{
				this.tmpLabel.text = string.Empty;
			}
		}
		this.UpdateVisuals();
	}

	// Token: 0x06002855 RID: 10325 RVA: 0x000D85CC File Offset: 0x000D67CC
	public bool IsLocationWithinPlotExtents(Vector3 worldPosition)
	{
		if (!this.buildAreaBounds.Contains(worldPosition))
		{
			return false;
		}
		Vector3 vector = this.buildArea.transform.InverseTransformPoint(worldPosition);
		Vector3 vector2 = this.buildArea.center + this.buildArea.size / 2f;
		Vector3 vector3 = this.buildArea.center - this.buildArea.size / 2f;
		return vector.x >= vector3.x && vector.x <= vector2.x && vector.y >= vector3.y && vector.y <= vector2.y && vector.z >= vector3.z && vector.z <= vector2.z;
	}

	// Token: 0x06002856 RID: 10326 RVA: 0x000D86A0 File Offset: 0x000D68A0
	public void OnAvailableResourceChange()
	{
		this.UpdateVisuals();
	}

	// Token: 0x06002857 RID: 10327 RVA: 0x000D86A8 File Offset: 0x000D68A8
	private void UpdateVisuals()
	{
		if (this.usedResources == null || this.piece.GetTable() == null)
		{
			return;
		}
		BuilderPiecePrivatePlot.PlotState plotState = this.plotState;
		if (plotState != BuilderPiecePrivatePlot.PlotState.Vacant)
		{
			if (plotState != BuilderPiecePrivatePlot.PlotState.Occupied)
			{
				return;
			}
			if (this.owningPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				this.UpdateVisualsForOwner();
				return;
			}
			this.SetBorderColor(this.placementDisallowedColor);
			int num = 0;
			while (num < this.resourceMeters.Count && num < 3)
			{
				int privateResourceLimitForType = this.piece.GetTable().GetPrivateResourceLimitForType(num);
				if (privateResourceLimitForType != 0)
				{
					this.resourceMeters[num].SetNormalizedFillTarget((float)(privateResourceLimitForType - this.usedResources[num]) / (float)privateResourceLimitForType);
				}
				num++;
			}
		}
		else
		{
			if (!this.doesLocalPlayerOwnAPlot)
			{
				this.UpdateVisualsForOwner();
				return;
			}
			this.SetBorderColor(this.placementDisallowedColor);
			for (int i = 0; i < this.resourceMeters.Count; i++)
			{
				if (i >= 3)
				{
					return;
				}
				int privateResourceLimitForType2 = this.piece.GetTable().GetPrivateResourceLimitForType(i);
				if (privateResourceLimitForType2 != 0)
				{
					this.resourceMeters[i].SetNormalizedFillTarget((float)(privateResourceLimitForType2 - this.usedResources[i]) / (float)privateResourceLimitForType2);
				}
			}
			return;
		}
	}

	// Token: 0x06002858 RID: 10328 RVA: 0x000D87C4 File Offset: 0x000D69C4
	private void UpdateVisualsForOwner()
	{
		bool flag = true;
		if (this.usedResources == null)
		{
			return;
		}
		if (BuilderPieceInteractor.instance == null || BuilderPieceInteractor.instance.heldChainCost == null)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			int num = this.usedResources[i];
			if (this.isLeftOverPlot)
			{
				num += BuilderPieceInteractor.instance.heldChainCost[0][i];
			}
			if (this.isRightOverPlot)
			{
				num += BuilderPieceInteractor.instance.heldChainCost[1][i];
			}
			int privateResourceLimitForType = this.piece.GetTable().GetPrivateResourceLimitForType(i);
			if (num < privateResourceLimitForType)
			{
				flag = false;
			}
			if (privateResourceLimitForType != 0 && this.resourceMeters.Count > i)
			{
				this.resourceMeters[i].SetNormalizedFillTarget((float)(privateResourceLimitForType - num) / (float)privateResourceLimitForType);
			}
		}
		if (flag)
		{
			this.SetBorderColor(this.placementDisallowedColor);
			return;
		}
		this.SetBorderColor(this.placementAllowedColor);
	}

	// Token: 0x06002859 RID: 10329 RVA: 0x000D88B0 File Offset: 0x000D6AB0
	private void SetBorderColor(Color color)
	{
		this.borderMeshes[0].GetPropertyBlock(this.materialProps);
		this.materialProps.SetColor(ShaderProps._BaseColor, color);
		foreach (MeshRenderer meshRenderer in this.borderMeshes)
		{
			meshRenderer.SetPropertyBlock(this.materialProps);
		}
	}

	// Token: 0x04003419 RID: 13337
	[SerializeField]
	private Color placementAllowedColor;

	// Token: 0x0400341A RID: 13338
	[SerializeField]
	private Color placementDisallowedColor;

	// Token: 0x0400341B RID: 13339
	[SerializeField]
	private Color overCapacityColor;

	// Token: 0x0400341C RID: 13340
	public List<MeshRenderer> borderMeshes;

	// Token: 0x0400341D RID: 13341
	public BoxCollider buildArea;

	// Token: 0x0400341E RID: 13342
	[SerializeField]
	private TMP_Text tmpLabel;

	// Token: 0x0400341F RID: 13343
	[SerializeField]
	private List<BuilderResourceMeter> resourceMeters;

	// Token: 0x04003420 RID: 13344
	[NonSerialized]
	public int[] usedResources;

	// Token: 0x04003421 RID: 13345
	[NonSerialized]
	public int[] tempResourceCount;

	// Token: 0x04003422 RID: 13346
	[SerializeField]
	private GameObject plotClaimedFX;

	// Token: 0x04003423 RID: 13347
	private BuilderPiece leftPotentialParent;

	// Token: 0x04003424 RID: 13348
	private BuilderPiece rightPotentialParent;

	// Token: 0x04003425 RID: 13349
	private bool isLeftOverPlot;

	// Token: 0x04003426 RID: 13350
	private bool isRightOverPlot;

	// Token: 0x04003427 RID: 13351
	private Bounds buildAreaBounds;

	// Token: 0x04003428 RID: 13352
	[HideInInspector]
	public BuilderPiece piece;

	// Token: 0x04003429 RID: 13353
	private int owningPlayerActorNumber;

	// Token: 0x0400342A RID: 13354
	private int attachedPieceCount;

	// Token: 0x0400342B RID: 13355
	[HideInInspector]
	public int privatePlotIndex;

	// Token: 0x0400342C RID: 13356
	[HideInInspector]
	public BuilderPiecePrivatePlot.PlotState plotState;

	// Token: 0x0400342D RID: 13357
	private bool doesLocalPlayerOwnAPlot;

	// Token: 0x0400342E RID: 13358
	private Queue<BuilderPiece> piecesToCount;

	// Token: 0x0400342F RID: 13359
	private bool initDone;

	// Token: 0x04003430 RID: 13360
	private MaterialPropertyBlock materialProps;

	// Token: 0x04003431 RID: 13361
	private List<Renderer> zoneRenderers = new List<Renderer>(12);

	// Token: 0x04003432 RID: 13362
	private bool inBuilderZone;

	// Token: 0x02000649 RID: 1609
	public enum PlotState
	{
		// Token: 0x04003434 RID: 13364
		Vacant,
		// Token: 0x04003435 RID: 13365
		Occupied
	}
}
