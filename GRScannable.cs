using System;
using UnityEngine;

// Token: 0x020007E4 RID: 2020
public class GRScannable : MonoBehaviour
{
	// Token: 0x0600338F RID: 13199 RVA: 0x00119E5B File Offset: 0x0011805B
	public virtual void Start()
	{
		if (this.gameEntity == null)
		{
			this.gameEntity = base.GetComponent<GameEntity>();
		}
	}

	// Token: 0x06003390 RID: 13200 RVA: 0x00119E77 File Offset: 0x00118077
	public virtual string GetTitleText(GhostReactor reactor)
	{
		return this.titleText;
	}

	// Token: 0x06003391 RID: 13201 RVA: 0x00119E7F File Offset: 0x0011807F
	public virtual string GetBodyText(GhostReactor reactor)
	{
		return this.bodyText;
	}

	// Token: 0x06003392 RID: 13202 RVA: 0x00119E87 File Offset: 0x00118087
	public virtual string GetAnnotationText(GhostReactor reactor)
	{
		return this.annotationText;
	}

	// Token: 0x040042E0 RID: 17120
	public GameEntity gameEntity;

	// Token: 0x040042E1 RID: 17121
	[SerializeField]
	protected string titleText;

	// Token: 0x040042E2 RID: 17122
	[SerializeField]
	protected string bodyText;

	// Token: 0x040042E3 RID: 17123
	[SerializeField]
	protected string annotationText;
}
