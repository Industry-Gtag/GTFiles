using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaLocomotion.Swimming;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000A4E RID: 2638
public class CustomMapsGorillaRopeSwing : GorillaRopeSwing
{
	// Token: 0x060043B2 RID: 17330 RVA: 0x001686FA File Offset: 0x001668FA
	protected override void Awake()
	{
		base.CalculateId(true);
		base.StartCoroutine(this.WaitForRopeLength());
	}

	// Token: 0x060043B3 RID: 17331 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void Start()
	{
	}

	// Token: 0x060043B4 RID: 17332 RVA: 0x00168710 File Offset: 0x00166910
	protected override void OnEnable()
	{
		if (!this.isRopeLengthSet)
		{
			return;
		}
		base.OnEnable();
	}

	// Token: 0x060043B5 RID: 17333 RVA: 0x00168721 File Offset: 0x00166921
	public void SetRopeLength(int length)
	{
		this.ropeLength = length;
		this.isRopeLengthSet = true;
	}

	// Token: 0x060043B6 RID: 17334 RVA: 0x00168734 File Offset: 0x00166934
	public void SetRopeProperties(GTObjectPlaceholder placeholder)
	{
		this.ropePlaceholder = placeholder;
		this.ropeLength = this.ropePlaceholder.ropeLength;
		this.ropeBitGenOffset = this.ropePlaceholder.ropeSegmentGenerationOffset;
		this.preExistingSegments = this.ropePlaceholder.ropeSwingSegments;
		this.ropeScale = this.ropePlaceholder.transform.localScale;
		base.transform.localScale = Vector3.one;
		this.isRopeLengthSet = true;
	}

	// Token: 0x060043B7 RID: 17335 RVA: 0x001687A8 File Offset: 0x001669A8
	private IEnumerator WaitForRopeLength()
	{
		while (!this.isRopeLengthSet)
		{
			yield return null;
		}
		this.RopeGeneration();
		base.Awake();
		base.OnEnable();
		base.Start();
		yield break;
	}

	// Token: 0x060043B8 RID: 17336 RVA: 0x001687B8 File Offset: 0x001669B8
	private void RopeGeneration()
	{
		List<Transform> list = new List<Transform>();
		if (this.preExistingSegments != null && this.preExistingSegments.Count > 0)
		{
			for (int i = 0; i < this.preExistingSegments.Count; i++)
			{
				this.preExistingSegments[i].transform.SetParent(base.transform);
				GorillaClimbable gorillaClimbable = this.preExistingSegments[i].AddComponent<GorillaClimbable>();
				gorillaClimbable.snapX = this.snapX;
				gorillaClimbable.snapY = this.snapY;
				gorillaClimbable.snapZ = this.snapZ;
				gorillaClimbable.maxDistanceSnap = this.maxDistanceSnap;
				gorillaClimbable.clip = this.onGrabSFX;
				gorillaClimbable.clipOnFullRelease = this.OnReleaseSFX;
				GorillaRopeSegment gorillaRopeSegment = this.preExistingSegments[i].AddComponent<GorillaRopeSegment>();
				gorillaRopeSegment.swing = this;
				gorillaRopeSegment.boneIndex = this.preExistingSegments[i].boneIndex;
				list.Add(this.preExistingSegments[i].transform);
			}
			base.transform.localScale = this.ropeScale;
			this.ropePlaceholder.transform.localScale = Vector3.one;
		}
		else
		{
			Vector3 vector = Vector3.zero;
			float y = this.prefabRopeBit.GetComponentInChildren<Renderer>().bounds.size.y;
			WaterVolume[] array = Object.FindObjectsByType<WaterVolume>(FindObjectsSortMode.None);
			List<Collider> list2 = new List<Collider>(array.Length);
			WaterVolume[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				foreach (Collider collider in array2[j].volumeColliders)
				{
					if (!(collider == null))
					{
						list2.Add(collider);
					}
				}
			}
			for (int k = 0; k < this.ropeLength + 1; k++)
			{
				bool flag = false;
				if (list2.Count > 0)
				{
					Collider collider2 = list2[0];
					if (collider2 != null)
					{
						Vector3 vector2 = base.transform.position + vector;
						Vector3 vector3 = vector2 + new Vector3(0f, -y, 0f);
						flag = collider2.bounds.Contains(vector2) || collider2.bounds.Contains(vector3);
					}
				}
				GameObject gameObject = Object.Instantiate<GameObject>(flag ? this.partiallyUnderwaterPrefab : this.prefabRopeBit, base.transform);
				gameObject.name = string.Format("RopeBone_{0:00}", k);
				gameObject.transform.localPosition = vector;
				gameObject.transform.localRotation = Quaternion.identity;
				vector += new Vector3(0f, -this.ropeBitGenOffset, 0f);
				GorillaRopeSegment component = gameObject.GetComponent<GorillaRopeSegment>();
				component.swing = this;
				component.boneIndex = k;
				list.Add(gameObject.transform);
			}
			list[0].GetComponent<BoxCollider>().center = new Vector3(0f, -0.65f, 0f);
			list[0].GetComponent<BoxCollider>().size = new Vector3(0.3f, 0.65f, 0.3f);
		}
		if (list.Count > 0)
		{
			list.Last<Transform>().gameObject.SetActive(false);
		}
		this.nodes = list.ToArray();
	}

	// Token: 0x040055B6 RID: 21942
	[SerializeField]
	private GameObject partiallyUnderwaterPrefab;

	// Token: 0x040055B7 RID: 21943
	private bool isRopeLengthSet;

	// Token: 0x040055B8 RID: 21944
	private List<RopeSwingSegment> preExistingSegments;

	// Token: 0x040055B9 RID: 21945
	private GTObjectPlaceholder ropePlaceholder;

	// Token: 0x040055BA RID: 21946
	private Vector3 ropeScale = Vector3.one;

	// Token: 0x040055BB RID: 21947
	public bool snapX;

	// Token: 0x040055BC RID: 21948
	public bool snapY;

	// Token: 0x040055BD RID: 21949
	public bool snapZ;

	// Token: 0x040055BE RID: 21950
	public float maxDistanceSnap = 0.05f;

	// Token: 0x040055BF RID: 21951
	public AudioClip onGrabSFX;

	// Token: 0x040055C0 RID: 21952
	public AudioClip OnReleaseSFX;
}
