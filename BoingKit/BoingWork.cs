using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001448 RID: 5192
	public static class BoingWork
	{
		// Token: 0x060082DD RID: 33501 RVA: 0x002ACD08 File Offset: 0x002AAF08
		internal static Vector3 ComputeTranslationalResults(Transform t, Vector3 src, Vector3 dst, BoingBehavior b)
		{
			if (!b.LockTranslationX && !b.LockTranslationY && !b.LockTranslationZ)
			{
				return dst;
			}
			Vector3 vector = dst - src;
			BoingManager.TranslationLockSpace translationLockSpace = b.TranslationLockSpace;
			if (translationLockSpace != BoingManager.TranslationLockSpace.Global)
			{
				if (translationLockSpace == BoingManager.TranslationLockSpace.Local)
				{
					if (b.LockTranslationX)
					{
						vector -= Vector3.Project(vector, t.right);
					}
					if (b.LockTranslationY)
					{
						vector -= Vector3.Project(vector, t.up);
					}
					if (b.LockTranslationZ)
					{
						vector -= Vector3.Project(vector, t.forward);
					}
				}
			}
			else
			{
				if (b.LockTranslationX)
				{
					vector.x = 0f;
				}
				if (b.LockTranslationY)
				{
					vector.y = 0f;
				}
				if (b.LockTranslationZ)
				{
					vector.z = 0f;
				}
			}
			return src + vector;
		}

		// Token: 0x02001449 RID: 5193
		public enum EffectorFlags
		{
			// Token: 0x040093F2 RID: 37874
			ContinuousMotion
		}

		// Token: 0x0200144A RID: 5194
		public enum ReactorFlags
		{
			// Token: 0x040093F4 RID: 37876
			TwoDDistanceCheck,
			// Token: 0x040093F5 RID: 37877
			TwoDPositionInfluence,
			// Token: 0x040093F6 RID: 37878
			TwoDRotationInfluence,
			// Token: 0x040093F7 RID: 37879
			EnablePositionEffect,
			// Token: 0x040093F8 RID: 37880
			EnableRotationEffect,
			// Token: 0x040093F9 RID: 37881
			EnableScaleEffect,
			// Token: 0x040093FA RID: 37882
			GlobalReactionUpVector,
			// Token: 0x040093FB RID: 37883
			EnablePropagation,
			// Token: 0x040093FC RID: 37884
			AnchorPropagationAtBorder,
			// Token: 0x040093FD RID: 37885
			FixedUpdate,
			// Token: 0x040093FE RID: 37886
			EarlyUpdate,
			// Token: 0x040093FF RID: 37887
			LateUpdate
		}

		// Token: 0x0200144B RID: 5195
		[Serializable]
		public struct Params
		{
			// Token: 0x060082DE RID: 33502 RVA: 0x002ACDE0 File Offset: 0x002AAFE0
			public static void Copy(ref BoingWork.Params from, ref BoingWork.Params to)
			{
				to.PositionParameterMode = from.PositionParameterMode;
				to.RotationParameterMode = from.RotationParameterMode;
				to.PositionExponentialHalfLife = from.PositionExponentialHalfLife;
				to.PositionOscillationHalfLife = from.PositionOscillationHalfLife;
				to.PositionOscillationFrequency = from.PositionOscillationFrequency;
				to.PositionOscillationDampingRatio = from.PositionOscillationDampingRatio;
				to.MoveReactionMultiplier = from.MoveReactionMultiplier;
				to.LinearImpulseMultiplier = from.LinearImpulseMultiplier;
				to.RotationExponentialHalfLife = from.RotationExponentialHalfLife;
				to.RotationOscillationHalfLife = from.RotationOscillationHalfLife;
				to.RotationOscillationFrequency = from.RotationOscillationFrequency;
				to.RotationOscillationDampingRatio = from.RotationOscillationDampingRatio;
				to.RotationReactionMultiplier = from.RotationReactionMultiplier;
				to.AngularImpulseMultiplier = from.AngularImpulseMultiplier;
				to.ScaleExponentialHalfLife = from.ScaleExponentialHalfLife;
				to.ScaleOscillationHalfLife = from.ScaleOscillationHalfLife;
				to.ScaleOscillationFrequency = from.ScaleOscillationFrequency;
				to.ScaleOscillationDampingRatio = from.ScaleOscillationDampingRatio;
			}

			// Token: 0x060082DF RID: 33503 RVA: 0x002ACEC8 File Offset: 0x002AB0C8
			public void Init()
			{
				this.InstanceID = -1;
				this.Bits.Clear();
				this.TwoDPlane = TwoDPlaneEnum.XZ;
				this.PositionParameterMode = ParameterMode.OscillationByHalfLife;
				this.RotationParameterMode = ParameterMode.OscillationByHalfLife;
				this.ScaleParameterMode = ParameterMode.OscillationByHalfLife;
				this.PositionExponentialHalfLife = 0.02f;
				this.PositionOscillationHalfLife = 0.1f;
				this.PositionOscillationFrequency = 5f;
				this.PositionOscillationDampingRatio = 0.5f;
				this.MoveReactionMultiplier = 1f;
				this.LinearImpulseMultiplier = 1f;
				this.RotationExponentialHalfLife = 0.02f;
				this.RotationOscillationHalfLife = 0.1f;
				this.RotationOscillationFrequency = 5f;
				this.RotationOscillationDampingRatio = 0.5f;
				this.RotationReactionMultiplier = 1f;
				this.AngularImpulseMultiplier = 1f;
				this.ScaleExponentialHalfLife = 0.02f;
				this.ScaleOscillationHalfLife = 0.1f;
				this.ScaleOscillationFrequency = 5f;
				this.ScaleOscillationDampingRatio = 0.5f;
				this.Instance.Reset();
			}

			// Token: 0x060082E0 RID: 33504 RVA: 0x002ACFBE File Offset: 0x002AB1BE
			public void AccumulateTarget(ref BoingEffector.Params effector, float dt)
			{
				this.Instance.AccumulateTarget(ref this, ref effector, dt);
			}

			// Token: 0x060082E1 RID: 33505 RVA: 0x002ACFCE File Offset: 0x002AB1CE
			public void EndAccumulateTargets()
			{
				this.Instance.EndAccumulateTargets(ref this);
			}

			// Token: 0x060082E2 RID: 33506 RVA: 0x002ACFDC File Offset: 0x002AB1DC
			public void Execute(float dt)
			{
				this.Instance.Execute(ref this, dt);
			}

			// Token: 0x060082E3 RID: 33507 RVA: 0x002ACFEC File Offset: 0x002AB1EC
			public void Execute(BoingBones bones, float dt)
			{
				float num = bones.MaxCollisionResolutionSpeed * dt;
				for (int i = 0; i < bones.BoneData.Length; i++)
				{
					BoingBones.Chain chain = bones.BoneChains[i];
					BoingBones.Bone[] array = bones.BoneData[i];
					if (array != null)
					{
						foreach (BoingBones.Bone bone in array)
						{
							if (chain.ParamsOverride == null)
							{
								bone.Instance.Execute(ref bones.Params, dt);
							}
							else
							{
								bone.Instance.Execute(ref chain.ParamsOverride.Params, dt);
							}
						}
						BoingBones.Bone bone2 = array[0];
						bone2.ScaleWs = (bone2.BlendedScaleLs = bone2.CachedScaleLs);
						bone2.UpdateBounds();
						chain.Bounds = bone2.Bounds;
						Vector3 position = bone2.Position;
						for (int k = 1; k < array.Length; k++)
						{
							BoingBones.Bone bone3 = array[k];
							BoingBones.Bone bone4 = array[bone3.ParentIndex];
							Vector3 vector = bone4.Instance.PositionSpring.Value - bone3.Instance.PositionSpring.Value;
							Vector3 vector2 = VectorUtil.NormalizeSafe(vector, Vector3.zero);
							float magnitude = vector.magnitude;
							float num2 = magnitude - bone3.FullyStiffToParentLength;
							float num3 = bone3.LengthStiffnessT * num2;
							BoingBones.Bone bone5 = bone3;
							bone5.Instance.PositionSpring.Value = bone5.Instance.PositionSpring.Value + num3 * vector2;
							Vector3 vector3 = Vector3.Project(bone3.Instance.PositionSpring.Velocity, vector2);
							BoingBones.Bone bone6 = bone3;
							bone6.Instance.PositionSpring.Velocity = bone6.Instance.PositionSpring.Velocity - bone3.LengthStiffnessT * vector3;
							if (bone3.BendAngleCap < MathUtil.Pi - MathUtil.Epsilon)
							{
								Vector3 position2 = bone3.Position;
								Vector3 vector4 = bone3.Instance.PositionSpring.Value - position;
								vector4 = VectorUtil.ClampBend(vector4, position2 - position, bone3.BendAngleCap);
								bone3.Instance.PositionSpring.Value = position + vector4;
							}
							if (bone3.SquashAndStretch > 0f)
							{
								float num4 = magnitude * MathUtil.InvSafe(bone3.FullyStiffToParentLength);
								Vector3 vector5 = VectorUtil.ComponentWiseDivSafe(Mathf.Clamp(Mathf.Sqrt(1f / num4), 1f / Mathf.Max(1f, chain.MaxStretch), Mathf.Max(1f, chain.MaxSquash)) * Vector3.one, bone4.ScaleWs);
								bone3.BlendedScaleLs = Vector3.Lerp(Vector3.Lerp(bone3.CachedScaleLs, vector5, bone3.SquashAndStretch), bone3.CachedScaleLs, bone3.AnimationBlend);
							}
							else
							{
								bone3.BlendedScaleLs = bone3.CachedScaleLs;
							}
							bone3.ScaleWs = VectorUtil.ComponentWiseMult(bone4.ScaleWs, bone3.BlendedScaleLs);
							bone3.UpdateBounds();
							chain.Bounds.Encapsulate(bone3.Bounds);
						}
						chain.Bounds.Expand(0.2f * Vector3.one);
						if (chain.EnableBoingKitCollision)
						{
							foreach (BoingBoneCollider boingBoneCollider in bones.BoingColliders)
							{
								if (!(boingBoneCollider == null) && chain.Bounds.Intersects(boingBoneCollider.Bounds))
								{
									foreach (BoingBones.Bone bone7 in array)
									{
										Vector3 vector6;
										if (bone7.Bounds.Intersects(boingBoneCollider.Bounds) && boingBoneCollider.Collide(bone7.Instance.PositionSpring.Value, bones.MinScale * bone7.CollisionRadius, out vector6))
										{
											BoingBones.Bone bone8 = bone7;
											bone8.Instance.PositionSpring.Value = bone8.Instance.PositionSpring.Value + VectorUtil.ClampLength(vector6, 0f, num);
											BoingBones.Bone bone9 = bone7;
											bone9.Instance.PositionSpring.Velocity = bone9.Instance.PositionSpring.Velocity - Vector3.Project(bone7.Instance.PositionSpring.Velocity, vector6);
										}
									}
								}
							}
						}
						SphereCollider sharedSphereCollider = BoingManager.SharedSphereCollider;
						if (chain.EnableUnityCollision && sharedSphereCollider != null)
						{
							sharedSphereCollider.enabled = true;
							foreach (Collider collider in bones.UnityColliders)
							{
								if (!(collider == null) && chain.Bounds.Intersects(collider.bounds))
								{
									foreach (BoingBones.Bone bone10 in array)
									{
										if (bone10.Bounds.Intersects(collider.bounds))
										{
											sharedSphereCollider.center = bone10.Instance.PositionSpring.Value;
											sharedSphereCollider.radius = bone10.CollisionRadius;
											Vector3 vector7;
											float num5;
											if (Physics.ComputePenetration(sharedSphereCollider, Vector3.zero, Quaternion.identity, collider, collider.transform.position, collider.transform.rotation, out vector7, out num5))
											{
												BoingBones.Bone bone11 = bone10;
												bone11.Instance.PositionSpring.Value = bone11.Instance.PositionSpring.Value + VectorUtil.ClampLength(vector7 * num5, 0f, num);
												BoingBones.Bone bone12 = bone10;
												bone12.Instance.PositionSpring.Velocity = bone12.Instance.PositionSpring.Velocity - Vector3.Project(bone10.Instance.PositionSpring.Velocity, vector7);
											}
										}
									}
								}
							}
							sharedSphereCollider.enabled = false;
						}
						if (chain.EnableInterChainCollision)
						{
							foreach (BoingBones.Bone bone13 in array)
							{
								for (int n = i + 1; n < bones.BoneData.Length; n++)
								{
									BoingBones.Chain chain2 = bones.BoneChains[n];
									BoingBones.Bone[] array3 = bones.BoneData[n];
									if (array3 != null && chain2.EnableInterChainCollision && chain.Bounds.Intersects(chain2.Bounds))
									{
										foreach (BoingBones.Bone bone14 in array3)
										{
											Vector3 vector8;
											if (Collision.SphereSphere(bone13.Instance.PositionSpring.Value, bones.MinScale * bone13.CollisionRadius, bone14.Instance.PositionSpring.Value, bones.MinScale * bone14.CollisionRadius, out vector8))
											{
												vector8 = VectorUtil.ClampLength(vector8, 0f, num);
												float num6 = bone14.CollisionRadius * MathUtil.InvSafe(bone13.CollisionRadius + bone14.CollisionRadius);
												BoingBones.Bone bone15 = bone13;
												bone15.Instance.PositionSpring.Value = bone15.Instance.PositionSpring.Value + num6 * vector8;
												BoingBones.Bone bone16 = bone14;
												bone16.Instance.PositionSpring.Value = bone16.Instance.PositionSpring.Value - (1f - num6) * vector8;
												BoingBones.Bone bone17 = bone13;
												bone17.Instance.PositionSpring.Velocity = bone17.Instance.PositionSpring.Velocity - Vector3.Project(bone13.Instance.PositionSpring.Velocity, vector8);
												BoingBones.Bone bone18 = bone14;
												bone18.Instance.PositionSpring.Velocity = bone18.Instance.PositionSpring.Velocity - Vector3.Project(bone14.Instance.PositionSpring.Velocity, vector8);
											}
										}
									}
								}
							}
						}
					}
				}
			}

			// Token: 0x060082E4 RID: 33508 RVA: 0x002AD7BF File Offset: 0x002AB9BF
			public void PullResults(BoingBones bones)
			{
				this.Instance.PullResults(bones);
			}

			// Token: 0x060082E5 RID: 33509 RVA: 0x002AD7CD File Offset: 0x002AB9CD
			private void SuppressWarnings()
			{
				this.m_padding0 = 0;
				this.m_padding1 = 0;
				this.m_padding2 = 0f;
				this.m_padding0 = this.m_padding1;
				this.m_padding1 = this.m_padding0;
				this.m_padding2 = (float)this.m_padding0;
			}

			// Token: 0x04009400 RID: 37888
			public static readonly int Stride = 112 + BoingWork.Params.InstanceData.Stride;

			// Token: 0x04009401 RID: 37889
			public int InstanceID;

			// Token: 0x04009402 RID: 37890
			public Bits32 Bits;

			// Token: 0x04009403 RID: 37891
			public TwoDPlaneEnum TwoDPlane;

			// Token: 0x04009404 RID: 37892
			private int m_padding0;

			// Token: 0x04009405 RID: 37893
			public ParameterMode PositionParameterMode;

			// Token: 0x04009406 RID: 37894
			public ParameterMode RotationParameterMode;

			// Token: 0x04009407 RID: 37895
			public ParameterMode ScaleParameterMode;

			// Token: 0x04009408 RID: 37896
			private int m_padding1;

			// Token: 0x04009409 RID: 37897
			[Range(0f, 5f)]
			public float PositionExponentialHalfLife;

			// Token: 0x0400940A RID: 37898
			[Range(0f, 5f)]
			public float PositionOscillationHalfLife;

			// Token: 0x0400940B RID: 37899
			[Range(0f, 10f)]
			public float PositionOscillationFrequency;

			// Token: 0x0400940C RID: 37900
			[Range(0f, 1f)]
			public float PositionOscillationDampingRatio;

			// Token: 0x0400940D RID: 37901
			[Range(0f, 10f)]
			public float MoveReactionMultiplier;

			// Token: 0x0400940E RID: 37902
			[Range(0f, 10f)]
			public float LinearImpulseMultiplier;

			// Token: 0x0400940F RID: 37903
			[Range(0f, 5f)]
			public float RotationExponentialHalfLife;

			// Token: 0x04009410 RID: 37904
			[Range(0f, 5f)]
			public float RotationOscillationHalfLife;

			// Token: 0x04009411 RID: 37905
			[Range(0f, 10f)]
			public float RotationOscillationFrequency;

			// Token: 0x04009412 RID: 37906
			[Range(0f, 1f)]
			public float RotationOscillationDampingRatio;

			// Token: 0x04009413 RID: 37907
			[Range(0f, 10f)]
			public float RotationReactionMultiplier;

			// Token: 0x04009414 RID: 37908
			[Range(0f, 10f)]
			public float AngularImpulseMultiplier;

			// Token: 0x04009415 RID: 37909
			[Range(0f, 5f)]
			public float ScaleExponentialHalfLife;

			// Token: 0x04009416 RID: 37910
			[Range(0f, 5f)]
			public float ScaleOscillationHalfLife;

			// Token: 0x04009417 RID: 37911
			[Range(0f, 10f)]
			public float ScaleOscillationFrequency;

			// Token: 0x04009418 RID: 37912
			[Range(0f, 1f)]
			public float ScaleOscillationDampingRatio;

			// Token: 0x04009419 RID: 37913
			public Vector3 RotationReactionUp;

			// Token: 0x0400941A RID: 37914
			private float m_padding2;

			// Token: 0x0400941B RID: 37915
			public BoingWork.Params.InstanceData Instance;

			// Token: 0x0200144C RID: 5196
			public struct InstanceData
			{
				// Token: 0x060082E7 RID: 33511 RVA: 0x002AD81C File Offset: 0x002ABA1C
				public void Reset()
				{
					this.PositionSpring.Reset();
					this.RotationSpring.Reset();
					this.ScaleSpring.Reset(Vector3.one, Vector3.zero);
					this.PositionPropagationWorkData = Vector3.zero;
					this.RotationPropagationWorkData = Vector3.zero;
				}

				// Token: 0x060082E8 RID: 33512 RVA: 0x002AD870 File Offset: 0x002ABA70
				public void Reset(Vector3 position, bool instantAccumulation)
				{
					this.PositionSpring.Reset(position);
					this.RotationSpring.Reset();
					this.ScaleSpring.Reset(Vector3.one, Vector3.zero);
					this.PositionPropagationWorkData = Vector3.zero;
					this.RotationPropagationWorkData = Vector3.zero;
					this.m_instantAccumulation = (instantAccumulation ? 1 : 0);
				}

				// Token: 0x060082E9 RID: 33513 RVA: 0x002AD8D4 File Offset: 0x002ABAD4
				public void PrepareExecute(ref BoingWork.Params p, Vector3 position, Quaternion rotation, Vector3 scale, bool accumulateEffectors)
				{
					this.PositionOrigin = position;
					this.PositionTarget = position;
					this.RotationTarget = (this.RotationOrigin = QuaternionUtil.ToVector4(rotation));
					this.ScaleTarget = scale;
					this.m_minScale = VectorUtil.MinComponent(scale);
					if (accumulateEffectors)
					{
						this.PositionTarget = Vector3.zero;
						this.RotationTarget = Vector4.zero;
						this.m_numEffectors = 0;
						this.m_upWs = (p.Bits.IsBitSet(6) ? p.RotationReactionUp : (rotation * VectorUtil.NormalizeSafe(p.RotationReactionUp, Vector3.up)));
						return;
					}
					this.m_numEffectors = -1;
					this.m_upWs = Vector3.zero;
				}

				// Token: 0x060082EA RID: 33514 RVA: 0x002AD990 File Offset: 0x002ABB90
				public void PrepareExecute(ref BoingWork.Params p, Vector3 gridCenter, Quaternion gridRotation, Vector3 cellOffset)
				{
					this.PositionOrigin = gridCenter + cellOffset;
					this.RotationOrigin = QuaternionUtil.ToVector4(Quaternion.identity);
					this.PositionTarget = Vector3.zero;
					this.RotationTarget = Vector4.zero;
					this.m_numEffectors = 0;
					this.m_upWs = (p.Bits.IsBitSet(6) ? p.RotationReactionUp : (gridRotation * VectorUtil.NormalizeSafe(p.RotationReactionUp, Vector3.up)));
					this.m_minScale = 1f;
				}

				// Token: 0x060082EB RID: 33515 RVA: 0x002ADA24 File Offset: 0x002ABC24
				public void AccumulateTarget(ref BoingWork.Params p, ref BoingEffector.Params effector, float dt)
				{
					Vector3 vector = (effector.Bits.IsBitSet(0) ? VectorUtil.GetClosestPointOnSegment(this.PositionOrigin, effector.PrevPosition, effector.CurrPosition) : effector.CurrPosition);
					Vector3 vector2 = this.PositionOrigin - vector;
					Vector3 vector3 = vector2;
					if (p.Bits.IsBitSet(0))
					{
						switch (p.TwoDPlane)
						{
						case TwoDPlaneEnum.XY:
							vector2.z = 0f;
							break;
						case TwoDPlaneEnum.XZ:
							vector2.y = 0f;
							break;
						case TwoDPlaneEnum.YZ:
							vector2.x = 0f;
							break;
						}
					}
					if (Mathf.Abs(vector2.x) > effector.Radius || Mathf.Abs(vector2.y) > effector.Radius || Mathf.Abs(vector2.z) > effector.Radius || vector2.sqrMagnitude > effector.Radius * effector.Radius)
					{
						return;
					}
					float magnitude = vector2.magnitude;
					float num = ((effector.Radius - effector.FullEffectRadius > MathUtil.Epsilon) ? (1f - Mathf.Clamp01((magnitude - effector.FullEffectRadius) / (effector.Radius - effector.FullEffectRadius))) : 1f);
					Vector3 vector4 = this.m_upWs;
					Vector3 vector5 = this.m_upWs;
					Vector3 vector6 = VectorUtil.NormalizeSafe(vector3, this.m_upWs);
					Vector3 vector7 = vector6;
					if (p.Bits.IsBitSet(1))
					{
						switch (p.TwoDPlane)
						{
						case TwoDPlaneEnum.XY:
							vector6.z = 0f;
							vector4.z = 0f;
							break;
						case TwoDPlaneEnum.XZ:
							vector6.y = 0f;
							vector4.y = 0f;
							break;
						case TwoDPlaneEnum.YZ:
							vector6.x = 0f;
							vector4.x = 0f;
							break;
						}
						if (vector4.sqrMagnitude < MathUtil.Epsilon)
						{
							switch (p.TwoDPlane)
							{
							case TwoDPlaneEnum.XY:
								vector4 = Vector3.up;
								break;
							case TwoDPlaneEnum.XZ:
								vector4 = Vector3.forward;
								break;
							case TwoDPlaneEnum.YZ:
								vector4 = Vector3.up;
								break;
							}
						}
						else
						{
							vector4.Normalize();
						}
						vector6 = VectorUtil.NormalizeSafe(vector6, vector4);
					}
					if (p.Bits.IsBitSet(2))
					{
						switch (p.TwoDPlane)
						{
						case TwoDPlaneEnum.XY:
							vector7.z = 0f;
							vector5.z = 0f;
							break;
						case TwoDPlaneEnum.XZ:
							vector7.y = 0f;
							vector5.y = 0f;
							break;
						case TwoDPlaneEnum.YZ:
							vector7.x = 0f;
							vector5.x = 0f;
							break;
						}
						if (vector5.sqrMagnitude < MathUtil.Epsilon)
						{
							switch (p.TwoDPlane)
							{
							case TwoDPlaneEnum.XY:
								vector5 = Vector3.up;
								break;
							case TwoDPlaneEnum.XZ:
								vector5 = Vector3.forward;
								break;
							case TwoDPlaneEnum.YZ:
								vector5 = Vector3.up;
								break;
							}
						}
						else
						{
							vector5.Normalize();
						}
						vector7 = VectorUtil.NormalizeSafe(vector7, vector5);
					}
					if (p.Bits.IsBitSet(3))
					{
						Vector3 vector8 = num * p.MoveReactionMultiplier * effector.MoveDistance * vector6;
						this.PositionTarget += vector8;
						this.PositionSpring.Velocity = this.PositionSpring.Velocity + num * p.LinearImpulseMultiplier * effector.LinearImpulse * effector.LinearVelocityDir * (60f * dt);
					}
					if (p.Bits.IsBitSet(4))
					{
						Vector3 vector9 = VectorUtil.NormalizeSafe(Vector3.Cross(vector5, vector7), VectorUtil.FindOrthogonal(vector5));
						Vector3 vector10 = num * p.RotationReactionMultiplier * effector.RotateAngle * vector9;
						this.RotationTarget += QuaternionUtil.ToVector4(QuaternionUtil.FromAngularVector(vector10));
						Vector3 vector11 = VectorUtil.NormalizeSafe(Vector3.Cross(effector.LinearVelocityDir, vector7 - 0.01f * Vector3.up), vector9);
						float num2 = num * p.AngularImpulseMultiplier * effector.AngularImpulse * (60f * dt);
						Vector4 vector12 = QuaternionUtil.ToVector4(QuaternionUtil.FromAngularVector(vector11));
						this.RotationSpring.VelocityVec = this.RotationSpring.VelocityVec + num2 * vector12;
					}
					this.m_numEffectors++;
				}

				// Token: 0x060082EC RID: 33516 RVA: 0x002ADECC File Offset: 0x002AC0CC
				public void EndAccumulateTargets(ref BoingWork.Params p)
				{
					if (this.m_numEffectors > 0)
					{
						this.PositionTarget *= this.m_minScale / (float)this.m_numEffectors;
						this.PositionTarget += this.PositionOrigin;
						this.RotationTarget /= (float)this.m_numEffectors;
						this.RotationTarget = QuaternionUtil.ToVector4(QuaternionUtil.FromVector4(this.RotationTarget, true) * QuaternionUtil.FromVector4(this.RotationOrigin, true));
						return;
					}
					this.PositionTarget = this.PositionOrigin;
					this.RotationTarget = this.RotationOrigin;
				}

				// Token: 0x060082ED RID: 33517 RVA: 0x002ADF74 File Offset: 0x002AC174
				public void Execute(ref BoingWork.Params p, float dt)
				{
					bool flag = this.m_numEffectors >= 0;
					bool flag2 = (flag ? (this.PositionSpring.Velocity.sqrMagnitude > MathUtil.Epsilon || (this.PositionSpring.Value - this.PositionTarget).sqrMagnitude > MathUtil.Epsilon) : p.Bits.IsBitSet(3));
					bool flag3 = (flag ? (this.RotationSpring.VelocityVec.sqrMagnitude > MathUtil.Epsilon || (this.RotationSpring.ValueVec - this.RotationTarget).sqrMagnitude > MathUtil.Epsilon) : p.Bits.IsBitSet(4));
					bool flag4 = p.Bits.IsBitSet(5) && (this.ScaleSpring.Value - this.ScaleTarget).sqrMagnitude > MathUtil.Epsilon;
					if (this.m_numEffectors == 0)
					{
						bool flag5 = true;
						if (flag2)
						{
							flag5 = false;
						}
						else
						{
							this.PositionSpring.Reset(this.PositionTarget);
						}
						if (flag3)
						{
							flag5 = false;
						}
						else
						{
							this.RotationSpring.Reset(QuaternionUtil.FromVector4(this.RotationTarget, true));
						}
						if (flag5)
						{
							return;
						}
					}
					if (this.m_instantAccumulation != 0)
					{
						this.PositionSpring.Value = this.PositionTarget;
						this.RotationSpring.ValueVec = this.RotationTarget;
						this.ScaleSpring.Value = this.ScaleTarget;
						this.m_instantAccumulation = 0;
					}
					else
					{
						if (flag2)
						{
							switch (p.PositionParameterMode)
							{
							case ParameterMode.Exponential:
								this.PositionSpring.TrackExponential(this.PositionTarget, p.PositionExponentialHalfLife, dt);
								break;
							case ParameterMode.OscillationByHalfLife:
								this.PositionSpring.TrackHalfLife(this.PositionTarget, p.PositionOscillationFrequency, p.PositionOscillationHalfLife, dt);
								break;
							case ParameterMode.OscillationByDampingRatio:
								this.PositionSpring.TrackDampingRatio(this.PositionTarget, p.PositionOscillationFrequency * MathUtil.TwoPi, p.PositionOscillationDampingRatio, dt);
								break;
							}
						}
						else
						{
							this.PositionSpring.Value = this.PositionTarget;
							this.PositionSpring.Velocity = Vector3.zero;
						}
						if (flag3)
						{
							switch (p.RotationParameterMode)
							{
							case ParameterMode.Exponential:
								this.RotationSpring.TrackExponential(this.RotationTarget, p.RotationExponentialHalfLife, dt);
								break;
							case ParameterMode.OscillationByHalfLife:
								this.RotationSpring.TrackHalfLife(this.RotationTarget, p.RotationOscillationFrequency, p.RotationOscillationHalfLife, dt);
								break;
							case ParameterMode.OscillationByDampingRatio:
								this.RotationSpring.TrackDampingRatio(this.RotationTarget, p.RotationOscillationFrequency * MathUtil.TwoPi, p.RotationOscillationDampingRatio, dt);
								break;
							}
						}
						else
						{
							this.RotationSpring.ValueVec = this.RotationTarget;
							this.RotationSpring.VelocityVec = Vector4.zero;
						}
						if (flag4)
						{
							switch (p.ScaleParameterMode)
							{
							case ParameterMode.Exponential:
								this.ScaleSpring.TrackExponential(this.ScaleTarget, p.ScaleExponentialHalfLife, dt);
								break;
							case ParameterMode.OscillationByHalfLife:
								this.ScaleSpring.TrackHalfLife(this.ScaleTarget, p.ScaleOscillationFrequency, p.ScaleOscillationHalfLife, dt);
								break;
							case ParameterMode.OscillationByDampingRatio:
								this.ScaleSpring.TrackDampingRatio(this.ScaleTarget, p.ScaleOscillationFrequency * MathUtil.TwoPi, p.ScaleOscillationDampingRatio, dt);
								break;
							}
						}
						else
						{
							this.ScaleSpring.Value = this.ScaleTarget;
							this.ScaleSpring.Velocity = Vector3.zero;
						}
					}
					if (!flag)
					{
						if (!flag2)
						{
							this.PositionSpring.Reset(this.PositionTarget);
						}
						if (!flag3)
						{
							this.RotationSpring.Reset(this.RotationTarget);
						}
					}
				}

				// Token: 0x060082EE RID: 33518 RVA: 0x002AE324 File Offset: 0x002AC524
				public void PullResults(BoingBones bones)
				{
					for (int i = 0; i < bones.BoneData.Length; i++)
					{
						BoingBones.Chain chain = bones.BoneChains[i];
						BoingBones.Bone[] array = bones.BoneData[i];
						if (array != null)
						{
							foreach (BoingBones.Bone bone in array)
							{
								bone.CachedPositionWs = bone.Position;
								bone.CachedPositionLs = bone.Transform.localPosition;
								bone.CachedRotationWs = bone.Rotation;
								bone.CachedRotationLs = bone.Transform.localRotation;
								bone.CachedScaleLs = bone.LocalScale;
							}
							for (int k = 0; k < array.Length; k++)
							{
								BoingBones.Bone bone2 = array[k];
								if (k == 0 && !chain.LooseRoot)
								{
									bone2.BlendedPositionWs = bone2.CachedPositionWs;
								}
								else
								{
									bone2.BlendedPositionWs = Vector3.Lerp(bone2.Instance.PositionSpring.Value, bone2.CachedPositionWs, bone2.AnimationBlend);
								}
							}
							for (int l = 0; l < array.Length; l++)
							{
								BoingBones.Bone bone3 = array[l];
								if (l == 0 && !chain.LooseRoot)
								{
									bone3.BlendedRotationWs = bone3.CachedRotationWs;
								}
								else if (bone3.ChildIndices == null)
								{
									if (bone3.ParentIndex >= 0)
									{
										BoingBones.Bone bone4 = array[bone3.ParentIndex];
										bone3.BlendedRotationWs = bone4.BlendedRotationWs * (bone4.RotationInverseWs * bone3.CachedRotationWs);
									}
								}
								else
								{
									Vector3 cachedPositionWs = bone3.CachedPositionWs;
									Vector3 vector = BoingWork.ComputeTranslationalResults(bone3.Transform, cachedPositionWs, bone3.BlendedPositionWs, bones);
									Quaternion quaternion = (bones.TwistPropagation ? bone3.SpringRotationWs : bone3.CachedRotationWs);
									Quaternion quaternion2 = Quaternion.Inverse(quaternion);
									if (bones.EnableRotationEffect)
									{
										Vector4 vector2 = Vector3.zero;
										float num = 0f;
										foreach (int num2 in bone3.ChildIndices)
										{
											if (num2 >= 0)
											{
												BoingBones.Bone bone5 = array[num2];
												Vector3 cachedPositionWs2 = bone5.CachedPositionWs;
												Vector3 vector3 = VectorUtil.NormalizeSafe(cachedPositionWs2 - cachedPositionWs, Vector3.zero);
												Vector3 vector4 = VectorUtil.NormalizeSafe(BoingWork.ComputeTranslationalResults(bone5.Transform, cachedPositionWs2, bone5.BlendedPositionWs, bones) - vector, Vector3.zero);
												Quaternion quaternion3 = Quaternion.FromToRotation(vector3, vector4);
												Vector4 vector5 = QuaternionUtil.ToVector4(quaternion2 * quaternion3);
												float num3 = Mathf.Max(MathUtil.Epsilon, chain.MaxLengthFromRoot - bone5.LengthFromRoot);
												vector2 += num3 * vector5;
												num += num3;
											}
										}
										if (num > 0f)
										{
											Vector4 vector6 = vector2 / num;
											bone3.RotationBackPropDeltaPs = QuaternionUtil.FromVector4(vector6, true);
											bone3.BlendedRotationWs = quaternion * bone3.RotationBackPropDeltaPs * quaternion;
										}
										else if (bone3.ParentIndex >= 0)
										{
											BoingBones.Bone bone6 = array[bone3.ParentIndex];
											bone3.BlendedRotationWs = bone6.BlendedRotationWs * (bone6.RotationInverseWs * quaternion);
										}
									}
								}
							}
							for (int m = 0; m < array.Length; m++)
							{
								BoingBones.Bone bone7 = array[m];
								if (m == 0 && !chain.LooseRoot)
								{
									bone7.Instance.PositionSpring.Reset(bone7.CachedPositionWs);
									bone7.Instance.RotationSpring.Reset(bone7.CachedRotationWs);
								}
								else
								{
									bone7.Transform.SetPositionAndRotation(BoingWork.ComputeTranslationalResults(bone7.Transform, bone7.Position, bone7.BlendedPositionWs, bones), bone7.BlendedRotationWs);
									bone7.Transform.localScale = bone7.BlendedScaleLs;
								}
							}
						}
					}
				}

				// Token: 0x060082EF RID: 33519 RVA: 0x002AE704 File Offset: 0x002AC904
				private void SuppressWarnings()
				{
					this.m_padding0 = 0f;
					this.m_padding1 = 0f;
					this.m_padding2 = 0f;
					this.m_padding3 = 0;
					this.m_padding4 = 0;
					this.m_padding5 = 0f;
					this.m_padding0 = this.m_padding1;
					this.m_padding1 = this.m_padding2;
					this.m_padding2 = (float)this.m_padding3;
					this.m_padding3 = this.m_padding4;
					this.m_padding4 = (int)this.m_padding0;
					this.m_padding5 = this.m_padding0;
				}

				// Token: 0x0400941C RID: 37916
				public static readonly int Stride = 144 + 2 * Vector3Spring.Stride + QuaternionSpring.Stride;

				// Token: 0x0400941D RID: 37917
				public Vector3 PositionTarget;

				// Token: 0x0400941E RID: 37918
				private float m_padding0;

				// Token: 0x0400941F RID: 37919
				public Vector3 PositionOrigin;

				// Token: 0x04009420 RID: 37920
				private float m_padding1;

				// Token: 0x04009421 RID: 37921
				public Vector4 RotationTarget;

				// Token: 0x04009422 RID: 37922
				public Vector4 RotationOrigin;

				// Token: 0x04009423 RID: 37923
				public Vector3 ScaleTarget;

				// Token: 0x04009424 RID: 37924
				private float m_padding2;

				// Token: 0x04009425 RID: 37925
				private int m_numEffectors;

				// Token: 0x04009426 RID: 37926
				private int m_instantAccumulation;

				// Token: 0x04009427 RID: 37927
				private int m_padding3;

				// Token: 0x04009428 RID: 37928
				private int m_padding4;

				// Token: 0x04009429 RID: 37929
				private Vector3 m_upWs;

				// Token: 0x0400942A RID: 37930
				private float m_minScale;

				// Token: 0x0400942B RID: 37931
				public Vector3Spring PositionSpring;

				// Token: 0x0400942C RID: 37932
				public QuaternionSpring RotationSpring;

				// Token: 0x0400942D RID: 37933
				public Vector3Spring ScaleSpring;

				// Token: 0x0400942E RID: 37934
				public Vector3 PositionPropagationWorkData;

				// Token: 0x0400942F RID: 37935
				private float m_padding5;

				// Token: 0x04009430 RID: 37936
				public Vector4 RotationPropagationWorkData;
			}
		}

		// Token: 0x0200144D RID: 5197
		public struct Output
		{
			// Token: 0x060082F1 RID: 33521 RVA: 0x002AE7B0 File Offset: 0x002AC9B0
			public Output(int instanceID, ref Vector3Spring positionSpring, ref QuaternionSpring rotationSpring, ref Vector3Spring scaleSpring)
			{
				this.InstanceID = instanceID;
				this.m_padding0 = (this.m_padding1 = (this.m_padding2 = 0));
				this.PositionSpring = positionSpring;
				this.RotationSpring = rotationSpring;
				this.ScaleSpring = scaleSpring;
			}

			// Token: 0x060082F2 RID: 33522 RVA: 0x002AE804 File Offset: 0x002ACA04
			public void GatherOutput(Dictionary<int, BoingBehavior> behaviorMap, BoingManager.UpdateMode updateMode)
			{
				BoingBehavior boingBehavior;
				if (!behaviorMap.TryGetValue(this.InstanceID, out boingBehavior))
				{
					return;
				}
				if (!boingBehavior.isActiveAndEnabled)
				{
					return;
				}
				if (boingBehavior.UpdateMode != updateMode)
				{
					return;
				}
				boingBehavior.GatherOutput(ref this);
			}

			// Token: 0x060082F3 RID: 33523 RVA: 0x002AE83C File Offset: 0x002ACA3C
			public void GatherOutput(Dictionary<int, BoingReactor> reactorMap, BoingManager.UpdateMode updateMode)
			{
				BoingReactor boingReactor;
				if (!reactorMap.TryGetValue(this.InstanceID, out boingReactor))
				{
					return;
				}
				if (!boingReactor.isActiveAndEnabled)
				{
					return;
				}
				if (boingReactor.UpdateMode != updateMode)
				{
					return;
				}
				boingReactor.GatherOutput(ref this);
			}

			// Token: 0x060082F4 RID: 33524 RVA: 0x002AE874 File Offset: 0x002ACA74
			private void SuppressWarnings()
			{
				this.m_padding0 = 0;
				this.m_padding1 = 0;
				this.m_padding2 = 0;
				this.m_padding0 = this.m_padding1;
				this.m_padding1 = this.m_padding2;
				this.m_padding2 = this.m_padding0;
			}

			// Token: 0x04009431 RID: 37937
			public static readonly int Stride = 16 + Vector3Spring.Stride + QuaternionSpring.Stride;

			// Token: 0x04009432 RID: 37938
			public int InstanceID;

			// Token: 0x04009433 RID: 37939
			public int m_padding0;

			// Token: 0x04009434 RID: 37940
			public int m_padding1;

			// Token: 0x04009435 RID: 37941
			public int m_padding2;

			// Token: 0x04009436 RID: 37942
			public Vector3Spring PositionSpring;

			// Token: 0x04009437 RID: 37943
			public QuaternionSpring RotationSpring;

			// Token: 0x04009438 RID: 37944
			public Vector3Spring ScaleSpring;
		}
	}
}
