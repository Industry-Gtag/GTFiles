using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Modio.Mods;
using TMPro;
using UnityEngine;

// Token: 0x02000AD4 RID: 2772
public class VirtualStumpTeleporter : MonoBehaviour, IBuildValidation, IGorillaSliceableSimple
{
	// Token: 0x06004723 RID: 18211 RVA: 0x0017FEBD File Offset: 0x0017E0BD
	public bool BuildValidationCheck()
	{
		if (this.netSerializer.IsNull())
		{
			Debug.LogError("VStump Teleporter \"" + base.gameObject.GetPath() + "\" needs a reference to a VirtualStumpTeleporterSerializer for networked FX to function. Check out the teleporter prefabs in arcade or the stump", this);
			return false;
		}
		return true;
	}

	// Token: 0x06004724 RID: 18212 RVA: 0x0017FEF0 File Offset: 0x0017E0F0
	public void SliceUpdate()
	{
		if (!this.accessDenied && NetworkSystem.Instance.netState != NetSystemState.Idle && NetworkSystem.Instance.netState != NetSystemState.InGame)
		{
			this.DenyAccess();
		}
		if (this.accessDenied && (NetworkSystem.Instance.netState == NetSystemState.Idle || NetworkSystem.Instance.netState == NetSystemState.InGame) && !UGCPermissionManager.HasNoMapAccess)
		{
			this.AllowAccess();
		}
	}

	// Token: 0x06004725 RID: 18213 RVA: 0x0017FF54 File Offset: 0x0017E154
	public void OnEnable()
	{
		if (this.netSerializer.IsNull())
		{
			Debug.LogWarning("[VStumpTeleporter.OnEnable] Net Serializer is null for \"" + base.gameObject.GetPath() + "\", networked teleport FX will not function.");
		}
		if (UGCPermissionManager.HasNoMapAccess || (NetworkSystem.Instance.netState != NetSystemState.Idle && NetworkSystem.Instance.netState != NetSystemState.InGame))
		{
			ushort num = VirtualStumpTeleporter.lastLoggingHandsMsgId;
			VirtualStumpTeleporter.lastLoggingHandsMsgId = 1;
			this.DenyAccess();
		}
		else
		{
			ushort num2 = VirtualStumpTeleporter.lastLoggingHandsMsgId;
			VirtualStumpTeleporter.lastLoggingHandsMsgId = 2;
			this.AllowAccess();
		}
		UGCPermissionManager.SubscribeToVirtualStumpEnabled(new Action(this.OnVirtualStumpEnabled));
		UGCPermissionManager.SubscribeToVirtualStumpDisabled(new Action(this.OnVirtualStumpDisabled));
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06004726 RID: 18214 RVA: 0x00180001 File Offset: 0x0017E201
	public void OnDisable()
	{
		this.AllowAccess();
		UGCPermissionManager.UnsubscribeFromVirtualStumpEnabled(new Action(this.OnVirtualStumpEnabled));
		UGCPermissionManager.UnsubscribeFromVirtualStumpDisabled(new Action(this.OnVirtualStumpDisabled));
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06004727 RID: 18215 RVA: 0x00180033 File Offset: 0x0017E233
	private void OnVirtualStumpEnabled()
	{
		this.AllowAccess();
		ushort num = VirtualStumpTeleporter.lastLoggingHandsMsgId;
		VirtualStumpTeleporter.lastLoggingHandsMsgId = 3;
	}

	// Token: 0x06004728 RID: 18216 RVA: 0x00180049 File Offset: 0x0017E249
	private void OnVirtualStumpDisabled()
	{
		this.DenyAccess();
		ushort num = VirtualStumpTeleporter.lastLoggingHandsMsgId;
		VirtualStumpTeleporter.lastLoggingHandsMsgId = 4;
	}

	// Token: 0x06004729 RID: 18217 RVA: 0x00180060 File Offset: 0x0017E260
	public void OnTriggerEnter(Collider other)
	{
		if (UGCPermissionManager.HasNoMapAccess || this.accessDenied || this.teleporting || CustomMapManager.WaitingForRoomJoin || CustomMapManager.WaitingForDisconnect)
		{
			return;
		}
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			this.triggerEntryTime = Time.time;
			this.ShowCountdownText();
		}
	}

	// Token: 0x0600472A RID: 18218 RVA: 0x001800C0 File Offset: 0x0017E2C0
	public void OnTriggerStay(Collider other)
	{
		if (UGCPermissionManager.HasNoMapAccess || this.accessDenied)
		{
			return;
		}
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject && this.triggerEntryTime >= 0f)
		{
			this.UpdateCountdownText();
			if (!this.teleporting && this.triggerEntryTime + this.stayInTriggerDuration <= Time.time)
			{
				this.TeleportPlayer();
				this.HideCountdownText();
			}
		}
	}

	// Token: 0x0600472B RID: 18219 RVA: 0x00180134 File Offset: 0x0017E334
	public void OnTriggerExit(Collider other)
	{
		if (UGCPermissionManager.HasNoMapAccess || this.accessDenied)
		{
			return;
		}
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			this.triggerEntryTime = -1f;
			this.HideCountdownText();
		}
	}

	// Token: 0x0600472C RID: 18220 RVA: 0x00180174 File Offset: 0x0017E374
	private void ShowCountdownText()
	{
		if (UGCPermissionManager.HasNoMapAccess || this.accessDenied)
		{
			return;
		}
		if (!this.countdownTexts.IsNullOrEmpty<TMP_Text>())
		{
			int num = 1 + Mathf.FloorToInt(this.stayInTriggerDuration);
			for (int i = 0; i < this.countdownTexts.Length; i++)
			{
				if (!this.countdownTexts[i].IsNull())
				{
					this.countdownTexts[i].text = num.ToString();
					this.countdownTexts[i].gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x0600472D RID: 18221 RVA: 0x001801F8 File Offset: 0x0017E3F8
	private void HideCountdownText()
	{
		if (!this.countdownTexts.IsNullOrEmpty<TMP_Text>())
		{
			for (int i = 0; i < this.countdownTexts.Length; i++)
			{
				if (!this.countdownTexts[i].IsNull())
				{
					this.countdownTexts[i].text = "";
					this.countdownTexts[i].gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x0600472E RID: 18222 RVA: 0x0018025C File Offset: 0x0017E45C
	private void UpdateCountdownText()
	{
		if (UGCPermissionManager.HasNoMapAccess || this.accessDenied)
		{
			return;
		}
		if (!this.countdownTexts.IsNullOrEmpty<TMP_Text>())
		{
			float num = this.stayInTriggerDuration - (Time.time - this.triggerEntryTime);
			int num2 = 1 + Mathf.FloorToInt(num);
			for (int i = 0; i < this.countdownTexts.Length; i++)
			{
				if (!this.countdownTexts[i].IsNull())
				{
					this.countdownTexts[i].text = num2.ToString();
				}
			}
		}
	}

	// Token: 0x0600472F RID: 18223 RVA: 0x001802DC File Offset: 0x0017E4DC
	public void TeleportPlayer()
	{
		if (UGCPermissionManager.HasNoMapAccess || this.accessDenied)
		{
			return;
		}
		if (!this.teleporting)
		{
			this.teleporting = true;
			GorillaTelemetry.EnqueueTelemetryEvent("vstump_teleported_in", new Dictionary<string, object>(), null);
			CustomMapManager.TeleportToVirtualStump(this, new Action<bool>(this.FinishTeleport));
		}
	}

	// Token: 0x06004730 RID: 18224 RVA: 0x0018032A File Offset: 0x0017E52A
	private void FinishTeleport(bool success = true)
	{
		if (this.teleporting)
		{
			this.teleporting = false;
			this.triggerEntryTime = -1f;
		}
	}

	// Token: 0x06004731 RID: 18225 RVA: 0x00180348 File Offset: 0x0017E548
	private void DenyAccess()
	{
		this.accessDenied = true;
		foreach (GameObject gameObject in this.accessDeniedEnabledObjects)
		{
			gameObject.SetActive(true);
		}
		foreach (GameObject gameObject2 in this.accessDeniedDisabledObjects)
		{
			gameObject2.SetActive(false);
		}
	}

	// Token: 0x06004732 RID: 18226 RVA: 0x001803E0 File Offset: 0x0017E5E0
	private void AllowAccess()
	{
		if (UGCPermissionManager.HasNoMapAccess)
		{
			return;
		}
		this.accessDenied = false;
		foreach (GameObject gameObject in this.accessDeniedEnabledObjects)
		{
			gameObject.SetActive(false);
		}
		foreach (GameObject gameObject2 in this.accessDeniedDisabledObjects)
		{
			gameObject2.SetActive(true);
		}
	}

	// Token: 0x06004733 RID: 18227 RVA: 0x00180480 File Offset: 0x0017E680
	private short GetIndex()
	{
		if (!this.netSerializer.IsNotNull())
		{
			return -1;
		}
		return this.netSerializer.GetTeleporterIndex(this);
	}

	// Token: 0x06004734 RID: 18228 RVA: 0x0018049D File Offset: 0x0017E69D
	public GTZone GetZone()
	{
		return this.entranceZone;
	}

	// Token: 0x06004735 RID: 18229 RVA: 0x001804A5 File Offset: 0x0017E6A5
	public GorillaNetworkJoinTrigger GetExitVStumpJoinTrigger()
	{
		return this.exitVStumpJoinTrigger;
	}

	// Token: 0x06004736 RID: 18230 RVA: 0x001804AD File Offset: 0x0017E6AD
	public Transform GetReturnTransform()
	{
		return this.returnLocation;
	}

	// Token: 0x06004737 RID: 18231 RVA: 0x001804B5 File Offset: 0x0017E6B5
	public long GetAutoLoadMapModId()
	{
		return this.autoLoadMapModId;
	}

	// Token: 0x06004738 RID: 18232 RVA: 0x001804BD File Offset: 0x0017E6BD
	public GameModeType GetAutoLoadGamemode()
	{
		return this.autoLoadGamemode;
	}

	// Token: 0x06004739 RID: 18233 RVA: 0x001804C5 File Offset: 0x0017E6C5
	public GameModeType GetReturnGamemode()
	{
		return this.forcedGamemodeUponReturn;
	}

	// Token: 0x0600473A RID: 18234 RVA: 0x001804D0 File Offset: 0x0017E6D0
	public void PlayTeleportEffects(bool forLocalPlayer, bool toVStump, AudioSource vStumpSFXAudioSource = null, bool sendRPC = false)
	{
		if (sendRPC && this.netSerializer.IsNotNull())
		{
			this.netSerializer.NotifyPlayerTeleporting(this.GetIndex(), vStumpSFXAudioSource);
		}
		ParticleSystem particleSystem;
		if (toVStump)
		{
			particleSystem = this.teleportToVStumpVFX;
			if (forLocalPlayer && vStumpSFXAudioSource.IsNotNull() && !this.teleportingPlayerSoundClips.IsNullOrEmpty<AudioClip>())
			{
				vStumpSFXAudioSource.clip = this.teleportingPlayerSoundClips[Random.Range(0, this.teleportingPlayerSoundClips.Count)];
				vStumpSFXAudioSource.Play();
			}
			if (!forLocalPlayer && this.teleporterSFXAudioSource.IsNotNull() && !this.observerSoundClips.IsNullOrEmpty<AudioClip>())
			{
				this.teleporterSFXAudioSource.clip = this.observerSoundClips[Random.Range(0, this.observerSoundClips.Count)];
				this.teleporterSFXAudioSource.Play();
			}
		}
		else
		{
			particleSystem = this.returnFromVStumpVFX;
			if (this.teleporterSFXAudioSource.IsNotNull())
			{
				if (forLocalPlayer && !this.teleportingPlayerSoundClips.IsNullOrEmpty<AudioClip>())
				{
					this.teleporterSFXAudioSource.clip = this.teleportingPlayerSoundClips[Random.Range(0, this.teleportingPlayerSoundClips.Count)];
				}
				else if (!forLocalPlayer && !this.observerSoundClips.IsNullOrEmpty<AudioClip>())
				{
					this.teleporterSFXAudioSource.clip = this.observerSoundClips[Random.Range(0, this.observerSoundClips.Count)];
				}
				this.teleporterSFXAudioSource.Play();
			}
		}
		if (particleSystem.IsNotNull())
		{
			particleSystem.Play();
		}
	}

	// Token: 0x040059A8 RID: 22952
	[SerializeField]
	private float stayInTriggerDuration = 3f;

	// Token: 0x040059A9 RID: 22953
	[SerializeField]
	private TMP_Text[] countdownTexts;

	// Token: 0x040059AA RID: 22954
	[SerializeField]
	private GameObject[] handHoldObjects;

	// Token: 0x040059AB RID: 22955
	[SerializeField]
	private List<GameObject> accessDeniedDisabledObjects = new List<GameObject>();

	// Token: 0x040059AC RID: 22956
	[SerializeField]
	private List<GameObject> accessDeniedEnabledObjects = new List<GameObject>();

	// Token: 0x040059AD RID: 22957
	[SerializeField]
	private Transform returnLocation;

	// Token: 0x040059AE RID: 22958
	[SerializeField]
	private GTZone entranceZone = GTZone.arcade;

	// Token: 0x040059AF RID: 22959
	[SerializeField]
	private GorillaNetworkJoinTrigger exitVStumpJoinTrigger;

	// Token: 0x040059B0 RID: 22960
	[SerializeField]
	private long autoLoadMapModId = ModId.Null;

	// Token: 0x040059B1 RID: 22961
	[SerializeField]
	private GameModeType autoLoadGamemode = GameModeType.None;

	// Token: 0x040059B2 RID: 22962
	[SerializeField]
	private GameModeType forcedGamemodeUponReturn = GameModeType.None;

	// Token: 0x040059B3 RID: 22963
	[SerializeField]
	private ParticleSystem teleportToVStumpVFX;

	// Token: 0x040059B4 RID: 22964
	[SerializeField]
	private ParticleSystem returnFromVStumpVFX;

	// Token: 0x040059B5 RID: 22965
	[SerializeField]
	private AudioSource teleporterSFXAudioSource;

	// Token: 0x040059B6 RID: 22966
	[SerializeField]
	private List<AudioClip> teleportingPlayerSoundClips = new List<AudioClip>();

	// Token: 0x040059B7 RID: 22967
	[SerializeField]
	private List<AudioClip> observerSoundClips = new List<AudioClip>();

	// Token: 0x040059B8 RID: 22968
	[SerializeField]
	private VirtualStumpTeleporterSerializer netSerializer;

	// Token: 0x040059B9 RID: 22969
	private VirtualStumpTeleporterSerializer mySerializer;

	// Token: 0x040059BA RID: 22970
	private bool accessDenied;

	// Token: 0x040059BB RID: 22971
	private bool teleporting;

	// Token: 0x040059BC RID: 22972
	private float triggerEntryTime = -1f;

	// Token: 0x040059BD RID: 22973
	[OnEnterPlay_Set(0)]
	private static ushort lastLoggingHandsMsgId;
}
