using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000527 RID: 1319
public class DrumsItem : MonoBehaviour, ISpawnable
{
	// Token: 0x17000391 RID: 913
	// (get) Token: 0x06002115 RID: 8469 RVA: 0x000B0F19 File Offset: 0x000AF119
	// (set) Token: 0x06002116 RID: 8470 RVA: 0x000B0F21 File Offset: 0x000AF121
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000392 RID: 914
	// (get) Token: 0x06002117 RID: 8471 RVA: 0x000B0F2A File Offset: 0x000AF12A
	// (set) Token: 0x06002118 RID: 8472 RVA: 0x000B0F32 File Offset: 0x000AF132
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06002119 RID: 8473 RVA: 0x000B0F3C File Offset: 0x000AF13C
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		this.leftHandIndicator = GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.rightHandIndicator = GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.sphereRadius = this.leftHandIndicator.GetComponent<SphereCollider>().radius;
		for (int i = 0; i < this.collidersForThisDrum.Length; i++)
		{
			this.collidersForThisDrumList.Add(this.collidersForThisDrum[i]);
		}
		for (int j = 0; j < this.drumsAS.Length; j++)
		{
			this.myRig.AssignDrumToMusicDrums(j + this.onlineOffset, this.drumsAS[j]);
		}
	}

	// Token: 0x0600211A RID: 8474 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x0600211B RID: 8475 RVA: 0x000B0FE4 File Offset: 0x000AF1E4
	private void LateUpdate()
	{
		this.CheckHandHit(ref this.leftHandIn, ref this.leftHandIndicator, true);
		this.CheckHandHit(ref this.rightHandIn, ref this.rightHandIndicator, false);
	}

	// Token: 0x0600211C RID: 8476 RVA: 0x000B100C File Offset: 0x000AF20C
	private void CheckHandHit(ref bool handIn, ref GorillaTriggerColliderHandIndicator handIndicator, bool isLeftHand)
	{
		this.spherecastSweep = handIndicator.transform.position - handIndicator.lastPosition;
		if (this.spherecastSweep.magnitude < 0.0001f)
		{
			this.spherecastSweep = Vector3.up * 0.0001f;
		}
		for (int i = 0; i < this.collidersHit.Length; i++)
		{
			this.collidersHit[i] = this.nullHit;
		}
		this.collidersHitCount = Physics.SphereCastNonAlloc(handIndicator.lastPosition, this.sphereRadius, this.spherecastSweep.normalized, this.collidersHit, this.spherecastSweep.magnitude, this.drumsTouchable, QueryTriggerInteraction.Collide);
		this.drumHit = false;
		if (this.collidersHitCount > 0)
		{
			this.hitList.Clear();
			for (int j = 0; j < this.collidersHit.Length; j++)
			{
				if (this.collidersHit[j].collider != null && this.collidersForThisDrumList.Contains(this.collidersHit[j].collider) && this.collidersHit[j].collider.gameObject.activeSelf)
				{
					this.hitList.Add(this.collidersHit[j]);
				}
			}
			this.hitList.Sort(new Comparison<RaycastHit>(this.RayCastHitCompare));
			int k = 0;
			while (k < this.hitList.Count)
			{
				this.tempDrum = this.hitList[k].collider.GetComponent<Drum>();
				if (this.tempDrum != null)
				{
					this.drumHit = true;
					if (!handIn && !this.tempDrum.disabler)
					{
						this.DrumHit(this.tempDrum, isLeftHand, handIndicator.currentVelocity.magnitude);
						break;
					}
					break;
				}
				else
				{
					k++;
				}
			}
		}
		if (!this.drumHit & handIn)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration);
		}
		handIn = this.drumHit;
	}

	// Token: 0x0600211D RID: 8477 RVA: 0x000B1227 File Offset: 0x000AF427
	private int RayCastHitCompare(RaycastHit a, RaycastHit b)
	{
		if (a.distance < b.distance)
		{
			return -1;
		}
		if (a.distance == b.distance)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x0600211E RID: 8478 RVA: 0x000B1250 File Offset: 0x000AF450
	public void DrumHit(Drum tempDrumInner, bool isLeftHand, float hitVelocity)
	{
		if (isLeftHand)
		{
			if (this.leftHandIn)
			{
				return;
			}
			this.leftHandIn = true;
		}
		else
		{
			if (this.rightHandIn)
			{
				return;
			}
			this.rightHandIn = true;
		}
		this.volToPlay = Mathf.Max(Mathf.Min(1f, hitVelocity / this.maxDrumVolumeVelocity) * this.maxDrumVolume, this.minDrumVolume);
		if (NetworkSystem.Instance.InRoom)
		{
			if (!this.myRig.isOfflineVRRig)
			{
				NetworkView netView = this.myRig.netView;
				if (netView != null)
				{
					netView.SendRPC("RPC_PlayDrum", RpcTarget.Others, new object[]
					{
						tempDrumInner.myIndex + this.onlineOffset,
						this.volToPlay
					});
				}
			}
			else
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayDrum", RpcTarget.Others, new object[]
				{
					tempDrumInner.myIndex + this.onlineOffset,
					this.volToPlay
				});
			}
		}
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 4f, GorillaTagger.Instance.tapHapticDuration);
		this.drumsAS[tempDrumInner.myIndex].volume = this.maxDrumVolume;
		this.drumsAS[tempDrumInner.myIndex].GTPlayOneShot(this.drumsAS[tempDrumInner.myIndex].clip, this.volToPlay);
	}

	// Token: 0x04002BD2 RID: 11218
	[Tooltip("Array of colliders for this specific drum.")]
	public Collider[] collidersForThisDrum;

	// Token: 0x04002BD3 RID: 11219
	private List<Collider> collidersForThisDrumList = new List<Collider>();

	// Token: 0x04002BD4 RID: 11220
	[Tooltip("AudioSources where each index must match the index given to the corresponding Drum component.")]
	public AudioSource[] drumsAS;

	// Token: 0x04002BD5 RID: 11221
	[Tooltip("Max volume a drum can reach.")]
	public float maxDrumVolume = 0.2f;

	// Token: 0x04002BD6 RID: 11222
	[Tooltip("Min volume a drum can reach.")]
	public float minDrumVolume = 0.05f;

	// Token: 0x04002BD7 RID: 11223
	[Tooltip("Multiplies against actual velocity before capping by min & maxDrumVolume values.")]
	public float maxDrumVolumeVelocity = 1f;

	// Token: 0x04002BD8 RID: 11224
	private bool rightHandIn;

	// Token: 0x04002BD9 RID: 11225
	private bool leftHandIn;

	// Token: 0x04002BDA RID: 11226
	private float volToPlay;

	// Token: 0x04002BDB RID: 11227
	private GorillaTriggerColliderHandIndicator rightHandIndicator;

	// Token: 0x04002BDC RID: 11228
	private GorillaTriggerColliderHandIndicator leftHandIndicator;

	// Token: 0x04002BDD RID: 11229
	private RaycastHit[] collidersHit = new RaycastHit[20];

	// Token: 0x04002BDE RID: 11230
	private Collider[] actualColliders = new Collider[20];

	// Token: 0x04002BDF RID: 11231
	public LayerMask drumsTouchable;

	// Token: 0x04002BE0 RID: 11232
	private float sphereRadius;

	// Token: 0x04002BE1 RID: 11233
	private Vector3 spherecastSweep;

	// Token: 0x04002BE2 RID: 11234
	private int collidersHitCount;

	// Token: 0x04002BE3 RID: 11235
	private List<RaycastHit> hitList = new List<RaycastHit>(20);

	// Token: 0x04002BE4 RID: 11236
	private Drum tempDrum;

	// Token: 0x04002BE5 RID: 11237
	private bool drumHit;

	// Token: 0x04002BE6 RID: 11238
	private RaycastHit nullHit;

	// Token: 0x04002BE7 RID: 11239
	public int onlineOffset;

	// Token: 0x04002BE8 RID: 11240
	[Tooltip("VRRig object of the player, used to determine if it is an offline rig.")]
	private VRRig myRig;
}
