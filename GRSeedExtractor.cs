using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x020007E7 RID: 2023
public class GRSeedExtractor : MonoBehaviour
{
	// Token: 0x170004AF RID: 1199
	// (get) Token: 0x0600339C RID: 13212 RVA: 0x0011A01B File Offset: 0x0011821B
	public bool StationOpen
	{
		get
		{
			return this.stationOpen;
		}
	}

	// Token: 0x170004B0 RID: 1200
	// (get) Token: 0x0600339D RID: 13213 RVA: 0x0011A023 File Offset: 0x00118223
	public bool StationOpenForLocalPlayer
	{
		get
		{
			return this.stationOpen && this.currentPlayerActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x170004B1 RID: 1201
	// (get) Token: 0x0600339E RID: 13214 RVA: 0x0011A046 File Offset: 0x00118246
	public int CurrentPlayerActorNumber
	{
		get
		{
			return this.currentPlayerActorNumber;
		}
	}

	// Token: 0x0600339F RID: 13215 RVA: 0x0011A050 File Offset: 0x00118250
	private void Awake()
	{
		this.triggerNotifier.TriggerEnterEvent += this.TriggerEntered;
		this.triggerNotifier.TriggerExitEvent += this.TriggerExited;
		this.coreDepositTriggerNotifier.TriggerEnterEvent += this.DepositorTriggerEntered;
		this.idCardScanner.OnPlayerCardSwipe += this.OnPlayerCardSwipe;
		for (int i = 0; i < this.maxVisualChaosSeedCount; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.chaosSeedVisualPrefab, base.transform);
			gameObject.SetActive(false);
			this.chaosSeedVisuals.Add(gameObject);
		}
		this.UpdateOverdrivePurchaseButtons();
		base.enabled = false;
	}

	// Token: 0x060033A0 RID: 13216 RVA: 0x0011A0FC File Offset: 0x001182FC
	public void Init(GRToolProgressionManager progression, GhostReactor gr)
	{
		this.ghostReactor = gr;
		this.toolProgressionManager = progression;
		this.toolProgressionManager.OnProgressionUpdated += this.OnResearchPointsUpdated;
		ProgressionManager.Instance.OnJucierStatusUpdated += this.OnPlayerStatusReceived;
		ProgressionManager.Instance.OnPurchaseOverdrive += this.OnPurchaseOverdrive;
		ProgressionManager.Instance.OnChaosDepositSuccess += this.TryDepositSeedServerResponse;
	}

	// Token: 0x060033A1 RID: 13217 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnEnable()
	{
	}

	// Token: 0x060033A2 RID: 13218 RVA: 0x0011A170 File Offset: 0x00118370
	private void OnDisable()
	{
		this.ClearSeedVisuals();
		this.machineHumAudioSource.gameObject.SetActive(false);
		this.juicerSlowParticles.gameObject.SetActive(false);
		base.StopAllCoroutines();
		for (int i = 0; i < this.disableDuringOverdrive.Count; i++)
		{
			this.disableDuringOverdrive[i].gameObject.SetActive(true);
		}
		for (int j = 0; j < this.enableDuringOverdrive.Count; j++)
		{
			this.enableDuringOverdrive[j].gameObject.SetActive(false);
		}
		this.overdriveLightSpinnerOff.localRotation = this.overdriveLightSpinnerOn.localRotation;
		this.overdriveBeepAudioSource.Stop();
		this.overdriveActive = false;
		this.processingAmount = 0f;
		this.processingAmountVisual = 0f;
		this.overdriveAmount = 0f;
		this.overdriveAmountVisual = 0f;
		this.currentPlayerData = default(GRSeedExtractor.PlayerData);
		this.overdriveLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.overdriveAmountVisual), 1f);
		this.processingLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.processingAmountVisual), 1f);
	}

	// Token: 0x060033A3 RID: 13219 RVA: 0x0011A2BC File Offset: 0x001184BC
	private void Update()
	{
		this.ValidateCurrentPlayer();
		if (this.stationOpen && this.shutterDoorOpenAmount < 1f)
		{
			float num = Time.time - this.currentPlayerData.latestRefreshTime;
			if (Time.time - this.stationOpenRequestTime >= 1f || num <= 5f)
			{
				float num2 = 1f / this.shutterDoorAnimTime;
				this.shutterDoorOpenAmount = Mathf.MoveTowards(this.shutterDoorOpenAmount, 1f, num2 * Time.deltaTime);
				Vector3 localPosition = this.shutterDoorParent.transform.localPosition;
				localPosition.y = Mathf.Lerp(this.shutterDoorLiftRange.x, this.shutterDoorLiftRange.y, this.shutterDoorOpenAmount);
				this.shutterDoorParent.transform.localPosition = localPosition;
			}
		}
		else if (!this.stationOpen && this.shutterDoorOpenAmount > 0f)
		{
			float num3 = 1f / this.shutterDoorAnimTime;
			this.shutterDoorOpenAmount = Mathf.MoveTowards(this.shutterDoorOpenAmount, 0f, num3 * Time.deltaTime);
			Vector3 localPosition2 = this.shutterDoorParent.transform.localPosition;
			localPosition2.y = Mathf.Lerp(this.shutterDoorLiftRange.x, this.shutterDoorLiftRange.y, this.shutterDoorOpenAmount);
			this.shutterDoorParent.transform.localPosition = localPosition2;
			if (this.shutterDoorOpenAmount <= 0f)
			{
				this.processingAmount = 0f;
				this.overdriveAmount = 0f;
			}
		}
		bool flag = this.seedProcessingStates.Count > 0 && this.seedProcessingStates[0].dropProgress >= 1f;
		if (this.overdriveActive)
		{
			this.overdriveLightSpinnerOn.Rotate(Vector3.forward, 360f * this.overdriveLightSpinRate * Time.deltaTime, Space.Self);
			this.overdriveAmountVisual = this.overdriveAmount;
			this.overdriveLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.overdriveAmountVisual), 1f);
			this.processingAmountVisual = this.processingAmount;
			this.processingLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.processingAmountVisual), 1f);
		}
		else
		{
			float num4 = 1f / this.overdriveFillTime;
			if (flag || this.overdriveAmount > this.overdriveAmountVisual || !this.stationOpen)
			{
				this.overdriveAmountVisual = Mathf.MoveTowards(this.overdriveAmountVisual, this.overdriveAmount, num4 * Time.deltaTime);
			}
			this.overdriveLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.overdriveAmountVisual), 1f);
			if (this.stationOpen)
			{
				float num5 = Mathf.Max(Time.time - this.currentPlayerData.latestRefreshTime, 0f);
				float num6 = this.currentPlayerData.coreProcessingPercentage + num5 / this.PROCESSING_TIME_SECONDS;
				this.processingAmount = Mathf.Clamp01(num6);
				this.estimatedJuiceTimeRemaining = (1f - this.processingAmount) * this.PROCESSING_TIME_SECONDS;
				if (this.StationOpenForLocalPlayer && num6 >= 1f && Time.time - this.lastServerRequestTime > this.timeBetweenServerRequests)
				{
					this.lastServerRequestTime = Time.time;
					ProgressionManager.Instance.GetJuicerStatus();
				}
			}
			if (flag)
			{
				this.machineHumAudioSource.gameObject.SetActive(true);
				this.juicerSlowParticles.gameObject.SetActive(true);
				this.processingAmountVisual = Mathf.MoveTowards(this.processingAmountVisual, this.processingAmount, num4 * Time.deltaTime);
			}
			else
			{
				this.processingAmountVisual = Mathf.MoveTowards(this.processingAmountVisual, 0f, num4 * Time.deltaTime);
				this.machineHumAudioSource.gameObject.SetActive(false);
				this.juicerSlowParticles.gameObject.SetActive(false);
			}
			this.processingLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.processingAmountVisual), 1f);
		}
		this.StepSeedVisualAnimation(Time.deltaTime);
		this.UpdateScreenDisplay();
		if (!this.stationOpen && this.shutterDoorOpenAmount <= 0f && this.overdriveAmountVisual <= 0f)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060033A4 RID: 13220 RVA: 0x0011A714 File Offset: 0x00118914
	private void ValidateCurrentPlayer()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.CloseStation();
			return;
		}
		if (this.ghostReactor.grManager.IsAuthority() && this.stationOpen)
		{
			bool flag = false;
			NetPlayer player = NetworkSystem.Instance.GetPlayer(this.currentPlayerActorNumber);
			RigContainer rigContainer;
			if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				float num = 5f;
				if (rigContainer.Rig != null && rigContainer.Rig.OwningNetPlayer == player && (rigContainer.Rig.GetMouthPosition() - base.transform.position).sqrMagnitude < num * num)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorCloseStation, NetworkSystem.Instance.LocalPlayer.ActorNumber, 0);
			}
		}
	}

	// Token: 0x060033A5 RID: 13221 RVA: 0x0011A7EC File Offset: 0x001189EC
	public void TriggerEntered(TriggerEventNotifier notifier, Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component != null && component.OwningNetPlayer != null && component.OwningNetPlayer.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber && NetworkSystem.Instance.InRoom)
		{
			ProgressionManager.Instance.GetJuicerStatus();
		}
	}

	// Token: 0x060033A6 RID: 13222 RVA: 0x0011A844 File Offset: 0x00118A44
	public void TriggerExited(TriggerEventNotifier notifier, Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component != null && component.OwningNetPlayer != null)
		{
			if (component.OwningNetPlayer.ActorNumber == this.currentPlayerActorNumber && this.stationOpen && this.ghostReactor.grManager.IsAuthority() && NetworkSystem.Instance.InRoom)
			{
				this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorCloseStation, NetworkSystem.Instance.LocalPlayer.ActorNumber, 0);
			}
			if (component.OwningNetPlayer.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.localPlayerData = default(GRSeedExtractor.PlayerData);
			}
		}
	}

	// Token: 0x060033A7 RID: 13223 RVA: 0x0011A8F4 File Offset: 0x00118AF4
	public void OnPlayerCardSwipe(int playerActorNumber)
	{
		if (playerActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber && NetworkSystem.Instance.InRoom)
		{
			this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorOpenStation, NetworkSystem.Instance.LocalPlayer.ActorNumber, 0);
			ProgressionManager.Instance.GetJuicerStatus();
		}
	}

	// Token: 0x060033A8 RID: 13224 RVA: 0x0011A94C File Offset: 0x00118B4C
	public void DepositorTriggerEntered(TriggerEventNotifier notifier, Collider other)
	{
		if (this.ghostReactor == null || this.ghostReactor.grManager == null || other == null || !NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (this.ghostReactor.grManager.IsAuthority() && other.attachedRigidbody != null)
		{
			GRCollectible component = other.attachedRigidbody.GetComponent<GRCollectible>();
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.zone);
			if (managerForZone != null && component != null && component.type == ProgressionManager.CoreType.ChaosSeed)
			{
				int netIdFromEntityId = managerForZone.GetNetIdFromEntityId(component.entity.id);
				int lastHeldByActorNumber = component.entity.lastHeldByActorNumber;
				bool player = NetworkSystem.Instance.GetPlayer(lastHeldByActorNumber) != null;
				float time = Time.time;
				if (player)
				{
					bool flag = false;
					for (int i = this.seedDepositsPending.Count - 1; i >= 0; i--)
					{
						if (time - this.seedDepositsPending[i].Item3 > 5f || managerForZone.GetGameEntityFromNetId(this.seedDepositsPending[i].Item1) == null || NetworkSystem.Instance.GetPlayer(this.seedDepositsPending[i].Item2) == null)
						{
							this.seedDepositsPending.RemoveAt(i);
						}
						else if (this.seedDepositsPending[i].Item1 == netIdFromEntityId)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						this.seedDepositsPending.Add(new ValueTuple<int, int, float, bool>(netIdFromEntityId, lastHeldByActorNumber, Time.time, false));
						this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorTryDepositSeed, lastHeldByActorNumber, netIdFromEntityId);
					}
				}
			}
		}
	}

	// Token: 0x060033A9 RID: 13225 RVA: 0x0011AAFD File Offset: 0x00118CFD
	public void OverdrivePurchaseButtonPressed()
	{
		if (this.overdrivePurchasePending)
		{
			this.overdrivePurchasePending = false;
		}
		else if (this.LocalPlayerCanPurchaseOverdrive())
		{
			this.overdrivePurchasePending = true;
		}
		this.UpdateOverdrivePurchaseButtons();
	}

	// Token: 0x060033AA RID: 13226 RVA: 0x0011AB28 File Offset: 0x00118D28
	private bool LocalPlayerCanPurchaseOverdrive()
	{
		if (Time.time - this.overdrivePurchaseTime > 5f)
		{
			this.overdriveServerConfirmationPending = false;
		}
		return this.StationOpenForLocalPlayer && !this.overdriveServerConfirmationPending && CosmeticsController.instance.CurrencyBalance >= 250 && this.localPlayerData.overdriveSupply <= 0f;
	}

	// Token: 0x060033AB RID: 13227 RVA: 0x0011AB90 File Offset: 0x00118D90
	public void OverdrivePurchaseConfirmButtonPressed()
	{
		if (this.overdrivePurchasePending)
		{
			this.overdrivePurchasePending = false;
			if (this.stationOpen && this.currentPlayerActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.overdriveServerConfirmationPending = true;
				this.overdrivePurchaseTime = Time.time;
				ProgressionManager.Instance.PurchaseOverdrive();
			}
		}
		this.UpdateOverdrivePurchaseButtons();
	}

	// Token: 0x060033AC RID: 13228 RVA: 0x0011ABF0 File Offset: 0x00118DF0
	public void OnPlayerStatusReceived(ProgressionManager.JuicerStatusResponse statusResponse)
	{
		if (statusResponse.MothershipId == GRPlayer.GetLocal().mothershipId && statusResponse.RefreshJuice)
		{
			this.toolProgressionManager.UpdateInventory();
		}
		this.PROCESSING_TIME_SECONDS = (float)statusResponse.CoreProcessingTimeSec;
		this.MAX_OVERDRIVE_USES = statusResponse.OverdriveCap / 100;
		float num = Mathf.Clamp01((float)statusResponse.OverdriveSupply / (float)statusResponse.OverdriveCap);
		int num2 = 0;
		bool flag = num < this.localPlayerData.overdriveSupply;
		bool flag2 = this.localPlayerData.overdriveSupply == 0f && this.localPlayerData.coreCount > statusResponse.CurrentCoreCount;
		if (statusResponse.CoresProcessedByOverdrive > 0 && (flag || flag2))
		{
			num2 = statusResponse.CoresProcessedByOverdrive;
		}
		this.localPlayerData.actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		this.localPlayerData.coreCount = statusResponse.CurrentCoreCount;
		this.localPlayerData.coreProcessingPercentage = Mathf.Clamp01(statusResponse.CoreProcessingPercent);
		this.localPlayerData.overdriveSupply = num;
		this.localPlayerData.coresProcessedByOverdrive = statusResponse.CoresProcessedByOverdrive;
		this.localPlayerData.coresPendingOverdriveProcessing = this.localPlayerData.coresPendingOverdriveProcessing + num2;
		this.localPlayerData.latestRefreshTime = Time.time;
		this.localPlayerData.researchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
		if (this.overdriveServerConfirmationPending && (this.localPlayerData.overdriveSupply > 0f || this.localPlayerData.coresProcessedByOverdrive > 0))
		{
			this.overdriveServerConfirmationPending = false;
		}
		if (this.stationOpen && this.currentPlayerActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber && NetworkSystem.Instance.InRoom)
		{
			this.currentPlayerData = this.localPlayerData;
			this.ghostReactor.grManager.RequestApplySeedExtractorState(this.localPlayerData.coreCount, this.localPlayerData.coresProcessedByOverdrive, this.localPlayerData.researchPoints, this.localPlayerData.coreProcessingPercentage, this.localPlayerData.overdriveSupply);
			this.OnStateUpdated();
		}
	}

	// Token: 0x060033AD RID: 13229 RVA: 0x0011ADF0 File Offset: 0x00118FF0
	private void TryDepositSeedServerResponse(bool succeeded)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		int num = -1;
		int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		for (int i = 0; i < this.seedDepositsPending.Count; i++)
		{
			if (this.seedDepositsPending[i].Item2 == actorNumber)
			{
				num = this.seedDepositsPending[i].Item1;
			}
		}
		if (num == -1)
		{
			return;
		}
		if (succeeded)
		{
			this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorDepositSeedSucceeded, actorNumber, num);
			this.RemovePendingSeedDeposit(num);
			GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
			grplayer.SendSeedDepositedTelemetry(this.PROCESSING_TIME_SECONDS.ToString(), this.currentPlayerData.coreCount);
			grplayer.IncrementChaosSeedsCollected(1);
			return;
		}
		this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorDepositSeedFailed, actorNumber, num);
	}

	// Token: 0x060033AE RID: 13230 RVA: 0x0011AEBC File Offset: 0x001190BC
	public void CardSwipeSuccess()
	{
		this.idCardScanner.onSucceeded.Invoke();
	}

	// Token: 0x060033AF RID: 13231 RVA: 0x0011AECE File Offset: 0x001190CE
	public void CardSwipeFail()
	{
		this.idCardScanner.onFailed.Invoke();
	}

	// Token: 0x060033B0 RID: 13232 RVA: 0x0011AEE0 File Offset: 0x001190E0
	public void TryDepositSeed(int playerActorNumber, int seedNetId)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(playerActorNumber);
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.zone);
		if (player == null || managerForZone == null)
		{
			return;
		}
		this.depositorAudioSource.PlayOneShot(this.seedDepositAttemptAudio, this.seedDepositAttemptVolume);
		if (player.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			bool flag = false;
			float time = Time.time;
			for (int i = this.seedDepositsPending.Count - 1; i >= 0; i--)
			{
				if (time - this.seedDepositsPending[i].Item3 > 5f || managerForZone.GetGameEntityFromNetId(this.seedDepositsPending[i].Item1) == null || NetworkSystem.Instance.GetPlayer(this.seedDepositsPending[i].Item2) == null)
				{
					this.seedDepositsPending.RemoveAt(i);
				}
				else if (this.seedDepositsPending[i].Item1 == seedNetId)
				{
					flag = true;
					if (this.seedDepositsPending[i].Item2 == NetworkSystem.Instance.LocalPlayer.ActorNumber && !this.seedDepositsPending[i].Item4)
					{
						ValueTuple<int, int, float, bool> valueTuple = this.seedDepositsPending[i];
						valueTuple.Item4 = true;
						this.seedDepositsPending[i] = valueTuple;
						ProgressionManager.Instance.DepositCore(ProgressionManager.CoreType.ChaosSeed);
					}
				}
			}
			if (!flag)
			{
				this.seedDepositsPending.Add(new ValueTuple<int, int, float, bool>(seedNetId, playerActorNumber, Time.time, true));
				ProgressionManager.Instance.DepositCore(ProgressionManager.CoreType.ChaosSeed);
			}
		}
	}

	// Token: 0x060033B1 RID: 13233 RVA: 0x0011B07C File Offset: 0x0011927C
	public bool ValidateSeedDepositSucceeded(int playerActorNumber, int entityNetId)
	{
		if (this.ghostReactor.grManager.IsAuthority())
		{
			bool flag = false;
			for (int i = 0; i < this.seedDepositsPending.Count; i++)
			{
				if (this.seedDepositsPending[i].Item1 == entityNetId && this.seedDepositsPending[i].Item2 == playerActorNumber)
				{
					flag = true;
				}
			}
			return flag;
		}
		return false;
	}

	// Token: 0x060033B2 RID: 13234 RVA: 0x0011B0E0 File Offset: 0x001192E0
	public void SeedDepositSucceeded(int playerActorNumber, int entityNetId)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		this.depositorParticles.Play();
		this.depositorAudioSource.PlayOneShot(this.seedDepositAudio, this.seedDepositVolume);
		this.RemovePendingSeedDeposit(entityNetId);
		if (playerActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			ProgressionManager.Instance.GetJuicerStatus();
		}
		if (!this.stationOpen && this.ghostReactor.grManager.IsAuthority())
		{
			this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorOpenStation, playerActorNumber, 0);
		}
	}

	// Token: 0x060033B3 RID: 13235 RVA: 0x0011B16D File Offset: 0x0011936D
	public void SeedDepositFailed(int playerActorNumber, int entityNetId)
	{
		this.depositorAudioSource.PlayOneShot(this.seedDepositFailedAudio, this.seedDepositFailedVolume);
		this.RemovePendingSeedDeposit(entityNetId);
	}

	// Token: 0x060033B4 RID: 13236 RVA: 0x0011B190 File Offset: 0x00119390
	private void RemovePendingSeedDeposit(int entityId)
	{
		for (int i = this.seedDepositsPending.Count - 1; i >= 0; i--)
		{
			if (this.seedDepositsPending[i].Item1 == entityId)
			{
				this.seedDepositsPending.RemoveAt(i);
			}
		}
	}

	// Token: 0x060033B5 RID: 13237 RVA: 0x0011B1D8 File Offset: 0x001193D8
	public void ApplyState(int playerActorNumber, int coreCount, int coresProcessedByOverdrive, int researchPoints, float coreProcessingPercentage, float overdriveSupply)
	{
		if (playerActorNumber == this.currentPlayerActorNumber)
		{
			if (this.currentPlayerData.actorNumber != playerActorNumber)
			{
				this.currentPlayerData = default(GRSeedExtractor.PlayerData);
			}
			coreCount = Mathf.Clamp(coreCount, 0, this.maxVisualChaosSeedCount);
			coresProcessedByOverdrive = Mathf.Clamp(coresProcessedByOverdrive, 0, this.MAX_OVERDRIVE_USES);
			coreProcessingPercentage = Mathf.Clamp(coreProcessingPercentage, 0f, 1f);
			overdriveSupply = Mathf.Clamp(overdriveSupply, 0f, 1f);
			bool flag = overdriveSupply < this.currentPlayerData.overdriveSupply;
			bool flag2 = this.currentPlayerData.overdriveSupply == 0f && this.currentPlayerData.coreCount > coreCount;
			if (playerActorNumber != NetworkSystem.Instance.LocalPlayer.ActorNumber && coresProcessedByOverdrive > 0 && (flag || flag2))
			{
				this.currentPlayerData.coresPendingOverdriveProcessing = this.currentPlayerData.coresPendingOverdriveProcessing + coresProcessedByOverdrive;
			}
			this.currentPlayerData.actorNumber = playerActorNumber;
			this.currentPlayerData.coreCount = coreCount;
			this.currentPlayerData.coresProcessedByOverdrive = coresProcessedByOverdrive;
			this.currentPlayerData.coreProcessingPercentage = coreProcessingPercentage;
			this.currentPlayerData.overdriveSupply = overdriveSupply;
			this.currentPlayerData.latestRefreshTime = Time.time;
			this.currentPlayerData.researchPoints = researchPoints;
			this.OnStateUpdated();
		}
	}

	// Token: 0x060033B6 RID: 13238 RVA: 0x0011B310 File Offset: 0x00119510
	public void OpenStation(int playerActorNumber)
	{
		if (NetworkSystem.Instance.GetPlayer(playerActorNumber) == null)
		{
			return;
		}
		if (!this.stationOpen)
		{
			this.doorAudioSource.PlayOneShot(this.doorOpenAudio, this.doorOpenVolume);
		}
		base.enabled = true;
		this.currentPlayerActorNumber = playerActorNumber;
		this.stationOpen = true;
		this.stationOpenRequestTime = Time.time;
		this.UpdateOverdrivePurchaseButtons();
	}

	// Token: 0x060033B7 RID: 13239 RVA: 0x0011B370 File Offset: 0x00119570
	public void CloseStation()
	{
		if (this.stationOpen)
		{
			this.doorAudioSource.PlayOneShot(this.doorCloseAudio, this.doorCloseVolume);
		}
		this.currentPlayerActorNumber = -1;
		this.stationOpen = false;
		this.UpdateOverdrivePurchaseButtons();
	}

	// Token: 0x060033B8 RID: 13240 RVA: 0x0011B3A8 File Offset: 0x001195A8
	private void UpdateOverdrivePurchaseButtons()
	{
		if (!this.LocalPlayerCanPurchaseOverdrive())
		{
			this.overdrivePurchaseButton.myTmpText.text = "";
			this.overdrivePurchaseButton.buttonRenderer.material = this.defaultButtonMaterial;
			this.overdriveConfirmButton.myTmpText.text = "";
			this.overdriveConfirmButton.buttonRenderer.material = this.defaultButtonMaterial;
			return;
		}
		if (this.overdrivePurchasePending)
		{
			this.overdrivePurchaseButton.myTmpText.text = "CANCEL";
			this.overdrivePurchaseButton.buttonRenderer.material = this.redButtonMaterial;
			this.overdriveConfirmButton.myTmpText.text = "CONFIRM";
			this.overdriveConfirmButton.buttonRenderer.material = this.greenButtonMaterial;
			return;
		}
		this.overdrivePurchaseButton.myTmpText.text = "BUY";
		this.overdrivePurchaseButton.buttonRenderer.material = this.defaultButtonMaterial;
		this.overdriveConfirmButton.myTmpText.text = "";
		this.overdriveConfirmButton.buttonRenderer.material = this.defaultButtonMaterial;
	}

	// Token: 0x060033B9 RID: 13241 RVA: 0x0011B4CC File Offset: 0x001196CC
	public void OnStateUpdated()
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.currentPlayerActorNumber);
		if (player == null)
		{
			this.CloseStation();
		}
		this.UpdateOverdrivePurchaseButtons();
		if (this.stationOpen && player != null)
		{
			if (this.overdriveActive)
			{
				return;
			}
			if (this.currentPlayerData.coresPendingOverdriveProcessing > 0)
			{
				int coresPendingOverdriveProcessing = this.currentPlayerData.coresPendingOverdriveProcessing;
				this.currentPlayerData.coresPendingOverdriveProcessing = 0;
				if (this.StationOpenForLocalPlayer)
				{
					this.localPlayerData.coresPendingOverdriveProcessing = 0;
				}
				this.overdrivePurchaseAnimationRoutine = base.StartCoroutine(this.OverdrivePurchaseAnimationVisual(coresPendingOverdriveProcessing));
				return;
			}
			this.processingAmount = this.currentPlayerData.coreProcessingPercentage;
			this.overdriveAmount = this.currentPlayerData.overdriveSupply;
			int num = Mathf.Clamp(this.currentPlayerData.coreCount, 0, this.maxVisualChaosSeedCount) - this.seedProcessingStates.Count;
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					this.DepositSeedVisual();
				}
				return;
			}
			if (num < 0)
			{
				for (int j = 0; j > num; j--)
				{
					this.CompleteSeedVisual();
				}
				return;
			}
		}
		else
		{
			this.screenText.text = "Player Data Lookup Failed.";
			this.overdriveAmount = 0f;
			this.processingAmount = 0f;
		}
	}

	// Token: 0x060033BA RID: 13242 RVA: 0x0011B600 File Offset: 0x00119800
	private void DepositSeedVisual()
	{
		for (int i = 0; i < this.chaosSeedVisuals.Count; i++)
		{
			if (!this.chaosSeedVisuals[i].activeSelf)
			{
				GRSeedExtractor.SeedProcessingVisualState seedProcessingVisualState = new GRSeedExtractor.SeedProcessingVisualState
				{
					poolIndex = i,
					rollAngle = 0f,
					speed = 0f,
					rampProgress = 0f,
					dropProgress = 0f
				};
				this.seedProcessingStates.Add(seedProcessingVisualState);
				this.chaosSeedVisuals[i].SetActive(true);
				this.chaosSeedVisuals[i].transform.localPosition = this.seedTubeStart.localPosition;
				this.chaosSeedVisuals[i].transform.localRotation = Quaternion.identity;
				this.chaosSeedVisuals[i].transform.localScale = Vector3.one * this.seedVisualScaleRange.y;
				this.seedTubeAudioSource.PlayOneShot(this.seedMovementAudio, this.seedMovementVolume);
				return;
			}
		}
	}

	// Token: 0x060033BB RID: 13243 RVA: 0x0011B720 File Offset: 0x00119920
	private void CompleteSeedVisual()
	{
		if (this.seedProcessingStates.Count > 0)
		{
			GRSeedExtractor.SeedProcessingVisualState seedProcessingVisualState = this.seedProcessingStates[0];
			this.chaosSeedVisuals[seedProcessingVisualState.poolIndex].SetActive(false);
			this.seedProcessingStates.RemoveAt(0);
		}
	}

	// Token: 0x060033BC RID: 13244 RVA: 0x0011B76C File Offset: 0x0011996C
	private void ClearSeedVisuals()
	{
		int count = this.seedProcessingStates.Count;
		for (int i = 0; i < count; i++)
		{
			this.CompleteSeedVisual();
		}
	}

	// Token: 0x060033BD RID: 13245 RVA: 0x0011B798 File Offset: 0x00119998
	private void UpdateScreenDisplay()
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.currentPlayerActorNumber);
		if (player == null || !this.stationOpen)
		{
			return;
		}
		int num = (int)this.estimatedJuiceTimeRemaining;
		if (this.currentPlayerActorNumber != this.currentDisplayData.playerActorNumber || this.currentPlayerData.coreCount != this.currentDisplayData.coreCount || this.currentPlayerData.overdriveSupply != this.currentDisplayData.overdriveSupply || this.currentPlayerData.researchPoints != this.currentDisplayData.researchPoints || num != this.currentDisplayData.juiceSecondsLeft)
		{
			this.currentDisplayData.playerActorNumber = this.currentPlayerActorNumber;
			this.currentDisplayData.coreCount = this.currentPlayerData.coreCount;
			this.currentDisplayData.overdriveSupply = this.currentPlayerData.overdriveSupply;
			this.currentDisplayData.researchPoints = this.currentPlayerData.researchPoints;
			this.currentDisplayData.juiceSecondsLeft = num;
			this.UpdateScreenSB.Clear();
			this.UpdateScreenSB.Append(player.SanitizedNickName + "\n");
			this.UpdateScreenSB.Append(string.Format("JUICE: <color=purple>⑮ {0}</color>\n\n", this.currentDisplayData.researchPoints));
			if (this.currentDisplayData.coreCount > 0)
			{
				this.UpdateScreenSB.Append(string.Format("Processing {0} Seeds", this.currentDisplayData.coreCount));
				int num2 = this.currentDisplayData.juiceSecondsLeft % 3;
				if (num2 == 2)
				{
					this.UpdateScreenSB.Append(".");
				}
				else if (num2 == 1)
				{
					this.UpdateScreenSB.Append("..");
				}
				else
				{
					this.UpdateScreenSB.Append("...");
				}
				int num3 = num / 3600;
				int num4 = num / 60 % 60;
				int num5 = num % 60;
				if (num3 > 0)
				{
					this.UpdateScreenSB.Append(string.Format("\nNext <color=purple>⑮</color> in {0}:{1:00}:{2:00}\n", num3, num4, num5));
				}
				else
				{
					this.UpdateScreenSB.Append(string.Format("\nNext <color=purple>⑮</color> in {0}:{1:00}\n", num4, num5));
				}
			}
			else
			{
				this.UpdateScreenSB.Append("Deposit Chaos Seed\nFor Juice Processing\n");
			}
			this.screenText.text = this.UpdateScreenSB.ToString();
		}
	}

	// Token: 0x060033BE RID: 13246 RVA: 0x0011B9FC File Offset: 0x00119BFC
	private void StepSeedVisualAnimation(float dt)
	{
		float magnitude = (this.seedTubeStart.position - this.seedTubeEnd.position).magnitude;
		float num = magnitude / this.seedVisualRollTime;
		for (int i = 0; i < this.seedProcessingStates.Count; i++)
		{
			GRSeedExtractor.SeedProcessingVisualState seedProcessingVisualState = this.seedProcessingStates[i];
			float num2 = 2f;
			if (i > 0)
			{
				num2 = this.seedProcessingStates[i - 1].rampProgress - 2f * this.visualChaosSeedRadius / magnitude;
			}
			if (seedProcessingVisualState.rampProgress < 1f)
			{
				GameObject gameObject = this.chaosSeedVisuals[seedProcessingVisualState.poolIndex];
				seedProcessingVisualState.speed = Mathf.MoveTowards(seedProcessingVisualState.speed, num, num * dt);
				float num3 = seedProcessingVisualState.speed * dt;
				float num4 = num3 / magnitude;
				seedProcessingVisualState.rampProgress = Mathf.Clamp01(seedProcessingVisualState.rampProgress + num4);
				if (seedProcessingVisualState.rampProgress >= num2)
				{
					seedProcessingVisualState.rampProgress = num2;
					seedProcessingVisualState.speed = 0f;
					num3 = 0f;
				}
				gameObject.transform.localPosition = Vector3.Lerp(this.seedTubeStart.localPosition, this.seedTubeEnd.localPosition, seedProcessingVisualState.rampProgress);
				seedProcessingVisualState.rollAngle += num3 / this.visualChaosSeedRadius;
				gameObject.transform.localRotation = Quaternion.AngleAxis(seedProcessingVisualState.rollAngle * 57.29578f, Vector3.forward);
			}
			if (i == 0 && seedProcessingVisualState.rampProgress >= 1f)
			{
				GameObject gameObject2 = this.chaosSeedVisuals[seedProcessingVisualState.poolIndex];
				if (seedProcessingVisualState.dropProgress < 1f)
				{
					seedProcessingVisualState.dropProgress += 1f / this.seedVisualDropTime * dt;
					seedProcessingVisualState.rampProgress = 1f + seedProcessingVisualState.dropProgress;
					float num5 = this.tubeEndToProcessingPathY.Evaluate(seedProcessingVisualState.dropProgress);
					float num6 = this.tubeEndToProcessingPathX.Evaluate(seedProcessingVisualState.dropProgress);
					Vector3 localPosition = gameObject2.transform.localPosition;
					localPosition.y = Mathf.Lerp(this.seedTubeEnd.localPosition.y, this.seedProcessingPosition.localPosition.y, num5);
					localPosition.x = Mathf.Lerp(this.seedTubeEnd.localPosition.x, this.seedProcessingPosition.localPosition.x, num6);
					gameObject2.transform.localPosition = localPosition;
					float num7 = seedProcessingVisualState.speed * dt;
					seedProcessingVisualState.rollAngle += num7 / this.visualChaosSeedRadius;
					gameObject2.transform.localRotation = Quaternion.AngleAxis(seedProcessingVisualState.rollAngle * 57.29578f, Vector3.forward);
					if (seedProcessingVisualState.dropProgress >= 1f)
					{
						this.juicerAudioSource.PlayOneShot(this.seedDropAudio, this.seedDropVolume);
					}
				}
				if (seedProcessingVisualState.dropProgress >= 1f && !this.drainingProcessingBeaker)
				{
					gameObject2.transform.localScale = Vector3.one * Mathf.Lerp(this.seedVisualScaleRange.y, this.seedVisualScaleRange.x, this.processingAmountVisual);
				}
			}
			this.seedProcessingStates[i] = seedProcessingVisualState;
		}
	}

	// Token: 0x060033BF RID: 13247 RVA: 0x0011BD47 File Offset: 0x00119F47
	private IEnumerator OverdrivePurchaseAnimationVisual(int coresToProcess)
	{
		this.overdriveActive = true;
		this.overdriveBeepAudioSource.loop = true;
		this.overdriveBeepAudioSource.volume = this.overdriveBeepingVolume;
		this.overdriveBeepAudioSource.clip = this.overdriveBeepingAudio;
		this.overdriveBeepAudioSource.Play();
		int num = Math.Min(coresToProcess + this.currentPlayerData.coreCount, this.maxVisualChaosSeedCount);
		while (this.seedProcessingStates.Count < num)
		{
			this.DepositSeedVisual();
		}
		for (int j = 0; j < this.disableDuringOverdrive.Count; j++)
		{
			this.disableDuringOverdrive[j].gameObject.SetActive(false);
		}
		for (int k = 0; k < this.enableDuringOverdrive.Count; k++)
		{
			this.enableDuringOverdrive[k].gameObject.SetActive(true);
		}
		this.overdriveMeterAudioSource.PlayOneShot(this.overdriveFillAudio, this.overdriveFillVolume);
		float overdriveFillRate = 1f / this.overdriveFillTime;
		float maxOverdriveFill = Mathf.Clamp01(this.currentPlayerData.overdriveSupply + (float)coresToProcess / (float)this.MAX_OVERDRIVE_USES);
		while (this.overdriveAmount < maxOverdriveFill)
		{
			this.overdriveAmount = Mathf.MoveTowards(this.overdriveAmount, maxOverdriveFill, overdriveFillRate * Time.deltaTime);
			yield return null;
		}
		this.overdriveMeterAudioSource.Stop();
		int num6;
		for (int i = 0; i < coresToProcess; i = num6)
		{
			float waitForSeedDepositStartTime = Time.time;
			bool flag = this.seedProcessingStates.Count > 0 && this.seedProcessingStates[0].dropProgress >= 1f;
			while (!flag && Time.time - waitForSeedDepositStartTime < 3f)
			{
				yield return null;
				flag = this.seedProcessingStates.Count > 0 && this.seedProcessingStates[0].dropProgress >= 1f;
			}
			this.juicerAudioSource.PlayOneShot(this.seedJuicingAudio, this.seedJuicingVolume);
			this.juicerOverdriveParticles.gameObject.SetActive(true);
			float num2 = Mathf.Clamp01(1f - this.processingAmount);
			float timeToProcess = num2 * this.overdriveProcessTime;
			float startingProcessingAmount = this.processingAmount;
			float num3 = num2 / (float)this.MAX_OVERDRIVE_USES;
			float startingOverdrive = this.overdriveAmount;
			float resultingOverdrive = Mathf.Clamp01(this.overdriveAmount - num3);
			float timeProcessing = 0f;
			while (timeProcessing < timeToProcess)
			{
				timeProcessing += Time.deltaTime;
				float num4 = timeProcessing / timeToProcess;
				this.overdriveAmount = Mathf.Lerp(startingOverdrive, resultingOverdrive, num4);
				this.processingAmount = Mathf.Lerp(startingProcessingAmount, 1f, num4);
				this.estimatedJuiceTimeRemaining = timeToProcess - timeProcessing;
				yield return null;
			}
			this.CompleteSeedVisual();
			this.juicerOverdriveParticles.gameObject.SetActive(false);
			this.drainingProcessingBeaker = true;
			float timeDepositing = 0f;
			while (timeDepositing < this.juiceDepositTime)
			{
				timeDepositing += Time.deltaTime;
				float num5 = timeDepositing / this.juiceDepositTime;
				this.processingAmount = Mathf.Lerp(1f, 0f, num5);
				yield return null;
			}
			this.drainingProcessingBeaker = false;
			num6 = i + 1;
		}
		if (this.currentPlayerData.coresPendingOverdriveProcessing == 0 && this.currentPlayerData.coreCount == 1)
		{
			if (this.seedProcessingStates.Count == 0)
			{
				this.DepositSeedVisual();
			}
			float timeDepositing = Time.time;
			bool flag2 = this.seedProcessingStates.Count > 0 && this.seedProcessingStates[0].dropProgress >= 1f;
			while (!flag2 && Time.time - timeDepositing < 3f)
			{
				yield return null;
				flag2 = this.seedProcessingStates.Count > 0 && this.seedProcessingStates[0].dropProgress >= 1f;
			}
			float timeProcessing = 0f;
			float resultingOverdrive = this.processingAmount;
			float startingOverdrive = this.overdriveAmount;
			float startingProcessingAmount = Mathf.Clamp01(this.currentPlayerData.coreProcessingPercentage - resultingOverdrive) * this.overdriveProcessTime;
			while (timeProcessing < startingProcessingAmount)
			{
				timeProcessing += Time.deltaTime;
				float num7 = timeProcessing / startingProcessingAmount;
				this.processingAmount = Mathf.Clamp01(Mathf.Lerp(resultingOverdrive, this.currentPlayerData.coreProcessingPercentage, num7));
				this.overdriveAmount = Mathf.Clamp01(Mathf.Lerp(startingOverdrive, this.currentPlayerData.overdriveSupply, num7));
				yield return null;
			}
		}
		for (int l = 0; l < this.disableDuringOverdrive.Count; l++)
		{
			this.disableDuringOverdrive[l].gameObject.SetActive(true);
		}
		for (int m = 0; m < this.enableDuringOverdrive.Count; m++)
		{
			this.enableDuringOverdrive[m].gameObject.SetActive(false);
		}
		this.overdriveLightSpinnerOff.localRotation = this.overdriveLightSpinnerOn.localRotation;
		this.overdriveBeepAudioSource.Stop();
		this.overdriveActive = false;
		if (this.StationOpenForLocalPlayer)
		{
			ProgressionManager.Instance.GetJuicerStatus();
		}
		this.OnStateUpdated();
		yield break;
	}

	// Token: 0x060033C0 RID: 13248 RVA: 0x0011BD60 File Offset: 0x00119F60
	public void OnResearchPointsUpdated()
	{
		int numberOfResearchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
		if (numberOfResearchPoints > this.localPlayerData.researchPoints)
		{
			GRPlayer.GetLocal().SendJuiceCollectedTelemetry(numberOfResearchPoints - this.localPlayerData.researchPoints, this.localPlayerData.coresProcessedByOverdrive);
		}
		this.localPlayerData.researchPoints = numberOfResearchPoints;
		if (this.StationOpenForLocalPlayer)
		{
			bool flag = this.currentPlayerData.researchPoints != this.localPlayerData.researchPoints;
			this.currentPlayerData.researchPoints = this.localPlayerData.researchPoints;
			if (flag)
			{
				this.ghostReactor.grManager.RequestApplySeedExtractorState(this.localPlayerData.coreCount, this.localPlayerData.coresProcessedByOverdrive, this.localPlayerData.researchPoints, this.localPlayerData.coreProcessingPercentage, this.localPlayerData.overdriveSupply);
				this.OnStateUpdated();
			}
		}
	}

	// Token: 0x060033C1 RID: 13249 RVA: 0x0011BE40 File Offset: 0x0011A040
	public void OnPurchaseOverdrive(bool success)
	{
		this.overdriveServerConfirmationPending = false;
		if (!success)
		{
			return;
		}
		GRPlayer.GetLocal().SendOverdrivePurchasedTelemetry(250, this.localPlayerData.coreCount);
	}

	// Token: 0x040042F1 RID: 17137
	private float PROCESSING_TIME_SECONDS = 600f;

	// Token: 0x040042F2 RID: 17138
	private int MAX_OVERDRIVE_USES = 6;

	// Token: 0x040042F3 RID: 17139
	[SerializeField]
	private GTZone zone;

	// Token: 0x040042F4 RID: 17140
	[SerializeField]
	private TriggerEventNotifier triggerNotifier;

	// Token: 0x040042F5 RID: 17141
	[SerializeField]
	private TriggerEventNotifier coreDepositTriggerNotifier;

	// Token: 0x040042F6 RID: 17142
	[SerializeField]
	private TMP_Text screenText;

	// Token: 0x040042F7 RID: 17143
	[SerializeField]
	private IDCardScanner idCardScanner;

	// Token: 0x040042F8 RID: 17144
	[SerializeField]
	private GameObject chaosSeedVisualPrefab;

	// Token: 0x040042F9 RID: 17145
	[Header("Overdrive Purchase Buttons")]
	[SerializeField]
	private GorillaPressableButton overdrivePurchaseButton;

	// Token: 0x040042FA RID: 17146
	[SerializeField]
	private GorillaPressableButton overdriveConfirmButton;

	// Token: 0x040042FB RID: 17147
	[SerializeField]
	private Material defaultButtonMaterial;

	// Token: 0x040042FC RID: 17148
	[SerializeField]
	private Material redButtonMaterial;

	// Token: 0x040042FD RID: 17149
	[SerializeField]
	private Material greenButtonMaterial;

	// Token: 0x040042FE RID: 17150
	[Header("Shutter Door Visual")]
	[SerializeField]
	private Transform shutterDoorParent;

	// Token: 0x040042FF RID: 17151
	[SerializeField]
	private Vector2 shutterDoorLiftRange = new Vector2(1.245f, 2.07f);

	// Token: 0x04004300 RID: 17152
	[SerializeField]
	private float shutterDoorAnimTime;

	// Token: 0x04004301 RID: 17153
	[Header("Seed Processing Visual")]
	[SerializeField]
	private Transform processingLiquidScaleParent;

	// Token: 0x04004302 RID: 17154
	[SerializeField]
	[Range(0f, 1f)]
	public float processingAmount;

	// Token: 0x04004303 RID: 17155
	private float processingAmountVisual;

	// Token: 0x04004304 RID: 17156
	[SerializeField]
	private Transform seedTubeStart;

	// Token: 0x04004305 RID: 17157
	[SerializeField]
	private Transform seedTubeEnd;

	// Token: 0x04004306 RID: 17158
	[SerializeField]
	private Transform seedProcessingPosition;

	// Token: 0x04004307 RID: 17159
	[SerializeField]
	private AnimationCurve tubeEndToProcessingPathY = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04004308 RID: 17160
	[SerializeField]
	private AnimationCurve tubeEndToProcessingPathX = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04004309 RID: 17161
	[SerializeField]
	private float visualChaosSeedRadius = 1f;

	// Token: 0x0400430A RID: 17162
	[SerializeField]
	private int maxVisualChaosSeedCount = 6;

	// Token: 0x0400430B RID: 17163
	[SerializeField]
	private float seedVisualRollTime = 2f;

	// Token: 0x0400430C RID: 17164
	[SerializeField]
	private float seedVisualDropTime = 0.5f;

	// Token: 0x0400430D RID: 17165
	[SerializeField]
	private Vector2 seedVisualScaleRange = new Vector2(0.1f, 1.25f);

	// Token: 0x0400430E RID: 17166
	[Header("Overdrive Visual")]
	[SerializeField]
	private Transform overdriveLiquidScaleParent;

	// Token: 0x0400430F RID: 17167
	[SerializeField]
	private Transform overdriveLightSpinnerOff;

	// Token: 0x04004310 RID: 17168
	[SerializeField]
	private Transform overdriveLightSpinnerOn;

	// Token: 0x04004311 RID: 17169
	[SerializeField]
	private List<Transform> enableDuringOverdrive = new List<Transform>();

	// Token: 0x04004312 RID: 17170
	[SerializeField]
	private List<Transform> disableDuringOverdrive = new List<Transform>();

	// Token: 0x04004313 RID: 17171
	[SerializeField]
	private float overdriveLightSpinRate = 1f;

	// Token: 0x04004314 RID: 17172
	[SerializeField]
	[Range(0f, 1f)]
	public float overdriveAmount;

	// Token: 0x04004315 RID: 17173
	private float overdriveAmountVisual;

	// Token: 0x04004316 RID: 17174
	[Header("VFX")]
	[SerializeField]
	private ParticleSystem depositorParticles;

	// Token: 0x04004317 RID: 17175
	[SerializeField]
	private ParticleSystem juicerSlowParticles;

	// Token: 0x04004318 RID: 17176
	[SerializeField]
	private ParticleSystem juicerOverdriveParticles;

	// Token: 0x04004319 RID: 17177
	[Header("Audio")]
	[SerializeField]
	private AudioSource depositorAudioSource;

	// Token: 0x0400431A RID: 17178
	[SerializeField]
	private AudioSource doorAudioSource;

	// Token: 0x0400431B RID: 17179
	[SerializeField]
	private AudioSource seedTubeAudioSource;

	// Token: 0x0400431C RID: 17180
	[SerializeField]
	private AudioSource juicerAudioSource;

	// Token: 0x0400431D RID: 17181
	[SerializeField]
	private AudioSource machineHumAudioSource;

	// Token: 0x0400431E RID: 17182
	[SerializeField]
	private AudioSource overdriveMeterAudioSource;

	// Token: 0x0400431F RID: 17183
	[SerializeField]
	private AudioSource overdriveBeepAudioSource;

	// Token: 0x04004320 RID: 17184
	[SerializeField]
	private AudioClip seedDepositAudio;

	// Token: 0x04004321 RID: 17185
	[SerializeField]
	private float seedDepositVolume = 0.5f;

	// Token: 0x04004322 RID: 17186
	[SerializeField]
	private AudioClip seedDepositFailedAudio;

	// Token: 0x04004323 RID: 17187
	[SerializeField]
	private float seedDepositFailedVolume = 0.5f;

	// Token: 0x04004324 RID: 17188
	[SerializeField]
	private AudioClip seedDepositAttemptAudio;

	// Token: 0x04004325 RID: 17189
	[SerializeField]
	private float seedDepositAttemptVolume = 0.5f;

	// Token: 0x04004326 RID: 17190
	[SerializeField]
	private AudioClip seedMovementAudio;

	// Token: 0x04004327 RID: 17191
	[SerializeField]
	private float seedMovementVolume = 0.5f;

	// Token: 0x04004328 RID: 17192
	[SerializeField]
	private AudioClip seedDropAudio;

	// Token: 0x04004329 RID: 17193
	[SerializeField]
	private float seedDropVolume = 0.5f;

	// Token: 0x0400432A RID: 17194
	[SerializeField]
	private AudioClip seedJuicingAudio;

	// Token: 0x0400432B RID: 17195
	[SerializeField]
	private float seedJuicingVolume = 0.5f;

	// Token: 0x0400432C RID: 17196
	[SerializeField]
	private AudioClip doorOpenAudio;

	// Token: 0x0400432D RID: 17197
	[SerializeField]
	private float doorOpenVolume = 0.5f;

	// Token: 0x0400432E RID: 17198
	[SerializeField]
	private AudioClip doorCloseAudio;

	// Token: 0x0400432F RID: 17199
	[SerializeField]
	private float doorCloseVolume = 0.5f;

	// Token: 0x04004330 RID: 17200
	[SerializeField]
	private AudioClip processingHumAudio;

	// Token: 0x04004331 RID: 17201
	[SerializeField]
	private float processingHumVolume = 0.5f;

	// Token: 0x04004332 RID: 17202
	[SerializeField]
	private AudioClip overdriveFillAudio;

	// Token: 0x04004333 RID: 17203
	[SerializeField]
	private float overdriveFillVolume = 0.5f;

	// Token: 0x04004334 RID: 17204
	[SerializeField]
	private AudioClip overdriveEngineAudio;

	// Token: 0x04004335 RID: 17205
	[SerializeField]
	private float overdriveEngineVolume = 0.5f;

	// Token: 0x04004336 RID: 17206
	[SerializeField]
	private AudioClip overdriveBeepingAudio;

	// Token: 0x04004337 RID: 17207
	[SerializeField]
	private float overdriveBeepingVolume = 0.5f;

	// Token: 0x04004338 RID: 17208
	private GRSeedExtractor.PlayerData localPlayerData;

	// Token: 0x04004339 RID: 17209
	private GRSeedExtractor.PlayerData currentPlayerData;

	// Token: 0x0400433A RID: 17210
	private GRSeedExtractor.ScreenDisplayData currentDisplayData;

	// Token: 0x0400433B RID: 17211
	private bool stationOpen;

	// Token: 0x0400433C RID: 17212
	private float stationOpenRequestTime;

	// Token: 0x0400433D RID: 17213
	private int currentPlayerActorNumber = -1;

	// Token: 0x0400433E RID: 17214
	private float shutterDoorOpenAmount;

	// Token: 0x0400433F RID: 17215
	private List<GameObject> chaosSeedVisuals = new List<GameObject>();

	// Token: 0x04004340 RID: 17216
	private bool overdrivePurchasePending;

	// Token: 0x04004341 RID: 17217
	private bool overdriveServerConfirmationPending;

	// Token: 0x04004342 RID: 17218
	private float overdrivePurchaseTime;

	// Token: 0x04004343 RID: 17219
	private bool overdriveActive;

	// Token: 0x04004344 RID: 17220
	private bool drainingProcessingBeaker;

	// Token: 0x04004345 RID: 17221
	private float estimatedJuiceTimeRemaining;

	// Token: 0x04004346 RID: 17222
	private float processingLiquidFollowRate = Mathf.Exp(2f);

	// Token: 0x04004347 RID: 17223
	private List<ValueTuple<int, int, float, bool>> seedDepositsPending = new List<ValueTuple<int, int, float, bool>>();

	// Token: 0x04004348 RID: 17224
	private Coroutine overdrivePurchaseAnimationRoutine;

	// Token: 0x04004349 RID: 17225
	private List<GRSeedExtractor.SeedProcessingVisualState> seedProcessingStates = new List<GRSeedExtractor.SeedProcessingVisualState>();

	// Token: 0x0400434A RID: 17226
	private float timeBetweenServerRequests = 3f;

	// Token: 0x0400434B RID: 17227
	private float lastServerRequestTime;

	// Token: 0x0400434C RID: 17228
	private GhostReactor ghostReactor;

	// Token: 0x0400434D RID: 17229
	private GRToolProgressionManager toolProgressionManager;

	// Token: 0x0400434E RID: 17230
	private StringBuilder UpdateScreenSB = new StringBuilder(256);

	// Token: 0x0400434F RID: 17231
	[Header("Debug Animation")]
	public int debugSeedCount;

	// Token: 0x04004350 RID: 17232
	public float debugSeedProcessingTime = 10f;

	// Token: 0x04004351 RID: 17233
	public float overdriveFillTime = 2f;

	// Token: 0x04004352 RID: 17234
	public float overdriveProcessTime = 1.5f;

	// Token: 0x04004353 RID: 17235
	public float juiceDepositTime = 0.75f;

	// Token: 0x020007E8 RID: 2024
	public struct PlayerData
	{
		// Token: 0x04004354 RID: 17236
		public int actorNumber;

		// Token: 0x04004355 RID: 17237
		public int coreCount;

		// Token: 0x04004356 RID: 17238
		public float coreProcessingPercentage;

		// Token: 0x04004357 RID: 17239
		public float overdriveSupply;

		// Token: 0x04004358 RID: 17240
		public int coresProcessedByOverdrive;

		// Token: 0x04004359 RID: 17241
		public int coresPendingOverdriveProcessing;

		// Token: 0x0400435A RID: 17242
		public int researchPoints;

		// Token: 0x0400435B RID: 17243
		public float latestRefreshTime;
	}

	// Token: 0x020007E9 RID: 2025
	private struct ScreenDisplayData
	{
		// Token: 0x0400435C RID: 17244
		public int playerActorNumber;

		// Token: 0x0400435D RID: 17245
		public int coreCount;

		// Token: 0x0400435E RID: 17246
		public float overdriveSupply;

		// Token: 0x0400435F RID: 17247
		public int researchPoints;

		// Token: 0x04004360 RID: 17248
		public int juiceSecondsLeft;
	}

	// Token: 0x020007EA RID: 2026
	private struct SeedProcessingVisualState
	{
		// Token: 0x04004361 RID: 17249
		public int poolIndex;

		// Token: 0x04004362 RID: 17250
		public float speed;

		// Token: 0x04004363 RID: 17251
		public float rollAngle;

		// Token: 0x04004364 RID: 17252
		public float rampProgress;

		// Token: 0x04004365 RID: 17253
		public float dropProgress;
	}
}
