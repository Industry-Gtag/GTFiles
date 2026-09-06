using System;
using System.Collections.Generic;
using CjLib;
using Unity.Collections;
using UnityEngine;

// Token: 0x020007C9 RID: 1993
public class GRNoiseEventManager : MonoBehaviourTick
{
	// Token: 0x060032C5 RID: 12997 RVA: 0x0011634C File Offset: 0x0011454C
	public void Awake()
	{
		GRNoiseEventManager.instance = this;
	}

	// Token: 0x060032C6 RID: 12998 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Start()
	{
	}

	// Token: 0x060032C7 RID: 12999 RVA: 0x00116354 File Offset: 0x00114554
	public override void Tick()
	{
		this.RemoveExpiredEvents();
		if (GhostReactorManager.noiseDebugEnabled)
		{
			this.RenderDebug();
		}
	}

	// Token: 0x060032C8 RID: 13000 RVA: 0x00116369 File Offset: 0x00114569
	private int FindUnusedEventEntry()
	{
		return -1;
	}

	// Token: 0x060032C9 RID: 13001 RVA: 0x0011636C File Offset: 0x0011456C
	public void AddNoiseEvent(Vector3 position, float magnitude = 1f, float duration = 1f)
	{
		GameNoiseEvent gameNoiseEvent = new GameNoiseEvent
		{
			position = position,
			eventTime = Time.timeAsDouble,
			duration = duration,
			magnitude = magnitude
		};
		int num = this.FindUnusedEventEntry();
		if (num == -1)
		{
			this.noiseEvents.Add(gameNoiseEvent);
			return;
		}
		this.noiseEvents[num] = gameNoiseEvent;
	}

	// Token: 0x060032CA RID: 13002 RVA: 0x001163CC File Offset: 0x001145CC
	public List<GameNoiseEvent> GetNoiseEventsInRadius(Vector3 origin, float radius)
	{
		List<GameNoiseEvent> list = new List<GameNoiseEvent>();
		float num = radius * radius;
		foreach (GameNoiseEvent gameNoiseEvent in this.noiseEvents)
		{
			if (gameNoiseEvent.IsValid())
			{
				float sqrMagnitude = (gameNoiseEvent.position - origin).sqrMagnitude;
				float num2 = gameNoiseEvent.magnitude * gameNoiseEvent.magnitude;
				if (sqrMagnitude < num * num2)
				{
					list.Add(gameNoiseEvent);
				}
			}
		}
		return list;
	}

	// Token: 0x060032CB RID: 13003 RVA: 0x00116460 File Offset: 0x00114660
	public bool GetMostRecentNoiseEventInRadius(Vector3 origin, float radius, out GameNoiseEvent outEvent)
	{
		double timeAsDouble = Time.timeAsDouble;
		float num = radius * radius;
		double num2 = -1.0;
		int num3 = -1;
		for (int i = 0; i < this.noiseEvents.Count; i++)
		{
			GameNoiseEvent gameNoiseEvent = this.noiseEvents[i];
			if (gameNoiseEvent.IsValid())
			{
				float sqrMagnitude = (gameNoiseEvent.position - origin).sqrMagnitude;
				float num4 = gameNoiseEvent.magnitude * gameNoiseEvent.magnitude;
				if (sqrMagnitude < num * num4)
				{
					double num5 = timeAsDouble - gameNoiseEvent.eventTime;
					if (num3 < 0 || num5 < num2)
					{
						num3 = i;
						num2 = num5;
					}
				}
			}
		}
		if (num3 < 0)
		{
			outEvent = default(GameNoiseEvent);
			return false;
		}
		outEvent = this.noiseEvents[num3];
		return true;
	}

	// Token: 0x060032CC RID: 13004 RVA: 0x0011651C File Offset: 0x0011471C
	public void RenderDebug()
	{
		int num = 0;
		float num2 = 5f;
		for (int i = 0; i < this.noiseEvents.Count; i++)
		{
			GameNoiseEvent gameNoiseEvent = this.noiseEvents[i];
			if (gameNoiseEvent.IsValid())
			{
				float num3 = this.debugMeshScale * gameNoiseEvent.magnitude * num2;
				DebugUtil.DrawSphere(gameNoiseEvent.position, num3, 8, 6, Color.green, true, DebugUtil.Style.Wireframe);
				num++;
			}
		}
	}

	// Token: 0x060032CD RID: 13005 RVA: 0x00116588 File Offset: 0x00114788
	private void RemoveExpiredEvents()
	{
		for (int i = 0; i < this.noiseEvents.Count; i++)
		{
			if (!this.noiseEvents[i].IsValid())
			{
				this.noiseEvents.RemoveAtSwapBack(i);
				i--;
			}
		}
	}

	// Token: 0x040041D8 RID: 16856
	private List<GameNoiseEvent> noiseEvents = new List<GameNoiseEvent>();

	// Token: 0x040041D9 RID: 16857
	public static GRNoiseEventManager instance;

	// Token: 0x040041DA RID: 16858
	public float debugMeshScale = 1f;
}
