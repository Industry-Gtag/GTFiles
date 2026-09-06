using System;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000658 RID: 1624
public class BuilderResourceMeter : MonoBehaviour
{
	// Token: 0x0600288F RID: 10383 RVA: 0x000DB434 File Offset: 0x000D9634
	private void Awake()
	{
		this.fillColor = this.resourceColors.GetColorForType(this._resourceType);
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		this.fillCube.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetColor(ShaderProps._BaseColor, this.fillColor);
		this.fillCube.SetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetColor(ShaderProps._BaseColor, this.emptyColor);
		this.emptyCube.SetPropertyBlock(materialPropertyBlock);
		this.fillAmount = this.fillTarget;
	}

	// Token: 0x06002890 RID: 10384 RVA: 0x000DB4B0 File Offset: 0x000D96B0
	private void Start()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.OnZoneChanged();
	}

	// Token: 0x06002891 RID: 10385 RVA: 0x000DB4DE File Offset: 0x000D96DE
	private void OnDestroy()
	{
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x06002892 RID: 10386 RVA: 0x000DB514 File Offset: 0x000D9714
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
		if (flag != this.inBuilderZone)
		{
			this.inBuilderZone = flag;
			if (!flag)
			{
				this.fillCube.enabled = false;
				this.emptyCube.enabled = false;
				return;
			}
			this.fillCube.enabled = true;
			this.emptyCube.enabled = true;
			this.OnAvailableResourcesChange();
		}
	}

	// Token: 0x06002893 RID: 10387 RVA: 0x000DB578 File Offset: 0x000D9778
	public void OnAvailableResourcesChange()
	{
		if (this.table == null || this.table.maxResources == null)
		{
			return;
		}
		this.resourceMax = this.table.maxResources[(int)this._resourceType];
		int num = this.table.usedResources[(int)this._resourceType];
		if (num != this.usedResource)
		{
			this.usedResource = num;
			this.SetNormalizedFillTarget((float)(this.resourceMax - this.usedResource) / (float)this.resourceMax);
		}
	}

	// Token: 0x06002894 RID: 10388 RVA: 0x000DB5F8 File Offset: 0x000D97F8
	public void UpdateMeterFill()
	{
		if (this.animatingMeter)
		{
			float num = Mathf.MoveTowards(this.fillAmount, this.fillTarget, this.lerpSpeed * Time.deltaTime);
			this.UpdateFill(num);
		}
	}

	// Token: 0x06002895 RID: 10389 RVA: 0x000DB634 File Offset: 0x000D9834
	private void UpdateFill(float newFill)
	{
		this.fillAmount = newFill;
		if (Mathf.Approximately(this.fillAmount, this.fillTarget))
		{
			this.fillAmount = this.fillTarget;
			this.animatingMeter = false;
		}
		if (!this.inBuilderZone)
		{
			return;
		}
		if (this.fillAmount <= 1E-45f)
		{
			this.fillCube.enabled = false;
			float num = this.meterHeight / this.meshHeight;
			Vector3 vector = new Vector3(this.emptyCube.transform.localScale.x, num, this.emptyCube.transform.localScale.z);
			Vector3 vector2 = new Vector3(0f, this.meterHeight / 2f, 0f);
			this.emptyCube.transform.localScale = vector;
			this.emptyCube.transform.localPosition = vector2;
			this.emptyCube.enabled = true;
			return;
		}
		if (this.fillAmount >= 1f)
		{
			float num2 = this.meterHeight / this.meshHeight;
			Vector3 vector3 = new Vector3(this.fillCube.transform.localScale.x, num2, this.fillCube.transform.localScale.z);
			Vector3 vector4 = new Vector3(0f, this.meterHeight / 2f, 0f);
			this.fillCube.transform.localScale = vector3;
			this.fillCube.transform.localPosition = vector4;
			this.fillCube.enabled = true;
			this.emptyCube.enabled = false;
			return;
		}
		float num3 = this.meterHeight / this.meshHeight * this.fillAmount;
		Vector3 vector5 = new Vector3(this.fillCube.transform.localScale.x, num3, this.fillCube.transform.localScale.z);
		Vector3 vector6 = new Vector3(0f, num3 * this.meshHeight / 2f, 0f);
		this.fillCube.transform.localScale = vector5;
		this.fillCube.transform.localPosition = vector6;
		this.fillCube.enabled = true;
		float num4 = this.meterHeight / this.meshHeight * (1f - this.fillAmount);
		Vector3 vector7 = new Vector3(this.emptyCube.transform.localScale.x, num4, this.emptyCube.transform.localScale.z);
		Vector3 vector8 = new Vector3(0f, this.meterHeight - num4 * this.meshHeight / 2f, 0f);
		this.emptyCube.transform.localScale = vector7;
		this.emptyCube.transform.localPosition = vector8;
		this.emptyCube.enabled = true;
	}

	// Token: 0x06002896 RID: 10390 RVA: 0x000DB908 File Offset: 0x000D9B08
	public void SetNormalizedFillTarget(float fill)
	{
		this.fillTarget = Mathf.Clamp(fill, 0f, 1f);
		this.animatingMeter = true;
	}

	// Token: 0x040034C9 RID: 13513
	public BuilderResourceColors resourceColors;

	// Token: 0x040034CA RID: 13514
	public MeshRenderer fillCube;

	// Token: 0x040034CB RID: 13515
	public MeshRenderer emptyCube;

	// Token: 0x040034CC RID: 13516
	private Color fillColor = Color.white;

	// Token: 0x040034CD RID: 13517
	public Color emptyColor = Color.black;

	// Token: 0x040034CE RID: 13518
	[FormerlySerializedAs("MeterHeight")]
	public float meterHeight = 2f;

	// Token: 0x040034CF RID: 13519
	public float meshHeight = 1f;

	// Token: 0x040034D0 RID: 13520
	public BuilderResourceType _resourceType;

	// Token: 0x040034D1 RID: 13521
	private float fillAmount;

	// Token: 0x040034D2 RID: 13522
	[Range(0f, 1f)]
	[SerializeField]
	private float fillTarget;

	// Token: 0x040034D3 RID: 13523
	public float lerpSpeed = 0.5f;

	// Token: 0x040034D4 RID: 13524
	private bool animatingMeter;

	// Token: 0x040034D5 RID: 13525
	private int resourceMax = -1;

	// Token: 0x040034D6 RID: 13526
	private int usedResource = -1;

	// Token: 0x040034D7 RID: 13527
	private bool inBuilderZone;

	// Token: 0x040034D8 RID: 13528
	internal BuilderTable table;
}
