using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020003CE RID: 974
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST && !BETA")]
public class PerfTestFPSCaptureController : MonoBehaviour
{
	// Token: 0x04002298 RID: 8856
	[SerializeField]
	private SerializablePerformanceReport<ScenePerformanceData> performanceSummary;
}
