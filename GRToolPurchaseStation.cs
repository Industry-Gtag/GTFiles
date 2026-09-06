using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200081C RID: 2076
public class GRToolPurchaseStation : MonoBehaviour
{
	// Token: 0x170004BB RID: 1211
	// (get) Token: 0x06003540 RID: 13632 RVA: 0x00124AE5 File Offset: 0x00122CE5
	public int ActiveEntryIndex
	{
		get
		{
			return this.activeEntryIndex;
		}
	}

	// Token: 0x06003541 RID: 13633 RVA: 0x00124AED File Offset: 0x00122CED
	public void Init(GhostReactorManager grManager, GhostReactor reactor)
	{
		this.grManager = grManager;
		this.reactor = reactor;
	}

	// Token: 0x06003542 RID: 13634 RVA: 0x00124AFD File Offset: 0x00122CFD
	public void RequestPurchaseButton(int actorNumber)
	{
		if (actorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.grManager.ToolPurchaseStationRequest(this.PurchaseStationId, GhostReactorManager.ToolPurchaseStationAction.TryPurchase);
		}
	}

	// Token: 0x06003543 RID: 13635 RVA: 0x00124B23 File Offset: 0x00122D23
	public void ShiftRightButton()
	{
		this.grManager.ToolPurchaseStationRequest(this.PurchaseStationId, GhostReactorManager.ToolPurchaseStationAction.ShiftRight);
	}

	// Token: 0x06003544 RID: 13636 RVA: 0x00124B37 File Offset: 0x00122D37
	public void ShiftLeftButton()
	{
		this.grManager.ToolPurchaseStationRequest(this.PurchaseStationId, GhostReactorManager.ToolPurchaseStationAction.ShiftLeft);
	}

	// Token: 0x06003545 RID: 13637 RVA: 0x00124B4B File Offset: 0x00122D4B
	public void ShiftRightAuthority()
	{
		this.activeEntryIndex = (this.activeEntryIndex + 1) % this.toolEntries.Count;
	}

	// Token: 0x06003546 RID: 13638 RVA: 0x00124B67 File Offset: 0x00122D67
	public void ShiftLeftAuthority()
	{
		this.activeEntryIndex = ((this.activeEntryIndex > 0) ? (this.activeEntryIndex - 1) : (this.toolEntries.Count - 1));
	}

	// Token: 0x06003547 RID: 13639 RVA: 0x00124B90 File Offset: 0x00122D90
	public void DebugPurchase()
	{
		int entityTypeId = this.toolEntries[this.activeEntryIndex].GetEntityTypeId();
		Vector3 localPosition = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localPosition;
		Quaternion localRotation = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localRotation;
		Quaternion quaternion = this.depositTransform.rotation * localRotation;
		Vector3 vector = this.depositTransform.position + this.depositTransform.rotation * localPosition;
		this.grManager.gameEntityManager.RequestCreateItem(entityTypeId, vector, quaternion, 0L);
		this.OnPurchaseSucceeded();
	}

	// Token: 0x06003548 RID: 13640 RVA: 0x00124C50 File Offset: 0x00122E50
	public bool TryPurchaseAuthority(GRPlayer player, out int itemCost)
	{
		int entityTypeId = this.toolEntries[this.activeEntryIndex].GetEntityTypeId();
		itemCost = this.reactor.GetItemCost(entityTypeId);
		if (this.debugIgnoreToolCost || player.ShiftCredits >= itemCost)
		{
			Vector3 localPosition = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localPosition;
			Quaternion localRotation = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localRotation;
			Quaternion quaternion = this.depositTransform.rotation * localRotation;
			Vector3 vector = this.depositTransform.position + this.depositTransform.rotation * localPosition;
			this.grManager.gameEntityManager.RequestCreateItem(entityTypeId, vector, quaternion, 0L);
			return true;
		}
		return false;
	}

	// Token: 0x06003549 RID: 13641 RVA: 0x00124D30 File Offset: 0x00122F30
	public void OnSelectionUpdate(int newSelectedIndex)
	{
		this.activeEntryIndex = Mathf.Clamp(newSelectedIndex % this.toolEntries.Count, 0, this.toolEntries.Count - 1);
		this.audioSource.PlayOneShot(this.nextItemAudio, this.nextItemVolume);
		this.displayItemNameText.text = this.toolEntries[this.activeEntryIndex].toolName;
		this.displayItemCostText.text = this.toolEntries[this.activeEntryIndex].toolCost.ToString();
	}

	// Token: 0x0600354A RID: 13642 RVA: 0x00124DC4 File Offset: 0x00122FC4
	public void OnPurchaseSucceeded()
	{
		this.animatingDeposit = true;
		this.animationStartTime = Time.time;
		this.audioSource.PlayOneShot(this.purchaseAudio, this.purchaseVolume);
		UnityEvent onSucceeded = this.idCardScanner.onSucceeded;
		if (onSucceeded != null)
		{
			onSucceeded.Invoke();
		}
		if (this.displayedEntryIndex < 0 || this.displayedEntryIndex >= this.toolEntries.Count)
		{
			this.displayedEntryIndex = this.activeEntryIndex;
		}
	}

	// Token: 0x0600354B RID: 13643 RVA: 0x00124E38 File Offset: 0x00123038
	public void OnPurchaseFailed()
	{
		this.audioSource.PlayOneShot(this.purchaseFailedAudio, this.purchaseFailedVolume);
		UnityEvent onFailed = this.idCardScanner.onFailed;
		if (onFailed == null)
		{
			return;
		}
		onFailed.Invoke();
	}

	// Token: 0x0600354C RID: 13644 RVA: 0x00124E66 File Offset: 0x00123066
	public Transform GetSpawnMarker()
	{
		return this.toolSpawnLocation;
	}

	// Token: 0x0600354D RID: 13645 RVA: 0x00124E6E File Offset: 0x0012306E
	public string GetCurrentToolName()
	{
		return this.toolEntries[this.activeEntryIndex].toolName;
	}

	// Token: 0x0600354E RID: 13646 RVA: 0x00124E86 File Offset: 0x00123086
	private void Awake()
	{
		this.depositLidOpenRot = Quaternion.Euler(this.depositLidOpenEuler);
		this.toolEntryRot = Quaternion.Euler(this.toolEntryRotEuler);
		this.toolExitRot = Quaternion.Euler(this.toolExitRotEuler);
	}

	// Token: 0x0600354F RID: 13647 RVA: 0x00124EBC File Offset: 0x001230BC
	private void Update()
	{
		if (!this.animatingSwap && !this.animatingDeposit && this.activeEntryIndex != this.displayedEntryIndex)
		{
			this.animatingSwap = true;
			this.animationStartTime = Time.time;
			this.animPrevToolIndex = this.displayedEntryIndex;
			this.animNextToolIndex = this.activeEntryIndex;
			this.toolEntryRot = Quaternion.AngleAxis(this.toolEntryRotDegrees, Random.onUnitSphere);
		}
		if (this.animatingSwap)
		{
			float num = (Time.time - this.animationStartTime) / this.nextToolAnimationTime;
			Transform transform = null;
			if (this.animPrevToolIndex >= 0 && this.animPrevToolIndex < this.toolEntries.Count)
			{
				transform = this.toolEntries[this.animPrevToolIndex].displayToolParent;
				transform.localRotation = Quaternion.Slerp(Quaternion.identity, this.toolExitRot, this.toolExitRotTimingCurve.Evaluate(num));
				transform.localPosition = Vector3.Lerp(Vector3.zero, this.toolExitPosOffset, this.toolExitPosTimingCurve.Evaluate(num));
			}
			Transform displayToolParent = this.toolEntries[this.animNextToolIndex].displayToolParent;
			displayToolParent.localRotation = Quaternion.Slerp(this.toolEntryRot, Quaternion.identity, this.toolEntryRotTimingCurve.Evaluate(num));
			displayToolParent.localPosition = Vector3.Lerp(this.toolEntryPosOffset, Vector3.zero, this.toolEntryPosTimingCurve.Evaluate(num));
			displayToolParent.gameObject.SetActive(true);
			if (num >= 1f)
			{
				if (transform != null)
				{
					transform.gameObject.SetActive(false);
				}
				this.displayedEntryIndex = this.animNextToolIndex;
				this.animatingSwap = false;
				return;
			}
		}
		else if (this.animatingDeposit)
		{
			float num2 = (Time.time - this.animationStartTime) / this.toolDepositAnimationTime;
			Transform displayToolParent2 = this.toolEntries[this.displayedEntryIndex].displayToolParent;
			Vector3 localPosition = displayToolParent2.localPosition;
			localPosition.y = Mathf.Lerp(0f, this.depositTransform.localPosition.y, this.toolDepositMotionCurveY.Evaluate(this.toolDepositTimingCurve.Evaluate(num2)));
			localPosition.z = Mathf.Lerp(0f, this.depositTransform.localPosition.z, this.toolDepositMotionCurveZ.Evaluate(this.toolDepositTimingCurve.Evaluate(num2)));
			displayToolParent2.localPosition = localPosition;
			this.depositLidTransform.localRotation = Quaternion.Slerp(Quaternion.identity, this.depositLidOpenRot, this.depositLidTimingCurve.Evaluate(num2));
			if (num2 >= 1f)
			{
				this.depositLidTransform.localRotation = Quaternion.identity;
				displayToolParent2.gameObject.SetActive(false);
				this.displayedEntryIndex = -1;
				this.animatingDeposit = false;
			}
		}
	}

	// Token: 0x04004557 RID: 17751
	[SerializeField]
	private List<GRToolPurchaseStation.ToolEntry> toolEntries = new List<GRToolPurchaseStation.ToolEntry>();

	// Token: 0x04004558 RID: 17752
	[SerializeField]
	private Transform displayTransform;

	// Token: 0x04004559 RID: 17753
	[SerializeField]
	private Transform depositTransform;

	// Token: 0x0400455A RID: 17754
	[SerializeField]
	private Transform toolSpawnLocation;

	// Token: 0x0400455B RID: 17755
	[SerializeField]
	private TMP_Text displayItemNameText;

	// Token: 0x0400455C RID: 17756
	[SerializeField]
	private TMP_Text displayItemCostText;

	// Token: 0x0400455D RID: 17757
	[SerializeField]
	private float nextToolAnimationTime = 0.5f;

	// Token: 0x0400455E RID: 17758
	[SerializeField]
	private float toolDepositAnimationTime = 1f;

	// Token: 0x0400455F RID: 17759
	[SerializeField]
	private Vector3 toolEntryPosOffset = new Vector3(0f, 0.25f, 0f);

	// Token: 0x04004560 RID: 17760
	[SerializeField]
	private Vector3 toolEntryRotEuler = new Vector3(0f, 0f, 15f);

	// Token: 0x04004561 RID: 17761
	[SerializeField]
	private float toolEntryRotDegrees = 15f;

	// Token: 0x04004562 RID: 17762
	[SerializeField]
	private Vector3 toolExitPosOffset = new Vector3(0f, 0f, -0.25f);

	// Token: 0x04004563 RID: 17763
	[SerializeField]
	private Vector3 toolExitRotEuler = new Vector3(180f, 0f, 0f);

	// Token: 0x04004564 RID: 17764
	[SerializeField]
	private AnimationCurve toolEntryPosTimingCurve;

	// Token: 0x04004565 RID: 17765
	[SerializeField]
	private AnimationCurve toolEntryRotTimingCurve;

	// Token: 0x04004566 RID: 17766
	[SerializeField]
	private AnimationCurve toolExitPosTimingCurve;

	// Token: 0x04004567 RID: 17767
	[SerializeField]
	private AnimationCurve toolExitRotTimingCurve;

	// Token: 0x04004568 RID: 17768
	[SerializeField]
	private AnimationCurve toolDepositTimingCurve;

	// Token: 0x04004569 RID: 17769
	[SerializeField]
	private AnimationCurve toolDepositMotionCurveY;

	// Token: 0x0400456A RID: 17770
	[SerializeField]
	private AnimationCurve toolDepositMotionCurveZ;

	// Token: 0x0400456B RID: 17771
	[SerializeField]
	private Transform depositLidTransform;

	// Token: 0x0400456C RID: 17772
	[SerializeField]
	private Vector3 depositLidOpenEuler = new Vector3(65f, 0f, 0f);

	// Token: 0x0400456D RID: 17773
	[SerializeField]
	private AnimationCurve depositLidTimingCurve;

	// Token: 0x0400456E RID: 17774
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400456F RID: 17775
	[SerializeField]
	private AudioClip nextItemAudio;

	// Token: 0x04004570 RID: 17776
	[SerializeField]
	private float nextItemVolume = 0.5f;

	// Token: 0x04004571 RID: 17777
	[SerializeField]
	private AudioClip purchaseAudio;

	// Token: 0x04004572 RID: 17778
	[SerializeField]
	private float purchaseVolume = 0.5f;

	// Token: 0x04004573 RID: 17779
	[SerializeField]
	private AudioClip purchaseFailedAudio;

	// Token: 0x04004574 RID: 17780
	[SerializeField]
	private float purchaseFailedVolume = 0.5f;

	// Token: 0x04004575 RID: 17781
	[SerializeField]
	private IDCardScanner idCardScanner;

	// Token: 0x04004576 RID: 17782
	private int activeEntryIndex = 1;

	// Token: 0x04004577 RID: 17783
	private int displayedEntryIndex = -1;

	// Token: 0x04004578 RID: 17784
	private float animationStartTime;

	// Token: 0x04004579 RID: 17785
	private bool animatingDeposit;

	// Token: 0x0400457A RID: 17786
	private bool animatingSwap;

	// Token: 0x0400457B RID: 17787
	private int animPrevToolIndex;

	// Token: 0x0400457C RID: 17788
	private int animNextToolIndex;

	// Token: 0x0400457D RID: 17789
	private Quaternion depositLidOpenRot = Quaternion.identity;

	// Token: 0x0400457E RID: 17790
	private Quaternion toolEntryRot = Quaternion.identity;

	// Token: 0x0400457F RID: 17791
	private Quaternion toolExitRot = Quaternion.identity;

	// Token: 0x04004580 RID: 17792
	private Coroutine vendingCoroutine;

	// Token: 0x04004581 RID: 17793
	private bool debugIgnoreToolCost;

	// Token: 0x04004582 RID: 17794
	[HideInInspector]
	public int PurchaseStationId;

	// Token: 0x04004583 RID: 17795
	private GhostReactorManager grManager;

	// Token: 0x04004584 RID: 17796
	private GhostReactor reactor;

	// Token: 0x0200081D RID: 2077
	[Serializable]
	public struct ToolEntry
	{
		// Token: 0x06003551 RID: 13649 RVA: 0x00125279 File Offset: 0x00123479
		public int GetEntityTypeId()
		{
			if (!this.entityTypeIdSet)
			{
				this.entityTypeId = this.entityPrefab.gameObject.name.GetStaticHash();
				this.entityTypeIdSet = true;
			}
			return this.entityTypeId;
		}

		// Token: 0x04004585 RID: 17797
		public Transform displayToolParent;

		// Token: 0x04004586 RID: 17798
		public GameEntity entityPrefab;

		// Token: 0x04004587 RID: 17799
		public string toolName;

		// Token: 0x04004588 RID: 17800
		public int toolCost;

		// Token: 0x04004589 RID: 17801
		private int entityTypeId;

		// Token: 0x0400458A RID: 17802
		private bool entityTypeIdSet;
	}
}
