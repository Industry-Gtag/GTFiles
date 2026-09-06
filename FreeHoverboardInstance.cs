using System;
using UnityEngine;

// Token: 0x02000903 RID: 2307
public class FreeHoverboardInstance : MonoBehaviour
{
	// Token: 0x1700057D RID: 1405
	// (get) Token: 0x06003C7D RID: 15485 RVA: 0x0014A334 File Offset: 0x00148534
	// (set) Token: 0x06003C7E RID: 15486 RVA: 0x0014A33C File Offset: 0x0014853C
	public Rigidbody Rigidbody { get; private set; }

	// Token: 0x1700057E RID: 1406
	// (get) Token: 0x06003C7F RID: 15487 RVA: 0x0014A345 File Offset: 0x00148545
	// (set) Token: 0x06003C80 RID: 15488 RVA: 0x0014A34D File Offset: 0x0014854D
	public Color boardColor { get; private set; }

	// Token: 0x06003C81 RID: 15489 RVA: 0x0014A358 File Offset: 0x00148558
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
		Material[] sharedMaterials = this.boardMesh.sharedMaterials;
		this.colorMaterial = new Material(sharedMaterials[1]);
		sharedMaterials[1] = this.colorMaterial;
		this.boardMesh.sharedMaterials = sharedMaterials;
	}

	// Token: 0x06003C82 RID: 15490 RVA: 0x0014A3A0 File Offset: 0x001485A0
	public void SetColor(Color col)
	{
		this.colorMaterial.color = col;
		this.boardColor = col;
	}

	// Token: 0x06003C83 RID: 15491 RVA: 0x0014A3B8 File Offset: 0x001485B8
	private void Update()
	{
		RaycastHit raycastHit;
		if (Physics.SphereCast(new Ray(base.transform.TransformPoint(this.sphereCastCenter), base.transform.TransformVector(Vector3.down)), this.sphereCastRadius, out raycastHit, 1f, this.hoverRaycastMask.value))
		{
			this.hasHoverPoint = true;
			this.hoverPoint = raycastHit.point;
			this.hoverNormal = raycastHit.normal;
			return;
		}
		this.hasHoverPoint = false;
	}

	// Token: 0x06003C84 RID: 15492 RVA: 0x0014A434 File Offset: 0x00148634
	private void FixedUpdate()
	{
		if (this.hasHoverPoint)
		{
			float num = Vector3.Dot(base.transform.TransformPoint(this.sphereCastCenter) - this.hoverPoint, this.hoverNormal);
			if (num < this.hoverHeight)
			{
				base.transform.position += this.hoverNormal * (this.hoverHeight - num);
				this.Rigidbody.linearVelocity = Vector3.ProjectOnPlane(this.Rigidbody.linearVelocity, this.hoverNormal);
				Vector3 vector = Quaternion.Inverse(base.transform.rotation) * this.Rigidbody.angularVelocity;
				vector.x *= this.avelocityDragWhileHovering;
				vector.z *= this.avelocityDragWhileHovering;
				this.Rigidbody.angularVelocity = base.transform.rotation * vector;
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.forward, this.hoverNormal), this.hoverNormal), this.hoverRotationLerp);
			}
		}
	}

	// Token: 0x04004D36 RID: 19766
	public int ownerActorNumber;

	// Token: 0x04004D37 RID: 19767
	public int boardIndex;

	// Token: 0x04004D38 RID: 19768
	[SerializeField]
	private Vector3 sphereCastCenter;

	// Token: 0x04004D39 RID: 19769
	[SerializeField]
	private float sphereCastRadius;

	// Token: 0x04004D3A RID: 19770
	[SerializeField]
	private LayerMask hoverRaycastMask;

	// Token: 0x04004D3B RID: 19771
	[SerializeField]
	private float hoverHeight;

	// Token: 0x04004D3C RID: 19772
	[SerializeField]
	private float hoverRotationLerp;

	// Token: 0x04004D3D RID: 19773
	[SerializeField]
	private float avelocityDragWhileHovering;

	// Token: 0x04004D3E RID: 19774
	[SerializeField]
	private MeshRenderer boardMesh;

	// Token: 0x04004D40 RID: 19776
	private Material colorMaterial;

	// Token: 0x04004D41 RID: 19777
	private bool hasHoverPoint;

	// Token: 0x04004D42 RID: 19778
	private Vector3 hoverPoint;

	// Token: 0x04004D43 RID: 19779
	private Vector3 hoverNormal;
}
