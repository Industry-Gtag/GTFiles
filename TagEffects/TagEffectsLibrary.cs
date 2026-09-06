using System;
using System.Collections;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x0200117B RID: 4475
	public class TagEffectsLibrary : MonoBehaviour
	{
		// Token: 0x17000AC5 RID: 2757
		// (get) Token: 0x06007036 RID: 28726 RVA: 0x002430D3 File Offset: 0x002412D3
		public static float FistBumpSpeedThreshold
		{
			get
			{
				return TagEffectsLibrary._instance.fistBumpSpeedThreshold;
			}
		}

		// Token: 0x17000AC6 RID: 2758
		// (get) Token: 0x06007037 RID: 28727 RVA: 0x002430DF File Offset: 0x002412DF
		public static float HighFiveSpeedThreshold
		{
			get
			{
				return TagEffectsLibrary._instance.highFiveSpeedThreshold;
			}
		}

		// Token: 0x17000AC7 RID: 2759
		// (get) Token: 0x06007038 RID: 28728 RVA: 0x002430EB File Offset: 0x002412EB
		public static bool DebugMode
		{
			get
			{
				return TagEffectsLibrary._instance.debugMode;
			}
		}

		// Token: 0x06007039 RID: 28729 RVA: 0x002430F7 File Offset: 0x002412F7
		private void Awake()
		{
			if (TagEffectsLibrary._instance != null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			TagEffectsLibrary._instance = this;
			this.tagEffectsPool = new Dictionary<string, Queue<GameObjectOnDisableDispatcher>>();
			this.tagEffectsComboLookUp = new Dictionary<TagEffectsCombo, TagEffectPack[]>();
		}

		// Token: 0x0600703A RID: 28730 RVA: 0x00243130 File Offset: 0x00241330
		public static void PlayEffect(Transform target, bool isLeftHand, float rigScale, TagEffectsLibrary.EffectType effectType, TagEffectPack playerCosmeticTagEffectPack, TagEffectPack otherPlayerCosmeticTagEffectPack, Quaternion rotation)
		{
			if (TagEffectsLibrary._instance == null)
			{
				return;
			}
			ModeTagEffect modeTagEffect = null;
			TagEffectPack tagEffectPack = null;
			GameModeType gameModeType = ((GameMode.ActiveGameMode != null) ? GameMode.ActiveGameMode.GameType() : GameModeType.Casual);
			for (int i = 0; i < TagEffectsLibrary._instance.defaultTagEffects.Length; i++)
			{
				if (TagEffectsLibrary._instance.defaultTagEffects[i] != null && TagEffectsLibrary._instance.defaultTagEffects[i].Modes.Contains(gameModeType))
				{
					modeTagEffect = TagEffectsLibrary._instance.defaultTagEffects[i];
					tagEffectPack = modeTagEffect.tagEffect;
					break;
				}
			}
			if (tagEffectPack == null)
			{
				return;
			}
			GameObject gameObject = tagEffectPack.firstPerson;
			GameObject gameObject2 = tagEffectPack.thirdPerson;
			GameObject gameObject3 = tagEffectPack.fistBump;
			GameObject gameObject4 = tagEffectPack.highFive;
			bool flag = tagEffectPack.firstPersonParentEffect;
			bool flag2 = tagEffectPack.thirdPersonParentEffect;
			bool flag3 = tagEffectPack.fistBumpParentEffect;
			bool flag4 = tagEffectPack.highFiveParentEffect;
			if (playerCosmeticTagEffectPack != null)
			{
				TagEffectPack tagEffectPack2 = TagEffectsLibrary.comboLookup(playerCosmeticTagEffectPack, otherPlayerCosmeticTagEffectPack);
				if (!modeTagEffect.blockFistBumpOverride && playerCosmeticTagEffectPack.fistBump != null)
				{
					gameObject3 = tagEffectPack2.fistBump;
					flag3 = tagEffectPack2.firstPersonParentEffect;
				}
				if (!modeTagEffect.blockHiveFiveOverride && playerCosmeticTagEffectPack.highFive != null)
				{
					gameObject4 = tagEffectPack2.highFive;
					flag4 = tagEffectPack2.highFiveParentEffect;
				}
			}
			if (otherPlayerCosmeticTagEffectPack != null)
			{
				if (!modeTagEffect.blockTagOverride && otherPlayerCosmeticTagEffectPack.firstPerson != null)
				{
					gameObject = otherPlayerCosmeticTagEffectPack.firstPerson;
					flag = otherPlayerCosmeticTagEffectPack.firstPersonParentEffect;
				}
				if (!modeTagEffect.blockTagOverride && otherPlayerCosmeticTagEffectPack.thirdPerson != null)
				{
					gameObject2 = otherPlayerCosmeticTagEffectPack.thirdPerson;
					flag2 = otherPlayerCosmeticTagEffectPack.thirdPersonParentEffect;
				}
			}
			switch (effectType)
			{
			case TagEffectsLibrary.EffectType.FIRST_PERSON:
				TagEffectsLibrary.placeEffects(gameObject, target, flag ? 1f : rigScale, false, flag, rotation);
				return;
			case TagEffectsLibrary.EffectType.THIRD_PERSON:
				TagEffectsLibrary.placeEffects(gameObject2, target, flag2 ? 1f : rigScale, false, flag2, rotation);
				return;
			case TagEffectsLibrary.EffectType.HIGH_FIVE:
				TagEffectsLibrary.placeEffects(gameObject4, target, flag4 ? 1f : rigScale, isLeftHand, flag4, rotation);
				return;
			case TagEffectsLibrary.EffectType.FIST_BUMP:
				TagEffectsLibrary.placeEffects(gameObject3, target, flag3 ? 1f : rigScale, isLeftHand, flag3, rotation);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600703B RID: 28731 RVA: 0x00243350 File Offset: 0x00241550
		private static TagEffectPack comboLookup(TagEffectPack playerCosmeticTagEffectPack, TagEffectPack otherPlayerCosmeticTagEffectPack)
		{
			if (otherPlayerCosmeticTagEffectPack == null)
			{
				return playerCosmeticTagEffectPack;
			}
			TagEffectsCombo tagEffectsCombo = new TagEffectsCombo();
			tagEffectsCombo.inputA = playerCosmeticTagEffectPack;
			tagEffectsCombo.inputB = otherPlayerCosmeticTagEffectPack;
			TagEffectPack[] array;
			if (!TagEffectsLibrary._instance.tagEffectsComboLookUp.TryGetValue(tagEffectsCombo, out array))
			{
				return playerCosmeticTagEffectPack;
			}
			int num = 0;
			if (GorillaComputer.instance != null)
			{
				num = GorillaComputer.instance.GetServerTime().Second;
			}
			return array[num % array.Length];
		}

		// Token: 0x0600703C RID: 28732 RVA: 0x002433C0 File Offset: 0x002415C0
		public static void placeEffects(GameObject prefab, Transform target, float scale, bool flipZAxis, bool parentEffect, Quaternion rotation)
		{
			if (prefab == null)
			{
				return;
			}
			Queue<GameObjectOnDisableDispatcher> queue;
			if (!TagEffectsLibrary._instance.tagEffectsPool.TryGetValue(prefab.name, out queue))
			{
				queue = new Queue<GameObjectOnDisableDispatcher>();
				TagEffectsLibrary._instance.tagEffectsPool.Add(prefab.name, queue);
			}
			if (queue.Count == 0 || (queue.Peek().gameObject.activeInHierarchy && queue.Count < 12))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(prefab, target.transform.position, rotation, parentEffect ? target : TagEffectsLibrary._instance.transform);
				gameObject.name = prefab.name;
				gameObject.transform.localScale = (flipZAxis ? new Vector3(scale, scale, -scale) : (Vector3.one * scale));
				GameObjectOnDisableDispatcher gameObjectOnDisableDispatcher;
				if (!gameObject.TryGetComponent<GameObjectOnDisableDispatcher>(out gameObjectOnDisableDispatcher))
				{
					gameObjectOnDisableDispatcher = gameObject.AddComponent<GameObjectOnDisableDispatcher>();
				}
				gameObjectOnDisableDispatcher.OnDisabled += TagEffectsLibrary.NewGameObjectOnDisableDispatcher_OnDisabled;
				gameObject.SetActive(true);
				queue.Enqueue(gameObjectOnDisableDispatcher);
				return;
			}
			GameObjectOnDisableDispatcher gameObjectOnDisableDispatcher2 = queue.Dequeue();
			TagEffectsLibrary._instance.StartCoroutine(TagEffectsLibrary._instance.RecycleGameObject(gameObjectOnDisableDispatcher2, target, scale, flipZAxis, parentEffect));
		}

		// Token: 0x0600703D RID: 28733 RVA: 0x002434DF File Offset: 0x002416DF
		private static void NewGameObjectOnDisableDispatcher_OnDisabled(GameObjectOnDisableDispatcher goodd)
		{
			TagEffectsLibrary._instance.StartCoroutine(TagEffectsLibrary._instance.ReclaimDisabled(goodd.transform));
		}

		// Token: 0x0600703E RID: 28734 RVA: 0x002434FC File Offset: 0x002416FC
		private IEnumerator RecycleGameObject(GameObjectOnDisableDispatcher recycledGameObject, Transform target, float scale, bool flipZAxis, bool parentEffect)
		{
			if (recycledGameObject.gameObject.activeInHierarchy)
			{
				recycledGameObject.gameObject.SetActive(false);
				recycledGameObject.OnDisabled -= TagEffectsLibrary.NewGameObjectOnDisableDispatcher_OnDisabled;
				yield return null;
			}
			recycledGameObject.transform.position = target.transform.position;
			recycledGameObject.transform.rotation = target.transform.rotation;
			recycledGameObject.transform.localScale = (flipZAxis ? new Vector3(scale, scale, -scale) : (Vector3.one * scale));
			recycledGameObject.transform.parent = (parentEffect ? target : TagEffectsLibrary._instance.transform);
			Queue<GameObjectOnDisableDispatcher> queue;
			if (TagEffectsLibrary._instance.tagEffectsPool.TryGetValue(recycledGameObject.gameObject.name, out queue))
			{
				recycledGameObject.gameObject.SetActive(true);
				queue.Enqueue(recycledGameObject);
			}
			yield break;
		}

		// Token: 0x0600703F RID: 28735 RVA: 0x00243529 File Offset: 0x00241729
		private IEnumerator ReclaimDisabled(Transform transform)
		{
			yield return null;
			transform.parent = TagEffectsLibrary._instance.transform;
			yield break;
		}

		// Token: 0x04008031 RID: 32817
		private const int OBJECT_QUEUE_LIMIT = 12;

		// Token: 0x04008032 RID: 32818
		[OnEnterPlay_SetNull]
		private static TagEffectsLibrary _instance;

		// Token: 0x04008033 RID: 32819
		[SerializeField]
		private float fistBumpSpeedThreshold = 1f;

		// Token: 0x04008034 RID: 32820
		[SerializeField]
		private float highFiveSpeedThreshold = 1f;

		// Token: 0x04008035 RID: 32821
		[SerializeField]
		private ModeTagEffect[] defaultTagEffects;

		// Token: 0x04008036 RID: 32822
		[SerializeField]
		private TagEffectsComboResult[] tagEffectsCombos;

		// Token: 0x04008037 RID: 32823
		[SerializeField]
		private bool debugMode;

		// Token: 0x04008038 RID: 32824
		private Dictionary<string, Queue<GameObjectOnDisableDispatcher>> tagEffectsPool;

		// Token: 0x04008039 RID: 32825
		private Dictionary<TagEffectsCombo, TagEffectPack[]> tagEffectsComboLookUp;

		// Token: 0x0200117C RID: 4476
		public enum EffectType
		{
			// Token: 0x0400803B RID: 32827
			FIRST_PERSON,
			// Token: 0x0400803C RID: 32828
			THIRD_PERSON,
			// Token: 0x0400803D RID: 32829
			HIGH_FIVE,
			// Token: 0x0400803E RID: 32830
			FIST_BUMP
		}
	}
}
