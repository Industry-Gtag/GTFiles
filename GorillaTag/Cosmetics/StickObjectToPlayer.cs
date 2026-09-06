using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012F2 RID: 4850
	public class StickObjectToPlayer : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000BCB RID: 3019
		// (get) Token: 0x06007991 RID: 31121 RVA: 0x0027A42E File Offset: 0x0027862E
		// (set) Token: 0x06007992 RID: 31122 RVA: 0x0027A436 File Offset: 0x00278636
		public bool TickRunning { get; set; }

		// Token: 0x06007993 RID: 31123 RVA: 0x0027A43F File Offset: 0x0027863F
		public void Tick()
		{
			if (!this.canSpawn && Time.time - this.lastSpawnedTime >= this.cooldown)
			{
				this.canSpawn = true;
			}
		}

		// Token: 0x06007994 RID: 31124 RVA: 0x0027A464 File Offset: 0x00278664
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
			this.canSpawn = true;
		}

		// Token: 0x06007995 RID: 31125 RVA: 0x0001A29F File Offset: 0x0001849F
		private void OnDisable()
		{
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06007996 RID: 31126 RVA: 0x0027A473 File Offset: 0x00278673
		public void SetOwner(NetPlayer player)
		{
			this.ownerPlayer = player;
		}

		// Token: 0x06007997 RID: 31127 RVA: 0x0027A47C File Offset: 0x0027867C
		private Transform MakeOrGetStickyContainer(Transform parent)
		{
			Transform transform = parent;
			foreach (Transform transform2 in parent.GetComponentsInChildren<Transform>(true))
			{
				if (!this.firstPersonView && transform2.CompareTag(this.parentTag))
				{
					transform = transform2;
					break;
				}
			}
			string text = "StickyObjects_" + this.objectToSpawn.name;
			Transform transform3 = transform.Find(text);
			if (transform3 != null)
			{
				return transform3;
			}
			GameObject gameObject = new GameObject(text);
			gameObject.transform.SetParent(transform, false);
			return gameObject.transform;
		}

		// Token: 0x06007998 RID: 31128 RVA: 0x0027A508 File Offset: 0x00278708
		public void Stick(bool leftHand, Collider other)
		{
			if (!this.canSpawn || other == null || !base.enabled)
			{
				return;
			}
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			if (!componentInParent)
			{
				return;
			}
			if (this.ownerPlayer != null && componentInParent.creator == this.ownerPlayer)
			{
				return;
			}
			Vector3 vector = ((this.spawnerRigidbody != null) ? this.spawnerRigidbody.linearVelocity : Vector3.zero);
			Vector3 vector2 = Time.fixedDeltaTime * 2f * vector;
			Vector3 vector3 = vector2.normalized;
			if (vector3 == Vector3.zero)
			{
				vector3 = base.transform.forward;
				vector2 = vector3 * 0.01f;
			}
			Vector3 vector4 = base.transform.position - vector2;
			Vector3 vector5;
			if (this.alignToHitNormal)
			{
				float magnitude = vector2.magnitude;
				RaycastHit raycastHit;
				if (other.Raycast(new Ray(vector4, vector3), out raycastHit, 2f * magnitude))
				{
					vector5 = raycastHit.point;
				}
				else
				{
					vector5 = other.ClosestPoint(vector4);
				}
			}
			else
			{
				vector5 = other.ClosestPoint(vector4);
			}
			Vector3 vector6 = this.GetSpawnPosition(this.spawnLocation, componentInParent).TransformPoint(this.positionOffset);
			if ((vector5 - vector6).magnitude <= this.stickRadius * componentInParent.scaleFactor)
			{
				if (NetworkSystem.Instance.LocalPlayer == componentInParent.creator)
				{
					if (this.firstPersonView && this.spawnLocation == StickObjectToPlayer.SpawnLocation.Head)
					{
						this.StickFirstPersonView();
					}
				}
				else
				{
					if (!this.thirdPersonView)
					{
						return;
					}
					Transform transform = this.MakeOrGetStickyContainer(componentInParent.transform);
					this.StickTo(transform, vector6, this.localEulerAngles);
				}
				UnityEvent onStickShared = this.OnStickShared;
				if (onStickShared == null)
				{
					return;
				}
				onStickShared.Invoke();
			}
		}

		// Token: 0x06007999 RID: 31129 RVA: 0x0027A6B4 File Offset: 0x002788B4
		private void StickFirstPersonView()
		{
			Transform cosmeticsHeadTarget = GTPlayer.Instance.CosmeticsHeadTarget;
			Vector3 vector = cosmeticsHeadTarget.TransformPoint(this.FPVOffset);
			Transform transform = this.MakeOrGetStickyContainer(cosmeticsHeadTarget);
			this.StickTo(transform, vector, this.FPVlocalEulerAngles);
		}

		// Token: 0x0600799A RID: 31130 RVA: 0x0027A6F0 File Offset: 0x002788F0
		private void StickTo(Transform parent, Vector3 position, Vector3 eulerAngle)
		{
			int num = 0;
			for (int i = 0; i < parent.childCount; i++)
			{
				if (parent.GetChild(i).gameObject.activeInHierarchy)
				{
					num++;
				}
			}
			if (num >= this.maxActiveStickies)
			{
				return;
			}
			this.stickyObject = ObjectPools.instance.Instantiate(this.objectToSpawn, true);
			if (this.stickyObject == null)
			{
				return;
			}
			this.stickyObject.transform.SetParent(parent, false);
			this.stickyObject.transform.position = position;
			this.stickyObject.transform.localEulerAngles = eulerAngle;
			this.lastSpawnedTime = Time.time;
			this.canSpawn = false;
		}

		// Token: 0x0600799B RID: 31131 RVA: 0x0027A7A0 File Offset: 0x002789A0
		private Transform GetSpawnPosition(StickObjectToPlayer.SpawnLocation spawnType, VRRig hitRig)
		{
			switch (spawnType)
			{
			case StickObjectToPlayer.SpawnLocation.Head:
				return hitRig.head.rigTarget.transform;
			case StickObjectToPlayer.SpawnLocation.RightHand:
				return hitRig.rightHand.rigTarget.transform;
			case StickObjectToPlayer.SpawnLocation.LeftHand:
				return hitRig.leftHand.rigTarget.transform;
			default:
				return null;
			}
		}

		// Token: 0x0600799C RID: 31132 RVA: 0x0027A7F8 File Offset: 0x002789F8
		public void Debug_StickToLocalPlayer()
		{
			Vector3 vector = this.GetSpawnPosition(this.spawnLocation, VRRig.LocalRig).TransformPoint(this.positionOffset);
			this.StickTo(VRRig.LocalRig.transform, vector, this.localEulerAngles);
		}

		// Token: 0x0600799D RID: 31133 RVA: 0x0027A839 File Offset: 0x00278A39
		public void Debug_StickToLocalPlayerFPV()
		{
			this.StickFirstPersonView();
		}

		// Token: 0x04008AD2 RID: 35538
		[Header("Shared Settings")]
		[Tooltip("Must be in the global object pool and have a tag.")]
		[SerializeField]
		private GameObject objectToSpawn;

		// Token: 0x04008AD3 RID: 35539
		[Tooltip("Optional: how many objects can be active at once")]
		[SerializeField]
		private int maxActiveStickies = 1;

		// Token: 0x04008AD4 RID: 35540
		[SerializeField]
		private StickObjectToPlayer.SpawnLocation spawnLocation;

		// Token: 0x04008AD5 RID: 35541
		[SerializeField]
		private float stickRadius = 0.5f;

		// Token: 0x04008AD6 RID: 35542
		[SerializeField]
		private bool alignToHitNormal = true;

		// Token: 0x04008AD7 RID: 35543
		[SerializeField]
		private Rigidbody spawnerRigidbody;

		// Token: 0x04008AD8 RID: 35544
		[SerializeField]
		private string parentTag = "GorillaHead";

		// Token: 0x04008AD9 RID: 35545
		[SerializeField]
		private float cooldown;

		// Token: 0x04008ADA RID: 35546
		[Header("Third Person View")]
		[Tooltip("If you are only interested in the FPV, don't check this box so that others don't see it.")]
		[SerializeField]
		private bool thirdPersonView = true;

		// Token: 0x04008ADB RID: 35547
		[SerializeField]
		private Vector3 positionOffset = new Vector3(0f, 0.02f, 0.17f);

		// Token: 0x04008ADC RID: 35548
		[Tooltip("Local rotation to apply to the spawned object (Euler angles, degrees)")]
		[SerializeField]
		private Vector3 localEulerAngles = Vector3.zero;

		// Token: 0x04008ADD RID: 35549
		[Header("First Person View")]
		[SerializeField]
		private bool firstPersonView;

		// Token: 0x04008ADE RID: 35550
		[SerializeField]
		private Vector3 FPVOffset = new Vector3(0f, 0.02f, 0.17f);

		// Token: 0x04008ADF RID: 35551
		[Tooltip("Local rotation to apply to the spawned object (Euler angles, degrees)")]
		[SerializeField]
		private Vector3 FPVlocalEulerAngles = Vector3.zero;

		// Token: 0x04008AE0 RID: 35552
		[Header("Events")]
		public UnityEvent OnStickShared;

		// Token: 0x04008AE1 RID: 35553
		private GameObject stickyObject;

		// Token: 0x04008AE2 RID: 35554
		private float lastSpawnedTime;

		// Token: 0x04008AE3 RID: 35555
		private bool canSpawn = true;

		// Token: 0x04008AE4 RID: 35556
		private NetPlayer ownerPlayer;

		// Token: 0x020012F3 RID: 4851
		private enum SpawnLocation
		{
			// Token: 0x04008AE7 RID: 35559
			Head,
			// Token: 0x04008AE8 RID: 35560
			RightHand,
			// Token: 0x04008AE9 RID: 35561
			LeftHand
		}
	}
}
