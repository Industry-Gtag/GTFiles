using System;
using UnityEngine;

// Token: 0x0200053F RID: 1343
public class PlantableObject : TransferrableObject
{
	// Token: 0x060021EA RID: 8682 RVA: 0x000B522B File Offset: 0x000B342B
	protected override void Awake()
	{
		base.Awake();
		this.materialPropertyBlock = new MaterialPropertyBlock();
	}

	// Token: 0x060021EB RID: 8683 RVA: 0x000B5240 File Offset: 0x000B3440
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.materialPropertyBlock.SetColor(ShaderProps._ColorR, this._colorR);
		this.flagRenderer.material = this.flagRenderer.sharedMaterial;
		this.flagRenderer.SetPropertyBlock(this.materialPropertyBlock);
		this.dippedColors = new PlantableObject.AppliedColors[20];
	}

	// Token: 0x060021EC RID: 8684 RVA: 0x000B52A0 File Offset: 0x000B34A0
	private void AssureShaderStuff()
	{
		if (!this.flagRenderer)
		{
			return;
		}
		if (this.materialPropertyBlock == null)
		{
			this.materialPropertyBlock = new MaterialPropertyBlock();
		}
		try
		{
			this.materialPropertyBlock.SetColor(ShaderProps._ColorR, this._colorR);
			this.materialPropertyBlock.SetColor(ShaderProps._ColorG, this._colorG);
		}
		catch
		{
			this.materialPropertyBlock = new MaterialPropertyBlock();
			this.materialPropertyBlock.SetColor(ShaderProps._ColorR, this._colorR);
			this.materialPropertyBlock.SetColor(ShaderProps._ColorG, this._colorG);
		}
		this.flagRenderer.material = this.flagRenderer.sharedMaterial;
		this.flagRenderer.SetPropertyBlock(this.materialPropertyBlock);
	}

	// Token: 0x1700039C RID: 924
	// (get) Token: 0x060021ED RID: 8685 RVA: 0x000B5370 File Offset: 0x000B3570
	// (set) Token: 0x060021EE RID: 8686 RVA: 0x000B5378 File Offset: 0x000B3578
	public Color colorR
	{
		get
		{
			return this._colorR;
		}
		set
		{
			this._colorR = value;
			this.AssureShaderStuff();
		}
	}

	// Token: 0x1700039D RID: 925
	// (get) Token: 0x060021EF RID: 8687 RVA: 0x000B5387 File Offset: 0x000B3587
	// (set) Token: 0x060021F0 RID: 8688 RVA: 0x000B538F File Offset: 0x000B358F
	public Color colorG
	{
		get
		{
			return this._colorG;
		}
		set
		{
			this._colorG = value;
			this.AssureShaderStuff();
		}
	}

	// Token: 0x1700039E RID: 926
	// (get) Token: 0x060021F1 RID: 8689 RVA: 0x000B539E File Offset: 0x000B359E
	// (set) Token: 0x060021F2 RID: 8690 RVA: 0x000B53A6 File Offset: 0x000B35A6
	public bool planted { get; private set; }

	// Token: 0x060021F3 RID: 8691 RVA: 0x000B53B0 File Offset: 0x000B35B0
	public void SetPlanted(bool newPlanted)
	{
		if (this.planted != newPlanted)
		{
			if (newPlanted)
			{
				if (!this.rigidbodyInstance.isKinematic)
				{
					this.rigidbodyInstance.isKinematic = true;
				}
				this.respawnAtTimestamp = Time.time + this.respawnAfterDuration;
			}
			else
			{
				this.respawnAtTimestamp = 0f;
			}
			this.planted = newPlanted;
		}
	}

	// Token: 0x060021F4 RID: 8692 RVA: 0x000B5408 File Offset: 0x000B3608
	private void AddRed()
	{
		this.AddColor(PlantableObject.AppliedColors.Red);
	}

	// Token: 0x060021F5 RID: 8693 RVA: 0x000B5411 File Offset: 0x000B3611
	private void AddGreen()
	{
		this.AddColor(PlantableObject.AppliedColors.Blue);
	}

	// Token: 0x060021F6 RID: 8694 RVA: 0x000B541A File Offset: 0x000B361A
	private void AddBlue()
	{
		this.AddColor(PlantableObject.AppliedColors.Green);
	}

	// Token: 0x060021F7 RID: 8695 RVA: 0x000B5423 File Offset: 0x000B3623
	private void AddBlack()
	{
		this.AddColor(PlantableObject.AppliedColors.Black);
	}

	// Token: 0x060021F8 RID: 8696 RVA: 0x000B542C File Offset: 0x000B362C
	public void AddColor(PlantableObject.AppliedColors color)
	{
		this.dippedColors[this.currentDipIndex] = color;
		this.currentDipIndex++;
		if (this.currentDipIndex >= this.dippedColors.Length)
		{
			this.currentDipIndex = 0;
		}
		this.UpdateDisplayedDippedColor();
	}

	// Token: 0x060021F9 RID: 8697 RVA: 0x000B5468 File Offset: 0x000B3668
	public void ClearColors()
	{
		for (int i = 0; i < this.dippedColors.Length; i++)
		{
			this.dippedColors[i] = PlantableObject.AppliedColors.None;
		}
		this.currentDipIndex = 0;
		this.UpdateDisplayedDippedColor();
	}

	// Token: 0x060021FA RID: 8698 RVA: 0x000B54A0 File Offset: 0x000B36A0
	public Color CalculateOutputColor()
	{
		Color color = Color.black;
		int num = 0;
		int num2 = 0;
		foreach (PlantableObject.AppliedColors appliedColors in this.dippedColors)
		{
			if (appliedColors == PlantableObject.AppliedColors.None)
			{
				break;
			}
			switch (appliedColors)
			{
			case PlantableObject.AppliedColors.Red:
				color += Color.red;
				num2++;
				break;
			case PlantableObject.AppliedColors.Green:
				color += Color.green;
				num2++;
				break;
			case PlantableObject.AppliedColors.Blue:
				color += Color.blue;
				num2++;
				break;
			case PlantableObject.AppliedColors.Black:
				num++;
				num2++;
				break;
			}
		}
		if (color == Color.black && num == 0)
		{
			return Color.white;
		}
		float num3 = Mathf.Max(new float[] { color.r, color.g, color.b });
		if (num3 == 0f)
		{
			return Color.black;
		}
		color /= num3;
		float num4 = (float)num / (float)num2;
		if (num4 > 0f)
		{
			color *= 1f - num4;
		}
		return color;
	}

	// Token: 0x060021FB RID: 8699 RVA: 0x000B55A9 File Offset: 0x000B37A9
	public void UpdateDisplayedDippedColor()
	{
		this.colorR = this.CalculateOutputColor();
	}

	// Token: 0x060021FC RID: 8700 RVA: 0x000B55B7 File Offset: 0x000B37B7
	public override void DropItem()
	{
		base.DropItem();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x060021FD RID: 8701 RVA: 0x000B55E4 File Offset: 0x000B37E4
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		this.itemState = (this.planted ? TransferrableObject.ItemStates.State1 : TransferrableObject.ItemStates.State0);
		if (this.respawnAtTimestamp != 0f && Time.time > this.respawnAtTimestamp)
		{
			this.respawnAtTimestamp = 0f;
			this.ResetToHome();
		}
	}

	// Token: 0x060021FE RID: 8702 RVA: 0x000B5634 File Offset: 0x000B3834
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x060021FF RID: 8703 RVA: 0x000B565E File Offset: 0x000B385E
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
	}

	// Token: 0x06002200 RID: 8704 RVA: 0x000B5668 File Offset: 0x000B3868
	public override bool ShouldBeKinematic()
	{
		return base.ShouldBeKinematic() || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x06002201 RID: 8705 RVA: 0x000B5680 File Offset: 0x000B3880
	public override void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		base.OnOwnershipTransferred(toPlayer, fromPlayer);
		if (toPlayer == null)
		{
			return;
		}
		if (toPlayer.IsLocal && this.itemState == TransferrableObject.ItemStates.State1)
		{
			this.respawnAtTimestamp = Time.time + this.respawnAfterDuration;
		}
		Action<Color> <>9__1;
		GorillaGameManager.OnInstanceReady(delegate
		{
			VRRig vrrig = GorillaGameManager.instance.FindPlayerVRRig(toPlayer);
			if (vrrig == null)
			{
				return;
			}
			VRRig vrrig2 = vrrig;
			Action<Color> action;
			if ((action = <>9__1) == null)
			{
				action = (<>9__1 = delegate(Color color1)
				{
					this.colorG = color1;
				});
			}
			vrrig2.OnColorInitialized(action);
		});
	}

	// Token: 0x04002CD0 RID: 11472
	public PlantablePoint point;

	// Token: 0x04002CD1 RID: 11473
	public float respawnAfterDuration;

	// Token: 0x04002CD2 RID: 11474
	private float respawnAtTimestamp;

	// Token: 0x04002CD3 RID: 11475
	public SkinnedMeshRenderer flagRenderer;

	// Token: 0x04002CD4 RID: 11476
	private MaterialPropertyBlock materialPropertyBlock;

	// Token: 0x04002CD5 RID: 11477
	[HideInInspector]
	[SerializeReference]
	private Color _colorR;

	// Token: 0x04002CD6 RID: 11478
	[HideInInspector]
	[SerializeReference]
	private Color _colorG;

	// Token: 0x04002CD8 RID: 11480
	public Transform flagTip;

	// Token: 0x04002CD9 RID: 11481
	public PlantableObject.AppliedColors[] dippedColors = new PlantableObject.AppliedColors[20];

	// Token: 0x04002CDA RID: 11482
	public int currentDipIndex;

	// Token: 0x02000540 RID: 1344
	public enum AppliedColors
	{
		// Token: 0x04002CDC RID: 11484
		None,
		// Token: 0x04002CDD RID: 11485
		Red,
		// Token: 0x04002CDE RID: 11486
		Green,
		// Token: 0x04002CDF RID: 11487
		Blue,
		// Token: 0x04002CE0 RID: 11488
		Black
	}
}
