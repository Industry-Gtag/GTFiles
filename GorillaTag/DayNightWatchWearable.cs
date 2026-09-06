using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02001209 RID: 4617
	public class DayNightWatchWearable : MonoBehaviour
	{
		// Token: 0x06007502 RID: 29954 RVA: 0x0025FD90 File Offset: 0x0025DF90
		private void Start()
		{
			if (!this.dayNightManager)
			{
				this.dayNightManager = BetterDayNightManager.instance;
			}
			this.rotationDegree = 0f;
			if (this.clockNeedle)
			{
				this.initialRotation = this.clockNeedle.localRotation;
			}
		}

		// Token: 0x06007503 RID: 29955 RVA: 0x0025FDE0 File Offset: 0x0025DFE0
		private void Update()
		{
			this.currentTimeOfDay = this.dayNightManager.currentTimeOfDay;
			double currentTimeInSeconds = ((ITimeOfDaySystem)this.dayNightManager).currentTimeInSeconds;
			double totalTimeInSeconds = ((ITimeOfDaySystem)this.dayNightManager).totalTimeInSeconds;
			this.rotationDegree = (float)(360.0 * currentTimeInSeconds / totalTimeInSeconds);
			this.rotationDegree = Mathf.Floor(this.rotationDegree);
			if (this.clockNeedle)
			{
				this.clockNeedle.localRotation = this.initialRotation * Quaternion.AngleAxis(this.rotationDegree, this.needleRotationAxis);
			}
		}

		// Token: 0x040084B3 RID: 33971
		[Tooltip("The transform that will be rotated to indicate the current time.")]
		public Transform clockNeedle;

		// Token: 0x040084B4 RID: 33972
		[FormerlySerializedAs("dialRotationAxis")]
		[Tooltip("The axis that the needle will rotate around.")]
		public Vector3 needleRotationAxis = Vector3.right;

		// Token: 0x040084B5 RID: 33973
		private BetterDayNightManager dayNightManager;

		// Token: 0x040084B6 RID: 33974
		[DebugOption]
		private float rotationDegree;

		// Token: 0x040084B7 RID: 33975
		private string currentTimeOfDay;

		// Token: 0x040084B8 RID: 33976
		private Quaternion initialRotation;
	}
}
