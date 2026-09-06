using System;
using Drawing;
using UnityEngine;

// Token: 0x02000AE8 RID: 2792
public class ComputePenetration : MonoBehaviour
{
	// Token: 0x060047B0 RID: 18352 RVA: 0x001836F3 File Offset: 0x001818F3
	public void Compute()
	{
		if (this.colliderA == null)
		{
			return;
		}
		this.colliderB == null;
	}

	// Token: 0x060047B1 RID: 18353 RVA: 0x00183714 File Offset: 0x00181914
	public void OnDrawGizmos()
	{
		if (this.colliderA.AsNull<Collider>() == null)
		{
			return;
		}
		if (this.colliderB.AsNull<Collider>() == null)
		{
			return;
		}
		Transform transform = this.colliderA.transform;
		Transform transform2 = this.colliderB.transform;
		if (this.lastUpdate.HasElapsed(0.5f, true))
		{
			this.overlapped = Physics.ComputePenetration(this.colliderA, transform.position, transform.rotation, this.colliderB, transform2.position, transform2.rotation, out this.direction, out this.distance);
		}
		Color color = (this.overlapped ? Color.red : Color.green);
		this.DrawCollider(this.colliderA, color);
		this.DrawCollider(this.colliderB, color);
		if (this.overlapped)
		{
			Vector3 position = this.colliderB.transform.position;
			Vector3 vector = position + this.direction * this.distance;
			Gizmos.DrawLine(position, vector);
		}
	}

	// Token: 0x060047B2 RID: 18354 RVA: 0x00183814 File Offset: 0x00181A14
	private unsafe void DrawCollider(Collider c, Color color)
	{
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithMatrix(c.transform.localToWorldMatrix))
		{
			commandBuilder.PushColor(color);
			BoxCollider boxCollider = c as BoxCollider;
			if (boxCollider == null)
			{
				SphereCollider sphereCollider = c as SphereCollider;
				if (sphereCollider == null)
				{
					CapsuleCollider capsuleCollider = c as CapsuleCollider;
					if (capsuleCollider != null)
					{
						commandBuilder.WireCapsule(capsuleCollider.center, Vector3.up, capsuleCollider.height, capsuleCollider.radius);
					}
				}
				else
				{
					commandBuilder.WireSphere(sphereCollider.center, sphereCollider.radius);
				}
			}
			else
			{
				commandBuilder.WireBox(boxCollider.center, boxCollider.size);
			}
			commandBuilder.PopColor();
		}
	}

	// Token: 0x04005A36 RID: 23094
	public Collider colliderA;

	// Token: 0x04005A37 RID: 23095
	public Collider colliderB;

	// Token: 0x04005A38 RID: 23096
	public bool overlapped;

	// Token: 0x04005A39 RID: 23097
	public Vector3 direction;

	// Token: 0x04005A3A RID: 23098
	public float distance;

	// Token: 0x04005A3B RID: 23099
	private TimeSince lastUpdate = TimeSince.Now();
}
