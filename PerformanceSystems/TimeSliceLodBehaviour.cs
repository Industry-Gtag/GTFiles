using System;
using UnityEngine;
using UnityEngine.Events;

namespace PerformanceSystems
{
	// Token: 0x02000F2B RID: 3883
	public class TimeSliceLodBehaviour : ATimeSliceBehaviour, ILod
	{
		// Token: 0x17000933 RID: 2355
		// (get) Token: 0x06005F2C RID: 24364 RVA: 0x001E3B66 File Offset: 0x001E1D66
		public Vector3 Position
		{
			get
			{
				return this._transform.position;
			}
		}

		// Token: 0x17000934 RID: 2356
		// (get) Token: 0x06005F2D RID: 24365 RVA: 0x001E3B73 File Offset: 0x001E1D73
		public float[] LodRanges
		{
			get
			{
				return this._lodRanges;
			}
		}

		// Token: 0x17000935 RID: 2357
		// (get) Token: 0x06005F2E RID: 24366 RVA: 0x001E3B7B File Offset: 0x001E1D7B
		public UnityEvent[] OnLodRangeEvents
		{
			get
			{
				return this._onLodRangeEvents;
			}
		}

		// Token: 0x17000936 RID: 2358
		// (get) Token: 0x06005F2F RID: 24367 RVA: 0x001E3B83 File Offset: 0x001E1D83
		public UnityEvent OnCulledEvent
		{
			get
			{
				return this._onCulledEvent;
			}
		}

		// Token: 0x17000937 RID: 2359
		// (get) Token: 0x06005F30 RID: 24368 RVA: 0x001E3B8B File Offset: 0x001E1D8B
		public int CurrentLod
		{
			get
			{
				return this._currentLod;
			}
		}

		// Token: 0x06005F31 RID: 24369 RVA: 0x001E3B93 File Offset: 0x001E1D93
		protected void Start()
		{
			this._updateIfDisabled = true;
			this._transform = base.transform;
		}

		// Token: 0x06005F32 RID: 24370 RVA: 0x001E3BA8 File Offset: 0x001E1DA8
		protected void SetLod(int newLod)
		{
			if (newLod == this._currentLod)
			{
				return;
			}
			this._currentLod = newLod;
			if (newLod < this._onLodRangeEvents.Length)
			{
				this._onLodRangeEvents[newLod].Invoke();
				return;
			}
			if (newLod == this._onLodRangeEvents.Length)
			{
				this._onCulledEvent.Invoke();
				return;
			}
			Debug.LogWarning(string.Format("No event for LOD [{0}]", newLod), this);
		}

		// Token: 0x06005F33 RID: 24371 RVA: 0x001E3C0C File Offset: 0x001E1E0C
		public void UpdateLod(Vector3 refPos)
		{
			Vector3 position = this._transform.position;
			float num = Vector3.Distance(refPos, position);
			for (int i = 0; i < this._lodRanges.Length; i++)
			{
				float num2 = this._lodRanges[i];
				if (num <= num2)
				{
					this.SetLod(i);
					return;
				}
			}
			this.SetLod(this._lodRanges.Length);
		}

		// Token: 0x06005F34 RID: 24372 RVA: 0x00002C2D File Offset: 0x00000E2D
		public override void SliceUpdate(float deltaTime)
		{
		}

		// Token: 0x06005F35 RID: 24373 RVA: 0x001E3C63 File Offset: 0x001E1E63
		public override void SliceUpdateAlways(float deltaTime)
		{
			this.UpdateLod(this._timeSliceControllerAsset.ReferenceTransform.position);
		}

		// Token: 0x04006DA5 RID: 28069
		[Space]
		[SerializeField]
		protected int _currentLod = -1;

		// Token: 0x04006DA6 RID: 28070
		[SerializeField]
		protected float[] _lodRanges;

		// Token: 0x04006DA7 RID: 28071
		[Space]
		[SerializeField]
		protected UnityEvent[] _onLodRangeEvents;

		// Token: 0x04006DA8 RID: 28072
		[Space]
		[SerializeField]
		protected UnityEvent _onCulledEvent;

		// Token: 0x04006DA9 RID: 28073
		protected Transform _transform;
	}
}
