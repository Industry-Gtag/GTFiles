using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000812 RID: 2066
[RequireComponent(typeof(GameEntity))]
public class GRToolLantern : MonoBehaviour, IGRSummoningEntity
{
	// Token: 0x060034D8 RID: 13528 RVA: 0x00122304 File Offset: 0x00120504
	private void Awake()
	{
		this.trackedEntities = new List<int>();
		this.state = GRToolLantern.State.Off;
		this.gameEntity.OnStateChanged += this.OnStateChanged;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.OnReleased));
		if (this.tool != null)
		{
			this.tool.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(this.tool);
		}
	}

	// Token: 0x060034D9 RID: 13529 RVA: 0x001223B9 File Offset: 0x001205B9
	private void OnEnable()
	{
		this.TurnOff();
		this.state = GRToolLantern.State.Off;
	}

	// Token: 0x060034DA RID: 13530 RVA: 0x001223C8 File Offset: 0x001205C8
	private void OnDestroy()
	{
		if (this.providingXRay && this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			this.DisableXRay();
		}
	}

	// Token: 0x060034DB RID: 13531 RVA: 0x001223E8 File Offset: 0x001205E8
	private void OnToolUpgraded(GRTool tool)
	{
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity1))
		{
			this.turnOnSound = this.upgrade1TurnOnSound;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity2))
		{
			this.turnOnSound = this.upgrade2TurnOnSound;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			this.turnOnSound = this.upgrade3TurnOnSound;
		}
	}

	// Token: 0x060034DC RID: 13532 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnGrabbed()
	{
	}

	// Token: 0x060034DD RID: 13533 RVA: 0x00122439 File Offset: 0x00120639
	public void OnReleased()
	{
		if (this.WasLastHeldLocal())
		{
			this.DisableXRay();
		}
	}

	// Token: 0x060034DE RID: 13534 RVA: 0x00122449 File Offset: 0x00120649
	private void EnableXRay()
	{
		if (!this.providingXRay && this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			GRPlayer.GetLocal().xRayVisionRefCount++;
			this.providingXRay = true;
		}
	}

	// Token: 0x060034DF RID: 13535 RVA: 0x0012247B File Offset: 0x0012067B
	private void DisableXRay()
	{
		if (this.providingXRay && this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			GRPlayer.GetLocal().xRayVisionRefCount--;
			this.providingXRay = false;
		}
	}

	// Token: 0x060034E0 RID: 13536 RVA: 0x001224B0 File Offset: 0x001206B0
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeldLocal() || this.tool.energy > 0)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x060034E1 RID: 13537 RVA: 0x001224E8 File Offset: 0x001206E8
	private void OnUpdateAuthority(float dt)
	{
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			bool flag = this.IsHeld();
			this.EnableLights(flag);
		}
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity2))
		{
			this.SetState(GRToolLantern.State.On);
			if (Time.timeAsDouble > this.lastFlareDropTime + this.minFlareDropInterval && this.IsButtonHeld() && this.tool.HasEnoughEnergy() && this.trackedEntities.Count < this.maxSpawnedFlares && this.lanternFlarePrefab != null)
			{
				if (this.gameEntity.IsAuthority())
				{
					Vector3 vector = base.transform.rotation * this.flareSpawnoffset;
					this.gameEntity.manager.RequestCreateItem(this.lanternFlarePrefab.name.GetStaticHash(), base.transform.position + vector, base.transform.rotation * Quaternion.Euler(10f, 0f, 10f), (long)this.gameEntity.GetNetId());
				}
				this.lastFlareDropTime = Time.timeAsDouble;
				this.tool.UseEnergy();
				this.audioSource.PlayOneShot(this.turnOnSound, this.turnOnSoundVolume);
				return;
			}
		}
		else
		{
			GRToolLantern.State state = this.state;
			if (state != GRToolLantern.State.Off)
			{
				if (state != GRToolLantern.State.On)
				{
					return;
				}
				this.timeOnSpentEnergy -= dt;
				if ((!this.IsButtonHeld() && this.timeOnSpentEnergy <= 0f) || this.tool.energy <= 0)
				{
					this.SetState(GRToolLantern.State.Off);
					this.gameEntity.RequestState(this.gameEntity.id, 0L);
					return;
				}
				if (this.IsButtonHeld() && this.timeOnSpentEnergy <= 0f)
				{
					this.TryConsumeEnergy();
				}
			}
			else if (this.IsButtonHeld() && this.tool.HasEnoughEnergy())
			{
				this.SetState(GRToolLantern.State.On);
				this.gameEntity.RequestState(this.gameEntity.id, 1L);
				return;
			}
		}
	}

	// Token: 0x060034E2 RID: 13538 RVA: 0x001226F0 File Offset: 0x001208F0
	private void TryConsumeEnergy()
	{
		if (this.tool.HasEnoughEnergy())
		{
			this.tool.UseEnergy();
			this.timeOnSpentEnergy = this.timeOnPerEnergyUseDurationSeconds * 10f * (float)this.tool.GetEnergyUseCost() / (float)this.tool.GetEnergyMax();
		}
	}

	// Token: 0x060034E3 RID: 13539 RVA: 0x00122744 File Offset: 0x00120944
	private void OnUpdateRemote(float dt)
	{
		GRToolLantern.State state = (GRToolLantern.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x060034E4 RID: 13540 RVA: 0x00122770 File Offset: 0x00120970
	private void SetState(GRToolLantern.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		if (!this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		GRToolLantern.State state = this.state;
		if (state != GRToolLantern.State.Off)
		{
			if (state == GRToolLantern.State.On)
			{
				this.TurnOn();
				return;
			}
		}
		else
		{
			this.TurnOff();
		}
	}

	// Token: 0x060034E5 RID: 13541 RVA: 0x001227B4 File Offset: 0x001209B4
	private void TurnOn()
	{
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			this.EnableXRay();
		}
		else
		{
			this.EnableLights(true);
		}
		this.audioSource.PlayOneShot(this.turnOnSound, this.turnOnSoundVolume);
		this.onHaptic.PlayIfHeldLocal(this.gameEntity);
		this.timeLastTurnedOn = Time.time;
	}

	// Token: 0x060034E6 RID: 13542 RVA: 0x00122814 File Offset: 0x00120A14
	private void EnableLights(bool isOn)
	{
		if (this.gameLight.gameObject.activeSelf == isOn)
		{
			return;
		}
		if (this.attributes.HasBeenInitialized())
		{
			this.gameLight.light.intensity = (float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.LightIntensity);
		}
		this.gameLight.gameObject.SetActive(isOn);
		for (int i = 0; i < this.meshAndMaterials.Count; i++)
		{
			MaterialUtils.SwapMaterial(this.meshAndMaterials[i], !isOn);
		}
	}

	// Token: 0x060034E7 RID: 13543 RVA: 0x0012289B File Offset: 0x00120A9B
	private void TurnOff()
	{
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			this.DisableXRay();
			return;
		}
		this.EnableLights(false);
	}

	// Token: 0x060034E8 RID: 13544 RVA: 0x001228BA File Offset: 0x00120ABA
	private bool IsHeld()
	{
		return this.gameEntity.IsHeld();
	}

	// Token: 0x060034E9 RID: 13545 RVA: 0x001228C7 File Offset: 0x00120AC7
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x060034EA RID: 13546 RVA: 0x001228E0 File Offset: 0x00120AE0
	private bool WasLastHeldLocal()
	{
		return this.gameEntity.lastHeldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x060034EB RID: 13547 RVA: 0x001228FC File Offset: 0x00120AFC
	private bool IsButtonHeld()
	{
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		if (num == -1)
		{
			return false;
		}
		if (!GamePlayer.IsLeftHand(num))
		{
			return gamePlayer.rig.rightIndex.calcT > 0.25f;
		}
		return gamePlayer.rig.leftIndex.calcT > 0.25f;
	}

	// Token: 0x060034EC RID: 13548 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnStateChanged(long prevState, long nextState)
	{
	}

	// Token: 0x060034ED RID: 13549 RVA: 0x00122970 File Offset: 0x00120B70
	public bool CanChangeState(long newStateIndex)
	{
		if (newStateIndex < 0L || newStateIndex >= 2L)
		{
			return false;
		}
		GRToolLantern.State state = (GRToolLantern.State)newStateIndex;
		if (state != GRToolLantern.State.Off)
		{
			return state == GRToolLantern.State.On && this.tool.energy > 0;
		}
		return Time.time > this.timeLastTurnedOn + this.minOnDuration || this.tool.energy <= 0;
	}

	// Token: 0x060034EE RID: 13550 RVA: 0x001229CC File Offset: 0x00120BCC
	public void AddTrackedEntity(GameEntity entityToTrack)
	{
		int netId = entityToTrack.GetNetId();
		this.trackedEntities.AddIfNew(netId);
	}

	// Token: 0x060034EF RID: 13551 RVA: 0x001229EC File Offset: 0x00120BEC
	public void RemoveTrackedEntity(GameEntity entityToRemove)
	{
		int netId = entityToRemove.GetNetId();
		if (this.trackedEntities.Contains(netId))
		{
			this.trackedEntities.Remove(netId);
		}
	}

	// Token: 0x060034F0 RID: 13552 RVA: 0x00122A1B File Offset: 0x00120C1B
	public void OnSummonedEntityInit(GameEntity entity)
	{
		this.AddTrackedEntity(entity);
	}

	// Token: 0x060034F1 RID: 13553 RVA: 0x00122A24 File Offset: 0x00120C24
	public void OnSummonedEntityDestroy(GameEntity entity)
	{
		this.RemoveTrackedEntity(entity);
	}

	// Token: 0x040044E0 RID: 17632
	public GameEntity gameEntity;

	// Token: 0x040044E1 RID: 17633
	public GRTool tool;

	// Token: 0x040044E2 RID: 17634
	public GameLight gameLight;

	// Token: 0x040044E3 RID: 17635
	public GRAttributes attributes;

	// Token: 0x040044E4 RID: 17636
	[SerializeField]
	private float timeOnPerEnergyUseDurationSeconds = 2f;

	// Token: 0x040044E5 RID: 17637
	[SerializeField]
	private int minEnergyPerUse = 1;

	// Token: 0x040044E6 RID: 17638
	[SerializeField]
	private float turnOnSoundVolume;

	// Token: 0x040044E7 RID: 17639
	[SerializeField]
	private AudioClip turnOnSound;

	// Token: 0x040044E8 RID: 17640
	[SerializeField]
	private AudioClip upgrade1TurnOnSound;

	// Token: 0x040044E9 RID: 17641
	[SerializeField]
	private AudioClip upgrade2TurnOnSound;

	// Token: 0x040044EA RID: 17642
	[SerializeField]
	private AudioClip upgrade3TurnOnSound;

	// Token: 0x040044EB RID: 17643
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040044EC RID: 17644
	public List<MeshAndMaterials> meshAndMaterials;

	// Token: 0x040044ED RID: 17645
	[Header("Haptic")]
	public AbilityHaptic onHaptic;

	// Token: 0x040044EE RID: 17646
	private float timeOnSpentEnergy;

	// Token: 0x040044EF RID: 17647
	private float timeLastTurnedOn;

	// Token: 0x040044F0 RID: 17648
	private float minOnDuration = 0.5f;

	// Token: 0x040044F1 RID: 17649
	private GRToolLantern.State state;

	// Token: 0x040044F2 RID: 17650
	private List<int> trackedEntities;

	// Token: 0x040044F3 RID: 17651
	private double lastFlareDropTime;

	// Token: 0x040044F4 RID: 17652
	public double minFlareDropInterval = 1.0;

	// Token: 0x040044F5 RID: 17653
	public GameEntity lanternFlarePrefab;

	// Token: 0x040044F6 RID: 17654
	public int maxSpawnedFlares = 10;

	// Token: 0x040044F7 RID: 17655
	private bool providingXRay;

	// Token: 0x040044F8 RID: 17656
	public Vector3 flareSpawnoffset = Vector3.zero;

	// Token: 0x02000813 RID: 2067
	private enum State
	{
		// Token: 0x040044FA RID: 17658
		Off,
		// Token: 0x040044FB RID: 17659
		On,
		// Token: 0x040044FC RID: 17660
		Count
	}
}
