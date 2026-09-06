using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020011A5 RID: 4517
	public class GorillaRopeSwing : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x060071D8 RID: 29144 RVA: 0x00250D84 File Offset: 0x0024EF84
		private void EdRecalculateId()
		{
			this.CalculateId(true);
		}

		// Token: 0x17000B29 RID: 2857
		// (get) Token: 0x060071D9 RID: 29145 RVA: 0x00250D8D File Offset: 0x0024EF8D
		// (set) Token: 0x060071DA RID: 29146 RVA: 0x00250D95 File Offset: 0x0024EF95
		public bool isIdle { get; private set; }

		// Token: 0x17000B2A RID: 2858
		// (get) Token: 0x060071DB RID: 29147 RVA: 0x00250D9E File Offset: 0x0024EF9E
		// (set) Token: 0x060071DC RID: 29148 RVA: 0x00250DA6 File Offset: 0x0024EFA6
		public bool isFullyIdle { get; private set; }

		// Token: 0x17000B2B RID: 2859
		// (get) Token: 0x060071DD RID: 29149 RVA: 0x00250DAF File Offset: 0x0024EFAF
		public bool SupportsMovingAtRuntime
		{
			get
			{
				return this.supportMovingAtRuntime;
			}
		}

		// Token: 0x17000B2C RID: 2860
		// (get) Token: 0x060071DE RID: 29150 RVA: 0x00250DB7 File Offset: 0x0024EFB7
		public bool hasPlayers
		{
			get
			{
				return this.localPlayerOn || this.remotePlayers.Count > 0;
			}
		}

		// Token: 0x060071DF RID: 29151 RVA: 0x00250DD4 File Offset: 0x0024EFD4
		protected virtual void Awake()
		{
			base.transform.rotation = Quaternion.identity;
			this.scaleFactor = (base.transform.lossyScale.x + base.transform.lossyScale.y + base.transform.lossyScale.z) / 3f;
			this.SetIsIdle(true, false);
		}

		// Token: 0x060071E0 RID: 29152 RVA: 0x00250E37 File Offset: 0x0024F037
		protected virtual void Start()
		{
			if (!this.useStaticId)
			{
				this.CalculateId(false);
			}
			RopeSwingManager.Register(this);
			this.started = true;
		}

		// Token: 0x060071E1 RID: 29153 RVA: 0x00250E55 File Offset: 0x0024F055
		private void OnDestroy()
		{
			if (RopeSwingManager.instance != null)
			{
				RopeSwingManager.Unregister(this);
			}
		}

		// Token: 0x060071E2 RID: 29154 RVA: 0x00250E6C File Offset: 0x0024F06C
		protected virtual void OnEnable()
		{
			base.transform.rotation = Quaternion.identity;
			this.scaleFactor = (base.transform.lossyScale.x + base.transform.lossyScale.y + base.transform.lossyScale.z) / 3f;
			this.SetIsIdle(true, true);
			VectorizedCustomRopeSimulation.Register(this);
			GorillaRopeSwingUpdateManager.RegisterRopeSwing(this);
		}

		// Token: 0x060071E3 RID: 29155 RVA: 0x00250EDB File Offset: 0x0024F0DB
		private void OnDisable()
		{
			if (!this.isIdle)
			{
				this.SetIsIdle(true, true);
			}
			VectorizedCustomRopeSimulation.Unregister(this);
			GorillaRopeSwingUpdateManager.UnregisterRopeSwing(this);
		}

		// Token: 0x060071E4 RID: 29156 RVA: 0x00250EFC File Offset: 0x0024F0FC
		internal void CalculateId(bool force = false)
		{
			Transform transform = base.transform;
			int staticHash = TransformUtils.GetScenePath(transform).GetStaticHash();
			int staticHash2 = base.GetType().Name.GetStaticHash();
			int num = StaticHash.Compute(staticHash, staticHash2);
			if (this.useStaticId)
			{
				if (string.IsNullOrEmpty(this.staticId) || force)
				{
					Vector3 position = transform.position;
					int num2 = StaticHash.Compute(position.x, position.y, position.z);
					int instanceID = transform.GetInstanceID();
					int num3 = StaticHash.Compute(num, num2, instanceID);
					this.staticId = string.Format("#ID_{0:X8}", num3);
				}
				this.ropeId = this.staticId.GetStaticHash();
				return;
			}
			this.ropeId = (Application.isPlaying ? num : 0);
		}

		// Token: 0x060071E5 RID: 29157 RVA: 0x00250FB8 File Offset: 0x0024F1B8
		public void InvokeUpdate()
		{
			if (this.isIdle)
			{
				this.isFullyIdle = true;
			}
			if (!this.isIdle)
			{
				int num = -1;
				if (this.localPlayerOn)
				{
					num = this.localPlayerBoneIndex;
				}
				else if (this.remotePlayers.Count > 0)
				{
					num = this.remotePlayers.First<KeyValuePair<int, int>>().Value;
				}
				if (num >= 0 && VectorizedCustomRopeSimulation.instance.GetNodeVelocity(this, num).magnitude > 2f && !this.ropeCreakSFX.isPlaying && Mathf.RoundToInt(Time.time) % 5 == 0)
				{
					this.ropeCreakSFX.GTPlay();
				}
				if (this.localPlayerOn)
				{
					float num2 = MathUtils.Linear(this.velocityTracker.GetLatestVelocity(true).magnitude / this.scaleFactor, 0f, 10f, -0.07f, 0.5f);
					if (num2 > 0f)
					{
						GorillaTagger.Instance.DoVibration(this.localPlayerXRNode, num2, Time.deltaTime);
					}
				}
				Transform bone = this.GetBone(this.lastNodeCheckIndex);
				Vector3 nodeVelocity = VectorizedCustomRopeSimulation.instance.GetNodeVelocity(this, this.lastNodeCheckIndex);
				if (Physics.SphereCastNonAlloc(bone.position, 0.2f * this.scaleFactor, nodeVelocity.normalized, this.nodeHits, 0.4f * this.scaleFactor, this.wallLayerMask, QueryTriggerInteraction.Ignore) > 0)
				{
					this.SetVelocity(this.lastNodeCheckIndex, Vector3.zero, false, default(PhotonMessageInfoWrapped));
				}
				if (nodeVelocity.magnitude <= 0.35f)
				{
					this.potentialIdleTimer += Time.deltaTime;
				}
				else
				{
					this.potentialIdleTimer = 0f;
				}
				if (this.potentialIdleTimer >= 2f)
				{
					this.SetIsIdle(true, false);
					this.potentialIdleTimer = 0f;
				}
				this.lastNodeCheckIndex++;
				if (this.lastNodeCheckIndex > this.nodes.Length)
				{
					this.lastNodeCheckIndex = 2;
				}
			}
			if (this.hasMonkeBlockParent && this.supportMovingAtRuntime)
			{
				base.transform.rotation = Quaternion.Euler(0f, base.transform.parent.rotation.eulerAngles.y, 0f);
			}
		}

		// Token: 0x060071E6 RID: 29158 RVA: 0x002511EC File Offset: 0x0024F3EC
		private void SetIsIdle(bool idle, bool resetPos = false)
		{
			this.isIdle = idle;
			this.ropeCreakSFX.gameObject.SetActive(!idle);
			if (idle)
			{
				this.ToggleVelocityTracker(false, 0, default(Vector3));
				if (resetPos)
				{
					Vector3 vector = Vector3.zero;
					for (int i = 0; i < this.nodes.Length; i++)
					{
						this.nodes[i].transform.localRotation = Quaternion.identity;
						this.nodes[i].transform.localPosition = vector;
						vector += new Vector3(0f, -this.ropeBitGenOffset, 0f);
					}
					return;
				}
			}
			else
			{
				this.isFullyIdle = false;
			}
		}

		// Token: 0x060071E7 RID: 29159 RVA: 0x00251293 File Offset: 0x0024F493
		public Transform GetBone(int index)
		{
			if (index >= this.nodes.Length)
			{
				return this.nodes.Last<Transform>();
			}
			return this.nodes[index];
		}

		// Token: 0x060071E8 RID: 29160 RVA: 0x002512B4 File Offset: 0x0024F4B4
		public int GetBoneIndex(Transform r)
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (this.nodes[i] == r)
				{
					return i;
				}
			}
			return this.nodes.Length - 1;
		}

		// Token: 0x060071E9 RID: 29161 RVA: 0x002512F0 File Offset: 0x0024F4F0
		public void AttachLocalPlayer(XRNode xrNode, Transform grabbedBone, Vector3 offset, Vector3 velocity)
		{
			int boneIndex = this.GetBoneIndex(grabbedBone);
			this.localPlayerBoneIndex = boneIndex;
			velocity /= this.scaleFactor;
			velocity *= this.settings.inheritVelocityMultiplier;
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = this.ropeId;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeBoneIndex = boneIndex;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = xrNode == XRNode.LeftHand;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIsPhotonView = false;
			}
			this.RefreshAllBonesMass();
			List<Vector3> list = new List<Vector3>();
			if (this.remotePlayers.Count <= 0)
			{
				foreach (Transform transform in this.nodes)
				{
					list.Add(transform.position);
				}
			}
			velocity.y = 0f;
			if (Time.time - this.lastGrabTime > 1f && (this.remotePlayers.Count == 0 || velocity.magnitude > 2.5f))
			{
				RopeSwingManager.instance.SendSetVelocity_RPC(this.ropeId, boneIndex, velocity, true);
			}
			this.lastGrabTime = Time.time;
			this.ropeCreakSFX.transform.parent = this.GetBone(Math.Max(0, boneIndex - 3)).transform;
			this.ropeCreakSFX.transform.localPosition = Vector3.zero;
			this.localPlayerOn = true;
			this.localPlayerXRNode = xrNode;
			this.ToggleVelocityTracker(true, boneIndex, offset);
		}

		// Token: 0x060071EA RID: 29162 RVA: 0x00251489 File Offset: 0x0024F689
		public void DetachLocalPlayer()
		{
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
			}
			this.localPlayerOn = false;
			this.localPlayerBoneIndex = 0;
			this.RefreshAllBonesMass();
		}

		// Token: 0x060071EB RID: 29163 RVA: 0x002514C8 File Offset: 0x0024F6C8
		private void ToggleVelocityTracker(bool enable, int boneIndex = 0, Vector3 offset = default(Vector3))
		{
			if (enable)
			{
				this.velocityTracker.transform.SetParent(this.GetBone(boneIndex));
				this.velocityTracker.transform.localPosition = offset;
				this.velocityTracker.ResetState();
			}
			this.velocityTracker.gameObject.SetActive(enable);
			if (enable)
			{
				this.velocityTracker.Tick();
			}
		}

		// Token: 0x060071EC RID: 29164 RVA: 0x0025152C File Offset: 0x0024F72C
		private void RefreshAllBonesMass()
		{
			int num = 0;
			foreach (KeyValuePair<int, int> keyValuePair in this.remotePlayers)
			{
				if (keyValuePair.Value > num)
				{
					num = keyValuePair.Value;
				}
			}
			if (this.localPlayerBoneIndex > num)
			{
				num = this.localPlayerBoneIndex;
			}
			VectorizedCustomRopeSimulation.instance.SetMassForPlayers(this, this.hasPlayers, num);
		}

		// Token: 0x060071ED RID: 29165 RVA: 0x002515B0 File Offset: 0x0024F7B0
		public bool AttachRemotePlayer(int playerId, int boneIndex, Transform offsetTransform, Vector3 offset)
		{
			Transform bone = this.GetBone(boneIndex);
			if (bone == null)
			{
				return false;
			}
			offsetTransform.SetParent(bone.transform);
			offsetTransform.localPosition = offset;
			offsetTransform.localRotation = Quaternion.identity;
			if (this.remotePlayers.ContainsKey(playerId))
			{
				Debug.LogError("already on the list!");
				return false;
			}
			this.remotePlayers.Add(playerId, boneIndex);
			this.RefreshAllBonesMass();
			return true;
		}

		// Token: 0x060071EE RID: 29166 RVA: 0x0025161D File Offset: 0x0024F81D
		public void DetachRemotePlayer(int playerId)
		{
			this.remotePlayers.Remove(playerId);
			this.RefreshAllBonesMass();
		}

		// Token: 0x060071EF RID: 29167 RVA: 0x00251634 File Offset: 0x0024F834
		public void SetVelocity(int boneIndex, Vector3 velocity, bool wholeRope, PhotonMessageInfoWrapped info)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			float num = 10000f;
			if (!(in velocity).IsValid(in num))
			{
				return;
			}
			velocity.x = Mathf.Clamp(velocity.x, -100f, 100f);
			velocity.y = Mathf.Clamp(velocity.y, -100f, 100f);
			velocity.z = Mathf.Clamp(velocity.z, -100f, 100f);
			boneIndex = Mathf.Clamp(boneIndex, 0, this.nodes.Length);
			Transform bone = this.GetBone(boneIndex);
			if (!bone)
			{
				return;
			}
			if (info.Sender != null && !info.Sender.IsLocal)
			{
				VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
				if (!vrrig || Vector3.Distance(bone.position, vrrig.transform.position) > 5f)
				{
					return;
				}
			}
			this.SetIsIdle(false, false);
			if (bone)
			{
				VectorizedCustomRopeSimulation.instance.SetVelocity(this, velocity, wholeRope, boneIndex);
			}
		}

		// Token: 0x060071F0 RID: 29168 RVA: 0x0025173C File Offset: 0x0024F93C
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.monkeBlockParent = base.GetComponentInParent<BuilderPiece>();
			this.hasMonkeBlockParent = this.monkeBlockParent != null;
			int num = StaticHash.Compute(pieceType, pieceId);
			this.staticId = string.Format("#ID_{0:X8}", num);
			this.ropeId = this.staticId.GetStaticHash();
			GorillaRopeSwing gorillaRopeSwing;
			if (this.started && !RopeSwingManager.instance.TryGetRope(this.ropeId, out gorillaRopeSwing))
			{
				RopeSwingManager.Register(this);
			}
		}

		// Token: 0x060071F1 RID: 29169 RVA: 0x002517B8 File Offset: 0x0024F9B8
		public void OnPieceDestroy()
		{
			RopeSwingManager.Unregister(this);
		}

		// Token: 0x060071F2 RID: 29170 RVA: 0x002517C0 File Offset: 0x0024F9C0
		public void OnPiecePlacementDeserialized()
		{
			VectorizedCustomRopeSimulation.Unregister(this);
			base.transform.rotation = Quaternion.identity;
			this.scaleFactor = (base.transform.lossyScale.x + base.transform.lossyScale.y + base.transform.lossyScale.z) / 3f;
			this.SetIsIdle(true, true);
			VectorizedCustomRopeSimulation.Register(this);
			if (this.monkeBlockParent != null)
			{
				this.supportMovingAtRuntime = this.IsAttachedToMovingPiece();
			}
		}

		// Token: 0x060071F3 RID: 29171 RVA: 0x00251849 File Offset: 0x0024FA49
		public void OnPieceActivate()
		{
			if (this.monkeBlockParent != null)
			{
				this.supportMovingAtRuntime = this.IsAttachedToMovingPiece();
			}
		}

		// Token: 0x060071F4 RID: 29172 RVA: 0x00251868 File Offset: 0x0024FA68
		private bool IsAttachedToMovingPiece()
		{
			return this.monkeBlockParent.attachIndex >= 0 && this.monkeBlockParent.attachIndex < this.monkeBlockParent.gridPlanes.Count && this.monkeBlockParent.gridPlanes[this.monkeBlockParent.attachIndex].GetMovingParentGrid() != null;
		}

		// Token: 0x060071F5 RID: 29173 RVA: 0x002518C8 File Offset: 0x0024FAC8
		public void OnPieceDeactivate()
		{
			this.supportMovingAtRuntime = false;
		}

		// Token: 0x040082A0 RID: 33440
		public int ropeId;

		// Token: 0x040082A1 RID: 33441
		public string staticId;

		// Token: 0x040082A2 RID: 33442
		public bool useStaticId;

		// Token: 0x040082A3 RID: 33443
		protected float ropeBitGenOffset = 1f;

		// Token: 0x040082A4 RID: 33444
		[SerializeField]
		protected GameObject prefabRopeBit;

		// Token: 0x040082A5 RID: 33445
		[SerializeField]
		private bool supportMovingAtRuntime;

		// Token: 0x040082A6 RID: 33446
		public Transform[] nodes = Array.Empty<Transform>();

		// Token: 0x040082A7 RID: 33447
		private Dictionary<int, int> remotePlayers = new Dictionary<int, int>();

		// Token: 0x040082A8 RID: 33448
		[NonSerialized]
		public float lastGrabTime;

		// Token: 0x040082A9 RID: 33449
		[SerializeField]
		private AudioSource ropeCreakSFX;

		// Token: 0x040082AA RID: 33450
		public GorillaVelocityTracker velocityTracker;

		// Token: 0x040082AB RID: 33451
		private bool localPlayerOn;

		// Token: 0x040082AC RID: 33452
		private int localPlayerBoneIndex;

		// Token: 0x040082AD RID: 33453
		private XRNode localPlayerXRNode;

		// Token: 0x040082AE RID: 33454
		private const float MAX_VELOCITY_FOR_IDLE = 0.5f;

		// Token: 0x040082AF RID: 33455
		private const float TIME_FOR_IDLE = 2f;

		// Token: 0x040082B2 RID: 33458
		private float potentialIdleTimer;

		// Token: 0x040082B3 RID: 33459
		[SerializeField]
		protected int ropeLength = 8;

		// Token: 0x040082B4 RID: 33460
		[SerializeField]
		private GorillaRopeSwingSettings settings;

		// Token: 0x040082B5 RID: 33461
		private bool hasMonkeBlockParent;

		// Token: 0x040082B6 RID: 33462
		private BuilderPiece monkeBlockParent;

		// Token: 0x040082B7 RID: 33463
		[NonSerialized]
		public int ropeDataStartIndex;

		// Token: 0x040082B8 RID: 33464
		[NonSerialized]
		public int ropeDataIndexOffset;

		// Token: 0x040082B9 RID: 33465
		[SerializeField]
		private LayerMask wallLayerMask;

		// Token: 0x040082BA RID: 33466
		private RaycastHit[] nodeHits = new RaycastHit[1];

		// Token: 0x040082BB RID: 33467
		private float scaleFactor = 1f;

		// Token: 0x040082BC RID: 33468
		private bool started;

		// Token: 0x040082BD RID: 33469
		private int lastNodeCheckIndex = 2;
	}
}
