using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012E0 RID: 4832
	public class AOESender : MonoBehaviour
	{
		// Token: 0x0600790C RID: 30988 RVA: 0x0027742B File Offset: 0x0027562B
		private void Awake()
		{
			if (this.hits == null || this.hits.Length != this.maxColliders)
			{
				this.hits = new Collider[Mathf.Max(8, this.maxColliders)];
			}
		}

		// Token: 0x0600790D RID: 30989 RVA: 0x0027745C File Offset: 0x0027565C
		private void OnEnable()
		{
			if (this.applyOnEnable)
			{
				this.ApplyAOE();
			}
			this.nextTime = Time.time + this.repeatInterval;
		}

		// Token: 0x0600790E RID: 30990 RVA: 0x0027747E File Offset: 0x0027567E
		private void Update()
		{
			if (this.repeatInterval > 0f && Time.time >= this.nextTime)
			{
				this.ApplyAOE();
				this.nextTime = Time.time + this.repeatInterval;
			}
		}

		// Token: 0x0600790F RID: 30991 RVA: 0x002774B2 File Offset: 0x002756B2
		public void ApplyAOE()
		{
			this.ApplyAOE(base.transform.position);
		}

		// Token: 0x06007910 RID: 30992 RVA: 0x002774C8 File Offset: 0x002756C8
		public void ApplyAOE(Vector3 worldOrigin)
		{
			this.visited.Clear();
			int num = Physics.OverlapSphereNonAlloc(worldOrigin, this.radius, this.hits, this.layerMask, this.triggerInteraction);
			float num2 = Mathf.Max(0.0001f, this.radius);
			for (int i = 0; i < num; i++)
			{
				Collider collider = this.hits[i];
				if (collider)
				{
					AOEReceiver componentInChildren = (collider.attachedRigidbody ? collider.attachedRigidbody.transform : collider.transform).GetComponentInChildren<AOEReceiver>(true);
					if (componentInChildren != null && this.TagValidation(componentInChildren.gameObject) && !this.visited.Contains(componentInChildren))
					{
						this.visited.Add(componentInChildren);
						float num3 = Vector3.Distance(worldOrigin, componentInChildren.transform.position);
						float num4 = Mathf.Clamp01(num3 / num2);
						float num5 = this.EvaluateFalloff(num4);
						float num6 = Mathf.Max(this.minStrength, this.strength * num5);
						AOEReceiver.AOEContext aoecontext = new AOEReceiver.AOEContext
						{
							origin = worldOrigin,
							radius = this.radius,
							instigator = base.gameObject,
							baseStrength = this.strength,
							finalStrength = num6,
							distance = num3,
							normalizedDistance = num4
						};
						componentInChildren.ReceiveAOE(in aoecontext);
					}
				}
			}
		}

		// Token: 0x06007911 RID: 30993 RVA: 0x00277640 File Offset: 0x00275840
		private float EvaluateFalloff(float t)
		{
			switch (this.falloffMode)
			{
			case AOESender.FalloffMode.None:
				return 1f;
			case AOESender.FalloffMode.Linear:
				return 1f - t;
			case AOESender.FalloffMode.AnimationCurve:
				return Mathf.Max(0f, this.falloffCurve.Evaluate(t));
			default:
				return 1f;
			}
		}

		// Token: 0x06007912 RID: 30994 RVA: 0x00277694 File Offset: 0x00275894
		private bool TagValidation(GameObject go)
		{
			if (go == null)
			{
				return false;
			}
			if (this.includeTags == null || this.includeTags.Length == 0)
			{
				return true;
			}
			string tag = go.tag;
			foreach (string text in this.includeTags)
			{
				if (!string.IsNullOrEmpty(text) && tag == text)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04008A0F RID: 35343
		[Min(0f)]
		[SerializeField]
		private float radius = 3f;

		// Token: 0x04008A10 RID: 35344
		[SerializeField]
		private LayerMask layerMask = -1;

		// Token: 0x04008A11 RID: 35345
		[SerializeField]
		private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;

		// Token: 0x04008A12 RID: 35346
		[Tooltip("If empty, all AOEReceiver targets pass. If not empty, only receivers with these tags pass.")]
		[SerializeField]
		private string[] includeTags;

		// Token: 0x04008A13 RID: 35347
		[SerializeField]
		private AOESender.FalloffMode falloffMode = AOESender.FalloffMode.Linear;

		// Token: 0x04008A14 RID: 35348
		[SerializeField]
		private AnimationCurve falloffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		// Token: 0x04008A15 RID: 35349
		[Tooltip("Base strength before distance falloff.")]
		[SerializeField]
		private float strength = 1f;

		// Token: 0x04008A16 RID: 35350
		[Tooltip("Optional after falloff, applied as: max(minStrength, base*falloff).")]
		[SerializeField]
		private float minStrength;

		// Token: 0x04008A17 RID: 35351
		[SerializeField]
		private bool applyOnEnable;

		// Token: 0x04008A18 RID: 35352
		[Min(0f)]
		[SerializeField]
		private float repeatInterval;

		// Token: 0x04008A19 RID: 35353
		[SerializeField]
		[Tooltip("Max colliders captured per trigger/apply.")]
		private int maxColliders = 16;

		// Token: 0x04008A1A RID: 35354
		private Collider[] hits;

		// Token: 0x04008A1B RID: 35355
		private readonly HashSet<AOEReceiver> visited = new HashSet<AOEReceiver>();

		// Token: 0x04008A1C RID: 35356
		private float nextTime;

		// Token: 0x020012E1 RID: 4833
		private enum FalloffMode
		{
			// Token: 0x04008A1E RID: 35358
			None,
			// Token: 0x04008A1F RID: 35359
			Linear,
			// Token: 0x04008A20 RID: 35360
			AnimationCurve
		}
	}
}
