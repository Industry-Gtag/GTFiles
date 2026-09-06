using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000D99 RID: 3481
public class CompositeTriggerEvents : MonoBehaviour
{
	// Token: 0x17000834 RID: 2100
	// (get) Token: 0x060055A0 RID: 21920 RVA: 0x001C000A File Offset: 0x001BE20A
	private Dictionary<Collider, int> CollderMasks
	{
		get
		{
			return this.overlapMask;
		}
	}

	// Token: 0x1400009B RID: 155
	// (add) Token: 0x060055A1 RID: 21921 RVA: 0x001C0014 File Offset: 0x001BE214
	// (remove) Token: 0x060055A2 RID: 21922 RVA: 0x001C004C File Offset: 0x001BE24C
	public event CompositeTriggerEvents.TriggerEvent CompositeTriggerEnter;

	// Token: 0x1400009C RID: 156
	// (add) Token: 0x060055A3 RID: 21923 RVA: 0x001C0084 File Offset: 0x001BE284
	// (remove) Token: 0x060055A4 RID: 21924 RVA: 0x001C00BC File Offset: 0x001BE2BC
	public event CompositeTriggerEvents.TriggerEvent CompositeTriggerExit;

	// Token: 0x060055A5 RID: 21925 RVA: 0x001C00F4 File Offset: 0x001BE2F4
	private void Awake()
	{
		if (this.individualTriggerColliders.Count > 32)
		{
			Debug.LogError("The max number of triggers was exceeded in this composite trigger event sender on GameObject: " + base.gameObject.name + ".");
		}
		for (int i = 0; i < this.individualTriggerColliders.Count; i++)
		{
			TriggerEventNotifier triggerEventNotifier = this.individualTriggerColliders[i].gameObject.AddComponent<TriggerEventNotifier>();
			triggerEventNotifier.maskIndex = i;
			triggerEventNotifier.TriggerEnterEvent += this.TriggerEnterReceiver;
			triggerEventNotifier.TriggerExitEvent += this.TriggerExitReceiver;
			this.triggerEventNotifiers.Add(triggerEventNotifier);
		}
	}

	// Token: 0x060055A6 RID: 21926 RVA: 0x001C0194 File Offset: 0x001BE394
	public void AddCollider(Collider colliderToAdd)
	{
		if (this.individualTriggerColliders.Count >= 32)
		{
			Debug.LogError("The max number of triggers are already present in this composite trigger event sender on GameObject: " + base.gameObject.name + ".");
			return;
		}
		this.individualTriggerColliders.Add(colliderToAdd);
		TriggerEventNotifier triggerEventNotifier = colliderToAdd.gameObject.AddComponent<TriggerEventNotifier>();
		triggerEventNotifier.maskIndex = this.GetNextMaskIndex();
		triggerEventNotifier.TriggerEnterEvent += this.TriggerEnterReceiver;
		triggerEventNotifier.TriggerExitEvent += this.TriggerExitReceiver;
		this.triggerEventNotifiers.Add(triggerEventNotifier);
		this.triggerEventNotifiers.Sort((TriggerEventNotifier a, TriggerEventNotifier b) => a.maskIndex.CompareTo(b.maskIndex));
	}

	// Token: 0x060055A7 RID: 21927 RVA: 0x001C0250 File Offset: 0x001BE450
	public void RemoveCollider(Collider colliderToRemove)
	{
		TriggerEventNotifier component = colliderToRemove.gameObject.GetComponent<TriggerEventNotifier>();
		if (component.IsNotNull())
		{
			foreach (KeyValuePair<Collider, int> keyValuePair in new Dictionary<Collider, int>(this.overlapMask))
			{
				this.TriggerExitReceiver(component, keyValuePair.Key);
			}
			component.maskIndex = -1;
			component.TriggerEnterEvent -= this.TriggerEnterReceiver;
			component.TriggerExitEvent -= this.TriggerExitReceiver;
			this.triggerEventNotifiers.Remove(component);
		}
		this.individualTriggerColliders.Remove(colliderToRemove);
	}

	// Token: 0x060055A8 RID: 21928 RVA: 0x001C0308 File Offset: 0x001BE508
	public void ResetColliders(bool sendExitEvent = true)
	{
		this.individualTriggerColliders.Clear();
		for (int i = this.triggerEventNotifiers.Count - 1; i >= 0; i--)
		{
			if (this.triggerEventNotifiers[i].IsNull())
			{
				this.triggerEventNotifiers.RemoveAt(i);
			}
			else
			{
				this.triggerEventNotifiers[i].maskIndex = -1;
				this.triggerEventNotifiers[i].TriggerEnterEvent -= this.TriggerEnterReceiver;
				this.triggerEventNotifiers[i].TriggerExitEvent -= this.TriggerExitReceiver;
				this.triggerEventNotifiers.RemoveAt(i);
			}
		}
		if (sendExitEvent)
		{
			foreach (KeyValuePair<Collider, int> keyValuePair in this.overlapMask)
			{
				CompositeTriggerEvents.TriggerEvent compositeTriggerExit = this.CompositeTriggerExit;
				if (compositeTriggerExit != null)
				{
					compositeTriggerExit(keyValuePair.Key);
				}
			}
		}
		this.overlapMask.Clear();
	}

	// Token: 0x060055A9 RID: 21929 RVA: 0x001C041C File Offset: 0x001BE61C
	public int GetNumColliders()
	{
		return this.individualTriggerColliders.Count;
	}

	// Token: 0x060055AA RID: 21930 RVA: 0x001C042C File Offset: 0x001BE62C
	public int GetNextMaskIndex()
	{
		if (this.individualTriggerColliders.Count >= 32)
		{
			Debug.LogError("The max number of triggers are already present in this composite trigger event sender on GameObject: " + base.gameObject.name + ".");
			return -1;
		}
		int num = 0;
		int num2 = 0;
		while (num2 < this.triggerEventNotifiers.Count && this.triggerEventNotifiers[num2].maskIndex == num)
		{
			num++;
			num2++;
		}
		return num;
	}

	// Token: 0x060055AB RID: 21931 RVA: 0x001C049C File Offset: 0x001BE69C
	private void OnDestroy()
	{
		for (int i = 0; i < this.triggerEventNotifiers.Count; i++)
		{
			if (this.triggerEventNotifiers[i] != null)
			{
				this.triggerEventNotifiers[i].TriggerEnterEvent -= this.TriggerEnterReceiver;
				this.triggerEventNotifiers[i].TriggerExitEvent -= this.TriggerExitReceiver;
			}
		}
	}

	// Token: 0x060055AC RID: 21932 RVA: 0x001C0510 File Offset: 0x001BE710
	public void TriggerEnterReceiver(TriggerEventNotifier notifier, Collider other)
	{
		int num;
		if (this.overlapMask.TryGetValue(other, out num))
		{
			num = this.SetMaskIndexTrue(num, notifier.maskIndex);
			this.overlapMask[other] = num;
			return;
		}
		int num2 = this.SetMaskIndexTrue(0, notifier.maskIndex);
		this.overlapMask.Add(other, num2);
		CompositeTriggerEvents.TriggerEvent compositeTriggerEnter = this.CompositeTriggerEnter;
		if (compositeTriggerEnter == null)
		{
			return;
		}
		compositeTriggerEnter(other);
	}

	// Token: 0x060055AD RID: 21933 RVA: 0x001C0578 File Offset: 0x001BE778
	public void TriggerExitReceiver(TriggerEventNotifier notifier, Collider other)
	{
		int num;
		if (this.overlapMask.TryGetValue(other, out num))
		{
			num = this.SetMaskIndexFalse(num, notifier.maskIndex);
			if (num == 0)
			{
				this.overlapMask.Remove(other);
				CompositeTriggerEvents.TriggerEvent compositeTriggerExit = this.CompositeTriggerExit;
				if (compositeTriggerExit == null)
				{
					return;
				}
				compositeTriggerExit(other);
				return;
			}
			else
			{
				this.overlapMask[other] = num;
			}
		}
	}

	// Token: 0x060055AE RID: 21934 RVA: 0x001C05D4 File Offset: 0x001BE7D4
	public void ResetColliderMask(Collider other)
	{
		int num;
		if (this.overlapMask.TryGetValue(other, out num))
		{
			if (num != 0)
			{
				CompositeTriggerEvents.TriggerEvent compositeTriggerExit = this.CompositeTriggerExit;
				if (compositeTriggerExit != null)
				{
					compositeTriggerExit(other);
				}
			}
			this.overlapMask.Remove(other);
		}
	}

	// Token: 0x060055AF RID: 21935 RVA: 0x001C0613 File Offset: 0x001BE813
	public void CompositeTriggerEnterReceiver(Collider other)
	{
		CompositeTriggerEvents.TriggerEvent compositeTriggerEnter = this.CompositeTriggerEnter;
		if (compositeTriggerEnter == null)
		{
			return;
		}
		compositeTriggerEnter(other);
	}

	// Token: 0x060055B0 RID: 21936 RVA: 0x001C0626 File Offset: 0x001BE826
	public void CompositeTriggerExitReceiver(Collider other)
	{
		CompositeTriggerEvents.TriggerEvent compositeTriggerExit = this.CompositeTriggerExit;
		if (compositeTriggerExit == null)
		{
			return;
		}
		compositeTriggerExit(other);
	}

	// Token: 0x060055B1 RID: 21937 RVA: 0x001C0639 File Offset: 0x001BE839
	private bool TestMaskIndex(int mask, int index)
	{
		return (mask & (1 << index)) != 0;
	}

	// Token: 0x060055B2 RID: 21938 RVA: 0x001C0646 File Offset: 0x001BE846
	private int SetMaskIndexTrue(int mask, int index)
	{
		return mask | (1 << index);
	}

	// Token: 0x060055B3 RID: 21939 RVA: 0x001C0650 File Offset: 0x001BE850
	private int SetMaskIndexFalse(int mask, int index)
	{
		return mask & ~(1 << index);
	}

	// Token: 0x060055B4 RID: 21940 RVA: 0x001C065C File Offset: 0x001BE85C
	private string MaskToString(int mask)
	{
		string text = "";
		for (int i = 31; i >= 0; i--)
		{
			text += (this.TestMaskIndex(mask, i) ? "1" : "0");
		}
		return text;
	}

	// Token: 0x04006702 RID: 26370
	[SerializeField]
	private List<Collider> individualTriggerColliders = new List<Collider>();

	// Token: 0x04006703 RID: 26371
	private List<TriggerEventNotifier> triggerEventNotifiers = new List<TriggerEventNotifier>();

	// Token: 0x04006704 RID: 26372
	private Dictionary<Collider, int> overlapMask = new Dictionary<Collider, int>();

	// Token: 0x02000D9A RID: 3482
	// (Invoke) Token: 0x060055B7 RID: 21943
	public delegate void TriggerEvent(Collider collider);
}
