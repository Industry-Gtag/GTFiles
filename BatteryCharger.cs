using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000194 RID: 404
public class BatteryCharger : MonoBehaviour
{
	// Token: 0x170000FF RID: 255
	// (get) Token: 0x06000ADC RID: 2780 RVA: 0x0003A533 File Offset: 0x00038733
	public int CurrentEventPhase
	{
		get
		{
			if (!(this.state != null))
			{
				return -1;
			}
			return this.state.EventPhase;
		}
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x0003A550 File Offset: 0x00038750
	internal int RegisterCrank(BatteryChargerCrank crank)
	{
		if (this.crankCount >= 20)
		{
			Debug.LogError(string.Format("BatteryCharger: too many cranks (max {0})", 20), this);
			return -1;
		}
		int num = this.crankCount;
		this.cranks[num] = crank;
		this.crankCount++;
		return num;
	}

	// Token: 0x17000100 RID: 256
	// (get) Token: 0x06000ADE RID: 2782 RVA: 0x000391DD File Offset: 0x000373DD
	private int LocalActorNr
	{
		get
		{
			if (PhotonNetwork.LocalPlayer == null)
			{
				return -1;
			}
			return PhotonNetwork.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x0003A5A0 File Offset: 0x000387A0
	private void OnEnable()
	{
		BatteryChargerState batteryChargerState;
		if (this.stateRef.TryResolve<BatteryChargerState>(out batteryChargerState))
		{
			this.Bind(batteryChargerState);
			return;
		}
		this.stateRef.AddCallbackOnLoad(new Action(this.OnStateSceneLoaded));
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x0003A5DB File Offset: 0x000387DB
	private void OnDisable()
	{
		this.stateRef.RemoveCallbackOnLoad(new Action(this.OnStateSceneLoaded));
		this.Unbind();
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x0003A5FC File Offset: 0x000387FC
	private void OnStateSceneLoaded()
	{
		BatteryChargerState batteryChargerState;
		if (this.stateRef.TryResolve<BatteryChargerState>(out batteryChargerState))
		{
			this.Bind(batteryChargerState);
		}
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x0003A620 File Offset: 0x00038820
	private void Bind(BatteryChargerState newState)
	{
		if (this.state == newState)
		{
			return;
		}
		this.Unbind();
		this.state = newState;
		if (this.state == null)
		{
			return;
		}
		this.state.onChargeChanged += this.OnChargeChanged;
		this.state.onFullyCharged += this.OnFullyCharged;
		this.state.onEventPhaseChanged += this.OnEventPhaseChanged;
		this.previousCharge = this.state.CurrentCharge;
		this.ApplyChargeVisuals();
		this.OnEventPhaseChanged(this.state.EventPhase);
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x0003A6C8 File Offset: 0x000388C8
	private void Unbind()
	{
		if (this.state == null)
		{
			return;
		}
		this.state.onChargeChanged -= this.OnChargeChanged;
		this.state.onFullyCharged -= this.OnFullyCharged;
		this.state.onEventPhaseChanged -= this.OnEventPhaseChanged;
		this.state = null;
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x0003A730 File Offset: 0x00038930
	private void LateUpdate()
	{
		if (this.state == null)
		{
			return;
		}
		int localActorNr = this.LocalActorNr;
		for (int i = 0; i < this.crankCount; i++)
		{
			if (!(this.cranks[i] == null))
			{
				if (this.state.crankSyncs[i].holderActorNr == localActorNr)
				{
					this.state.UpdateLocalCrankState(i, this.cranks[i].IsHeldLeftHand, this.cranks[i].CurrentAngle);
				}
				this.UpdateRemoteCrankVisual(this.cranks[i], this.state.crankSyncs[i], localActorNr);
			}
		}
		if (this.chargingLoopSound != null)
		{
			bool flag = false;
			for (int j = 0; j < 20; j++)
			{
				if (this.state.crankSyncs[j].holderActorNr != -1)
				{
					flag = true;
					break;
				}
			}
			if (flag && !this.chargingLoopSound.isPlaying)
			{
				this.chargingLoopSound.Play();
				return;
			}
			if (!flag && this.chargingLoopSound.isPlaying)
			{
				this.chargingLoopSound.Stop();
			}
		}
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x0003A844 File Offset: 0x00038A44
	private void UpdateRemoteCrankVisual(BatteryChargerCrank crank, BatteryChargerState.CrankSyncState syncState, int localActor)
	{
		if (crank == null || syncState.holderActorNr == localActor)
		{
			return;
		}
		if (syncState.holderActorNr != -1)
		{
			VRRig vrrig = BatteryChargerState.FindRigForActor(syncState.holderActorNr);
			if (vrrig != null)
			{
				crank.UpdateFromRemoteHand(vrrig, syncState.isLeftHand);
				return;
			}
		}
		crank.SetVisualAngle(syncState.angle);
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x0003A89C File Offset: 0x00038A9C
	internal bool IsCrankHeldLocally(int crankIndex)
	{
		return !(this.state == null) && crankIndex >= 0 && crankIndex < 20 && this.state.crankSyncs[crankIndex].holderActorNr == this.LocalActorNr;
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x0003A8D5 File Offset: 0x00038AD5
	public void SetEventPhase(int phase)
	{
		if (this.state == null)
		{
			Debug.LogWarning("BatteryChargeState has not been binded yet!");
			return;
		}
		this.state.SetEventPhase(phase);
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x0003A8FC File Offset: 0x00038AFC
	public void SetChargePerCrankDegree(float chargeRate)
	{
		this.state.SetChargePerCrankDegree(chargeRate);
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x0003A90A File Offset: 0x00038B0A
	internal bool OnCrankGrabbed(int crankIndex, bool isLeftHand)
	{
		return this.state.NotifyCrankGrabbed(crankIndex, isLeftHand);
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x0003A919 File Offset: 0x00038B19
	internal void OnCrankReleased(int crankIndex, float finalAngle)
	{
		this.state.NotifyCrankReleased(crankIndex, finalAngle);
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x0003A928 File Offset: 0x00038B28
	internal void OnCrankInput(int crankIndex, float degrees)
	{
		this.state.NotifyCrankInput(crankIndex, degrees);
		this.ApplyChargeVisuals();
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x0003A940 File Offset: 0x00038B40
	private void OnChargeChanged()
	{
		for (int i = 0; i < this.actions.Length; i++)
		{
			if ((this.actions[i].Direction == BatteryCharger.BatteryChargerEvent.VDirection.Up && this.previousCharge < this.state.CurrentCharge && this.previousCharge < this.actions[i].Value && this.state.CurrentCharge >= this.actions[i].Value) || (this.actions[i].Direction == BatteryCharger.BatteryChargerEvent.VDirection.Down && this.previousCharge > this.state.CurrentCharge && this.previousCharge > this.actions[i].Value && this.state.CurrentCharge <= this.actions[i].Value))
			{
				UnityEvent action = this.actions[i].Action;
				if (action != null)
				{
					action.Invoke();
				}
			}
		}
		this.previousCharge = this.state.CurrentCharge;
		this.ApplyChargeVisuals();
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x0003AA38 File Offset: 0x00038C38
	private void OnFullyCharged()
	{
		if (this.fullyChargedSound != null)
		{
			this.fullyChargedSound.GTPlay();
		}
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x0003AA54 File Offset: 0x00038C54
	private void OnEventPhaseChanged(int phase)
	{
		for (int i = 0; i < this.eventPhases.Length; i++)
		{
			BatteryCharger.EventPhaseObjects eventPhaseObjects = this.eventPhases[i];
			if (((eventPhaseObjects != null) ? eventPhaseObjects.objects : null) != null)
			{
				bool flag = i == phase;
				for (int j = 0; j < this.eventPhases[i].objects.Length; j++)
				{
					if (this.eventPhases[i].objects[j] != null)
					{
						this.eventPhases[i].objects[j].SetActive(flag);
					}
				}
			}
		}
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x0003AAD8 File Offset: 0x00038CD8
	private void ApplyChargeVisuals()
	{
		if (this.state == null)
		{
			return;
		}
		float chargePercent = this.state.ChargePercent;
		if (this.chargeFillTransform != null)
		{
			this.chargeFillTransform.localRotation = Quaternion.Euler(0f, 0f, chargePercent * this.chargeFullRollAngle);
		}
		if (this.chargeFillRenderer != null)
		{
			this.chargeFillRenderer.material.color = Color.Lerp(this.emptyColor, this.fullColor, chargePercent);
		}
	}

	// Token: 0x04000D22 RID: 3362
	[Header("Network State")]
	[SerializeField]
	private XSceneRef stateRef;

	// Token: 0x04000D23 RID: 3363
	[Header("Charge Visuals")]
	[Tooltip("Transform rotated on its local Z axis to show charge level")]
	[SerializeField]
	private Transform chargeFillTransform;

	// Token: 0x04000D24 RID: 3364
	[Tooltip("Local Z rotation in degrees when fully charged")]
	[SerializeField]
	private float chargeFullRollAngle = -180f;

	// Token: 0x04000D25 RID: 3365
	[Tooltip("Renderer whose material color lerps with charge")]
	[SerializeField]
	private Renderer chargeFillRenderer;

	// Token: 0x04000D26 RID: 3366
	[SerializeField]
	private Color emptyColor = Color.red;

	// Token: 0x04000D27 RID: 3367
	[SerializeField]
	private Color fullColor = Color.green;

	// Token: 0x04000D28 RID: 3368
	[Header("Audio")]
	[SerializeField]
	private AudioSource chargingLoopSound;

	// Token: 0x04000D29 RID: 3369
	[SerializeField]
	private AudioSource fullyChargedSound;

	// Token: 0x04000D2A RID: 3370
	[Header("Event Phases")]
	[SerializeField]
	private BatteryCharger.EventPhaseObjects[] eventPhases;

	// Token: 0x04000D2B RID: 3371
	private BatteryChargerState state;

	// Token: 0x04000D2C RID: 3372
	private BatteryChargerCrank[] cranks = new BatteryChargerCrank[20];

	// Token: 0x04000D2D RID: 3373
	private int crankCount;

	// Token: 0x04000D2E RID: 3374
	[SerializeField]
	private BatteryCharger.BatteryChargerEvent[] actions;

	// Token: 0x04000D2F RID: 3375
	private float previousCharge;

	// Token: 0x02000195 RID: 405
	[Serializable]
	private class EventPhaseObjects
	{
		// Token: 0x04000D30 RID: 3376
		public string friendlyName;

		// Token: 0x04000D31 RID: 3377
		public GameObject[] objects;
	}

	// Token: 0x02000196 RID: 406
	[Serializable]
	private class BatteryChargerEvent
	{
		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000AF2 RID: 2802 RVA: 0x0003AB96 File Offset: 0x00038D96
		public BatteryCharger.BatteryChargerEvent.VDirection Direction
		{
			get
			{
				return this.direction;
			}
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000AF3 RID: 2803 RVA: 0x0003AB9E File Offset: 0x00038D9E
		public float Value
		{
			get
			{
				return this.value;
			}
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000AF4 RID: 2804 RVA: 0x0003ABA6 File Offset: 0x00038DA6
		public UnityEvent Action
		{
			get
			{
				return this.action;
			}
		}

		// Token: 0x04000D32 RID: 3378
		[SerializeField]
		private BatteryCharger.BatteryChargerEvent.VDirection direction;

		// Token: 0x04000D33 RID: 3379
		[SerializeField]
		private float value;

		// Token: 0x04000D34 RID: 3380
		[SerializeField]
		private UnityEvent action;

		// Token: 0x02000197 RID: 407
		public enum VDirection
		{
			// Token: 0x04000D36 RID: 3382
			Up,
			// Token: 0x04000D37 RID: 3383
			Down
		}
	}
}
