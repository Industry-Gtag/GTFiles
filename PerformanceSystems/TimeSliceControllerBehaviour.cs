using System;
using UnityEngine;

namespace PerformanceSystems
{
	// Token: 0x02000F2A RID: 3882
	public class TimeSliceControllerBehaviour : MonoBehaviour
	{
		// Token: 0x06005F29 RID: 24361 RVA: 0x001E3B4C File Offset: 0x001E1D4C
		private void Awake()
		{
			this._timeSliceControllerAsset.InitializeReferenceTransformWithMainCam();
		}

		// Token: 0x06005F2A RID: 24362 RVA: 0x001E3B59 File Offset: 0x001E1D59
		private void Update()
		{
			this._timeSliceControllerAsset.Update();
		}

		// Token: 0x04006DA4 RID: 28068
		[SerializeField]
		private TimeSliceControllerAsset _timeSliceControllerAsset;
	}
}
