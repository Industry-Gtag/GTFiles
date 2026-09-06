using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTag;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000150 RID: 336
public class SIQuestBoard : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060008D5 RID: 2261 RVA: 0x00030370 File Offset: 0x0002E570
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		for (int i = 0; i < this.questDisplays.Count; i++)
		{
			stream.SendNext(this.questDisplays[i].activePlayerActorNumber);
		}
	}

	// Token: 0x060008D6 RID: 2262 RVA: 0x000303B0 File Offset: 0x0002E5B0
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		for (int i = 0; i < this.questDisplays.Count; i++)
		{
			this.questDisplays[i].activePlayerActorNumber = (int)stream.ReceiveNext();
		}
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x000303EF File Offset: 0x0002E5EF
	public void GrantBonusPointProgress()
	{
		if (!this.bounds.Contains(GTPlayer.Instance.HeadCenterPosition))
		{
			return;
		}
		SIPlayer.LocalPlayer.GetBonusProgress(this.superInfection.siManager);
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x00030420 File Offset: 0x0002E620
	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (this.superInfection.siManager.gameEntityManager.IsAuthority())
		{
			this.AuthorityUpdateScreenAssignments();
		}
		DateTime utcNow = DateTime.UtcNow;
		DateTime dateTime = utcNow.Date + SIProgression.Instance.CROSSOVER_TIME_OF_DAY;
		if (dateTime < utcNow)
		{
			dateTime = dateTime.AddDays(1.0);
		}
		TimeSpan timeSpan = dateTime - utcNow;
		GTTime.TryUpdateTimeText(this.timeToNewQuests, timeSpan, SIQuestBoard._timeToNewQuests_chars, 15, ref SIQuestBoard._lastTotalSeconds);
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x000304A4 File Offset: 0x0002E6A4
	private void AuthorityUpdateScreenAssignments()
	{
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			list.Add(allNetPlayers[i].ActorNumber);
		}
		for (int j = 0; j < this.questDisplays.Count; j++)
		{
			int activePlayerActorNumber = this.questDisplays[j].activePlayerActorNumber;
			if (activePlayerActorNumber != -1)
			{
				if (!list.Contains(activePlayerActorNumber))
				{
					this.questDisplays[j].activePlayerActorNumber = -1;
				}
				else if (!list2.Contains(activePlayerActorNumber))
				{
					list2.Add(activePlayerActorNumber);
				}
			}
		}
		for (int k = 0; k < allNetPlayers.Length; k++)
		{
			int actorNumber = allNetPlayers[k].ActorNumber;
			if (!list2.Contains(actorNumber))
			{
				for (int l = 0; l < this.questDisplays.Count; l++)
				{
					if (this.questDisplays[l].activePlayerActorNumber == -1)
					{
						this.questDisplays[l].activePlayerActorNumber = actorNumber;
						break;
					}
				}
			}
		}
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x000305B4 File Offset: 0x0002E7B4
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (this.bonusPointArea.gameObject.activeSelf)
		{
			this.bounds = this.bonusPointArea.bounds;
			this.bonusPointArea.gameObject.SetActive(false);
		}
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x00019269 File Offset: 0x00017469
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void ForceCompleteQuest(int index)
	{
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void CheatAddPoints(int points)
	{
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void CheatAddBonusPoints(int points)
	{
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x000305F4 File Offset: 0x0002E7F4
	public void CheatRoomFXDurationPlus()
	{
		if (this.currentDuration < SIQuestBoard.RoomFXDurationState._120seconds)
		{
			this.currentDuration++;
		}
		this.RoomFXDurationReadout.text = string.Format("{0}secs", this.roomFXDurations[this.currentDuration]);
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x00030644 File Offset: 0x0002E844
	public void CheatRoomFXDurationMinus()
	{
		if (this.currentDuration > SIQuestBoard.RoomFXDurationState._15seconds)
		{
			this.currentDuration--;
		}
		this.RoomFXDurationReadout.text = string.Format("{0}secs", this.roomFXDurations[this.currentDuration]);
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x00030693 File Offset: 0x0002E893
	public void CheatRoomFX_Underwater()
	{
		this.StartRoomFX(SuperInfectionManager.RoomFXType.Underwater, this.roomFXDurations[this.currentDuration]);
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x000306AD File Offset: 0x0002E8AD
	public void CheatRoomFX_LunarMode()
	{
		this.StartRoomFX(SuperInfectionManager.RoomFXType.LunarMode, this.roomFXDurations[this.currentDuration]);
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x000306C7 File Offset: 0x0002E8C7
	public void CheatRoomFX_ConstLowG()
	{
		this.StartRoomFX(SuperInfectionManager.RoomFXType.ConstLowG, this.roomFXDurations[this.currentDuration]);
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x000306E1 File Offset: 0x0002E8E1
	public void CheatRoomFX_Bouncy()
	{
		this.StartRoomFX(SuperInfectionManager.RoomFXType.Bouncy, this.roomFXDurations[this.currentDuration]);
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x000306FB File Offset: 0x0002E8FB
	public void CheatRoomFX_Supercharge()
	{
		this.StartRoomFX(SuperInfectionManager.RoomFXType.Supercharge, this.roomFXDurations[this.currentDuration]);
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void StartRoomFX(SuperInfectionManager.RoomFXType fxType, float duration)
	{
	}

	// Token: 0x04000AF1 RID: 2801
	public SuperInfection superInfection;

	// Token: 0x04000AF2 RID: 2802
	public List<SIUIPlayerQuestDisplay> questDisplays;

	// Token: 0x04000AF3 RID: 2803
	public BoxCollider bonusPointArea;

	// Token: 0x04000AF4 RID: 2804
	public Bounds bounds;

	// Token: 0x04000AF5 RID: 2805
	public ParticleSystem celebrateParticle;

	// Token: 0x04000AF6 RID: 2806
	public TextMeshProUGUI timeToNewQuests;

	// Token: 0x04000AF7 RID: 2807
	private static readonly char[] _timeToNewQuests_chars = "NEW QUESTS IN: ??:??:??".ToCharArray();

	// Token: 0x04000AF8 RID: 2808
	private const int _timeToNewQuests_index = 15;

	// Token: 0x04000AF9 RID: 2809
	private static int _lastTotalSeconds;

	// Token: 0x04000AFA RID: 2810
	private Dictionary<SIQuestBoard.RoomFXDurationState, float> roomFXDurations = new Dictionary<SIQuestBoard.RoomFXDurationState, float>
	{
		{
			SIQuestBoard.RoomFXDurationState._15seconds,
			15f
		},
		{
			SIQuestBoard.RoomFXDurationState._30seconds,
			30f
		},
		{
			SIQuestBoard.RoomFXDurationState._60seconds,
			60f
		},
		{
			SIQuestBoard.RoomFXDurationState._90seconds,
			90f
		},
		{
			SIQuestBoard.RoomFXDurationState._120seconds,
			120f
		}
	};

	// Token: 0x04000AFB RID: 2811
	private SIQuestBoard.RoomFXDurationState currentDuration = SIQuestBoard.RoomFXDurationState._30seconds;

	// Token: 0x04000AFC RID: 2812
	[SerializeField]
	private TextMeshPro RoomFXDurationReadout;

	// Token: 0x02000151 RID: 337
	private enum RoomFXDurationState
	{
		// Token: 0x04000AFE RID: 2814
		_15seconds,
		// Token: 0x04000AFF RID: 2815
		_30seconds,
		// Token: 0x04000B00 RID: 2816
		_60seconds,
		// Token: 0x04000B01 RID: 2817
		_90seconds,
		// Token: 0x04000B02 RID: 2818
		_120seconds
	}
}
