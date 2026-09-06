using System;
using UnityEngine;

// Token: 0x020001C8 RID: 456
public class GhostLab : MonoBehaviourTick, IBuildValidation
{
	// Token: 0x06000C18 RID: 3096 RVA: 0x00041DDA File Offset: 0x0003FFDA
	private void Awake()
	{
		this.relState = Object.FindFirstObjectByType<GhostLabReliableState>();
		this.doorState = GhostLab.EntranceDoorsState.BothClosed;
		this.doorOpen = new bool[this.relState.singleDoorCount];
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool BuildValidationCheck()
	{
		return true;
	}

	// Token: 0x06000C1A RID: 3098 RVA: 0x00041E04 File Offset: 0x00040004
	public void DoorButtonPress(int buttonIndex, bool forSingleDoor)
	{
		if (!forSingleDoor)
		{
			this.UpdateEntranceDoorsState(buttonIndex);
			return;
		}
		this.UpdateDoorState(buttonIndex);
		this.relState.UpdateSingleDoorState(buttonIndex);
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x00041E24 File Offset: 0x00040024
	public void UpdateDoorState(int buttonIndex)
	{
		if ((this.doorOpen[buttonIndex] && this.slidingDoor[buttonIndex].localPosition == this.singleDoorTravelDistance) || (!this.doorOpen[buttonIndex] && this.slidingDoor[buttonIndex].localPosition == Vector3.zero))
		{
			this.doorOpen[buttonIndex] = !this.doorOpen[buttonIndex];
		}
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x00041E8C File Offset: 0x0004008C
	public void UpdateEntranceDoorsState(int buttonIndex)
	{
		if (this.outerDoor == null || this.innerDoor == null)
		{
			return;
		}
		if (this.doorState == GhostLab.EntranceDoorsState.BothClosed)
		{
			if (!(this.innerDoor.localPosition != Vector3.zero) && !(this.outerDoor.localPosition != Vector3.zero))
			{
				if (buttonIndex == 0 || buttonIndex == 1)
				{
					this.doorState = GhostLab.EntranceDoorsState.OuterDoorOpen;
				}
				if (buttonIndex == 2 || buttonIndex == 3)
				{
					this.doorState = GhostLab.EntranceDoorsState.InnerDoorOpen;
				}
			}
		}
		else if (this.innerDoor.localPosition == this.doorTravelDistance || this.outerDoor.localPosition == this.doorTravelDistance)
		{
			this.doorState = GhostLab.EntranceDoorsState.BothClosed;
		}
		this.relState.UpdateEntranceDoorsState(this.doorState);
	}

	// Token: 0x06000C1D RID: 3101 RVA: 0x00041F54 File Offset: 0x00040154
	public override void Tick()
	{
		this.SynchStates();
		if (this.innerDoor != null && this.outerDoor != null)
		{
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			switch (this.doorState)
			{
			case GhostLab.EntranceDoorsState.InnerDoorOpen:
				zero2 = this.doorTravelDistance;
				break;
			case GhostLab.EntranceDoorsState.OuterDoorOpen:
				zero = this.doorTravelDistance;
				break;
			}
			this.outerDoor.localPosition = Vector3.MoveTowards(this.outerDoor.localPosition, zero, this.doorMoveSpeed * Time.deltaTime);
			this.innerDoor.localPosition = Vector3.MoveTowards(this.innerDoor.localPosition, zero2, this.doorMoveSpeed * Time.deltaTime);
		}
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < this.slidingDoor.Length; i++)
		{
			if (this.doorOpen[i])
			{
				vector = this.singleDoorTravelDistance;
			}
			else
			{
				vector = Vector3.zero;
			}
			this.slidingDoor[i].localPosition = Vector3.MoveTowards(this.slidingDoor[i].localPosition, vector, this.singleDoorMoveSpeed * Time.deltaTime);
		}
	}

	// Token: 0x06000C1E RID: 3102 RVA: 0x00042078 File Offset: 0x00040278
	private void SynchStates()
	{
		this.doorState = this.relState.doorState;
		for (int i = 0; i < this.doorOpen.Length; i++)
		{
			this.doorOpen[i] = this.relState.singleDoorOpen[i];
		}
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x000420C0 File Offset: 0x000402C0
	public bool IsDoorMoving(bool singleDoor, int index)
	{
		if (singleDoor)
		{
			return (this.doorOpen[index] && this.slidingDoor[index].localPosition != this.singleDoorTravelDistance) || (!this.doorOpen[index] && this.slidingDoor[index].localPosition != Vector3.zero);
		}
		if (index == 0 || index == 1)
		{
			return (this.doorState == GhostLab.EntranceDoorsState.OuterDoorOpen && this.outerDoor.localPosition != this.doorTravelDistance) || (this.doorState != GhostLab.EntranceDoorsState.OuterDoorOpen && this.outerDoor.localPosition != Vector3.zero);
		}
		return (this.doorState == GhostLab.EntranceDoorsState.InnerDoorOpen && this.innerDoor.localPosition != this.doorTravelDistance) || (this.doorState != GhostLab.EntranceDoorsState.InnerDoorOpen && this.innerDoor.localPosition != Vector3.zero);
	}

	// Token: 0x04000EB8 RID: 3768
	public Transform outerDoor;

	// Token: 0x04000EB9 RID: 3769
	public Transform innerDoor;

	// Token: 0x04000EBA RID: 3770
	public Vector3 doorTravelDistance;

	// Token: 0x04000EBB RID: 3771
	public float doorMoveSpeed;

	// Token: 0x04000EBC RID: 3772
	public float singleDoorMoveSpeed;

	// Token: 0x04000EBD RID: 3773
	public GhostLab.EntranceDoorsState doorState;

	// Token: 0x04000EBE RID: 3774
	public GhostLabReliableState relState;

	// Token: 0x04000EBF RID: 3775
	public Transform[] slidingDoor;

	// Token: 0x04000EC0 RID: 3776
	public Vector3 singleDoorTravelDistance;

	// Token: 0x04000EC1 RID: 3777
	private bool[] doorOpen;

	// Token: 0x020001C9 RID: 457
	public enum EntranceDoorsState
	{
		// Token: 0x04000EC3 RID: 3779
		BothClosed,
		// Token: 0x04000EC4 RID: 3780
		InnerDoorOpen,
		// Token: 0x04000EC5 RID: 3781
		OuterDoorOpen
	}
}
