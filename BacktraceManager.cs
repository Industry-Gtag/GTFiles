using System;
using System.Globalization;
using Backtrace.Unity;
using Backtrace.Unity.Model;
using GorillaNetworking;
using PlayFab;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000D82 RID: 3458
public class BacktraceManager : MonoBehaviour
{
	// Token: 0x06005539 RID: 21817 RVA: 0x001BE792 File Offset: 0x001BC992
	public virtual void Awake()
	{
		base.GetComponent<BacktraceClient>().BeforeSend = delegate(BacktraceData data)
		{
			if (new Unity.Mathematics.Random((uint)(Time.realtimeSinceStartupAsDouble * 1000.0)).NextDouble() > this.backtraceSampleRate)
			{
				return null;
			}
			return data;
		};
	}

	// Token: 0x0600553A RID: 21818 RVA: 0x001BE7AB File Offset: 0x001BC9AB
	private void Start()
	{
		PlayFabTitleDataCache.Instance.GetTitleData("BacktraceSampleRate", delegate(string data)
		{
			if (data != null)
			{
				double.TryParse(data.Trim('"'), NumberStyles.Any, CultureInfo.InvariantCulture, out this.backtraceSampleRate);
				Debug.Log(string.Format("Set backtrace sample rate to: {0}", this.backtraceSampleRate));
			}
		}, delegate(PlayFabError e)
		{
			Debug.LogError(string.Format("Error getting Backtrace sample rate: {0}", e));
		}, false);
	}

	// Token: 0x040066CC RID: 26316
	public double backtraceSampleRate = 0.01;
}
