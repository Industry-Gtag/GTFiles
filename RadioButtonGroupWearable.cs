using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002F7 RID: 759
public class RadioButtonGroupWearable : MonoBehaviour, ISpawnable
{
	// Token: 0x170001E9 RID: 489
	// (get) Token: 0x06001354 RID: 4948 RVA: 0x000666DA File Offset: 0x000648DA
	// (set) Token: 0x06001355 RID: 4949 RVA: 0x000666E2 File Offset: 0x000648E2
	public bool IsSpawned { get; set; }

	// Token: 0x170001EA RID: 490
	// (get) Token: 0x06001356 RID: 4950 RVA: 0x000666EB File Offset: 0x000648EB
	// (set) Token: 0x06001357 RID: 4951 RVA: 0x000666F3 File Offset: 0x000648F3
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06001358 RID: 4952 RVA: 0x000666FC File Offset: 0x000648FC
	private void Start()
	{
		this.stateBitsWriteInfo = VRRig.WearablePackedStatesBitWriteInfos[(int)this.assignedSlot];
		if (!this.ownerRig.isLocal)
		{
			GorillaPressableButton[] array = this.buttons;
			for (int i = 0; i < array.Length; i++)
			{
				Collider component = array[i].GetComponent<Collider>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
		}
	}

	// Token: 0x06001359 RID: 4953 RVA: 0x0006675A File Offset: 0x0006495A
	private void OnEnable()
	{
		this.SharedRefreshState();
	}

	// Token: 0x0600135A RID: 4954 RVA: 0x00066762 File Offset: 0x00064962
	private int GetCurrentState()
	{
		return GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
	}

	// Token: 0x0600135B RID: 4955 RVA: 0x0006678A File Offset: 0x0006498A
	private void Update()
	{
		if (this.ownerRig.isLocal)
		{
			return;
		}
		if (this.lastReportedState != this.GetCurrentState())
		{
			this.SharedRefreshState();
		}
	}

	// Token: 0x0600135C RID: 4956 RVA: 0x000667B0 File Offset: 0x000649B0
	public void SharedRefreshState()
	{
		int currentState = this.GetCurrentState();
		int num = (this.AllowSelectNone ? (currentState - 1) : currentState);
		for (int i = 0; i < this.buttons.Length; i++)
		{
			this.buttons[i].isOn = num == i;
			this.buttons[i].UpdateColor();
		}
		if (this.lastReportedState != currentState)
		{
			this.lastReportedState = currentState;
			this.OnSelectionChanged.Invoke(currentState);
		}
	}

	// Token: 0x0600135D RID: 4957 RVA: 0x00066820 File Offset: 0x00064A20
	public void OnPress(GorillaPressableButton button)
	{
		int currentState = this.GetCurrentState();
		int num = Array.IndexOf<GorillaPressableButton>(this.buttons, button);
		if (this.AllowSelectNone)
		{
			num++;
		}
		int num2 = num;
		if (this.AllowSelectNone && num == currentState)
		{
			num2 = 0;
		}
		this.ownerRig.WearablePackedStates = GTBitOps.WriteBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo, num2);
		this.SharedRefreshState();
	}

	// Token: 0x0600135E RID: 4958 RVA: 0x00066885 File Offset: 0x00064A85
	public void OnSpawn(VRRig rig)
	{
		this.ownerRig = rig;
	}

	// Token: 0x0600135F RID: 4959 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnDespawn()
	{
	}

	// Token: 0x0400179A RID: 6042
	[SerializeField]
	private bool AllowSelectNone = true;

	// Token: 0x0400179B RID: 6043
	[SerializeField]
	private GorillaPressableButton[] buttons;

	// Token: 0x0400179C RID: 6044
	[SerializeField]
	private UnityEvent<int> OnSelectionChanged;

	// Token: 0x0400179D RID: 6045
	[Tooltip("This is to determine what bit to change in VRRig.WearablesPackedStates.")]
	[SerializeField]
	private VRRig.WearablePackedStateSlots assignedSlot = VRRig.WearablePackedStateSlots.Pants1;

	// Token: 0x0400179E RID: 6046
	private int lastReportedState;

	// Token: 0x0400179F RID: 6047
	private VRRig ownerRig;

	// Token: 0x040017A0 RID: 6048
	private GTBitOps.BitWriteInfo stateBitsWriteInfo;
}
