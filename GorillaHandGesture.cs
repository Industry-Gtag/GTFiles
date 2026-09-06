using System;
using UnityEngine;

// Token: 0x020002D4 RID: 724
[CreateAssetMenu(fileName = "New Hand Gesture", menuName = "Gorilla/Hand Gesture")]
public class GorillaHandGesture : ScriptableObject
{
	// Token: 0x170001CA RID: 458
	// (get) Token: 0x06001288 RID: 4744 RVA: 0x0006358C File Offset: 0x0006178C
	// (set) Token: 0x06001289 RID: 4745 RVA: 0x0006359B File Offset: 0x0006179B
	public GestureHandNode hand
	{
		get
		{
			return (GestureHandNode)this.nodes[0];
		}
		set
		{
			this.nodes[0] = value;
		}
	}

	// Token: 0x170001CB RID: 459
	// (get) Token: 0x0600128A RID: 4746 RVA: 0x000635A6 File Offset: 0x000617A6
	// (set) Token: 0x0600128B RID: 4747 RVA: 0x000635B0 File Offset: 0x000617B0
	public GestureNode palm
	{
		get
		{
			return this.nodes[1];
		}
		set
		{
			this.nodes[1] = value;
		}
	}

	// Token: 0x170001CC RID: 460
	// (get) Token: 0x0600128C RID: 4748 RVA: 0x000635BB File Offset: 0x000617BB
	// (set) Token: 0x0600128D RID: 4749 RVA: 0x000635C5 File Offset: 0x000617C5
	public GestureNode wrist
	{
		get
		{
			return this.nodes[2];
		}
		set
		{
			this.nodes[2] = value;
		}
	}

	// Token: 0x170001CD RID: 461
	// (get) Token: 0x0600128E RID: 4750 RVA: 0x000635D0 File Offset: 0x000617D0
	// (set) Token: 0x0600128F RID: 4751 RVA: 0x000635DA File Offset: 0x000617DA
	public GestureNode digits
	{
		get
		{
			return this.nodes[3];
		}
		set
		{
			this.nodes[3] = value;
		}
	}

	// Token: 0x170001CE RID: 462
	// (get) Token: 0x06001290 RID: 4752 RVA: 0x000635E5 File Offset: 0x000617E5
	// (set) Token: 0x06001291 RID: 4753 RVA: 0x000635F4 File Offset: 0x000617F4
	public GestureDigitNode thumb
	{
		get
		{
			return (GestureDigitNode)this.nodes[4];
		}
		set
		{
			this.nodes[4] = value;
		}
	}

	// Token: 0x170001CF RID: 463
	// (get) Token: 0x06001292 RID: 4754 RVA: 0x000635FF File Offset: 0x000617FF
	// (set) Token: 0x06001293 RID: 4755 RVA: 0x0006360E File Offset: 0x0006180E
	public GestureDigitNode index
	{
		get
		{
			return (GestureDigitNode)this.nodes[5];
		}
		set
		{
			this.nodes[5] = value;
		}
	}

	// Token: 0x170001D0 RID: 464
	// (get) Token: 0x06001294 RID: 4756 RVA: 0x00063619 File Offset: 0x00061819
	// (set) Token: 0x06001295 RID: 4757 RVA: 0x00063628 File Offset: 0x00061828
	public GestureDigitNode middle
	{
		get
		{
			return (GestureDigitNode)this.nodes[6];
		}
		set
		{
			this.nodes[6] = value;
		}
	}

	// Token: 0x06001296 RID: 4758 RVA: 0x00063633 File Offset: 0x00061833
	private static GestureNode[] InitNodes()
	{
		return new GestureNode[]
		{
			new GestureHandNode(),
			new GestureNode(),
			new GestureNode(),
			new GestureNode(),
			new GestureDigitNode(),
			new GestureDigitNode(),
			new GestureDigitNode()
		};
	}

	// Token: 0x040016A6 RID: 5798
	public bool track = true;

	// Token: 0x040016A7 RID: 5799
	public GestureNode[] nodes = GorillaHandGesture.InitNodes();
}
