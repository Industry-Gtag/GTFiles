using System;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x0200052B RID: 1323
public class GeodeItem : TransferrableObject
{
	// Token: 0x06002130 RID: 8496 RVA: 0x000B19E9 File Offset: 0x000AFBE9
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.hasEffectsGameObject = this.effectsGameObject != null;
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x06002131 RID: 8497 RVA: 0x000B1A0B File Offset: 0x000AFC0B
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.prevItemState = TransferrableObject.ItemStates.State0;
		this.InitToDefault();
	}

	// Token: 0x06002132 RID: 8498 RVA: 0x000B1A27 File Offset: 0x000AFC27
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06002133 RID: 8499 RVA: 0x000B1A3C File Offset: 0x000AFC3C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return base.OnRelease(zoneReleased, releasingHand) && this.itemState != TransferrableObject.ItemStates.State0 && !base.InHand();
	}

	// Token: 0x06002134 RID: 8500 RVA: 0x000B1A60 File Offset: 0x000AFC60
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		UnityEvent<GeodeItem> onGeodeGrabbed = this.OnGeodeGrabbed;
		if (onGeodeGrabbed == null)
		{
			return;
		}
		onGeodeGrabbed.Invoke(this);
	}

	// Token: 0x06002135 RID: 8501 RVA: 0x000B1A7C File Offset: 0x000AFC7C
	private void InitToDefault()
	{
		this.cooldownRemaining = 0f;
		this.effectsHaveBeenPlayed = false;
		if (this.hasEffectsGameObject)
		{
			this.effectsGameObject.SetActive(false);
		}
		this.geodeFullMesh.SetActive(true);
		for (int i = 0; i < this.geodeCrackedMeshes.Length; i++)
		{
			this.geodeCrackedMeshes[i].SetActive(false);
		}
		this.hitLastFrame = false;
	}

	// Token: 0x06002136 RID: 8502 RVA: 0x000B1AE4 File Offset: 0x000AFCE4
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			this.cooldownRemaining -= Time.deltaTime;
			if (this.cooldownRemaining <= 0f)
			{
				this.itemState = TransferrableObject.ItemStates.State0;
				this.OnItemStateChanged();
			}
			return;
		}
		if (this.velocityEstimator.linearVelocity.magnitude < this.minHitVelocity)
		{
			return;
		}
		if (base.InHand())
		{
			int num = Physics.SphereCastNonAlloc(this.geodeFullMesh.transform.position, this.sphereRayRadius * Mathf.Abs(this.geodeFullMesh.transform.lossyScale.x), this.geodeFullMesh.transform.TransformDirection(Vector3.forward), this.collidersHit, this.rayCastMaxDistance, this.collisionLayerMask, QueryTriggerInteraction.Collide);
			this.hitLastFrame = num > 0;
		}
		if (!this.hitLastFrame)
		{
			return;
		}
		if (!GorillaParent.hasInstance)
		{
			return;
		}
		UnityEvent<GeodeItem> onGeodeCracked = this.OnGeodeCracked;
		if (onGeodeCracked != null)
		{
			onGeodeCracked.Invoke(this);
		}
		this.itemState = TransferrableObject.ItemStates.State1;
		this.cooldownRemaining = this.cooldown;
		this.index = (this.randomizeGeode ? this.RandomPickCrackedGeode() : 0);
	}

	// Token: 0x06002137 RID: 8503 RVA: 0x000B1C0C File Offset: 0x000AFE0C
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		this.currentItemState = this.itemState;
		if (this.currentItemState != this.prevItemState)
		{
			this.OnItemStateChanged();
		}
		this.prevItemState = this.currentItemState;
	}

	// Token: 0x06002138 RID: 8504 RVA: 0x000B1C40 File Offset: 0x000AFE40
	private void OnItemStateChanged()
	{
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.InitToDefault();
			return;
		}
		this.geodeFullMesh.SetActive(false);
		for (int i = 0; i < this.geodeCrackedMeshes.Length; i++)
		{
			this.geodeCrackedMeshes[i].SetActive(i == this.index);
		}
		RigContainer rigContainer;
		if (NetworkSystem.Instance.InRoom && GorillaGameManager.instance != null && !this.effectsHaveBeenPlayed && VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.LocalPlayer, out rigContainer))
		{
			rigContainer.Rig.netView.SendRPC("RPC_PlayGeodeEffect", RpcTarget.All, new object[] { this.geodeFullMesh.transform.position });
			this.effectsHaveBeenPlayed = true;
		}
		if (!NetworkSystem.Instance.InRoom && !this.effectsHaveBeenPlayed)
		{
			if (this.audioSource)
			{
				this.audioSource.GTPlay();
			}
			this.effectsHaveBeenPlayed = true;
		}
	}

	// Token: 0x06002139 RID: 8505 RVA: 0x000B1D39 File Offset: 0x000AFF39
	private int RandomPickCrackedGeode()
	{
		return Random.Range(0, this.geodeCrackedMeshes.Length);
	}

	// Token: 0x04002C00 RID: 11264
	[Tooltip("This GameObject will activate when the geode hits the ground with enough force.")]
	public GameObject effectsGameObject;

	// Token: 0x04002C01 RID: 11265
	public LayerMask collisionLayerMask;

	// Token: 0x04002C02 RID: 11266
	[Tooltip("Used to calculate velocity of the geode.")]
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04002C03 RID: 11267
	public float cooldown = 5f;

	// Token: 0x04002C04 RID: 11268
	[Tooltip("The velocity of the geode must be greater than this value to activate the effect.")]
	public float minHitVelocity = 0.2f;

	// Token: 0x04002C05 RID: 11269
	[Tooltip("Geode's full mesh before cracking")]
	public GameObject geodeFullMesh;

	// Token: 0x04002C06 RID: 11270
	[Tooltip("Geode's cracked open half different meshes, picked randomly")]
	public GameObject[] geodeCrackedMeshes;

	// Token: 0x04002C07 RID: 11271
	[Tooltip("The distance between te geode and the layer mask to detect whether it hits it")]
	public float rayCastMaxDistance = 0.2f;

	// Token: 0x04002C08 RID: 11272
	[FormerlySerializedAs("collisionRadius")]
	public float sphereRayRadius = 0.05f;

	// Token: 0x04002C09 RID: 11273
	[DebugReadout]
	private float cooldownRemaining;

	// Token: 0x04002C0A RID: 11274
	[DebugReadout]
	private bool hitLastFrame;

	// Token: 0x04002C0B RID: 11275
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002C0C RID: 11276
	public bool randomizeGeode = true;

	// Token: 0x04002C0D RID: 11277
	public UnityEvent<GeodeItem> OnGeodeCracked;

	// Token: 0x04002C0E RID: 11278
	public UnityEvent<GeodeItem> OnGeodeGrabbed;

	// Token: 0x04002C0F RID: 11279
	private bool hasEffectsGameObject;

	// Token: 0x04002C10 RID: 11280
	private bool effectsHaveBeenPlayed;

	// Token: 0x04002C11 RID: 11281
	private RaycastHit hit;

	// Token: 0x04002C12 RID: 11282
	private RaycastHit[] collidersHit = new RaycastHit[20];

	// Token: 0x04002C13 RID: 11283
	private TransferrableObject.ItemStates currentItemState;

	// Token: 0x04002C14 RID: 11284
	private TransferrableObject.ItemStates prevItemState;

	// Token: 0x04002C15 RID: 11285
	private int index;
}
