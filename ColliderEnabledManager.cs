using System;
using UnityEngine;

// Token: 0x0200003B RID: 59
public class ColliderEnabledManager : MonoBehaviour
{
	// Token: 0x060000F1 RID: 241 RVA: 0x000060CA File Offset: 0x000042CA
	private void Start()
	{
		this.floorEnabled = true;
		this.floorCollidersEnabled = true;
		ColliderEnabledManager.instance = this;
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x000060E0 File Offset: 0x000042E0
	private void OnDestroy()
	{
		ColliderEnabledManager.instance = null;
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x000060E8 File Offset: 0x000042E8
	public void DisableFloorForFrame()
	{
		this.floorEnabled = false;
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x000060F4 File Offset: 0x000042F4
	private void LateUpdate()
	{
		if (!this.floorEnabled && this.floorCollidersEnabled)
		{
			this.DisableFloor();
		}
		if (!this.floorCollidersEnabled && Time.time > this.timeDisabled + this.disableLength)
		{
			this.floorCollidersEnabled = true;
		}
		Collider[] array = this.floorCollider;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = this.floorCollidersEnabled;
		}
		if (this.floorCollidersEnabled)
		{
			GorillaSurfaceOverride[] array2 = this.walls;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].overrideIndex = this.wallsBeforeMaterial;
			}
		}
		else
		{
			GorillaSurfaceOverride[] array2 = this.walls;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].overrideIndex = this.wallsAfterMaterial;
			}
		}
		this.floorEnabled = true;
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x000061B4 File Offset: 0x000043B4
	private void DisableFloor()
	{
		this.floorCollidersEnabled = false;
		this.timeDisabled = Time.time;
	}

	// Token: 0x040000FE RID: 254
	public static ColliderEnabledManager instance;

	// Token: 0x040000FF RID: 255
	public Collider[] floorCollider;

	// Token: 0x04000100 RID: 256
	public bool floorEnabled;

	// Token: 0x04000101 RID: 257
	public bool wasFloorEnabled;

	// Token: 0x04000102 RID: 258
	public bool floorCollidersEnabled;

	// Token: 0x04000103 RID: 259
	[GorillaSoundLookup]
	public int wallsBeforeMaterial;

	// Token: 0x04000104 RID: 260
	[GorillaSoundLookup]
	public int wallsAfterMaterial;

	// Token: 0x04000105 RID: 261
	public GorillaSurfaceOverride[] walls;

	// Token: 0x04000106 RID: 262
	public float timeDisabled;

	// Token: 0x04000107 RID: 263
	public float disableLength;
}
