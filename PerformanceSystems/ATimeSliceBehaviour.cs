using System;
using UnityEngine;

namespace PerformanceSystems
{
	// Token: 0x02000F25 RID: 3877
	public abstract class ATimeSliceBehaviour : MonoBehaviour, ITimeSlice
	{
		// Token: 0x06005F0C RID: 24332 RVA: 0x001E3765 File Offset: 0x001E1965
		protected void Awake()
		{
			this._timeSliceControllerAsset.AddTimeSliceBehaviour(this);
		}

		// Token: 0x06005F0D RID: 24333 RVA: 0x001E3773 File Offset: 0x001E1973
		protected void OnDestroy()
		{
			this._timeSliceControllerAsset.RemoveTimeSliceBehaviour(this);
		}

		// Token: 0x06005F0E RID: 24334 RVA: 0x001E3784 File Offset: 0x001E1984
		public void SliceUpdate()
		{
			float num = Time.realtimeSinceStartup - this._lastUpdateTime;
			this._lastUpdateTime = Time.realtimeSinceStartup;
			this.SliceUpdateAlways(num);
			if (this._updateIfDisabled || base.gameObject.activeSelf)
			{
				this.SliceUpdate(num);
			}
		}

		// Token: 0x06005F0F RID: 24335
		public abstract void SliceUpdate(float deltaTime);

		// Token: 0x06005F10 RID: 24336
		public abstract void SliceUpdateAlways(float deltaTime);

		// Token: 0x04006D99 RID: 28057
		[SerializeField]
		protected TimeSliceControllerAsset _timeSliceControllerAsset;

		// Token: 0x04006D9A RID: 28058
		[SerializeField]
		protected bool _updateIfDisabled = true;

		// Token: 0x04006D9B RID: 28059
		protected float _lastUpdateTime;
	}
}
