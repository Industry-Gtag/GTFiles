using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000CC RID: 204
public class ShadeRevealer : TransferrableObject
{
	// Token: 0x060004EE RID: 1262 RVA: 0x0001B7F0 File Offset: 0x000199F0
	protected override void Awake()
	{
		base.Awake();
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		for (int i = 0; i < this.enableWhenScanning.Length; i++)
		{
			hashSet.Add(this.enableWhenScanning[i]);
		}
		for (int j = 0; j < this.enableWhenTracking.Length; j++)
		{
			hashSet.Add(this.enableWhenTracking[j]);
		}
		for (int k = 0; k < this.enableWhenLocked.Length; k++)
		{
			hashSet.Add(this.enableWhenLocked[k]);
		}
		for (int l = 0; l < this.enableWhenPrimed.Length; l++)
		{
			hashSet.Add(this.enableWhenPrimed[l]);
		}
		this.objectsToDisableWhenOff = new GameObject[hashSet.Count];
		hashSet.CopyTo(this.objectsToDisableWhenOff);
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x0001B8B4 File Offset: 0x00019AB4
	private float GetDistanceToBeamRay(Vector3 toPosition)
	{
		return Vector3.Cross(this.beamForward.forward, toPosition).magnitude;
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x0001B8DC File Offset: 0x00019ADC
	public ShadeRevealer.State GetBeamStateForPosition(Vector3 toPosition, float tolerance)
	{
		if (toPosition.magnitude <= this.beamLength + tolerance && Vector3.Dot(toPosition.normalized, this.beamForward.forward) > 0f)
		{
			float num = this.GetDistanceToBeamRay(toPosition) - tolerance;
			if (num <= this.lockThreshold)
			{
				return ShadeRevealer.State.LOCKED;
			}
			if (num <= this.trackThreshold)
			{
				return ShadeRevealer.State.TRACKING;
			}
		}
		return ShadeRevealer.State.SCANNING;
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x0001B939 File Offset: 0x00019B39
	public ShadeRevealer.State GetBeamStateForCritter(CosmeticCritter critter, float tolerance)
	{
		return this.GetBeamStateForPosition(critter.transform.position - this.beamForward.position, tolerance);
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x0001B95D File Offset: 0x00019B5D
	public bool CritterWithinBeamThreshold(CosmeticCritter critter, ShadeRevealer.State criteria, float tolerance)
	{
		return this.GetBeamStateForCritter(critter, tolerance) >= criteria;
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x0001B96D File Offset: 0x00019B6D
	public void SetBestBeamState(ShadeRevealer.State state)
	{
		if (state > this.pendingBeamState)
		{
			this.pendingBeamState = state;
		}
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x0001B980 File Offset: 0x00019B80
	private void SetObjectsEnabledFromState(ShadeRevealer.State state)
	{
		for (int i = 0; i < this.objectsToDisableWhenOff.Length; i++)
		{
			this.objectsToDisableWhenOff[i].SetActive(false);
		}
		GameObject[] array;
		switch (state)
		{
		case ShadeRevealer.State.SCANNING:
			array = this.enableWhenScanning;
			break;
		case ShadeRevealer.State.TRACKING:
			array = this.enableWhenTracking;
			break;
		case ShadeRevealer.State.LOCKED:
			array = this.enableWhenLocked;
			break;
		case ShadeRevealer.State.PRIMED:
			array = this.enableWhenPrimed;
			break;
		default:
			return;
		}
		for (int j = 0; j < array.Length; j++)
		{
			array[j].SetActive(true);
		}
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x0001BA00 File Offset: 0x00019C00
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.currentBeamState != this.pendingBeamState)
		{
			this.currentBeamState = this.pendingBeamState;
			this.SetObjectsEnabledFromState(this.currentBeamState);
		}
		this.beamSFX.pitch = 1f + this.shadeCatcher.GetActionTimeFrac() * 2f;
		if (this.isScanning)
		{
			this.pendingBeamState = ShadeRevealer.State.SCANNING;
		}
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x0001BA6A File Offset: 0x00019C6A
	public void StartScanning()
	{
		this.shadeCatcher.enabled = true;
		this.initialActivationSFX.GTPlay();
		this.beamSFX.GTPlay();
		this.isScanning = true;
		this.currentBeamState = ShadeRevealer.State.OFF;
		this.pendingBeamState = ShadeRevealer.State.SCANNING;
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x0001BAA4 File Offset: 0x00019CA4
	public void StopScanning()
	{
		if (this.currentBeamState == ShadeRevealer.State.PRIMED)
		{
			UnityEvent unityEvent = this.onShadeLaunched;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
		}
		this.shadeCatcher.enabled = false;
		this.initialActivationSFX.GTStop();
		this.beamSFX.GTStop();
		this.isScanning = false;
		this.currentBeamState = ShadeRevealer.State.OFF;
		this.pendingBeamState = ShadeRevealer.State.OFF;
		this.SetObjectsEnabledFromState(ShadeRevealer.State.OFF);
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x0001BB0C File Offset: 0x00019D0C
	public void ShadeCaught()
	{
		this.shadeCatcher.enabled = false;
		this.beamSFX.GTStop();
		this.catchSFX.GTPlay();
		this.catchFX.Play();
		this.isScanning = false;
		this.currentBeamState = ShadeRevealer.State.OFF;
		this.pendingBeamState = ShadeRevealer.State.PRIMED;
	}

	// Token: 0x0400058A RID: 1418
	[SerializeField]
	private AudioSource initialActivationSFX;

	// Token: 0x0400058B RID: 1419
	[SerializeField]
	private AudioSource beamSFX;

	// Token: 0x0400058C RID: 1420
	[SerializeField]
	private AudioSource catchSFX;

	// Token: 0x0400058D RID: 1421
	[SerializeField]
	private ParticleSystem catchFX;

	// Token: 0x0400058E RID: 1422
	[Space]
	[SerializeField]
	private CosmeticCritterCatcherShade shadeCatcher;

	// Token: 0x0400058F RID: 1423
	[Space]
	[Tooltip("The transform that represents the origin of the revealer beam.")]
	[SerializeField]
	private Transform beamForward;

	// Token: 0x04000590 RID: 1424
	[Tooltip("The maximum length of the beam.")]
	[SerializeField]
	private float beamLength;

	// Token: 0x04000591 RID: 1425
	[Tooltip("If the Shade is this close to the beam, set it to flee and have all Revealers enter Tracking mode.")]
	[SerializeField]
	private float trackThreshold;

	// Token: 0x04000592 RID: 1426
	[Tooltip("If the Shade is this close to the beam, slow it down.")]
	[SerializeField]
	private float lockThreshold;

	// Token: 0x04000593 RID: 1427
	[Tooltip("Editor-only object to help test the thresholds.")]
	[SerializeField]
	private Transform thresholdTester;

	// Token: 0x04000594 RID: 1428
	[Tooltip("Whether to draw the tester or not.")]
	[SerializeField]
	private bool drawThresholdTesterInEditor = true;

	// Token: 0x04000595 RID: 1429
	[Space]
	[Tooltip("Enable these objects while the beam is in Scanning mode.")]
	[SerializeField]
	private GameObject[] enableWhenScanning;

	// Token: 0x04000596 RID: 1430
	[Tooltip("Enable these objects while the beam is in Tracking mode.")]
	[SerializeField]
	private GameObject[] enableWhenTracking;

	// Token: 0x04000597 RID: 1431
	[Tooltip("Enable these objects while the beam is in Locked mode.")]
	[SerializeField]
	private GameObject[] enableWhenLocked;

	// Token: 0x04000598 RID: 1432
	[Tooltip("Enable these objects while ready to fire.")]
	[SerializeField]
	private GameObject[] enableWhenPrimed;

	// Token: 0x04000599 RID: 1433
	[Space]
	[SerializeField]
	private UnityEvent onShadeLaunched;

	// Token: 0x0400059A RID: 1434
	private bool isScanning;

	// Token: 0x0400059B RID: 1435
	private ShadeRevealer.State currentBeamState;

	// Token: 0x0400059C RID: 1436
	private ShadeRevealer.State pendingBeamState;

	// Token: 0x0400059D RID: 1437
	private GameObject[] objectsToDisableWhenOff;

	// Token: 0x020000CD RID: 205
	public enum State
	{
		// Token: 0x0400059F RID: 1439
		OFF,
		// Token: 0x040005A0 RID: 1440
		SCANNING,
		// Token: 0x040005A1 RID: 1441
		TRACKING,
		// Token: 0x040005A2 RID: 1442
		LOCKED,
		// Token: 0x040005A3 RID: 1443
		PRIMED
	}
}
