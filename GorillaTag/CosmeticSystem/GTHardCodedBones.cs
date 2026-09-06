using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02001287 RID: 4743
	public static class GTHardCodedBones
	{
		// Token: 0x0600777D RID: 30589 RVA: 0x0026B7A5 File Offset: 0x002699A5
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void HandleRuntimeInitialize_OnBeforeSceneLoad()
		{
			VRRigCache.OnPostInitialize += GTHardCodedBones.HandleVRRigCache_OnPostInitialize;
		}

		// Token: 0x0600777E RID: 30590 RVA: 0x0026B7B8 File Offset: 0x002699B8
		private static void HandleVRRigCache_OnPostInitialize()
		{
			VRRigCache.OnPostInitialize -= GTHardCodedBones.HandleVRRigCache_OnPostInitialize;
			GTHardCodedBones.HandleVRRigCache_OnPostSpawnRig();
			VRRigCache.OnPostSpawnRig += GTHardCodedBones.HandleVRRigCache_OnPostSpawnRig;
		}

		// Token: 0x0600777F RID: 30591 RVA: 0x0026B7E1 File Offset: 0x002699E1
		private static void HandleVRRigCache_OnPostSpawnRig()
		{
			if (VRRigCache.isInitialized)
			{
				bool isQuitting = ApplicationQuittingState.IsQuitting;
			}
		}

		// Token: 0x06007780 RID: 30592 RVA: 0x0008409E File Offset: 0x0008229E
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetBoneIndex(GTHardCodedBones.EBone bone)
		{
			return (int)bone;
		}

		// Token: 0x06007781 RID: 30593 RVA: 0x0026B7F0 File Offset: 0x002699F0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetBoneIndex(string name)
		{
			for (int i = 0; i < GTHardCodedBones.kBoneNames.Length; i++)
			{
				if (GTHardCodedBones.kBoneNames[i] == name)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x06007782 RID: 30594 RVA: 0x0026B824 File Offset: 0x00269A24
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneIndexByName(string name, out int out_index)
		{
			for (int i = 0; i < GTHardCodedBones.kBoneNames.Length; i++)
			{
				if (GTHardCodedBones.kBoneNames[i] == name)
				{
					out_index = i;
					return true;
				}
			}
			out_index = 0;
			return false;
		}

		// Token: 0x06007783 RID: 30595 RVA: 0x0026B85B File Offset: 0x00269A5B
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GTHardCodedBones.EBone GetBone(string name)
		{
			return (GTHardCodedBones.EBone)GTHardCodedBones.GetBoneIndex(name);
		}

		// Token: 0x06007784 RID: 30596 RVA: 0x0026B864 File Offset: 0x00269A64
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneByName(string name, out GTHardCodedBones.EBone out_eBone)
		{
			int num;
			if (GTHardCodedBones.TryGetBoneIndexByName(name, out num))
			{
				out_eBone = (GTHardCodedBones.EBone)num;
				return true;
			}
			out_eBone = GTHardCodedBones.EBone.None;
			return false;
		}

		// Token: 0x06007785 RID: 30597 RVA: 0x0026B884 File Offset: 0x00269A84
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetBoneName(int boneIndex)
		{
			return GTHardCodedBones.kBoneNames[boneIndex];
		}

		// Token: 0x06007786 RID: 30598 RVA: 0x0026B88D File Offset: 0x00269A8D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneName(int boneIndex, out string out_name)
		{
			if (boneIndex >= 0 && boneIndex < GTHardCodedBones.kBoneNames.Length)
			{
				out_name = GTHardCodedBones.kBoneNames[boneIndex];
				return true;
			}
			out_name = "None";
			return false;
		}

		// Token: 0x06007787 RID: 30599 RVA: 0x0026B8B0 File Offset: 0x00269AB0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetBoneName(GTHardCodedBones.EBone bone)
		{
			return GTHardCodedBones.GetBoneName((int)bone);
		}

		// Token: 0x06007788 RID: 30600 RVA: 0x0026B8B8 File Offset: 0x00269AB8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneName(GTHardCodedBones.EBone bone, out string out_name)
		{
			return GTHardCodedBones.TryGetBoneName((int)bone, out out_name);
		}

		// Token: 0x06007789 RID: 30601 RVA: 0x0026B8C4 File Offset: 0x00269AC4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long GetBoneBitFlag(string name)
		{
			if (name == "None")
			{
				return 0L;
			}
			for (int i = 0; i < GTHardCodedBones.kBoneNames.Length; i++)
			{
				if (GTHardCodedBones.kBoneNames[i] == name)
				{
					return 1L << i - 1;
				}
			}
			return 0L;
		}

		// Token: 0x0600778A RID: 30602 RVA: 0x0026B90E File Offset: 0x00269B0E
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long GetBoneBitFlag(GTHardCodedBones.EBone bone)
		{
			if (bone == GTHardCodedBones.EBone.None)
			{
				return 0L;
			}
			return 1L << bone - GTHardCodedBones.EBone.rig;
		}

		// Token: 0x0600778B RID: 30603 RVA: 0x0026B91F File Offset: 0x00269B1F
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static EHandedness GetHandednessFromBone(GTHardCodedBones.EBone bone)
		{
			if ((GTHardCodedBones.GetBoneBitFlag(bone) & 1728432283058160L) != 0L)
			{
				return EHandedness.Left;
			}
			if ((GTHardCodedBones.GetBoneBitFlag(bone) & 1769114204897280L) == 0L)
			{
				return EHandedness.None;
			}
			return EHandedness.Right;
		}

		// Token: 0x0600778C RID: 30604 RVA: 0x0026B94C File Offset: 0x00269B4C
		public static bool TryGetBoneXforms(VRRig vrRig, out Transform[] outBoneXforms, out string outErrorMsg)
		{
			outErrorMsg = string.Empty;
			if (vrRig == null)
			{
				outErrorMsg = "The VRRig is null.";
				outBoneXforms = Array.Empty<Transform>();
				return false;
			}
			int instanceID = vrRig.GetInstanceID();
			if (GTHardCodedBones._gInstIds_To_boneXforms.TryGetValue(instanceID, out outBoneXforms))
			{
				return true;
			}
			if (!GTHardCodedBones.TryGetBoneXforms(vrRig.mainSkin, out outBoneXforms, out outErrorMsg))
			{
				return false;
			}
			VRRigAnchorOverrides componentInChildren = vrRig.GetComponentInChildren<VRRigAnchorOverrides>(true);
			BodyDockPositions componentInChildren2 = vrRig.GetComponentInChildren<BodyDockPositions>(true);
			outBoneXforms[46] = componentInChildren2.leftBackTransform;
			outBoneXforms[47] = componentInChildren2.rightBackTransform;
			outBoneXforms[42] = componentInChildren2.chestTransform;
			outBoneXforms[43] = componentInChildren.CurrentBadgeTransform;
			outBoneXforms[44] = componentInChildren.nameTransform;
			outBoneXforms[52] = componentInChildren.huntComputer;
			outBoneXforms[50] = componentInChildren.friendshipBraceletLeftAnchor;
			outBoneXforms[51] = componentInChildren.friendshipBraceletRightAnchor;
			GTHardCodedBones._gInstIds_To_boneXforms[instanceID] = outBoneXforms;
			return true;
		}

		// Token: 0x0600778D RID: 30605 RVA: 0x0026BA18 File Offset: 0x00269C18
		public static bool TryGetSlotAnchorXforms(VRRig vrRig, out Transform[] outSlotXforms, out string outErrorMsg)
		{
			outErrorMsg = string.Empty;
			if (vrRig == null)
			{
				outErrorMsg = "The VRRig is null.";
				outSlotXforms = Array.Empty<Transform>();
				return false;
			}
			int instanceID = vrRig.GetInstanceID();
			if (GTHardCodedBones._gInstIds_To_slotXforms.TryGetValue(instanceID, out outSlotXforms))
			{
				return true;
			}
			Transform[] array;
			if (!GTHardCodedBones.TryGetBoneXforms(vrRig.mainSkin, out array, out outErrorMsg))
			{
				return false;
			}
			outSlotXforms = new Transform[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				outSlotXforms[i] = array[i];
			}
			BodyDockPositions componentInChildren = vrRig.GetComponentInChildren<BodyDockPositions>(true);
			outSlotXforms[7] = componentInChildren.leftArmTransform;
			outSlotXforms[25] = componentInChildren.rightArmTransform;
			outSlotXforms[8] = componentInChildren.leftHandTransform;
			outSlotXforms[26] = componentInChildren.rightHandTransform;
			GTHardCodedBones._gInstIds_To_slotXforms[instanceID] = outSlotXforms;
			return true;
		}

		// Token: 0x0600778E RID: 30606 RVA: 0x0026BAD0 File Offset: 0x00269CD0
		public static bool TryGetBoneXforms(SkinnedMeshRenderer skinnedMeshRenderer, out Transform[] outBoneXforms, out string outErrorMsg)
		{
			outErrorMsg = string.Empty;
			if (skinnedMeshRenderer == null)
			{
				outErrorMsg = "The SkinnedMeshRenderer was null.";
				outBoneXforms = Array.Empty<Transform>();
				return false;
			}
			int instanceID = skinnedMeshRenderer.GetInstanceID();
			if (GTHardCodedBones._gInstIds_To_boneXforms.TryGetValue(instanceID, out outBoneXforms))
			{
				return true;
			}
			GTHardCodedBones._gMissingBonesReport.Clear();
			Transform[] bones = skinnedMeshRenderer.bones;
			for (int i = 0; i < bones.Length; i++)
			{
				if (bones[i] == null)
				{
					Debug.LogError(string.Format("this should never happen -- skinned mesh bone index {0} is null in component: ", i) + "\"" + skinnedMeshRenderer.GetComponentPath(int.MaxValue) + "\"", skinnedMeshRenderer);
				}
				else if (bones[i].parent == null)
				{
					Debug.LogError(string.Format("unexpected and unhandled scenario -- skinned mesh bone at index {0} has no parent in ", i) + "component: \"" + skinnedMeshRenderer.GetComponentPath(int.MaxValue) + "\"", skinnedMeshRenderer);
				}
				else
				{
					bones[i] = (bones[i].name.EndsWith("_new") ? bones[i].parent : bones[i]);
				}
			}
			outBoneXforms = new Transform[GTHardCodedBones.kBoneNames.Length];
			for (int j = 1; j < GTHardCodedBones.kBoneNames.Length; j++)
			{
				string text = GTHardCodedBones.kBoneNames[j];
				if (!(text == "None") && !text.EndsWith("_end") && !text.Contains("Anchor") && j != 1)
				{
					bool flag = false;
					foreach (Transform transform in bones)
					{
						if (!(transform == null) && !(transform.name != text))
						{
							outBoneXforms[j] = transform;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						GTHardCodedBones._gMissingBonesReport.Add(j);
					}
				}
			}
			for (int l = 1; l < GTHardCodedBones.kBoneNames.Length; l++)
			{
				string text2 = GTHardCodedBones.kBoneNames[l];
				if (text2.EndsWith("_end"))
				{
					string text3 = text2;
					int boneIndex = GTHardCodedBones.GetBoneIndex(text3.Substring(0, text3.Length - 4));
					if (boneIndex < 0)
					{
						GTHardCodedBones._gMissingBonesReport.Add(l);
					}
					else
					{
						Transform transform2 = outBoneXforms[boneIndex];
						if (transform2 == null)
						{
							GTHardCodedBones._gMissingBonesReport.Add(l);
						}
						else
						{
							Transform transform3 = transform2.Find(text2);
							if (transform3 == null)
							{
								GTHardCodedBones._gMissingBonesReport.Add(l);
							}
							else
							{
								outBoneXforms[l] = transform3;
							}
						}
					}
				}
			}
			Transform transform4 = outBoneXforms[2];
			if (transform4 != null && transform4.parent != null)
			{
				outBoneXforms[1] = transform4.parent.parent;
			}
			else
			{
				GTHardCodedBones._gMissingBonesReport.Add(1);
			}
			for (int m = 1; m < GTHardCodedBones.kBoneNames.Length; m++)
			{
				string text4 = GTHardCodedBones.kBoneNames[m];
				if (text4.Contains("Anchor"))
				{
					Transform transform5;
					if (transform4.TryFindByPath("/**/" + text4, out transform5, false))
					{
						outBoneXforms[m] = transform5;
					}
					else
					{
						GameObject gameObject = new GameObject(text4);
						gameObject.transform.SetParent(transform4, false);
						outBoneXforms[m] = gameObject.transform;
					}
				}
			}
			GTHardCodedBones._gInstIds_To_boneXforms[instanceID] = outBoneXforms;
			if (GTHardCodedBones._gMissingBonesReport.Count == 0)
			{
				return true;
			}
			string text5 = "The SkinnedMeshRenderer on \"" + skinnedMeshRenderer.name + "\" did not have these expected bones: ";
			foreach (int num in GTHardCodedBones._gMissingBonesReport)
			{
				text5 = text5 + "\n- " + GTHardCodedBones.kBoneNames[num];
			}
			outErrorMsg = text5;
			return true;
		}

		// Token: 0x0600778F RID: 30607 RVA: 0x0026BE74 File Offset: 0x0026A074
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneXform(Transform[] boneXforms, string boneName, out Transform boneXform)
		{
			boneXform = boneXforms[GTHardCodedBones.GetBoneIndex(boneName)];
			return boneXform != null;
		}

		// Token: 0x06007790 RID: 30608 RVA: 0x0026BE88 File Offset: 0x0026A088
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneXform(Transform[] boneXforms, GTHardCodedBones.EBone eBone, out Transform boneXform)
		{
			boneXform = boneXforms[GTHardCodedBones.GetBoneIndex(eBone)];
			return boneXform != null;
		}

		// Token: 0x06007791 RID: 30609 RVA: 0x0026BE9C File Offset: 0x0026A09C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetFirstBoneInParents(Transform transform, out GTHardCodedBones.EBone eBone, out Transform boneXform)
		{
			while (transform != null)
			{
				string name = transform.name;
				if (name == "DropZoneAnchor" && transform.parent != null)
				{
					string name2 = transform.parent.name;
					if (name2 == "Slingshot Chest Snap")
					{
						eBone = GTHardCodedBones.EBone.body_AnchorFront_StowSlot;
						boneXform = transform;
						return true;
					}
					if (name2 == "TransferrableItemLeftArm")
					{
						eBone = GTHardCodedBones.EBone.forearm_L;
						boneXform = transform;
						return true;
					}
					if (name2 == "TransferrableItemLeftShoulder")
					{
						eBone = GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot;
						boneXform = transform;
						return true;
					}
					if (name2 == "TransferrableItemRightShoulder")
					{
						eBone = GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot;
						boneXform = transform;
						return true;
					}
				}
				else
				{
					if (name == "TransferrableItemLeftHand")
					{
						eBone = GTHardCodedBones.EBone.hand_L;
						boneXform = transform;
						return true;
					}
					if (name == "TransferrableItemRightHand")
					{
						eBone = GTHardCodedBones.EBone.hand_R;
						boneXform = transform;
						return true;
					}
				}
				GTHardCodedBones.EBone bone = GTHardCodedBones.GetBone(transform.name);
				if (bone != GTHardCodedBones.EBone.None)
				{
					eBone = bone;
					boneXform = transform;
					return true;
				}
				transform = transform.parent;
			}
			eBone = GTHardCodedBones.EBone.None;
			boneXform = null;
			return false;
		}

		// Token: 0x06007792 RID: 30610 RVA: 0x0026BF8C File Offset: 0x0026A18C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GTHardCodedBones.EBone GetBoneEnumOfCosmeticPosStateFlag(TransferrableObject.PositionState positionState)
		{
			if (positionState <= TransferrableObject.PositionState.OnChest)
			{
				switch (positionState)
				{
				case TransferrableObject.PositionState.None:
					break;
				case TransferrableObject.PositionState.OnLeftArm:
					return GTHardCodedBones.EBone.forearm_L;
				case TransferrableObject.PositionState.OnRightArm:
					return GTHardCodedBones.EBone.forearm_R;
				case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm:
				case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.InLeftHand:
				case TransferrableObject.PositionState.OnRightArm | TransferrableObject.PositionState.InLeftHand:
				case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm | TransferrableObject.PositionState.InLeftHand:
					goto IL_005F;
				case TransferrableObject.PositionState.InLeftHand:
					return GTHardCodedBones.EBone.hand_L;
				case TransferrableObject.PositionState.InRightHand:
					return GTHardCodedBones.EBone.hand_R;
				default:
					if (positionState != TransferrableObject.PositionState.OnChest)
					{
						goto IL_005F;
					}
					return GTHardCodedBones.EBone.body_AnchorFront_StowSlot;
				}
			}
			else
			{
				if (positionState == TransferrableObject.PositionState.OnLeftShoulder)
				{
					return GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot;
				}
				if (positionState == TransferrableObject.PositionState.OnRightShoulder)
				{
					return GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot;
				}
				if (positionState != TransferrableObject.PositionState.Dropped)
				{
					goto IL_005F;
				}
			}
			return GTHardCodedBones.EBone.None;
			IL_005F:
			throw new ArgumentOutOfRangeException(positionState.ToString());
		}

		// Token: 0x06007793 RID: 30611 RVA: 0x0026C00C File Offset: 0x0026A20C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<GTHardCodedBones.EBone> GetBoneEnumsFromCosmeticBodyDockDropPosFlags(BodyDockPositions.DropPositions enumFlags)
		{
			BodyDockPositions.DropPositions[] values = EnumData<BodyDockPositions.DropPositions>.Shared.Values;
			List<GTHardCodedBones.EBone> list = new List<GTHardCodedBones.EBone>(32);
			foreach (BodyDockPositions.DropPositions dropPositions in values)
			{
				if (dropPositions != BodyDockPositions.DropPositions.All && dropPositions != BodyDockPositions.DropPositions.None && dropPositions != BodyDockPositions.DropPositions.MaxDropPostions && (enumFlags & dropPositions) != BodyDockPositions.DropPositions.None)
				{
					list.Add(GTHardCodedBones._k_bodyDockDropPosition_to_eBone[dropPositions]);
				}
			}
			return list;
		}

		// Token: 0x06007794 RID: 30612 RVA: 0x0026C064 File Offset: 0x0026A264
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<GTHardCodedBones.EBone> GetBoneEnumsFromCosmeticTransferrablePosStateFlags(TransferrableObject.PositionState enumFlags)
		{
			TransferrableObject.PositionState[] values = EnumData<TransferrableObject.PositionState>.Shared.Values;
			List<GTHardCodedBones.EBone> list = new List<GTHardCodedBones.EBone>(32);
			foreach (TransferrableObject.PositionState positionState in values)
			{
				if (positionState != TransferrableObject.PositionState.None && positionState != TransferrableObject.PositionState.Dropped && (enumFlags & positionState) != TransferrableObject.PositionState.None)
				{
					list.Add(GTHardCodedBones._k_transferrablePosState_to_eBone[positionState]);
				}
			}
			return list;
		}

		// Token: 0x06007795 RID: 30613 RVA: 0x0026C0B8 File Offset: 0x0026A2B8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetTransferrablePosStateFromBoneEnum(GTHardCodedBones.EBone eBone, out TransferrableObject.PositionState outPosState)
		{
			return GTHardCodedBones._k_eBone_to_transferrablePosState.TryGetValue(eBone, out outPosState);
		}

		// Token: 0x06007796 RID: 30614 RVA: 0x0026C0C8 File Offset: 0x0026A2C8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Transform GetBoneXformOfCosmeticPosStateFlag(TransferrableObject.PositionState anchorPosState, Transform[] bones)
		{
			if (bones.Length != 53)
			{
				throw new Exception(string.Format("{0}: Supplied bones array length is {1} but requires ", "GTHardCodedBones", bones.Length) + string.Format("{0}.", 53));
			}
			int boneIndex = GTHardCodedBones.GetBoneIndex(GTHardCodedBones.GetBoneEnumOfCosmeticPosStateFlag(anchorPosState));
			if (boneIndex != -1)
			{
				return bones[boneIndex];
			}
			return null;
		}

		// Token: 0x04008750 RID: 34640
		public const int kBoneCount = 53;

		// Token: 0x04008751 RID: 34641
		public static readonly string[] kBoneNames = new string[]
		{
			"None", "rig", "body", "head", "head_end", "shoulder.L", "upper_arm.L", "forearm.L", "hand.L", "palm.01.L",
			"palm.02.L", "thumb.01.L", "thumb.02.L", "thumb.03.L", "thumb.03.L_end", "f_index.01.L", "f_index.02.L", "f_index.03.L", "f_index.03.L_end", "f_middle.01.L",
			"f_middle.02.L", "f_middle.03.L", "f_middle.03.L_end", "shoulder.R", "upper_arm.R", "forearm.R", "hand.R", "palm.01.R", "palm.02.R", "thumb.01.R",
			"thumb.02.R", "thumb.03.R", "thumb.03.R_end", "f_index.01.R", "f_index.02.R", "f_index.03.R", "f_index.03.R_end", "f_middle.01.R", "f_middle.02.R", "f_middle.03.R",
			"f_middle.03.R_end", "body_AnchorTop_Neck", "body_AnchorFront_StowSlot", "body_AnchorFrontLeft_Badge", "body_AnchorFrontRight_NameTag", "body_AnchorBack", "body_AnchorBackLeft_StowSlot", "body_AnchorBackRight_StowSlot", "body_AnchorBottom", "body_AnchorBackBottom_Tail",
			"hand_L_AnchorBack", "hand_R_AnchorBack", "hand_L_AnchorFront_GameModeItemSlot"
		};

		// Token: 0x04008752 RID: 34642
		private const long kLeftSideMask = 1728432283058160L;

		// Token: 0x04008753 RID: 34643
		private const long kRightSideMask = 1769114204897280L;

		// Token: 0x04008754 RID: 34644
		private static readonly Dictionary<BodyDockPositions.DropPositions, GTHardCodedBones.EBone> _k_bodyDockDropPosition_to_eBone = new Dictionary<BodyDockPositions.DropPositions, GTHardCodedBones.EBone>
		{
			{
				BodyDockPositions.DropPositions.None,
				GTHardCodedBones.EBone.None
			},
			{
				BodyDockPositions.DropPositions.LeftArm,
				GTHardCodedBones.EBone.forearm_L
			},
			{
				BodyDockPositions.DropPositions.RightArm,
				GTHardCodedBones.EBone.forearm_R
			},
			{
				BodyDockPositions.DropPositions.Chest,
				GTHardCodedBones.EBone.body_AnchorFront_StowSlot
			},
			{
				BodyDockPositions.DropPositions.LeftBack,
				GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot
			},
			{
				BodyDockPositions.DropPositions.RightBack,
				GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot
			}
		};

		// Token: 0x04008755 RID: 34645
		private static readonly Dictionary<TransferrableObject.PositionState, GTHardCodedBones.EBone> _k_transferrablePosState_to_eBone = new Dictionary<TransferrableObject.PositionState, GTHardCodedBones.EBone>
		{
			{
				TransferrableObject.PositionState.None,
				GTHardCodedBones.EBone.None
			},
			{
				TransferrableObject.PositionState.OnLeftArm,
				GTHardCodedBones.EBone.forearm_L
			},
			{
				TransferrableObject.PositionState.OnRightArm,
				GTHardCodedBones.EBone.forearm_R
			},
			{
				TransferrableObject.PositionState.InLeftHand,
				GTHardCodedBones.EBone.hand_L
			},
			{
				TransferrableObject.PositionState.InRightHand,
				GTHardCodedBones.EBone.hand_R
			},
			{
				TransferrableObject.PositionState.OnChest,
				GTHardCodedBones.EBone.body_AnchorFront_StowSlot
			},
			{
				TransferrableObject.PositionState.OnLeftShoulder,
				GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot
			},
			{
				TransferrableObject.PositionState.OnRightShoulder,
				GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot
			},
			{
				TransferrableObject.PositionState.Dropped,
				GTHardCodedBones.EBone.None
			}
		};

		// Token: 0x04008756 RID: 34646
		private static readonly Dictionary<GTHardCodedBones.EBone, TransferrableObject.PositionState> _k_eBone_to_transferrablePosState = new Dictionary<GTHardCodedBones.EBone, TransferrableObject.PositionState>
		{
			{
				GTHardCodedBones.EBone.None,
				TransferrableObject.PositionState.None
			},
			{
				GTHardCodedBones.EBone.forearm_L,
				TransferrableObject.PositionState.OnLeftArm
			},
			{
				GTHardCodedBones.EBone.forearm_R,
				TransferrableObject.PositionState.OnRightArm
			},
			{
				GTHardCodedBones.EBone.hand_L,
				TransferrableObject.PositionState.InLeftHand
			},
			{
				GTHardCodedBones.EBone.hand_R,
				TransferrableObject.PositionState.InRightHand
			},
			{
				GTHardCodedBones.EBone.body_AnchorFront_StowSlot,
				TransferrableObject.PositionState.OnChest
			},
			{
				GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot,
				TransferrableObject.PositionState.OnLeftShoulder
			},
			{
				GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot,
				TransferrableObject.PositionState.OnRightShoulder
			}
		};

		// Token: 0x04008757 RID: 34647
		[OnEnterPlay_Clear]
		[OnExitPlay_Clear]
		private static readonly List<int> _gMissingBonesReport = new List<int>(53);

		// Token: 0x04008758 RID: 34648
		[OnEnterPlay_Clear]
		[OnExitPlay_Clear]
		private static readonly Dictionary<int, Transform[]> _gInstIds_To_boneXforms = new Dictionary<int, Transform[]>(20);

		// Token: 0x04008759 RID: 34649
		[OnEnterPlay_Clear]
		[OnExitPlay_Clear]
		private static readonly Dictionary<int, Transform[]> _gInstIds_To_slotXforms = new Dictionary<int, Transform[]>(20);

		// Token: 0x02001288 RID: 4744
		public enum EBone
		{
			// Token: 0x0400875B RID: 34651
			None,
			// Token: 0x0400875C RID: 34652
			rig,
			// Token: 0x0400875D RID: 34653
			body,
			// Token: 0x0400875E RID: 34654
			head,
			// Token: 0x0400875F RID: 34655
			head_end,
			// Token: 0x04008760 RID: 34656
			shoulder_L,
			// Token: 0x04008761 RID: 34657
			upper_arm_L,
			// Token: 0x04008762 RID: 34658
			forearm_L,
			// Token: 0x04008763 RID: 34659
			hand_L,
			// Token: 0x04008764 RID: 34660
			palm_01_L,
			// Token: 0x04008765 RID: 34661
			palm_02_L,
			// Token: 0x04008766 RID: 34662
			thumb_01_L,
			// Token: 0x04008767 RID: 34663
			thumb_02_L,
			// Token: 0x04008768 RID: 34664
			thumb_03_L,
			// Token: 0x04008769 RID: 34665
			thumb_03_L_end,
			// Token: 0x0400876A RID: 34666
			f_index_01_L,
			// Token: 0x0400876B RID: 34667
			f_index_02_L,
			// Token: 0x0400876C RID: 34668
			f_index_03_L,
			// Token: 0x0400876D RID: 34669
			f_index_03_L_end,
			// Token: 0x0400876E RID: 34670
			f_middle_01_L,
			// Token: 0x0400876F RID: 34671
			f_middle_02_L,
			// Token: 0x04008770 RID: 34672
			f_middle_03_L,
			// Token: 0x04008771 RID: 34673
			f_middle_03_L_end,
			// Token: 0x04008772 RID: 34674
			shoulder_R,
			// Token: 0x04008773 RID: 34675
			upper_arm_R,
			// Token: 0x04008774 RID: 34676
			forearm_R,
			// Token: 0x04008775 RID: 34677
			hand_R,
			// Token: 0x04008776 RID: 34678
			palm_01_R,
			// Token: 0x04008777 RID: 34679
			palm_02_R,
			// Token: 0x04008778 RID: 34680
			thumb_01_R,
			// Token: 0x04008779 RID: 34681
			thumb_02_R,
			// Token: 0x0400877A RID: 34682
			thumb_03_R,
			// Token: 0x0400877B RID: 34683
			thumb_03_R_end,
			// Token: 0x0400877C RID: 34684
			f_index_01_R,
			// Token: 0x0400877D RID: 34685
			f_index_02_R,
			// Token: 0x0400877E RID: 34686
			f_index_03_R,
			// Token: 0x0400877F RID: 34687
			f_index_03_R_end,
			// Token: 0x04008780 RID: 34688
			f_middle_01_R,
			// Token: 0x04008781 RID: 34689
			f_middle_02_R,
			// Token: 0x04008782 RID: 34690
			f_middle_03_R,
			// Token: 0x04008783 RID: 34691
			f_middle_03_R_end,
			// Token: 0x04008784 RID: 34692
			body_AnchorTop_Neck,
			// Token: 0x04008785 RID: 34693
			body_AnchorFront_StowSlot,
			// Token: 0x04008786 RID: 34694
			body_AnchorFrontLeft_Badge,
			// Token: 0x04008787 RID: 34695
			body_AnchorFrontRight_NameTag,
			// Token: 0x04008788 RID: 34696
			body_AnchorBack,
			// Token: 0x04008789 RID: 34697
			body_AnchorBackLeft_StowSlot,
			// Token: 0x0400878A RID: 34698
			body_AnchorBackRight_StowSlot,
			// Token: 0x0400878B RID: 34699
			body_AnchorBottom,
			// Token: 0x0400878C RID: 34700
			body_AnchorBackBottom_Tail,
			// Token: 0x0400878D RID: 34701
			hand_L_AnchorBack,
			// Token: 0x0400878E RID: 34702
			hand_R_AnchorBack,
			// Token: 0x0400878F RID: 34703
			hand_L_AnchorFront_GameModeItemSlot
		}

		// Token: 0x02001289 RID: 4745
		public enum EStowSlots
		{
			// Token: 0x04008791 RID: 34705
			None,
			// Token: 0x04008792 RID: 34706
			forearm_L = 7,
			// Token: 0x04008793 RID: 34707
			forearm_R = 25,
			// Token: 0x04008794 RID: 34708
			body_AnchorFront_Chest = 42,
			// Token: 0x04008795 RID: 34709
			body_AnchorBackLeft = 46,
			// Token: 0x04008796 RID: 34710
			body_AnchorBackRight
		}

		// Token: 0x0200128A RID: 4746
		public enum EHandAndStowSlots
		{
			// Token: 0x04008798 RID: 34712
			None,
			// Token: 0x04008799 RID: 34713
			forearm_L = 7,
			// Token: 0x0400879A RID: 34714
			hand_L,
			// Token: 0x0400879B RID: 34715
			forearm_R = 25,
			// Token: 0x0400879C RID: 34716
			hand_R,
			// Token: 0x0400879D RID: 34717
			body_AnchorFront_Chest = 42,
			// Token: 0x0400879E RID: 34718
			body_AnchorBackLeft = 46,
			// Token: 0x0400879F RID: 34719
			body_AnchorBackRight
		}

		// Token: 0x0200128B RID: 4747
		public enum ECosmeticSlots
		{
			// Token: 0x040087A1 RID: 34721
			Hat = 4,
			// Token: 0x040087A2 RID: 34722
			Badge = 43,
			// Token: 0x040087A3 RID: 34723
			Face = 3,
			// Token: 0x040087A4 RID: 34724
			ArmLeft = 6,
			// Token: 0x040087A5 RID: 34725
			ArmRight = 24,
			// Token: 0x040087A6 RID: 34726
			BackLeft = 46,
			// Token: 0x040087A7 RID: 34727
			BackRight,
			// Token: 0x040087A8 RID: 34728
			HandLeft = 8,
			// Token: 0x040087A9 RID: 34729
			HandRight = 26,
			// Token: 0x040087AA RID: 34730
			Chest = 42,
			// Token: 0x040087AB RID: 34731
			Fur = 1,
			// Token: 0x040087AC RID: 34732
			Shirt,
			// Token: 0x040087AD RID: 34733
			Pants = 48,
			// Token: 0x040087AE RID: 34734
			Back = 45,
			// Token: 0x040087AF RID: 34735
			Arms = 2,
			// Token: 0x040087B0 RID: 34736
			TagEffect = 0
		}

		// Token: 0x0200128C RID: 4748
		[Serializable]
		public struct SturdyEBone : ISerializationCallbackReceiver
		{
			// Token: 0x17000BA8 RID: 2984
			// (get) Token: 0x06007798 RID: 30616 RVA: 0x0026C424 File Offset: 0x0026A624
			// (set) Token: 0x06007799 RID: 30617 RVA: 0x0026C42C File Offset: 0x0026A62C
			public GTHardCodedBones.EBone Bone
			{
				get
				{
					return this._bone;
				}
				set
				{
					this._bone = value;
					this._boneName = GTHardCodedBones.GetBoneName(this._bone);
				}
			}

			// Token: 0x0600779A RID: 30618 RVA: 0x0026C446 File Offset: 0x0026A646
			public SturdyEBone(GTHardCodedBones.EBone bone)
			{
				this._bone = bone;
				this._boneName = null;
			}

			// Token: 0x0600779B RID: 30619 RVA: 0x0026C456 File Offset: 0x0026A656
			public SturdyEBone(string boneName)
			{
				this._bone = GTHardCodedBones.GetBone(boneName);
				this._boneName = null;
			}

			// Token: 0x0600779C RID: 30620 RVA: 0x0026C46B File Offset: 0x0026A66B
			public static implicit operator GTHardCodedBones.EBone(GTHardCodedBones.SturdyEBone sturdyBone)
			{
				return sturdyBone.Bone;
			}

			// Token: 0x0600779D RID: 30621 RVA: 0x0026C474 File Offset: 0x0026A674
			public static implicit operator GTHardCodedBones.SturdyEBone(GTHardCodedBones.EBone bone)
			{
				return new GTHardCodedBones.SturdyEBone(bone);
			}

			// Token: 0x0600779E RID: 30622 RVA: 0x0026C46B File Offset: 0x0026A66B
			public static explicit operator int(GTHardCodedBones.SturdyEBone sturdyBone)
			{
				return (int)sturdyBone.Bone;
			}

			// Token: 0x0600779F RID: 30623 RVA: 0x0026C47C File Offset: 0x0026A67C
			public override string ToString()
			{
				return this._boneName;
			}

			// Token: 0x060077A0 RID: 30624 RVA: 0x00002C2D File Offset: 0x00000E2D
			void ISerializationCallbackReceiver.OnBeforeSerialize()
			{
			}

			// Token: 0x060077A1 RID: 30625 RVA: 0x0026C484 File Offset: 0x0026A684
			void ISerializationCallbackReceiver.OnAfterDeserialize()
			{
				if (string.IsNullOrEmpty(this._boneName))
				{
					this._bone = GTHardCodedBones.EBone.None;
					this._boneName = "None";
					return;
				}
				GTHardCodedBones.EBone bone = GTHardCodedBones.GetBone(this._boneName);
				if (bone != GTHardCodedBones.EBone.None)
				{
					this._bone = bone;
				}
			}

			// Token: 0x040087B1 RID: 34737
			[SerializeField]
			private GTHardCodedBones.EBone _bone;

			// Token: 0x040087B2 RID: 34738
			[SerializeField]
			private string _boneName;
		}
	}
}
