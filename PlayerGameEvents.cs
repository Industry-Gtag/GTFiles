using System;

// Token: 0x02000253 RID: 595
public class PlayerGameEvents
{
	// Token: 0x14000022 RID: 34
	// (add) Token: 0x06000FEA RID: 4074 RVA: 0x00056444 File Offset: 0x00054644
	// (remove) Token: 0x06000FEB RID: 4075 RVA: 0x00056478 File Offset: 0x00054678
	public static event Action<string> OnGameModeObjectiveTrigger;

	// Token: 0x14000023 RID: 35
	// (add) Token: 0x06000FEC RID: 4076 RVA: 0x000564AC File Offset: 0x000546AC
	// (remove) Token: 0x06000FED RID: 4077 RVA: 0x000564E0 File Offset: 0x000546E0
	public static event Action<string> OnGameModeCompleteRound;

	// Token: 0x14000024 RID: 36
	// (add) Token: 0x06000FEE RID: 4078 RVA: 0x00056514 File Offset: 0x00054714
	// (remove) Token: 0x06000FEF RID: 4079 RVA: 0x00056548 File Offset: 0x00054748
	public static event Action<string> OnGrabbedObject;

	// Token: 0x14000025 RID: 37
	// (add) Token: 0x06000FF0 RID: 4080 RVA: 0x0005657C File Offset: 0x0005477C
	// (remove) Token: 0x06000FF1 RID: 4081 RVA: 0x000565B0 File Offset: 0x000547B0
	public static event Action<string> OnDroppedObject;

	// Token: 0x14000026 RID: 38
	// (add) Token: 0x06000FF2 RID: 4082 RVA: 0x000565E4 File Offset: 0x000547E4
	// (remove) Token: 0x06000FF3 RID: 4083 RVA: 0x00056618 File Offset: 0x00054818
	public static event Action<string> OnEatObject;

	// Token: 0x14000027 RID: 39
	// (add) Token: 0x06000FF4 RID: 4084 RVA: 0x0005664C File Offset: 0x0005484C
	// (remove) Token: 0x06000FF5 RID: 4085 RVA: 0x00056680 File Offset: 0x00054880
	public static event Action<string> OnTapObject;

	// Token: 0x14000028 RID: 40
	// (add) Token: 0x06000FF6 RID: 4086 RVA: 0x000566B4 File Offset: 0x000548B4
	// (remove) Token: 0x06000FF7 RID: 4087 RVA: 0x000566E8 File Offset: 0x000548E8
	public static event Action<string> OnLaunchedProjectile;

	// Token: 0x14000029 RID: 41
	// (add) Token: 0x06000FF8 RID: 4088 RVA: 0x0005671C File Offset: 0x0005491C
	// (remove) Token: 0x06000FF9 RID: 4089 RVA: 0x00056750 File Offset: 0x00054950
	public static event Action<float, float> OnPlayerMoved;

	// Token: 0x1400002A RID: 42
	// (add) Token: 0x06000FFA RID: 4090 RVA: 0x00056784 File Offset: 0x00054984
	// (remove) Token: 0x06000FFB RID: 4091 RVA: 0x000567B8 File Offset: 0x000549B8
	public static event Action<float, float> OnPlayerSwam;

	// Token: 0x1400002B RID: 43
	// (add) Token: 0x06000FFC RID: 4092 RVA: 0x000567EC File Offset: 0x000549EC
	// (remove) Token: 0x06000FFD RID: 4093 RVA: 0x00056820 File Offset: 0x00054A20
	public static event Action<string> OnTriggerHandEffect;

	// Token: 0x1400002C RID: 44
	// (add) Token: 0x06000FFE RID: 4094 RVA: 0x00056854 File Offset: 0x00054A54
	// (remove) Token: 0x06000FFF RID: 4095 RVA: 0x00056888 File Offset: 0x00054A88
	public static event Action<string> OnEnterLocation;

	// Token: 0x1400002D RID: 45
	// (add) Token: 0x06001000 RID: 4096 RVA: 0x000568BC File Offset: 0x00054ABC
	// (remove) Token: 0x06001001 RID: 4097 RVA: 0x000568F0 File Offset: 0x00054AF0
	public static event Action<string, int> OnMiscEvent;

	// Token: 0x1400002E RID: 46
	// (add) Token: 0x06001002 RID: 4098 RVA: 0x00056924 File Offset: 0x00054B24
	// (remove) Token: 0x06001003 RID: 4099 RVA: 0x00056958 File Offset: 0x00054B58
	public static event Action<string> OnCritterEvent;

	// Token: 0x06001004 RID: 4100 RVA: 0x0005698C File Offset: 0x00054B8C
	public static void GameModeObjectiveTriggered()
	{
		string text = GorillaGameManager.instance.GameModeName();
		Action<string> onGameModeObjectiveTrigger = PlayerGameEvents.OnGameModeObjectiveTrigger;
		if (onGameModeObjectiveTrigger == null)
		{
			return;
		}
		onGameModeObjectiveTrigger(text);
	}

	// Token: 0x06001005 RID: 4101 RVA: 0x000569B4 File Offset: 0x00054BB4
	public static void GameModeCompleteRound()
	{
		string text = GorillaGameManager.instance.GameModeName();
		Action<string> onGameModeCompleteRound = PlayerGameEvents.OnGameModeCompleteRound;
		if (onGameModeCompleteRound == null)
		{
			return;
		}
		onGameModeCompleteRound(text);
	}

	// Token: 0x06001006 RID: 4102 RVA: 0x000569DC File Offset: 0x00054BDC
	public static void GrabbedObject(string objectName)
	{
		Action<string> onGrabbedObject = PlayerGameEvents.OnGrabbedObject;
		if (onGrabbedObject == null)
		{
			return;
		}
		onGrabbedObject(objectName);
	}

	// Token: 0x06001007 RID: 4103 RVA: 0x000569EE File Offset: 0x00054BEE
	public static void DroppedObject(string objectName)
	{
		Action<string> onDroppedObject = PlayerGameEvents.OnDroppedObject;
		if (onDroppedObject == null)
		{
			return;
		}
		onDroppedObject(objectName);
	}

	// Token: 0x06001008 RID: 4104 RVA: 0x00056A00 File Offset: 0x00054C00
	public static void EatObject(string objectName)
	{
		Action<string> onEatObject = PlayerGameEvents.OnEatObject;
		if (onEatObject == null)
		{
			return;
		}
		onEatObject(objectName);
	}

	// Token: 0x06001009 RID: 4105 RVA: 0x00056A12 File Offset: 0x00054C12
	public static void TapObject(string objectName)
	{
		Action<string> onTapObject = PlayerGameEvents.OnTapObject;
		if (onTapObject == null)
		{
			return;
		}
		onTapObject(objectName);
	}

	// Token: 0x0600100A RID: 4106 RVA: 0x00056A24 File Offset: 0x00054C24
	public static void LaunchedProjectile(string objectName)
	{
		Action<string> onLaunchedProjectile = PlayerGameEvents.OnLaunchedProjectile;
		if (onLaunchedProjectile == null)
		{
			return;
		}
		onLaunchedProjectile(objectName);
	}

	// Token: 0x0600100B RID: 4107 RVA: 0x00056A36 File Offset: 0x00054C36
	public static void PlayerMoved(float distance, float speed)
	{
		Action<float, float> onPlayerMoved = PlayerGameEvents.OnPlayerMoved;
		if (onPlayerMoved == null)
		{
			return;
		}
		onPlayerMoved(distance, speed);
	}

	// Token: 0x0600100C RID: 4108 RVA: 0x00056A49 File Offset: 0x00054C49
	public static void PlayerSwam(float distance, float speed)
	{
		Action<float, float> onPlayerSwam = PlayerGameEvents.OnPlayerSwam;
		if (onPlayerSwam == null)
		{
			return;
		}
		onPlayerSwam(distance, speed);
	}

	// Token: 0x0600100D RID: 4109 RVA: 0x00056A5C File Offset: 0x00054C5C
	public static void TriggerHandEffect(string effectName)
	{
		Action<string> onTriggerHandEffect = PlayerGameEvents.OnTriggerHandEffect;
		if (onTriggerHandEffect == null)
		{
			return;
		}
		onTriggerHandEffect(effectName);
	}

	// Token: 0x0600100E RID: 4110 RVA: 0x00056A6E File Offset: 0x00054C6E
	public static void TriggerEnterLocation(string locationName)
	{
		Action<string> onEnterLocation = PlayerGameEvents.OnEnterLocation;
		if (onEnterLocation == null)
		{
			return;
		}
		onEnterLocation(locationName);
	}

	// Token: 0x0600100F RID: 4111 RVA: 0x00056A80 File Offset: 0x00054C80
	public static void MiscEvent(string eventName, int count = 1)
	{
		Action<string, int> onMiscEvent = PlayerGameEvents.OnMiscEvent;
		if (onMiscEvent == null)
		{
			return;
		}
		onMiscEvent(eventName, count);
	}

	// Token: 0x06001010 RID: 4112 RVA: 0x00056A93 File Offset: 0x00054C93
	public static void CritterEvent(string eventName)
	{
		Action<string> onCritterEvent = PlayerGameEvents.OnCritterEvent;
		if (onCritterEvent == null)
		{
			return;
		}
		onCritterEvent(eventName);
	}

	// Token: 0x02000254 RID: 596
	public enum EventType
	{
		// Token: 0x04001333 RID: 4915
		NONE,
		// Token: 0x04001334 RID: 4916
		GameModeObjective,
		// Token: 0x04001335 RID: 4917
		GameModeCompleteRound,
		// Token: 0x04001336 RID: 4918
		GrabbedObject,
		// Token: 0x04001337 RID: 4919
		DroppedObject,
		// Token: 0x04001338 RID: 4920
		EatObject,
		// Token: 0x04001339 RID: 4921
		TapObject,
		// Token: 0x0400133A RID: 4922
		LaunchedProjectile,
		// Token: 0x0400133B RID: 4923
		PlayerMoved,
		// Token: 0x0400133C RID: 4924
		PlayerSwam,
		// Token: 0x0400133D RID: 4925
		TriggerHandEfffect,
		// Token: 0x0400133E RID: 4926
		EnterLocation,
		// Token: 0x0400133F RID: 4927
		MiscEvent
	}
}
