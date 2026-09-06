using System;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using GorillaTag.Gravity;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion
{
	// Token: 0x02001190 RID: 4496
	public class GTPlayerTransform : MonkeGravityController
	{
		// Token: 0x17000B11 RID: 2833
		// (get) Token: 0x06007141 RID: 28993 RVA: 0x0024C30E File Offset: 0x0024A50E
		// (set) Token: 0x06007142 RID: 28994 RVA: 0x0024C315 File Offset: 0x0024A515
		public static float GravityStrength { get; private set; }

		// Token: 0x17000B12 RID: 2834
		// (get) Token: 0x06007143 RID: 28995 RVA: 0x0024C31D File Offset: 0x0024A51D
		// (set) Token: 0x06007144 RID: 28996 RVA: 0x0024C324 File Offset: 0x0024A524
		public static Vector3 GravityForce { get; private set; } = Physics.gravity;

		// Token: 0x17000B13 RID: 2835
		// (get) Token: 0x06007145 RID: 28997 RVA: 0x0024C32C File Offset: 0x0024A52C
		// (set) Token: 0x06007146 RID: 28998 RVA: 0x0024C333 File Offset: 0x0024A533
		public static Vector3 Up { get; private set; } = Vector3.up;

		// Token: 0x17000B14 RID: 2836
		// (get) Token: 0x06007147 RID: 28999 RVA: 0x0024C33B File Offset: 0x0024A53B
		// (set) Token: 0x06007148 RID: 29000 RVA: 0x0024C342 File Offset: 0x0024A542
		public static Vector3 PhysicsUp { get; private set; } = Vector3.up;

		// Token: 0x17000B15 RID: 2837
		// (get) Token: 0x06007149 RID: 29001 RVA: 0x0024C34A File Offset: 0x0024A54A
		// (set) Token: 0x0600714A RID: 29002 RVA: 0x0024C351 File Offset: 0x0024A551
		public static Vector3 Down { get; private set; } = Vector3.down;

		// Token: 0x17000B16 RID: 2838
		// (get) Token: 0x0600714B RID: 29003 RVA: 0x0024C359 File Offset: 0x0024A559
		// (set) Token: 0x0600714C RID: 29004 RVA: 0x0024C360 File Offset: 0x0024A560
		public static Vector3 PhysicsDown { get; private set; } = Vector3.down;

		// Token: 0x17000B17 RID: 2839
		// (get) Token: 0x0600714D RID: 29005 RVA: 0x0024C368 File Offset: 0x0024A568
		// (set) Token: 0x0600714E RID: 29006 RVA: 0x0024C36F File Offset: 0x0024A56F
		public static Vector3 Forward { get; private set; } = Vector3.forward;

		// Token: 0x17000B18 RID: 2840
		// (get) Token: 0x0600714F RID: 29007 RVA: 0x0024C377 File Offset: 0x0024A577
		// (set) Token: 0x06007150 RID: 29008 RVA: 0x0024C37E File Offset: 0x0024A57E
		public static Vector3 Right { get; private set; } = Vector3.right;

		// Token: 0x17000B19 RID: 2841
		// (get) Token: 0x06007151 RID: 29009 RVA: 0x0024C386 File Offset: 0x0024A586
		public static Quaternion BodyRotation
		{
			get
			{
				return GTPlayerTransform.k_bodyTransform.rotation;
			}
		}

		// Token: 0x17000B1A RID: 2842
		// (get) Token: 0x06007152 RID: 29010 RVA: 0x0024C392 File Offset: 0x0024A592
		// (set) Token: 0x06007153 RID: 29011 RVA: 0x0024C399 File Offset: 0x0024A599
		public static bool IgnoreGravityRotation { get; set; } = false;

		// Token: 0x17000B1B RID: 2843
		// (get) Token: 0x06007154 RID: 29012 RVA: 0x0024C3A1 File Offset: 0x0024A5A1
		// (set) Token: 0x06007155 RID: 29013 RVA: 0x0024C3A8 File Offset: 0x0024A5A8
		public static bool IgnoreGravityForce { get; set; } = false;

		// Token: 0x17000B1C RID: 2844
		// (get) Token: 0x06007156 RID: 29014 RVA: 0x0024C3B0 File Offset: 0x0024A5B0
		public static Vector3 RotationPosOffsetChange
		{
			get
			{
				return GTPlayerTransform.k_rotationPosOffsetChange;
			}
		}

		// Token: 0x17000B1D RID: 2845
		// (get) Token: 0x06007157 RID: 29015 RVA: 0x0024C3B7 File Offset: 0x0024A5B7
		// (set) Token: 0x06007158 RID: 29016 RVA: 0x0024C3BE File Offset: 0x0024A5BE
		public static GTPlayerTransform Instance { get; private set; }

		// Token: 0x06007159 RID: 29017 RVA: 0x0024C3C8 File Offset: 0x0024A5C8
		public static void RotateToUp(in Vector3 targetUp)
		{
			if (targetUp == GTPlayerTransform.Up)
			{
				GTPlayerTransform.Instance.ClearRotationRecovery();
				return;
			}
			Vector3 up = GTPlayerTransform.Up;
			GTPlayerTransform.RotateFromToDirection(in up, in targetUp);
		}

		// Token: 0x0600715A RID: 29018 RVA: 0x0024C400 File Offset: 0x0024A600
		public static void RotateFromToDirection(in Vector3 currentDir, in Vector3 targetDir)
		{
			Quaternion rotation = GTPlayerTransform.k_rigidBody.rotation;
			Quaternion quaternion = Quaternion.FromToRotation(currentDir, targetDir) * rotation;
			GTPlayerTransform.SetRotation(in quaternion, in rotation);
		}

		// Token: 0x0600715B RID: 29019 RVA: 0x0024C43C File Offset: 0x0024A63C
		public static void RotateBy(in Quaternion rotation)
		{
			Quaternion rotation2 = GTPlayerTransform.k_transform.rotation;
			Quaternion quaternion = rotation2 * rotation;
			GTPlayerTransform.SetRotation(in quaternion, in rotation2);
		}

		// Token: 0x0600715C RID: 29020 RVA: 0x0024C46C File Offset: 0x0024A66C
		public static void SetRotation(in Quaternion targetRotation)
		{
			Quaternion rotation = GTPlayerTransform.k_rigidBody.rotation;
			GTPlayerTransform.SetRotation(in targetRotation, in rotation);
		}

		// Token: 0x0600715D RID: 29021 RVA: 0x0024C48C File Offset: 0x0024A68C
		private static void SetRotation(in Quaternion newRotation, in Quaternion currentRotation)
		{
			readonly ref GTPlayer.HandState leftHandRef = ref GTPlayerTransform.k_playerInstance.LeftHandRef;
			readonly ref GTPlayer.HandState rightHandRef = ref GTPlayerTransform.k_playerInstance.RightHandRef;
			Vector3 position = GTPlayerTransform.k_rigidBody.position;
			Quaternion quaternion = newRotation * Quaternion.Inverse(currentRotation);
			Vector3 position2 = GTPlayerTransform.k_bodyTransform.position;
			Vector3 vector = GTPlayerTransform.GetRotatedDifference(in position, in position2, in quaternion);
			bool flag = false;
			bool flag2 = false;
			GorillaHandClimber currentClimber = GTPlayerTransform.k_playerInstance.CurrentClimber;
			if (GTPlayerTransform.k_playerInstance.isClimbing)
			{
				flag = currentClimber.xrNode == XRNode.LeftHand;
				flag2 = currentClimber.xrNode == XRNode.RightHand;
			}
			if (leftHandRef.wasColliding || leftHandRef.wasSliding || flag)
			{
				Vector3 vector2 = (flag ? currentClimber.transform.position : leftHandRef.lastPosition);
				Vector3 rotatedDifference = GTPlayerTransform.GetRotatedDifference(in position, in vector2, in quaternion);
				Vector3 vector3 = Vector3.Normalize(rotatedDifference);
				RaycastHit raycastHit;
				if (!flag)
				{
					Vector3 vector4 = vector3;
					raycastHit = leftHandRef.lastHitInfo;
					if (Vector3.Dot(vector4, raycastHit.normal) > 0f)
					{
						goto IL_0114;
					}
				}
				vector = rotatedDifference;
				Vector3 vector5 = vector;
				raycastHit = leftHandRef.lastHitInfo;
				vector = vector5 - raycastHit.normal * 0.0001f;
			}
			IL_0114:
			if (rightHandRef.wasColliding || rightHandRef.wasSliding || flag2)
			{
				Vector3 vector6 = (flag2 ? currentClimber.transform.position : rightHandRef.lastPosition);
				Vector3 rotatedDifference2 = GTPlayerTransform.GetRotatedDifference(in position, in vector6, in quaternion);
				Vector3 vector7 = Vector3.Normalize(rotatedDifference2);
				RaycastHit raycastHit;
				if (!flag2)
				{
					Vector3 vector8 = vector7;
					raycastHit = rightHandRef.lastHitInfo;
					if (Vector3.Dot(vector8, raycastHit.normal) > 0f)
					{
						goto IL_01A1;
					}
				}
				vector = rotatedDifference2;
				Vector3 vector9 = vector;
				raycastHit = rightHandRef.lastHitInfo;
				vector = vector9 - raycastHit.normal * 0.0001f;
			}
			IL_01A1:
			GTPlayerTransform.k_rotationPosOffsetChange -= vector;
			Vector3 vector10 = position - vector;
			GTPlayerTransform.k_rigidBody.position = vector10;
			GTPlayerTransform.k_rigidBody.rotation = newRotation;
			GTPlayerTransform.k_transform.SetPositionAndRotation(vector10, newRotation);
			GTPlayerTransform.Up = newRotation * Vector3.up;
			GTPlayerTransform.Down = GTPlayerTransform.Up * -1f;
			GTPlayerTransform.Forward = newRotation * Vector3.forward;
			GTPlayerTransform.Right = newRotation * Vector3.right;
		}

		// Token: 0x0600715E RID: 29022 RVA: 0x0024C6D8 File Offset: 0x0024A8D8
		private static Vector3 GetRotatedDifference(in Vector3 pivotPoint, in Vector3 worldPoint, in Quaternion rotation)
		{
			Vector3 vector = worldPoint - pivotPoint;
			return rotation * vector - vector;
		}

		// Token: 0x0600715F RID: 29023 RVA: 0x0024C709 File Offset: 0x0024A909
		public static void ApplyRotationOverride(in Quaternion rotation, int frameTime)
		{
			GTPlayerTransform.SetRotation(in rotation);
			GTPlayerTransform.k_rotationOverrideFrameTime = frameTime;
		}

		// Token: 0x06007160 RID: 29024 RVA: 0x0024C717 File Offset: 0x0024A917
		public static void ResetRotationPositionOffset()
		{
			GTPlayerTransform.k_rotationPosOffsetChange = Vector3.zero;
		}

		// Token: 0x06007161 RID: 29025 RVA: 0x0024C724 File Offset: 0x0024A924
		public static void TeleportFromTo(Transform sourceNode, Transform targetNode, bool keepVelocity, bool centre, in Vector3? offset = null)
		{
			Vector3 position = GTPlayerTransform.k_rigidBody.position;
			Quaternion rotation = GTPlayerTransform.k_rigidBody.rotation;
			Vector3 vector = sourceNode.InverseTransformPoint(position);
			Vector3 vector2 = targetNode.TransformPoint(vector);
			if (offset != null)
			{
				vector2 += offset.Value;
			}
			Quaternion quaternion = sourceNode.InverseTransformRotation(rotation);
			Quaternion quaternion2 = targetNode.TransformRotation(quaternion);
			GTPlayerTransform.TeleportTo(in vector2, in quaternion2, in rotation, keepVelocity, centre);
		}

		// Token: 0x06007162 RID: 29026 RVA: 0x0024C790 File Offset: 0x0024A990
		public static void TeleportTo(in Vector3 targetPos, in Quaternion targetRot, bool keepVelocity, bool centre)
		{
			Quaternion rotation = GTPlayerTransform.k_rigidBody.rotation;
			GTPlayerTransform.TeleportTo(in targetPos, in targetRot, in rotation, keepVelocity, centre);
		}

		// Token: 0x06007163 RID: 29027 RVA: 0x0024C7B4 File Offset: 0x0024A9B4
		private static void TeleportTo(in Vector3 targetPos, in Quaternion targetRot, in Quaternion currentRot, bool keepVelocity, bool centre)
		{
			Vector3 vector = targetPos;
			if (centre)
			{
				Vector3 position = GTPlayerTransform.k_rigidBody.position;
				Vector3 vector2 = GTPlayerTransform.k_playerInstance.mainCamera.transform.position - position;
				vector -= vector2;
			}
			GTPlayerTransform.k_rigidBody.isKinematic = true;
			GTPlayerTransform.k_rigidBody.position = vector;
			GTPlayerTransform.k_transform.position = vector;
			GTPlayerTransform.SetRotation(in targetRot, in currentRot);
			GTPlayerTransform.k_rigidBody.isKinematic = false;
			if (keepVelocity)
			{
				Vector3 linearVelocity = GTPlayerTransform.k_rigidBody.linearVelocity;
				GTPlayerTransform.k_rigidBody.linearVelocity = targetRot * Quaternion.Inverse(currentRot) * linearVelocity;
			}
			else
			{
				GTPlayerTransform.k_rigidBody.linearVelocity = Vector3.zero;
			}
			GTPlayerTransform.k_playerInstance.TeleportCleanup();
		}

		// Token: 0x06007164 RID: 29028 RVA: 0x0024C87C File Offset: 0x0024AA7C
		protected override void Awake()
		{
			base.Awake();
			if (!base.Register)
			{
				Debug.LogError("GTPlayerTransform: failed to load required references", base.gameObject);
			}
			GTPlayerTransform.Instance = this;
			GTPlayerTransform.k_transform = this.m_targetTransform;
			GTPlayerTransform.k_rigidBody = this.m_targetRigidBody;
			GTPlayerTransform.k_bodyTransform = this.m_gtPlayerBodyTransform;
			GTPlayerTransform.k_playerInstance = this.m_gtPlayerInstance;
			GTPlayerTransform.GravityStrength = Physics.gravity.magnitude * -1f;
			GTPlayerTransform.GravityForce = Physics.gravity;
			GTPlayerTransform.Up = GTPlayerTransform.k_transform.up;
			GTPlayerTransform.Forward = GTPlayerTransform.k_transform.forward;
			GTPlayerTransform.Right = GTPlayerTransform.k_transform.right;
			GTPlayerTransform.Down = GTPlayerTransform.Up * -1f;
			this.m_globalGravityIntent = false;
		}

		// Token: 0x06007165 RID: 29029 RVA: 0x0024C944 File Offset: 0x0024AB44
		public override void ApplyGravityUpRotation(in Vector3 upDir, float speed)
		{
			if (GTPlayerTransform.IgnoreGravityRotation || GTPlayerTransform.k_rotationOverrideFrameTime >= Time.frameCount - 1)
			{
				return;
			}
			if (base.InstantRotation)
			{
				GTPlayerTransform.RotateToUp(in upDir);
				return;
			}
			float num = Vector3.Angle(GTPlayerTransform.Up, upDir);
			float num2 = num * 0.017453292f;
			if (num > this.m_smallRotationAngleThreshold)
			{
				GTPlayerTransform.k_useFastRotation = true;
			}
			float num3 = (GTPlayerTransform.k_useFastRotation ? speed : (this.m_smallRotationSpeed * Time.fixedDeltaTime));
			Vector3 vector;
			if (num2 <= num3)
			{
				vector = upDir;
				GTPlayerTransform.k_useFastRotation = false;
			}
			else
			{
				Vector3 vector2 = upDir;
				if (Mathf.Approximately(num, 180f))
				{
					switch (this.m_preferredRotationDirection)
					{
					case RotationDirection.Forward:
						vector2 = GTPlayerTransform.k_bodyTransform.forward;
						break;
					case RotationDirection.Backward:
						vector2 = GTPlayerTransform.k_bodyTransform.forward * -1f;
						break;
					case RotationDirection.Left:
						vector2 = GTPlayerTransform.k_bodyTransform.right * -1f;
						break;
					case RotationDirection.Right:
						vector2 = GTPlayerTransform.k_bodyTransform.right;
						break;
					}
				}
				vector = Vector3.RotateTowards(GTPlayerTransform.Up, vector2, num3, 0f);
			}
			GTPlayerTransform.RotateToUp(in vector);
		}

		// Token: 0x06007166 RID: 29030 RVA: 0x0024CA68 File Offset: 0x0024AC68
		public override void ApplyGravityForce(in Vector3 force, ForceMode forceType = ForceMode.Acceleration)
		{
			GTPlayerTransform.GravityForce = force;
			Vector3 vector = GTPlayerTransform.GravityForce;
			GTPlayerTransform.GravityStrength = vector.magnitude * -1f;
			if (GTPlayerTransform.IgnoreGravityForce || GTPlayerTransform.k_playerInstance.isClimbing || GTPlayerTransform.k_playerInstance.GravityOverrideCount > 0)
			{
				return;
			}
			vector = force * GTPlayerTransform.k_playerInstance.scale;
			base.ApplyGravityForce(in vector, forceType);
		}

		// Token: 0x06007167 RID: 29031 RVA: 0x0024CAD8 File Offset: 0x0024ACD8
		public override Vector3 GetWorldPoint()
		{
			return GTPlayerTransform.k_bodyTransform.position;
		}

		// Token: 0x17000B1E RID: 2846
		// (get) Token: 0x06007168 RID: 29032 RVA: 0x0024CAE4 File Offset: 0x0024ACE4
		public override float Scale
		{
			get
			{
				return VRRig.LocalRig.scaleFactor;
			}
		}

		// Token: 0x06007169 RID: 29033 RVA: 0x0024CAF0 File Offset: 0x0024ACF0
		public override void CallBack()
		{
			if (GTPlayerTransform.IgnoreGravityForce || GTPlayerTransform.k_playerInstance.isClimbing || GTPlayerTransform.k_playerInstance.GravityOverrideCount > 0)
			{
				return;
			}
			base.CallBack();
			GTPlayerTransform.PhysicsUp = base.GravityUp;
			GTPlayerTransform.PhysicsDown = base.GravityDown;
			if (base.GravityZonesCount > 0)
			{
				return;
			}
			if (GTPlayerTransform.Up != GTPlayerTransform.PhysicsUp)
			{
				Vector3 physicsUp = GTPlayerTransform.PhysicsUp;
				this.ApplyGravityUpRotation(in physicsUp, MonkeGravityManager.DefaultGravityInfo.rotationSpeed * Time.fixedDeltaTime);
			}
		}

		// Token: 0x040081C5 RID: 33221
		private static Vector3 k_rotationPosOffsetChange = Vector3.zero;

		// Token: 0x040081C7 RID: 33223
		private static Transform k_transform;

		// Token: 0x040081C8 RID: 33224
		private static Rigidbody k_rigidBody;

		// Token: 0x040081C9 RID: 33225
		private static Transform k_bodyTransform;

		// Token: 0x040081CA RID: 33226
		private static GTPlayer k_playerInstance;

		// Token: 0x040081CB RID: 33227
		private static int k_rotationOverrideFrameTime;

		// Token: 0x040081CC RID: 33228
		private static bool k_useFastRotation;

		// Token: 0x040081CD RID: 33229
		[SerializeField]
		private Transform m_gtPlayerBodyTransform;

		// Token: 0x040081CE RID: 33230
		[SerializeField]
		private GTPlayer m_gtPlayerInstance;

		// Token: 0x040081CF RID: 33231
		[Header("Slow rotation for small angles")]
		[Tooltip("If rotating less than this distance (degrees), use the slow speed.")]
		[SerializeField]
		private float m_smallRotationAngleThreshold = 45f;

		// Token: 0x040081D0 RID: 33232
		[Tooltip("Rotation speed (rad/s) used for rotations less than the small-angle threshold.")]
		[SerializeField]
		private float m_smallRotationSpeed = 5f;
	}
}
