using System;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020004B9 RID: 1209
public class PaintbrawlBalloons : MonoBehaviour
{
	// Token: 0x06001D84 RID: 7556 RVA: 0x0009F6D0 File Offset: 0x0009D8D0
	protected void Awake()
	{
		this.matPropBlock = new MaterialPropertyBlock();
		this.renderers = new Renderer[this.balloons.Length];
		this.balloonsCachedActiveState = new bool[this.balloons.Length];
		for (int i = 0; i < this.balloons.Length; i++)
		{
			this.renderers[i] = this.balloons[i].GetComponentInChildren<Renderer>();
			this.balloonsCachedActiveState[i] = this.balloons[i].activeSelf;
		}
		this.colorShaderPropID = ShaderProps._Color;
	}

	// Token: 0x06001D85 RID: 7557 RVA: 0x0009F756 File Offset: 0x0009D956
	protected void OnEnable()
	{
		this.UpdateBalloonColors();
	}

	// Token: 0x06001D86 RID: 7558 RVA: 0x0009F760 File Offset: 0x0009D960
	protected void LateUpdate()
	{
		if (GorillaGameManager.instance != null && (this.bMgr != null || GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>() != null))
		{
			if (this.bMgr == null)
			{
				this.bMgr = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
			}
			int playerLives = this.bMgr.GetPlayerLives(this.myRig.creator);
			for (int i = 0; i < this.balloons.Length; i++)
			{
				bool flag = playerLives >= i + 1;
				if (flag != this.balloonsCachedActiveState[i])
				{
					this.balloonsCachedActiveState[i] = flag;
					this.balloons[i].SetActive(flag);
					if (!flag)
					{
						this.PopBalloon(i);
					}
				}
			}
		}
		else if (GorillaGameManager.instance != null)
		{
			base.gameObject.SetActive(false);
		}
		this.UpdateBalloonColors();
	}

	// Token: 0x06001D87 RID: 7559 RVA: 0x0009F84C File Offset: 0x0009DA4C
	private void PopBalloon(int i)
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(this.balloonPopFXPrefab, true);
		gameObject.transform.position = this.balloons[i].transform.position;
		GorillaColorizableBase componentInChildren = gameObject.GetComponentInChildren<GorillaColorizableBase>();
		if (componentInChildren != null)
		{
			componentInChildren.SetColor(this.teamColor);
		}
	}

	// Token: 0x06001D88 RID: 7560 RVA: 0x0009F8A4 File Offset: 0x0009DAA4
	public void UpdateBalloonColors()
	{
		if (this.bMgr != null && this.myRig.creator != null)
		{
			if (this.bMgr.OnRedTeam(this.myRig.creator))
			{
				this.teamColor = this.orangeColor;
			}
			else if (this.bMgr.OnBlueTeam(this.myRig.creator))
			{
				this.teamColor = this.blueColor;
			}
			else
			{
				this.teamColor = (this.myRig ? this.myRig.playerColor : this.defaultColor);
			}
		}
		if (this.teamColor != this.lastColor)
		{
			this.lastColor = this.teamColor;
			foreach (Renderer renderer in this.renderers)
			{
				if (renderer)
				{
					foreach (Material material in renderer.materials)
					{
						if (!(material == null))
						{
							if (material.HasProperty(ShaderProps._BaseColor))
							{
								material.SetColor(ShaderProps._BaseColor, this.teamColor);
							}
							if (material.HasProperty(ShaderProps._Color))
							{
								material.SetColor(ShaderProps._Color, this.teamColor);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x040027BE RID: 10174
	public VRRig myRig;

	// Token: 0x040027BF RID: 10175
	public GameObject[] balloons;

	// Token: 0x040027C0 RID: 10176
	public Color orangeColor;

	// Token: 0x040027C1 RID: 10177
	public Color blueColor;

	// Token: 0x040027C2 RID: 10178
	public Color defaultColor;

	// Token: 0x040027C3 RID: 10179
	public Color lastColor;

	// Token: 0x040027C4 RID: 10180
	public GameObject balloonPopFXPrefab;

	// Token: 0x040027C5 RID: 10181
	[HideInInspector]
	public GorillaPaintbrawlManager bMgr;

	// Token: 0x040027C6 RID: 10182
	public Player myPlayer;

	// Token: 0x040027C7 RID: 10183
	private int colorShaderPropID;

	// Token: 0x040027C8 RID: 10184
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x040027C9 RID: 10185
	private bool[] balloonsCachedActiveState;

	// Token: 0x040027CA RID: 10186
	private Renderer[] renderers;

	// Token: 0x040027CB RID: 10187
	private Color teamColor;
}
