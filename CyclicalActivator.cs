using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000095 RID: 149
public class CyclicalActivator : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060003A9 RID: 937 RVA: 0x0001212B File Offset: 0x0001032B
	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060003AA RID: 938 RVA: 0x00012134 File Offset: 0x00010334
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060003AB RID: 939 RVA: 0x00015224 File Offset: 0x00013424
	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (GorillaComputer.instance == null || GorillaComputer.instance.GetServerTime().Year < 2000)
		{
			return;
		}
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		float num = (float)(serverTime.Minute * 60) + ((float)serverTime.Second + (float)serverTime.Millisecond * 0.001f);
		if (num == this.previousS)
		{
			return;
		}
		for (int i = 0; i < this.objects.Length; i++)
		{
			this.objects[i].gameObject.SetActive(this.objects[i].schedule.CheckTime(num));
		}
		this.previousS = num;
	}

	// Token: 0x0400042F RID: 1071
	[SerializeField]
	private CyclicalActivator.CyclicalActivatorObject[] objects;

	// Token: 0x04000430 RID: 1072
	private float previousS = -1f;

	// Token: 0x02000096 RID: 150
	[Serializable]
	private class CyclicalActivatorObjectScheduleNode
	{
		// Token: 0x04000431 RID: 1073
		public Vector2 secondsActiveRange;
	}

	// Token: 0x02000097 RID: 151
	[Serializable]
	private class CyclicalActivatorObjectSchedule
	{
		// Token: 0x060003AE RID: 942 RVA: 0x000152EC File Offset: 0x000134EC
		public bool CheckTime(float nowSeconds)
		{
			nowSeconds %= (float)this.totalSeconds;
			for (int i = 0; i < this.schedule.Length; i++)
			{
				if (this.schedule[i].secondsActiveRange.x <= nowSeconds && this.schedule[i].secondsActiveRange.y > nowSeconds)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000432 RID: 1074
		[Range(10f, 3599f)]
		public int totalSeconds = 60;

		// Token: 0x04000433 RID: 1075
		public CyclicalActivator.CyclicalActivatorObjectScheduleNode[] schedule;
	}

	// Token: 0x02000098 RID: 152
	[Serializable]
	private class CyclicalActivatorObject
	{
		// Token: 0x04000434 RID: 1076
		public GameObject gameObject;

		// Token: 0x04000435 RID: 1077
		public CyclicalActivator.CyclicalActivatorObjectSchedule schedule;
	}
}
