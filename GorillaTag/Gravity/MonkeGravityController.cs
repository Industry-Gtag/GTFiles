using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x0200123E RID: 4670
	public class MonkeGravityController : MonoBehaviour, ICallbackUnique, ICallBack
	{
		// Token: 0x17000B77 RID: 2935
		// (get) Token: 0x06007652 RID: 30290 RVA: 0x0026658E File Offset: 0x0026478E
		public Collider ActivatorCollider
		{
			get
			{
				return this.m_activatorCollider;
			}
		}

		// Token: 0x17000B78 RID: 2936
		// (get) Token: 0x06007653 RID: 30291 RVA: 0x00266596 File Offset: 0x00264796
		public Rigidbody TargetRigidBody
		{
			get
			{
				return this.m_targetRigidBody;
			}
		}

		// Token: 0x17000B79 RID: 2937
		// (get) Token: 0x06007654 RID: 30292 RVA: 0x0026659E File Offset: 0x0026479E
		public Transform TargetTransform
		{
			get
			{
				return this.m_targetTransform;
			}
		}

		// Token: 0x17000B7A RID: 2938
		// (get) Token: 0x06007655 RID: 30293 RVA: 0x002665A6 File Offset: 0x002647A6
		public virtual float Scale
		{
			get
			{
				return this.TargetTransform.localScale.x;
			}
		}

		// Token: 0x17000B7B RID: 2939
		// (get) Token: 0x06007656 RID: 30294 RVA: 0x002665B8 File Offset: 0x002647B8
		protected bool InstantRotation
		{
			get
			{
				return this.m_instantRotation;
			}
		}

		// Token: 0x17000B7C RID: 2940
		// (get) Token: 0x06007657 RID: 30295 RVA: 0x002665C0 File Offset: 0x002647C0
		protected bool OverrideForceMode
		{
			get
			{
				return this.m_overrideForceMode;
			}
		}

		// Token: 0x17000B7D RID: 2941
		// (get) Token: 0x06007658 RID: 30296 RVA: 0x002665C8 File Offset: 0x002647C8
		// (set) Token: 0x06007659 RID: 30297 RVA: 0x002665D0 File Offset: 0x002647D0
		public RotationDirection PreferredRotationDirection
		{
			get
			{
				return this.m_preferredRotationDirection;
			}
			set
			{
				this.m_preferredRotationDirection = value;
			}
		}

		// Token: 0x17000B7E RID: 2942
		// (get) Token: 0x0600765A RID: 30298 RVA: 0x002665D9 File Offset: 0x002647D9
		public bool Register
		{
			get
			{
				return this.m_register;
			}
		}

		// Token: 0x17000B7F RID: 2943
		// (get) Token: 0x0600765B RID: 30299 RVA: 0x002665E1 File Offset: 0x002647E1
		// (set) Token: 0x0600765C RID: 30300 RVA: 0x002665E9 File Offset: 0x002647E9
		public Vector3 GravityUp { get; private set; } = Vector3.up;

		// Token: 0x17000B80 RID: 2944
		// (get) Token: 0x0600765D RID: 30301 RVA: 0x002665F2 File Offset: 0x002647F2
		// (set) Token: 0x0600765E RID: 30302 RVA: 0x002665FA File Offset: 0x002647FA
		public Vector3 GravityDown { get; private set; } = Vector3.down;

		// Token: 0x17000B81 RID: 2945
		// (get) Token: 0x0600765F RID: 30303 RVA: 0x00266603 File Offset: 0x00264803
		// (set) Token: 0x06007660 RID: 30304 RVA: 0x0026660B File Offset: 0x0026480B
		public float GravityMultiplier { get; set; } = 1f;

		// Token: 0x17000B82 RID: 2946
		// (get) Token: 0x06007661 RID: 30305 RVA: 0x00266614 File Offset: 0x00264814
		// (set) Token: 0x06007662 RID: 30306 RVA: 0x0026661C File Offset: 0x0026481C
		public Vector3 PersonalGravityDirection { get; set; } = Vector3.up;

		// Token: 0x06007663 RID: 30307 RVA: 0x00266625 File Offset: 0x00264825
		public void SetPersonalGravityDirection(Vector3 direction)
		{
			this.PersonalGravityDirection = direction.normalized;
		}

		// Token: 0x06007664 RID: 30308 RVA: 0x00266634 File Offset: 0x00264834
		public void SetPersonalGravityDirection(Transform reference)
		{
			this.PersonalGravityDirection = reference.up;
		}

		// Token: 0x17000B83 RID: 2947
		// (get) Token: 0x06007665 RID: 30309 RVA: 0x00266642 File Offset: 0x00264842
		public int GravityZonesCount
		{
			get
			{
				return this.m_gravityZones.Count;
			}
		}

		// Token: 0x17000B84 RID: 2948
		// (get) Token: 0x06007666 RID: 30310 RVA: 0x0026664F File Offset: 0x0026484F
		// (set) Token: 0x06007667 RID: 30311 RVA: 0x00266657 File Offset: 0x00264857
		public bool GlobalGravityIntent
		{
			get
			{
				return this.m_globalGravityIntent;
			}
			set
			{
				this.m_globalGravityIntent = value;
			}
		}

		// Token: 0x17000B85 RID: 2949
		// (get) Token: 0x06007668 RID: 30312 RVA: 0x00266660 File Offset: 0x00264860
		// (set) Token: 0x06007669 RID: 30313 RVA: 0x00266668 File Offset: 0x00264868
		bool ICallbackUnique.Registered { get; set; }

		// Token: 0x0600766A RID: 30314 RVA: 0x00266674 File Offset: 0x00264874
		protected virtual void Awake()
		{
			if (this.m_targetRigidBody.IsNull())
			{
				this.m_targetRigidBody = base.GetComponent<Rigidbody>();
			}
			if (this.m_targetTransform.IsNull())
			{
				this.m_targetTransform = base.transform;
			}
			if (this.m_alwaysInZone.IsNull() && this.m_activatorCollider.IsNull())
			{
				this.m_activatorCollider = base.GetComponentInChildren<Collider>();
				if (this.m_activatorCollider.IsNull())
				{
					return;
				}
			}
			if (this.m_targetRigidBody.IsNull())
			{
				return;
			}
			this.m_register = true;
			this.m_globalGravityIntent = this.m_targetRigidBody.useGravity;
		}

		// Token: 0x0600766B RID: 30315 RVA: 0x0026670D File Offset: 0x0026490D
		protected virtual void OnEnable()
		{
			if (!this.m_register)
			{
				return;
			}
			MonkeGravityManager.AddMonkeGravityController(this);
			if (this.m_alwaysInZone != null)
			{
				this.m_alwaysInZone.AddTarget(this);
			}
		}

		// Token: 0x0600766C RID: 30316 RVA: 0x00266738 File Offset: 0x00264938
		protected virtual void OnDisable()
		{
			if (!this.m_register)
			{
				return;
			}
			this.m_targetRigidBody.useGravity = this.m_globalGravityIntent;
			MonkeGravityManager.RemoveMonkeGravityController(this);
			this.ClearAllGravityZones();
		}

		// Token: 0x0600766D RID: 30317 RVA: 0x00266760 File Offset: 0x00264960
		private ValueTuple<Vector3, Vector3, float, bool> ProcessGravityZones(in Vector3 position)
		{
			MonkeGravityController.<>c__DisplayClass61_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.cumulativeVelocity = Vector3.zero;
			CS$<>8__locals1.cumulativeGravityDirection = Vector3.zero;
			CS$<>8__locals1.totalRotationSpeed = 0f;
			CS$<>8__locals1.rotationCount = 0;
			BasicGravityZone basicGravityZone = null;
			float num = float.MaxValue;
			int num2 = int.MinValue;
			int gravityZonesCount = this.GravityZonesCount;
			BasicGravityZone basicGravityZone2 = null;
			for (int i = 0; i < gravityZonesCount; i++)
			{
				BasicGravityZone basicGravityZone3 = this.m_gravityZones[i];
				int authorityLevel = basicGravityZone3.AuthorityLevel;
				if (authorityLevel > num2)
				{
					num2 = authorityLevel;
				}
				if (authorityLevel == this.m_highestAuthorityLevel)
				{
					basicGravityZone2 = basicGravityZone3;
					float sqrMagnitude = (basicGravityZone2.transform.position - position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						basicGravityZone = basicGravityZone2;
					}
					GravityZoneRule gravityRule = basicGravityZone2.GravityRule;
					if (gravityRule > GravityZoneRule.Closest && gravityRule == GravityZoneRule.Additive)
					{
						this.<ProcessGravityZones>g___ProcessGravityInfo|61_0(in basicGravityZone2, ref CS$<>8__locals1);
					}
				}
			}
			if (num2 != this.m_highestAuthorityLevel)
			{
				this.m_highestAuthorityLevel = num2;
				return this.ProcessGravityZones(in position);
			}
			if (basicGravityZone2.IsNotNull() && basicGravityZone2.GravityRule == GravityZoneRule.Newest)
			{
				this.<ProcessGravityZones>g___ProcessGravityInfo|61_0(in basicGravityZone2, ref CS$<>8__locals1);
			}
			if (basicGravityZone.IsNotNull() && basicGravityZone.GravityRule == GravityZoneRule.Closest)
			{
				this.<ProcessGravityZones>g___ProcessGravityInfo|61_0(in basicGravityZone, ref CS$<>8__locals1);
			}
			return new ValueTuple<Vector3, Vector3, float, bool>(CS$<>8__locals1.cumulativeVelocity, CS$<>8__locals1.cumulativeGravityDirection, (CS$<>8__locals1.rotationCount > 0) ? (CS$<>8__locals1.totalRotationSpeed / (float)CS$<>8__locals1.rotationCount) : 0f, CS$<>8__locals1.cumulativeGravityDirection != Vector3.zero);
		}

		// Token: 0x0600766E RID: 30318 RVA: 0x002668D4 File Offset: 0x00264AD4
		public virtual void CallBack()
		{
			ValueTuple<Vector3, Vector3, float, bool> valueTuple;
			if (this.m_gravityZones.Count > 0)
			{
				Vector3 worldPoint = this.GetWorldPoint();
				valueTuple = this.ProcessGravityZones(in worldPoint);
				this.GravityDown = valueTuple.Item1.normalized;
				this.GravityUp = -this.GravityDown;
			}
			else
			{
				GravityInfo defaultGravityInfo = MonkeGravityManager.DefaultGravityInfo;
				valueTuple.Item1 = defaultGravityInfo.gravityUpDirection * defaultGravityInfo.gravityStrength;
				valueTuple.Item2 = defaultGravityInfo.rotationDirection;
				valueTuple.Item3 = defaultGravityInfo.rotationSpeed;
				valueTuple.Item4 = defaultGravityInfo.rotate;
				this.GravityUp = defaultGravityInfo.gravityUpDirection;
				this.GravityDown = -this.GravityUp;
			}
			this.ApplyGravityForce(in valueTuple.Item1, ForceMode.Acceleration);
			if ((valueTuple.Item4 || this.m_needsRotationRecovery) && this.m_useRotation && valueTuple.Item2 != Vector3.zero)
			{
				this.ApplyGravityUpRotation(in valueTuple.Item2, valueTuple.Item3 * Time.fixedDeltaTime);
			}
		}

		// Token: 0x0600766F RID: 30319 RVA: 0x002669D7 File Offset: 0x00264BD7
		public virtual Vector3 GetWorldPoint()
		{
			return this.m_targetTransform.position;
		}

		// Token: 0x06007670 RID: 30320 RVA: 0x002669E4 File Offset: 0x00264BE4
		public virtual void OnEnteredGravityZone(BasicGravityZone zone)
		{
			if (!this.m_gravityZones.Contains(zone))
			{
				this.m_gravityZones.Add(zone);
			}
			if (this.m_targetRigidBody.useGravity)
			{
				this.m_targetRigidBody.useGravity = false;
			}
			int authorityLevel = zone.AuthorityLevel;
			if (authorityLevel > this.m_highestAuthorityLevel)
			{
				this.m_highestAuthorityLevel = authorityLevel;
			}
		}

		// Token: 0x06007671 RID: 30321 RVA: 0x00266A3C File Offset: 0x00264C3C
		public virtual void OnLeftGravityZone(BasicGravityZone zone)
		{
			this.m_gravityZones.Remove(zone);
			if (this.m_gravityZones.Count < 1)
			{
				this.m_targetRigidBody.useGravity = this.m_globalGravityIntent;
				this.m_needsRotationRecovery = true;
				this.m_highestAuthorityLevel = int.MinValue;
				return;
			}
			int num = int.MinValue;
			foreach (BasicGravityZone basicGravityZone in this.m_gravityZones)
			{
				if (basicGravityZone.AuthorityLevel > num)
				{
					num = basicGravityZone.AuthorityLevel;
				}
			}
			this.m_highestAuthorityLevel = num;
		}

		// Token: 0x06007672 RID: 30322 RVA: 0x00266AE4 File Offset: 0x00264CE4
		public void ClearAllGravityZones()
		{
			if (this.m_gravityZones.Count < 1)
			{
				return;
			}
			for (int i = this.m_gravityZones.Count - 1; i > -1; i--)
			{
				BasicGravityZone basicGravityZone = this.m_gravityZones[i];
				basicGravityZone.RemoveTarget(this);
				this.OnLeftGravityZone(basicGravityZone);
			}
		}

		// Token: 0x06007673 RID: 30323 RVA: 0x00266B33 File Offset: 0x00264D33
		public virtual void ApplyGravityForce(in Vector3 force, ForceMode forceType = ForceMode.Acceleration)
		{
			if (this.m_targetRigidBody.isKinematic)
			{
				return;
			}
			this.m_targetRigidBody.AddForce(force * this.GravityMultiplier, this.m_overrideForceMode ? this.m_forceModeOverride : forceType);
		}

		// Token: 0x06007674 RID: 30324 RVA: 0x00266B70 File Offset: 0x00264D70
		public void ClearRotationRecovery()
		{
			this.m_needsRotationRecovery = false;
		}

		// Token: 0x06007675 RID: 30325 RVA: 0x00266B7C File Offset: 0x00264D7C
		public virtual void ApplyGravityUpRotation(in Vector3 upDir, float speed)
		{
			Vector3 up = this.m_targetTransform.up;
			Vector3 vector2;
			if (!this.m_instantRotation)
			{
				Vector3 vector = upDir;
				if (vector == up * -1f)
				{
					switch (this.m_preferredRotationDirection)
					{
					case RotationDirection.Forward:
						vector = this.m_targetTransform.forward;
						break;
					case RotationDirection.Backward:
						vector = this.m_targetTransform.forward * -1f;
						break;
					case RotationDirection.Left:
						vector = this.m_targetTransform.right * -1f;
						break;
					case RotationDirection.Right:
						vector = this.m_targetTransform.right;
						break;
					}
				}
				vector2 = Vector3.RotateTowards(up, vector, speed, 0f);
			}
			else
			{
				vector2 = upDir;
			}
			Quaternion quaternion = Quaternion.FromToRotation(up, vector2);
			this.m_targetRigidBody.MoveRotation(quaternion * this.m_targetTransform.rotation);
			if (quaternion == Quaternion.identity)
			{
				this.m_needsRotationRecovery = false;
			}
		}

		// Token: 0x06007676 RID: 30326 RVA: 0x00266C78 File Offset: 0x00264E78
		public void CopyProperties(MonkeGravityControllerSettings settings, BasicGravityZone alwaysInZone)
		{
			this.m_useRotation = settings.useRotation;
			this.m_instantRotation = settings.instantRotation;
			this.m_overrideForceMode = settings.overrideForceMode;
			this.m_forceModeOverride = settings.forceModeOverride;
			RotationDirection rotationDirection;
			switch (settings.preferredRotationDirection)
			{
			case MonkeGravityControllerSettings.RotationDirection.None:
				rotationDirection = RotationDirection.None;
				break;
			case MonkeGravityControllerSettings.RotationDirection.Forward:
				rotationDirection = RotationDirection.Forward;
				break;
			case MonkeGravityControllerSettings.RotationDirection.Backward:
				rotationDirection = RotationDirection.Backward;
				break;
			case MonkeGravityControllerSettings.RotationDirection.Left:
				rotationDirection = RotationDirection.Left;
				break;
			case MonkeGravityControllerSettings.RotationDirection.Right:
				rotationDirection = RotationDirection.Right;
				break;
			default:
				throw new NotImplementedException();
			}
			this.m_preferredRotationDirection = rotationDirection;
			if (this.m_register)
			{
				MonkeGravityManager.RemoveMonkeGravityController(this);
				this.m_register = false;
			}
			if (settings.activatorCollider != null)
			{
				this.m_activatorCollider = settings.activatorCollider;
			}
			if (settings.targetRigidbody != null)
			{
				this.m_targetRigidBody = settings.targetRigidbody;
			}
			if (settings.targetTransform != null)
			{
				this.m_targetTransform = settings.targetTransform;
			}
			this.m_alwaysInZone = alwaysInZone;
			if (this.m_targetRigidBody.IsNull())
			{
				this.m_targetRigidBody = base.GetComponent<Rigidbody>();
			}
			if (this.m_targetTransform.IsNull())
			{
				this.m_targetTransform = base.transform;
			}
			if (this.m_activatorCollider.IsNull())
			{
				this.m_activatorCollider = base.GetComponent<Collider>();
			}
			if (this.m_activatorCollider.IsNull() && this.m_alwaysInZone == null)
			{
				return;
			}
			if (this.m_targetRigidBody.IsNull())
			{
				return;
			}
			this.m_register = true;
			this.m_globalGravityIntent = this.m_targetRigidBody.useGravity;
			if (base.isActiveAndEnabled)
			{
				MonkeGravityManager.AddMonkeGravityController(this);
				if (this.m_alwaysInZone != null)
				{
					this.m_alwaysInZone.AddTarget(this);
				}
			}
		}

		// Token: 0x06007677 RID: 30327 RVA: 0x00266E18 File Offset: 0x00265018
		public void AssignReferencesIfMissing(Rigidbody rb, Collider col, Transform tr)
		{
			if (this.m_register)
			{
				return;
			}
			this.m_targetRigidBody = rb;
			this.m_activatorCollider = col;
			this.m_targetTransform = tr;
			this.m_globalGravityIntent = rb.useGravity;
			this.m_register = true;
		}

		// Token: 0x06007679 RID: 30329 RVA: 0x00266EAC File Offset: 0x002650AC
		[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void <ProcessGravityZones>g___ProcessGravityInfo|61_0(in BasicGravityZone gZone, ref MonkeGravityController.<>c__DisplayClass61_0 A_2)
		{
			GravityInfo gravityInfo;
			if (!gZone.GetGravityInfo(this, out gravityInfo))
			{
				return;
			}
			A_2.cumulativeVelocity += gravityInfo.gravityUpDirection * gravityInfo.gravityStrength;
			if (gravityInfo.rotate && gravityInfo.rotationSpeed > 0f)
			{
				int num = A_2.rotationCount + 1;
				A_2.rotationCount = num;
				A_2.totalRotationSpeed += gravityInfo.rotationSpeed;
				A_2.cumulativeGravityDirection += gravityInfo.rotationDirection * Mathf.Max(0.01f, Mathf.Abs(gravityInfo.gravityStrength));
			}
		}

		// Token: 0x040085E8 RID: 34280
		[SerializeField]
		private Collider m_activatorCollider;

		// Token: 0x040085E9 RID: 34281
		[SerializeField]
		protected Rigidbody m_targetRigidBody;

		// Token: 0x040085EA RID: 34282
		[SerializeField]
		protected Transform m_targetTransform;

		// Token: 0x040085EB RID: 34283
		[SerializeField]
		private bool m_instantRotation;

		// Token: 0x040085EC RID: 34284
		[SerializeField]
		private bool m_useRotation;

		// Token: 0x040085ED RID: 34285
		[SerializeField]
		private bool m_overrideForceMode;

		// Token: 0x040085EE RID: 34286
		[SerializeField]
		private ForceMode m_forceModeOverride;

		// Token: 0x040085EF RID: 34287
		[SerializeField]
		private BasicGravityZone m_alwaysInZone;

		// Token: 0x040085F0 RID: 34288
		[Tooltip("The direction we wish to rotate if the target is 180 degrees off.")]
		[SerializeField]
		protected RotationDirection m_preferredRotationDirection = RotationDirection.Right;

		// Token: 0x040085F1 RID: 34289
		private bool m_register;

		// Token: 0x040085F2 RID: 34290
		private readonly List<BasicGravityZone> m_gravityZones = new List<BasicGravityZone>(3);

		// Token: 0x040085F3 RID: 34291
		private bool m_needsRotationRecovery;

		// Token: 0x040085F8 RID: 34296
		protected bool m_globalGravityIntent;

		// Token: 0x040085F9 RID: 34297
		private int m_highestAuthorityLevel = int.MinValue;
	}
}
