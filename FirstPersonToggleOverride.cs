using System;
using UnityEngine;

// Token: 0x020000A1 RID: 161
[RequireComponent(typeof(Renderer))]
public class FirstPersonToggleOverride : MonoBehaviour
{
	// Token: 0x17000049 RID: 73
	// (get) Token: 0x060003FE RID: 1022 RVA: 0x00017DE8 File Offset: 0x00015FE8
	public bool Toggle
	{
		get
		{
			return this.toggle;
		}
	}

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x060003FF RID: 1023 RVA: 0x00017DF0 File Offset: 0x00015FF0
	public Renderer Renderer
	{
		get
		{
			return this._renderer;
		}
	}

	// Token: 0x0400046E RID: 1134
	[SerializeField]
	private Renderer _renderer;

	// Token: 0x0400046F RID: 1135
	[SerializeField]
	private bool toggle;

	// Token: 0x04000470 RID: 1136
	[SerializeField]
	private bool doNotToggle = true;
}
