using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020007B5 RID: 1973
public class GRFirstTimeUserExperience : MonoBehaviour
{
	// Token: 0x0600327C RID: 12924 RVA: 0x001059C3 File Offset: 0x00103BC3
	[ContextMenu("Set Player Pref")]
	private void RemovePlayerPref()
	{
		PlayerPrefs.SetString("spawnInWrongStump", "flagged");
		PlayerPrefs.Save();
	}

	// Token: 0x0600327D RID: 12925 RVA: 0x00114978 File Offset: 0x00112B78
	private void OnEnable()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.flickerSphere.SetActive(false);
		this.logoQuad.SetActive(false);
		this.flickerSphereOrigParent = this.flickerSphere.transform.parent;
		GameLightingManager.instance.SetCustomDynamicLightingEnabled(true);
		this.playerLight = GorillaTagger.Instance.mainCamera.GetComponentInChildren<GameLight>(true);
		this.playerLight.gameObject.SetActive(true);
		this.ChangeState(GRFirstTimeUserExperience.TransitionState.Waiting);
	}

	// Token: 0x0600327E RID: 12926 RVA: 0x001149FC File Offset: 0x00112BFC
	public void ChangeState(GRFirstTimeUserExperience.TransitionState state)
	{
		this.transitionState = state;
		switch (state)
		{
		case GRFirstTimeUserExperience.TransitionState.Waiting:
			this.stateStartTime = Time.time;
			return;
		case GRFirstTimeUserExperience.TransitionState.Flicker:
			this.transitionState = GRFirstTimeUserExperience.TransitionState.Flicker;
			this.flickerSphere.transform.SetParent(GTPlayer.Instance.headCollider.transform, false);
			this.flickerSphere.SetActive(true);
			this.logoQuad.SetActive(false);
			this.stateStartTime = Time.time;
			return;
		case GRFirstTimeUserExperience.TransitionState.Logo:
			this.stateStartTime = Time.time;
			this.flickerSphere.SetActive(true);
			this.logoQuad.SetActive(true);
			return;
		case GRFirstTimeUserExperience.TransitionState.ZoneLoad:
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.OnSceneLoadsCompleted = (Action)Delegate.Combine(instance.OnSceneLoadsCompleted, new Action(this.OnZoneLoadComplete));
			ZoneManagement.SetActiveZone(this.teleportZone);
			return;
		}
		case GRFirstTimeUserExperience.TransitionState.Teleport:
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this.joinRoomTrigger, JoinType.Solo, null, false);
			GTPlayer.Instance.TeleportTo(this.teleportLocation.position, this.teleportLocation.rotation, false, false);
			GTPlayer.Instance.InitializeValues();
			this.stateStartTime = Time.time;
			return;
		case GRFirstTimeUserExperience.TransitionState.Exit:
			this.flickerSphere.transform.SetParent(this.flickerSphereOrigParent, false);
			this.flickerSphere.SetActive(false);
			this.logoQuad.SetActive(false);
			this.rootObject.SetActive(false);
			GorillaTagger.Instance.mainCamera.GetComponentInChildren<GameLight>(true).gameObject.SetActive(false);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600327F RID: 12927 RVA: 0x00114B7F File Offset: 0x00112D7F
	private void OnZoneLoadComplete()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.OnSceneLoadsCompleted = (Action)Delegate.Remove(instance.OnSceneLoadsCompleted, new Action(this.OnZoneLoadComplete));
		this.ChangeState(GRFirstTimeUserExperience.TransitionState.Teleport);
	}

	// Token: 0x06003280 RID: 12928 RVA: 0x00114BB0 File Offset: 0x00112DB0
	public void InterruptWaitingTimer()
	{
		this.stateStartTime = -1f;
		for (int i = 0; i < this.delayObjects.Count; i++)
		{
			this.delayObjects[i].enabledTime = this.stateStartTime;
		}
	}

	// Token: 0x06003281 RID: 12929 RVA: 0x00114BF8 File Offset: 0x00112DF8
	private void Update()
	{
		switch (this.transitionState)
		{
		case GRFirstTimeUserExperience.TransitionState.Waiting:
			if (PrivateUIRoom.GetInOverlay())
			{
				if (this.stateStartTime >= 0f)
				{
					this.InterruptWaitingTimer();
				}
			}
			else if (this.stateStartTime < 0f)
			{
				this.stateStartTime = Time.time;
			}
			if (this.stateStartTime >= 0f && Time.time - this.stateStartTime >= this.transitionDelay)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.Flicker);
				return;
			}
			break;
		case GRFirstTimeUserExperience.TransitionState.Flicker:
		{
			float num = Time.time - this.stateStartTime;
			if (this.stateStartTime >= 0f && num >= this.flickerDuration)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.Logo);
				return;
			}
			bool flag = this.flickerTimeline.Evaluate(num / this.flickerDuration) < 0f;
			this.flickerSphere.SetActive(flag);
			if (flag && !this.flickerLightWasOff)
			{
				if (this.audioSource != null && this.flickerAudioCount < this.flickerAudio.Count && this.flickerAudio[this.flickerAudioCount] != null)
				{
					this.audioSource.PlayOneShot(this.flickerAudio[this.flickerAudioCount]);
				}
				this.flickerAudioCount++;
			}
			this.flickerLightWasOff = flag;
			return;
		}
		case GRFirstTimeUserExperience.TransitionState.Logo:
			if (this.stateStartTime >= 0f && Time.time - this.stateStartTime >= this.logoDisplayTime)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.ZoneLoad);
				return;
			}
			break;
		case GRFirstTimeUserExperience.TransitionState.ZoneLoad:
			break;
		case GRFirstTimeUserExperience.TransitionState.Teleport:
			if (this.stateStartTime >= 0f && Time.time - this.stateStartTime >= this.teleportSettleTime)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.Exit);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x04004161 RID: 16737
	public Transform spawnPoint;

	// Token: 0x04004162 RID: 16738
	public GameObject rootObject;

	// Token: 0x04004163 RID: 16739
	public GameObject flickerSphere;

	// Token: 0x04004164 RID: 16740
	public GameObject logoQuad;

	// Token: 0x04004165 RID: 16741
	public AnimationCurve flickerTimeline;

	// Token: 0x04004166 RID: 16742
	public float flickerDuration = 3f;

	// Token: 0x04004167 RID: 16743
	public GTZone teleportZone = GTZone.none;

	// Token: 0x04004168 RID: 16744
	public Transform teleportLocation;

	// Token: 0x04004169 RID: 16745
	public float transitionDelay = 60f;

	// Token: 0x0400416A RID: 16746
	public float logoDisplayTime = 4f;

	// Token: 0x0400416B RID: 16747
	public float teleportSettleTime = 1f;

	// Token: 0x0400416C RID: 16748
	public GorillaNetworkJoinTrigger joinRoomTrigger;

	// Token: 0x0400416D RID: 16749
	public List<AudioClip> flickerAudio = new List<AudioClip>();

	// Token: 0x0400416E RID: 16750
	public List<DisableGameObjectDelayed> delayObjects;

	// Token: 0x0400416F RID: 16751
	private Transform flickerSphereOrigParent;

	// Token: 0x04004170 RID: 16752
	private float stateStartTime = -1f;

	// Token: 0x04004171 RID: 16753
	private bool flickerLightWasOff;

	// Token: 0x04004172 RID: 16754
	private int flickerAudioCount;

	// Token: 0x04004173 RID: 16755
	private AudioSource audioSource;

	// Token: 0x04004174 RID: 16756
	private GRFirstTimeUserExperience.TransitionState transitionState;

	// Token: 0x04004175 RID: 16757
	public GameLight playerLight;

	// Token: 0x020007B6 RID: 1974
	public enum TransitionState
	{
		// Token: 0x04004177 RID: 16759
		Waiting,
		// Token: 0x04004178 RID: 16760
		Flicker,
		// Token: 0x04004179 RID: 16761
		Logo,
		// Token: 0x0400417A RID: 16762
		ZoneLoad,
		// Token: 0x0400417B RID: 16763
		Teleport,
		// Token: 0x0400417C RID: 16764
		Exit
	}
}
