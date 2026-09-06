using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000274 RID: 628
public class PlayableBoundaryManager : MonoBehaviour
{
	// Token: 0x170001AB RID: 427
	// (get) Token: 0x060010F8 RID: 4344 RVA: 0x0005B08A File Offset: 0x0005928A
	// (set) Token: 0x060010F9 RID: 4345 RVA: 0x0005B0A2 File Offset: 0x000592A2
	public static bool ShouldRender
	{
		get
		{
			return Shader.GetGlobalFloat(PlayableBoundaryManager._GTGameModes_PlayableBoundary_IsEnabled) > 0f;
		}
		set
		{
			Shader.SetGlobalFloat(PlayableBoundaryManager._GTGameModes_PlayableBoundary_IsEnabled, (float)(value ? 1 : 0));
		}
	}

	// Token: 0x060010FA RID: 4346 RVA: 0x0005B0BB File Offset: 0x000592BB
	protected void Awake()
	{
		if (!Application.isPlaying)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060010FB RID: 4347 RVA: 0x0005B0CC File Offset: 0x000592CC
	public void Setup()
	{
		Shader.SetGlobalFloat(PlayableBoundaryManager._GTGameModes_PlayableBoundary_NonZeroSmoothRadius, this.m_smoothFactor);
		Vector3 position = base.transform.position;
		SRand srand = new SRand(StaticHash.Compute(position.x, position.y, position.z));
		this._cylinders_centers[0] = new Vector3(position.x, position.y, position.z);
		this._cylinders_radiusHeights[0] = new Vector2(this.m_bigCylinderRadius * this.radiusScale, 100f);
		for (int i = 1; i < 8; i++)
		{
			Vector3 vector = position + srand.NextPointInsideSphere(this.m_bigCylinderRadius * this.radiusScale);
			this._cylinders_centers[i] = new Vector4(vector.x, vector.y, vector.z, 0f);
			this._cylinders_radiusHeights[i] = new Vector4(this.m_smallCylindersRadius * this.radiusScale, 100f, 0f, 0f);
		}
	}

	// Token: 0x060010FC RID: 4348 RVA: 0x0005B1E4 File Offset: 0x000593E4
	private void OnEnable()
	{
		PlayableBoundaryManager.ShouldRender = true;
		this.Setup();
	}

	// Token: 0x060010FD RID: 4349 RVA: 0x0005B1F2 File Offset: 0x000593F2
	private void OnDisable()
	{
		PlayableBoundaryManager.ShouldRender = false;
	}

	// Token: 0x060010FE RID: 4350 RVA: 0x0005B1FC File Offset: 0x000593FC
	public unsafe void UpdateSim()
	{
		if (Time.frameCount == this._lastFrameUpdated)
		{
			return;
		}
		this._lastFrameUpdated = Time.frameCount;
		Vector4[] array = this._cylinders_centers;
		if (array != null && array.Length == 8)
		{
			array = this._cylinders_radiusHeights;
			if (array != null && array.Length == 8)
			{
				if (this.m_smallCylindersMoveTimeScale > 0.0)
				{
					Vector3 position = base.transform.position;
					float num = (float)((double)(GTTime.TimeAsMilliseconds() % 86400000L) * this.m_smallCylindersMoveTimeScale / 1000.0);
					this._cylinders_centers[0] = new Vector3(position.x, position.y, position.z);
					this._cylinders_radiusHeights[0] = new Vector2(this.m_bigCylinderRadius * this.radiusScale, 100f);
					for (int i = 1; i < 8; i++)
					{
						float num2 = (float)i * 0.125f;
						Vector3 vector = *PlayableBoundaryManager.Hash3(num2 * 1.17f) + *PlayableBoundaryManager.Hash3(num2 * 13.7f) * num;
						Vector3 vector2 = position + vector.Sin() * this.m_bigCylinderRadius * this.radiusScale;
						this._cylinders_centers[i] = new Vector4(vector2.x, vector2.y, vector2.z, 0f);
						this._cylinders_radiusHeights[i] = new Vector4(this.m_smallCylindersRadius * this.radiusScale, 100f, 0f, 0f);
					}
				}
				Shader.SetGlobalVectorArray(PlayableBoundaryManager._GTGameModes_PlayableBoundary_Cylinders_Centers, this._cylinders_centers);
				Shader.SetGlobalVectorArray(PlayableBoundaryManager._GTGameModes_PlayableBoundary_Cylinders_RadiusHeights, this._cylinders_radiusHeights);
				for (int j = 0; j < this.tracked.Count; j++)
				{
					PlayableBoundaryTracker playableBoundaryTracker = this.tracked[j];
					if (playableBoundaryTracker)
					{
						playableBoundaryTracker.UpdateSignedDistanceToBoundary(this._GetSignedDistanceToBoundary(playableBoundaryTracker.transform.position, playableBoundaryTracker.radius), Time.deltaTime);
					}
				}
				Shader.SetGlobalFloat(PlayableBoundaryManager._GTGameModes_PlayableBoundary_NonZeroSmoothRadius, this.m_smoothFactor);
				return;
			}
		}
	}

	// Token: 0x060010FF RID: 4351 RVA: 0x0005B438 File Offset: 0x00059638
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float _GetSignedDistanceToBoundary(float3 tracked_center, float tracked_radius)
	{
		float num = float.MaxValue;
		float smoothFactor = this.GetSmoothFactor();
		for (int i = 0; i < 8; i++)
		{
			float3 @float = this._cylinders_centers[i].xyz - tracked_center;
			float x = this._cylinders_radiusHeights[i].x;
			float num2 = math.length(@float.xz) - x;
			num = this.SDFSmoothMerge(num, num2, smoothFactor);
		}
		return num - tracked_radius;
	}

	// Token: 0x06001100 RID: 4352 RVA: 0x0005B4B4 File Offset: 0x000596B4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float SDFSmoothMerge(float signedDist1, float signedDist2, float smoothRadius)
	{
		float num = -math.length(math.min(new float2(signedDist1 - smoothRadius, signedDist2 - smoothRadius), new float2(0f, 0f)));
		float num2 = math.max(math.min(signedDist1, signedDist2), smoothRadius);
		return num + num2;
	}

	// Token: 0x06001101 RID: 4353 RVA: 0x0005B4F8 File Offset: 0x000596F8
	private static ref Vector3 Hash3(float n)
	{
		PlayableBoundaryManager.kHashVec.x = Mathf.Sin(n) * 43758.547f % 1f;
		PlayableBoundaryManager.kHashVec.y = Mathf.Sin(n + 1f) * 22578.146f % 1f;
		PlayableBoundaryManager.kHashVec.z = Mathf.Sin(n + 2f) * 19642.35f % 1f;
		return ref PlayableBoundaryManager.kHashVec;
	}

	// Token: 0x06001102 RID: 4354 RVA: 0x0005B56C File Offset: 0x0005976C
	private float GetSmoothFactor()
	{
		float num = this.m_smoothFactor;
		if (this.m_bigCylinderRadius <= 1f)
		{
			num *= math.max(this.m_bigCylinderRadius, 0f);
		}
		return math.max(num, 1E-06f);
	}

	// Token: 0x0400143A RID: 5178
	public List<PlayableBoundaryTracker> tracked = new List<PlayableBoundaryTracker>(10);

	// Token: 0x0400143B RID: 5179
	[Space]
	[Range(0f, 128f)]
	public float m_bigCylinderRadius = 8f;

	// Token: 0x0400143C RID: 5180
	public float m_smoothFactor = 1.5f;

	// Token: 0x0400143D RID: 5181
	public float m_smallCylindersRadius = 3f;

	// Token: 0x0400143E RID: 5182
	[SerializeField]
	private double m_smallCylindersMoveTimeScale = 0.25;

	// Token: 0x0400143F RID: 5183
	[Space]
	private readonly Vector4[] _cylinders_centers = new Vector4[8];

	// Token: 0x04001440 RID: 5184
	private readonly Vector4[] _cylinders_radiusHeights = new Vector4[8];

	// Token: 0x04001441 RID: 5185
	private static ShaderHashId _GTGameModes_PlayableBoundary_Cylinders_Centers = "_GTGameModes_PlayableBoundary_Cylinders_Centers";

	// Token: 0x04001442 RID: 5186
	private static ShaderHashId _GTGameModes_PlayableBoundary_Cylinders_RadiusHeights = "_GTGameModes_PlayableBoundary_Cylinders_RadiusHeights";

	// Token: 0x04001443 RID: 5187
	private static ShaderHashId _GTGameModes_PlayableBoundary_NonZeroSmoothRadius = "_GTGameModes_PlayableBoundary_NonZeroSmoothRadius";

	// Token: 0x04001444 RID: 5188
	private static ShaderHashId _GTGameModes_PlayableBoundary_IsEnabled = "_GTGameModes_PlayableBoundary_IsEnabled";

	// Token: 0x04001445 RID: 5189
	private const int _k_cylinders_count = 8;

	// Token: 0x04001446 RID: 5190
	[NonSerialized]
	public float radiusScale = 1f;

	// Token: 0x04001447 RID: 5191
	private int _lastFrameUpdated = -1;

	// Token: 0x04001448 RID: 5192
	private static Vector3 kHashVec = Vector3.zero;
}
