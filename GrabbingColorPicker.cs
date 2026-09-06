using System;
using GorillaNetworking;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000585 RID: 1413
public class GrabbingColorPicker : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x1400004E RID: 78
	// (add) Token: 0x060023DD RID: 9181 RVA: 0x000C153C File Offset: 0x000BF73C
	// (remove) Token: 0x060023DE RID: 9182 RVA: 0x000C1574 File Offset: 0x000BF774
	public event Action ColorChanged;

	// Token: 0x170003C7 RID: 967
	// (get) Token: 0x060023DF RID: 9183 RVA: 0x000C15A9 File Offset: 0x000BF7A9
	// (set) Token: 0x060023E0 RID: 9184 RVA: 0x000C15B1 File Offset: 0x000BF7B1
	public int Segment1 { get; private set; }

	// Token: 0x170003C8 RID: 968
	// (get) Token: 0x060023E1 RID: 9185 RVA: 0x000C15BA File Offset: 0x000BF7BA
	// (set) Token: 0x060023E2 RID: 9186 RVA: 0x000C15C2 File Offset: 0x000BF7C2
	public int Segment2 { get; private set; }

	// Token: 0x170003C9 RID: 969
	// (get) Token: 0x060023E3 RID: 9187 RVA: 0x000C15CB File Offset: 0x000BF7CB
	// (set) Token: 0x060023E4 RID: 9188 RVA: 0x000C15D3 File Offset: 0x000BF7D3
	public int Segment3 { get; private set; }

	// Token: 0x060023E5 RID: 9189 RVA: 0x000C15DC File Offset: 0x000BF7DC
	private void Start()
	{
		if (!this.setPlayerColor)
		{
			return;
		}
		float @float = PlayerPrefs.GetFloat("redValue", 0f);
		float float2 = PlayerPrefs.GetFloat("greenValue", 0f);
		float float3 = PlayerPrefs.GetFloat("blueValue", 0f);
		this.LoadColor(@float, float2, float3);
	}

	// Token: 0x060023E6 RID: 9190 RVA: 0x000C162C File Offset: 0x000BF82C
	public void LoadColor(float r, float g, float b)
	{
		this.Segment1 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, r));
		this.Segment2 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, g));
		this.Segment3 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, b));
		this.R_PushSlider.SetProgress(r);
		this.G_PushSlider.SetProgress(g);
		this.B_PushSlider.SetProgress(b);
		this.UpdateDisplay();
	}

	// Token: 0x060023E7 RID: 9191 RVA: 0x000C16B4 File Offset: 0x000BF8B4
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (!this.setPlayerColor)
		{
			return;
		}
		CosmeticsController.OnPlayerColorSet = (Action<float, float, float>)Delegate.Combine(CosmeticsController.OnPlayerColorSet, new Action<float, float, float>(this.LoadColor));
		if (GorillaTagger.Instance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.OnColorChanged += this.HandleLocalColorChanged;
		}
	}

	// Token: 0x060023E8 RID: 9192 RVA: 0x000C172C File Offset: 0x000BF92C
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (!this.setPlayerColor)
		{
			return;
		}
		CosmeticsController.OnPlayerColorSet = (Action<float, float, float>)Delegate.Remove(CosmeticsController.OnPlayerColorSet, new Action<float, float, float>(this.LoadColor));
		if (GorillaTagger.Instance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.OnColorChanged -= this.HandleLocalColorChanged;
		}
	}

	// Token: 0x060023E9 RID: 9193 RVA: 0x000C17A4 File Offset: 0x000BF9A4
	public void SliceUpdate()
	{
		this.hasUpdated = false;
		float progress = this.R_PushSlider.GetProgress();
		float progress2 = this.G_PushSlider.GetProgress();
		float progress3 = this.B_PushSlider.GetProgress();
		if (Mathf.Approximately(progress, this._cachedR) && Mathf.Approximately(progress2, this._cachedG) && Mathf.Approximately(progress3, this._cachedB))
		{
			return;
		}
		this.hasUpdated = true;
		this._cachedR = progress;
		this._cachedG = progress2;
		this._cachedB = progress3;
		int segment = this.Segment1;
		int segment2 = this.Segment2;
		int segment3 = this.Segment3;
		this.Segment1 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, this._cachedR));
		this.Segment2 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, this._cachedG));
		this.Segment3 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, this._cachedB));
		if (segment != this.Segment1 || segment2 != this.Segment2 || segment3 != this.Segment3)
		{
			this.hasUpdated = true;
			if (this.setPlayerColor)
			{
				this.SetPlayerColor();
			}
			this.UpdateDisplay();
			this.UpdateColor.Invoke(new Vector3((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f));
			if (segment != this.Segment1)
			{
				this.R_SliderAudio.GTPlay();
			}
			if (segment2 != this.Segment2)
			{
				this.G_SliderAudio.GTPlay();
			}
			if (segment3 != this.Segment3)
			{
				this.B_SliderAudio.GTPlay();
			}
			Action colorChanged = this.ColorChanged;
			if (colorChanged == null)
			{
				return;
			}
			colorChanged();
		}
	}

	// Token: 0x060023EA RID: 9194 RVA: 0x000C195C File Offset: 0x000BFB5C
	private void SetPlayerColor()
	{
		PlayerPrefs.SetFloat("redValue", (float)this.Segment1 / 9f);
		PlayerPrefs.SetFloat("greenValue", (float)this.Segment2 / 9f);
		PlayerPrefs.SetFloat("blueValue", (float)this.Segment3 / 9f);
		GorillaTagger.Instance.UpdateColor((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f);
		GorillaComputer.instance.UpdateColor((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f);
		PlayerPrefs.Save();
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
			{
				(float)this.Segment1 / 9f,
				(float)this.Segment2 / 9f,
				(float)this.Segment3 / 9f
			});
		}
	}

	// Token: 0x060023EB RID: 9195 RVA: 0x000C1A80 File Offset: 0x000BFC80
	private void SetSliderColors(float r, float g, float b)
	{
		if (!this.hasUpdated)
		{
			this.Segment1 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, r));
			this.Segment2 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, g));
			this.Segment3 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, b));
			this.R_PushSlider.SetProgress(r);
			this.G_PushSlider.SetProgress(g);
			this.B_PushSlider.SetProgress(b);
			this.UpdateDisplay();
		}
	}

	// Token: 0x060023EC RID: 9196 RVA: 0x000C1B10 File Offset: 0x000BFD10
	private void HandleLocalColorChanged(Color newColor)
	{
		this.SetSliderColors(newColor.r, newColor.g, newColor.b);
	}

	// Token: 0x060023ED RID: 9197 RVA: 0x000C1B2C File Offset: 0x000BFD2C
	private void UpdateDisplay()
	{
		this.textR.text = this.Segment1.ToString();
		this.textG.text = this.Segment2.ToString();
		this.textB.text = this.Segment3.ToString();
		Color color = new Color((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f);
		Renderer[] componentsInChildren = this.ColorSwatch.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material[] materials = componentsInChildren[i].materials;
			for (int j = 0; j < materials.Length; j++)
			{
				materials[j].color = color;
			}
		}
	}

	// Token: 0x060023EE RID: 9198 RVA: 0x000C1BF1 File Offset: 0x000BFDF1
	public void ResetSliders(Vector3 v)
	{
		this.SetSliderColors(v.x, v.y, v.z);
	}

	// Token: 0x04002F23 RID: 12067
	[SerializeField]
	private bool setPlayerColor = true;

	// Token: 0x04002F24 RID: 12068
	[SerializeField]
	private PushableSlider R_PushSlider;

	// Token: 0x04002F25 RID: 12069
	[SerializeField]
	private PushableSlider G_PushSlider;

	// Token: 0x04002F26 RID: 12070
	[SerializeField]
	private PushableSlider B_PushSlider;

	// Token: 0x04002F27 RID: 12071
	[SerializeField]
	private AudioSource R_SliderAudio;

	// Token: 0x04002F28 RID: 12072
	[SerializeField]
	private AudioSource G_SliderAudio;

	// Token: 0x04002F29 RID: 12073
	[SerializeField]
	private AudioSource B_SliderAudio;

	// Token: 0x04002F2A RID: 12074
	[SerializeField]
	private TextMeshPro textR;

	// Token: 0x04002F2B RID: 12075
	[SerializeField]
	private TextMeshPro textG;

	// Token: 0x04002F2C RID: 12076
	[SerializeField]
	private TextMeshPro textB;

	// Token: 0x04002F2D RID: 12077
	[SerializeField]
	private GameObject ColorSwatch;

	// Token: 0x04002F2E RID: 12078
	[SerializeField]
	private UnityEvent<Vector3> UpdateColor;

	// Token: 0x04002F33 RID: 12083
	private float _cachedR = float.MinValue;

	// Token: 0x04002F34 RID: 12084
	private float _cachedG = float.MinValue;

	// Token: 0x04002F35 RID: 12085
	private float _cachedB = float.MinValue;

	// Token: 0x04002F36 RID: 12086
	private bool hasUpdated;
}
