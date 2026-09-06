using System;
using System.Collections.Generic;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000776 RID: 1910
public class GRElevator : MonoBehaviour
{
	// Token: 0x0600306A RID: 12394 RVA: 0x0010668C File Offset: 0x0010488C
	private void OnEnable()
	{
		GRElevatorManager.RegisterElevator(this);
		this.ambientAudio.clip = this.ambientLoopClip;
		this.ambientAudio.Play();
	}

	// Token: 0x0600306B RID: 12395 RVA: 0x001066B0 File Offset: 0x001048B0
	private void OnDisable()
	{
		GRElevatorManager.DeregisterElevator(this);
	}

	// Token: 0x0600306C RID: 12396 RVA: 0x001066B8 File Offset: 0x001048B8
	private void Awake()
	{
		this.typeButtonDict = new Dictionary<GRElevator.ButtonType, GRElevatorButton>();
		for (int i = 0; i < this.elevatorButtons.Count; i++)
		{
			this.typeButtonDict.TryAdd(this.elevatorButtons[i].buttonType, this.elevatorButtons[i]);
		}
		this.travelDistance = (this.openTargetTop.position - this.closedTargetTop.position).magnitude;
		this.doorOpenSpeed = this.travelDistance / this.openTravelDuration;
		this.doorCloseSpeed = this.travelDistance / this.closeTravelDuration;
		this.state = GRElevator.ElevatorState.DoorClosed;
		this.UpdateLocalState(this.state);
	}

	// Token: 0x0600306D RID: 12397 RVA: 0x00106771 File Offset: 0x00104971
	public void PressButton(int type)
	{
		GRElevatorManager.ElevatorButtonPressed((GRElevator.ButtonType)type, this.location);
	}

	// Token: 0x0600306E RID: 12398 RVA: 0x00106780 File Offset: 0x00104980
	public void PressButtonVisuals(GRElevator.ButtonType type)
	{
		GRElevatorButton grelevatorButton;
		if (this.typeButtonDict.TryGetValue(type, out grelevatorButton))
		{
			grelevatorButton.Pressed();
		}
	}

	// Token: 0x0600306F RID: 12399 RVA: 0x001067A3 File Offset: 0x001049A3
	public void PlayDing()
	{
		this.ambientAudio.PlayOneShot(this.dingClip);
	}

	// Token: 0x06003070 RID: 12400 RVA: 0x001067B6 File Offset: 0x001049B6
	public void PlayButtonPress()
	{
		this.buttonBank.Play();
	}

	// Token: 0x06003071 RID: 12401 RVA: 0x001067C4 File Offset: 0x001049C4
	public void PlayElevatorMoving()
	{
		if (this.ambientAudio.isPlaying && this.ambientAudio.clip == this.travellingLoopClip)
		{
			return;
		}
		this.ambientAudio.clip = this.travellingLoopClip;
		this.ambientAudio.loop = true;
		this.ambientAudio.time = 0f;
		this.ambientAudio.Play();
	}

	// Token: 0x06003072 RID: 12402 RVA: 0x00106830 File Offset: 0x00104A30
	public void PlayElevatorStopped()
	{
		if (this.ambientAudio.isPlaying && this.ambientAudio.clip == this.ambientLoopClip)
		{
			return;
		}
		this.ambientAudio.clip = this.ambientLoopClip;
		this.ambientAudio.loop = true;
		this.ambientAudio.time = 0f;
		this.ambientAudio.Play();
	}

	// Token: 0x06003073 RID: 12403 RVA: 0x0010689B File Offset: 0x00104A9B
	public void PlayElevatorMusic(float time = 0f)
	{
		if (this.musicAudio.isPlaying)
		{
			return;
		}
		this.musicAudio.time = time;
		this.musicAudio.Play();
	}

	// Token: 0x06003074 RID: 12404 RVA: 0x001068C2 File Offset: 0x00104AC2
	public void PlayDoorOpenBegin()
	{
		this.doorAudio.clip = this.doorOpenClip;
		this.doorAudio.time = 0f;
		this.doorAudio.Play();
	}

	// Token: 0x06003075 RID: 12405 RVA: 0x001068F0 File Offset: 0x00104AF0
	public void PlayDoorCloseBegin()
	{
		this.doorAudio.clip = this.doorCloseClip;
		this.doorAudio.time = 0f;
		this.doorAudio.Play();
	}

	// Token: 0x06003076 RID: 12406 RVA: 0x0010691E File Offset: 0x00104B1E
	public void PlayDoorOpenTravel()
	{
		this.doorAudio.time = this.adjustedOffsetTime + this.openBeginDuration;
	}

	// Token: 0x06003077 RID: 12407 RVA: 0x00106938 File Offset: 0x00104B38
	public void PlayDoorCloseTravel()
	{
		this.doorAudio.time = this.adjustedOffsetTime + this.closeBeginDuration;
	}

	// Token: 0x06003078 RID: 12408 RVA: 0x00106954 File Offset: 0x00104B54
	public bool DoorsFullyClosed()
	{
		return (this.upperDoor.position - this.closedTargetTop.position).sqrMagnitude < 0.0001f;
	}

	// Token: 0x06003079 RID: 12409 RVA: 0x0010698C File Offset: 0x00104B8C
	public bool DoorsFullyOpen()
	{
		return (this.upperDoor.position - this.openTargetTop.position).sqrMagnitude < 0.0001f;
	}

	// Token: 0x0600307A RID: 12410 RVA: 0x001069C4 File Offset: 0x00104BC4
	public void UpdateLocalState(GRElevator.ElevatorState newState)
	{
		if (newState == this.state)
		{
			return;
		}
		this.state = newState;
		switch (newState)
		{
		case GRElevator.ElevatorState.DoorBeginClosing:
			if (this.DoorsFullyClosed())
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorClosed);
				return;
			}
			this.doorMoveBeginTime = Time.time;
			this.SetDoorClosedBeginTime();
			this.PlayDoorCloseBegin();
			return;
		case GRElevator.ElevatorState.DoorMovingClosing:
			this.PlayDoorCloseTravel();
			return;
		case GRElevator.ElevatorState.DoorEndClosing:
		case GRElevator.ElevatorState.DoorEndOpening:
			break;
		case GRElevator.ElevatorState.DoorClosed:
			this.upperDoor.position = this.closedTargetTop.position;
			this.lowerDoor.position = this.closedTargetBottom.position;
			return;
		case GRElevator.ElevatorState.DoorBeginOpening:
			if (this.DoorsFullyOpen())
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorOpen);
				return;
			}
			this.doorMoveBeginTime = Time.time;
			this.SetDoorOpenBeginTime();
			this.PlayDoorOpenBegin();
			return;
		case GRElevator.ElevatorState.DoorMovingOpening:
			this.PlayDoorOpenTravel();
			return;
		case GRElevator.ElevatorState.DoorOpen:
			this.upperDoor.position = this.openTargetTop.position;
			this.lowerDoor.position = this.openTargetBottom.position;
			break;
		default:
			return;
		}
	}

	// Token: 0x0600307B RID: 12411 RVA: 0x00106AC0 File Offset: 0x00104CC0
	public void UpdateRemoteState(GRElevator.ElevatorState remoteNewState)
	{
		if (GRElevator.StateIsOpeningState(remoteNewState) && GRElevator.StateIsClosingState(this.state))
		{
			this.UpdateLocalState(GRElevator.ElevatorState.DoorBeginOpening);
			return;
		}
		if (GRElevator.StateIsClosingState(remoteNewState) && GRElevator.StateIsOpeningState(this.state))
		{
			this.UpdateLocalState(GRElevator.ElevatorState.DoorBeginClosing);
		}
	}

	// Token: 0x0600307C RID: 12412 RVA: 0x00106AFC File Offset: 0x00104CFC
	public void SetDoorOpenBeginTime()
	{
		float num = (this.travelDistance - (this.upperDoor.position - this.openTargetTop.position).magnitude) / this.travelDistance;
		this.adjustedOffsetTime = num * this.openTravelDuration;
	}

	// Token: 0x0600307D RID: 12413 RVA: 0x00106B4C File Offset: 0x00104D4C
	public void SetDoorClosedBeginTime()
	{
		float num = (this.travelDistance - (this.upperDoor.position - this.closedTargetTop.position).magnitude) / this.travelDistance;
		this.adjustedOffsetTime = num * this.closeTravelDuration;
	}

	// Token: 0x0600307E RID: 12414 RVA: 0x00106B99 File Offset: 0x00104D99
	public static bool StateIsOpeningState(GRElevator.ElevatorState checkState)
	{
		return checkState == GRElevator.ElevatorState.DoorMovingOpening || checkState == GRElevator.ElevatorState.DoorBeginOpening || checkState == GRElevator.ElevatorState.DoorEndOpening || checkState == GRElevator.ElevatorState.DoorOpen;
	}

	// Token: 0x0600307F RID: 12415 RVA: 0x00106BAD File Offset: 0x00104DAD
	public static bool StateIsClosingState(GRElevator.ElevatorState checkState)
	{
		return checkState == GRElevator.ElevatorState.DoorMovingClosing || checkState == GRElevator.ElevatorState.DoorBeginClosing || checkState == GRElevator.ElevatorState.DoorEndClosing || checkState == GRElevator.ElevatorState.DoorClosed;
	}

	// Token: 0x06003080 RID: 12416 RVA: 0x00106BC0 File Offset: 0x00104DC0
	public bool DoorIsOpening()
	{
		return GRElevator.StateIsOpeningState(this.state);
	}

	// Token: 0x06003081 RID: 12417 RVA: 0x00106BCD File Offset: 0x00104DCD
	public bool DoorIsClosing()
	{
		return GRElevator.StateIsClosingState(this.state);
	}

	// Token: 0x06003082 RID: 12418 RVA: 0x00106BDC File Offset: 0x00104DDC
	public void PhysicalElevatorUpdate()
	{
		switch (this.state)
		{
		case GRElevator.ElevatorState.DoorBeginClosing:
			if (Time.time > this.doorMoveBeginTime + this.closeBeginDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorMovingClosing);
			}
			break;
		case GRElevator.ElevatorState.DoorMovingClosing:
			if (Time.time > this.doorMoveBeginTime - this.adjustedOffsetTime + this.closeBeginDuration + this.closeTravelDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorEndClosing);
			}
			break;
		case GRElevator.ElevatorState.DoorEndClosing:
			if (Time.time > this.doorMoveBeginTime - this.adjustedOffsetTime + this.closeBeginDuration + this.closeTravelDuration + this.closeEndDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorClosed);
			}
			break;
		case GRElevator.ElevatorState.DoorBeginOpening:
			if (Time.time > this.doorMoveBeginTime + this.openBeginDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorMovingOpening);
			}
			break;
		case GRElevator.ElevatorState.DoorMovingOpening:
			if (Time.time > this.doorMoveBeginTime - this.adjustedOffsetTime + this.openBeginDuration + this.openTravelDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorEndOpening);
			}
			break;
		case GRElevator.ElevatorState.DoorEndOpening:
			if (Time.time > this.doorMoveBeginTime - this.adjustedOffsetTime + this.openBeginDuration + this.openTravelDuration + this.openEndDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorOpen);
			}
			break;
		}
		GRElevator.ElevatorState elevatorState = this.state;
		Transform transform;
		Transform transform2;
		float num;
		if (elevatorState != GRElevator.ElevatorState.DoorMovingClosing)
		{
			if (elevatorState == GRElevator.ElevatorState.DoorMovingOpening)
			{
				transform = this.openTargetTop;
				transform2 = this.openTargetBottom;
				num = this.doorOpenSpeed;
			}
			else
			{
				transform = this.upperDoor;
				transform2 = this.lowerDoor;
				num = 1f;
			}
		}
		else
		{
			transform = this.closedTargetTop;
			transform2 = this.closedTargetBottom;
			num = this.doorCloseSpeed;
		}
		this.upperDoor.position = Vector3.MoveTowards(this.upperDoor.position, transform.position, Time.deltaTime * num);
		this.lowerDoor.position = Vector3.MoveTowards(this.lowerDoor.position, transform2.position, Time.deltaTime * num);
	}

	// Token: 0x04003DE9 RID: 15849
	public GRElevatorManager.ElevatorLocation location;

	// Token: 0x04003DEA RID: 15850
	public Transform upperDoor;

	// Token: 0x04003DEB RID: 15851
	public Transform lowerDoor;

	// Token: 0x04003DEC RID: 15852
	public Transform closedTargetTop;

	// Token: 0x04003DED RID: 15853
	public Transform closedTargetBottom;

	// Token: 0x04003DEE RID: 15854
	public Transform openTargetTop;

	// Token: 0x04003DEF RID: 15855
	public Transform openTargetBottom;

	// Token: 0x04003DF0 RID: 15856
	public TextMeshPro outerText;

	// Token: 0x04003DF1 RID: 15857
	public TextMeshPro innerText;

	// Token: 0x04003DF2 RID: 15858
	public List<GRElevatorButton> elevatorButtons;

	// Token: 0x04003DF3 RID: 15859
	private Dictionary<GRElevator.ButtonType, GRElevatorButton> typeButtonDict;

	// Token: 0x04003DF4 RID: 15860
	public GorillaFriendCollider friendCollider;

	// Token: 0x04003DF5 RID: 15861
	public GorillaNetworkJoinTrigger joinTrigger;

	// Token: 0x04003DF6 RID: 15862
	public SoundBankPlayer buttonBank;

	// Token: 0x04003DF7 RID: 15863
	public AudioSource doorAudio;

	// Token: 0x04003DF8 RID: 15864
	public AudioSource ambientAudio;

	// Token: 0x04003DF9 RID: 15865
	public AudioSource musicAudio;

	// Token: 0x04003DFA RID: 15866
	public AudioClip travellingLoopClip;

	// Token: 0x04003DFB RID: 15867
	public AudioClip ambientLoopClip;

	// Token: 0x04003DFC RID: 15868
	public AudioClip dingClip;

	// Token: 0x04003DFD RID: 15869
	public AudioClip doorOpenClip;

	// Token: 0x04003DFE RID: 15870
	public AudioClip doorCloseClip;

	// Token: 0x04003DFF RID: 15871
	public float adjustedOffsetTime;

	// Token: 0x04003E00 RID: 15872
	public float doorMoveBeginTime;

	// Token: 0x04003E01 RID: 15873
	public float doorOpenSpeed = 0.5f;

	// Token: 0x04003E02 RID: 15874
	public float doorCloseSpeed = 0.5f;

	// Token: 0x04003E03 RID: 15875
	public float closeBeginDuration;

	// Token: 0x04003E04 RID: 15876
	public float closeTravelDuration;

	// Token: 0x04003E05 RID: 15877
	public float closeEndDuration;

	// Token: 0x04003E06 RID: 15878
	public float openBeginDuration;

	// Token: 0x04003E07 RID: 15879
	public float openTravelDuration;

	// Token: 0x04003E08 RID: 15880
	public float openEndDuration;

	// Token: 0x04003E09 RID: 15881
	public float travelDistance;

	// Token: 0x04003E0A RID: 15882
	public GRElevator.ElevatorState state;

	// Token: 0x04003E0B RID: 15883
	public GameObject collidersAndVisuals;

	// Token: 0x04003E0C RID: 15884
	public GameObject videoDisplay;

	// Token: 0x04003E0D RID: 15885
	public AudioSource videoAudio;

	// Token: 0x02000777 RID: 1911
	public enum ElevatorState
	{
		// Token: 0x04003E0F RID: 15887
		DoorBeginClosing,
		// Token: 0x04003E10 RID: 15888
		DoorMovingClosing,
		// Token: 0x04003E11 RID: 15889
		DoorEndClosing,
		// Token: 0x04003E12 RID: 15890
		DoorClosed,
		// Token: 0x04003E13 RID: 15891
		DoorBeginOpening,
		// Token: 0x04003E14 RID: 15892
		DoorMovingOpening,
		// Token: 0x04003E15 RID: 15893
		DoorEndOpening,
		// Token: 0x04003E16 RID: 15894
		DoorOpen,
		// Token: 0x04003E17 RID: 15895
		None
	}

	// Token: 0x02000778 RID: 1912
	[Serializable]
	public enum ButtonType
	{
		// Token: 0x04003E19 RID: 15897
		Mall = 1,
		// Token: 0x04003E1A RID: 15898
		City,
		// Token: 0x04003E1B RID: 15899
		GhostReactor,
		// Token: 0x04003E1C RID: 15900
		Open,
		// Token: 0x04003E1D RID: 15901
		Close,
		// Token: 0x04003E1E RID: 15902
		Summon,
		// Token: 0x04003E1F RID: 15903
		MonkeBlocks,
		// Token: 0x04003E20 RID: 15904
		VIMExperience1,
		// Token: 0x04003E21 RID: 15905
		VIMExperience2,
		// Token: 0x04003E22 RID: 15906
		VIMExperience3,
		// Token: 0x04003E23 RID: 15907
		VIMExperience4,
		// Token: 0x04003E24 RID: 15908
		GhostEntrance,
		// Token: 0x04003E25 RID: 15909
		Count
	}
}
