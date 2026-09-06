using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxels;

// Token: 0x020001F4 RID: 500
public class VoxelInteractor : MonoBehaviour
{
	// Token: 0x06000D2A RID: 3370 RVA: 0x00048621 File Offset: 0x00046821
	private void OnDisable()
	{
		this.StopOngoingAction();
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x00048629 File Offset: 0x00046829
	public void StartOngoingAction()
	{
		if (this._active)
		{
			return;
		}
		if (this._actionRoutine != null)
		{
			base.StopCoroutine(this._actionRoutine);
		}
		this._active = true;
		this._actionRoutine = base.StartCoroutine(this.DoContinuousAction());
	}

	// Token: 0x06000D2C RID: 3372 RVA: 0x00048661 File Offset: 0x00046861
	public void StopOngoingAction()
	{
		if (this._actionRoutine != null)
		{
			base.StopCoroutine(this._actionRoutine);
			this._actionRoutine = null;
		}
		this._active = false;
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x00048688 File Offset: 0x00046888
	public void PerformAction()
	{
		if (Time.time < this._nextActionTime)
		{
			return;
		}
		RaycastHit raycastHit;
		if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.forward * this.rayLength, out raycastHit, this.layerMask, QueryTriggerInteraction.Ignore))
		{
			ChunkComponent component = raycastHit.collider.GetComponent<ChunkComponent>();
			if (component)
			{
				component.World.Mine(raycastHit, this.action);
			}
		}
		this._nextActionTime = Time.time + this.cooldown;
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x00048724 File Offset: 0x00046924
	public void PerformActionOmnidirectional()
	{
		if (Time.time < this._nextActionTime)
		{
			return;
		}
		if (VoxelInteractor._hitWorlds == null)
		{
			VoxelInteractor._hitWorlds = new List<VoxelWorld>();
		}
		if (VoxelInteractor._hitColliders == null)
		{
			VoxelInteractor._hitColliders = new Collider[20];
		}
		VoxelInteractor._hitWorlds.Clear();
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.action.radius, VoxelInteractor._hitColliders, this.layerMask);
		if (num == VoxelInteractor._hitColliders.Length)
		{
			Array.Resize<Collider>(ref VoxelInteractor._hitColliders, VoxelInteractor._hitColliders.Length * 2);
		}
		for (int i = 0; i < num; i++)
		{
			ChunkComponent chunkComponent;
			if (VoxelInteractor._hitColliders[i].TryGetComponent<ChunkComponent>(out chunkComponent) && !VoxelInteractor._hitWorlds.Contains(chunkComponent.World))
			{
				VoxelInteractor._hitWorlds.Add(chunkComponent.World);
				chunkComponent.World.PerformAction(base.transform.position, this.action);
			}
		}
		this._nextActionTime = Time.time + this.cooldown;
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x00048822 File Offset: 0x00046A22
	private IEnumerator DoContinuousAction()
	{
		while (this._active)
		{
			while (Time.time < this._nextActionTime)
			{
				yield return null;
			}
			if (this._active)
			{
				this.PerformAction();
			}
		}
		this._actionRoutine = null;
		yield break;
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x00048834 File Offset: 0x00046A34
	public bool ApplyVoxelAction(Collision collision)
	{
		ChunkComponent component = collision.gameObject.GetComponent<ChunkComponent>();
		if (component)
		{
			component.World.Mine(collision, this.action);
		}
		return component;
	}

	// Token: 0x06000D31 RID: 3377 RVA: 0x00048870 File Offset: 0x00046A70
	public bool ApplyVoxelAction(RaycastHit hit)
	{
		ChunkComponent component = hit.collider.GetComponent<ChunkComponent>();
		if (component)
		{
			component.World.Mine(hit, this.action);
		}
		return component;
	}

	// Token: 0x06000D32 RID: 3378 RVA: 0x000488AC File Offset: 0x00046AAC
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Vector3 vector = base.transform.position + base.transform.forward * this.rayLength;
		Gizmos.DrawLine(base.transform.position, vector);
		Gizmos.DrawWireSphere(base.transform.position, 0.01f);
		Gizmos.DrawWireSphere(vector, 0.01f);
	}

	// Token: 0x04000FC8 RID: 4040
	[SerializeField]
	private LayerMask layerMask = 1;

	// Token: 0x04000FC9 RID: 4041
	[SerializeField]
	private float rayLength = 0.1f;

	// Token: 0x04000FCA RID: 4042
	[SerializeField]
	[Range(0.25f, 2f)]
	private float cooldown = 0.25f;

	// Token: 0x04000FCB RID: 4043
	[SerializeField]
	private VoxelAction action = new VoxelAction
	{
		strength = 0.5f,
		radius = 0.5f,
		operation = OperationType.Subtract
	};

	// Token: 0x04000FCC RID: 4044
	private bool _active;

	// Token: 0x04000FCD RID: 4045
	private Coroutine _actionRoutine;

	// Token: 0x04000FCE RID: 4046
	private float _nextActionTime;

	// Token: 0x04000FCF RID: 4047
	[OnEnterPlay_SetNull]
	private static List<VoxelWorld> _hitWorlds;

	// Token: 0x04000FD0 RID: 4048
	[OnEnterPlay_SetNull]
	private static Collider[] _hitColliders;
}
