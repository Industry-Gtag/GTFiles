using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000875 RID: 2165
public class GorillaGuardianZoneManager : MonoBehaviourPunCallbacks, IPunObservable, IGorillaSliceableSimple
{
	// Token: 0x17000503 RID: 1283
	// (get) Token: 0x06003858 RID: 14424 RVA: 0x001332B2 File Offset: 0x001314B2
	public NetPlayer CurrentGuardian
	{
		get
		{
			return this.guardianPlayer;
		}
	}

	// Token: 0x06003859 RID: 14425 RVA: 0x001332BC File Offset: 0x001314BC
	public void Awake()
	{
		GorillaGuardianZoneManager.zoneManagers.Add(this);
		this.idol.gameObject.SetActive(false);
		foreach (Transform transform in this.idolPositions)
		{
			transform.gameObject.SetActive(false);
		}
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null && gorillaGuardianManager.isPlaying && PhotonNetwork.IsMasterClient)
		{
			this.StartPlaying();
		}
	}

	// Token: 0x0600385A RID: 14426 RVA: 0x00133354 File Offset: 0x00131554
	private void Start()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x0600385B RID: 14427 RVA: 0x0013337C File Offset: 0x0013157C
	public void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		GorillaGuardianZoneManager.zoneManagers.Remove(this);
	}

	// Token: 0x0600385C RID: 14428 RVA: 0x001333B0 File Offset: 0x001315B0
	public override void OnEnable()
	{
		base.OnEnable();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600385D RID: 14429 RVA: 0x001333BF File Offset: 0x001315BF
	public override void OnDisable()
	{
		base.OnDisable();
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600385E RID: 14430 RVA: 0x001333D0 File Offset: 0x001315D0
	public void SliceUpdate()
	{
		float idolActivationDisplay = this._idolActivationDisplay;
		float num = 0f;
		if (this._currentActivationTime < 0f)
		{
			this._idolActivationDisplay = 0f;
			this._progressing = false;
		}
		else
		{
			num = Mathf.Min(Time.time - this._lastTappedTime, this.activationTimePerTap);
			this._progressing = num < this.activationTimePerTap;
			this._idolActivationDisplay = (this._currentActivationTime + num) / this.requiredActivationTime;
		}
		if (idolActivationDisplay != this._idolActivationDisplay)
		{
			this.idol.UpdateActivationProgress(this._currentActivationTime + num, this._progressing);
		}
	}

	// Token: 0x0600385F RID: 14431 RVA: 0x00133467 File Offset: 0x00131667
	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		this.StopPlaying();
	}

	// Token: 0x06003860 RID: 14432 RVA: 0x00133475 File Offset: 0x00131675
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		if (this.guardianPlayer == null || this.guardianPlayer.GetPlayerRef() == otherPlayer)
		{
			this.SetGuardian(null);
		}
		NetPlayer previousGuardian = this._previousGuardian;
		if (((previousGuardian != null) ? previousGuardian.GetPlayerRef() : null) == otherPlayer)
		{
			this._previousGuardian = null;
		}
	}

	// Token: 0x06003861 RID: 14433 RVA: 0x001334B0 File Offset: 0x001316B0
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.IsInZone(this.zone);
		if (flag != this._zoneIsActive || !this._zoneStateChanged)
		{
			this._zoneIsActive = flag;
			this.idol.OnZoneActiveStateChanged(this._zoneIsActive);
			this._zoneStateChanged = true;
		}
		if (!this._zoneIsActive)
		{
			return;
		}
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null && gorillaGuardianManager.isPlaying && gorillaGuardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer) && this.guardianPlayer != null && this.guardianPlayer != NetworkSystem.Instance.LocalPlayer)
		{
			gorillaGuardianManager.RequestEjectGuardian(NetworkSystem.Instance.LocalPlayer);
		}
	}

	// Token: 0x06003862 RID: 14434 RVA: 0x00133554 File Offset: 0x00131754
	public void StartPlaying()
	{
		if (!this.IsZoneValid())
		{
			return;
		}
		this._currentActivationTime = -1f;
		if (this.guardianPlayer != null && !this.guardianPlayer.InRoom())
		{
			this.SetGuardian(null);
			this._previousGuardian = null;
		}
		this.idol.gameObject.SetActive(true);
		this.SelectNextIdol();
		this.SetIdolPosition(this.currentIdol);
	}

	// Token: 0x06003863 RID: 14435 RVA: 0x001335BC File Offset: 0x001317BC
	public void StopPlaying()
	{
		this._currentActivationTime = -1f;
		this.currentIdol = -1;
		this.idol.gameObject.SetActive(false);
		this._progressing = false;
		this._lastTappedTime = 0f;
		this.SetGuardian(null);
		this._previousGuardian = null;
	}

	// Token: 0x06003864 RID: 14436 RVA: 0x0013360C File Offset: 0x0013180C
	public void SetScaleCenterPoint(Transform scaleCenterPoint)
	{
		this.guardianSizeChanger.SetScaleCenterPoint(scaleCenterPoint);
	}

	// Token: 0x06003865 RID: 14437 RVA: 0x0013361A File Offset: 0x0013181A
	public void IdolWasTapped(NetPlayer tapper)
	{
		if (tapper != null && (!GameMode.ParticipatingPlayers.Contains(tapper) || tapper == this.guardianPlayer))
		{
			return;
		}
		if (!this.IsZoneValid())
		{
			return;
		}
		if (this.UpdateTapCount(tapper))
		{
			this.IdolActivated(tapper);
		}
	}

	// Token: 0x06003866 RID: 14438 RVA: 0x0013364F File Offset: 0x0013184F
	public bool IsZoneValid()
	{
		return NetworkSystem.Instance.SessionIsPrivate || ZoneManagement.IsInZone(this.zone);
	}

	// Token: 0x06003867 RID: 14439 RVA: 0x0013366C File Offset: 0x0013186C
	private bool UpdateTapCount(NetPlayer tapper)
	{
		if (this.guardianPlayer == null && this._previousGuardian == null)
		{
			return true;
		}
		if (this._currentActivationTime < 0f)
		{
			this._currentActivationTime = 0f;
			this._lastTappedTime = Time.time;
		}
		if (!this._progressing)
		{
			float num = Mathf.Min(Time.time - this._lastTappedTime, this.activationTimePerTap);
			this._lastTappedTime = Time.time;
			if (num + this._currentActivationTime >= this.requiredActivationTime)
			{
				return true;
			}
			this._currentActivationTime += num;
		}
		return false;
	}

	// Token: 0x06003868 RID: 14440 RVA: 0x001336FA File Offset: 0x001318FA
	private void IdolActivated(NetPlayer activater)
	{
		this._currentActivationTime = -1f;
		this.SetGuardian(activater);
		this.SelectNextIdol();
		this.MoveIdolPosition(this.currentIdol);
	}

	// Token: 0x06003869 RID: 14441 RVA: 0x00133724 File Offset: 0x00131924
	public void SetGuardian(NetPlayer newGuardian)
	{
		if (this.guardianPlayer == newGuardian)
		{
			return;
		}
		if (this.guardianPlayer != null)
		{
			if (NetworkSystem.Instance.LocalPlayer == this.guardianPlayer)
			{
				this.PlayerLostGuardianSFX.Play();
			}
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(this.guardianPlayer, out rigContainer))
			{
				rigContainer.Rig.EnableGuardianEjectWatch(false);
				this.guardianSizeChanger.unacceptRig(rigContainer.Rig);
				int num = (RoomSystem.JoinedRoom ? rigContainer.netView.ViewID : rigContainer.CachedNetViewID);
				if (GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex == num)
				{
					GorillaTagger.Instance.offlineVRRig.DroppedByPlayer(rigContainer.Rig, Vector3.zero);
					if (this.guardianPlayer == NetworkSystem.Instance.LocalPlayer)
					{
						bool flag = GorillaTagger.Instance.offlineVRRig.grabbedRopeBoneIndex == 1;
						EquipmentInteractor.instance.UpdateHandEquipment(null, flag);
					}
				}
			}
		}
		this._previousGuardian = this.guardianPlayer;
		this.guardianPlayer = newGuardian;
		if (this.guardianPlayer != null)
		{
			if (NetworkSystem.Instance.LocalPlayer == this.guardianPlayer)
			{
				this.PlayerGainGuardianSFX.Play();
			}
			else
			{
				this.ObserverGainGuardianSFX.Play();
			}
			RigContainer rigContainer2;
			if (VRRigCache.Instance.TryGetVrrig(this.guardianPlayer, out rigContainer2))
			{
				rigContainer2.Rig.EnableGuardianEjectWatch(true);
				this.guardianSizeChanger.acceptRig(rigContainer2.Rig);
			}
			PlayerGameEvents.GameModeCompleteRound();
			if (NetworkSystem.Instance.LocalPlayer == this.guardianPlayer)
			{
				PlayerGameEvents.GameModeObjectiveTriggered();
			}
		}
	}

	// Token: 0x0600386A RID: 14442 RVA: 0x001338A7 File Offset: 0x00131AA7
	public bool IsPlayerGuardian(NetPlayer player)
	{
		return player == this.guardianPlayer;
	}

	// Token: 0x0600386B RID: 14443 RVA: 0x001338B2 File Offset: 0x00131AB2
	private int SelectNextIdol()
	{
		if (this.idolPositions == null || this.idolPositions.Count == 0)
		{
			GTDev.Log<string>("No Guardian Idols possible to select.", null);
			return -1;
		}
		this.currentIdol = this.SelectRandomIdol();
		return this.currentIdol;
	}

	// Token: 0x0600386C RID: 14444 RVA: 0x001338E8 File Offset: 0x00131AE8
	private int SelectRandomIdol()
	{
		int num;
		if (this.currentIdol != -1 && this.idolPositions.Count > 1)
		{
			num = (this.currentIdol + Random.Range(1, this.idolPositions.Count)) % this.idolPositions.Count;
		}
		else
		{
			num = Random.Range(0, this.idolPositions.Count);
		}
		return num;
	}

	// Token: 0x0600386D RID: 14445 RVA: 0x00133948 File Offset: 0x00131B48
	private int SelectFarthestFromGuardian()
	{
		if (!(GorillaGameManager.instance is GorillaGuardianManager))
		{
			return this.SelectRandomIdol();
		}
		RigContainer rigContainer;
		if (this.guardianPlayer != null && VRRigCache.Instance.TryGetVrrig(this.guardianPlayer, out rigContainer))
		{
			Vector3 position = rigContainer.transform.position;
			int num = -1;
			float num2 = 0f;
			for (int i = 0; i < this.idolPositions.Count; i++)
			{
				float num3 = Vector3.SqrMagnitude(this.idolPositions[i].transform.position - position);
				if (num3 > num2)
				{
					num2 = num3;
					num = i;
				}
			}
			if (num != -1)
			{
				return num;
			}
		}
		return this.SelectRandomIdol();
	}

	// Token: 0x0600386E RID: 14446 RVA: 0x001339F0 File Offset: 0x00131BF0
	private int SelectFarFromNearestPlayer()
	{
		List<Transform> list = this.SortByDistanceToNearestPlayer();
		if (list.Count > 1 && this.currentIdol >= 0 && this.currentIdol < list.Count)
		{
			list.Remove(this.idolPositions[this.currentIdol]);
		}
		int num = Random.Range(list.Count / 2, list.Count);
		Transform transform = list[num];
		return this.idolPositions.IndexOf(transform);
	}

	// Token: 0x0600386F RID: 14447 RVA: 0x00133A64 File Offset: 0x00131C64
	private List<Transform> SortByDistanceToNearestPlayer()
	{
		GorillaGuardianZoneManager.<>c__DisplayClass49_0 CS$<>8__locals1 = new GorillaGuardianZoneManager.<>c__DisplayClass49_0();
		CS$<>8__locals1.playerPositions = new List<Vector3>();
		foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
		{
			if (!rigContainer.IsNull())
			{
				CS$<>8__locals1.playerPositions.Add(rigContainer.transform.position);
			}
		}
		this._sortedIdolPositions.Clear();
		foreach (Transform transform in this.idolPositions)
		{
			this._sortedIdolPositions.Add(transform);
		}
		this._sortedIdolPositions.Sort(new Comparison<Transform>(CS$<>8__locals1.<SortByDistanceToNearestPlayer>g__CompareNearestPlayerDistance|0));
		return this._sortedIdolPositions;
	}

	// Token: 0x06003870 RID: 14448 RVA: 0x00133B4C File Offset: 0x00131D4C
	public void TriggerIdolKnockback()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		for (int i = 0; i < RoomSystem.PlayersInRoom.Count; i++)
		{
			RigContainer rigContainer;
			if ((this.knockbackIncludesGuardian || RoomSystem.PlayersInRoom[i] != this.guardianPlayer) && VRRigCache.Instance.TryGetVrrig(RoomSystem.PlayersInRoom[i], out rigContainer))
			{
				Vector3 vector = rigContainer.Rig.transform.position - this.idol.transform.position;
				if (Vector3.SqrMagnitude(vector) < this.idolKnockbackRadius * this.idolKnockbackRadius)
				{
					Vector3 vector2 = (vector - Vector3.up * Vector3.Dot(Vector3.up, vector)).normalized * this.idolKnockbackStrengthHoriz + Vector3.up * this.idolKnockbackStrengthVert;
					RoomSystem.LaunchPlayer(RoomSystem.PlayersInRoom[i], vector2);
				}
			}
		}
	}

	// Token: 0x06003871 RID: 14449 RVA: 0x00133C48 File Offset: 0x00131E48
	private void SetIdolPosition(int index)
	{
		if (index < 0 || index >= this.idolPositions.Count)
		{
			GTDev.Log<string>("Invalid index received", null);
			return;
		}
		this.idol.gameObject.SetActive(true);
		this.idol.SetPosition(this.idolPositions[index].position);
	}

	// Token: 0x06003872 RID: 14450 RVA: 0x00133CA0 File Offset: 0x00131EA0
	private void MoveIdolPosition(int index)
	{
		if (index < 0 || index >= this.idolPositions.Count)
		{
			GTDev.Log<string>("Invalid index received", null);
			return;
		}
		this.idol.gameObject.SetActive(true);
		this.idol.MovePositions(this.idolPositions[index].position);
		if (base.photonView.IsMine)
		{
			this.idolMoveCount++;
		}
	}

	// Token: 0x06003873 RID: 14451 RVA: 0x00133D14 File Offset: 0x00131F14
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager == null || !gorillaGuardianManager.isPlaying || player != NetworkSystem.Instance.MasterClient)
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext((this.guardianPlayer != null) ? this.guardianPlayer.ActorNumber : 0);
			stream.SendNext(this._currentActivationTime);
			stream.SendNext(this.currentIdol);
			stream.SendNext(this.idolMoveCount);
			return;
		}
		int num = (int)stream.ReceiveNext();
		float num2 = (float)stream.ReceiveNext();
		int num3 = (int)stream.ReceiveNext();
		int num4 = (int)stream.ReceiveNext();
		if (float.IsNaN(num2) || float.IsInfinity(num2))
		{
			return;
		}
		this.SetGuardian((num != 0) ? NetworkSystem.Instance.GetPlayer(num) : null);
		if (num2 != this._currentActivationTime)
		{
			this._currentActivationTime = num2;
			this._lastTappedTime = Time.time;
		}
		if (num3 != this.currentIdol || num4 != this.idolMoveCount)
		{
			if (this.currentIdol == -1)
			{
				this.SetIdolPosition(num3);
			}
			else
			{
				this.MoveIdolPosition(num3);
			}
			this.currentIdol = num3;
			this.idolMoveCount = num4;
		}
	}

	// Token: 0x04004863 RID: 18531
	public static List<GorillaGuardianZoneManager> zoneManagers = new List<GorillaGuardianZoneManager>();

	// Token: 0x04004864 RID: 18532
	[SerializeField]
	private GTZone zone;

	// Token: 0x04004865 RID: 18533
	[SerializeField]
	private SizeChanger guardianSizeChanger;

	// Token: 0x04004866 RID: 18534
	[SerializeField]
	private TappableGuardianIdol idol;

	// Token: 0x04004867 RID: 18535
	[SerializeField]
	private List<Transform> idolPositions;

	// Token: 0x04004868 RID: 18536
	[Space]
	[SerializeField]
	private float requiredActivationTime = 10f;

	// Token: 0x04004869 RID: 18537
	[SerializeField]
	private float activationTimePerTap = 1f;

	// Token: 0x0400486A RID: 18538
	[Space]
	[SerializeField]
	private bool knockbackIncludesGuardian = true;

	// Token: 0x0400486B RID: 18539
	[SerializeField]
	private float idolKnockbackRadius = 6f;

	// Token: 0x0400486C RID: 18540
	[SerializeField]
	private float idolKnockbackStrengthVert = 12f;

	// Token: 0x0400486D RID: 18541
	[SerializeField]
	private float idolKnockbackStrengthHoriz = 15f;

	// Token: 0x0400486E RID: 18542
	[Space]
	[SerializeField]
	private SoundBankPlayer PlayerGainGuardianSFX;

	// Token: 0x0400486F RID: 18543
	[SerializeField]
	private SoundBankPlayer PlayerLostGuardianSFX;

	// Token: 0x04004870 RID: 18544
	[SerializeField]
	private SoundBankPlayer ObserverGainGuardianSFX;

	// Token: 0x04004871 RID: 18545
	private NetPlayer guardianPlayer;

	// Token: 0x04004872 RID: 18546
	private NetPlayer _previousGuardian;

	// Token: 0x04004873 RID: 18547
	private int currentIdol = -1;

	// Token: 0x04004874 RID: 18548
	private int idolMoveCount;

	// Token: 0x04004875 RID: 18549
	private List<Transform> _sortedIdolPositions = new List<Transform>();

	// Token: 0x04004876 RID: 18550
	private float _currentActivationTime = -1f;

	// Token: 0x04004877 RID: 18551
	private float _lastTappedTime;

	// Token: 0x04004878 RID: 18552
	private bool _progressing;

	// Token: 0x04004879 RID: 18553
	private float _idolActivationDisplay;

	// Token: 0x0400487A RID: 18554
	private bool _zoneIsActive;

	// Token: 0x0400487B RID: 18555
	private bool _zoneStateChanged;
}
