using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI
{
	// Token: 0x02001078 RID: 4216
	public class AIEntity : MonoBehaviour
	{
		// Token: 0x06006948 RID: 26952 RVA: 0x0021E708 File Offset: 0x0021C908
		protected void Awake()
		{
			this.navMeshAgent = base.gameObject.GetComponent<NavMeshAgent>();
			this.animator = base.gameObject.GetComponent<Animator>();
			if (this.waypointsContainer != null)
			{
				foreach (Transform transform in this.waypointsContainer.GetComponentsInChildren<Transform>())
				{
					this.waypoints.Add(transform);
				}
			}
		}

		// Token: 0x06006949 RID: 26953 RVA: 0x0021E770 File Offset: 0x0021C970
		protected void ChooseRandomTarget()
		{
			int randomTarget = Random.Range(0, VRRigCache.ActiveRigs.Count);
			int num = VRRigCache.ActiveRigContainers.FindIndex((RigContainer x) => x.Rig.creator != null && x.Rig.creator == VRRigCache.ActiveRigContainers[randomTarget].Rig.creator);
			if (num == -1)
			{
				num = Random.Range(0, VRRigCache.ActiveRigs.Count);
			}
			if (num < VRRigCache.ActiveRigContainers.Count)
			{
				this.targetPlayer = VRRigCache.ActiveRigContainers[num].Rig.creator;
				this.followTarget = VRRigCache.ActiveRigContainers[num].Rig.head.rigTarget;
				NavMeshHit navMeshHit;
				this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, this.navMeshSampleRange, 1);
				return;
			}
			this.targetPlayer = null;
			this.followTarget = null;
		}

		// Token: 0x0600694A RID: 26954 RVA: 0x0021E840 File Offset: 0x0021CA40
		protected void ChooseClosestTarget()
		{
			VRRig vrrig = null;
			float num = float.MaxValue;
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				VRRig rig = rigContainer.Rig;
				if (rig.head != null && !rig.head.rigTarget.IsNull())
				{
					float sqrMagnitude = (base.transform.position - rig.head.rigTarget.transform.position).sqrMagnitude;
					if (sqrMagnitude < this.minChaseRange * this.minChaseRange && sqrMagnitude < num)
					{
						num = sqrMagnitude;
						vrrig = rig;
					}
				}
			}
			if (vrrig.IsNotNull())
			{
				this.targetPlayer = vrrig.creator;
				this.followTarget = vrrig.head.rigTarget;
				NavMeshHit navMeshHit;
				this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, this.navMeshSampleRange, 1);
				return;
			}
			this.targetPlayer = null;
			this.followTarget = null;
		}

		// Token: 0x040078DF RID: 30943
		public GameObject waypointsContainer;

		// Token: 0x040078E0 RID: 30944
		public Transform circleCenter;

		// Token: 0x040078E1 RID: 30945
		public float circleRadius;

		// Token: 0x040078E2 RID: 30946
		public float angularSpeed;

		// Token: 0x040078E3 RID: 30947
		public float patrolSpeed;

		// Token: 0x040078E4 RID: 30948
		public float fleeSpeed;

		// Token: 0x040078E5 RID: 30949
		public NavMeshAgent navMeshAgent;

		// Token: 0x040078E6 RID: 30950
		public Animator animator;

		// Token: 0x040078E7 RID: 30951
		public float fleeRang;

		// Token: 0x040078E8 RID: 30952
		public float fleeSpeedMult;

		// Token: 0x040078E9 RID: 30953
		public float minChaseRange;

		// Token: 0x040078EA RID: 30954
		public float attackDistance;

		// Token: 0x040078EB RID: 30955
		public float navMeshSampleRange = 5f;

		// Token: 0x040078EC RID: 30956
		internal readonly List<Transform> waypoints = new List<Transform>();

		// Token: 0x040078ED RID: 30957
		internal float defaultSpeed;

		// Token: 0x040078EE RID: 30958
		public Transform followTarget;

		// Token: 0x040078EF RID: 30959
		public NetPlayer targetPlayer;

		// Token: 0x040078F0 RID: 30960
		public bool targetIsOnNavMesh;
	}
}
