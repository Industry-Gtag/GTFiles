using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020011E4 RID: 4580
	[AddComponentMenu("GorillaTag/ContainerLiquid (GTag)")]
	[ExecuteInEditMode]
	public class ContainerLiquid : MonoBehaviour
	{
		// Token: 0x17000B44 RID: 2884
		// (get) Token: 0x06007462 RID: 29794 RVA: 0x0025D76C File Offset: 0x0025B96C
		[DebugReadout]
		public bool isEmpty
		{
			get
			{
				return this.fillAmount <= this.refillThreshold;
			}
		}

		// Token: 0x17000B45 RID: 2885
		// (get) Token: 0x06007463 RID: 29795 RVA: 0x0025D77F File Offset: 0x0025B97F
		// (set) Token: 0x06007464 RID: 29796 RVA: 0x0025D787 File Offset: 0x0025B987
		public Vector3 cupTopWorldPos { get; private set; }

		// Token: 0x17000B46 RID: 2886
		// (get) Token: 0x06007465 RID: 29797 RVA: 0x0025D790 File Offset: 0x0025B990
		// (set) Token: 0x06007466 RID: 29798 RVA: 0x0025D798 File Offset: 0x0025B998
		public Vector3 bottomLipWorldPos { get; private set; }

		// Token: 0x17000B47 RID: 2887
		// (get) Token: 0x06007467 RID: 29799 RVA: 0x0025D7A1 File Offset: 0x0025B9A1
		// (set) Token: 0x06007468 RID: 29800 RVA: 0x0025D7A9 File Offset: 0x0025B9A9
		public Vector3 liquidPlaneWorldPos { get; private set; }

		// Token: 0x17000B48 RID: 2888
		// (get) Token: 0x06007469 RID: 29801 RVA: 0x0025D7B2 File Offset: 0x0025B9B2
		// (set) Token: 0x0600746A RID: 29802 RVA: 0x0025D7BA File Offset: 0x0025B9BA
		public Vector3 liquidPlaneWorldNormal { get; private set; }

		// Token: 0x0600746B RID: 29803 RVA: 0x0025D7C4 File Offset: 0x0025B9C4
		protected bool IsValidLiquidSurfaceValues()
		{
			return this.meshRenderer != null && this.meshFilter != null && this.spillParticleSystem != null && !string.IsNullOrEmpty(this.liquidColorShaderPropertyName) && !string.IsNullOrEmpty(this.liquidPlaneNormalShaderPropertyName) && !string.IsNullOrEmpty(this.liquidPlanePositionShaderPropertyName);
		}

		// Token: 0x0600746C RID: 29804 RVA: 0x0025D828 File Offset: 0x0025BA28
		protected void InitializeLiquidSurface()
		{
			this.liquidColorShaderProp = Shader.PropertyToID(this.liquidColorShaderPropertyName);
			this.liquidPlaneNormalShaderProp = Shader.PropertyToID(this.liquidPlaneNormalShaderPropertyName);
			this.liquidPlanePositionShaderProp = Shader.PropertyToID(this.liquidPlanePositionShaderPropertyName);
			this.localMeshBounds = this.meshFilter.sharedMesh.bounds;
		}

		// Token: 0x0600746D RID: 29805 RVA: 0x0025D880 File Offset: 0x0025BA80
		protected void InitializeParticleSystem()
		{
			this.spillParticleSystem.main.startColor = this.liquidColor;
		}

		// Token: 0x0600746E RID: 29806 RVA: 0x0025D8AB File Offset: 0x0025BAAB
		protected void Awake()
		{
			this.matPropBlock = new MaterialPropertyBlock();
			this.topVerts = this.GetTopVerts();
		}

		// Token: 0x0600746F RID: 29807 RVA: 0x0025D8C4 File Offset: 0x0025BAC4
		protected void OnEnable()
		{
			if (Application.isPlaying)
			{
				base.enabled = this.useLiquidShader && this.IsValidLiquidSurfaceValues();
				if (base.enabled)
				{
					this.InitializeLiquidSurface();
				}
				this.InitializeParticleSystem();
				this.useFloater = this.floater != null;
			}
		}

		// Token: 0x06007470 RID: 29808 RVA: 0x0025D918 File Offset: 0x0025BB18
		protected void LateUpdate()
		{
			this.UpdateRefillTimer();
			Transform transform = base.transform;
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			Bounds bounds = this.meshRenderer.bounds;
			Vector3 vector = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
			Vector3 vector2 = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
			this.liquidPlaneWorldPos = Vector3.Lerp(vector, vector2, this.fillAmount);
			Vector3 vector3 = transform.InverseTransformPoint(this.liquidPlaneWorldPos);
			float deltaTime = Time.deltaTime;
			this.temporalWobbleAmp = Vector2.Lerp(this.temporalWobbleAmp, Vector2.zero, deltaTime * this.recovery);
			float num = 6.2831855f * this.wobbleFrequency;
			float num2 = Mathf.Lerp(this.lastSineWave, Mathf.Sin(num * Time.realtimeSinceStartup), deltaTime * Mathf.Clamp(this.lastVelocity.magnitude + this.lastAngularVelocity.magnitude, this.thickness, 10f));
			Vector2 vector4 = this.temporalWobbleAmp * num2;
			this.liquidPlaneWorldNormal = new Vector3(vector4.x, -1f, vector4.y).normalized;
			Vector3 vector5 = transform.InverseTransformDirection(this.liquidPlaneWorldNormal);
			if (this.useLiquidShader)
			{
				this.matPropBlock.SetVector(this.liquidPlaneNormalShaderProp, vector5);
				this.matPropBlock.SetVector(this.liquidPlanePositionShaderProp, vector3);
				this.matPropBlock.SetVector(this.liquidColorShaderProp, this.liquidColor.linear);
				if (this.useLiquidVolume)
				{
					float num3 = MathUtils.Linear(this.fillAmount, 0f, 1f, this.liquidVolumeMinMax.x, this.liquidVolumeMinMax.y);
					this.matPropBlock.SetFloat(ShaderProps._LiquidFill, num3);
				}
				this.meshRenderer.SetPropertyBlock(this.matPropBlock);
			}
			if (this.useFloater)
			{
				float num4 = Mathf.Lerp(this.localMeshBounds.min.y, this.localMeshBounds.max.y, this.fillAmount);
				this.floater.localPosition = this.floater.localPosition.WithY(num4);
			}
			Vector3 vector6 = (this.lastPos - position) / deltaTime;
			Vector3 angularVelocity = GorillaMath.GetAngularVelocity(this.lastRot, rotation);
			this.temporalWobbleAmp.x = this.temporalWobbleAmp.x + Mathf.Clamp((vector6.x + vector6.y * 0.2f + angularVelocity.z + angularVelocity.y) * this.wobbleMax, -this.wobbleMax, this.wobbleMax);
			this.temporalWobbleAmp.y = this.temporalWobbleAmp.y + Mathf.Clamp((vector6.z + vector6.y * 0.2f + angularVelocity.x + angularVelocity.y) * this.wobbleMax, -this.wobbleMax, this.wobbleMax);
			this.lastPos = position;
			this.lastRot = rotation;
			this.lastSineWave = num2;
			this.lastVelocity = vector6;
			this.lastAngularVelocity = angularVelocity;
			this.meshRenderer.enabled = !this.keepMeshHidden && !this.isEmpty;
			float x = transform.lossyScale.x;
			float num5 = this.localMeshBounds.extents.x * x;
			float y = this.localMeshBounds.extents.y;
			Vector3 vector7 = this.localMeshBounds.center + new Vector3(0f, y, 0f);
			this.cupTopWorldPos = transform.TransformPoint(vector7);
			Vector3 up = transform.up;
			Vector3 vector8 = transform.InverseTransformDirection(Vector3.down);
			float num6 = float.MinValue;
			Vector3 vector9 = Vector3.zero;
			for (int i = 0; i < this.topVerts.Length; i++)
			{
				float num7 = Vector3.Dot(this.topVerts[i], vector8);
				if (num7 > num6)
				{
					num6 = num7;
					vector9 = this.topVerts[i];
				}
			}
			this.bottomLipWorldPos = transform.TransformPoint(vector9);
			float num8 = Mathf.Clamp01((this.liquidPlaneWorldPos.y - this.bottomLipWorldPos.y) / (num5 * 2f));
			bool flag = num8 > 1E-05f;
			ParticleSystem.EmissionModule emission = this.spillParticleSystem.emission;
			emission.enabled = flag;
			if (flag)
			{
				if (!this.spillSoundBankPlayer.isPlaying)
				{
					this.spillSoundBankPlayer.Play();
				}
				this.spillParticleSystem.transform.position = Vector3.Lerp(this.bottomLipWorldPos, this.cupTopWorldPos, num8);
				this.spillParticleSystem.shape.radius = num5 * num8;
				ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
				float num9 = num8 * this.maxSpillRate;
				rateOverTime.constant = num9;
				emission.rateOverTime = rateOverTime;
				this.fillAmount -= num9 * deltaTime * 0.01f;
			}
			if (this.isEmpty && !this.wasEmptyLastFrame && !this.emptySoundBankPlayer.isPlaying)
			{
				this.emptySoundBankPlayer.Play();
			}
			else if (!this.isEmpty && this.wasEmptyLastFrame && !this.refillSoundBankPlayer.isPlaying)
			{
				this.refillSoundBankPlayer.Play();
			}
			this.wasEmptyLastFrame = this.isEmpty;
		}

		// Token: 0x06007471 RID: 29809 RVA: 0x0025DEA4 File Offset: 0x0025C0A4
		public void UpdateRefillTimer()
		{
			if (this.refillDelay < 0f || !this.isEmpty)
			{
				return;
			}
			if (this.refillTimer < 0f)
			{
				this.refillTimer = this.refillDelay;
				this.fillAmount = this.refillAmount;
				return;
			}
			this.refillTimer -= Time.deltaTime;
		}

		// Token: 0x06007472 RID: 29810 RVA: 0x0025DF00 File Offset: 0x0025C100
		private Vector3[] GetTopVerts()
		{
			Vector3[] vertices = this.meshFilter.sharedMesh.vertices;
			List<Vector3> list = new List<Vector3>(vertices.Length);
			float num = float.MinValue;
			foreach (Vector3 vector in vertices)
			{
				if (vector.y > num)
				{
					num = vector.y;
				}
			}
			foreach (Vector3 vector2 in vertices)
			{
				if (Mathf.Abs(vector2.y - num) < 0.001f)
				{
					list.Add(vector2);
				}
			}
			return list.ToArray();
		}

		// Token: 0x0400842A RID: 33834
		[Tooltip("Used to determine the world space bounds of the container.")]
		public MeshRenderer meshRenderer;

		// Token: 0x0400842B RID: 33835
		[Tooltip("Used to determine the local space bounds of the container.")]
		public MeshFilter meshFilter;

		// Token: 0x0400842C RID: 33836
		[Tooltip("If you are only using the liquid mesh to calculate the volume of the container and do not need visuals then set this to true.")]
		public bool keepMeshHidden;

		// Token: 0x0400842D RID: 33837
		[Tooltip("The object that will float on top of the liquid.")]
		public Transform floater;

		// Token: 0x0400842E RID: 33838
		public bool useLiquidShader = true;

		// Token: 0x0400842F RID: 33839
		public bool useLiquidVolume;

		// Token: 0x04008430 RID: 33840
		public Vector2 liquidVolumeMinMax = Vector2.up;

		// Token: 0x04008431 RID: 33841
		public string liquidColorShaderPropertyName = "_BaseColor";

		// Token: 0x04008432 RID: 33842
		public string liquidPlaneNormalShaderPropertyName = "_LiquidPlaneNormal";

		// Token: 0x04008433 RID: 33843
		public string liquidPlanePositionShaderPropertyName = "_LiquidPlanePosition";

		// Token: 0x04008434 RID: 33844
		[Tooltip("Emits drips when pouring.")]
		public ParticleSystem spillParticleSystem;

		// Token: 0x04008435 RID: 33845
		[SoundBankInfo]
		public SoundBankPlayer emptySoundBankPlayer;

		// Token: 0x04008436 RID: 33846
		[SoundBankInfo]
		public SoundBankPlayer refillSoundBankPlayer;

		// Token: 0x04008437 RID: 33847
		[SoundBankInfo]
		public SoundBankPlayer spillSoundBankPlayer;

		// Token: 0x04008438 RID: 33848
		public Color liquidColor = new Color(0.33f, 0.25f, 0.21f, 1f);

		// Token: 0x04008439 RID: 33849
		[Tooltip("The amount of liquid currently in the container. This value is passed to the shader.")]
		[Range(0f, 1f)]
		public float fillAmount = 0.85f;

		// Token: 0x0400843A RID: 33850
		[Tooltip("This is what fillAmount will be after automatic refilling.")]
		public float refillAmount = 0.85f;

		// Token: 0x0400843B RID: 33851
		[Tooltip("Set to a negative value to disable.")]
		public float refillDelay = 10f;

		// Token: 0x0400843C RID: 33852
		[Tooltip("The point that the liquid should be considered empty and should be auto refilled.")]
		public float refillThreshold = 0.1f;

		// Token: 0x0400843D RID: 33853
		public float wobbleMax = 0.2f;

		// Token: 0x0400843E RID: 33854
		public float wobbleFrequency = 1f;

		// Token: 0x0400843F RID: 33855
		public float recovery = 1f;

		// Token: 0x04008440 RID: 33856
		public float thickness = 1f;

		// Token: 0x04008441 RID: 33857
		public float maxSpillRate = 100f;

		// Token: 0x04008446 RID: 33862
		[DebugReadout]
		private bool wasEmptyLastFrame;

		// Token: 0x04008447 RID: 33863
		private int liquidColorShaderProp;

		// Token: 0x04008448 RID: 33864
		private int liquidPlaneNormalShaderProp;

		// Token: 0x04008449 RID: 33865
		private int liquidPlanePositionShaderProp;

		// Token: 0x0400844A RID: 33866
		private float refillTimer;

		// Token: 0x0400844B RID: 33867
		private float lastSineWave;

		// Token: 0x0400844C RID: 33868
		private float lastWobble;

		// Token: 0x0400844D RID: 33869
		private Vector2 temporalWobbleAmp;

		// Token: 0x0400844E RID: 33870
		private Vector3 lastPos;

		// Token: 0x0400844F RID: 33871
		private Vector3 lastVelocity;

		// Token: 0x04008450 RID: 33872
		private Vector3 lastAngularVelocity;

		// Token: 0x04008451 RID: 33873
		private Quaternion lastRot;

		// Token: 0x04008452 RID: 33874
		private MaterialPropertyBlock matPropBlock;

		// Token: 0x04008453 RID: 33875
		private Bounds localMeshBounds;

		// Token: 0x04008454 RID: 33876
		private bool useFloater;

		// Token: 0x04008455 RID: 33877
		private Vector3[] topVerts;
	}
}
