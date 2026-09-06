using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200065C RID: 1628
public class BuilderRoom : MonoBehaviour
{
	// Token: 0x040034E1 RID: 13537
	public List<GameObject> disableColliderRoots;

	// Token: 0x040034E2 RID: 13538
	public List<GameObject> disableRenderRoots;

	// Token: 0x040034E3 RID: 13539
	public List<GameObject> disableGameObjectsForScene;

	// Token: 0x040034E4 RID: 13540
	public List<GameObject> disableObjectsForPersistent;

	// Token: 0x040034E5 RID: 13541
	public List<MeshRenderer> disabledRenderersForPersistent;

	// Token: 0x040034E6 RID: 13542
	public List<Collider> disabledCollidersForScene;
}
