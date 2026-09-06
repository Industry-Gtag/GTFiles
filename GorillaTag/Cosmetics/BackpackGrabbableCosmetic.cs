using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012FD RID: 4861
	public class BackpackGrabbableCosmetic : HoldableObject
	{
		// Token: 0x060079F1 RID: 31217 RVA: 0x0027CD6F File Offset: 0x0027AF6F
		private void Awake()
		{
			this.currentItemsCount = this.startItemsCount;
			this.canGrab = true;
		}

		// Token: 0x060079F2 RID: 31218 RVA: 0x00002C2D File Offset: 0x00000E2D
		public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
		{
		}

		// Token: 0x060079F3 RID: 31219 RVA: 0x00002C2D File Offset: 0x00000E2D
		public override void DropItemCleanup()
		{
		}

		// Token: 0x060079F4 RID: 31220 RVA: 0x0027CD84 File Offset: 0x0027AF84
		public void Update()
		{
			if (!this.canGrab && Time.time - this.lastGrabTime >= this.coolDownTimer)
			{
				this.canGrab = true;
			}
		}

		// Token: 0x060079F5 RID: 31221 RVA: 0x0027CDAC File Offset: 0x0027AFAC
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (this.IsEmpty())
			{
				Debug.LogWarning("Can't remove item, Backpack is empty, need to refill.");
				return;
			}
			if (!this.canGrab)
			{
				return;
			}
			this.lastGrabTime = Time.time;
			this.canGrab = false;
			SnowballThrowable snowballThrowable;
			((grabbingHand == EquipmentInteractor.instance.leftHand) ? SnowballMaker.leftHandInstance : SnowballMaker.rightHandInstance).TryCreateSnowball(this.materialIndex, out snowballThrowable);
			this.RemoveItem();
		}

		// Token: 0x060079F6 RID: 31222 RVA: 0x0027CE1B File Offset: 0x0027B01B
		public void AddItem()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.maxCapacity <= this.currentItemsCount)
			{
				Debug.LogWarning("Can't add item, backpack is at full capacity.");
				return;
			}
			this.currentItemsCount++;
			this.UpdateState();
		}

		// Token: 0x060079F7 RID: 31223 RVA: 0x0027CE53 File Offset: 0x0027B053
		public void RemoveItem()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.currentItemsCount < 0)
			{
				Debug.LogWarning("Can't remove item, Backpack is empty.");
				return;
			}
			this.currentItemsCount--;
			this.UpdateState();
		}

		// Token: 0x060079F8 RID: 31224 RVA: 0x0027CE86 File Offset: 0x0027B086
		public void RefillBackpack()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.currentItemsCount == this.startItemsCount)
			{
				return;
			}
			this.currentItemsCount = this.startItemsCount;
			this.UpdateState();
		}

		// Token: 0x060079F9 RID: 31225 RVA: 0x0027CEB2 File Offset: 0x0027B0B2
		public void EmptyBackpack()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.currentItemsCount == 0)
			{
				return;
			}
			this.currentItemsCount = 0;
			this.UpdateState();
		}

		// Token: 0x060079FA RID: 31226 RVA: 0x0027CED3 File Offset: 0x0027B0D3
		public bool IsFull()
		{
			return !this.useCapacity || this.maxCapacity == this.currentItemsCount;
		}

		// Token: 0x060079FB RID: 31227 RVA: 0x0027CEEE File Offset: 0x0027B0EE
		public bool IsEmpty()
		{
			return this.useCapacity && this.currentItemsCount == 0;
		}

		// Token: 0x060079FC RID: 31228 RVA: 0x0027CF04 File Offset: 0x0027B104
		private void UpdateState()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.currentItemsCount == this.maxCapacity)
			{
				UnityEvent onReachedMaxCapacity = this.OnReachedMaxCapacity;
				if (onReachedMaxCapacity == null)
				{
					return;
				}
				onReachedMaxCapacity.Invoke();
				return;
			}
			else
			{
				if (this.currentItemsCount != 0)
				{
					if (this.currentItemsCount == this.startItemsCount)
					{
						UnityEvent onRefilled = this.OnRefilled;
						if (onRefilled == null)
						{
							return;
						}
						onRefilled.Invoke();
					}
					return;
				}
				UnityEvent onFullyEmptied = this.OnFullyEmptied;
				if (onFullyEmptied == null)
				{
					return;
				}
				onFullyEmptied.Invoke();
				return;
			}
		}

		// Token: 0x04008B5D RID: 35677
		[GorillaSoundLookup]
		public int materialIndex;

		// Token: 0x04008B5E RID: 35678
		[SerializeField]
		private bool useCapacity = true;

		// Token: 0x04008B5F RID: 35679
		[SerializeField]
		private float coolDownTimer = 2f;

		// Token: 0x04008B60 RID: 35680
		[SerializeField]
		private int maxCapacity;

		// Token: 0x04008B61 RID: 35681
		[SerializeField]
		private int startItemsCount;

		// Token: 0x04008B62 RID: 35682
		[Space]
		public UnityEvent OnReachedMaxCapacity;

		// Token: 0x04008B63 RID: 35683
		public UnityEvent OnFullyEmptied;

		// Token: 0x04008B64 RID: 35684
		public UnityEvent OnRefilled;

		// Token: 0x04008B65 RID: 35685
		private int currentItemsCount;

		// Token: 0x04008B66 RID: 35686
		private bool canGrab;

		// Token: 0x04008B67 RID: 35687
		private float lastGrabTime;
	}
}
