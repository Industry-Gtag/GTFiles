using System;
using UnityEngine;

// Token: 0x02000426 RID: 1062
public class TestScreen : ArcadeGame
{
	// Token: 0x06001944 RID: 6468 RVA: 0x00036275 File Offset: 0x00034475
	public override byte[] GetNetworkState()
	{
		return null;
	}

	// Token: 0x06001945 RID: 6469 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void SetNetworkState(byte[] b)
	{
	}

	// Token: 0x06001946 RID: 6470 RVA: 0x0008E814 File Offset: 0x0008CA14
	private int buttonToLightIndex(int player, ArcadeButtons button)
	{
		int num = 0;
		if (button <= ArcadeButtons.RIGHT)
		{
			switch (button)
			{
			case ArcadeButtons.GRAB:
				num = 0;
				break;
			case ArcadeButtons.UP:
				num = 1;
				break;
			case ArcadeButtons.GRAB | ArcadeButtons.UP:
				break;
			case ArcadeButtons.DOWN:
				num = 2;
				break;
			default:
				if (button != ArcadeButtons.LEFT)
				{
					if (button == ArcadeButtons.RIGHT)
					{
						num = 4;
					}
				}
				else
				{
					num = 3;
				}
				break;
			}
		}
		else if (button != ArcadeButtons.B0)
		{
			if (button != ArcadeButtons.B1)
			{
				if (button == ArcadeButtons.TRIGGER)
				{
					num = 7;
				}
			}
			else
			{
				num = 6;
			}
		}
		else
		{
			num = 5;
		}
		return (player * 8 + num) % this.lights.Length;
	}

	// Token: 0x06001947 RID: 6471 RVA: 0x0008E88B File Offset: 0x0008CA8B
	protected override void ButtonUp(int player, ArcadeButtons button)
	{
		this.lights[this.buttonToLightIndex(player, button)].color = Color.red;
	}

	// Token: 0x06001948 RID: 6472 RVA: 0x0008E8A6 File Offset: 0x0008CAA6
	protected override void ButtonDown(int player, ArcadeButtons button)
	{
		this.lights[this.buttonToLightIndex(player, button)].color = Color.green;
	}

	// Token: 0x06001949 RID: 6473 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnTimeout()
	{
	}

	// Token: 0x0400245B RID: 9307
	[SerializeField]
	private SpriteRenderer[] lights;

	// Token: 0x0400245C RID: 9308
	[SerializeField]
	private Transform dot;
}
