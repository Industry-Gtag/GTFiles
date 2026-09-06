using System;
using System.Collections.Generic;
using UnityEngine;

namespace PerformanceSystems
{
	// Token: 0x02000F29 RID: 3881
	[CreateAssetMenu(menuName = "PerformanceTools/TimeSlicer/TimeSliceController", fileName = "TimeSliceController")]
	public class TimeSliceControllerAsset : ScriptableObject
	{
		// Token: 0x17000932 RID: 2354
		// (get) Token: 0x06005F1D RID: 24349 RVA: 0x001E389D File Offset: 0x001E1A9D
		public Transform ReferenceTransform
		{
			get
			{
				return this._referenceTransform;
			}
		}

		// Token: 0x06005F1E RID: 24350 RVA: 0x001E38A5 File Offset: 0x001E1AA5
		private void RemovePendingObjects()
		{
			this._currentTimeSliceBehaviours.FastRemove(this._timeSliceBehavioursToRemove);
			this._timeSliceBehavioursToRemove.Clear();
		}

		// Token: 0x06005F1F RID: 24351 RVA: 0x001E38C4 File Offset: 0x001E1AC4
		private void AddPendingObjects()
		{
			foreach (ITimeSlice timeSlice in this._timeSliceBehavioursToAdd)
			{
				if (!this._currentTimeSliceBehaviours.Contains(timeSlice))
				{
					this._currentTimeSliceBehaviours.Add(timeSlice);
				}
			}
			this._timeSliceBehavioursToAdd.Clear();
		}

		// Token: 0x06005F20 RID: 24352 RVA: 0x001E3938 File Offset: 0x001E1B38
		private void UpdateCurrentSliceObjects()
		{
			int count = this._currentTimeSliceBehaviours.Count;
			if (count == 0)
			{
				return;
			}
			int num = Mathf.Max(1, this._timeSlices);
			this._sliceSize = Mathf.CeilToInt((float)count / (float)num);
			if (this._sliceSize <= 0)
			{
				this._sliceSize = 1;
			}
			int num2 = this._sliceSize * this._currentSlice;
			if (num2 >= count)
			{
				num2 = Mathf.Max(0, count - this._sliceSize);
			}
			int num3 = Mathf.Min(this._sliceSize, count - num2);
			if (num3 <= 0)
			{
				return;
			}
			for (int i = 0; i < num3; i++)
			{
				int num4 = num2 + i;
				if (num4 < 0 || num4 >= this._currentTimeSliceBehaviours.Count)
				{
					break;
				}
				ITimeSlice timeSlice = this._currentTimeSliceBehaviours[num4];
				if (timeSlice != null)
				{
					timeSlice.SliceUpdate();
				}
			}
		}

		// Token: 0x06005F21 RID: 24353 RVA: 0x001E39F9 File Offset: 0x001E1BF9
		public void SetRefTransform(Transform refTransform)
		{
			this._referenceTransform = refTransform;
			this._isActive = this._referenceTransform != null;
		}

		// Token: 0x06005F22 RID: 24354 RVA: 0x001E3A14 File Offset: 0x001E1C14
		public void AddTimeSliceBehaviour(ITimeSlice timeSlice)
		{
			if (this._currentTimeSliceBehaviours.Contains(timeSlice))
			{
				return;
			}
			this._timeSliceBehavioursToAdd.Add(timeSlice);
		}

		// Token: 0x06005F23 RID: 24355 RVA: 0x001E3A32 File Offset: 0x001E1C32
		public void RemoveTimeSliceBehaviour(ITimeSlice timeSlice)
		{
			if (!this._currentTimeSliceBehaviours.Contains(timeSlice))
			{
				this._timeSliceBehavioursToRemove.Remove(timeSlice);
				return;
			}
			this._timeSliceBehavioursToRemove.Add(timeSlice);
		}

		// Token: 0x06005F24 RID: 24356 RVA: 0x001E3A60 File Offset: 0x001E1C60
		public void Update()
		{
			this.InitializeReferenceTransformWithMainCam();
			if (!this._isActive)
			{
				return;
			}
			if (this._currentSlice == 0)
			{
				this.RemovePendingObjects();
				this.AddPendingObjects();
			}
			this.UpdateCurrentSliceObjects();
			this._currentSlice = (this._currentSlice + 1) % Mathf.Max(1, this._timeSlices);
		}

		// Token: 0x06005F25 RID: 24357 RVA: 0x001E3AB1 File Offset: 0x001E1CB1
		public void InitializeReferenceTransformWithMainCam()
		{
			if (this._referenceTransform == null)
			{
				Camera main = Camera.main;
				this._referenceTransform = ((main != null) ? main.transform : null);
			}
			this._isActive = this._referenceTransform != null;
		}

		// Token: 0x06005F26 RID: 24358 RVA: 0x001E3AEA File Offset: 0x001E1CEA
		private void OnDisable()
		{
			this.ClearAsset();
		}

		// Token: 0x06005F27 RID: 24359 RVA: 0x001E3AF2 File Offset: 0x001E1CF2
		public void ClearAsset()
		{
			this._currentTimeSliceBehaviours.Clear();
			this._timeSliceBehavioursToAdd.Clear();
			this._timeSliceBehavioursToRemove.Clear();
			this._referenceTransform = null;
		}

		// Token: 0x04006D9C RID: 28060
		private readonly List<ITimeSlice> _currentTimeSliceBehaviours = new List<ITimeSlice>();

		// Token: 0x04006D9D RID: 28061
		private readonly HashSet<ITimeSlice> _timeSliceBehavioursToAdd = new HashSet<ITimeSlice>();

		// Token: 0x04006D9E RID: 28062
		private readonly HashSet<ITimeSlice> _timeSliceBehavioursToRemove = new HashSet<ITimeSlice>();

		// Token: 0x04006D9F RID: 28063
		private Transform _referenceTransform;

		// Token: 0x04006DA0 RID: 28064
		[Range(1f, 150f)]
		[SerializeField]
		private int _timeSlices = 1;

		// Token: 0x04006DA1 RID: 28065
		private int _currentSlice;

		// Token: 0x04006DA2 RID: 28066
		private bool _isActive;

		// Token: 0x04006DA3 RID: 28067
		private int _sliceSize;
	}
}
