using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000343 RID: 835
public class GorillaStatusToThermalTemperatureMono : MonoBehaviour, ISpawnable
{
	// Token: 0x1700020B RID: 523
	// (get) Token: 0x0600147D RID: 5245 RVA: 0x0006E08A File Offset: 0x0006C28A
	// (set) Token: 0x0600147E RID: 5246 RVA: 0x0006E092 File Offset: 0x0006C292
	public bool hasRig { get; private set; }

	// Token: 0x1700020C RID: 524
	// (get) Token: 0x0600147F RID: 5247 RVA: 0x0006E09B File Offset: 0x0006C29B
	public VRRig rig
	{
		get
		{
			return this.m_rig;
		}
	}

	// Token: 0x06001480 RID: 5248 RVA: 0x0006E0A4 File Offset: 0x0006C2A4
	public void SetRig(VRRig newRig)
	{
		if (newRig == this.m_rig)
		{
			return;
		}
		if (this.hasRig)
		{
			VRRig rig = this.m_rig;
			rig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(rig.OnMaterialIndexChanged, new Action<int, int>(this._OnMatChanged));
		}
		this.m_rig = newRig;
		this.hasRig = newRig != null;
		if (!this.hasRig || !base.isActiveAndEnabled)
		{
			return;
		}
		VRRig rig2 = this.m_rig;
		rig2.OnMaterialIndexChanged = (Action<int, int>)Delegate.Combine(rig2.OnMaterialIndexChanged, new Action<int, int>(this._OnMatChanged));
		this._InitRuntimeArray();
		this._OnMatChanged(-1, this.m_rig.setMatIndex);
	}

	// Token: 0x06001481 RID: 5249 RVA: 0x0006E153 File Offset: 0x0006C353
	protected void Awake()
	{
		this.hasRig = this.m_rig != null;
		this._InitRuntimeArray();
	}

	// Token: 0x06001482 RID: 5250 RVA: 0x0006E170 File Offset: 0x0006C370
	private void _InitRuntimeArray()
	{
		if (!this.hasRig || this._runtimeMatIndexes_to_temperatures != null)
		{
			return;
		}
		int num = VRRig.LocalRig.materialsToChangeTo.Length;
		this._runtimeMatIndexes_to_temperatures = new float[num];
		for (int i = 0; i < this._runtimeMatIndexes_to_temperatures.Length; i++)
		{
			this._runtimeMatIndexes_to_temperatures[i] = -32768f;
		}
		foreach (GorillaStatusToThermalTemperatureMono._MaterialIndexToTemperature materialIndexToTemperature in this.m_materialIndexesToTemperatures)
		{
			foreach (int num2 in materialIndexToTemperature.matIndexes)
			{
				if (num2 >= 0 && num2 < num)
				{
					this._runtimeMatIndexes_to_temperatures[num2] = materialIndexToTemperature.temperature;
				}
			}
		}
		if (!Application.isEditor)
		{
			this.m_materialIndexesToTemperatures = null;
		}
	}

	// Token: 0x06001483 RID: 5251 RVA: 0x0006E230 File Offset: 0x0006C430
	protected void OnEnable()
	{
		if (!this.hasRig || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.m_thermalSourceVolume == null)
		{
			GTDev.LogError<string>("[GorillaStatusToThermalTemperatureMono]  ERROR!!!  Disabling because thermal source is not assigned. Path=" + base.transform.GetPathQ(), this, null);
			base.enabled = false;
			return;
		}
		VRRig rig = this.m_rig;
		rig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Combine(rig.OnMaterialIndexChanged, new Action<int, int>(this._OnMatChanged));
		this._OnMatChanged(-1, this.m_rig.setMatIndex);
	}

	// Token: 0x06001484 RID: 5252 RVA: 0x0006E2B8 File Offset: 0x0006C4B8
	protected void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting || !this.hasRig)
		{
			return;
		}
		VRRig rig = this.m_rig;
		rig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(rig.OnMaterialIndexChanged, new Action<int, int>(this._OnMatChanged));
	}

	// Token: 0x06001485 RID: 5253 RVA: 0x0006E2F4 File Offset: 0x0006C4F4
	private void _OnMatChanged(int oldIndex, int newIndex)
	{
		float num = this._runtimeMatIndexes_to_temperatures[newIndex];
		this.m_thermalSourceVolume.celsius = num;
		this.m_thermalSourceVolume.enabled = num > -32767.99f;
	}

	// Token: 0x1700020D RID: 525
	// (get) Token: 0x06001486 RID: 5254 RVA: 0x0006E329 File Offset: 0x0006C529
	// (set) Token: 0x06001487 RID: 5255 RVA: 0x0006E331 File Offset: 0x0006C531
	public bool IsSpawned { get; set; }

	// Token: 0x1700020E RID: 526
	// (get) Token: 0x06001488 RID: 5256 RVA: 0x0006E33A File Offset: 0x0006C53A
	// (set) Token: 0x06001489 RID: 5257 RVA: 0x0006E342 File Offset: 0x0006C542
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x0600148A RID: 5258 RVA: 0x0006E34B File Offset: 0x0006C54B
	public void OnSpawn(VRRig newRig)
	{
		this.SetRig(newRig);
	}

	// Token: 0x0600148B RID: 5259 RVA: 0x0006E354 File Offset: 0x0006C554
	public void OnDespawn()
	{
		this.SetRig(null);
	}

	// Token: 0x04001953 RID: 6483
	private const string preLog = "[GorillaStatusToThermalTemperatureMono]  ";

	// Token: 0x04001954 RID: 6484
	private const string preErr = "[GorillaStatusToThermalTemperatureMono]  ERROR!!!  ";

	// Token: 0x04001955 RID: 6485
	[Tooltip("Should either be assigned here or via another script.")]
	[SerializeField]
	private VRRig m_rig;

	// Token: 0x04001957 RID: 6487
	[SerializeField]
	private ThermalSourceVolume m_thermalSourceVolume;

	// Token: 0x04001958 RID: 6488
	[SerializeField]
	private GorillaStatusToThermalTemperatureMono._MaterialIndexToTemperature[] m_materialIndexesToTemperatures;

	// Token: 0x04001959 RID: 6489
	[DebugReadout]
	private float[] _runtimeMatIndexes_to_temperatures;

	// Token: 0x0400195A RID: 6490
	private const float _k_invalidTemperature = -32768f;

	// Token: 0x02000344 RID: 836
	[Serializable]
	private struct _MaterialIndexToTemperature
	{
		// Token: 0x0400195D RID: 6493
		public int[] matIndexes;

		// Token: 0x0400195E RID: 6494
		public float temperature;
	}
}
