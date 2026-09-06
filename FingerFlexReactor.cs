using System;
using UnityEngine;

// Token: 0x02000696 RID: 1686
public class FingerFlexReactor : MonoBehaviour
{
	// Token: 0x06002A0B RID: 10763 RVA: 0x000E2A50 File Offset: 0x000E0C50
	private void Setup()
	{
		this._rig = base.GetComponentInParent<VRRig>();
		if (!this._rig)
		{
			return;
		}
		this._fingers = new VRMap[]
		{
			this._rig.leftThumb,
			this._rig.leftIndex,
			this._rig.leftMiddle,
			this._rig.rightThumb,
			this._rig.rightIndex,
			this._rig.rightMiddle
		};
	}

	// Token: 0x06002A0C RID: 10764 RVA: 0x000E2AD7 File Offset: 0x000E0CD7
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x06002A0D RID: 10765 RVA: 0x000E2ADF File Offset: 0x000E0CDF
	private void FixedUpdate()
	{
		this.UpdateBlendShapes();
	}

	// Token: 0x06002A0E RID: 10766 RVA: 0x000E2AE8 File Offset: 0x000E0CE8
	public void UpdateBlendShapes()
	{
		if (!this._rig)
		{
			return;
		}
		if (this._blendShapeTargets == null || this._fingers == null)
		{
			return;
		}
		if (this._blendShapeTargets.Length == 0 || this._fingers.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this._blendShapeTargets.Length; i++)
		{
			FingerFlexReactor.BlendShapeTarget blendShapeTarget = this._blendShapeTargets[i];
			if (blendShapeTarget != null)
			{
				int sourceFinger = (int)blendShapeTarget.sourceFinger;
				if (sourceFinger != -1)
				{
					SkinnedMeshRenderer targetRenderer = blendShapeTarget.targetRenderer;
					if (targetRenderer)
					{
						float lerpValue = FingerFlexReactor.GetLerpValue(this._fingers[sourceFinger]);
						Vector2 inputRange = blendShapeTarget.inputRange;
						Vector2 outputRange = blendShapeTarget.outputRange;
						float num = MathUtils.Linear(lerpValue, inputRange.x, inputRange.y, outputRange.x, outputRange.y);
						blendShapeTarget.currentValue = num;
						targetRenderer.SetBlendShapeWeight(blendShapeTarget.blendShapeIndex, num);
					}
				}
			}
		}
	}

	// Token: 0x06002A0F RID: 10767 RVA: 0x000E2BBC File Offset: 0x000E0DBC
	private static float GetLerpValue(VRMap map)
	{
		VRMapThumb vrmapThumb = map as VRMapThumb;
		float num;
		if (vrmapThumb == null)
		{
			VRMapIndex vrmapIndex = map as VRMapIndex;
			if (vrmapIndex == null)
			{
				VRMapMiddle vrmapMiddle = map as VRMapMiddle;
				if (vrmapMiddle == null)
				{
					num = 0f;
				}
				else
				{
					num = vrmapMiddle.calcT;
				}
			}
			else
			{
				num = vrmapIndex.calcT;
			}
		}
		else
		{
			num = ((vrmapThumb.calcT > 0.1f) ? 1f : 0f);
		}
		return num;
	}

	// Token: 0x040036AE RID: 13998
	[SerializeField]
	private VRRig _rig;

	// Token: 0x040036AF RID: 13999
	[SerializeField]
	private VRMap[] _fingers = new VRMap[0];

	// Token: 0x040036B0 RID: 14000
	[SerializeField]
	private FingerFlexReactor.BlendShapeTarget[] _blendShapeTargets = new FingerFlexReactor.BlendShapeTarget[0];

	// Token: 0x02000697 RID: 1687
	[Serializable]
	public class BlendShapeTarget
	{
		// Token: 0x040036B1 RID: 14001
		public FingerFlexReactor.FingerMap sourceFinger;

		// Token: 0x040036B2 RID: 14002
		public SkinnedMeshRenderer targetRenderer;

		// Token: 0x040036B3 RID: 14003
		public int blendShapeIndex;

		// Token: 0x040036B4 RID: 14004
		public Vector2 inputRange = new Vector2(0f, 1f);

		// Token: 0x040036B5 RID: 14005
		public Vector2 outputRange = new Vector2(0f, 1f);

		// Token: 0x040036B6 RID: 14006
		[NonSerialized]
		public float currentValue;
	}

	// Token: 0x02000698 RID: 1688
	public enum FingerMap
	{
		// Token: 0x040036B8 RID: 14008
		None = -1,
		// Token: 0x040036B9 RID: 14009
		LeftThumb,
		// Token: 0x040036BA RID: 14010
		LeftIndex,
		// Token: 0x040036BB RID: 14011
		LeftMiddle,
		// Token: 0x040036BC RID: 14012
		RightThumb,
		// Token: 0x040036BD RID: 14013
		RightIndex,
		// Token: 0x040036BE RID: 14014
		RightMiddle
	}
}
