using System;
using UnityEngine;

// Token: 0x020004F1 RID: 1265
public class TransformReset : MonoBehaviour
{
	// Token: 0x06001EB8 RID: 7864 RVA: 0x000A3FEC File Offset: 0x000A21EC
	private void Awake()
	{
		Transform[] componentsInChildren = base.GetComponentsInChildren<Transform>();
		this.transformList = new TransformReset.OriginalGameObjectTransform[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			this.transformList[i] = new TransformReset.OriginalGameObjectTransform(componentsInChildren[i]);
		}
		this.ResetTransforms();
	}

	// Token: 0x06001EB9 RID: 7865 RVA: 0x000A4038 File Offset: 0x000A2238
	public void ReturnTransforms()
	{
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.tempTransformList)
		{
			originalGameObjectTransform.thisTransform.position = originalGameObjectTransform.thisPosition;
			originalGameObjectTransform.thisTransform.rotation = originalGameObjectTransform.thisRotation;
		}
	}

	// Token: 0x06001EBA RID: 7866 RVA: 0x000A4088 File Offset: 0x000A2288
	public void SetScale(float ratio)
	{
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.transformList)
		{
			originalGameObjectTransform.thisTransform.localScale *= ratio;
		}
	}

	// Token: 0x06001EBB RID: 7867 RVA: 0x000A40CC File Offset: 0x000A22CC
	public void ResetTransforms()
	{
		this.tempTransformList = new TransformReset.OriginalGameObjectTransform[this.transformList.Length];
		for (int i = 0; i < this.transformList.Length; i++)
		{
			this.tempTransformList[i] = new TransformReset.OriginalGameObjectTransform(this.transformList[i].thisTransform);
		}
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.transformList)
		{
			originalGameObjectTransform.thisTransform.position = originalGameObjectTransform.thisPosition;
			originalGameObjectTransform.thisTransform.rotation = originalGameObjectTransform.thisRotation;
		}
	}

	// Token: 0x04002906 RID: 10502
	private TransformReset.OriginalGameObjectTransform[] transformList;

	// Token: 0x04002907 RID: 10503
	private TransformReset.OriginalGameObjectTransform[] tempTransformList;

	// Token: 0x020004F2 RID: 1266
	private struct OriginalGameObjectTransform
	{
		// Token: 0x06001EBD RID: 7869 RVA: 0x000A4164 File Offset: 0x000A2364
		public OriginalGameObjectTransform(Transform constructionTransform)
		{
			this._thisTransform = constructionTransform;
			this._thisPosition = constructionTransform.position;
			this._thisRotation = constructionTransform.rotation;
		}

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x06001EBE RID: 7870 RVA: 0x000A4185 File Offset: 0x000A2385
		// (set) Token: 0x06001EBF RID: 7871 RVA: 0x000A418D File Offset: 0x000A238D
		public Transform thisTransform
		{
			get
			{
				return this._thisTransform;
			}
			set
			{
				this._thisTransform = value;
			}
		}

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x06001EC0 RID: 7872 RVA: 0x000A4196 File Offset: 0x000A2396
		// (set) Token: 0x06001EC1 RID: 7873 RVA: 0x000A419E File Offset: 0x000A239E
		public Vector3 thisPosition
		{
			get
			{
				return this._thisPosition;
			}
			set
			{
				this._thisPosition = value;
			}
		}

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x06001EC2 RID: 7874 RVA: 0x000A41A7 File Offset: 0x000A23A7
		// (set) Token: 0x06001EC3 RID: 7875 RVA: 0x000A41AF File Offset: 0x000A23AF
		public Quaternion thisRotation
		{
			get
			{
				return this._thisRotation;
			}
			set
			{
				this._thisRotation = value;
			}
		}

		// Token: 0x04002908 RID: 10504
		private Transform _thisTransform;

		// Token: 0x04002909 RID: 10505
		private Vector3 _thisPosition;

		// Token: 0x0400290A RID: 10506
		private Quaternion _thisRotation;
	}
}
