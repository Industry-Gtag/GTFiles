using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000124 RID: 292
public class SIUpgradeXformOffsetter : MonoBehaviour
{
	// Token: 0x06000739 RID: 1849 RVA: 0x0002918C File Offset: 0x0002738C
	protected void Awake()
	{
		if (this.m_superInfectionGadget == null)
		{
			Debug.LogError("[SIUpgradeXformOffsetter]  ERROR!!!  Awake: Disabling component because `m_superInfectionGadget` is null. Path=" + base.transform.GetPathQ(), this);
			base.enabled = false;
			return;
		}
		foreach (SIUpgradeXformOffsetter.SIUpgradeXformOffsetOp siupgradeXformOffsetOp in this.m_upgradeXformOffsetOps)
		{
			if (!(siupgradeXformOffsetOp.xform != null) && !(siupgradeXformOffsetOp.targetXform != null))
			{
				Debug.LogError("[SIUpgradeXformOffsetter]  ERROR!!!  Awake: Disabling component because null reference in `m_upgradeXformOffsetOps` array. Path=" + base.transform.GetPathQ(), this);
				base.enabled = false;
				return;
			}
		}
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x00029226 File Offset: 0x00027426
	protected void OnEnable()
	{
		SIGadget superInfectionGadget = this.m_superInfectionGadget;
		superInfectionGadget.OnPostRefreshVisuals = (Action<SIUpgradeSet>)Delegate.Combine(superInfectionGadget.OnPostRefreshVisuals, new Action<SIUpgradeSet>(this._HandleGadgetOnPostRefreshVisuals));
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x0002924F File Offset: 0x0002744F
	protected void OnDisable()
	{
		SIGadget superInfectionGadget = this.m_superInfectionGadget;
		superInfectionGadget.OnPostRefreshVisuals = (Action<SIUpgradeSet>)Delegate.Remove(superInfectionGadget.OnPostRefreshVisuals, new Action<SIUpgradeSet>(this._HandleGadgetOnPostRefreshVisuals));
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x00029278 File Offset: 0x00027478
	private void _HandleGadgetOnPostRefreshVisuals(SIUpgradeSet upgradeSet)
	{
		for (int i = 0; i < this.m_upgradeXformOffsetOps.Length; i++)
		{
			SIUpgradeXformOffsetter.SIUpgradeXformOffsetOp siupgradeXformOffsetOp = this.m_upgradeXformOffsetOps[i];
			if (upgradeSet.Contains(siupgradeXformOffsetOp.upgradeType))
			{
				siupgradeXformOffsetOp.xform.SetLocalPositionAndRotation(siupgradeXformOffsetOp.targetXform.localPosition, siupgradeXformOffsetOp.targetXform.localRotation);
				siupgradeXformOffsetOp.xform.localScale = siupgradeXformOffsetOp.targetXform.localScale;
			}
		}
	}

	// Token: 0x0400097F RID: 2431
	private const string preLog = "[SIUpgradeXformOffsetter]  ";

	// Token: 0x04000980 RID: 2432
	private const string preErr = "[SIUpgradeXformOffsetter]  ERROR!!!  ";

	// Token: 0x04000981 RID: 2433
	[SerializeField]
	private SIGadget m_superInfectionGadget;

	// Token: 0x04000982 RID: 2434
	[SerializeField]
	private SIUpgradeXformOffsetter.SIUpgradeXformOffsetOp[] m_upgradeXformOffsetOps;

	// Token: 0x02000125 RID: 293
	[Serializable]
	public struct SIUpgradeXformOffsetOp
	{
		// Token: 0x04000983 RID: 2435
		public SIUpgradeType upgradeType;

		// Token: 0x04000984 RID: 2436
		public Transform xform;

		// Token: 0x04000985 RID: 2437
		[FormerlySerializedAs("newParent")]
		public Transform targetXform;
	}
}
