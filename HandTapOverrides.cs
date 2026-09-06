using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020002DE RID: 734
[Serializable]
public class HandTapOverrides
{
	// Token: 0x040016D8 RID: 5848
	private const string PREFAB_TOOLTIP = "Must be in the global object pool and have a tag.\n\nPrefabs can have an FXModifier component to be adjusted after creation.";

	// Token: 0x040016D9 RID: 5849
	public bool overrideSurfacePrefab;

	// Token: 0x040016DA RID: 5850
	[Tooltip("Must be in the global object pool and have a tag.\n\nPrefabs can have an FXModifier component to be adjusted after creation.")]
	public HashWrapper surfaceTapPrefab;

	// Token: 0x040016DB RID: 5851
	public bool overrideGamemodePrefab;

	// Token: 0x040016DC RID: 5852
	[Tooltip("Must be in the global object pool and have a tag.\n\nPrefabs can have an FXModifier component to be adjusted after creation.")]
	public HashWrapper gamemodeTapPrefab;

	// Token: 0x040016DD RID: 5853
	public bool overrideSound;

	// Token: 0x040016DE RID: 5854
	public AudioClip tapSound;
}
