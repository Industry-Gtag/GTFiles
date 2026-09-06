using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004E0 RID: 1248
public class RigEventGate : MonoBehaviour, IBuildValidation
{
	// Token: 0x06001E66 RID: 7782 RVA: 0x000A29E8 File Offset: 0x000A0BE8
	private void OnEnable()
	{
		if (this.rigCollection == null)
		{
			return;
		}
		VRRigCollection vrrigCollection = this.rigCollection;
		vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.OnJoined));
		VRRigCollection vrrigCollection2 = this.rigCollection;
		vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.OnLeft));
	}

	// Token: 0x06001E67 RID: 7783 RVA: 0x000A2A54 File Offset: 0x000A0C54
	private void OnDisable()
	{
		if (this.rigCollection == null)
		{
			return;
		}
		VRRigCollection vrrigCollection = this.rigCollection;
		vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Remove(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.OnJoined));
		VRRigCollection vrrigCollection2 = this.rigCollection;
		vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Remove(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.OnLeft));
	}

	// Token: 0x06001E68 RID: 7784 RVA: 0x000A2AC0 File Offset: 0x000A0CC0
	private void OnDestroy()
	{
		if (this.rigCollection == null)
		{
			return;
		}
		VRRigCollection vrrigCollection = this.rigCollection;
		vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Remove(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.OnJoined));
		VRRigCollection vrrigCollection2 = this.rigCollection;
		vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Remove(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.OnLeft));
	}

	// Token: 0x06001E69 RID: 7785 RVA: 0x000A2B2C File Offset: 0x000A0D2C
	private void OnJoined(RigContainer rc)
	{
		int num = ((this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count);
		this.countChanged(this.gameObjects.Count, this.gameObjects.Count, num - 1, num, null);
	}

	// Token: 0x06001E6A RID: 7786 RVA: 0x000A2B7C File Offset: 0x000A0D7C
	private void OnLeft(RigContainer rc)
	{
		RigEventVolumeTrigger rigEventVolumeTrigger = null;
		for (int i = 0; i < this.gameObjects.Count; i++)
		{
			if (this.gameObjects[i].Rig == rc.Rig)
			{
				rigEventVolumeTrigger = this.gameObjects[i];
			}
		}
		int num = ((this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count);
		if (rigEventVolumeTrigger != null)
		{
			this.gameObjects.Remove(rigEventVolumeTrigger);
			this.countChanged(this.gameObjects.Count + 1, this.gameObjects.Count, num + 1, num, null);
			return;
		}
		this.countChanged(this.gameObjects.Count, this.gameObjects.Count, num + 1, num, null);
	}

	// Token: 0x06001E6B RID: 7787 RVA: 0x000A2C4C File Offset: 0x000A0E4C
	private void OnTriggerEnter(Collider other)
	{
		RigEventVolumeTrigger rigEventVolumeTrigger;
		if (!other.gameObject.TryGetComponent<RigEventVolumeTrigger>(out rigEventVolumeTrigger) || base.transform.InverseTransformPoint(rigEventVolumeTrigger.transform.position).z < 0f)
		{
			return;
		}
		if (this.gameObjects.Contains(rigEventVolumeTrigger))
		{
			int num = ((this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count);
			int count = this.gameObjects.Count;
			this.gameObjects.Remove(rigEventVolumeTrigger);
			this.countChanged(count, this.gameObjects.Count, num, num, rigEventVolumeTrigger);
		}
	}

	// Token: 0x06001E6C RID: 7788 RVA: 0x000A2CEC File Offset: 0x000A0EEC
	private void OnTriggerExit(Collider other)
	{
		RigEventVolumeTrigger rigEventVolumeTrigger;
		if (!other.gameObject.TryGetComponent<RigEventVolumeTrigger>(out rigEventVolumeTrigger) || base.transform.InverseTransformPoint(rigEventVolumeTrigger.transform.position).z < 0f)
		{
			return;
		}
		if (!this.gameObjects.Contains(rigEventVolumeTrigger))
		{
			int num = ((this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count);
			int count = this.gameObjects.Count;
			this.gameObjects.Add(rigEventVolumeTrigger);
			this.countChanged(count, this.gameObjects.Count, num, num, rigEventVolumeTrigger);
		}
	}

	// Token: 0x06001E6D RID: 7789 RVA: 0x000A2D88 File Offset: 0x000A0F88
	private void countChanged(int oldValue, int newValue, int oldPlayerCount, int newPlayerCount, RigEventVolumeTrigger rig)
	{
		if (newValue > oldValue)
		{
			if (rig != null)
			{
				UnityEvent<VRRig> rigExits = this.RigExits;
				if (rigExits != null)
				{
					rigExits.Invoke(rig.Rig);
				}
			}
			if ((this.mode == RigEventGate.Mode.RELATIVE && (float)newValue / (float)newPlayerCount >= this.relThreshold && (float)oldValue / (float)oldPlayerCount < this.relThreshold) || (this.mode == RigEventGate.Mode.ABSOLUTE && newValue >= this.absThreshold && oldValue < this.absThreshold))
			{
				UnityEvent goesOverThreshold = this.GoesOverThreshold;
				if (goesOverThreshold == null)
				{
					return;
				}
				goesOverThreshold.Invoke();
			}
		}
	}

	// Token: 0x06001E6E RID: 7790 RVA: 0x000A2E09 File Offset: 0x000A1009
	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.mode == RigEventGate.Mode.RELATIVE && this.rigCollection == null)
		{
			Debug.Log("RigEventGate on " + base.name + " is set to RELATIVE mode but has no Player Count Source. This will crash!");
			return false;
		}
		return true;
	}

	// Token: 0x04002891 RID: 10385
	private List<RigEventVolumeTrigger> gameObjects = new List<RigEventVolumeTrigger>();

	// Token: 0x04002892 RID: 10386
	[SerializeField]
	private RigEventGate.Mode mode = RigEventGate.Mode.ABSOLUTE;

	// Token: 0x04002893 RID: 10387
	[Range(0.05f, 1f)]
	[SerializeField]
	private float relThreshold = 0.05f;

	// Token: 0x04002894 RID: 10388
	[SerializeField]
	private VRRigCollection rigCollection;

	// Token: 0x04002895 RID: 10389
	[Range(1f, 20f)]
	[SerializeField]
	private int absThreshold = 1;

	// Token: 0x04002896 RID: 10390
	[SerializeField]
	private UnityEvent<VRRig> RigExits;

	// Token: 0x04002897 RID: 10391
	[SerializeField]
	private UnityEvent GoesOverThreshold;

	// Token: 0x020004E1 RID: 1249
	private enum Mode
	{
		// Token: 0x04002899 RID: 10393
		RELATIVE,
		// Token: 0x0400289A RID: 10394
		ABSOLUTE
	}
}
