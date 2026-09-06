using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020011A1 RID: 4513
	public class CustomRopeSimulation : MonoBehaviour
	{
		// Token: 0x060071D0 RID: 29136 RVA: 0x00250994 File Offset: 0x0024EB94
		private void Start()
		{
			Vector3 position = base.transform.position;
			for (int i = 0; i < this.nodeCount; i++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.ropeNodePrefab);
				gameObject.transform.parent = base.transform;
				gameObject.transform.position = position;
				this.nodes.Add(gameObject.transform);
				position.y -= this.nodeDistance;
			}
			this.nodes[this.nodes.Count - 1].GetComponentInChildren<Renderer>().enabled = false;
			this.burstNodes = new NativeArray<BurstRopeNode>(this.nodes.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		}

		// Token: 0x060071D1 RID: 29137 RVA: 0x00250A44 File Offset: 0x0024EC44
		private void OnDestroy()
		{
			this.burstNodes.Dispose();
		}

		// Token: 0x060071D2 RID: 29138 RVA: 0x00250A54 File Offset: 0x0024EC54
		private void Update()
		{
			new SolveRopeJob
			{
				fixedDeltaTime = Time.deltaTime,
				gravity = this.gravity,
				nodes = this.burstNodes,
				nodeDistance = this.nodeDistance,
				rootPos = base.transform.position
			}.Run<SolveRopeJob>();
			for (int i = 0; i < this.burstNodes.Length; i++)
			{
				this.nodes[i].position = this.burstNodes[i].curPos;
				if (i > 0)
				{
					Vector3 vector = this.burstNodes[i - 1].curPos - this.burstNodes[i].curPos;
					this.nodes[i].up = -vector;
				}
			}
		}

		// Token: 0x04008291 RID: 33425
		private List<Transform> nodes = new List<Transform>();

		// Token: 0x04008292 RID: 33426
		[SerializeField]
		private GameObject ropeNodePrefab;

		// Token: 0x04008293 RID: 33427
		[SerializeField]
		private int nodeCount = 10;

		// Token: 0x04008294 RID: 33428
		[SerializeField]
		private float nodeDistance = 0.4f;

		// Token: 0x04008295 RID: 33429
		[SerializeField]
		private Vector3 gravity = new Vector3(0f, -9.81f, 0f);

		// Token: 0x04008296 RID: 33430
		private NativeArray<BurstRopeNode> burstNodes;
	}
}
