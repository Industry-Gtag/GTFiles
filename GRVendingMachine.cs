using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200083D RID: 2109
public class GRVendingMachine : MonoBehaviour
{
	// Token: 0x06003635 RID: 13877 RVA: 0x0012BC3D File Offset: 0x00129E3D
	public void Setup(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x06003636 RID: 13878 RVA: 0x0012BC46 File Offset: 0x00129E46
	public Transform GetSpawnMarker()
	{
		return this.itemSpawnLocation;
	}

	// Token: 0x06003637 RID: 13879 RVA: 0x0012BC4E File Offset: 0x00129E4E
	public void NavButtonPressedLeft()
	{
		this.hIndex = Mathf.Max(0, this.hIndex - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x06003638 RID: 13880 RVA: 0x0012BC6A File Offset: 0x00129E6A
	public void NavButtonPressedRight()
	{
		this.hIndex = Mathf.Min(this.hIndex + 1, this.horizontalSteps - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x06003639 RID: 13881 RVA: 0x0012BC8D File Offset: 0x00129E8D
	public void NavButtonPressedUp()
	{
		this.vIndex = Mathf.Max(0, this.vIndex - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x0600363A RID: 13882 RVA: 0x0012BCA9 File Offset: 0x00129EA9
	public void NavButtonPressedDown()
	{
		this.vIndex = Mathf.Min(this.vIndex + 1, this.verticalSteps - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x0600363B RID: 13883 RVA: 0x0012BCCC File Offset: 0x00129ECC
	public void RequestPurchase()
	{
		if (!this.currentlyVending)
		{
			int num = this.vIndex * this.horizontalSteps + this.hIndex;
			if (num >= 0 && num < this.vendingEntries.Count)
			{
				this.vendingIndex = num;
				if (this.vendingCoroutine != null)
				{
					base.StopCoroutine(this.vendingCoroutine);
				}
				this.vendingCoroutine = base.StartCoroutine(this.VendingCoroutine());
			}
		}
	}

	// Token: 0x0600363C RID: 13884 RVA: 0x0012BD38 File Offset: 0x00129F38
	private void RefreshCardReaderDisplay()
	{
		int num = this.vIndex * this.horizontalSteps + this.hIndex;
		if (num >= 0 && num < this.vendingEntries.Count)
		{
			int entityTypeId = this.vendingEntries[num].GetEntityTypeId();
			int itemCost = this.reactor.GetItemCost(entityTypeId);
			this.cardDisplayText.text = this.vendingEntries[num].itemName + "\n" + itemCost.ToString();
		}
	}

	// Token: 0x0600363D RID: 13885 RVA: 0x0012BDBB File Offset: 0x00129FBB
	private void Update()
	{
		if (!this.currentlyVending)
		{
			this.MoveTransportToSlot(this.hIndex, this.vIndex, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime);
		}
	}

	// Token: 0x0600363E RID: 13886 RVA: 0x0012BDF8 File Offset: 0x00129FF8
	private bool MoveTransportToSlot(int x, int y, int rows, int cols, float xSpeed, float ySpeed, float dt)
	{
		Vector3 vector = Vector3.Lerp(this.horizontalMin.position, this.horizontalMax.position, (float)x / (float)(rows - 1));
		Vector3 vector2 = Vector3.Lerp(this.verticalMin.position, this.verticalMax.position, (float)y / (float)(cols - 1));
		this.horizontalTransport.position = Vector3.MoveTowards(this.horizontalTransport.position, vector, xSpeed * dt);
		this.verticalTransport.position = Vector3.MoveTowards(this.verticalTransport.position, vector2, ySpeed * dt);
		float sqrMagnitude = (this.horizontalTransport.position - vector).sqrMagnitude;
		float sqrMagnitude2 = (this.verticalTransport.position - vector2).sqrMagnitude;
		return sqrMagnitude > 0.001f || sqrMagnitude2 > 0.001f;
	}

	// Token: 0x0600363F RID: 13887 RVA: 0x0012BED2 File Offset: 0x0012A0D2
	private IEnumerator VendingCoroutine()
	{
		this.currentlyVending = true;
		while (this.MoveTransportToSlot(this.hIndex, this.vIndex, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime))
		{
			yield return null;
		}
		int entityTypeId = this.vendingEntries[this.vendingIndex].GetEntityTypeId();
		int itemCost = this.reactor.GetItemCost(entityTypeId);
		if (this.debugUnlimitedPurchasing || VRRig.LocalRig.GetComponent<GRPlayer>().ShiftCredits >= itemCost)
		{
			this.vendingEntries[this.vendingIndex].transportVisual.gameObject.SetActive(true);
			while (this.MoveTransportToSlot(this.horizontalSteps - 1, this.verticalSteps - 1, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime))
			{
				yield return null;
			}
			float depositPosSqDist = (this.horizontalTransport.position - this.depositLocation.position).sqrMagnitude;
			while (depositPosSqDist > 0.001f)
			{
				this.horizontalTransport.position = Vector3.MoveTowards(this.horizontalTransport.position, this.depositLocation.position, this.horizontalSpeed * Time.deltaTime);
				depositPosSqDist = (this.horizontalTransport.position - this.depositLocation.position).sqrMagnitude;
				yield return null;
			}
			this.vendingEntries[this.vendingIndex].transportVisual.gameObject.SetActive(false);
			while (this.MoveTransportToSlot(this.horizontalSteps - 1, this.verticalSteps - 1, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime))
			{
				yield return null;
			}
		}
		this.currentlyVending = false;
		yield break;
	}

	// Token: 0x040046E4 RID: 18148
	[SerializeField]
	private Transform horizontalTransport;

	// Token: 0x040046E5 RID: 18149
	[SerializeField]
	private Transform verticalTransport;

	// Token: 0x040046E6 RID: 18150
	[SerializeField]
	private Transform horizontalMin;

	// Token: 0x040046E7 RID: 18151
	[SerializeField]
	private Transform horizontalMax;

	// Token: 0x040046E8 RID: 18152
	[SerializeField]
	private Transform verticalMin;

	// Token: 0x040046E9 RID: 18153
	[SerializeField]
	private Transform verticalMax;

	// Token: 0x040046EA RID: 18154
	[SerializeField]
	private Transform depositLocation;

	// Token: 0x040046EB RID: 18155
	[SerializeField]
	private Transform itemSpawnLocation;

	// Token: 0x040046EC RID: 18156
	[SerializeField]
	private TMP_Text cardDisplayText;

	// Token: 0x040046ED RID: 18157
	[SerializeField]
	private int horizontalSteps = 4;

	// Token: 0x040046EE RID: 18158
	[SerializeField]
	private int verticalSteps = 3;

	// Token: 0x040046EF RID: 18159
	[SerializeField]
	private float horizontalSpeed = 0.25f;

	// Token: 0x040046F0 RID: 18160
	[SerializeField]
	private float verticalSpeed = 0.25f;

	// Token: 0x040046F1 RID: 18161
	[SerializeField]
	private bool debugUnlimitedPurchasing;

	// Token: 0x040046F2 RID: 18162
	[SerializeField]
	private List<GRVendingMachine.VendingEntry> vendingEntries = new List<GRVendingMachine.VendingEntry>();

	// Token: 0x040046F3 RID: 18163
	private int hIndex;

	// Token: 0x040046F4 RID: 18164
	private int vIndex;

	// Token: 0x040046F5 RID: 18165
	private bool currentlyVending;

	// Token: 0x040046F6 RID: 18166
	private int vendingIndex;

	// Token: 0x040046F7 RID: 18167
	private Coroutine vendingCoroutine;

	// Token: 0x040046F8 RID: 18168
	public int VendingMachineId;

	// Token: 0x040046F9 RID: 18169
	private GhostReactor reactor;

	// Token: 0x0200083E RID: 2110
	[Serializable]
	public struct VendingEntry
	{
		// Token: 0x06003641 RID: 13889 RVA: 0x0012BF18 File Offset: 0x0012A118
		public int GetEntityTypeId()
		{
			if (!this.entityTypeIdSet)
			{
				this.entityTypeId = this.entityPrefab.gameObject.name.GetStaticHash();
				this.entityTypeIdSet = true;
			}
			return this.entityTypeId;
		}

		// Token: 0x040046FA RID: 18170
		public Transform transportVisual;

		// Token: 0x040046FB RID: 18171
		public GameEntity entityPrefab;

		// Token: 0x040046FC RID: 18172
		public string itemName;

		// Token: 0x040046FD RID: 18173
		private int entityTypeId;

		// Token: 0x040046FE RID: 18174
		private bool entityTypeIdSet;
	}
}
