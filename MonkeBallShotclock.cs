using System;
using TMPro;
using UnityEngine;

// Token: 0x02000614 RID: 1556
public class MonkeBallShotclock : MonoBehaviourTick
{
	// Token: 0x060026E4 RID: 9956 RVA: 0x000CDEBC File Offset: 0x000CC0BC
	public override void Tick()
	{
		if (this._time >= 0f)
		{
			this._time -= Time.deltaTime;
			this.UpdateTimeText(this._time);
			if (this._time < 0f)
			{
				this.SetBackboard(this.neutralMaterial);
			}
		}
	}

	// Token: 0x060026E5 RID: 9957 RVA: 0x000CDF10 File Offset: 0x000CC110
	public void SetTime(int teamId, float time)
	{
		this._time = time;
		if (teamId == -1)
		{
			this._time = 0f;
			this.SetBackboard(this.neutralMaterial);
		}
		else if (teamId >= 0 && teamId < this.teamMaterials.Length)
		{
			this.SetBackboard(this.teamMaterials[teamId]);
		}
		this.UpdateTimeText(time);
	}

	// Token: 0x060026E6 RID: 9958 RVA: 0x000CDF65 File Offset: 0x000CC165
	private void SetBackboard(Material teamMaterial)
	{
		if (this.backboard != null)
		{
			this.backboard.material = teamMaterial;
		}
	}

	// Token: 0x060026E7 RID: 9959 RVA: 0x000CDF84 File Offset: 0x000CC184
	private void UpdateTimeText(float time)
	{
		int num = Mathf.CeilToInt(time);
		if (this._timeInt != num)
		{
			this._timeInt = num;
			this.timeRemainingLabel.text = this._timeInt.ToString("#00");
		}
	}

	// Token: 0x0400325A RID: 12890
	public Renderer backboard;

	// Token: 0x0400325B RID: 12891
	public Material[] teamMaterials;

	// Token: 0x0400325C RID: 12892
	public Material neutralMaterial;

	// Token: 0x0400325D RID: 12893
	public TextMeshPro timeRemainingLabel;

	// Token: 0x0400325E RID: 12894
	private float _time;

	// Token: 0x0400325F RID: 12895
	private int _timeInt = -1;
}
