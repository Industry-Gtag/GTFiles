using System;
using System.Collections.Generic;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001318 RID: 4888
	public class CosmeticsProximityReactor : MonoBehaviour, ISpawnable
	{
		// Token: 0x17000C00 RID: 3072
		// (get) Token: 0x06007AA6 RID: 31398 RVA: 0x0027FFC4 File Offset: 0x0027E1C4
		// (set) Token: 0x06007AA7 RID: 31399 RVA: 0x0027FFCC File Offset: 0x0027E1CC
		public bool IsMatched { get; set; }

		// Token: 0x17000C01 RID: 3073
		// (get) Token: 0x06007AA8 RID: 31400 RVA: 0x0027FFD5 File Offset: 0x0027E1D5
		// (set) Token: 0x06007AA9 RID: 31401 RVA: 0x0027FFDD File Offset: 0x0027E1DD
		private VRRig MyRig { get; set; }

		// Token: 0x06007AAA RID: 31402 RVA: 0x0027FFE6 File Offset: 0x0027E1E6
		public VRRig GetOwnerRig()
		{
			return this.MyRig;
		}

		// Token: 0x17000C02 RID: 3074
		// (get) Token: 0x06007AAB RID: 31403 RVA: 0x0027FFEE File Offset: 0x0027E1EE
		// (set) Token: 0x06007AAC RID: 31404 RVA: 0x0027FFF6 File Offset: 0x0027E1F6
		public bool IsSpawned { get; set; }

		// Token: 0x17000C03 RID: 3075
		// (get) Token: 0x06007AAD RID: 31405 RVA: 0x0027FFFF File Offset: 0x0027E1FF
		// (set) Token: 0x06007AAE RID: 31406 RVA: 0x00280007 File Offset: 0x0027E207
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x17000C04 RID: 3076
		// (get) Token: 0x06007AAF RID: 31407 RVA: 0x00280010 File Offset: 0x0027E210
		public bool IsBelow
		{
			get
			{
				using (List<CosmeticsProximityReactor.InteractionSetting>.Enumerator enumerator = this.blocks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.wasBelow)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x06007AB0 RID: 31408 RVA: 0x0028006C File Offset: 0x0027E26C
		public void OnSpawn(VRRig rig)
		{
			if (this.MyRig == null)
			{
				this.MyRig = rig;
			}
		}

		// Token: 0x06007AB1 RID: 31409 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnDespawn()
		{
		}

		// Token: 0x06007AB2 RID: 31410 RVA: 0x00280083 File Offset: 0x0027E283
		private void Start()
		{
			this.IsMatched = false;
			if (CosmeticsProximityReactorManager.Instance != null)
			{
				CosmeticsProximityReactorManager.Instance.Register(this);
			}
		}

		// Token: 0x06007AB3 RID: 31411 RVA: 0x002800A4 File Offset: 0x0027E2A4
		private void OnEnable()
		{
			if (this.MyRig == null)
			{
				this.MyRig = base.GetComponentInParent<VRRig>();
			}
			if (CosmeticsProximityReactorManager.Instance != null)
			{
				CosmeticsProximityReactorManager.Instance.Register(this);
			}
		}

		// Token: 0x06007AB4 RID: 31412 RVA: 0x002800D8 File Offset: 0x0027E2D8
		private void OnDisable()
		{
			if (CosmeticsProximityReactorManager.Instance)
			{
				CosmeticsProximityReactorManager.Instance.Unregister(this);
			}
		}

		// Token: 0x06007AB5 RID: 31413 RVA: 0x002800F4 File Offset: 0x0027E2F4
		public IReadOnlyList<string> GetTypes()
		{
			List<string> sharedKeysCache = CosmeticsProximityReactorManager.SharedKeysCache;
			sharedKeysCache.Clear();
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic)
				{
					if (interactionSetting.interactionKeys != null)
					{
						foreach (string text in interactionSetting.interactionKeys)
						{
							if (!string.IsNullOrEmpty(text) && !sharedKeysCache.Contains(text))
							{
								sharedKeysCache.Add(text);
							}
						}
					}
					if (interactionSetting.listenerKeys != null)
					{
						foreach (string text2 in interactionSetting.listenerKeys)
						{
							if (!string.IsNullOrEmpty(text2) && !sharedKeysCache.Contains(text2))
							{
								sharedKeysCache.Add(text2);
							}
						}
					}
				}
			}
			return sharedKeysCache;
		}

		// Token: 0x06007AB6 RID: 31414 RVA: 0x0028021C File Offset: 0x0027E41C
		public bool IsGorillaBody()
		{
			return this.itemKind == CosmeticsProximityReactor.ItemKind.GorillaBody;
		}

		// Token: 0x06007AB7 RID: 31415 RVA: 0x00280227 File Offset: 0x0027E427
		public bool IsCosmeticItem()
		{
			return this.itemKind == CosmeticsProximityReactor.ItemKind.Cosmetic;
		}

		// Token: 0x06007AB8 RID: 31416 RVA: 0x00280234 File Offset: 0x0027E434
		public bool AcceptsAnySource()
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.gorillaBodyMask != CosmeticsProximityReactor.GorillaBodyPart.None)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007AB9 RID: 31417 RVA: 0x00280298 File Offset: 0x0027E498
		public bool AcceptsThisSource(CosmeticsProximityReactor.GorillaBodyPart kind)
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.AcceptsGorillaBodyPart(kind))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007ABA RID: 31418 RVA: 0x00280300 File Offset: 0x0027E500
		public float GetCosmeticPairThresholdWith(CosmeticsProximityReactor other, out bool any)
		{
			any = false;
			float num = float.MaxValue;
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting.AllowsRig(this.MyRig, other.MyRig))
				{
					foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting2 in other.blocks)
					{
						if (interactionSetting2.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting2.AllowsRig(other.MyRig, this.MyRig) && interactionSetting.CanTriggerFrom(interactionSetting2))
						{
							any = true;
							if (interactionSetting.proximityThreshold < num)
							{
								num = interactionSetting.proximityThreshold;
							}
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06007ABB RID: 31419 RVA: 0x002803F0 File Offset: 0x0027E5F0
		public float GetSourceThresholdFor(CosmeticsProximityReactor gorillaBody, out bool any)
		{
			any = false;
			float num = float.MaxValue;
			CosmeticsProximityReactor.GorillaBodyPart gorillaBodyPart = gorillaBody.gorillaBodyParts;
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.AcceptsGorillaBodyPart(gorillaBodyPart) && interactionSetting.AllowsRig(this.MyRig, gorillaBody.MyRig))
				{
					any = true;
					if (interactionSetting.proximityThreshold < num)
					{
						num = interactionSetting.proximityThreshold;
					}
				}
			}
			return num;
		}

		// Token: 0x06007ABC RID: 31420 RVA: 0x00280488 File Offset: 0x0027E688
		public void OnCosmeticBelowWith(CosmeticsProximityReactor other, Vector3 contact)
		{
			float time = Time.time;
			bool flag = false;
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting.AllowsRig(this.MyRig, other.MyRig))
				{
					bool flag2 = false;
					foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting2 in other.blocks)
					{
						if (interactionSetting2.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting2.AllowsRig(other.MyRig, this.MyRig) && interactionSetting.CanTriggerFrom(interactionSetting2))
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						interactionSetting.FireBelow(this.MyRig, contact, time);
						if (interactionSetting.wasBelow)
						{
							interactionSetting.isMatched = true;
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				this.IsMatched = true;
			}
		}

		// Token: 0x06007ABD RID: 31421 RVA: 0x002805A0 File Offset: 0x0027E7A0
		public void WhileCosmeticBelowWith(CosmeticsProximityReactor other, Vector3 contact)
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting.isMatched && interactionSetting.AllowsRig(this.MyRig, other.MyRig))
				{
					bool flag = false;
					foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting2 in other.blocks)
					{
						if (interactionSetting2.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting2.AllowsRig(other.MyRig, this.MyRig) && interactionSetting.CanTriggerFrom(interactionSetting2))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						interactionSetting.FireWhile(this.MyRig, contact);
					}
				}
			}
		}

		// Token: 0x06007ABE RID: 31422 RVA: 0x00280698 File Offset: 0x0027E898
		public void OnCosmeticAboveAll()
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting.isMatched)
				{
					interactionSetting.FireAbove(this.MyRig);
				}
			}
			this.RefreshAggregateMatched();
		}

		// Token: 0x06007ABF RID: 31423 RVA: 0x00280708 File Offset: 0x0027E908
		public void OnSourceBelow(Vector3 contact, CosmeticsProximityReactor.GorillaBodyPart kind, VRRig sourceRig)
		{
			float time = Time.time;
			bool flag = false;
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.AcceptsGorillaBodyPart(kind) && interactionSetting.AllowsRig(this.MyRig, sourceRig))
				{
					interactionSetting.FireBelow(this.MyRig, contact, time);
					if (interactionSetting.wasBelow)
					{
						interactionSetting.isMatched = true;
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.RefreshAggregateMatched();
			}
		}

		// Token: 0x06007AC0 RID: 31424 RVA: 0x002807A8 File Offset: 0x0027E9A8
		public void WhileSourceBelow(Vector3 contact, CosmeticsProximityReactor.GorillaBodyPart kind, VRRig sourceRig)
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.AcceptsGorillaBodyPart(kind) && interactionSetting.isMatched && interactionSetting.AllowsRig(this.MyRig, sourceRig))
				{
					interactionSetting.FireWhile(this.MyRig, contact);
				}
			}
		}

		// Token: 0x06007AC1 RID: 31425 RVA: 0x0028082C File Offset: 0x0027EA2C
		public void OnSourceAboveAll()
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.isMatched)
				{
					interactionSetting.FireAbove(this.MyRig);
				}
			}
			this.RefreshAggregateMatched();
		}

		// Token: 0x06007AC2 RID: 31426 RVA: 0x0028089C File Offset: 0x0027EA9C
		public bool HasAnyCosmeticMatch()
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting.isMatched)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007AC3 RID: 31427 RVA: 0x00280900 File Offset: 0x0027EB00
		private bool HasAnyGorillaBodyPartMatch()
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.isMatched)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007AC4 RID: 31428 RVA: 0x00280964 File Offset: 0x0027EB64
		public void RefreshAggregateMatched()
		{
			this.IsMatched = this.HasAnyCosmeticMatch() || this.HasAnyGorillaBodyPartMatch();
		}

		// Token: 0x04008C40 RID: 35904
		[Tooltip("Is this object a Cosmetic or a gorilla body part like hand? (gorilla body slot is reserved for Gorilla Player Networked)")]
		public CosmeticsProximityReactor.ItemKind itemKind;

		// Token: 0x04008C41 RID: 35905
		[FormerlySerializedAs("sourceKinds")]
		public CosmeticsProximityReactor.GorillaBodyPart gorillaBodyParts;

		// Token: 0x04008C42 RID: 35906
		public List<CosmeticsProximityReactor.InteractionSetting> blocks = new List<CosmeticsProximityReactor.InteractionSetting>();

		// Token: 0x04008C43 RID: 35907
		[Tooltip("If enabled, this cosmetic ignores other instances that share the same PlayFabID.")]
		public bool ignoreSameCosmeticInstances;

		// Token: 0x04008C44 RID: 35908
		public string PlayFabID = "";

		// Token: 0x04008C45 RID: 35909
		[Tooltip("If collider is not assigned, we will use the position of this object to find the distance between two cosmetic/body part")]
		public Collider collider;

		// Token: 0x04008C48 RID: 35912
		private RubberDuckEvents _events;

		// Token: 0x02001319 RID: 4889
		public enum ItemKind
		{
			// Token: 0x04008C4C RID: 35916
			Cosmetic,
			// Token: 0x04008C4D RID: 35917
			GorillaBody
		}

		// Token: 0x0200131A RID: 4890
		[Flags]
		public enum GorillaBodyPart
		{
			// Token: 0x04008C4F RID: 35919
			None = 0,
			// Token: 0x04008C50 RID: 35920
			HandLeft = 1,
			// Token: 0x04008C51 RID: 35921
			HandRight = 2,
			// Token: 0x04008C52 RID: 35922
			Mouth = 4
		}

		// Token: 0x0200131B RID: 4891
		public enum InteractionMode
		{
			// Token: 0x04008C54 RID: 35924
			CosmeticToCosmetic,
			// Token: 0x04008C55 RID: 35925
			CosmeticToEnvironment,
			// Token: 0x04008C56 RID: 35926
			GorillaBodyToCosmetic
		}

		// Token: 0x0200131C RID: 4892
		public enum TargetType
		{
			// Token: 0x04008C58 RID: 35928
			Owner,
			// Token: 0x04008C59 RID: 35929
			Others,
			// Token: 0x04008C5A RID: 35930
			All
		}

		// Token: 0x0200131D RID: 4893
		[Serializable]
		public class InteractionSetting
		{
			// Token: 0x06007AC6 RID: 31430 RVA: 0x0028099B File Offset: 0x0027EB9B
			public bool IsCosmeticToCosmetic()
			{
				return this.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic;
			}

			// Token: 0x06007AC7 RID: 31431 RVA: 0x002809A6 File Offset: 0x0027EBA6
			public bool IsCosmeticToEnvironment()
			{
				return this.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToEnvironment;
			}

			// Token: 0x06007AC8 RID: 31432 RVA: 0x002809B1 File Offset: 0x0027EBB1
			public bool IsGorillaBodyToCosmetic()
			{
				return this.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic;
			}

			// Token: 0x06007AC9 RID: 31433 RVA: 0x002809BC File Offset: 0x0027EBBC
			public bool AcceptsGorillaBodyPart(CosmeticsProximityReactor.GorillaBodyPart kind)
			{
				return this.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && (this.gorillaBodyMask & kind) > CosmeticsProximityReactor.GorillaBodyPart.None;
			}

			// Token: 0x06007ACA RID: 31434 RVA: 0x002809D4 File Offset: 0x0027EBD4
			public bool CanTriggerFrom(CosmeticsProximityReactor.InteractionSetting other)
			{
				if (this.mode != CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic || other == null || other.mode != CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic)
				{
					return false;
				}
				if (other.interactionKeys == null || other.interactionKeys.Count == 0)
				{
					return false;
				}
				if (this.ignoreKeys != null && this.ignoreKeys.Count > 0)
				{
					foreach (string text in other.interactionKeys)
					{
						if (!string.IsNullOrEmpty(text) && this.ignoreKeys.Contains(text))
						{
							return false;
						}
					}
				}
				foreach (string text2 in other.interactionKeys)
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
				return false;
			}

			// Token: 0x06007ACB RID: 31435 RVA: 0x00280AFC File Offset: 0x0027ECFC
			public bool CanPlay(float now)
			{
				return now - this.lastEffectTime >= this.cooldownTime;
			}

			// Token: 0x06007ACC RID: 31436 RVA: 0x00280B14 File Offset: 0x0027ED14
			public void FireBelow(VRRig rig, Vector3 contact, float now)
			{
				if (!this.wasBelow && this.CanPlay(now))
				{
					if (rig != null && rig.isLocal)
					{
						UnityEvent<Vector3> unityEvent = this.onBelowLocal;
						if (unityEvent != null)
						{
							unityEvent.Invoke(contact);
						}
					}
					UnityEvent<Vector3> unityEvent2 = this.onBelowShared;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke(contact);
					}
					this.wasBelow = true;
					this.lastEffectTime = now;
				}
			}

			// Token: 0x06007ACD RID: 31437 RVA: 0x00280B75 File Offset: 0x0027ED75
			public void FireWhile(VRRig rig, Vector3 contact)
			{
				if (rig != null && rig.isLocal)
				{
					UnityEvent<Vector3> unityEvent = this.whileBelowLocal;
					if (unityEvent != null)
					{
						unityEvent.Invoke(contact);
					}
				}
				UnityEvent<Vector3> unityEvent2 = this.whileBelowShared;
				if (unityEvent2 == null)
				{
					return;
				}
				unityEvent2.Invoke(contact);
			}

			// Token: 0x06007ACE RID: 31438 RVA: 0x00280BAC File Offset: 0x0027EDAC
			public void FireAbove(VRRig rig)
			{
				if (this.wasBelow)
				{
					if (rig != null && rig.isLocal)
					{
						UnityEvent unityEvent = this.onAboveLocal;
						if (unityEvent != null)
						{
							unityEvent.Invoke();
						}
					}
					UnityEvent unityEvent2 = this.onAboveShared;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke();
					}
					this.wasBelow = false;
					this.isMatched = false;
				}
			}

			// Token: 0x06007ACF RID: 31439 RVA: 0x00280C04 File Offset: 0x0027EE04
			public bool AllowsRig(VRRig myRig, VRRig otherRig)
			{
				if (myRig == null || otherRig == null)
				{
					return true;
				}
				switch (this.targetType)
				{
				case CosmeticsProximityReactor.TargetType.Owner:
					return myRig == otherRig;
				case CosmeticsProximityReactor.TargetType.Others:
					return myRig != otherRig;
				}
				return true;
			}

			// Token: 0x04008C5B RID: 35931
			[Tooltip("Determines what type of interaction this block handles.\n• CosmeticToCosmetic: triggers when two cosmetics with matching keys are nearby.\n• CosmeticToEnvironment: broadcasts keys that EnvironmentProximityReactor objects listen for. Use this to mark a cosmetic as a trigger for scene objects.\n• GorillaBodyToCosmetic: triggers when a Gorilla body part (hand, head, etc.) is near this cosmetic.")]
			public CosmeticsProximityReactor.InteractionMode mode;

			// Token: 0x04008C5C RID: 35932
			[Tooltip("Keys this block broadcasts. Other cosmetics or environment objects whose Key list or Listener list contain a matching key can react to this block.")]
			public List<string> interactionKeys = new List<string>();

			// Token: 0x04008C5D RID: 35933
			[Tooltip("If the other side is broadcasting any of these keys, this block will not fire, even if another key matches.")]
			public List<string> ignoreKeys = new List<string>();

			// Token: 0x04008C5E RID: 35934
			[Tooltip("Keys this block silently listens for. When the other side broadcasts one of these keys, this block fires. Listener keys are never broadcast outward, so two Listener-only objects will never trigger each other.")]
			public List<string> listenerKeys = new List<string>();

			// Token: 0x04008C5F RID: 35935
			[Tooltip("Specifies which Gorilla body parts (e.g., Hands, Head) can trigger this interaction.\nUse this when the Mode is set to GorillaBodyToCosmetic.")]
			public CosmeticsProximityReactor.GorillaBodyPart gorillaBodyMask;

			// Token: 0x04008C60 RID: 35936
			[Tooltip("The distance threshold (in meters) for triggering the interaction.\nIf another object enters this range, the OnBelow and WhileBelow events are fired.")]
			public float proximityThreshold = 0.15f;

			// Token: 0x04008C61 RID: 35937
			[Tooltip("Minimum time (in seconds) between consecutive triggers for this interaction block.\nPrevents rapid re-triggering when objects remain within proximity.")]
			[SerializeField]
			private float cooldownTime = 0.5f;

			// Token: 0x04008C62 RID: 35938
			[Tooltip("Who is allowed to trigger this block (if gorilla body part is selected).\n• Owner: only this cosmetic's own rig/body can trigger this.\n• Others: only other players' rigs/bodies can trigger this.\n• All: anyone can trigger.\n\nNote: everyone will still be able to see the result when it triggers.")]
			public CosmeticsProximityReactor.TargetType targetType = CosmeticsProximityReactor.TargetType.All;

			// Token: 0x04008C63 RID: 35939
			public UnityEvent<Vector3> onBelowLocal;

			// Token: 0x04008C64 RID: 35940
			public UnityEvent<Vector3> onBelowShared;

			// Token: 0x04008C65 RID: 35941
			public UnityEvent<Vector3> whileBelowLocal;

			// Token: 0x04008C66 RID: 35942
			public UnityEvent<Vector3> whileBelowShared;

			// Token: 0x04008C67 RID: 35943
			public UnityEvent onAboveLocal;

			// Token: 0x04008C68 RID: 35944
			public UnityEvent onAboveShared;

			// Token: 0x04008C69 RID: 35945
			[NonSerialized]
			public bool wasBelow;

			// Token: 0x04008C6A RID: 35946
			[NonSerialized]
			public bool isMatched;

			// Token: 0x04008C6B RID: 35947
			[NonSerialized]
			public float lastEffectTime = -9999f;
		}
	}
}
