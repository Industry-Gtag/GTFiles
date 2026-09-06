using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004E2 RID: 1250
public class RigEventVolume : MonoBehaviour
{
	// Token: 0x17000330 RID: 816
	// (get) Token: 0x06001E70 RID: 7792 RVA: 0x000A2E6A File Offset: 0x000A106A
	public VRRig[] Rigs
	{
		get
		{
			return this.rigs.ToArray();
		}
	}

	// Token: 0x17000331 RID: 817
	// (get) Token: 0x06001E71 RID: 7793 RVA: 0x000A2E77 File Offset: 0x000A1077
	public int RigCount
	{
		get
		{
			return this.gameObjects.Keys.Count;
		}
	}

	// Token: 0x17000332 RID: 818
	// (get) Token: 0x06001E72 RID: 7794 RVA: 0x000A2E89 File Offset: 0x000A1089
	public bool LocalRigPresent
	{
		get
		{
			return this.localRigPresent;
		}
	}

	// Token: 0x14000042 RID: 66
	// (add) Token: 0x06001E73 RID: 7795 RVA: 0x000A2E94 File Offset: 0x000A1094
	// (remove) Token: 0x06001E74 RID: 7796 RVA: 0x000A2ECC File Offset: 0x000A10CC
	public event Action OnCountChanged;

	// Token: 0x06001E75 RID: 7797 RVA: 0x000A2F04 File Offset: 0x000A1104
	private void OnEnable()
	{
		if (this.mode != RigEventVolume.Mode.RELATIVE)
		{
			NetworkSystem.Instance.OnPlayerLeft += this.OnNetLeft;
			return;
		}
		if (this.rigCollection != null)
		{
			VRRigCollection vrrigCollection = this.rigCollection;
			vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.OnJoined));
			VRRigCollection vrrigCollection2 = this.rigCollection;
			vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.OnLeft));
			return;
		}
		NetworkSystem.Instance.OnPlayerJoined += this.OnNetJoined;
		NetworkSystem.Instance.OnPlayerLeft += this.OnNetLeft;
	}

	// Token: 0x06001E76 RID: 7798 RVA: 0x000A2FE0 File Offset: 0x000A11E0
	private void OnDisable()
	{
		if (this.mode != RigEventVolume.Mode.RELATIVE)
		{
			NetworkSystem.Instance.OnPlayerLeft -= this.OnNetLeft;
			return;
		}
		if (this.rigCollection != null)
		{
			VRRigCollection vrrigCollection = this.rigCollection;
			vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Remove(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.OnJoined));
			VRRigCollection vrrigCollection2 = this.rigCollection;
			vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Remove(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.OnLeft));
			return;
		}
		NetworkSystem.Instance.OnPlayerJoined -= this.OnNetJoined;
		NetworkSystem.Instance.OnPlayerLeft -= this.OnNetLeft;
	}

	// Token: 0x06001E77 RID: 7799 RVA: 0x000A30BC File Offset: 0x000A12BC
	private void OnNetJoined(NetPlayer np)
	{
		int num = (int)((PhotonNetwork.CurrentRoom == null) ? 1 : PhotonNetwork.CurrentRoom.PlayerCount);
		this.countChanged(this.gameObjects.Count, this.gameObjects.Count, num - 1, num);
	}

	// Token: 0x06001E78 RID: 7800 RVA: 0x000A3100 File Offset: 0x000A1300
	private void OnNetLeft(NetPlayer np)
	{
		foreach (VRRig vrrig in VRRigCache.AllRigs)
		{
			if (vrrig.creator == np && this.rigs.Contains(vrrig))
			{
				RigEventVolumeTrigger rigEventVolumeTrigger = null;
				foreach (RigEventVolumeTrigger rigEventVolumeTrigger2 in this.gameObjects.Keys)
				{
					if (rigEventVolumeTrigger2.Rig == vrrig)
					{
						rigEventVolumeTrigger = rigEventVolumeTrigger2;
						break;
					}
				}
				this.HandleRigExit(rigEventVolumeTrigger, vrrig);
			}
		}
		if (this.mode == RigEventVolume.Mode.RELATIVE)
		{
			int num = (int)((PhotonNetwork.CurrentRoom == null) ? 1 : PhotonNetwork.CurrentRoom.PlayerCount);
			this.countChanged(this.gameObjects.Count, this.gameObjects.Count, num + 1, num);
		}
	}

	// Token: 0x06001E79 RID: 7801 RVA: 0x000A31FC File Offset: 0x000A13FC
	private void OnJoined(RigContainer rc)
	{
		int num = ((this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count);
		this.countChanged(this.gameObjects.Count, this.gameObjects.Count, num - 1, num);
	}

	// Token: 0x06001E7A RID: 7802 RVA: 0x000A324C File Offset: 0x000A144C
	private void OnLeft(RigContainer rc)
	{
		int num = ((this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count);
		this.countChanged(this.gameObjects.Count, this.gameObjects.Count, num + 1, num);
	}

	// Token: 0x06001E7B RID: 7803 RVA: 0x000A329C File Offset: 0x000A149C
	private void OnTriggerEnter(Collider other)
	{
		RigEventVolumeTrigger rigEventVolumeTrigger;
		if (other.gameObject.TryGetComponent<RigEventVolumeTrigger>(out rigEventVolumeTrigger))
		{
			if (!this.gameObjects.ContainsKey(rigEventVolumeTrigger))
			{
				this.gameObjects.Add(rigEventVolumeTrigger, 0);
				this.rigs.Add(rigEventVolumeTrigger.Rig);
				UnityEvent<VRRig> rigEnters = this.RigEnters;
				if (rigEnters != null)
				{
					rigEnters.Invoke(rigEventVolumeTrigger.Rig);
				}
				if (rigEventVolumeTrigger.Rig == VRRig.LocalRig)
				{
					UnityEvent<VRRig> localRigEnters = this.LocalRigEnters;
					if (localRigEnters != null)
					{
						localRigEnters.Invoke(rigEventVolumeTrigger.Rig);
					}
					this.localRigPresent = true;
				}
				if (this.mode != RigEventVolume.Mode.NONE)
				{
					int num = ((this.rigCollection == null) ? ((int)((PhotonNetwork.CurrentRoom == null) ? 1 : PhotonNetwork.CurrentRoom.PlayerCount)) : this.rigCollection.Rigs.Count);
					this.countChanged(this.gameObjects.Count - 1, this.gameObjects.Count, num, num);
					return;
				}
			}
			else
			{
				Dictionary<RigEventVolumeTrigger, int> dictionary = this.gameObjects;
				RigEventVolumeTrigger rigEventVolumeTrigger2 = rigEventVolumeTrigger;
				int num2 = dictionary[rigEventVolumeTrigger2];
				dictionary[rigEventVolumeTrigger2] = num2 + 1;
			}
		}
	}

	// Token: 0x06001E7C RID: 7804 RVA: 0x000A33AC File Offset: 0x000A15AC
	private void OnTriggerExit(Collider other)
	{
		RigEventVolumeTrigger rigEventVolumeTrigger;
		if (other.gameObject.TryGetComponent<RigEventVolumeTrigger>(out rigEventVolumeTrigger))
		{
			if (!this.gameObjects.ContainsKey(rigEventVolumeTrigger))
			{
				return;
			}
			Dictionary<RigEventVolumeTrigger, int> dictionary = this.gameObjects;
			RigEventVolumeTrigger rigEventVolumeTrigger2 = rigEventVolumeTrigger;
			int num = dictionary[rigEventVolumeTrigger2];
			dictionary[rigEventVolumeTrigger2] = num - 1;
			if (this.gameObjects[rigEventVolumeTrigger] < 0)
			{
				this.HandleRigExit(rigEventVolumeTrigger, rigEventVolumeTrigger.Rig);
			}
		}
	}

	// Token: 0x06001E7D RID: 7805 RVA: 0x000A340C File Offset: 0x000A160C
	private void HandleRigExit(RigEventVolumeTrigger trigger, VRRig rig)
	{
		if (trigger)
		{
			this.gameObjects.Remove(trigger);
		}
		this.rigs.Remove(rig);
		UnityEvent<VRRig> rigExits = this.RigExits;
		if (rigExits != null)
		{
			rigExits.Invoke(rig);
		}
		if (rig == VRRig.LocalRig)
		{
			UnityEvent<VRRig> localRigExits = this.LocalRigExits;
			if (localRigExits != null)
			{
				localRigExits.Invoke(rig);
			}
			this.localRigPresent = false;
		}
		if (this.mode != RigEventVolume.Mode.NONE)
		{
			int num = ((this.rigCollection == null) ? ((int)((PhotonNetwork.CurrentRoom == null) ? 1 : PhotonNetwork.CurrentRoom.PlayerCount)) : this.rigCollection.Rigs.Count);
			this.countChanged(this.gameObjects.Count + 1, this.gameObjects.Count, num, num);
		}
	}

	// Token: 0x06001E7E RID: 7806 RVA: 0x000A34D4 File Offset: 0x000A16D4
	private void countChanged(int oldValue, int newValue, int oldPlayerCount, int newPlayerCount)
	{
		float num = (float)newValue / (float)newPlayerCount;
		if (newValue > oldValue)
		{
			if ((this.mode == RigEventVolume.Mode.RELATIVE && num >= this.relThreshold && (float)oldValue / (float)oldPlayerCount < this.relThreshold) || (this.mode == RigEventVolume.Mode.ABSOLUTE && newValue >= this.absThreshold && oldValue < this.absThreshold))
			{
				UnityEvent goesOverThreshold = this.GoesOverThreshold;
				if (goesOverThreshold != null)
				{
					goesOverThreshold.Invoke();
				}
			}
		}
		else if (newValue < oldValue && ((this.mode == RigEventVolume.Mode.RELATIVE && num < this.relThreshold && (float)oldValue / (float)oldPlayerCount >= this.relThreshold) || (this.mode == RigEventVolume.Mode.ABSOLUTE && newValue < this.absThreshold && oldValue >= this.absThreshold)))
		{
			UnityEvent goesUnderThreshold = this.GoesUnderThreshold;
			if (goesUnderThreshold != null)
			{
				goesUnderThreshold.Invoke();
			}
		}
		Action onCountChanged = this.OnCountChanged;
		if (onCountChanged != null)
		{
			onCountChanged();
		}
		if (this.applyMultipliers)
		{
			UnityEvent<int> countChangedAbsolute = this.CountChangedAbsolute;
			if (countChangedAbsolute != null)
			{
				countChangedAbsolute.Invoke(Mathf.RoundToInt(this.MulitplierAbsolute.Evaluate((float)newValue)));
			}
			UnityEvent<float> countChangedRelative = this.CountChangedRelative;
			if (countChangedRelative == null)
			{
				return;
			}
			countChangedRelative.Invoke(this.MulitplierAbsolute.Evaluate(num));
			return;
		}
		else
		{
			UnityEvent<int> countChangedAbsolute2 = this.CountChangedAbsolute;
			if (countChangedAbsolute2 != null)
			{
				countChangedAbsolute2.Invoke(newValue);
			}
			UnityEvent<float> countChangedRelative2 = this.CountChangedRelative;
			if (countChangedRelative2 == null)
			{
				return;
			}
			countChangedRelative2.Invoke(num);
			return;
		}
	}

	// Token: 0x0400289B RID: 10395
	private Dictionary<RigEventVolumeTrigger, int> gameObjects = new Dictionary<RigEventVolumeTrigger, int>();

	// Token: 0x0400289C RID: 10396
	[SerializeField]
	private RigEventVolume.Mode mode = RigEventVolume.Mode.ABSOLUTE;

	// Token: 0x0400289D RID: 10397
	[Range(0.05f, 1f)]
	[SerializeField]
	private float relThreshold = 0.05f;

	// Token: 0x0400289E RID: 10398
	[SerializeField]
	private VRRigCollection rigCollection;

	// Token: 0x0400289F RID: 10399
	[Range(1f, 20f)]
	[SerializeField]
	private int absThreshold = 1;

	// Token: 0x040028A0 RID: 10400
	[SerializeField]
	private UnityEvent<VRRig> RigEnters;

	// Token: 0x040028A1 RID: 10401
	[SerializeField]
	private UnityEvent<VRRig> RigExits;

	// Token: 0x040028A2 RID: 10402
	[SerializeField]
	private UnityEvent GoesOverThreshold;

	// Token: 0x040028A3 RID: 10403
	[SerializeField]
	private UnityEvent GoesUnderThreshold;

	// Token: 0x040028A4 RID: 10404
	[SerializeField]
	private UnityEvent<VRRig> LocalRigEnters;

	// Token: 0x040028A5 RID: 10405
	[SerializeField]
	private UnityEvent<VRRig> LocalRigExits;

	// Token: 0x040028A6 RID: 10406
	private List<VRRig> rigs = new List<VRRig>();

	// Token: 0x040028A7 RID: 10407
	private bool localRigPresent;

	// Token: 0x040028A9 RID: 10409
	[SerializeField]
	private UnityEvent<int> CountChangedAbsolute;

	// Token: 0x040028AA RID: 10410
	[SerializeField]
	private UnityEvent<float> CountChangedRelative;

	// Token: 0x040028AB RID: 10411
	[SerializeField]
	private bool applyMultipliers;

	// Token: 0x040028AC RID: 10412
	[SerializeField]
	private AnimationCurve MulitplierAbsolute;

	// Token: 0x040028AD RID: 10413
	[SerializeField]
	private AnimationCurve MulitplierRelative;

	// Token: 0x020004E3 RID: 1251
	private enum Mode
	{
		// Token: 0x040028AF RID: 10415
		RELATIVE,
		// Token: 0x040028B0 RID: 10416
		ABSOLUTE,
		// Token: 0x040028B1 RID: 10417
		NONE
	}
}
