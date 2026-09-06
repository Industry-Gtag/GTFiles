using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001329 RID: 4905
	public class EnvironmentProximityReactor : MonoBehaviour
	{
		// Token: 0x06007B1A RID: 31514 RVA: 0x0028329D File Offset: 0x0028149D
		private void OnEnable()
		{
			if (!this.useStaticId)
			{
				this.CalculateId(false);
			}
			EnvironmentProximityReactorManager.Register(this);
		}

		// Token: 0x06007B1B RID: 31515 RVA: 0x002832B4 File Offset: 0x002814B4
		private void OnDisable()
		{
			EnvironmentProximityReactorManager.Unregister(this);
			this.ResetBlockState();
		}

		// Token: 0x06007B1C RID: 31516 RVA: 0x002832C4 File Offset: 0x002814C4
		private void Update()
		{
			if (CosmeticsProximityReactorManager.Instance == null)
			{
				return;
			}
			NetworkSystem instance = NetworkSystem.Instance;
			int? num;
			if (instance == null)
			{
				num = null;
			}
			else
			{
				NetPlayer localPlayer = instance.LocalPlayer;
				num = ((localPlayer != null) ? new int?(localPlayer.ActorNumber) : null);
			}
			int num2 = num ?? (-1);
			IReadOnlyList<CosmeticsProximityReactor> cosmetics = CosmeticsProximityReactorManager.Instance.Cosmetics;
			float time = Time.time;
			for (int i = 0; i < this.blocks.Count; i++)
			{
				EnvironmentProximityReactor.InteractionBlock interactionBlock = this.blocks[i];
				bool flag = false;
				Vector3 vector = base.transform.position;
				for (int j = 0; j < cosmetics.Count; j++)
				{
					CosmeticsProximityReactor cosmeticsProximityReactor = cosmetics[j];
					if (!(cosmeticsProximityReactor == null))
					{
						VRRig ownerRig = cosmeticsProximityReactor.GetOwnerRig();
						Vector3 vector2;
						if (!(ownerRig == null) && ownerRig.isLocal && interactionBlock.CanTriggerFrom(cosmeticsProximityReactor) && this.AreWithinThreshold(cosmeticsProximityReactor, interactionBlock.proximityThreshold, out vector2))
						{
							flag = true;
							vector = vector2;
							break;
						}
					}
				}
				if (flag)
				{
					if (!interactionBlock.wasBelow && interactionBlock.CanPlay(time))
					{
						interactionBlock.wasBelow = true;
						interactionBlock.lastTriggerTime = time;
						UnityEvent<Vector3> onBelowLocal = interactionBlock.onBelowLocal;
						if (onBelowLocal != null)
						{
							onBelowLocal.Invoke(vector);
						}
						if (interactionBlock.activeSharedActors.Count == 0)
						{
							UnityEvent<Vector3> onBelowShared = interactionBlock.onBelowShared;
							if (onBelowShared != null)
							{
								onBelowShared.Invoke(vector);
							}
						}
						interactionBlock.localActorInSet = num2;
						interactionBlock.activeSharedActors.Add(num2);
						EnvironmentProximityReactorManager instance2 = EnvironmentProximityReactorManager.Instance;
						if (instance2 != null)
						{
							instance2.BroadcastProximityState(this.reactorId, i, true);
						}
					}
					else if (interactionBlock.wasBelow)
					{
						UnityEvent<Vector3> whileBelowLocal = interactionBlock.whileBelowLocal;
						if (whileBelowLocal != null)
						{
							whileBelowLocal.Invoke(vector);
						}
						UnityEvent<Vector3> whileBelowShared = interactionBlock.whileBelowShared;
						if (whileBelowShared != null)
						{
							whileBelowShared.Invoke(vector);
						}
					}
				}
				else if (interactionBlock.wasBelow)
				{
					interactionBlock.wasBelow = false;
					UnityEvent onAboveLocal = interactionBlock.onAboveLocal;
					if (onAboveLocal != null)
					{
						onAboveLocal.Invoke();
					}
					interactionBlock.activeSharedActors.Remove(interactionBlock.localActorInSet);
					if (interactionBlock.activeSharedActors.Count == 0)
					{
						UnityEvent onAboveShared = interactionBlock.onAboveShared;
						if (onAboveShared != null)
						{
							onAboveShared.Invoke();
						}
					}
					EnvironmentProximityReactorManager instance3 = EnvironmentProximityReactorManager.Instance;
					if (instance3 != null)
					{
						instance3.BroadcastProximityState(this.reactorId, i, false);
					}
				}
				if (interactionBlock.activeSharedActors.Count > 0 && !interactionBlock.wasBelow)
				{
					UnityEvent<Vector3> whileBelowShared2 = interactionBlock.whileBelowShared;
					if (whileBelowShared2 != null)
					{
						whileBelowShared2.Invoke(base.transform.position);
					}
				}
			}
		}

		// Token: 0x06007B1D RID: 31517 RVA: 0x0028355C File Offset: 0x0028175C
		public void SyncStateTo(NetPlayer newPlayer, EnvironmentProximityReactorManager manager)
		{
			for (int i = 0; i < this.blocks.Count; i++)
			{
				if (this.blocks[i].wasBelow)
				{
					manager.BroadcastProximityStateTo(newPlayer, this.reactorId, i, true);
				}
			}
		}

		// Token: 0x06007B1E RID: 31518 RVA: 0x002835A4 File Offset: 0x002817A4
		public void ClearRemoteActors()
		{
			foreach (EnvironmentProximityReactor.InteractionBlock interactionBlock in this.blocks)
			{
				bool flag = interactionBlock.wasBelow && interactionBlock.activeSharedActors.Contains(interactionBlock.localActorInSet);
				bool flag2 = interactionBlock.activeSharedActors.Count > 0;
				interactionBlock.activeSharedActors.Clear();
				if (flag)
				{
					interactionBlock.activeSharedActors.Add(interactionBlock.localActorInSet);
				}
				else if (flag2)
				{
					UnityEvent onAboveShared = interactionBlock.onAboveShared;
					if (onAboveShared != null)
					{
						onAboveShared.Invoke();
					}
				}
			}
		}

		// Token: 0x06007B1F RID: 31519 RVA: 0x00283650 File Offset: 0x00281850
		public void RemoveSharedActor(int actorNumber)
		{
			for (int i = 0; i < this.blocks.Count; i++)
			{
				EnvironmentProximityReactor.InteractionBlock interactionBlock = this.blocks[i];
				if (interactionBlock.activeSharedActors.Remove(actorNumber) && interactionBlock.activeSharedActors.Count == 0)
				{
					UnityEvent onAboveShared = interactionBlock.onAboveShared;
					if (onAboveShared != null)
					{
						onAboveShared.Invoke();
					}
				}
			}
		}

		// Token: 0x06007B20 RID: 31520 RVA: 0x002836AC File Offset: 0x002818AC
		public void ApplySharedProximity(int blockIndex, bool isBelow, int senderActorNumber)
		{
			if (blockIndex < 0 || blockIndex >= this.blocks.Count)
			{
				return;
			}
			EnvironmentProximityReactor.InteractionBlock interactionBlock = this.blocks[blockIndex];
			if (isBelow)
			{
				bool flag = interactionBlock.activeSharedActors.Count == 0;
				interactionBlock.activeSharedActors.Add(senderActorNumber);
				if (flag)
				{
					UnityEvent<Vector3> onBelowShared = interactionBlock.onBelowShared;
					if (onBelowShared == null)
					{
						return;
					}
					onBelowShared.Invoke(base.transform.position);
					return;
				}
			}
			else if (interactionBlock.activeSharedActors.Remove(senderActorNumber) && interactionBlock.activeSharedActors.Count == 0)
			{
				UnityEvent onAboveShared = interactionBlock.onAboveShared;
				if (onAboveShared == null)
				{
					return;
				}
				onAboveShared.Invoke();
			}
		}

		// Token: 0x06007B21 RID: 31521 RVA: 0x00283740 File Offset: 0x00281940
		private bool AreWithinThreshold(CosmeticsProximityReactor cosmetic, float threshold, out Vector3 contactPoint)
		{
			Vector3 vector = ((cosmetic.collider == null) ? cosmetic.transform.position : cosmetic.collider.ClosestPoint(base.transform.position));
			Vector3 vector2 = ((this.proximityCollider == null) ? base.transform.position : this.proximityCollider.ClosestPoint(vector));
			contactPoint = (vector + vector2) * 0.5f;
			return Vector3.Distance(vector, vector2) <= threshold;
		}

		// Token: 0x06007B22 RID: 31522 RVA: 0x002837CC File Offset: 0x002819CC
		private void CalculateId(bool force = false)
		{
			Transform transform = base.transform;
			int hashCode = TransformUtils.ComputePathHash(transform).ToId128().GetHashCode();
			int staticHash = base.GetType().Name.GetStaticHash();
			int hashCode2 = transform.position.QuantizedId128().GetHashCode();
			int num = StaticHash.Compute(hashCode, staticHash, hashCode2);
			if (this.useStaticId)
			{
				if (string.IsNullOrEmpty(this.staticId) || force)
				{
					int instanceID = transform.GetInstanceID();
					int num2 = StaticHash.Compute(num, instanceID);
					this.staticId = string.Format("#ID_{0:X8}", num2);
				}
				this.reactorId = this.staticId.GetStaticHash();
				return;
			}
			this.reactorId = (Application.isPlaying ? num : 0);
		}

		// Token: 0x06007B23 RID: 31523 RVA: 0x00283898 File Offset: 0x00281A98
		private void ResetBlockState()
		{
			foreach (EnvironmentProximityReactor.InteractionBlock interactionBlock in this.blocks)
			{
				interactionBlock.wasBelow = false;
				interactionBlock.activeSharedActors.Clear();
				interactionBlock.localActorInSet = int.MinValue;
				interactionBlock.lastTriggerTime = -9999f;
			}
		}

		// Token: 0x06007B24 RID: 31524 RVA: 0x0028390C File Offset: 0x00281B0C
		private void EdRecalculateId()
		{
			this.CalculateId(true);
		}

		// Token: 0x04008CF9 RID: 36089
		public List<EnvironmentProximityReactor.InteractionBlock> blocks = new List<EnvironmentProximityReactor.InteractionBlock>();

		// Token: 0x04008CFA RID: 36090
		[Tooltip("Optional collider for precise proximity measurement. If unassigned, the transform position is used.")]
		public Collider proximityCollider;

		// Token: 0x04008CFB RID: 36091
		public int reactorId;

		// Token: 0x04008CFC RID: 36092
		public string staticId;

		// Token: 0x04008CFD RID: 36093
		[Tooltip("Leave off for most objects- the ID is computed automatically from the hierarchy path, type name, and world position, so no manual setup is needed.\n\nEnable only if this object is expected to move or be renamed in the editor after the ID has already been referenced elsewhere When enabled, the ID is pinned to the Static ID string above so it stays stable across repositions. Hit Recalculate once to generate it, then leave it alone.")]
		public bool useStaticId;

		// Token: 0x0200132A RID: 4906
		[Serializable]
		public class InteractionBlock
		{
			// Token: 0x06007B26 RID: 31526 RVA: 0x00283928 File Offset: 0x00281B28
			public bool CanTriggerFrom(CosmeticsProximityReactor cosmetic)
			{
				foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in cosmetic.blocks)
				{
					if ((interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic || interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToEnvironment) && interactionSetting.interactionKeys != null && interactionSetting.interactionKeys.Count != 0)
					{
						if (this.ignoreKeys != null && this.ignoreKeys.Count > 0)
						{
							bool flag = false;
							foreach (string text in interactionSetting.interactionKeys)
							{
								if (!string.IsNullOrEmpty(text) && this.ignoreKeys.Contains(text))
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								continue;
							}
						}
						foreach (string text2 in interactionSetting.interactionKeys)
						{
							if (!string.IsNullOrEmpty(text2))
							{
								if (this.interactionKeys != null && this.interactionKeys.Contains(text2))
								{
									return true;
								}
								if (this.listenerKeys != null && this.listenerKeys.Contains(text2))
								{
									return true;
								}
							}
						}
					}
				}
				return false;
			}

			// Token: 0x06007B27 RID: 31527 RVA: 0x00283AC4 File Offset: 0x00281CC4
			public bool CanPlay(float now)
			{
				return now - this.lastTriggerTime >= this.cooldownTime;
			}

			// Token: 0x04008CFE RID: 36094
			[Tooltip("Keys this block broadcasts. Cosmetics whose Key List or Listener List contains a matching key can trigger this block.")]
			public List<string> interactionKeys = new List<string>();

			// Token: 0x04008CFF RID: 36095
			[Tooltip("If the cosmetic broadcasts any of these keys this block will not fire, even if another key matches.")]
			public List<string> ignoreKeys = new List<string>();

			// Token: 0x04008D00 RID: 36096
			[Tooltip("React when a cosmetic broadcasts one of these keys. Listener keys are never broadcast outward, so two Listener-only objects will never trigger each other.")]
			public List<string> listenerKeys = new List<string>();

			// Token: 0x04008D01 RID: 36097
			[Tooltip("Distance (m) at which a cosmetic triggers this block.")]
			public float proximityThreshold = 0.3f;

			// Token: 0x04008D02 RID: 36098
			[Tooltip("Minimum seconds between consecutive OnBelow triggers for this block.")]
			[SerializeField]
			private float cooldownTime = 0.5f;

			// Token: 0x04008D03 RID: 36099
			[Tooltip("Fires immediately on the client whose cosmetic crossed below the threshold. Local-only")]
			public UnityEvent<Vector3> onBelowLocal;

			// Token: 0x04008D04 RID: 36100
			[Tooltip("Fires on aLL clients when any player's cosmetic crosses below the threshold.")]
			public UnityEvent<Vector3> onBelowShared;

			// Token: 0x04008D05 RID: 36101
			[Tooltip("Fires every frame on the triggering client while the cosmetic remains below the threshold. Local-only")]
			public UnityEvent<Vector3> whileBelowLocal;

			// Token: 0x04008D06 RID: 36102
			[Tooltip("Fires every frame on ALL clients while any player's cosmetic remains below the threshold.")]
			public UnityEvent<Vector3> whileBelowShared;

			// Token: 0x04008D07 RID: 36103
			[Tooltip("Fires on the triggering client when the cosmetic goes back above the threshold.")]
			public UnityEvent onAboveLocal;

			// Token: 0x04008D08 RID: 36104
			[Tooltip("Fires on aLL clients when the cosmetic goes back above the threshold.")]
			public UnityEvent onAboveShared;

			// Token: 0x04008D09 RID: 36105
			[NonSerialized]
			public bool wasBelow;

			// Token: 0x04008D0A RID: 36106
			[NonSerialized]
			public readonly HashSet<int> activeSharedActors = new HashSet<int>();

			// Token: 0x04008D0B RID: 36107
			[NonSerialized]
			public int localActorInSet = int.MinValue;

			// Token: 0x04008D0C RID: 36108
			[NonSerialized]
			public float lastTriggerTime = -9999f;
		}
	}
}
