using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200034F RID: 847
public class GTPosRotConstraints : MonoBehaviour, ISpawnable
{
	// Token: 0x060014DB RID: 5339 RVA: 0x0006F8B8 File Offset: 0x0006DAB8
	public void Awake()
	{
		if (this._shouldCallOnSpawnDuringAwake)
		{
			VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
			if (componentInParent == null)
			{
				return;
			}
			((ISpawnable)this).OnSpawn(componentInParent);
		}
	}

	// Token: 0x17000213 RID: 531
	// (get) Token: 0x060014DC RID: 5340 RVA: 0x0006F8E8 File Offset: 0x0006DAE8
	// (set) Token: 0x060014DD RID: 5341 RVA: 0x0006F8F0 File Offset: 0x0006DAF0
	public bool IsSpawned { get; set; }

	// Token: 0x17000214 RID: 532
	// (get) Token: 0x060014DE RID: 5342 RVA: 0x0006F8F9 File Offset: 0x0006DAF9
	// (set) Token: 0x060014DF RID: 5343 RVA: 0x0006F901 File Offset: 0x0006DB01
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060014E0 RID: 5344 RVA: 0x0006F90C File Offset: 0x0006DB0C
	void ISpawnable.OnSpawn(VRRig rig)
	{
		Transform[] array = Array.Empty<Transform>();
		string text;
		if (rig != null && !GTHardCodedBones.TryGetBoneXforms(rig, out array, out text))
		{
			Debug.LogError("GTPosRotConstraints: Error getting bone Transforms: " + text, this);
			return;
		}
		for (int i = 0; i < this.constraints.Length; i++)
		{
			GorillaPosRotConstraint gorillaPosRotConstraint = this.constraints[i];
			if (Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.x, 0f) && Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.y, 0f) && Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.z, 0f) && Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.w, 0f))
			{
				gorillaPosRotConstraint.rotationOffset = Quaternion.identity;
			}
			if (!gorillaPosRotConstraint.follower)
			{
				Debug.LogError(string.Concat(new string[]
				{
					string.Format("{0}: Disabling component! At index {1}, Transform `follower` is ", "GTPosRotConstraints", i),
					"null. Affected component path: ",
					base.transform.GetPathQ(),
					"\n- Affected component path: ",
					base.transform.GetPathQ()
				}), this);
				base.enabled = false;
				return;
			}
			if (gorillaPosRotConstraint.sourceGorillaBone == GTHardCodedBones.EBone.None)
			{
				if (!gorillaPosRotConstraint.source)
				{
					if (string.IsNullOrEmpty(gorillaPosRotConstraint.sourceRelativePath))
					{
						Debug.LogError(string.Format("{0}: Disabling component! At index {1} Transform `source` is ", "GTPosRotConstraints", i) + "null, not EBone, and `sourceRelativePath` is null or empty.\n- Affected component path: " + base.transform.GetPathQ(), this);
						base.enabled = false;
						return;
					}
					if (!base.transform.TryFindByPath(gorillaPosRotConstraint.sourceRelativePath, out gorillaPosRotConstraint.source, false))
					{
						Debug.LogError(string.Concat(new string[]
						{
							string.Format("{0}: Disabling component! At index {1} Transform `source` is ", "GTPosRotConstraints", i),
							"null, not EBone, and could not find by path: \"",
							gorillaPosRotConstraint.sourceRelativePath,
							"\"\n- Affected component path: ",
							base.transform.GetPathQ()
						}), this);
						base.enabled = false;
						return;
					}
				}
				this.constraints[i] = gorillaPosRotConstraint;
			}
			else
			{
				if (rig == null)
				{
					Debug.LogError("GTPosRotConstraints: Disabling component! `VRRig` could not be found in parents, but " + string.Format("bone at index {0} is set to use EBone `{1}` but without `VRRig` it cannot ", i, gorillaPosRotConstraint.sourceGorillaBone) + "be resolved.\n- Affected component path: " + base.transform.GetPathQ(), this);
					base.enabled = false;
					return;
				}
				int boneIndex = GTHardCodedBones.GetBoneIndex(gorillaPosRotConstraint.sourceGorillaBone);
				if (boneIndex <= 0)
				{
					Debug.LogError(string.Format("{0}: (should never happen) Disabling component! At index {1}, could ", "GTPosRotConstraints", i) + string.Format("not find EBone `{0}`.\n", gorillaPosRotConstraint.sourceGorillaBone) + "- Affected component path: " + base.transform.GetPathQ(), this);
					base.enabled = false;
					return;
				}
				gorillaPosRotConstraint.source = array[boneIndex];
				if (!gorillaPosRotConstraint.source)
				{
					Debug.LogError(string.Concat(new string[]
					{
						string.Format("{0}: Disabling component! At index {1}, bone {2} was ", "GTPosRotConstraints", i, gorillaPosRotConstraint.sourceGorillaBone),
						"not present in `VRRig` path: ",
						rig.transform.GetPathQ(),
						"\n- Affected component path: ",
						base.transform.GetPathQ()
					}), this);
					base.enabled = false;
					return;
				}
				this.constraints[i] = gorillaPosRotConstraint;
			}
		}
		if (base.isActiveAndEnabled && !this._registerOnEnable)
		{
			GTPosRotConstraintManager.Register(this);
		}
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060014E2 RID: 5346 RVA: 0x0006FC83 File Offset: 0x0006DE83
	protected void OnEnable()
	{
		if (this.IsSpawned || this._registerOnEnable)
		{
			GTPosRotConstraintManager.Register(this);
		}
	}

	// Token: 0x060014E3 RID: 5347 RVA: 0x0006FC9B File Offset: 0x0006DE9B
	protected void OnDisable()
	{
		GTPosRotConstraintManager.Unregister(this);
	}

	// Token: 0x04001997 RID: 6551
	[SerializeField]
	private bool _shouldCallOnSpawnDuringAwake;

	// Token: 0x04001998 RID: 6552
	[Tooltip("Used for actors that get disabled and re-enabled")]
	[SerializeField]
	private bool _registerOnEnable;

	// Token: 0x04001999 RID: 6553
	public GorillaPosRotConstraint[] constraints;
}
