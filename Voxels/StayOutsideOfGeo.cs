using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels
{
	// Token: 0x02001396 RID: 5014
	public class StayOutsideOfGeo : MonoBehaviour
	{
		// Token: 0x06007D4F RID: 32079 RVA: 0x0028FF40 File Offset: 0x0028E140
		private void Reset()
		{
			this._target = base.transform;
		}

		// Token: 0x06007D50 RID: 32080 RVA: 0x0028FF50 File Offset: 0x0028E150
		private void Start()
		{
			if (this._target == null)
			{
				this._target = base.transform;
			}
			this._voxelWorld = VoxelWorld.GetFor(base.gameObject);
			if (this._voxelWorld == null)
			{
				Debug.LogError("VoxelWorld not found in the scene. Please ensure there is a VoxelWorld component present.");
				base.enabled = false;
			}
		}

		// Token: 0x06007D51 RID: 32081 RVA: 0x0028FFA4 File Offset: 0x0028E1A4
		private void Update()
		{
			int3 voxelForWorldPosition = this._voxelWorld.GetVoxelForWorldPosition(this._target.position + this._targetOffset);
			if (this.TestPosition(voxelForWorldPosition, true).Item1)
			{
				this.AddPositionToHistory(voxelForWorldPosition);
				return;
			}
			if (this.ResolvePenetration(voxelForWorldPosition))
			{
				Debug.Log(string.Format("Successfully resolved penetration for {0} at position {1}", this._target.name, voxelForWorldPosition), this);
				return;
			}
			for (int i = 10; i < 100; i += 10)
			{
				for (int j = 0; j < 10; j++)
				{
					int num = voxelForWorldPosition.x + global::UnityEngine.Random.Range(-i, i);
					int num2 = voxelForWorldPosition.y + global::UnityEngine.Random.Range(-i, i);
					int num3 = voxelForWorldPosition.z + global::UnityEngine.Random.Range(-i, i);
					int3 @int = new int3(num, num2, num3);
					if (this.IsOutsideGeo(@int))
					{
						Debug.Log(string.Format("Found valid random position {0} outside geo for {1}", @int, this._target.name), this);
						this.SetPosition(this._voxelWorld.GetWorldPosition(@int));
						return;
					}
				}
			}
		}

		// Token: 0x06007D52 RID: 32082 RVA: 0x002900BC File Offset: 0x0028E2BC
		private bool ResolvePenetration(int3 pos)
		{
			Debug.LogWarning(string.Format("{0} inside geo in {1} [{2}={3}->{4:F2}]", new object[]
			{
				base.name,
				this._voxelWorld.GetChunkForLocalPosition(pos),
				this._target.position.RoundToInt(),
				this._voxelWorld.GetVoxelForWorldPosition(this._target.position),
				this._maxDensity
			}), this);
			for (int i = 0; i < this._positionHistory.Count; i++)
			{
				int3 @int = this.PopMostRecentPosition();
				if (!@int.Equals(int3.zero))
				{
					ValueTuple<bool, int3> valueTuple = this.TestPosition(@int, false);
					if (valueTuple.Item1)
					{
						Debug.Log(string.Format("Found valid position {0} near recent position {1} at index {2}", valueTuple.Item2, @int, i));
						this.SetPosition(this._voxelWorld.GetWorldPosition(valueTuple.Item2));
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06007D53 RID: 32083 RVA: 0x002901B8 File Offset: 0x0028E3B8
		private void SetPosition(Vector3 worldPosition)
		{
			Debug.Log(string.Format("Moving {0} from {1} to {2}", this._target, this._target.position, worldPosition), this);
			Debug.DrawLine(this._target.position, worldPosition, Color.red, 5f);
			if (this._disableOnMove)
			{
				this._disableOnMove.enabled = false;
			}
			this._target.position = worldPosition;
			if (this._disableOnMove)
			{
				this._disableOnMove.enabled = true;
			}
		}

		// Token: 0x06007D54 RID: 32084 RVA: 0x0029024C File Offset: 0x0028E44C
		[return: TupleElementNames(new string[] { "match", "position" })]
		private ValueTuple<bool, int3> TestPosition(int3 position, bool useThreshold = true)
		{
			float num = this._voxelWorld.GetDensityAt(position, 0).ToFloat();
			this._maxDensity = Mathf.Max(num, float.MinValue);
			this._minDensity = Mathf.Min(num, float.MaxValue);
			float num2 = (useThreshold ? this._threshold : 0f);
			if (this._voxelWorld.GetDensityAt(position, 0).ToFloat() < num2)
			{
				for (int i = position.x - 1; i <= position.x + 1; i++)
				{
					for (int j = position.y - 1; j <= position.y + 1; j++)
					{
						for (int k = position.z - 1; k <= position.z + 1; k++)
						{
							num = this._voxelWorld.GetDensityAt(new int3(i, j, k), 0).ToFloat();
							this._maxDensity = Mathf.Max(num, this._maxDensity);
							this._minDensity = Mathf.Min(num, this._minDensity);
							if (num < 0f)
							{
								return new ValueTuple<bool, int3>(true, new int3(i, j, k));
							}
						}
					}
				}
			}
			return new ValueTuple<bool, int3>(false, position);
		}

		// Token: 0x06007D55 RID: 32085 RVA: 0x00290370 File Offset: 0x0028E570
		private bool IsOutsideGeo(int3 position)
		{
			return this._voxelWorld.GetDensityAt(position, 0).ToFloat() < 0f;
		}

		// Token: 0x06007D56 RID: 32086 RVA: 0x0029038C File Offset: 0x0028E58C
		private void AddPositionToHistory(int3 position)
		{
			if (this._positionHistory.Contains(position))
			{
				return;
			}
			if (this._positionHistory.Count >= this._maxHistorySize)
			{
				this._positionHistory[this._historyIndex] = position;
			}
			else
			{
				this._positionHistory.Add(position);
			}
			this._historyIndex = (this._historyIndex + 1) % this._maxHistorySize;
		}

		// Token: 0x06007D57 RID: 32087 RVA: 0x002903F0 File Offset: 0x0028E5F0
		private int3 GetMostRecentPosition()
		{
			if (this._positionHistory.Count == 0)
			{
				return int3.zero;
			}
			return this._positionHistory[(this._historyIndex - 1 + this._positionHistory.Count) % this._positionHistory.Count];
		}

		// Token: 0x06007D58 RID: 32088 RVA: 0x00290430 File Offset: 0x0028E630
		private int3 PopMostRecentPosition()
		{
			if (this._positionHistory.Count == 0)
			{
				return int3.zero;
			}
			this._historyIndex = (this._historyIndex - 1 + this._maxHistorySize) % this._maxHistorySize;
			return this._positionHistory[this._historyIndex];
		}

		// Token: 0x04009037 RID: 36919
		[SerializeField]
		private Transform _target;

		// Token: 0x04009038 RID: 36920
		[SerializeField]
		private Vector3 _targetOffset = Vector3.zero;

		// Token: 0x04009039 RID: 36921
		[SerializeField]
		private float _threshold = 0.4f;

		// Token: 0x0400903A RID: 36922
		[SerializeField]
		private bool _pauseOnPenetration = true;

		// Token: 0x0400903B RID: 36923
		[SerializeField]
		private Collider _disableOnMove;

		// Token: 0x0400903C RID: 36924
		private VoxelWorld _voxelWorld;

		// Token: 0x0400903D RID: 36925
		private List<int3> _positionHistory = new List<int3>();

		// Token: 0x0400903E RID: 36926
		private int _maxHistorySize = 10;

		// Token: 0x0400903F RID: 36927
		private int _historyIndex;

		// Token: 0x04009040 RID: 36928
		private float _maxDensity;

		// Token: 0x04009041 RID: 36929
		private float _minDensity;
	}
}
