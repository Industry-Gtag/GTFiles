using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001421 RID: 5153
	public class BoingBones : BoingReactor
	{
		// Token: 0x060081FE RID: 33278 RVA: 0x002A7C93 File Offset: 0x002A5E93
		protected override void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060081FF RID: 33279 RVA: 0x002A7C9B File Offset: 0x002A5E9B
		protected override void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x06008200 RID: 33280 RVA: 0x002A7CA3 File Offset: 0x002A5EA3
		protected override void OnUpgrade(Version oldVersion, Version newVersion)
		{
			base.OnUpgrade(oldVersion, newVersion);
			if (oldVersion.Revision < 33)
			{
				this.TwistPropagation = false;
			}
		}

		// Token: 0x06008201 RID: 33281 RVA: 0x002A7CBF File Offset: 0x002A5EBF
		public void OnValidate()
		{
			this.RescanBoneChains();
			this.UpdateCollisionRadius();
		}

		// Token: 0x06008202 RID: 33282 RVA: 0x002A7CCD File Offset: 0x002A5ECD
		public override void OnEnable()
		{
			base.OnEnable();
			this.RescanBoneChains();
			this.Reboot();
		}

		// Token: 0x06008203 RID: 33283 RVA: 0x002A7CE1 File Offset: 0x002A5EE1
		public override void OnDisable()
		{
			base.OnDisable();
			this.Restore();
		}

		// Token: 0x06008204 RID: 33284 RVA: 0x002A7CF0 File Offset: 0x002A5EF0
		public void RescanBoneChains()
		{
			if (this.BoneChains == null)
			{
				return;
			}
			int num = this.BoneChains.Length;
			if (this.BoneData == null || this.BoneData.Length != num)
			{
				BoingBones.Bone[][] array = new BoingBones.Bone[num][];
				if (this.BoneData != null)
				{
					int i = 0;
					int num2 = Mathf.Min(this.BoneData.Length, num);
					while (i < num2)
					{
						array[i] = this.BoneData[i];
						i++;
					}
				}
				this.BoneData = array;
			}
			Queue<BoingBones.RescanEntry> queue = new Queue<BoingBones.RescanEntry>();
			for (int j = 0; j < num; j++)
			{
				BoingBones.Chain chain = this.BoneChains[j];
				bool flag = false;
				if (this.BoneData[j] == null)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedRoot == null)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedRoot != chain.Root)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedExclusion != null != (chain.Exclusion != null))
				{
					flag = true;
				}
				if (!flag && chain.Exclusion != null)
				{
					if (chain.m_scannedExclusion.Length != chain.Exclusion.Length)
					{
						flag = true;
					}
					else
					{
						for (int k = 0; k < chain.m_scannedExclusion.Length; k++)
						{
							if (!(chain.m_scannedExclusion[k] == chain.Exclusion[k]))
							{
								flag = true;
								break;
							}
						}
					}
				}
				Transform transform = ((chain != null) ? chain.Root : null);
				int num3 = ((transform != null) ? Codec.HashTransformHierarchy(transform) : (-1));
				if (!flag && transform != null && chain.m_hierarchyHash != num3)
				{
					flag = true;
				}
				if (flag)
				{
					if (transform == null)
					{
						this.BoneData[j] = null;
					}
					else
					{
						chain.m_scannedRoot = chain.Root;
						chain.m_scannedExclusion = chain.Exclusion.ToArray<Transform>();
						chain.m_hierarchyHash = num3;
						chain.MaxLengthFromRoot = 0f;
						List<BoingBones.Bone> list = new List<BoingBones.Bone>();
						queue.Enqueue(new BoingBones.RescanEntry(transform, -1, 0f));
						while (queue.Count > 0)
						{
							BoingBones.RescanEntry rescanEntry = queue.Dequeue();
							if (!chain.Exclusion.Contains(rescanEntry.Transform))
							{
								int count = list.Count;
								Transform transform2 = rescanEntry.Transform;
								int[] array2 = new int[transform2.childCount];
								for (int l = 0; l < array2.Length; l++)
								{
									array2[l] = -1;
								}
								int num4 = 0;
								int m = 0;
								int childCount = transform2.childCount;
								while (m < childCount)
								{
									Transform child = transform2.GetChild(m);
									if (!chain.Exclusion.Contains(child))
									{
										float num5 = Vector3.Distance(rescanEntry.Transform.position, child.position);
										float num6 = rescanEntry.LengthFromRoot + num5;
										queue.Enqueue(new BoingBones.RescanEntry(child, count, num6));
										num4++;
									}
									m++;
								}
								chain.MaxLengthFromRoot = Mathf.Max(rescanEntry.LengthFromRoot, chain.MaxLengthFromRoot);
								BoingBones.Bone bone = new BoingBones.Bone(transform2, rescanEntry.ParentIndex, rescanEntry.LengthFromRoot);
								if (num4 > 0)
								{
									bone.ChildIndices = array2;
								}
								list.Add(bone);
							}
						}
						for (int n = 0; n < list.Count; n++)
						{
							BoingBones.Bone bone2 = list[n];
							if (bone2.ParentIndex >= 0)
							{
								BoingBones.Bone bone3 = list[bone2.ParentIndex];
								int num7 = 0;
								while (bone3.ChildIndices[num7] >= 0)
								{
									num7++;
								}
								if (num7 < bone3.ChildIndices.Length)
								{
									bone3.ChildIndices[num7] = n;
								}
							}
						}
						if (list.Count != 0)
						{
							float num8 = MathUtil.InvSafe(chain.MaxLengthFromRoot);
							for (int num9 = 0; num9 < list.Count; num9++)
							{
								BoingBones.Bone bone4 = list[num9];
								float num10 = Mathf.Clamp01(bone4.LengthFromRoot * num8);
								bone4.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, num10, chain.CollisionRadiusCustomCurve);
							}
							this.BoneData[j] = list.ToArray();
							this.Reboot(j);
						}
					}
				}
			}
		}

		// Token: 0x06008205 RID: 33285 RVA: 0x002A810C File Offset: 0x002A630C
		private void UpdateCollisionRadius()
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					float num = MathUtil.InvSafe(chain.MaxLengthFromRoot);
					foreach (BoingBones.Bone bone in array)
					{
						float num2 = Mathf.Clamp01(bone.LengthFromRoot * num);
						bone.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, num2, chain.CollisionRadiusCustomCurve);
					}
				}
			}
		}

		// Token: 0x06008206 RID: 33286 RVA: 0x002A8194 File Offset: 0x002A6394
		public override void Reboot()
		{
			base.Reboot();
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				this.Reboot(i);
			}
		}

		// Token: 0x06008207 RID: 33287 RVA: 0x002A81C4 File Offset: 0x002A63C4
		public void Reboot(int iChain)
		{
			BoingBones.Bone[] array = this.BoneData[iChain];
			if (array == null)
			{
				return;
			}
			foreach (BoingBones.Bone bone in array)
			{
				bone.Instance.PositionSpring.Reset(bone.Position);
				bone.Instance.RotationSpring.Reset(bone.Rotation);
				bone.CachedPositionWs = bone.Position;
				bone.CachedPositionLs = bone.Transform.localPosition;
				bone.CachedRotationWs = bone.Rotation;
				bone.CachedRotationLs = bone.Transform.localRotation;
				bone.CachedScaleLs = bone.LocalScale;
			}
			this.CachedTransformValid = true;
		}

		// Token: 0x17000C7A RID: 3194
		// (get) Token: 0x06008208 RID: 33288 RVA: 0x002A8269 File Offset: 0x002A6469
		internal float MinScale
		{
			get
			{
				return this.m_minScale;
			}
		}

		// Token: 0x06008209 RID: 33289 RVA: 0x002A8274 File Offset: 0x002A6474
		public override void PrepareExecute()
		{
			base.PrepareExecute();
			this.Params.Bits.SetBit(4, false);
			float fixedDeltaTime = Time.fixedDeltaTime;
			float num = ((this.UpdateMode == BoingManager.UpdateMode.FixedUpdate) ? fixedDeltaTime : Time.deltaTime);
			this.m_minScale = Mathf.Min(base.transform.localScale.x, Mathf.Min(base.transform.localScale.y, base.transform.localScale.z));
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null && !(chain.Root == null) && array.Length != 0)
				{
					Vector3 vector = chain.Gravity * num;
					float num2 = 0f;
					foreach (BoingBones.Bone bone in array)
					{
						if (bone.ParentIndex < 0)
						{
							if (!chain.LooseRoot)
							{
								bone.Instance.PositionSpring.Reset(bone.Position);
								bone.Instance.RotationSpring.Reset(bone.Rotation);
							}
							bone.LengthFromRoot = 0f;
						}
						else
						{
							BoingBones.Bone bone2 = array[bone.ParentIndex];
							float num3 = Vector3.Distance(bone.Position, bone2.Position);
							bone.LengthFromRoot = bone2.LengthFromRoot + num3;
							num2 = Mathf.Max(num2, bone.LengthFromRoot);
						}
					}
					float num4 = MathUtil.InvSafe(num2);
					foreach (BoingBones.Bone bone3 in array)
					{
						float num5 = bone3.LengthFromRoot * num4;
						bone3.AnimationBlend = BoingBones.Chain.EvaluateCurve(chain.AnimationBlendCurveType, num5, chain.AnimationBlendCustomCurve);
						bone3.LengthStiffness = BoingBones.Chain.EvaluateCurve(chain.LengthStiffnessCurveType, num5, chain.LengthStiffnessCustomCurve);
						bone3.LengthStiffnessT = 1f - Mathf.Pow(1f - bone3.LengthStiffness, 30f * fixedDeltaTime);
						bone3.FullyStiffToParentLength = ((bone3.ParentIndex >= 0) ? Vector3.Distance(array[bone3.ParentIndex].Position, bone3.Position) : 0f);
						bone3.PoseStiffness = BoingBones.Chain.EvaluateCurve(chain.PoseStiffnessCurveType, num5, chain.PoseStiffnessCustomCurve);
						bone3.BendAngleCap = chain.MaxBendAngleCap * MathUtil.Deg2Rad * BoingBones.Chain.EvaluateCurve(chain.BendAngleCapCurveType, num5, chain.BendAngleCapCustomCurve);
						bone3.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, num5, chain.CollisionRadiusCustomCurve);
						bone3.SquashAndStretch = BoingBones.Chain.EvaluateCurve(chain.SquashAndStretchCurveType, num5, chain.SquashAndStretchCustomCurve);
					}
					Vector3 position = array[0].Position;
					for (int l = 0; l < array.Length; l++)
					{
						BoingBones.Bone bone4 = array[l];
						float num6 = bone4.LengthFromRoot * num4;
						bone4.AnimationBlend = BoingBones.Chain.EvaluateCurve(chain.AnimationBlendCurveType, num6, chain.AnimationBlendCustomCurve);
						bone4.LengthStiffness = BoingBones.Chain.EvaluateCurve(chain.LengthStiffnessCurveType, num6, chain.LengthStiffnessCustomCurve);
						bone4.PoseStiffness = BoingBones.Chain.EvaluateCurve(chain.PoseStiffnessCurveType, num6, chain.PoseStiffnessCustomCurve);
						bone4.BendAngleCap = chain.MaxBendAngleCap * MathUtil.Deg2Rad * BoingBones.Chain.EvaluateCurve(chain.BendAngleCapCurveType, num6, chain.BendAngleCapCustomCurve);
						bone4.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, num6, chain.CollisionRadiusCustomCurve);
						bone4.SquashAndStretch = BoingBones.Chain.EvaluateCurve(chain.SquashAndStretchCurveType, num6, chain.SquashAndStretchCustomCurve);
						if (l > 0)
						{
							BoingBones.Bone bone5 = bone4;
							bone5.Instance.PositionSpring.Velocity = bone5.Instance.PositionSpring.Velocity + vector;
						}
						bone4.RotationInverseWs = Quaternion.Inverse(bone4.Rotation);
						bone4.SpringRotationWs = bone4.Instance.RotationSpring.ValueQuat;
						bone4.SpringRotationInverseWs = Quaternion.Inverse(bone4.SpringRotationWs);
						Vector3 vector2 = bone4.Position;
						Quaternion quaternion = bone4.Rotation;
						Vector3 localScale = bone4.LocalScale;
						if (bone4.ParentIndex >= 0)
						{
							BoingBones.Bone bone6 = array[bone4.ParentIndex];
							Vector3 position2 = bone6.Position;
							Vector3 value = bone6.Instance.PositionSpring.Value;
							Vector3 vector3 = bone6.SpringRotationInverseWs * (bone4.Instance.PositionSpring.Value - value);
							Quaternion quaternion2 = bone6.SpringRotationInverseWs * bone4.Instance.RotationSpring.ValueQuat;
							Vector3 position3 = bone4.Position;
							Quaternion rotation = bone4.Rotation;
							Vector3 vector4 = bone6.RotationInverseWs * (position3 - position2);
							Quaternion quaternion3 = bone6.RotationInverseWs * rotation;
							float poseStiffness = bone4.PoseStiffness;
							Vector3 vector5 = Vector3.Lerp(vector3, vector4, poseStiffness);
							Quaternion quaternion4 = Quaternion.Slerp(quaternion2, quaternion3, poseStiffness);
							vector2 = value + bone6.SpringRotationWs * vector5;
							quaternion = bone6.SpringRotationWs * quaternion4;
							if (bone4.BendAngleCap < MathUtil.Pi - MathUtil.Epsilon)
							{
								Vector3 vector6 = vector2 - position;
								vector6 = VectorUtil.ClampBend(vector6, position3 - position, bone4.BendAngleCap);
								vector2 = position + vector6;
							}
						}
						if (chain.ParamsOverride == null)
						{
							bone4.Instance.PrepareExecute(ref this.Params, vector2, quaternion, localScale, true);
						}
						else
						{
							bone4.Instance.PrepareExecute(ref chain.ParamsOverride.Params, vector2, quaternion, localScale, true);
						}
					}
				}
			}
		}

		// Token: 0x0600820A RID: 33290 RVA: 0x002A882C File Offset: 0x002A6A2C
		public void AccumulateTarget(ref BoingEffector.Params effector, float dt)
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null && chain.EffectorReaction)
				{
					foreach (BoingBones.Bone bone in array)
					{
						if (chain.ParamsOverride == null)
						{
							bone.Instance.AccumulateTarget(ref this.Params, ref effector, dt);
						}
						else
						{
							Bits32 bits = chain.ParamsOverride.Params.Bits;
							chain.ParamsOverride.Params.Bits = this.Params.Bits;
							bone.Instance.AccumulateTarget(ref chain.ParamsOverride.Params, ref effector, dt);
							chain.ParamsOverride.Params.Bits = bits;
						}
					}
				}
			}
		}

		// Token: 0x0600820B RID: 33291 RVA: 0x002A8914 File Offset: 0x002A6B14
		public void EndAccumulateTargets()
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					foreach (BoingBones.Bone bone in array)
					{
						if (chain.ParamsOverride == null)
						{
							bone.Instance.EndAccumulateTargets(ref this.Params);
						}
						else
						{
							bone.Instance.EndAccumulateTargets(ref chain.ParamsOverride.Params);
						}
					}
				}
			}
		}

		// Token: 0x0600820C RID: 33292 RVA: 0x002A8998 File Offset: 0x002A6B98
		public override void Restore()
		{
			if (!this.CachedTransformValid)
			{
				return;
			}
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						BoingBones.Bone bone = array[j];
						if (j != 0 || chain.LooseRoot)
						{
							bone.Transform.SetLocalPositionAndRotation(bone.CachedPositionLs, bone.CachedRotationLs);
							bone.Transform.localScale = bone.CachedScaleLs;
						}
					}
				}
			}
		}

		// Token: 0x040092DA RID: 37594
		[SerializeField]
		internal BoingBones.Bone[][] BoneData;

		// Token: 0x040092DB RID: 37595
		public BoingBones.Chain[] BoneChains = new BoingBones.Chain[1];

		// Token: 0x040092DC RID: 37596
		public bool TwistPropagation = true;

		// Token: 0x040092DD RID: 37597
		[Range(0.1f, 20f)]
		public float MaxCollisionResolutionSpeed = 3f;

		// Token: 0x040092DE RID: 37598
		public BoingBoneCollider[] BoingColliders = new BoingBoneCollider[0];

		// Token: 0x040092DF RID: 37599
		public Collider[] UnityColliders = new Collider[0];

		// Token: 0x040092E0 RID: 37600
		public bool DebugDrawRawBones;

		// Token: 0x040092E1 RID: 37601
		public bool DebugDrawTargetBones;

		// Token: 0x040092E2 RID: 37602
		public bool DebugDrawBoingBones;

		// Token: 0x040092E3 RID: 37603
		public bool DebugDrawFinalBones;

		// Token: 0x040092E4 RID: 37604
		public bool DebugDrawColliders;

		// Token: 0x040092E5 RID: 37605
		public bool DebugDrawChainBounds;

		// Token: 0x040092E6 RID: 37606
		public bool DebugDrawBoneNames;

		// Token: 0x040092E7 RID: 37607
		public bool DebugDrawLengthFromRoot;

		// Token: 0x040092E8 RID: 37608
		private float m_minScale = 1f;

		// Token: 0x02001422 RID: 5154
		[Serializable]
		public class Bone
		{
			// Token: 0x17000C7B RID: 3195
			// (get) Token: 0x0600820E RID: 33294 RVA: 0x002A8A78 File Offset: 0x002A6C78
			internal Vector3 Position
			{
				get
				{
					this.CheckResetFlags();
					if (this.updatedPos)
					{
						this.updatedPos = false;
						this.position = this.Transform.position;
					}
					return this.position;
				}
			}

			// Token: 0x17000C7C RID: 3196
			// (get) Token: 0x0600820F RID: 33295 RVA: 0x002A8AA6 File Offset: 0x002A6CA6
			internal Quaternion Rotation
			{
				get
				{
					this.CheckResetFlags();
					if (this.updatedRot)
					{
						this.updatedRot = false;
						this.rotation = this.Transform.rotation;
					}
					return this.rotation;
				}
			}

			// Token: 0x17000C7D RID: 3197
			// (get) Token: 0x06008210 RID: 33296 RVA: 0x002A8AD4 File Offset: 0x002A6CD4
			internal Vector3 LocalScale
			{
				get
				{
					this.CheckResetFlags();
					if (this.updatedScale)
					{
						this.updatedScale = false;
						this.localScale = this.Transform.localScale;
					}
					return this.localScale;
				}
			}

			// Token: 0x06008211 RID: 33297 RVA: 0x002A8B04 File Offset: 0x002A6D04
			private void CheckResetFlags()
			{
				if (this.Transform.hasChanged)
				{
					this.updatedPos = (this.updatedRot = (this.updatedScale = true));
					this.Transform.hasChanged = false;
				}
			}

			// Token: 0x06008212 RID: 33298 RVA: 0x002A8B43 File Offset: 0x002A6D43
			internal void UpdateBounds()
			{
				this.Bounds = new Bounds(this.Instance.PositionSpring.Value, 2f * this.CollisionRadius * Vector3.one);
			}

			// Token: 0x06008213 RID: 33299 RVA: 0x002A8B78 File Offset: 0x002A6D78
			internal Bone(Transform transform, int iParent, float lengthFromRoot)
			{
				this.Transform = transform;
				this.RotationInverseWs = Quaternion.identity;
				this.ParentIndex = iParent;
				this.LengthFromRoot = lengthFromRoot;
				this.Instance.Reset();
				this.CachedPositionWs = transform.position;
				this.CachedPositionLs = transform.localPosition;
				this.CachedRotationWs = transform.rotation;
				this.CachedRotationLs = transform.localRotation;
				this.CachedScaleLs = transform.localScale;
				this.AnimationBlend = 0f;
				this.LengthStiffness = 0f;
				this.PoseStiffness = 0f;
				this.BendAngleCap = 180f;
				this.CollisionRadius = 0f;
			}

			// Token: 0x040092E9 RID: 37609
			internal BoingWork.Params.InstanceData Instance;

			// Token: 0x040092EA RID: 37610
			internal Transform Transform;

			// Token: 0x040092EB RID: 37611
			internal Vector3 ScaleWs;

			// Token: 0x040092EC RID: 37612
			internal Vector3 CachedScaleLs;

			// Token: 0x040092ED RID: 37613
			internal Vector3 BlendedPositionWs;

			// Token: 0x040092EE RID: 37614
			internal Vector3 BlendedScaleLs;

			// Token: 0x040092EF RID: 37615
			internal Vector3 CachedPositionWs;

			// Token: 0x040092F0 RID: 37616
			internal Vector3 CachedPositionLs;

			// Token: 0x040092F1 RID: 37617
			internal Bounds Bounds;

			// Token: 0x040092F2 RID: 37618
			internal Quaternion RotationInverseWs;

			// Token: 0x040092F3 RID: 37619
			internal Quaternion SpringRotationWs;

			// Token: 0x040092F4 RID: 37620
			internal Quaternion SpringRotationInverseWs;

			// Token: 0x040092F5 RID: 37621
			internal Quaternion CachedRotationWs;

			// Token: 0x040092F6 RID: 37622
			internal Quaternion CachedRotationLs;

			// Token: 0x040092F7 RID: 37623
			internal Quaternion BlendedRotationWs;

			// Token: 0x040092F8 RID: 37624
			internal Quaternion RotationBackPropDeltaPs;

			// Token: 0x040092F9 RID: 37625
			internal int ParentIndex;

			// Token: 0x040092FA RID: 37626
			internal int[] ChildIndices;

			// Token: 0x040092FB RID: 37627
			internal float LengthFromRoot;

			// Token: 0x040092FC RID: 37628
			internal float AnimationBlend;

			// Token: 0x040092FD RID: 37629
			internal float LengthStiffness;

			// Token: 0x040092FE RID: 37630
			internal float LengthStiffnessT;

			// Token: 0x040092FF RID: 37631
			internal float FullyStiffToParentLength;

			// Token: 0x04009300 RID: 37632
			internal float PoseStiffness;

			// Token: 0x04009301 RID: 37633
			internal float BendAngleCap;

			// Token: 0x04009302 RID: 37634
			internal float CollisionRadius;

			// Token: 0x04009303 RID: 37635
			internal float SquashAndStretch;

			// Token: 0x04009304 RID: 37636
			private bool updatedPos;

			// Token: 0x04009305 RID: 37637
			private bool updatedRot;

			// Token: 0x04009306 RID: 37638
			private bool updatedScale;

			// Token: 0x04009307 RID: 37639
			private Vector3 position;

			// Token: 0x04009308 RID: 37640
			private Quaternion rotation;

			// Token: 0x04009309 RID: 37641
			private Vector3 localScale;
		}

		// Token: 0x02001423 RID: 5155
		[Serializable]
		public class Chain
		{
			// Token: 0x06008214 RID: 33300 RVA: 0x002A8C2C File Offset: 0x002A6E2C
			public static float EvaluateCurve(BoingBones.Chain.CurveType type, float t, AnimationCurve curve)
			{
				switch (type)
				{
				case BoingBones.Chain.CurveType.ConstantOne:
					return 1f;
				case BoingBones.Chain.CurveType.ConstantHalf:
					return 0.5f;
				case BoingBones.Chain.CurveType.ConstantZero:
					return 0f;
				case BoingBones.Chain.CurveType.RootOneTailHalf:
					return 1f - 0.5f * Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootOneTailZero:
					return 1f - Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootHalfTailOne:
					return 0.5f + 0.5f * Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootZeroTailOne:
					return Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.Custom:
					return curve.Evaluate(t);
				default:
					return 0f;
				}
			}

			// Token: 0x0400930A RID: 37642
			[Tooltip("Root Transform object from which to build a chain (or tree if a bone has multiple children) of bouncy boing bones.")]
			public Transform Root;

			// Token: 0x0400930B RID: 37643
			[Tooltip("List of Transform objects to exclude from chain building.")]
			public Transform[] Exclusion;

			// Token: 0x0400930C RID: 37644
			[Tooltip("Enable to allow reaction to boing effectors.")]
			public bool EffectorReaction = true;

			// Token: 0x0400930D RID: 37645
			[Tooltip("Enable to allow root Transform object to be sprung around as well. Otherwise, no effects will be applied to the root Transform object.")]
			public bool LooseRoot;

			// Token: 0x0400930E RID: 37646
			[Tooltip("Assign a SharedParamsOverride asset to override the parameters for this chain. Useful for chains using different parameters than that of the BoingBones component.")]
			public SharedBoingParams ParamsOverride;

			// Token: 0x0400930F RID: 37647
			[ConditionalField(null, null, null, null, null, null, null, Label = "Animation Blend", Tooltip = "Animation blend determines each bone's final transform between the original raw transform and its corresponding boing bone. 1.0 means 100% contribution from raw (or animated) transform. 0.0 means 100% contribution from boing bone.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's animation blend:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType AnimationBlendCurveType = BoingBones.Chain.CurveType.RootOneTailZero;

			// Token: 0x04009310 RID: 37648
			[ConditionalField("AnimationBlendCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve AnimationBlendCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

			// Token: 0x04009311 RID: 37649
			[ConditionalField(null, null, null, null, null, null, null, Label = "Length Stiffness", Tooltip = "Length stiffness determines how much each target bone (target transform each boing bone is sprung towards) tries to maintain original distance from its parent. 1.0 means 100% distance maintenance. 0.0 means 0% distance maintenance.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's length stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType LengthStiffnessCurveType;

			// Token: 0x04009312 RID: 37650
			[ConditionalField("LengthStiffnessCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve LengthStiffnessCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04009313 RID: 37651
			[ConditionalField(null, null, null, null, null, null, null, Label = "Pose Stiffness", Tooltip = "Pose stiffness determines how much each target bone (target transform each boing bone is sprung towards) tries to maintain original transform. 1.0 means 100% original transform maintenance. 0.0 means 0% original transform maintenance.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's pose stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType PoseStiffnessCurveType;

			// Token: 0x04009314 RID: 37652
			[ConditionalField("PoseStiffnessCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve PoseStiffnessCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04009315 RID: 37653
			[ConditionalField(null, null, null, null, null, null, null, Label = "Bend Angle Cap", Tooltip = "Maximum bone bend angle cap.", Min = 0f, Max = 180f)]
			public float MaxBendAngleCap = 180f;

			// Token: 0x04009316 RID: 37654
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Curve Type", Tooltip = "Percentage(0.0 = 0 %; 1.0 = 100 %) of maximum bone bend angle cap.Bend angle cap limits how much each bone can bend relative to the root (in degrees). 1.0 means 100% maximum bend angle cap. 0.0 means 0% maximum bend angle cap.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's pose stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType BendAngleCapCurveType;

			// Token: 0x04009317 RID: 37655
			[ConditionalField("BendAngleCapCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "    Custom Curve")]
			public AnimationCurve BendAngleCapCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04009318 RID: 37656
			[ConditionalField(null, null, null, null, null, null, null, Label = "Collision Radius", Tooltip = "Maximum bone collision radius.")]
			public float MaxCollisionRadius = 0.1f;

			// Token: 0x04009319 RID: 37657
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Curve Type", Tooltip = "Percentage (0.0 = 0%; 1.0 = 100%) of maximum bone collision radius.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's collision radius:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType CollisionRadiusCurveType;

			// Token: 0x0400931A RID: 37658
			[ConditionalField("CollisionRadiusCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "    Custom Curve")]
			public AnimationCurve CollisionRadiusCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x0400931B RID: 37659
			[ConditionalField(null, null, null, null, null, null, null, Label = "Boing Kit Collision", Tooltip = "Enable to allow this chain to collide with Boing Kit's own implementation of lightweight colliders")]
			public bool EnableBoingKitCollision;

			// Token: 0x0400931C RID: 37660
			[ConditionalField(null, null, null, null, null, null, null, Label = "Unity Collision", Tooltip = "Enable to allow this chain to collide with Unity colliders.")]
			public bool EnableUnityCollision;

			// Token: 0x0400931D RID: 37661
			[ConditionalField(null, null, null, null, null, null, null, Label = "Inter-Chain Collision", Tooltip = "Enable to allow this chain to collide with other chain (under the same BoingBones component) with inter-chain collision enabled.")]
			public bool EnableInterChainCollision;

			// Token: 0x0400931E RID: 37662
			public Vector3 Gravity = Vector3.zero;

			// Token: 0x0400931F RID: 37663
			internal Bounds Bounds;

			// Token: 0x04009320 RID: 37664
			[ConditionalField(null, null, null, null, null, null, null, Label = "Squash & Stretch", Tooltip = "Percentage (0.0 = 0%; 1.0 = 100%) of each bone's squash & stretch effect. Squash & stretch is the effect of volume preservation by scaling bones based on how compressed or stretched the distances between bones become.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's squash & stretch effect amount:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType SquashAndStretchCurveType = BoingBones.Chain.CurveType.ConstantZero;

			// Token: 0x04009321 RID: 37665
			[ConditionalField("SquashAndStretchCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve SquashAndStretchCustomCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

			// Token: 0x04009322 RID: 37666
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Max Squash", Tooltip = "Maximum squash amount. For example, 2.0 means a maximum scale of 200% when squashed.", Min = 1f, Max = 5f)]
			public float MaxSquash = 1.1f;

			// Token: 0x04009323 RID: 37667
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Max Stretch", Tooltip = "Maximum stretch amount. For example, 2.0 means a minimum scale of 50% when stretched (200% stretched).", Min = 1f, Max = 5f)]
			public float MaxStretch = 2f;

			// Token: 0x04009324 RID: 37668
			internal Transform m_scannedRoot;

			// Token: 0x04009325 RID: 37669
			internal Transform[] m_scannedExclusion;

			// Token: 0x04009326 RID: 37670
			internal int m_hierarchyHash = -1;

			// Token: 0x04009327 RID: 37671
			internal float MaxLengthFromRoot;

			// Token: 0x02001424 RID: 5156
			public enum CurveType
			{
				// Token: 0x04009329 RID: 37673
				ConstantOne,
				// Token: 0x0400932A RID: 37674
				ConstantHalf,
				// Token: 0x0400932B RID: 37675
				ConstantZero,
				// Token: 0x0400932C RID: 37676
				RootOneTailHalf,
				// Token: 0x0400932D RID: 37677
				RootOneTailZero,
				// Token: 0x0400932E RID: 37678
				RootHalfTailOne,
				// Token: 0x0400932F RID: 37679
				RootZeroTailOne,
				// Token: 0x04009330 RID: 37680
				Custom
			}
		}

		// Token: 0x02001425 RID: 5157
		private class RescanEntry
		{
			// Token: 0x06008216 RID: 33302 RVA: 0x002A8DDC File Offset: 0x002A6FDC
			internal RescanEntry(Transform transform, int iParent, float lengthFromRoot)
			{
				this.Transform = transform;
				this.ParentIndex = iParent;
				this.LengthFromRoot = lengthFromRoot;
			}

			// Token: 0x04009331 RID: 37681
			internal Transform Transform;

			// Token: 0x04009332 RID: 37682
			internal int ParentIndex;

			// Token: 0x04009333 RID: 37683
			internal float LengthFromRoot;
		}
	}
}
