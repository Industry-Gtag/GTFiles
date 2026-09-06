using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using GT_CustomMapSupportRuntime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Gravity
{
	// Token: 0x02001236 RID: 4662
	public class BasicGravityZone : MonoBehaviour, ICallbackUnique, ICallBack
	{
		// Token: 0x17000B70 RID: 2928
		// (get) Token: 0x0600760C RID: 30220 RVA: 0x00265645 File Offset: 0x00263845
		public GravityZoneRule GravityRule
		{
			get
			{
				return this.m_gravityRule;
			}
		}

		// Token: 0x17000B71 RID: 2929
		// (get) Token: 0x0600760D RID: 30221 RVA: 0x0026564D File Offset: 0x0026384D
		public int AuthorityLevel
		{
			get
			{
				return this.m_authorityLevel;
			}
		}

		// Token: 0x17000B72 RID: 2930
		// (get) Token: 0x0600760E RID: 30222 RVA: 0x00265655 File Offset: 0x00263855
		protected float RotationSpeed
		{
			get
			{
				return this.m_rotationSpeed;
			}
		}

		// Token: 0x17000B73 RID: 2931
		// (get) Token: 0x0600760F RID: 30223 RVA: 0x0026565D File Offset: 0x0026385D
		private IReadOnlyList<MonkeGravityController> GravityTargets
		{
			get
			{
				return this.m_gravityTargets.GetReadonlyList();
			}
		}

		// Token: 0x17000B74 RID: 2932
		// (get) Token: 0x06007610 RID: 30224 RVA: 0x0026566A File Offset: 0x0026386A
		// (set) Token: 0x06007611 RID: 30225 RVA: 0x00265672 File Offset: 0x00263872
		bool ICallbackUnique.Registered { get; set; }

		// Token: 0x06007612 RID: 30226 RVA: 0x0026567B File Offset: 0x0026387B
		protected virtual void Awake()
		{
			this.CalculateDependentVars();
		}

		// Token: 0x06007613 RID: 30227 RVA: 0x00265684 File Offset: 0x00263884
		private void CalculateDependentVars()
		{
			this.m_gravityDirection = base.gameObject.transform.up;
			this.invertRotationDirection = (this.gravityStrength > 0f && !this.invertRotationDirection) || (this.gravityStrength <= 0f && this.invertRotationDirection);
			this.m_rotationSpeed = (this.m_useRotationSpeedOverride ? this.m_rotationSpeedOverride : MonkeGravityManager.DefaultGravityInfo.rotationSpeed);
		}

		// Token: 0x06007614 RID: 30228 RVA: 0x002656FB File Offset: 0x002638FB
		protected virtual void OnEnable()
		{
			this.m_gravityTargets.ItemProcessor = new InAction<MonkeGravityController>(this.ProcessGravityTargets);
		}

		// Token: 0x06007615 RID: 30229 RVA: 0x00265714 File Offset: 0x00263914
		protected virtual void OnDisable()
		{
			foreach (KeyValuePair<MonkeGravityController, Coroutine> keyValuePair in this.m_pendingTransitions)
			{
				if (keyValuePair.Value != null)
				{
					base.StopCoroutine(keyValuePair.Value);
				}
			}
			this.m_pendingTransitions.Clear();
			this.m_gravityTargets.ItemProcessor = new InAction<MonkeGravityController>(this.ProcessRemoveTargets);
			this.m_gravityTargets.ProcessList();
			this.m_gravityTargets.Clear();
			this.m_targetGravityInfos.Clear();
			MonkeGravityManager.RemoveGravityCallback(this);
		}

		// Token: 0x06007616 RID: 30230 RVA: 0x002657C0 File Offset: 0x002639C0
		public virtual void CallBack()
		{
			this.m_gravityTargets.ProcessList();
		}

		// Token: 0x06007617 RID: 30231 RVA: 0x002657CD File Offset: 0x002639CD
		private void ProcessRemoveTargets(in MonkeGravityController target)
		{
			target.OnLeftGravityZone(this);
		}

		// Token: 0x06007618 RID: 30232 RVA: 0x002657D8 File Offset: 0x002639D8
		private void ProcessGravityTargets(in MonkeGravityController targetController)
		{
			if (!this.PassesScaleFilter(targetController))
			{
				this.OnTargetFilteredOut(targetController);
			}
			GravityInfo gravityInfo = default(GravityInfo);
			Vector3 worldPoint = targetController.GetWorldPoint();
			Vector3 gravityVectorAtPoint = this.GetGravityVectorAtPoint(in worldPoint, in targetController);
			Vector3 normalized = gravityVectorAtPoint.normalized;
			gravityInfo.gravityUpDirection = normalized;
			gravityInfo.rotationDirection = this.GetRotationDirection(in normalized);
			gravityInfo.gravityStrength = this.GetGravityStrength(in gravityVectorAtPoint);
			gravityInfo.rotationSpeed = this.GetRotationSpeed(in gravityVectorAtPoint);
			gravityInfo.rotate = this.GetRotationIntent(in gravityVectorAtPoint);
			this.m_targetGravityInfos[targetController] = gravityInfo;
		}

		// Token: 0x06007619 RID: 30233 RVA: 0x0026586B File Offset: 0x00263A6B
		protected virtual Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			return this.m_gravityDirection;
		}

		// Token: 0x0600761A RID: 30234 RVA: 0x00265873 File Offset: 0x00263A73
		protected virtual float GetGravityStrength(in Vector3 offsetFromGravity)
		{
			return this.gravityStrength;
		}

		// Token: 0x0600761B RID: 30235 RVA: 0x0026587B File Offset: 0x00263A7B
		protected virtual bool GetRotationIntent(in Vector3 offsetFromGravity)
		{
			return this.rotateTarget;
		}

		// Token: 0x0600761C RID: 30236 RVA: 0x00265883 File Offset: 0x00263A83
		protected virtual Vector3 GetRotationDirection(in Vector3 gravityDirection)
		{
			if (this.invertRotationDirection)
			{
				return -gravityDirection;
			}
			return gravityDirection;
		}

		// Token: 0x0600761D RID: 30237 RVA: 0x00265655 File Offset: 0x00263855
		protected virtual float GetRotationSpeed(in Vector3 offsetFromGravity)
		{
			return this.m_rotationSpeed;
		}

		// Token: 0x0600761E RID: 30238 RVA: 0x0026589F File Offset: 0x00263A9F
		public bool GetGravityInfo(MonkeGravityController target, out GravityInfo info)
		{
			return this.m_targetGravityInfos.TryGetValue(target, out info);
		}

		// Token: 0x0600761F RID: 30239 RVA: 0x002658AE File Offset: 0x00263AAE
		public void AddTargetLocalPlayer()
		{
			this.AddTarget(GTPlayerTransform.Instance);
		}

		// Token: 0x06007620 RID: 30240 RVA: 0x002658BB File Offset: 0x00263ABB
		public void RemoveTargetLocalPlayer()
		{
			this.RemoveTarget(GTPlayerTransform.Instance);
		}

		// Token: 0x06007621 RID: 30241 RVA: 0x002658C8 File Offset: 0x00263AC8
		public void RemoveTarget(MonkeGravityController target)
		{
			this.CancelPending(target);
			this.RemoveTargetImmediate(target);
		}

		// Token: 0x06007622 RID: 30242 RVA: 0x002658D8 File Offset: 0x00263AD8
		public void AddTarget(MonkeGravityController target)
		{
			this.CancelPending(target);
			this.AddTargetImmediate(target);
		}

		// Token: 0x06007623 RID: 30243 RVA: 0x002658E8 File Offset: 0x00263AE8
		public void RemoveTarget(MonkeGravityController target, float delay)
		{
			if (delay <= 0f || !base.isActiveAndEnabled)
			{
				this.RemoveTarget(target);
				return;
			}
			this.CancelPending(target);
			if (this.m_gravityTargets.Contains(in target))
			{
				this.m_pendingTransitions[target] = base.StartCoroutine(this.DelayedTransition(target, delay, false));
			}
		}

		// Token: 0x06007624 RID: 30244 RVA: 0x00265940 File Offset: 0x00263B40
		public void AddTarget(MonkeGravityController target, float delay)
		{
			if (delay <= 0f || !base.isActiveAndEnabled)
			{
				this.AddTarget(target);
				return;
			}
			this.CancelPending(target);
			if (!this.m_gravityTargets.Contains(in target))
			{
				this.m_pendingTransitions[target] = base.StartCoroutine(this.DelayedTransition(target, delay, true));
			}
		}

		// Token: 0x06007625 RID: 30245 RVA: 0x00265998 File Offset: 0x00263B98
		private void CancelPending(MonkeGravityController target)
		{
			Coroutine coroutine;
			if (this.m_pendingTransitions.TryGetValue(target, out coroutine))
			{
				if (coroutine != null)
				{
					base.StopCoroutine(coroutine);
				}
				this.m_pendingTransitions.Remove(target);
			}
		}

		// Token: 0x06007626 RID: 30246 RVA: 0x002659CC File Offset: 0x00263BCC
		private IEnumerator DelayedTransition(MonkeGravityController target, float delay, bool add)
		{
			yield return new WaitForSeconds(delay);
			this.m_pendingTransitions.Remove(target);
			if (add)
			{
				this.AddTargetImmediate(target);
			}
			else
			{
				this.RemoveTargetImmediate(target);
			}
			yield break;
		}

		// Token: 0x06007627 RID: 30247 RVA: 0x002659F0 File Offset: 0x00263BF0
		private void RemoveTargetImmediate(MonkeGravityController target)
		{
			if (!target.Register || !this.m_gravityTargets.Remove(in target))
			{
				return;
			}
			this.m_targetGravityInfos.Remove(target);
			target.OnLeftGravityZone(this);
			this.OnTargetExited(target);
			if (target == GTPlayerTransform.Instance)
			{
				UnityEvent unityEvent = this.onLocalPlayerExited;
				if (unityEvent != null)
				{
					unityEvent.Invoke();
				}
			}
			if (this.m_gravityTargets.Count < 1)
			{
				MonkeGravityManager.RemoveGravityCallback(this);
			}
		}

		// Token: 0x06007628 RID: 30248 RVA: 0x00265A64 File Offset: 0x00263C64
		private void AddTargetImmediate(MonkeGravityController target)
		{
			if (!target.Register || this.m_gravityTargets.Contains(in target))
			{
				return;
			}
			this.m_gravityTargets.Add(in target);
			target.OnEnteredGravityZone(this);
			if (target == GTPlayerTransform.Instance)
			{
				UnityEvent unityEvent = this.onLocalPlayerEntered;
				if (unityEvent != null)
				{
					unityEvent.Invoke();
				}
			}
			MonkeGravityManager.AddGravityCallback(this);
		}

		// Token: 0x06007629 RID: 30249 RVA: 0x00002C2D File Offset: 0x00000E2D
		protected virtual void OnTargetExited(MonkeGravityController target)
		{
		}

		// Token: 0x0600762A RID: 30250 RVA: 0x00002C2D File Offset: 0x00000E2D
		protected virtual void OnTargetFilteredOut(MonkeGravityController target)
		{
		}

		// Token: 0x0600762B RID: 30251 RVA: 0x00265AC4 File Offset: 0x00263CC4
		private bool PassesScaleFilter(MonkeGravityController target)
		{
			if (this.scaleFilter == GravityZoneScaleFilter.Anyone)
			{
				return true;
			}
			bool flag = target.Scale < 1f;
			if (this.scaleFilter != GravityZoneScaleFilter.SmallOnly)
			{
				return !flag;
			}
			return flag;
		}

		// Token: 0x0600762C RID: 30252 RVA: 0x00265AF8 File Offset: 0x00263CF8
		private void OnTriggerEnter(Collider other)
		{
			ValueTuple<bool, MonkeGravityController> monkeGravityController = MonkeGravityManager.GetMonkeGravityController(other);
			if (!monkeGravityController.Item1)
			{
				return;
			}
			this.AddTarget(monkeGravityController.Item2);
		}

		// Token: 0x0600762D RID: 30253 RVA: 0x00265B24 File Offset: 0x00263D24
		private void OnTriggerExit(Collider other)
		{
			ValueTuple<bool, MonkeGravityController> monkeGravityController = MonkeGravityManager.GetMonkeGravityController(other);
			if (!monkeGravityController.Item1)
			{
				return;
			}
			this.RemoveTarget(monkeGravityController.Item2);
		}

		// Token: 0x0600762E RID: 30254 RVA: 0x00265B50 File Offset: 0x00263D50
		public void CopyProperties(BasicGravityZoneSettings settings)
		{
			this.gravityStrength = settings.gravityStrength;
			GravityZoneScaleFilter gravityZoneScaleFilter;
			switch (settings.scaleFilter)
			{
			case BasicGravityZoneSettings.GravityZoneScaleFilter.Anyone:
				gravityZoneScaleFilter = GravityZoneScaleFilter.Anyone;
				break;
			case BasicGravityZoneSettings.GravityZoneScaleFilter.SmallOnly:
				gravityZoneScaleFilter = GravityZoneScaleFilter.SmallOnly;
				break;
			case BasicGravityZoneSettings.GravityZoneScaleFilter.NotSmall:
				gravityZoneScaleFilter = GravityZoneScaleFilter.NotSmall;
				break;
			default:
				throw new NotImplementedException();
			}
			this.scaleFilter = gravityZoneScaleFilter;
			GravityZoneRule gravityZoneRule;
			switch (settings.gravityRule)
			{
			case BasicGravityZoneSettings.GravityZoneRule.Newest:
				gravityZoneRule = GravityZoneRule.Newest;
				break;
			case BasicGravityZoneSettings.GravityZoneRule.Closest:
				gravityZoneRule = GravityZoneRule.Closest;
				break;
			case BasicGravityZoneSettings.GravityZoneRule.Additive:
				gravityZoneRule = GravityZoneRule.Additive;
				break;
			default:
				throw new NotImplementedException();
			}
			this.m_gravityRule = gravityZoneRule;
			this.m_authorityLevel = settings.authorityLevel;
			this.invertRotationDirection = settings.invertRotationDirection;
			this.rotateTarget = settings.rotateTarget;
			this.m_useRotationSpeedOverride = settings.useRotationSpeedOverride;
			this.m_rotationSpeedOverride = settings.rotationSpeedOverride;
			this.CalculateDependentVars();
		}

		// Token: 0x040085AA RID: 34218
		[Header("Gravity Settings")]
		[Tooltip("negative number pulls, positive number expels")]
		public float gravityStrength = -9.81f;

		// Token: 0x040085AB RID: 34219
		[Tooltip("Filter which players are affected based on scale. Small = scale < 1")]
		[SerializeField]
		private GravityZoneScaleFilter scaleFilter;

		// Token: 0x040085AC RID: 34220
		[Tooltip("- Newest: Only in effect when this is the newest zone entered by the physics object. \n\n- Closest: if this gravity zone is the closest, then it will have effect. \n\n- Additive:  always in effect when a physics object is inside.")]
		[SerializeField]
		private GravityZoneRule m_gravityRule = GravityZoneRule.Closest;

		// Token: 0x040085AD RID: 34221
		[Tooltip("The gravity zone with the highest authority will cause gravity zones with a lower authority level to be ignored. Gravity zones with the same authority level will follow the Gravity Rule setting.")]
		[SerializeField]
		private int m_authorityLevel;

		// Token: 0x040085AE RID: 34222
		[Header("Rotation Settings")]
		[Tooltip("If enabled, rotates the target away from gravity direction to be upside down")]
		[SerializeField]
		protected bool invertRotationDirection;

		// Token: 0x040085AF RID: 34223
		[SerializeField]
		protected bool rotateTarget = true;

		// Token: 0x040085B0 RID: 34224
		[SerializeField]
		private bool m_useRotationSpeedOverride;

		// Token: 0x040085B1 RID: 34225
		[SerializeField]
		[FormerlySerializedAs("rotationSpeed")]
		private float m_rotationSpeedOverride = 10f;

		// Token: 0x040085B2 RID: 34226
		[NonSerialized]
		private float m_rotationSpeed;

		// Token: 0x040085B3 RID: 34227
		[Header("Events")]
		public UnityEvent onLocalPlayerEntered;

		// Token: 0x040085B4 RID: 34228
		public UnityEvent onLocalPlayerExited;

		// Token: 0x040085B5 RID: 34229
		protected Vector3 m_gravityDirection;

		// Token: 0x040085B6 RID: 34230
		protected ListProcessor<MonkeGravityController> m_gravityTargets = new ListProcessor<MonkeGravityController>(5, null);

		// Token: 0x040085B7 RID: 34231
		private Dictionary<MonkeGravityController, GravityInfo> m_targetGravityInfos = new Dictionary<MonkeGravityController, GravityInfo>(5);

		// Token: 0x040085B8 RID: 34232
		private readonly Dictionary<MonkeGravityController, Coroutine> m_pendingTransitions = new Dictionary<MonkeGravityController, Coroutine>(5);
	}
}
