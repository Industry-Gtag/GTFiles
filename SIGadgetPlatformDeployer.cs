using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000111 RID: 273
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetPlatformDeployer : SIGadget, I_SIDisruptable, IEnergyGadget
{
	// Token: 0x06000670 RID: 1648 RVA: 0x00023E40 File Offset: 0x00022040
	private void Start()
	{
		this.previewPlatform.SetActive(false);
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnReleased = (Action)Delegate.Combine(gameEntity.OnReleased, new Action(this.HandleStopInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnUnsnapped = (Action)Delegate.Combine(gameEntity2.OnUnsnapped, new Action(this.HandleStopInteraction));
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x00023EA8 File Offset: 0x000220A8
	private void OnDestroy()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnReleased = (Action)Delegate.Remove(gameEntity.OnReleased, new Action(this.HandleStopInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnUnsnapped = (Action)Delegate.Remove(gameEntity2.OnUnsnapped, new Action(this.HandleStopInteraction));
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x00023F03 File Offset: 0x00022103
	private void HandleStopInteraction()
	{
		this.SetState(SIGadgetPlatformDeployer.State.Idle);
	}

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x06000673 RID: 1651 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool UsesEnergy
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700006C RID: 108
	// (get) Token: 0x06000674 RID: 1652 RVA: 0x00023F0F File Offset: 0x0002210F
	public bool IsFull
	{
		get
		{
			return this.remainingRechargeTime <= 0f;
		}
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x00023F24 File Offset: 0x00022124
	public void UpdateRecharge(float dt)
	{
		if (this.remainingRechargeTime > 0f)
		{
			int num = Mathf.CeilToInt(this.remainingRechargeTime / this.chargeRecoveryTime);
			this.remainingRechargeTime = Mathf.Max(this.remainingRechargeTime - dt, 0f);
			int num2 = Mathf.CeilToInt(this.remainingRechargeTime / this.chargeRecoveryTime);
			this.chargeDisplay.UpdateDisplay(this.maxCharges - num2);
			if (num2 != num && this.gameEntity.IsHeldOrSnappedByLocalPlayer)
			{
				this.rechargeSFX.Play();
				bool flag;
				if (base.FindAttachedHand(out flag))
				{
					GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				}
			}
		}
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x00023FE4 File Offset: 0x000221E4
	protected override void OnUpdateAuthority(float dt)
	{
		SIGadgetPlatformDeployer.State state = this.state;
		if (state != SIGadgetPlatformDeployer.State.Idle)
		{
			if (state != SIGadgetPlatformDeployer.State.Deploying)
			{
				return;
			}
			if (this.CheckReleaseInputs())
			{
				if (this.IsChargeAvailable())
				{
					this.TryDeployPlatform();
				}
				this.SetStateAuthority(SIGadgetPlatformDeployer.State.Idle);
				return;
			}
			this.UpdatePreview();
			return;
		}
		else
		{
			if (this.CheckInitInputs())
			{
				if (this.IsChargeAvailable())
				{
					if (this.isInstancePlace)
					{
						if (!this.wasInputPressed)
						{
							this.TryDeployInstantPlatform();
						}
					}
					else
					{
						this.SetStateAuthority(SIGadgetPlatformDeployer.State.Deploying);
					}
				}
				this.wasInputPressed = true;
				return;
			}
			this.wasInputPressed = false;
			return;
		}
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x00024064 File Offset: 0x00022264
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetPlatformDeployer.State state = (SIGadgetPlatformDeployer.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
		SIGadgetPlatformDeployer.State state2 = this.state;
		if (state2 != SIGadgetPlatformDeployer.State.Idle && state2 == SIGadgetPlatformDeployer.State.Deploying)
		{
			this.UpdatePreview();
		}
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x000240A4 File Offset: 0x000222A4
	private bool CheckInitInputs()
	{
		if (!this.buttonActivatable.CheckInput(this.inputSensitivity))
		{
			return false;
		}
		if (this.isInstancePlace)
		{
			return true;
		}
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		Vector3 position = gamePlayer.leftHand.position;
		Vector3 position2 = gamePlayer.rightHand.position;
		return Vector3.Distance(position, position2) <= this.activationHandDistance;
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x00024105 File Offset: 0x00022305
	private bool CheckReleaseInputs()
	{
		return !this.buttonActivatable.CheckInput(this.inputSensitivity);
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x0002411B File Offset: 0x0002231B
	private bool IsChargeAvailable()
	{
		return (float)this.maxCharges * this.chargeRecoveryTime - this.remainingRechargeTime > this.chargeRecoveryTime;
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x0002413D File Offset: 0x0002233D
	private void SpendCharge()
	{
		this.remainingRechargeTime += this.chargeRecoveryTime;
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x00024152 File Offset: 0x00022352
	private static bool IsLeftHandOrSnapSlot(int handIndex)
	{
		return handIndex == 0 || handIndex == 2;
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x00024160 File Offset: 0x00022360
	private void TryDeployInstantPlatform()
	{
		if (base.IsBlocked())
		{
			this.blockedSFX.Play();
			return;
		}
		GamePlayer gamePlayer;
		if (!this.TryGetGamePlayer(out gamePlayer))
		{
			return;
		}
		int num = gamePlayer.FindSnapIndex(this.gameEntity.id);
		if (num == -1)
		{
			num = gamePlayer.FindHandIndex(this.gameEntity.id);
		}
		if (num == -1)
		{
			return;
		}
		Vector3 vector;
		Quaternion quaternion;
		if (this.gameEntity.IsHeldByLocalPlayer())
		{
			vector = base.transform.position - base.transform.up * this.handDepthOffset;
			quaternion = base.transform.rotation;
			Debug.DrawRay(base.transform.position, -base.transform.up * 0.3f, Color.blue, 10f);
			Debug.DrawRay(base.transform.position, base.transform.forward * 0.3f, Color.blue, 10f);
			Debug.DrawRay(vector, quaternion * Vector3.forward * 0.3f, Color.green, 10f);
		}
		else
		{
			Transform transform = (SIGadgetPlatformDeployer.IsLeftHandOrSnapSlot(num) ? gamePlayer.leftHand : gamePlayer.rightHand);
			vector = transform.position;
			Vector3 up = transform.up;
			Vector3 right = transform.right;
			Debug.DrawRay(vector, right * 0.3f, Color.red, 10f);
			Debug.DrawRay(vector, up * 0.3f, Color.red, 10f);
			quaternion = Quaternion.LookRotation(up, right);
			vector += right * this.handDepthOffset;
			Debug.DrawRay(vector, quaternion * Vector3.forward * 0.3f, Color.green, 10f);
		}
		this.DeployPlatform(vector, quaternion);
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x00024338 File Offset: 0x00022538
	private void TryDeployPlatform()
	{
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		Vector3 position = gamePlayer.leftHand.position;
		Vector3 position2 = gamePlayer.rightHand.position;
		if (Vector3.Distance(position, position2) > this.deployMinRequiredHandDistance)
		{
			if (base.IsBlocked())
			{
				this.blockedSFX.Play();
				return;
			}
			Vector3 vector;
			Quaternion quaternion;
			Vector3 vector2;
			if (this.TryGetPlatformPosRotScale(out vector, out quaternion, out vector2))
			{
				this.DeployPlatform(vector, quaternion);
				return;
			}
		}
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x000243A4 File Offset: 0x000225A4
	private void DeployPlatform(Vector3 pos, Quaternion rot)
	{
		this.SpendCharge();
		this.CreateLocalPlatformInstance(pos, rot);
		int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		if (this.gameEntity.IsAuthority())
		{
			base.SendAuthorityToClientRPC(0, new object[] { actorNumber, pos, rot });
			return;
		}
		base.SendClientToAuthorityRPC(0, new object[] { actorNumber, pos, rot });
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x00024430 File Offset: 0x00022630
	public override void ProcessClientToAuthorityRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
		if (rpcID == 0)
		{
			if (data == null || data.Length != 3)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			Vector3 vector;
			if (!GameEntityManager.ValidateDataType<Vector3>(data[1], out vector))
			{
				return;
			}
			Quaternion quaternion;
			if (!GameEntityManager.ValidateDataType<Quaternion>(data[2], out quaternion))
			{
				return;
			}
			if (!this.gameEntity.IsAttachedToPlayer(NetPlayer.Get(info.Sender)))
			{
				return;
			}
			if (Vector3.Distance(base.transform.position, vector) > 2f)
			{
				return;
			}
			this.CreateLocalPlatformInstance(vector, quaternion);
			base.SendAuthorityToClientRPC(0, data);
		}
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x000244B4 File Offset: 0x000226B4
	public override void ProcessAuthorityToClientRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
		if (rpcID == 0)
		{
			if (data == null || data.Length != 3)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			Vector3 vector;
			if (!GameEntityManager.ValidateDataType<Vector3>(data[1], out vector))
			{
				return;
			}
			Quaternion quaternion;
			if (!GameEntityManager.ValidateDataType<Quaternion>(data[2], out quaternion))
			{
				return;
			}
			if (num != NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.CreateLocalPlatformInstance(vector, quaternion);
			}
		}
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x00024510 File Offset: 0x00022710
	private void CreateLocalPlatformInstance(Vector3 pos, Quaternion rot)
	{
		if (this.deployedPlatformCount >= this.maxCharges)
		{
			return;
		}
		GameObject gameObject = ObjectPools.instance.Instantiate(this.platformPrefab, true);
		if (gameObject != null)
		{
			SIGadgetPlatformDeployerPlatform component = gameObject.GetComponent<SIGadgetPlatformDeployerPlatform>();
			if (component != null)
			{
				this.deployedPlatformCount++;
				SIGadgetPlatformDeployerPlatform sigadgetPlatformDeployerPlatform = component;
				sigadgetPlatformDeployerPlatform.OnDisabled = (Action)Delegate.Combine(sigadgetPlatformDeployerPlatform.OnDisabled, new Action(delegate
				{
					this.deployedPlatformCount--;
				}));
			}
			gameObject.transform.SetPositionAndRotation(pos, rot);
			ISIGameDeployable isigameDeployable;
			if (gameObject.TryGetComponent<ISIGameDeployable>(out isigameDeployable))
			{
				isigameDeployable.ApplyUpgrades(this.instanceUpgrades);
			}
		}
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x000245AA File Offset: 0x000227AA
	private void SetStateAuthority(SIGadgetPlatformDeployer.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x000245CC File Offset: 0x000227CC
	private void SetState(SIGadgetPlatformDeployer.State newState)
	{
		if (newState == this.state || !this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		SIGadgetPlatformDeployer.State state = this.state;
		if (state == SIGadgetPlatformDeployer.State.Idle)
		{
			this.SetPreviewVisibility(false);
			return;
		}
		if (state != SIGadgetPlatformDeployer.State.Deploying)
		{
			return;
		}
		this.SetPreviewVisibility(true);
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x00024612 File Offset: 0x00022812
	public bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 2L;
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x00024621 File Offset: 0x00022821
	private void SetPreviewVisibility(bool enabled)
	{
		this.previewPlatform.SetActive(enabled);
		if (enabled)
		{
			this.UpdatePreview();
		}
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x00024638 File Offset: 0x00022838
	private void UpdatePreview()
	{
		Vector3 vector;
		Quaternion quaternion;
		Vector3 vector2;
		if (this.TryGetPlatformPosRotScale(out vector, out quaternion, out vector2))
		{
			this.previewPlatform.transform.SetPositionAndRotation(vector, quaternion);
			this.previewPlatform.transform.localScale = vector2;
			GamePlayer gamePlayer;
			if (this.TryGetGamePlayer(out gamePlayer))
			{
				Vector3 position = gamePlayer.leftHand.position;
				Vector3 position2 = gamePlayer.rightHand.position;
				if (Vector3.Distance(position, position2) > this.deployMinRequiredHandDistance)
				{
					this.previewMesh.material = this.validPreviewMaterial;
					return;
				}
				this.previewMesh.material = this.invalidPreviewMaterial;
			}
		}
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x000246CC File Offset: 0x000228CC
	private bool TryGetPlatformPosRotScale(out Vector3 pos, out Quaternion rot, out Vector3 scale)
	{
		pos = Vector3.zero;
		rot = Quaternion.identity;
		scale = Vector3.one;
		GamePlayer gamePlayer;
		if (this.TryGetGamePlayer(out gamePlayer))
		{
			Vector3 position = gamePlayer.leftHand.position;
			Vector3 position2 = gamePlayer.rightHand.position;
			Vector3 position3 = gamePlayer.rig.head.rigTarget.position;
			Vector3 vector = (position + position2) / 2f;
			Vector3 normalized = (position3 - vector).normalized;
			Vector3 vector2 = Vector3.ProjectOnPlane((position - position2).normalized, normalized);
			pos = vector + -normalized * this.handDepthOffset;
			rot = Quaternion.LookRotation(vector2, normalized);
			return true;
		}
		return false;
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x000247A0 File Offset: 0x000229A0
	private bool TryGetGamePlayer(out GamePlayer player)
	{
		player = null;
		return GamePlayer.TryGetGamePlayer(this.gameEntity.snappedByActorNumber, out player) || GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out player);
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x000247D0 File Offset: 0x000229D0
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this.instanceUpgrades = withUpgrades;
		bool flag = withUpgrades.Contains(SIUpgradeType.Platform_Capacity);
		this.maxCharges = (flag ? this.maxChargesHighCapacity : this.maxChargesDefault);
		this.chargeDisplay = (flag ? this.chargeDisplayHighCapacity : this.chargeDisplayDefault);
		this.chargeRecoveryTime = (withUpgrades.Contains(SIUpgradeType.Platform_Cooldown) ? this.chargeRecoveryTimeFast : this.chargeRecoveryTimeDefault);
	}

	// Token: 0x0600068B RID: 1675 RVA: 0x00024841 File Offset: 0x00022A41
	public void Disrupt(float disruptTime)
	{
		this.remainingRechargeTime = (float)this.maxCharges * this.chargeRecoveryTime + disruptTime;
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x00024859 File Offset: 0x00022A59
	protected override void HandleBlockedActionChanged(bool isBlocked)
	{
		this.blockedDisplayMesh.material = (isBlocked ? this.blockedMat : this.unblockedMat);
	}

	// Token: 0x040007DA RID: 2010
	[SerializeField]
	private GameButtonActivatable buttonActivatable;

	// Token: 0x040007DB RID: 2011
	[SerializeField]
	private SoundBankPlayer rechargeSFX;

	// Token: 0x040007DC RID: 2012
	[SerializeField]
	private SoundBankPlayer blockedSFX;

	// Token: 0x040007DD RID: 2013
	[SerializeField]
	private MeshRenderer blockedDisplayMesh;

	// Token: 0x040007DE RID: 2014
	[SerializeField]
	private Material unblockedMat;

	// Token: 0x040007DF RID: 2015
	[SerializeField]
	private Material blockedMat;

	// Token: 0x040007E0 RID: 2016
	[SerializeField]
	private GameObject platformPrefab;

	// Token: 0x040007E1 RID: 2017
	[Header("Activation")]
	[SerializeField]
	private bool isInstancePlace;

	// Token: 0x040007E2 RID: 2018
	[SerializeField]
	private float activationHandDistance = 0.2f;

	// Token: 0x040007E3 RID: 2019
	[SerializeField]
	private float inputSensitivity = 0.25f;

	// Token: 0x040007E4 RID: 2020
	[Header("Deploy")]
	[SerializeField]
	private float deployMinRequiredHandDistance = 0.2f;

	// Token: 0x040007E5 RID: 2021
	[SerializeField]
	private GameObject previewPlatform;

	// Token: 0x040007E6 RID: 2022
	[SerializeField]
	private float handInset = 0.1f;

	// Token: 0x040007E7 RID: 2023
	[SerializeField]
	private float handDepthOffset = 0.3f;

	// Token: 0x040007E8 RID: 2024
	[SerializeField]
	private MeshRenderer previewMesh;

	// Token: 0x040007E9 RID: 2025
	[SerializeField]
	private Material validPreviewMaterial;

	// Token: 0x040007EA RID: 2026
	[SerializeField]
	private Material invalidPreviewMaterial;

	// Token: 0x040007EB RID: 2027
	[Header("Charges")]
	private int maxCharges = 3;

	// Token: 0x040007EC RID: 2028
	private float chargeRecoveryTime = 10f;

	// Token: 0x040007ED RID: 2029
	private SIChargeDisplay chargeDisplay;

	// Token: 0x040007EE RID: 2030
	[SerializeField]
	private int maxChargesDefault = 3;

	// Token: 0x040007EF RID: 2031
	[SerializeField]
	private int maxChargesHighCapacity = 5;

	// Token: 0x040007F0 RID: 2032
	[SerializeField]
	private SIChargeDisplay chargeDisplayDefault;

	// Token: 0x040007F1 RID: 2033
	[SerializeField]
	private SIChargeDisplay chargeDisplayHighCapacity;

	// Token: 0x040007F2 RID: 2034
	[SerializeField]
	private float chargeRecoveryTimeDefault = 10f;

	// Token: 0x040007F3 RID: 2035
	[SerializeField]
	private float chargeRecoveryTimeFast = 5f;

	// Token: 0x040007F4 RID: 2036
	private SIGadgetPlatformDeployer.State state;

	// Token: 0x040007F5 RID: 2037
	private bool wasInputPressed;

	// Token: 0x040007F6 RID: 2038
	private float remainingRechargeTime;

	// Token: 0x040007F7 RID: 2039
	private SIUpgradeSet instanceUpgrades;

	// Token: 0x040007F8 RID: 2040
	private const float MAX_DEPLOY_DIST = 2f;

	// Token: 0x040007F9 RID: 2041
	private int deployedPlatformCount;

	// Token: 0x02000112 RID: 274
	private enum State
	{
		// Token: 0x040007FB RID: 2043
		Idle,
		// Token: 0x040007FC RID: 2044
		Deploying,
		// Token: 0x040007FD RID: 2045
		Count
	}
}
