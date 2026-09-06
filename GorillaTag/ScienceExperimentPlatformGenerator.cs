using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag.GuidedRefs;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02001218 RID: 4632
	public class ScienceExperimentPlatformGenerator : MonoBehaviourPun, ITickSystemPost, IGuidedRefReceiverMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x0600756F RID: 30063 RVA: 0x00263933 File Offset: 0x00261B33
		private void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
			this.scienceExperimentManager = base.GetComponent<ScienceExperimentManager>();
		}

		// Token: 0x06007570 RID: 30064 RVA: 0x00263947 File Offset: 0x00261B47
		private void OnEnable()
		{
			if (((IGuidedRefReceiverMono)this).GuidedRefsWaitingToResolveCount > 0)
			{
				return;
			}
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x06007571 RID: 30065 RVA: 0x0015AB83 File Offset: 0x00158D83
		protected void OnDisable()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x17000B65 RID: 2917
		// (get) Token: 0x06007572 RID: 30066 RVA: 0x00263959 File Offset: 0x00261B59
		// (set) Token: 0x06007573 RID: 30067 RVA: 0x00263961 File Offset: 0x00261B61
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x06007574 RID: 30068 RVA: 0x0026396C File Offset: 0x00261B6C
		void ITickSystemPost.PostTick()
		{
			double num = (PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.unscaledTimeAsDouble);
			this.UpdateTrails(num);
			this.RemoveExpiredBubbles(num);
			this.SpawnNewBubbles(num);
			this.UpdateActiveBubbles(num);
		}

		// Token: 0x06007575 RID: 30069 RVA: 0x002639AC File Offset: 0x00261BAC
		private void RemoveExpiredBubbles(double currentTime)
		{
			for (int i = this.activeBubbles.Count - 1; i >= 0; i--)
			{
				if (Mathf.Clamp01((float)(currentTime - this.activeBubbles[i].spawnTime) / this.activeBubbles[i].lifetime) >= 1f)
				{
					this.activeBubbles[i].bubble.Pop();
					this.activeBubbles.RemoveAt(i);
				}
			}
		}

		// Token: 0x06007576 RID: 30070 RVA: 0x00263A28 File Offset: 0x00261C28
		private void SpawnNewBubbles(double currentTime)
		{
			if (base.photonView.IsMine && this.scienceExperimentManager.GameState == ScienceExperimentManager.RisingLiquidState.Rising)
			{
				int num = Mathf.Min((int)(this.rockCountVsLavaProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.bubbleCountMultiplier), this.maxBubbleCount) - this.activeBubbles.Count;
				if (this.activeBubbles.Count < this.maxBubbleCount)
				{
					for (int i = 0; i < num; i++)
					{
						this.SpawnRockAuthority(currentTime, this.scienceExperimentManager.RiseProgressLinear);
					}
				}
			}
		}

		// Token: 0x06007577 RID: 30071 RVA: 0x00263AB8 File Offset: 0x00261CB8
		private void UpdateActiveBubbles(double currentTime)
		{
			if (this.liquidSurfacePlane == null)
			{
				return;
			}
			float y = this.liquidSurfacePlane.transform.position.y;
			float num = this.bubblePopWobbleAmplitude * Mathf.Sin(this.bubblePopWobbleFrequency * 0.5f * 3.1415927f * Time.time);
			for (int i = 0; i < this.activeBubbles.Count; i++)
			{
				ScienceExperimentPlatformGenerator.BubbleData bubbleData = this.activeBubbles[i];
				float num2 = Mathf.Clamp01((float)(currentTime - bubbleData.spawnTime) / bubbleData.lifetime);
				float num3 = bubbleData.spawnSize * this.rockSizeVsLifetime.Evaluate(num2) * this.scaleFactor;
				bubbleData.position.y = y;
				bubbleData.bubble.body.gameObject.transform.localScale = Vector3.one * num3;
				bubbleData.bubble.body.MovePosition(bubbleData.position);
				float num4 = (float)((double)bubbleData.lifetime + bubbleData.spawnTime - currentTime);
				if (num4 < this.bubblePopAnticipationTime)
				{
					float num5 = Mathf.Clamp01(1f - num4 / this.bubblePopAnticipationTime);
					bubbleData.bubble.bubbleMesh.transform.localScale = Vector3.one * (1f + num5 * num);
				}
				this.activeBubbles[i] = bubbleData;
			}
		}

		// Token: 0x06007578 RID: 30072 RVA: 0x00263C20 File Offset: 0x00261E20
		private void UpdateTrails(double currentTime)
		{
			if (base.photonView.IsMine)
			{
				int num = (int)(this.trailCountVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailCountMultiplier) - this.trailHeads.Count;
				if (num > 0 && this.scienceExperimentManager.GameState == ScienceExperimentManager.RisingLiquidState.Rising)
				{
					for (int i = 0; i < num; i++)
					{
						this.SpawnTrailAuthority(currentTime, this.scienceExperimentManager.RiseProgressLinear);
					}
				}
				else if (num < 0)
				{
					for (int j = 0; j > num; j--)
					{
						this.trailHeads.RemoveAt(0);
					}
				}
				float num2 = this.trailSpawnRateVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailSpawnRateMultiplier;
				float num3 = this.trailBubbleBoundaryRadiusVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.surfaceRadiusSpawnRange.y;
				for (int k = this.trailHeads.Count - 1; k >= 0; k--)
				{
					if ((float)(currentTime - this.trailHeads[k].spawnTime) > num2)
					{
						float num4 = -this.trailMaxTurnAngle;
						float num5 = this.trailMaxTurnAngle;
						float num6 = Vector3.SignedAngle(this.trailHeads[k].direction, this.trailHeads[k].position - this.liquidSurfacePlane.transform.position, Vector3.up);
						float num7 = num3 - Vector3.Distance(this.trailHeads[k].position, this.liquidSurfacePlane.transform.position);
						if (num7 < this.trailEdgeAvoidanceSpawnsMinMax.x * this.trailDistanceBetweenSpawns * this.scaleFactor)
						{
							float num8 = Mathf.InverseLerp(this.trailEdgeAvoidanceSpawnsMinMax.x * this.trailDistanceBetweenSpawns * this.scaleFactor, this.trailEdgeAvoidanceSpawnsMinMax.y * this.trailDistanceBetweenSpawns * this.scaleFactor, num7);
							if (num6 > 0f)
							{
								float num9 = num6 - 90f * num8;
								num5 = Mathf.Min(num5, num9);
								num4 = Mathf.Min(num4, num5 - this.trailMaxTurnAngle);
							}
							else
							{
								float num10 = num6 + 90f * num8;
								num4 = Mathf.Max(num4, num10);
								num5 = Mathf.Max(num5, num4 + this.trailMaxTurnAngle);
							}
						}
						Vector3 vector = Quaternion.AngleAxis(Random.Range(num4, num5), Vector3.up) * this.trailHeads[k].direction;
						Vector3 vector2 = this.trailHeads[k].position + vector * this.trailDistanceBetweenSpawns * this.scaleFactor - this.liquidSurfacePlane.transform.position;
						if (vector2.sqrMagnitude > this.surfaceRadiusSpawnRange.y * this.surfaceRadiusSpawnRange.y)
						{
							vector2 = vector2.normalized * this.surfaceRadiusSpawnRange.y;
						}
						Vector2 vector3 = new Vector2(vector2.x, vector2.z);
						float num11 = this.trailBubbleSize;
						float num12 = this.trailBubbleLifetimeVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailBubbleLifetimeMultiplier;
						this.trailHeads.RemoveAt(k);
						base.photonView.RPC("SpawnSodaBubbleRPC", RpcTarget.Others, new object[] { vector3, num11, num12, currentTime });
						this.SpawnSodaBubbleLocal(vector3, num11, num12, currentTime, true, vector);
					}
				}
			}
		}

		// Token: 0x06007579 RID: 30073 RVA: 0x00263FB8 File Offset: 0x002621B8
		private void SpawnRockAuthority(double currentTime, float lavaProgress)
		{
			if (base.photonView.IsMine)
			{
				float num = this.rockLifetimeMultiplierVsLavaProgress.Evaluate(lavaProgress);
				float num2 = this.rockMaxSizeMultiplierVsLavaProgress.Evaluate(lavaProgress);
				float num3 = Random.Range(this.lifetimeRange.x, this.lifetimeRange.y) * num;
				float num4 = Random.Range(this.sizeRange.x, this.sizeRange.y * num2);
				float num5 = this.spawnRadiusMultiplierVsLavaProgress.Evaluate(lavaProgress);
				Vector2 vector = Random.insideUnitCircle.normalized * Random.Range(this.surfaceRadiusSpawnRange.x, this.surfaceRadiusSpawnRange.y) * num5;
				vector = this.GetSpawnPositionWithClearance(vector, num4 * this.scaleFactor, this.surfaceRadiusSpawnRange.y, this.liquidSurfacePlane.transform.position);
				base.photonView.RPC("SpawnSodaBubbleRPC", RpcTarget.Others, new object[] { vector, num4, num3, currentTime });
				this.SpawnSodaBubbleLocal(vector, num4, num3, currentTime, false, default(Vector3));
			}
		}

		// Token: 0x0600757A RID: 30074 RVA: 0x002640F0 File Offset: 0x002622F0
		private void SpawnTrailAuthority(double currentTime, float lavaProgress)
		{
			if (base.photonView.IsMine)
			{
				float num = this.trailBubbleLifetimeVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailBubbleLifetimeMultiplier;
				float num2 = this.trailBubbleSize;
				Vector2 vector = Random.insideUnitCircle.normalized * Random.Range(this.surfaceRadiusSpawnRange.x, this.surfaceRadiusSpawnRange.y);
				vector = this.GetSpawnPositionWithClearance(vector, num2 * this.scaleFactor, this.surfaceRadiusSpawnRange.y, this.liquidSurfacePlane.transform.position);
				Vector3 vector2 = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up) * Vector3.forward;
				base.photonView.RPC("SpawnSodaBubbleRPC", RpcTarget.Others, new object[] { vector, num2, num, currentTime });
				this.SpawnSodaBubbleLocal(vector, num2, num, currentTime, true, vector2);
			}
		}

		// Token: 0x0600757B RID: 30075 RVA: 0x002641F8 File Offset: 0x002623F8
		private void SpawnSodaBubbleLocal(Vector2 surfacePosLocal, float spawnSize, float lifetime, double spawnTime, bool addAsTrail = false, Vector3 direction = default(Vector3))
		{
			if (this.activeBubbles.Count < this.maxBubbleCount)
			{
				Vector3 vector = this.liquidSurfacePlane.transform.position + new Vector3(surfacePosLocal.x, 0f, surfacePosLocal.y);
				ScienceExperimentPlatformGenerator.BubbleData bubbleData = new ScienceExperimentPlatformGenerator.BubbleData
				{
					position = vector,
					spawnSize = spawnSize,
					lifetime = lifetime,
					spawnTime = spawnTime,
					isTrail = false
				};
				bubbleData.bubble = ObjectPools.instance.Instantiate(this.spawnedPrefab, bubbleData.position, Quaternion.identity, 0f, true).GetComponent<SodaBubble>();
				if (base.photonView.IsMine && addAsTrail)
				{
					bubbleData.direction = direction;
					bubbleData.isTrail = true;
					this.trailHeads.Add(bubbleData);
				}
				this.activeBubbles.Add(bubbleData);
			}
		}

		// Token: 0x0600757C RID: 30076 RVA: 0x002642E0 File Offset: 0x002624E0
		[PunRPC]
		public void SpawnSodaBubbleRPC(Vector2 surfacePosLocal, float spawnSize, float lifetime, double spawnTime, PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "SpawnSodaBubbleRPC");
			if (info.Sender == PhotonNetwork.MasterClient)
			{
				if (!float.IsFinite(spawnSize) || !float.IsFinite(lifetime) || !double.IsFinite(spawnTime))
				{
					return;
				}
				float num = Mathf.Clamp01(this.scienceExperimentManager.RiseProgressLinear);
				(ref surfacePosLocal).ClampThisMagnitudeSafe(this.surfaceRadiusSpawnRange.y);
				spawnSize = Mathf.Clamp(spawnSize, this.sizeRange.x, this.sizeRange.y * this.rockMaxSizeMultiplierVsLavaProgress.Evaluate(num));
				lifetime = Mathf.Clamp(lifetime, this.lifetimeRange.x, this.lifetimeRange.y * this.rockLifetimeMultiplierVsLavaProgress.Evaluate(num));
				double num2 = (PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.unscaledTimeAsDouble);
				spawnTime = ((Mathf.Abs((float)(spawnTime - num2)) < 10f) ? spawnTime : num2);
				this.SpawnSodaBubbleLocal(surfacePosLocal, spawnSize, lifetime, spawnTime, false, default(Vector3));
			}
		}

		// Token: 0x0600757D RID: 30077 RVA: 0x002643E0 File Offset: 0x002625E0
		private Vector2 GetSpawnPositionWithClearance(Vector2 inputPosition, float inputSize, float maxDistance, Vector3 lavaSurfaceOrigin)
		{
			Vector2 vector = inputPosition;
			for (int i = 0; i < this.activeBubbles.Count; i++)
			{
				Vector3 vector2 = this.activeBubbles[i].position - lavaSurfaceOrigin;
				Vector2 vector3 = new Vector2(vector2.x, vector2.z);
				Vector2 vector4 = vector - vector3;
				float num = (inputSize + this.activeBubbles[i].spawnSize * this.scaleFactor) * 0.5f;
				if (vector4.sqrMagnitude < num * num)
				{
					float magnitude = vector4.magnitude;
					if (magnitude > 0.001f)
					{
						Vector2 vector5 = vector4 / magnitude;
						vector += vector5 * (num - magnitude);
						if (vector.sqrMagnitude > maxDistance * maxDistance)
						{
							vector = vector.normalized * maxDistance;
						}
					}
				}
			}
			if (vector.sqrMagnitude > this.surfaceRadiusSpawnRange.y * this.surfaceRadiusSpawnRange.y)
			{
				vector = vector.normalized * this.surfaceRadiusSpawnRange.y;
			}
			return vector;
		}

		// Token: 0x0600757E RID: 30078 RVA: 0x002644F3 File Offset: 0x002626F3
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterReceiverField<ScienceExperimentPlatformGenerator>(this, "liquidSurfacePlane", ref this.liquidSurfacePlane_gRef);
			GuidedRefHub.ReceiverFullyRegistered<ScienceExperimentPlatformGenerator>(this);
		}

		// Token: 0x17000B66 RID: 2918
		// (get) Token: 0x0600757F RID: 30079 RVA: 0x0026450C File Offset: 0x0026270C
		// (set) Token: 0x06007580 RID: 30080 RVA: 0x00264514 File Offset: 0x00262714
		int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

		// Token: 0x06007581 RID: 30081 RVA: 0x0026451D File Offset: 0x0026271D
		bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
		{
			return GuidedRefHub.TryResolveField<ScienceExperimentPlatformGenerator, Transform>(this, ref this.liquidSurfacePlane, this.liquidSurfacePlane_gRef, target);
		}

		// Token: 0x06007582 RID: 30082 RVA: 0x00264532 File Offset: 0x00262732
		void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
		{
			if (!base.enabled)
			{
				return;
			}
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x06007583 RID: 30083 RVA: 0x0015AB83 File Offset: 0x00158D83
		void IGuidedRefReceiverMono.OnGuidedRefTargetDestroyed(int fieldId)
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x06007585 RID: 30085 RVA: 0x000874AD File Offset: 0x000856AD
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x06007586 RID: 30086 RVA: 0x00019405 File Offset: 0x00017605
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x0400854A RID: 34122
		[SerializeField]
		private GameObject spawnedPrefab;

		// Token: 0x0400854B RID: 34123
		[SerializeField]
		private float scaleFactor = 0.03f;

		// Token: 0x0400854C RID: 34124
		[Header("Random Bubbles")]
		[SerializeField]
		private Vector2 surfaceRadiusSpawnRange = new Vector2(0.1f, 0.7f);

		// Token: 0x0400854D RID: 34125
		[SerializeField]
		private Vector2 lifetimeRange = new Vector2(5f, 10f);

		// Token: 0x0400854E RID: 34126
		[SerializeField]
		private Vector2 sizeRange = new Vector2(0.5f, 2f);

		// Token: 0x0400854F RID: 34127
		[SerializeField]
		private AnimationCurve rockCountVsLavaProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04008550 RID: 34128
		[SerializeField]
		[FormerlySerializedAs("rockCountMultiplier")]
		private float bubbleCountMultiplier = 80f;

		// Token: 0x04008551 RID: 34129
		[SerializeField]
		private int maxBubbleCount = 100;

		// Token: 0x04008552 RID: 34130
		[SerializeField]
		private AnimationCurve rockLifetimeMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x04008553 RID: 34131
		[SerializeField]
		private AnimationCurve rockMaxSizeMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x04008554 RID: 34132
		[SerializeField]
		private AnimationCurve spawnRadiusMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x04008555 RID: 34133
		[SerializeField]
		private AnimationCurve rockSizeVsLifetime = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04008556 RID: 34134
		[Header("Bubble Trails")]
		[SerializeField]
		private AnimationCurve trailSpawnRateVsProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04008557 RID: 34135
		[SerializeField]
		private float trailSpawnRateMultiplier = 1f;

		// Token: 0x04008558 RID: 34136
		[SerializeField]
		private AnimationCurve trailBubbleLifetimeVsProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04008559 RID: 34137
		[SerializeField]
		private AnimationCurve trailBubbleBoundaryRadiusVsProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400855A RID: 34138
		[SerializeField]
		private float trailBubbleLifetimeMultiplier = 6f;

		// Token: 0x0400855B RID: 34139
		[SerializeField]
		private float trailDistanceBetweenSpawns = 3f;

		// Token: 0x0400855C RID: 34140
		[SerializeField]
		private float trailMaxTurnAngle = 55f;

		// Token: 0x0400855D RID: 34141
		[SerializeField]
		private float trailBubbleSize = 1.5f;

		// Token: 0x0400855E RID: 34142
		[SerializeField]
		private AnimationCurve trailCountVsProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400855F RID: 34143
		[SerializeField]
		private float trailCountMultiplier = 12f;

		// Token: 0x04008560 RID: 34144
		[SerializeField]
		private Vector2 trailEdgeAvoidanceSpawnsMinMax = new Vector2(3f, 1f);

		// Token: 0x04008561 RID: 34145
		[Header("Feedback Effects")]
		[SerializeField]
		private float bubblePopAnticipationTime = 2f;

		// Token: 0x04008562 RID: 34146
		[SerializeField]
		private float bubblePopWobbleFrequency = 25f;

		// Token: 0x04008563 RID: 34147
		[SerializeField]
		private float bubblePopWobbleAmplitude = 0.01f;

		// Token: 0x04008564 RID: 34148
		[SerializeField]
		private Transform liquidSurfacePlane;

		// Token: 0x04008565 RID: 34149
		[SerializeField]
		private GuidedRefReceiverFieldInfo liquidSurfacePlane_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x04008566 RID: 34150
		private List<ScienceExperimentPlatformGenerator.BubbleData> activeBubbles = new List<ScienceExperimentPlatformGenerator.BubbleData>();

		// Token: 0x04008567 RID: 34151
		private List<ScienceExperimentPlatformGenerator.BubbleData> trailHeads = new List<ScienceExperimentPlatformGenerator.BubbleData>();

		// Token: 0x04008568 RID: 34152
		private List<ScienceExperimentPlatformGenerator.BubbleSpawnDebug> bubbleSpawnDebug = new List<ScienceExperimentPlatformGenerator.BubbleSpawnDebug>();

		// Token: 0x04008569 RID: 34153
		private ScienceExperimentManager scienceExperimentManager;

		// Token: 0x02001219 RID: 4633
		private struct BubbleData
		{
			// Token: 0x0400856C RID: 34156
			public Vector3 position;

			// Token: 0x0400856D RID: 34157
			public Vector3 direction;

			// Token: 0x0400856E RID: 34158
			public float spawnSize;

			// Token: 0x0400856F RID: 34159
			public float lifetime;

			// Token: 0x04008570 RID: 34160
			public double spawnTime;

			// Token: 0x04008571 RID: 34161
			public bool isTrail;

			// Token: 0x04008572 RID: 34162
			public SodaBubble bubble;
		}

		// Token: 0x0200121A RID: 4634
		private struct BubbleSpawnDebug
		{
			// Token: 0x04008573 RID: 34163
			public Vector3 initialPosition;

			// Token: 0x04008574 RID: 34164
			public Vector3 initialDirection;

			// Token: 0x04008575 RID: 34165
			public Vector3 spawnPosition;

			// Token: 0x04008576 RID: 34166
			public float minAngle;

			// Token: 0x04008577 RID: 34167
			public float maxAngle;

			// Token: 0x04008578 RID: 34168
			public float edgeCorrectionAngle;

			// Token: 0x04008579 RID: 34169
			public double spawnTime;
		}
	}
}
