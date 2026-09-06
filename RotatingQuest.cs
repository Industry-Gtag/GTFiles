using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

// Token: 0x02000269 RID: 617
[Serializable]
public class RotatingQuest
{
	// Token: 0x1700019F RID: 415
	// (get) Token: 0x0600107D RID: 4221 RVA: 0x0005818E File Offset: 0x0005638E
	[JsonIgnore]
	public bool IsMovementQuest
	{
		get
		{
			return this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance;
		}
	}

	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x0600107E RID: 4222 RVA: 0x000581A5 File Offset: 0x000563A5
	// (set) Token: 0x0600107F RID: 4223 RVA: 0x000581AD File Offset: 0x000563AD
	[JsonIgnore]
	public GTZone RequiredZone { get; private set; } = GTZone.none;

	// Token: 0x06001080 RID: 4224 RVA: 0x000581B6 File Offset: 0x000563B6
	public void SetRequiredZone()
	{
		this.RequiredZone = ((this.requiredZones.Count > 0) ? this.requiredZones[Random.Range(0, this.requiredZones.Count)] : GTZone.none);
	}

	// Token: 0x06001081 RID: 4225 RVA: 0x000581EC File Offset: 0x000563EC
	public void AddEventListener()
	{
		if (this.isQuestComplete)
		{
			return;
		}
		switch (this.questType)
		{
		case QuestType.gameModeObjective:
			PlayerGameEvents.OnGameModeObjectiveTrigger += this.OnGameEventOccurence;
			return;
		case QuestType.gameModeRound:
			PlayerGameEvents.OnGameModeCompleteRound += this.OnGameEventOccurence;
			return;
		case QuestType.grabObject:
			PlayerGameEvents.OnGrabbedObject += this.OnGameEventOccurence;
			return;
		case QuestType.dropObject:
			PlayerGameEvents.OnDroppedObject += this.OnGameEventOccurence;
			return;
		case QuestType.eatObject:
			PlayerGameEvents.OnEatObject += this.OnGameEventOccurence;
			return;
		case QuestType.tapObject:
			PlayerGameEvents.OnTapObject += this.OnGameEventOccurence;
			return;
		case QuestType.launchedProjectile:
			PlayerGameEvents.OnLaunchedProjectile += this.OnGameEventOccurence;
			return;
		case QuestType.moveDistance:
			PlayerGameEvents.OnPlayerMoved += this.OnGameMoveEvent;
			return;
		case QuestType.swimDistance:
			PlayerGameEvents.OnPlayerSwam += this.OnGameMoveEvent;
			return;
		case QuestType.triggerHandEffect:
			PlayerGameEvents.OnTriggerHandEffect += this.OnGameEventOccurence;
			return;
		case QuestType.enterLocation:
			PlayerGameEvents.OnEnterLocation += this.OnGameEventOccurence;
			return;
		case QuestType.misc:
			PlayerGameEvents.OnMiscEvent += this.OnGameEventOccurence;
			return;
		case QuestType.critter:
			PlayerGameEvents.OnCritterEvent += this.OnGameEventOccurence;
			return;
		default:
			return;
		}
	}

	// Token: 0x06001082 RID: 4226 RVA: 0x00058330 File Offset: 0x00056530
	public void RemoveEventListener()
	{
		switch (this.questType)
		{
		case QuestType.gameModeObjective:
			PlayerGameEvents.OnGameModeObjectiveTrigger -= this.OnGameEventOccurence;
			return;
		case QuestType.gameModeRound:
			PlayerGameEvents.OnGameModeCompleteRound -= this.OnGameEventOccurence;
			return;
		case QuestType.grabObject:
			PlayerGameEvents.OnGrabbedObject -= this.OnGameEventOccurence;
			return;
		case QuestType.dropObject:
			PlayerGameEvents.OnDroppedObject -= this.OnGameEventOccurence;
			return;
		case QuestType.eatObject:
			PlayerGameEvents.OnEatObject -= this.OnGameEventOccurence;
			return;
		case QuestType.tapObject:
			PlayerGameEvents.OnTapObject -= this.OnGameEventOccurence;
			return;
		case QuestType.launchedProjectile:
			PlayerGameEvents.OnLaunchedProjectile -= this.OnGameEventOccurence;
			return;
		case QuestType.moveDistance:
			PlayerGameEvents.OnPlayerMoved -= this.OnGameMoveEvent;
			return;
		case QuestType.swimDistance:
			PlayerGameEvents.OnPlayerSwam -= this.OnGameMoveEvent;
			return;
		case QuestType.triggerHandEffect:
			PlayerGameEvents.OnTriggerHandEffect -= this.OnGameEventOccurence;
			return;
		case QuestType.enterLocation:
			PlayerGameEvents.OnEnterLocation -= this.OnGameEventOccurence;
			return;
		case QuestType.misc:
			PlayerGameEvents.OnMiscEvent -= this.OnGameEventOccurence;
			return;
		case QuestType.critter:
			PlayerGameEvents.OnCritterEvent -= this.OnGameEventOccurence;
			return;
		default:
			return;
		}
	}

	// Token: 0x06001083 RID: 4227 RVA: 0x0005846C File Offset: 0x0005666C
	public void ApplySavedProgress(int progress)
	{
		if (this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance)
		{
			this.moveDistance = (float)progress;
			this.occurenceCount = Mathf.FloorToInt(this.moveDistance);
			this.isQuestComplete = this.occurenceCount >= this.requiredOccurenceCount;
			return;
		}
		this.occurenceCount = progress;
		this.isQuestComplete = this.occurenceCount >= this.requiredOccurenceCount;
	}

	// Token: 0x06001084 RID: 4228 RVA: 0x000584DB File Offset: 0x000566DB
	public int GetProgress()
	{
		if (this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance)
		{
			return Mathf.FloorToInt(this.moveDistance);
		}
		return this.occurenceCount;
	}

	// Token: 0x06001085 RID: 4229 RVA: 0x00058502 File Offset: 0x00056702
	private void OnGameEventOccurence(string eventName)
	{
		this.OnGameEventOccurence(eventName, 1);
	}

	// Token: 0x06001086 RID: 4230 RVA: 0x0005850C File Offset: 0x0005670C
	private void OnGameEventOccurence(string eventName, int count)
	{
		if (this.RequiredZone != GTZone.none && !ZoneManagement.IsInZone(this.RequiredZone))
		{
			return;
		}
		string.IsNullOrEmpty(this.questOccurenceFilter);
		if (eventName.StartsWith(this.questOccurenceFilter))
		{
			this.SetProgress(this.occurenceCount + count);
		}
	}

	// Token: 0x06001087 RID: 4231 RVA: 0x0005855C File Offset: 0x0005675C
	private void OnGameMoveEvent(float distance, float speed)
	{
		if (this.RequiredZone != GTZone.none && !ZoneManagement.IsInZone(this.RequiredZone))
		{
			return;
		}
		if (!(this.questOccurenceFilter == "maxSpeed"))
		{
			this.moveDistance += distance;
			this.SetProgress(Mathf.FloorToInt(this.moveDistance));
			return;
		}
		if (speed <= this.moveDistance)
		{
			return;
		}
		this.moveDistance = speed;
		this.SetProgress(Mathf.FloorToInt(this.moveDistance));
	}

	// Token: 0x06001088 RID: 4232 RVA: 0x000585D8 File Offset: 0x000567D8
	private void SetProgress(int progress)
	{
		if (this.isQuestComplete)
		{
			return;
		}
		if (this.occurenceCount == progress)
		{
			return;
		}
		this.lastChange = Time.frameCount;
		this.occurenceCount = progress;
		if (this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance)
		{
			this.moveDistance = (float)progress;
		}
		if (this.occurenceCount >= this.requiredOccurenceCount)
		{
			this.Complete();
		}
		this.questManager.HandleQuestProgressChanged(false);
	}

	// Token: 0x06001089 RID: 4233 RVA: 0x00058645 File Offset: 0x00056845
	private void Complete()
	{
		if (this.isQuestComplete)
		{
			return;
		}
		this.isQuestComplete = true;
		this.RemoveEventListener();
		this.questManager.HandleQuestCompleted(this.questID);
	}

	// Token: 0x0600108A RID: 4234 RVA: 0x0005866E File Offset: 0x0005686E
	public string GetTextDescription()
	{
		return this.<GetTextDescription>g__GetActionName|32_0().ToUpper() + this.<GetTextDescription>g__GetLocationText|32_1().ToUpper();
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x0005868B File Offset: 0x0005688B
	public string GetProgressText()
	{
		if (!this.isQuestComplete)
		{
			return string.Format("{0}/{1}", this.occurenceCount, this.requiredOccurenceCount);
		}
		return "[DONE]";
	}

	// Token: 0x0600108D RID: 4237 RVA: 0x000586E8 File Offset: 0x000568E8
	[CompilerGenerated]
	private string <GetTextDescription>g__GetActionName|32_0()
	{
		switch (this.questType)
		{
		case QuestType.none:
			return "[UNDEFINED]";
		case QuestType.gameModeObjective:
			return this.questName;
		case QuestType.gameModeRound:
			return this.questName;
		case QuestType.grabObject:
			return this.questName;
		case QuestType.dropObject:
			return this.questName;
		case QuestType.eatObject:
			return this.questName;
		case QuestType.launchedProjectile:
			return this.questName;
		case QuestType.moveDistance:
			return this.questName;
		case QuestType.swimDistance:
			return this.questName;
		case QuestType.triggerHandEffect:
			return this.questName;
		case QuestType.enterLocation:
			return this.questName;
		case QuestType.misc:
			return this.questName;
		}
		return this.questName;
	}

	// Token: 0x0600108E RID: 4238 RVA: 0x000587AB File Offset: 0x000569AB
	[CompilerGenerated]
	private string <GetTextDescription>g__GetLocationText|32_1()
	{
		if (this.RequiredZone == GTZone.none)
		{
			return "";
		}
		return string.Format(" IN {0}", this.RequiredZone);
	}

	// Token: 0x040013B8 RID: 5048
	public bool disable;

	// Token: 0x040013B9 RID: 5049
	public int questID;

	// Token: 0x040013BA RID: 5050
	public float weight = 1f;

	// Token: 0x040013BB RID: 5051
	public QuestCategory category;

	// Token: 0x040013BC RID: 5052
	public string questName = "UNNAMED QUEST";

	// Token: 0x040013BD RID: 5053
	public QuestType questType;

	// Token: 0x040013BE RID: 5054
	public string questOccurenceFilter;

	// Token: 0x040013BF RID: 5055
	public int requiredOccurenceCount = 1;

	// Token: 0x040013C0 RID: 5056
	[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
	public List<GTZone> requiredZones;

	// Token: 0x040013C1 RID: 5057
	[Space]
	[NonSerialized]
	public bool isQuestActive;

	// Token: 0x040013C2 RID: 5058
	[NonSerialized]
	public bool isQuestComplete;

	// Token: 0x040013C3 RID: 5059
	[NonSerialized]
	public bool isDailyQuest;

	// Token: 0x040013C4 RID: 5060
	[NonSerialized]
	public int lastChange;

	// Token: 0x040013C6 RID: 5062
	[NonSerialized]
	public int occurenceCount;

	// Token: 0x040013C7 RID: 5063
	private float moveDistance;

	// Token: 0x040013C8 RID: 5064
	[NonSerialized]
	public GorillaQuestManager questManager;
}
