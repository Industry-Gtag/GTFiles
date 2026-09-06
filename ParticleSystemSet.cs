using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x0200048F RID: 1167
public class ParticleSystemSet : MonoBehaviour
{
	// Token: 0x06001C63 RID: 7267 RVA: 0x00099B58 File Offset: 0x00097D58
	private void Awake()
	{
		this.localScale = base.transform.localScale;
		this.ps = base.GetComponentsInChildren<ParticleSystem>();
		List<ParticleSystem.MainModule> list = new List<ParticleSystem.MainModule>();
		List<ParticleSystem.EmissionModule> list2 = new List<ParticleSystem.EmissionModule>();
		this.skipSet = new HashSet<ParticleSystem.MainModule>();
		for (int i = 0; i < this.ps.Length; i++)
		{
			list.Add(this.ps[i].main);
			list2.Add(this.ps[i].emission);
		}
		for (int j = 0; j < this.skipForSimulationSpeed.Length; j++)
		{
			this.skipSet.Add(this.skipForSimulationSpeed[j].GetComponent<ParticleSystem>().main);
		}
		this.psMains = list.ToArray();
		this.psEmits = list2.ToArray();
		this.SetPlayBackSpeed(0f);
	}

	// Token: 0x06001C64 RID: 7268 RVA: 0x00099C26 File Offset: 0x00097E26
	public void SetFadeRate(float rate)
	{
		if (rate > 0f)
		{
			this.fadeRate = rate;
		}
	}

	// Token: 0x06001C65 RID: 7269 RVA: 0x00099C38 File Offset: 0x00097E38
	public void SetPlayBackSpeed(float target)
	{
		for (int i = 0; i < this.psMains.Length; i++)
		{
			if (!this.skipSet.Contains(this.psMains[i]))
			{
				this.psMains[i].simulationSpeed = target;
			}
		}
	}

	// Token: 0x06001C66 RID: 7270 RVA: 0x00099C84 File Offset: 0x00097E84
	public async void FadePlayBackSpeed(float target)
	{
		this.loop = this.fadeRate > 0f;
		while (this.loop)
		{
			for (int i = 0; i < this.psMains.Length; i++)
			{
				if (!this.skipSet.Contains(this.psMains[i]))
				{
					this.psMains[i].simulationSpeed = Mathf.MoveTowards(this.psMains[i].simulationSpeed, target, Time.deltaTime * this.fadeRate);
					this.loop = this.psMains[i].simulationSpeed != target;
				}
			}
			await Task.Yield();
		}
	}

	// Token: 0x06001C67 RID: 7271 RVA: 0x00099CC4 File Offset: 0x00097EC4
	public void SetColor(string RRGGBB)
	{
		Color color = new Color((float)int.Parse(RRGGBB.Substring(0, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBB.Substring(2, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBB.Substring(4, 2), NumberStyles.HexNumber) / 255f);
		float num;
		float num2;
		float num3;
		Color.RGBToHSV(color, out num, out num2, out num3);
		Color color2 = Color.HSVToRGB(num, num2, num3 / 4f);
		for (int i = 0; i < this.psMains.Length; i++)
		{
			this.psMains[i].startColor = new ParticleSystem.MinMaxGradient(color, color2);
		}
	}

	// Token: 0x06001C68 RID: 7272 RVA: 0x00099D70 File Offset: 0x00097F70
	public void SetColors(string RRGGBBRRGGBB)
	{
		Color color = new Color((float)int.Parse(RRGGBBRRGGBB.Substring(0, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBBRRGGBB.Substring(2, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBBRRGGBB.Substring(4, 2), NumberStyles.HexNumber) / 255f);
		Color color2 = new Color((float)int.Parse(RRGGBBRRGGBB.Substring(6, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBBRRGGBB.Substring(8, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBBRRGGBB.Substring(10, 2), NumberStyles.HexNumber) / 255f);
		for (int i = 0; i < this.psMains.Length; i++)
		{
			this.psMains[i].startColor = new ParticleSystem.MinMaxGradient(color, color2);
		}
	}

	// Token: 0x06001C69 RID: 7273 RVA: 0x00099E50 File Offset: 0x00098050
	public void Pause()
	{
		for (int i = 0; i < this.ps.Length; i++)
		{
			this.ps[i].Pause();
		}
	}

	// Token: 0x06001C6A RID: 7274 RVA: 0x00099E80 File Offset: 0x00098080
	public void StartEmission()
	{
		for (int i = 0; i < this.ps.Length; i++)
		{
			this.psMains[i].prewarm = false;
			this.ps[i].Play();
		}
		for (int j = 0; j < this.ActiveDuringEmission.Length; j++)
		{
			this.ActiveDuringEmission[j].SetActive(true);
		}
	}

	// Token: 0x06001C6B RID: 7275 RVA: 0x00099EE0 File Offset: 0x000980E0
	public void StopEmission()
	{
		for (int i = 0; i < this.ps.Length; i++)
		{
			this.ps[i].Stop();
		}
		for (int j = 0; j < this.ActiveDuringEmission.Length; j++)
		{
			this.ActiveDuringEmission[j].SetActive(false);
		}
	}

	// Token: 0x06001C6C RID: 7276 RVA: 0x00099F30 File Offset: 0x00098130
	public void Clear()
	{
		for (int i = 0; i < this.ps.Length; i++)
		{
			this.ps[i].Clear();
		}
	}

	// Token: 0x06001C6D RID: 7277 RVA: 0x00099F5D File Offset: 0x0009815D
	public void SetScaleXZ(float scaler)
	{
		base.transform.localScale = new Vector3(this.localScale.x * scaler, this.localScale.y, this.localScale.z * scaler);
	}

	// Token: 0x06001C6E RID: 7278 RVA: 0x00099F94 File Offset: 0x00098194
	public async void FadeScaleXZ(float scaler)
	{
		Vector3 targetScale = new Vector3(this.localScale.x * scaler, this.localScale.y, this.localScale.z * scaler);
		this.loop = this.fadeRate > 0f;
		while (this.loop)
		{
			for (int i = 0; i < this.psMains.Length; i++)
			{
				base.transform.localScale = Vector3.MoveTowards(base.transform.localScale, targetScale, Time.deltaTime * this.fadeRate);
			}
			this.loop = base.transform.localScale != targetScale;
			await Task.Yield();
		}
	}

	// Token: 0x0400267F RID: 9855
	[SerializeField]
	private GameObject[] ActiveDuringEmission;

	// Token: 0x04002680 RID: 9856
	private Vector3 localScale = Vector3.one;

	// Token: 0x04002681 RID: 9857
	private ParticleSystem[] ps;

	// Token: 0x04002682 RID: 9858
	private ParticleSystem.MainModule[] psMains;

	// Token: 0x04002683 RID: 9859
	public GameObject[] skipForSimulationSpeed;

	// Token: 0x04002684 RID: 9860
	private HashSet<ParticleSystem.MainModule> skipSet;

	// Token: 0x04002685 RID: 9861
	private ParticleSystem.EmissionModule[] psEmits;

	// Token: 0x04002686 RID: 9862
	private bool loop;

	// Token: 0x04002687 RID: 9863
	private float fadeRate = 1f;
}
