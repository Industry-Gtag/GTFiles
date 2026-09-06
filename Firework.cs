using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000E1A RID: 3610
public class Firework : MonoBehaviour
{
	// Token: 0x06005870 RID: 22640 RVA: 0x001CB73C File Offset: 0x001C993C
	private void Launch()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this._controller)
		{
			this._controller.Launch(this);
		}
	}

	// Token: 0x06005871 RID: 22641 RVA: 0x001CB760 File Offset: 0x001C9960
	private void OnValidate()
	{
		if (!this._controller)
		{
			this._controller = base.GetComponentInParent<FireworksController>();
		}
		if (!this._controller)
		{
			return;
		}
		Firework[] array = this._controller.fireworks;
		if (array.Contains(this))
		{
			return;
		}
		array = (from x in array.Concat(new Firework[] { this })
			where x != null
			select x).ToArray<Firework>();
		this._controller.fireworks = array;
	}

	// Token: 0x06005872 RID: 22642 RVA: 0x001CB7F0 File Offset: 0x001C99F0
	private void OnDrawGizmos()
	{
		if (!this._controller)
		{
			return;
		}
		this._controller.RenderGizmo(this, Color.cyan);
	}

	// Token: 0x06005873 RID: 22643 RVA: 0x001CB811 File Offset: 0x001C9A11
	private void OnDrawGizmosSelected()
	{
		if (!this._controller)
		{
			return;
		}
		this._controller.RenderGizmo(this, Color.yellow);
	}

	// Token: 0x0400688C RID: 26764
	[SerializeField]
	private FireworksController _controller;

	// Token: 0x0400688D RID: 26765
	[Space]
	public Transform origin;

	// Token: 0x0400688E RID: 26766
	public Transform target;

	// Token: 0x0400688F RID: 26767
	[Space]
	public Color colorOrigin = Color.cyan;

	// Token: 0x04006890 RID: 26768
	public Color colorTarget = Color.magenta;

	// Token: 0x04006891 RID: 26769
	[Space]
	public AudioSource sourceOrigin;

	// Token: 0x04006892 RID: 26770
	public AudioSource sourceTarget;

	// Token: 0x04006893 RID: 26771
	[Space]
	public ParticleSystem trail;

	// Token: 0x04006894 RID: 26772
	[Space]
	public ParticleSystem[] explosions;

	// Token: 0x04006895 RID: 26773
	[Space]
	public bool doTrail = true;

	// Token: 0x04006896 RID: 26774
	public bool doTrailAudio = true;

	// Token: 0x04006897 RID: 26775
	public bool doExplosion = true;
}
