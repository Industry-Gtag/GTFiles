using System;
using UnityEngine;

// Token: 0x020007AF RID: 1967
public class GREnemyXRayVisionEffect : MonoBehaviour
{
	// Token: 0x06003268 RID: 12904 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void Awake()
	{
	}

	// Token: 0x06003269 RID: 12905 RVA: 0x0011464E File Offset: 0x0011284E
	private void Start()
	{
		base.InvokeRepeating("UpdateEffect", 0f, 0.5f);
	}

	// Token: 0x0600326A RID: 12906 RVA: 0x00114665 File Offset: 0x00112865
	private bool ShouldShowEffect()
	{
		return GRPlayer.GetLocal().HasXRayVision();
	}

	// Token: 0x0600326B RID: 12907 RVA: 0x00114671 File Offset: 0x00112871
	private void UpdateEffect()
	{
		this.enemyXRayEffect.SetActive(this.ShouldShowEffect());
	}

	// Token: 0x04004152 RID: 16722
	public GameObject enemyXRayEffect;
}
