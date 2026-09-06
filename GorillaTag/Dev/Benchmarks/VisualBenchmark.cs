using System;
using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

namespace GorillaTag.Dev.Benchmarks
{
	// Token: 0x02001274 RID: 4724
	public class VisualBenchmark : MonoBehaviour
	{
		// Token: 0x0600775C RID: 30556 RVA: 0x0026AC84 File Offset: 0x00268E84
		protected void Awake()
		{
			Application.quitting += delegate
			{
				VisualBenchmark.isQuitting = true;
			};
			List<ProfilerRecorderHandle> list = new List<ProfilerRecorderHandle>(5500);
			ProfilerRecorderHandle.GetAvailable(list);
			List<VisualBenchmark.StatInfo> list2 = new List<VisualBenchmark.StatInfo>(600);
			foreach (ProfilerRecorderHandle profilerRecorderHandle in list)
			{
				ProfilerRecorderDescription description = ProfilerRecorderHandle.GetDescription(profilerRecorderHandle);
				if (description.Category == ProfilerCategory.Render)
				{
					list2.Add(new VisualBenchmark.StatInfo
					{
						name = description.Name,
						unit = description.UnitType
					});
				}
			}
			this.availableRenderStats = list2.ToArray();
			List<Transform> list3 = new List<Transform>(this.benchmarkLocations.Length);
			foreach (Transform transform in this.benchmarkLocations)
			{
				if (transform != null)
				{
					list3.Add(transform);
				}
			}
			this.benchmarkLocations = list3.ToArray();
		}

		// Token: 0x0600775D RID: 30557 RVA: 0x0026ADAC File Offset: 0x00268FAC
		protected void OnEnable()
		{
			this.renderStatsRecorders = new ProfilerRecorder[this.availableRenderStats.Length];
			for (int i = 0; i < this.availableRenderStats.Length; i++)
			{
				this.renderStatsRecorders[i] = ProfilerRecorder.StartNew(ProfilerCategory.Render, this.availableRenderStats[i].name, 1, ProfilerRecorderOptions.Default);
			}
			this.state = VisualBenchmark.EState.Setup;
		}

		// Token: 0x0600775E RID: 30558 RVA: 0x0026AE10 File Offset: 0x00269010
		protected void OnDisable()
		{
			foreach (ProfilerRecorder profilerRecorder in this.renderStatsRecorders)
			{
				profilerRecorder.Dispose();
			}
		}

		// Token: 0x0600775F RID: 30559 RVA: 0x0026AE44 File Offset: 0x00269044
		protected void LateUpdate()
		{
			if (VisualBenchmark.isQuitting)
			{
				return;
			}
			switch (this.state)
			{
			case VisualBenchmark.EState.Setup:
				this.sb.Clear();
				this.currentLocationIndex = 0;
				this.lastTime = Time.realtimeSinceStartup;
				this.state = VisualBenchmark.EState.WaitingBeforeCollectingGarbage;
				return;
			case VisualBenchmark.EState.WaitingBeforeCollectingGarbage:
				if (Time.realtimeSinceStartup - this.lastTime >= this.collectGarbageDelay)
				{
					this.lastTime = Time.time;
					GC.Collect();
					this.state = VisualBenchmark.EState.WaitingBeforeRecordingStats;
					return;
				}
				break;
			case VisualBenchmark.EState.WaitingBeforeRecordingStats:
				if (Time.time - this.lastTime >= this.recordStatsDelay)
				{
					this.lastTime = Time.time;
					this.RecordLocationStats(this.benchmarkLocations[this.currentLocationIndex]);
					if (this.currentLocationIndex < this.benchmarkLocations.Length - 1)
					{
						this.currentLocationIndex++;
						this.state = VisualBenchmark.EState.WaitingBeforeCollectingGarbage;
						return;
					}
					this.state = VisualBenchmark.EState.TearDown;
					return;
				}
				break;
			case VisualBenchmark.EState.TearDown:
				Debug.Log(this.sb.ToString());
				this.state = VisualBenchmark.EState.Setup;
				if (this.sb.Length > this.sb.Capacity)
				{
					Debug.Log("Capacity exceeded on string builder, increase string builder's capacity. " + string.Format("capacity={0}, length={1}", this.sb.Capacity, this.sb.Length), this);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06007760 RID: 30560 RVA: 0x0026AF9C File Offset: 0x0026919C
		private void RecordLocationStats(Transform xform)
		{
			this.sb.Append("Location: ");
			this.sb.Append(xform.name);
			this.sb.Append("\n");
			this.sb.Append("pos=");
			this.sb.Append(xform.position.ToString("F3"));
			this.sb.Append(" rot=");
			this.sb.Append(xform.rotation.ToString("F3"));
			this.sb.Append(" scale=");
			this.sb.Append(xform.lossyScale.ToString("F3"));
			this.sb.Append("\n");
			for (int i = 0; i < this.renderStatsRecorders.Length; i++)
			{
				this.sb.Append(this.availableRenderStats[i].name);
				this.sb.Append(": ");
				ProfilerMarkerDataUnit unit = this.availableRenderStats[i].unit;
				if (unit != ProfilerMarkerDataUnit.TimeNanoseconds)
				{
					if (unit == ProfilerMarkerDataUnit.Bytes)
					{
						this.sb.Append((double)this.renderStatsRecorders[i].LastValue / 1024.0);
						this.sb.Append("kb");
					}
					else
					{
						this.sb.Append(this.renderStatsRecorders[i].LastValue);
						this.sb.Append(' ');
						this.sb.Append(this.availableRenderStats[i].unit.ToString());
					}
				}
				else
				{
					this.sb.Append((double)this.renderStatsRecorders[i].LastValue / 1000000.0);
					this.sb.Append("ms");
				}
				this.sb.Append('\n');
			}
		}

		// Token: 0x040086DF RID: 34527
		[Tooltip("the camera will be moved and rotated to these spots and record stats.")]
		public Transform[] benchmarkLocations;

		// Token: 0x040086E0 RID: 34528
		[Tooltip("How long to wait before calling GC.Collect() to clean up memory.")]
		public float collectGarbageDelay = 2f;

		// Token: 0x040086E1 RID: 34529
		[Tooltip("How long to wait before recording stats after the camera was moved to a new location.\nThis + collectGarbageDelay is the total time spent at each location.")]
		private float recordStatsDelay = 2f;

		// Token: 0x040086E2 RID: 34530
		[Tooltip("The camera to use for profiling. If null, a new camera will be created.")]
		private Camera cam;

		// Token: 0x040086E3 RID: 34531
		private VisualBenchmark.StatInfo[] availableRenderStats;

		// Token: 0x040086E4 RID: 34532
		private ProfilerRecorder[] renderStatsRecorders;

		// Token: 0x040086E5 RID: 34533
		private static bool isQuitting = true;

		// Token: 0x040086E6 RID: 34534
		private int currentLocationIndex;

		// Token: 0x040086E7 RID: 34535
		private VisualBenchmark.EState state = VisualBenchmark.EState.WaitingBeforeCollectingGarbage;

		// Token: 0x040086E8 RID: 34536
		private float lastTime;

		// Token: 0x040086E9 RID: 34537
		private readonly StringBuilder sb = new StringBuilder(1024);

		// Token: 0x02001275 RID: 4725
		private struct StatInfo
		{
			// Token: 0x040086EA RID: 34538
			public string name;

			// Token: 0x040086EB RID: 34539
			public ProfilerMarkerDataUnit unit;
		}

		// Token: 0x02001276 RID: 4726
		private enum EState
		{
			// Token: 0x040086ED RID: 34541
			Setup,
			// Token: 0x040086EE RID: 34542
			WaitingBeforeCollectingGarbage,
			// Token: 0x040086EF RID: 34543
			WaitingBeforeRecordingStats,
			// Token: 0x040086F0 RID: 34544
			TearDown
		}
	}
}
