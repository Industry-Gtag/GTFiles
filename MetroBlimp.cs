using System;
using UnityEngine;

// Token: 0x020001BE RID: 446
public class MetroBlimp : MonoBehaviour
{
	// Token: 0x06000BF3 RID: 3059 RVA: 0x0004163F File Offset: 0x0003F83F
	private void Awake()
	{
		this._startLocalHeight = base.transform.localPosition.y;
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x00041658 File Offset: 0x0003F858
	public void Tick()
	{
		bool flag = Mathf.Sin(Time.time * 2f) * 0.5f + 0.5f < 0.0001f;
		int num = Mathf.CeilToInt(this._numHandsOnBlimp / 2f);
		if (this._numHandsOnBlimp == 0f)
		{
			this._topStayTime = 0f;
			if (flag)
			{
				this.blimpRenderer.material.DisableKeyword("_INNER_GLOW");
			}
		}
		else
		{
			this._topStayTime += Time.deltaTime;
			if (flag)
			{
				this.blimpRenderer.material.EnableKeyword("_INNER_GLOW");
			}
		}
		Vector3 localPosition = base.transform.localPosition;
		Vector3 vector = localPosition;
		float y = vector.y;
		float num2 = this._startLocalHeight + this.descendOffset;
		float deltaTime = Time.deltaTime;
		if (num > 0)
		{
			if (y > num2)
			{
				vector += Vector3.down * (this.descendSpeed * (float)num * deltaTime);
			}
		}
		else if (y < this._startLocalHeight)
		{
			vector += Vector3.up * (this.ascendSpeed * deltaTime);
		}
		base.transform.localPosition = Vector3.Slerp(localPosition, vector, 0.5f);
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x00041787 File Offset: 0x0003F987
	private static bool IsPlayerHand(Collider c)
	{
		return c.gameObject.IsOnLayer(UnityLayer.GorillaHand);
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x00041796 File Offset: 0x0003F996
	private void OnTriggerEnter(Collider other)
	{
		if (MetroBlimp.IsPlayerHand(other))
		{
			this._numHandsOnBlimp += 1f;
		}
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x000417B2 File Offset: 0x0003F9B2
	private void OnTriggerExit(Collider other)
	{
		if (MetroBlimp.IsPlayerHand(other))
		{
			this._numHandsOnBlimp -= 1f;
		}
	}

	// Token: 0x04000E86 RID: 3718
	public MetroSpotlight spotLightLeft;

	// Token: 0x04000E87 RID: 3719
	public MetroSpotlight spotLightRight;

	// Token: 0x04000E88 RID: 3720
	[Space]
	public BoxCollider topCollider;

	// Token: 0x04000E89 RID: 3721
	public Material blimpMaterial;

	// Token: 0x04000E8A RID: 3722
	public Renderer blimpRenderer;

	// Token: 0x04000E8B RID: 3723
	[Space]
	public float ascendSpeed = 1f;

	// Token: 0x04000E8C RID: 3724
	public float descendSpeed = 0.5f;

	// Token: 0x04000E8D RID: 3725
	public float descendOffset = -24.1f;

	// Token: 0x04000E8E RID: 3726
	public float descendReactionTime = 3f;

	// Token: 0x04000E8F RID: 3727
	[Space]
	[NonSerialized]
	private float _startLocalHeight;

	// Token: 0x04000E90 RID: 3728
	[NonSerialized]
	private float _topStayTime;

	// Token: 0x04000E91 RID: 3729
	[NonSerialized]
	private float _numHandsOnBlimp;

	// Token: 0x04000E92 RID: 3730
	[NonSerialized]
	private bool _lowering;

	// Token: 0x04000E93 RID: 3731
	private const string _INNER_GLOW = "_INNER_GLOW";
}
