using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020002A5 RID: 677
public class ApplyMaterialProperty : MonoBehaviour
{
	// Token: 0x060011CB RID: 4555 RVA: 0x0005F740 File Offset: 0x0005D940
	private void Start()
	{
		this.UpdateShaderPropertyIds();
		if (this.applyOnStart)
		{
			this.Apply();
		}
	}

	// Token: 0x060011CC RID: 4556 RVA: 0x0005F758 File Offset: 0x0005D958
	public void Apply()
	{
		if (!this._renderer)
		{
			this._renderer = base.GetComponent<Renderer>();
		}
		ApplyMaterialProperty.ApplyMode applyMode = this.mode;
		if (applyMode == ApplyMaterialProperty.ApplyMode.MaterialInstance)
		{
			this.ApplyMaterialInstance();
			return;
		}
		if (applyMode != ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock)
		{
			return;
		}
		this.ApplyMaterialPropertyBlock();
	}

	// Token: 0x060011CD RID: 4557 RVA: 0x0005F79A File Offset: 0x0005D99A
	public void SetColor(string propertyName, Color color)
	{
		this.SetColor(Shader.PropertyToID(propertyName), color);
	}

	// Token: 0x060011CE RID: 4558 RVA: 0x0005F7A9 File Offset: 0x0005D9A9
	public void SetColor(int propertyId, Color color)
	{
		ApplyMaterialProperty.CustomMaterialData orCreateData = this.GetOrCreateData(propertyId, null);
		orCreateData.dataType = ApplyMaterialProperty.SuportedTypes.Color;
		orCreateData.color = color;
	}

	// Token: 0x060011CF RID: 4559 RVA: 0x0005F7C0 File Offset: 0x0005D9C0
	public void SetFloat(string propertyName, float value)
	{
		this.SetFloat(Shader.PropertyToID(propertyName), value);
	}

	// Token: 0x060011D0 RID: 4560 RVA: 0x0005F7CF File Offset: 0x0005D9CF
	public void SetFloat(int propertyId, float value)
	{
		ApplyMaterialProperty.CustomMaterialData orCreateData = this.GetOrCreateData(propertyId, null);
		orCreateData.dataType = ApplyMaterialProperty.SuportedTypes.Float;
		orCreateData.@float = value;
	}

	// Token: 0x060011D1 RID: 4561 RVA: 0x0005F7E8 File Offset: 0x0005D9E8
	private ApplyMaterialProperty.CustomMaterialData GetOrCreateData(int id, string propertyName)
	{
		for (int i = 0; i < this.customData.Count; i++)
		{
			if (this.customData[i].id == id)
			{
				return this.customData[i];
			}
		}
		ApplyMaterialProperty.CustomMaterialData customMaterialData = new ApplyMaterialProperty.CustomMaterialData(id, propertyName);
		this.customData.Add(customMaterialData);
		return customMaterialData;
	}

	// Token: 0x060011D2 RID: 4562 RVA: 0x0005F844 File Offset: 0x0005DA44
	private void ApplyMaterialInstance()
	{
		if (!this._instance)
		{
			this._instance = base.GetComponent<MaterialInstance>();
			if (this._instance == null)
			{
				this._instance = base.gameObject.AddComponent<MaterialInstance>();
			}
		}
		Material material = (this.targetMaterial = this._instance.Material);
		for (int i = 0; i < this.customData.Count; i++)
		{
			switch (this.customData[i].dataType)
			{
			case ApplyMaterialProperty.SuportedTypes.Color:
				material.SetColor(this.customData[i].id, this.customData[i].color);
				break;
			case ApplyMaterialProperty.SuportedTypes.Float:
				material.SetFloat(this.customData[i].id, this.customData[i].@float);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector2:
				material.SetVector(this.customData[i].id, this.customData[i].vector2);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector3:
				material.SetVector(this.customData[i].id, this.customData[i].vector3);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector4:
				material.SetVector(this.customData[i].id, this.customData[i].vector4);
				break;
			case ApplyMaterialProperty.SuportedTypes.Texture2D:
				material.SetTexture(this.customData[i].id, this.customData[i].texture2D);
				break;
			}
		}
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x060011D3 RID: 4563 RVA: 0x0005FA10 File Offset: 0x0005DC10
	private void ApplyMaterialPropertyBlock()
	{
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		this._renderer.GetPropertyBlock(this._block);
		for (int i = 0; i < this.customData.Count; i++)
		{
			switch (this.customData[i].dataType)
			{
			case ApplyMaterialProperty.SuportedTypes.Color:
				this._block.SetColor(this.customData[i].id, this.customData[i].color);
				break;
			case ApplyMaterialProperty.SuportedTypes.Float:
				this._block.SetFloat(this.customData[i].id, this.customData[i].@float);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector2:
				this._block.SetVector(this.customData[i].id, this.customData[i].vector2);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector3:
				this._block.SetVector(this.customData[i].id, this.customData[i].vector3);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector4:
				this._block.SetVector(this.customData[i].id, this.customData[i].vector4);
				break;
			case ApplyMaterialProperty.SuportedTypes.Texture2D:
				this._block.SetTexture(this.customData[i].id, this.customData[i].texture2D);
				break;
			}
		}
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x060011D4 RID: 4564 RVA: 0x0005FBD0 File Offset: 0x0005DDD0
	private void UpdateShaderPropertyIds()
	{
		for (int i = 0; i < this.customData.Count; i++)
		{
			if (this.customData[i] != null && !string.IsNullOrEmpty(this.customData[i].name))
			{
				this.customData[i].id = Shader.PropertyToID(this.customData[i].name);
			}
		}
	}

	// Token: 0x0400154E RID: 5454
	public ApplyMaterialProperty.ApplyMode mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;

	// Token: 0x0400154F RID: 5455
	[FormerlySerializedAs("materialToApplyBlock")]
	public Material targetMaterial;

	// Token: 0x04001550 RID: 5456
	[SerializeField]
	private MaterialInstance _instance;

	// Token: 0x04001551 RID: 5457
	[SerializeField]
	private Renderer _renderer;

	// Token: 0x04001552 RID: 5458
	public List<ApplyMaterialProperty.CustomMaterialData> customData;

	// Token: 0x04001553 RID: 5459
	[SerializeField]
	private bool applyOnStart;

	// Token: 0x04001554 RID: 5460
	[NonSerialized]
	private MaterialPropertyBlock _block;

	// Token: 0x020002A6 RID: 678
	public enum ApplyMode
	{
		// Token: 0x04001556 RID: 5462
		MaterialInstance,
		// Token: 0x04001557 RID: 5463
		MaterialPropertyBlock
	}

	// Token: 0x020002A7 RID: 679
	public enum SuportedTypes
	{
		// Token: 0x04001559 RID: 5465
		Color,
		// Token: 0x0400155A RID: 5466
		Float,
		// Token: 0x0400155B RID: 5467
		Vector2,
		// Token: 0x0400155C RID: 5468
		Vector3,
		// Token: 0x0400155D RID: 5469
		Vector4,
		// Token: 0x0400155E RID: 5470
		Texture2D
	}

	// Token: 0x020002A8 RID: 680
	[Serializable]
	public class CustomMaterialData
	{
		// Token: 0x060011D6 RID: 4566 RVA: 0x0005FC50 File Offset: 0x0005DE50
		public CustomMaterialData(string propertyName)
		{
			this.name = propertyName;
			this.id = Shader.PropertyToID(propertyName);
			this.dataType = ApplyMaterialProperty.SuportedTypes.Color;
			this.color = default(Color);
			this.@float = 0f;
			this.vector2 = default(Vector2);
			this.vector3 = default(Vector3);
			this.vector4 = default(Vector4);
			this.texture2D = null;
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x0005FCC0 File Offset: 0x0005DEC0
		public CustomMaterialData(int propertyId, string propertyName)
		{
			this.name = propertyName;
			this.id = propertyId;
			this.dataType = ApplyMaterialProperty.SuportedTypes.Color;
			this.color = default(Color);
			this.@float = 0f;
			this.vector2 = default(Vector2);
			this.vector3 = default(Vector3);
			this.vector4 = default(Vector4);
			this.texture2D = null;
		}

		// Token: 0x060011D8 RID: 4568 RVA: 0x0005FD2C File Offset: 0x0005DF2C
		public override int GetHashCode()
		{
			return new ValueTuple<int, ApplyMaterialProperty.SuportedTypes, Color, float, Vector2, Vector3, Vector4, ValueTuple<Texture2D>>(this.id, this.dataType, this.color, this.@float, this.vector2, this.vector3, this.vector4, new ValueTuple<Texture2D>(this.texture2D)).GetHashCode();
		}

		// Token: 0x0400155F RID: 5471
		public string name;

		// Token: 0x04001560 RID: 5472
		public int id;

		// Token: 0x04001561 RID: 5473
		public ApplyMaterialProperty.SuportedTypes dataType;

		// Token: 0x04001562 RID: 5474
		public Color color;

		// Token: 0x04001563 RID: 5475
		public float @float;

		// Token: 0x04001564 RID: 5476
		public Vector2 vector2;

		// Token: 0x04001565 RID: 5477
		public Vector3 vector3;

		// Token: 0x04001566 RID: 5478
		public Vector4 vector4;

		// Token: 0x04001567 RID: 5479
		public Texture2D texture2D;
	}
}
