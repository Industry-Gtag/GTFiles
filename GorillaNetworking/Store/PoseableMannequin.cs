using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking.Store
{
	// Token: 0x02001151 RID: 4433
	public class PoseableMannequin : MonoBehaviour
	{
		// Token: 0x06006F46 RID: 28486 RVA: 0x0023E185 File Offset: 0x0023C385
		public void Start()
		{
			if (this.skinnedMeshRenderer)
			{
				this.skinnedMeshRenderer.gameObject.SetActive(false);
			}
			if (this.staticGorillaMesh)
			{
				this.staticGorillaMesh.gameObject.SetActive(true);
			}
		}

		// Token: 0x06006F47 RID: 28487 RVA: 0x00092236 File Offset: 0x00090436
		private string GetPrefabPathFromCurrentPrefabStage()
		{
			return "";
		}

		// Token: 0x06006F48 RID: 28488 RVA: 0x00092236 File Offset: 0x00090436
		private string GetMeshPathFromPrefabPath(string prefabPath)
		{
			return "";
		}

		// Token: 0x06006F49 RID: 28489 RVA: 0x0023E1C3 File Offset: 0x0023C3C3
		public void BakeSkinnedMesh()
		{
			this.BakeAndSaveMeshInPath(this.GetMeshPathFromPrefabPath(this.GetPrefabPathFromCurrentPrefabStage()));
		}

		// Token: 0x06006F4A RID: 28490 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void BakeAndSaveMeshInPath(string meshPath)
		{
		}

		// Token: 0x06006F4B RID: 28491 RVA: 0x0023E1D7 File Offset: 0x0023C3D7
		private void UpdateStaticMeshMannequin()
		{
			this.staticGorillaMesh.sharedMesh = this.BakedColliderMesh;
			this.staticGorillaMeshRenderer.sharedMaterials = this.skinnedMeshRenderer.sharedMaterials;
			this.staticGorillaMeshCollider.sharedMesh = this.BakedColliderMesh;
		}

		// Token: 0x06006F4C RID: 28492 RVA: 0x0023E211 File Offset: 0x0023C411
		private void UpdateSkinnedMeshCollider()
		{
			this.skinnedMeshCollider.sharedMesh = this.BakedColliderMesh;
		}

		// Token: 0x06006F4D RID: 28493 RVA: 0x0023E224 File Offset: 0x0023C424
		public void UpdateGTPosRotConstraints()
		{
			GTPosRotConstraints[] array = this.cosmeticConstraints;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].constraints.ForEach(delegate(GorillaPosRotConstraint c)
				{
					c.follower.rotation = c.source.rotation;
					c.follower.position = c.source.position;
				});
			}
		}

		// Token: 0x06006F4E RID: 28494 RVA: 0x0023E274 File Offset: 0x0023C474
		private void HookupCosmeticConstraints()
		{
			this.cosmeticConstraints = base.GetComponentsInChildren<GTPosRotConstraints>();
			foreach (GTPosRotConstraints gtposRotConstraints in this.cosmeticConstraints)
			{
				for (int j = 0; j < gtposRotConstraints.constraints.Length; j++)
				{
					gtposRotConstraints.constraints[j].source = this.FindBone(gtposRotConstraints.constraints[j].follower.name);
				}
			}
		}

		// Token: 0x06006F4F RID: 28495 RVA: 0x0023E2E8 File Offset: 0x0023C4E8
		private Transform FindBone(string boneName)
		{
			foreach (Transform transform in this.skinnedMeshRenderer.bones)
			{
				if (transform.name == boneName)
				{
					return transform;
				}
			}
			return null;
		}

		// Token: 0x06006F50 RID: 28496 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void CreasteTestClip()
		{
		}

		// Token: 0x06006F51 RID: 28497 RVA: 0x0023E324 File Offset: 0x0023C524
		public void SerializeVRRig()
		{
			base.StartCoroutine(this.SaveLocalPlayerPose());
		}

		// Token: 0x06006F52 RID: 28498 RVA: 0x0023E333 File Offset: 0x0023C533
		public IEnumerator SaveLocalPlayerPose()
		{
			yield return null;
			yield break;
		}

		// Token: 0x06006F53 RID: 28499 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void SerializeOutBonesFromSkinnedMesh(SkinnedMeshRenderer paramSkinnedMeshRenderer)
		{
		}

		// Token: 0x06006F54 RID: 28500 RVA: 0x0023E33C File Offset: 0x0023C53C
		public void SetCurvesForBone(SkinnedMeshRenderer paramSkinnedMeshRenderer, AnimationClip clip, Transform bone)
		{
			Keyframe[] array = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.x)
			};
			Keyframe[] array2 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.y)
			};
			Keyframe[] array3 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.z)
			};
			Keyframe[] array4 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.w)
			};
			AnimationCurve animationCurve = new AnimationCurve(array);
			AnimationCurve animationCurve2 = new AnimationCurve(array2);
			AnimationCurve animationCurve3 = new AnimationCurve(array3);
			AnimationCurve animationCurve4 = new AnimationCurve(array4);
			string text = "";
			string text2 = bone.name.Replace("_new", "");
			foreach (Transform transform in this.skinnedMeshRenderer.bones)
			{
				if (transform.name == text2)
				{
					text = transform.GetPath(this.skinnedMeshRenderer.transform.parent).TrimStart('/');
					break;
				}
			}
			clip.SetCurve(text, typeof(Transform), "m_LocalRotation.x", animationCurve);
			clip.SetCurve(text, typeof(Transform), "m_LocalRotation.y", animationCurve2);
			clip.SetCurve(text, typeof(Transform), "m_LocalRotation.z", animationCurve3);
			clip.SetCurve(text, typeof(Transform), "m_LocalRotation.w", animationCurve4);
		}

		// Token: 0x06006F55 RID: 28501 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void UpdatePrefabWithAnimationClip(string AnimationFileName)
		{
		}

		// Token: 0x06006F56 RID: 28502 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void LoadPoseOntoMannequin(AnimationClip clip, float frameTime = 0f)
		{
		}

		// Token: 0x06006F57 RID: 28503 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnValidate()
		{
		}

		// Token: 0x04007F47 RID: 32583
		public SkinnedMeshRenderer skinnedMeshRenderer;

		// Token: 0x04007F48 RID: 32584
		[FormerlySerializedAs("meshCollider")]
		public MeshCollider skinnedMeshCollider;

		// Token: 0x04007F49 RID: 32585
		public GTPosRotConstraints[] cosmeticConstraints;

		// Token: 0x04007F4A RID: 32586
		public Mesh BakedColliderMesh;

		// Token: 0x04007F4B RID: 32587
		[SerializeField]
		[FormerlySerializedAs("liveAssetPath")]
		protected string prefabAssetPath;

		// Token: 0x04007F4C RID: 32588
		[SerializeField]
		protected string prefabFolderPath;

		// Token: 0x04007F4D RID: 32589
		[SerializeField]
		protected string prefabAssetName;

		// Token: 0x04007F4E RID: 32590
		public MeshFilter staticGorillaMesh;

		// Token: 0x04007F4F RID: 32591
		public MeshCollider staticGorillaMeshCollider;

		// Token: 0x04007F50 RID: 32592
		public MeshRenderer staticGorillaMeshRenderer;
	}
}
