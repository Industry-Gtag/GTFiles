using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000251 RID: 593
public class PlayerGameEventListener : MonoBehaviour
{
	// Token: 0x06000FE0 RID: 4064 RVA: 0x0005610E File Offset: 0x0005430E
	private void OnEnable()
	{
		this.SubscribeToEvents();
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x00056116 File Offset: 0x00054316
	private void OnDisable()
	{
		this.UnsubscribeFromEvents();
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x00056120 File Offset: 0x00054320
	private void SubscribeToEvents()
	{
		switch (this.eventType)
		{
		case PlayerGameEvents.EventType.NONE:
			return;
		case PlayerGameEvents.EventType.GameModeObjective:
			PlayerGameEvents.OnGameModeObjectiveTrigger += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.GameModeCompleteRound:
			PlayerGameEvents.OnGameModeCompleteRound += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.GrabbedObject:
			PlayerGameEvents.OnGrabbedObject += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.DroppedObject:
			PlayerGameEvents.OnDroppedObject += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.EatObject:
			PlayerGameEvents.OnEatObject += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.TapObject:
			PlayerGameEvents.OnTapObject += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.LaunchedProjectile:
			PlayerGameEvents.OnLaunchedProjectile += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.PlayerMoved:
			PlayerGameEvents.OnPlayerMoved += this.OnGameMoveEventTriggered;
			return;
		case PlayerGameEvents.EventType.PlayerSwam:
			PlayerGameEvents.OnPlayerSwam += this.OnGameMoveEventTriggered;
			return;
		case PlayerGameEvents.EventType.TriggerHandEfffect:
			PlayerGameEvents.OnTriggerHandEffect += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.EnterLocation:
			PlayerGameEvents.OnEnterLocation += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.MiscEvent:
			PlayerGameEvents.OnMiscEvent += this.OnGameEventTriggered;
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x00056254 File Offset: 0x00054454
	private void UnsubscribeFromEvents()
	{
		switch (this.eventType)
		{
		case PlayerGameEvents.EventType.NONE:
			return;
		case PlayerGameEvents.EventType.GameModeObjective:
			PlayerGameEvents.OnGameModeObjectiveTrigger -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.GameModeCompleteRound:
			PlayerGameEvents.OnGameModeCompleteRound -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.GrabbedObject:
			PlayerGameEvents.OnGrabbedObject -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.DroppedObject:
			PlayerGameEvents.OnDroppedObject -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.EatObject:
			PlayerGameEvents.OnEatObject -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.TapObject:
			PlayerGameEvents.OnTapObject -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.LaunchedProjectile:
			PlayerGameEvents.OnLaunchedProjectile -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.PlayerMoved:
			PlayerGameEvents.OnPlayerMoved -= this.OnGameMoveEventTriggered;
			return;
		case PlayerGameEvents.EventType.PlayerSwam:
			PlayerGameEvents.OnPlayerSwam -= this.OnGameMoveEventTriggered;
			return;
		case PlayerGameEvents.EventType.TriggerHandEfffect:
			PlayerGameEvents.OnTriggerHandEffect -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.EnterLocation:
			PlayerGameEvents.OnEnterLocation -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.MiscEvent:
			PlayerGameEvents.OnMiscEvent -= this.OnGameEventTriggered;
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06000FE4 RID: 4068 RVA: 0x00056385 File Offset: 0x00054585
	private void OnGameMoveEventTriggered(float distance, float speed)
	{
		Debug.LogError("Movement events not supported - please implement");
	}

	// Token: 0x06000FE5 RID: 4069 RVA: 0x00056391 File Offset: 0x00054591
	public void OnGameEventTriggered(string eventName)
	{
		this.OnGameEventTriggered(eventName, 1);
	}

	// Token: 0x06000FE6 RID: 4070 RVA: 0x0005639C File Offset: 0x0005459C
	public void OnGameEventTriggered(string eventName, int count)
	{
		if (!string.IsNullOrEmpty(this.filter) && !eventName.StartsWith(this.filter))
		{
			return;
		}
		if (this._cooldownEnd > Time.time)
		{
			return;
		}
		this._cooldownEnd = Time.time + this.cooldown;
		UnityEvent unityEvent = this.onGameEvent;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		UnityEvent<int> unityEvent2 = this.onGameEventCounted;
		if (unityEvent2 == null)
		{
			return;
		}
		unityEvent2.Invoke(count);
	}

	// Token: 0x0400131E RID: 4894
	[SerializeField]
	private PlayerGameEvents.EventType eventType;

	// Token: 0x0400131F RID: 4895
	[Tooltip("Cooldown in seconds")]
	[SerializeField]
	private string filter;

	// Token: 0x04001320 RID: 4896
	[SerializeField]
	private float cooldown = 1f;

	// Token: 0x04001321 RID: 4897
	[SerializeField]
	private UnityEvent onGameEvent;

	// Token: 0x04001322 RID: 4898
	[SerializeField]
	private UnityEvent<int> onGameEventCounted;

	// Token: 0x04001323 RID: 4899
	private float _cooldownEnd;
}
