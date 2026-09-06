using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x02001241 RID: 4673
	public class MonkeGravityManager : MonoBehaviour
	{
		// Token: 0x0600767A RID: 30330 RVA: 0x00266F54 File Offset: 0x00265154
		private void Awake()
		{
			Vector3 vector = -Physics.gravity.normalized;
			MonkeGravityManager.k_defaultGravityInfo.gravityUpDirection = vector;
			MonkeGravityManager.k_defaultGravityInfo.rotationDirection = vector;
			MonkeGravityManager.k_defaultGravityInfo.rotationSpeed = this.defaultRotationSpeed;
			MonkeGravityManager.k_defaultGravityInfo.gravityStrength = this.defaultGravityStrength;
		}

		// Token: 0x0600767B RID: 30331 RVA: 0x00266FAA File Offset: 0x002651AA
		private void FixedUpdate()
		{
			MonkeGravityManager.k_zones.RunCallbacks();
			MonkeGravityManager.k_controllers.RunCallbacks();
		}

		// Token: 0x17000B86 RID: 2950
		// (get) Token: 0x0600767C RID: 30332 RVA: 0x00266FC0 File Offset: 0x002651C0
		public static GravityInfo DefaultGravityInfo
		{
			get
			{
				return MonkeGravityManager.k_defaultGravityInfo;
			}
		}

		// Token: 0x0600767D RID: 30333 RVA: 0x00266FC8 File Offset: 0x002651C8
		public static void AddMonkeGravityController(MonkeGravityController gravity)
		{
			Collider activatorCollider = gravity.ActivatorCollider;
			if (activatorCollider != null)
			{
				MonkeGravityManager.k_allowedColliders.TryAdd(activatorCollider, gravity);
			}
			MonkeGravityManager.k_controllers.Add(in gravity);
		}

		// Token: 0x0600767E RID: 30334 RVA: 0x00266FFE File Offset: 0x002651FE
		public static void RemoveMonkeGravityController(MonkeGravityController gravity)
		{
			if (gravity.ActivatorCollider != null)
			{
				MonkeGravityManager.k_allowedColliders.Remove(gravity.ActivatorCollider);
			}
			MonkeGravityManager.k_controllers.Remove(in gravity);
		}

		// Token: 0x0600767F RID: 30335 RVA: 0x0026702C File Offset: 0x0026522C
		[return: TupleElementNames(new string[] { "found", "target" })]
		public static ValueTuple<bool, MonkeGravityController> GetMonkeGravityController(Collider collider)
		{
			MonkeGravityController monkeGravityController;
			return new ValueTuple<bool, MonkeGravityController>(MonkeGravityManager.k_allowedColliders.TryGetValue(collider, out monkeGravityController), monkeGravityController);
		}

		// Token: 0x06007680 RID: 30336 RVA: 0x0026704C File Offset: 0x0026524C
		public static void AddGravityCallback(BasicGravityZone zone)
		{
			MonkeGravityManager.k_zones.Add(in zone);
		}

		// Token: 0x06007681 RID: 30337 RVA: 0x0026705A File Offset: 0x0026525A
		public static void RemoveGravityCallback(BasicGravityZone zone)
		{
			MonkeGravityManager.k_zones.Remove(in zone);
		}

		// Token: 0x04008605 RID: 34309
		[SerializeField]
		private float defaultRotationSpeed = 10f;

		// Token: 0x04008606 RID: 34310
		[SerializeField]
		private float defaultGravityStrength = -9.3f;

		// Token: 0x04008607 RID: 34311
		private static readonly CallbackContainerUnique<BasicGravityZone> k_zones = new CallbackContainerUnique<BasicGravityZone>(5);

		// Token: 0x04008608 RID: 34312
		private static readonly Dictionary<Collider, MonkeGravityController> k_allowedColliders = new Dictionary<Collider, MonkeGravityController>(10);

		// Token: 0x04008609 RID: 34313
		private static readonly CallbackContainerUnique<MonkeGravityController> k_controllers = new CallbackContainerUnique<MonkeGravityController>(10);

		// Token: 0x0400860A RID: 34314
		private static GravityInfo k_defaultGravityInfo = new GravityInfo
		{
			gravityUpDirection = Vector3.up,
			rotationDirection = Vector3.up,
			rotationSpeed = 10f,
			gravityStrength = -9.3f,
			rotate = false
		};
	}
}
