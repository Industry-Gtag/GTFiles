using System;
using UnityEngine;

// Token: 0x02000E9A RID: 3738
public class ZoneGraphBSP : MonoBehaviour
{
	// Token: 0x170008AD RID: 2221
	// (get) Token: 0x06005ACB RID: 23243 RVA: 0x001D8E6F File Offset: 0x001D706F
	// (set) Token: 0x06005ACC RID: 23244 RVA: 0x001D8E76 File Offset: 0x001D7076
	public static ZoneGraphBSP Instance { get; private set; }

	// Token: 0x06005ACD RID: 23245 RVA: 0x001D8E7E File Offset: 0x001D707E
	private void Awake()
	{
		if (ZoneGraphBSP.Instance == null)
		{
			ZoneGraphBSP.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06005ACE RID: 23246 RVA: 0x001D8E9C File Offset: 0x001D709C
	public void Preprocess()
	{
		BoxCollider[] componentsInChildren = base.GetComponentsInChildren<BoxCollider>(true);
		if (componentsInChildren != null)
		{
			foreach (BoxCollider boxCollider in componentsInChildren)
			{
				if (boxCollider.transform.GetComponent<ZoneDef>() != null)
				{
					Object.Destroy(boxCollider);
				}
				else
				{
					Object.Destroy(boxCollider.gameObject);
				}
			}
		}
	}

	// Token: 0x06005ACF RID: 23247 RVA: 0x001D8EF0 File Offset: 0x001D70F0
	public void CompileBSP()
	{
		ZoneDef[] componentsInChildren = base.gameObject.GetComponentsInChildren<ZoneDef>();
		this.bspTree = BSPTreeBuilder.BuildTree(componentsInChildren);
		if (this.bspTree != null && this.bspTree.nodes != null)
		{
			Debug.Log(string.Format("BSP Tree compiled with {0} zones, {1} nodes", componentsInChildren.Length, this.bspTree.nodes.Length));
			return;
		}
		Debug.Log("BSP Tree compilation failed - no zones found");
	}

	// Token: 0x06005AD0 RID: 23248 RVA: 0x001D8F5E File Offset: 0x001D715E
	public ZoneDef FindZoneAtPoint(Vector3 worldPoint)
	{
		SerializableBSPTree serializableBSPTree = this.bspTree;
		if (serializableBSPTree == null)
		{
			return null;
		}
		return serializableBSPTree.FindZone(worldPoint);
	}

	// Token: 0x06005AD1 RID: 23249 RVA: 0x001D8F72 File Offset: 0x001D7172
	public bool IsPointInAnyZone(Vector3 worldPoint)
	{
		return this.FindZoneAtPoint(worldPoint) != null;
	}

	// Token: 0x06005AD2 RID: 23250 RVA: 0x001D8F81 File Offset: 0x001D7181
	public bool HasCompiledTree()
	{
		return this.bspTree != null && this.bspTree.nodes != null && this.bspTree.nodes.Length != 0;
	}

	// Token: 0x06005AD3 RID: 23251 RVA: 0x001D8FA9 File Offset: 0x001D71A9
	public SerializableBSPTree GetBSPTree()
	{
		return this.bspTree;
	}

	// Token: 0x04006BC8 RID: 27592
	[SerializeField]
	private SerializableBSPTree bspTree;
}
