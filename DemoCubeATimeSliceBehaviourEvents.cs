using System;
using PerformanceSystems;
using UnityEngine;

// Token: 0x02000D76 RID: 3446
public class DemoCubeATimeSliceBehaviourEvents : TimeSliceLodBehaviour
{
	// Token: 0x060054F8 RID: 21752 RVA: 0x001BDDE4 File Offset: 0x001BBFE4
	protected new void Awake()
	{
		base.Awake();
		this._renderer = base.GetComponent<Renderer>();
	}

	// Token: 0x060054F9 RID: 21753 RVA: 0x001BDDF8 File Offset: 0x001BBFF8
	public override void SliceUpdate(float deltaTime)
	{
		float num = 0f;
		for (int i = 0; i < this._iterationsOfExpensiveOp; i++)
		{
			num += Mathf.Sqrt((float)i * deltaTime);
		}
	}

	// Token: 0x060054FA RID: 21754 RVA: 0x001BDE28 File Offset: 0x001BC028
	public void OnLod0Enter()
	{
		this._renderer.material = this._red;
		base.gameObject.SetActive(true);
	}

	// Token: 0x060054FB RID: 21755 RVA: 0x001BDE47 File Offset: 0x001BC047
	public void OnLod1Enter()
	{
		this._renderer.material = this._green;
		base.gameObject.SetActive(true);
	}

	// Token: 0x060054FC RID: 21756 RVA: 0x00044B04 File Offset: 0x00042D04
	public void OnLodExit()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x040066AB RID: 26283
	[SerializeField]
	private int _iterationsOfExpensiveOp = 200;

	// Token: 0x040066AC RID: 26284
	[SerializeField]
	private Material _red;

	// Token: 0x040066AD RID: 26285
	[SerializeField]
	private Material _green;

	// Token: 0x040066AE RID: 26286
	private Renderer _renderer;
}
