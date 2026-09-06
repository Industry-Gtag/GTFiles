using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Audio;
using Voxels;

// Token: 0x020001F8 RID: 504
public class Voxel_Pickaxe : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06000D3F RID: 3391 RVA: 0x00048B4E File Offset: 0x00046D4E
	// (set) Token: 0x06000D40 RID: 3392 RVA: 0x00048B56 File Offset: 0x00046D56
	public bool Held { get; set; }

	// Token: 0x06000D41 RID: 3393 RVA: 0x00048B5F File Offset: 0x00046D5F
	private void Reset()
	{
		this._layerMask = LayerMask.GetMask(new string[] { "Default" });
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x00048B7C File Offset: 0x00046D7C
	private void Awake()
	{
		this._gameEntity = base.GetComponent<GameEntity>();
		this._layerMask = LayerMask.GetMask(new string[] { "Default" });
		if (this.sound.transform == base.transform)
		{
			Debug.LogError("Audio source for " + base.name + " must be on a separate gameobject!", this);
		}
	}

	// Token: 0x06000D43 RID: 3395 RVA: 0x00048BE4 File Offset: 0x00046DE4
	private void OnEnable()
	{
		if (this._gameEntity != null)
		{
			GameEntity gameEntity = this._gameEntity;
			gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.StartGrabbing));
			GameEntity gameEntity2 = this._gameEntity;
			gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.StopGrabbing));
		}
		this._isLocal = base.GetComponentInParent<VRRig>() == VRRig.LocalRig;
		this.ResetVelocity();
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x00048C6C File Offset: 0x00046E6C
	private void OnDisable()
	{
		if (this._gameEntity)
		{
			GameEntity gameEntity = this._gameEntity;
			gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.StartGrabbing));
			GameEntity gameEntity2 = this._gameEntity;
			gameEntity2.OnReleased = (Action)Delegate.Remove(gameEntity2.OnReleased, new Action(this.StopGrabbing));
		}
	}

	// Token: 0x06000D45 RID: 3397 RVA: 0x00048CD4 File Offset: 0x00046ED4
	private void FixedUpdate()
	{
		if (!this.Held)
		{
			return;
		}
		for (int i = 0; i < this.points.Length; i++)
		{
			this.UpdateInteractionPoint(ref this.points[i]);
		}
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x00048D10 File Offset: 0x00046F10
	private void StartGrabbing()
	{
		if (this._gameEntity.heldByActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
		{
			return;
		}
		this.Held = true;
		VRRig componentInParent = base.GetComponentInParent<VRRig>();
		this._isLocal = componentInParent == VRRig.LocalRig;
		this.ResetVelocity();
	}

	// Token: 0x06000D47 RID: 3399 RVA: 0x00048D5A File Offset: 0x00046F5A
	private void StopGrabbing()
	{
		if (this._gameEntity.lastHeldByActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
		{
			return;
		}
		this.Held = false;
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x00048D7C File Offset: 0x00046F7C
	private void ResetVelocity()
	{
		for (int i = 0; i < this.points.Length; i++)
		{
			this.points[i].position = (this.points[i].previousPosition = this.points[i].transform.position);
		}
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x00048DD8 File Offset: 0x00046FD8
	private void UpdateInteractionPoint(ref Voxel_Pickaxe.InteractionPoint point)
	{
		point.previousPosition = point.position;
		point.position = point.transform.position;
		if (Time.time < this._nextHitTime)
		{
			return;
		}
		Vector3 vector = (point.position - point.previousPosition) / Time.fixedDeltaTime;
		float magnitude = vector.magnitude;
		if (magnitude < this.minHitSpeed)
		{
			return;
		}
		bool flag = Vector3.Dot(vector.normalized, point.transform.forward) >= this.alignThreshold;
		RaycastHit raycastHit;
		if (Physics.Linecast(point.previousPosition, point.position, out raycastHit, this._layerMask, QueryTriggerInteraction.Ignore))
		{
			ChunkComponent component = raycastHit.collider.GetComponent<ChunkComponent>();
			if (component && flag && magnitude >= this.minMineSpeed)
			{
				this.Play(this.goodHit, raycastHit.point);
				if (this._isLocal)
				{
					component.World.Mine(raycastHit, this.mine);
				}
			}
			else
			{
				this.Play(this.badHit, raycastHit.point);
			}
			this._nextHitTime = Time.time + this.hitCooldown;
		}
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x00048EF4 File Offset: 0x000470F4
	private void Play(AudioResource resource, Vector3 position)
	{
		if (!resource)
		{
			return;
		}
		this.sound.Stop();
		this.sound.resource = resource;
		this.sound.transform.position = position;
		this.sound.Play();
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x00048F32 File Offset: 0x00047132
	public void OnEntityInit()
	{
		if (this.sound != null)
		{
			this.sound.transform.parent = null;
		}
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x00048F54 File Offset: 0x00047154
	public void OnEntityDestroy()
	{
		if (ApplicationQuittingState.IsQuitting || this == null || this.sound == null || !base.gameObject.scene.isLoaded)
		{
			return;
		}
		this.sound.transform.parent = base.transform;
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long newState)
	{
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x00048FAC File Offset: 0x000471AC
	private void OnDrawGizmosSelected()
	{
		if (this.points == null)
		{
			return;
		}
		Gizmos.color = Color.green;
		foreach (Voxel_Pickaxe.InteractionPoint interactionPoint in this.points)
		{
			Gizmos.DrawWireSphere(interactionPoint.transform.position, 0.02f);
			Gizmos.DrawLine(interactionPoint.transform.position, interactionPoint.transform.position + interactionPoint.transform.forward * 0.5f);
		}
	}

	// Token: 0x04000FDD RID: 4061
	public VoxelAction mine = new VoxelAction
	{
		strength = 1f,
		radius = 0.5f,
		operation = OperationType.Subtract
	};

	// Token: 0x04000FDE RID: 4062
	public Voxel_Pickaxe.InteractionPoint[] points;

	// Token: 0x04000FDF RID: 4063
	public AudioResource goodHit;

	// Token: 0x04000FE0 RID: 4064
	public AudioResource badHit;

	// Token: 0x04000FE1 RID: 4065
	public AudioSource sound;

	// Token: 0x04000FE2 RID: 4066
	public float hitCooldown = 0.5f;

	// Token: 0x04000FE3 RID: 4067
	public float minHitSpeed = 1f;

	// Token: 0x04000FE4 RID: 4068
	public float minMineSpeed = 5f;

	// Token: 0x04000FE5 RID: 4069
	public float alignThreshold = 0.7f;

	// Token: 0x04000FE6 RID: 4070
	private GameEntity _gameEntity;

	// Token: 0x04000FE7 RID: 4071
	private int _layerMask;

	// Token: 0x04000FE8 RID: 4072
	private float _nextHitTime;

	// Token: 0x04000FE9 RID: 4073
	private bool _isLocal;

	// Token: 0x020001F9 RID: 505
	[Serializable]
	public struct InteractionPoint
	{
		// Token: 0x04000FEB RID: 4075
		public Transform transform;

		// Token: 0x04000FEC RID: 4076
		public Vector3 previousPosition;

		// Token: 0x04000FED RID: 4077
		public Vector3 position;
	}
}
