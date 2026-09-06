using System;
using UnityEngine;

// Token: 0x02000B0A RID: 2826
public class MainCamera : MonoBehaviourStatic<MainCamera>
{
	// Token: 0x0600484D RID: 18509 RVA: 0x0018558D File Offset: 0x0018378D
	public static implicit operator Camera(MainCamera mc)
	{
		return mc.camera;
	}

	// Token: 0x04005AF3 RID: 23283
	public Camera camera;
}
