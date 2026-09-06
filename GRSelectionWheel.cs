using System;
using System.Collections.Generic;
using GorillaExtensions;
using TMPro;
using UnityEngine;

// Token: 0x020007EC RID: 2028
public class GRSelectionWheel : MonoBehaviour, ITickSystemTick
{
	// Token: 0x170004B4 RID: 1204
	// (get) Token: 0x060033C9 RID: 13257 RVA: 0x0011C6CE File Offset: 0x0011A8CE
	// (set) Token: 0x060033CA RID: 13258 RVA: 0x0011C6D6 File Offset: 0x0011A8D6
	public bool TickRunning { get; set; }

	// Token: 0x060033CB RID: 13259 RVA: 0x0011C6DF File Offset: 0x0011A8DF
	public void Start()
	{
		this.targetPage = 0;
	}

	// Token: 0x060033CC RID: 13260 RVA: 0x0001A297 File Offset: 0x00018497
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060033CD RID: 13261 RVA: 0x0001A29F File Offset: 0x0001849F
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x060033CE RID: 13262 RVA: 0x0011C6E8 File Offset: 0x0011A8E8
	public void ShowText(bool showText)
	{
		foreach (TMP_Text tmp_Text in this.shelfNames)
		{
			tmp_Text.enabled = showText;
		}
	}

	// Token: 0x060033CF RID: 13263 RVA: 0x0011C73C File Offset: 0x0011A93C
	public void InitFromNameList(List<string> shelves)
	{
		this.shelfNames.Clear();
		for (int i = 0; i < shelves.Count; i++)
		{
			TMP_Text tmp_Text = Object.Instantiate<TMP_Text>(this.templateText);
			tmp_Text.text = shelves[i];
			this.shelfNames.Add(tmp_Text);
			tmp_Text.transform.SetParent(base.transform, false);
		}
		this.UpdateVisuals();
	}

	// Token: 0x060033D0 RID: 13264 RVA: 0x0011C7A4 File Offset: 0x0011A9A4
	public void Tick()
	{
		if (!this.isBeingDrivenRemotely)
		{
			float num = this.deltaAngle * (float)this.shelfNames.Count;
			float num2 = this.currentAngle / this.deltaAngle;
			int num3 = (int)(num2 + 0.5f);
			if (this.rotSpeedMult == 0f)
			{
				float num4 = ((float)num3 - num2) * this.deltaAngle;
				this.currentAngle += num4 * (1f - Mathf.Exp(-20f * Time.deltaTime));
				this.targetPage = num3;
			}
			else
			{
				this.currentAngle += this.rotSpeedMult * Time.deltaTime * this.rotSpeed;
				this.currentAngle = Mathf.Clamp(this.currentAngle, -this.deltaAngle * 0.4f, num - this.deltaAngle + this.deltaAngle * 0.4f);
			}
		}
		int num5 = (int)(this.currentAngle / this.deltaAngle + 0.5f);
		if (this.lastPlayedAudioTickPage != num5)
		{
			this.lastPlayedAudioTickPage = num5;
			this.audioSource.GTPlay();
		}
		float num6 = 0.005f;
		if (Math.Abs(this.lastAngle - this.currentAngle) > num6)
		{
			this.UpdateVisuals();
		}
		this.lastAngle = this.currentAngle;
	}

	// Token: 0x060033D1 RID: 13265 RVA: 0x0011C8E3 File Offset: 0x0011AAE3
	public void SetRotationSpeed(float speed)
	{
		this.rotSpeedMult = Mathf.Sign(speed) * Mathf.Pow(Mathf.Abs(speed), 2f);
	}

	// Token: 0x060033D2 RID: 13266 RVA: 0x0011C902 File Offset: 0x0011AB02
	public void SetTargetShelf(int shelf)
	{
		this.currentAngle += (float)(shelf - this.targetPage) * this.deltaAngle;
		this.targetPage = shelf;
	}

	// Token: 0x060033D3 RID: 13267 RVA: 0x0011C928 File Offset: 0x0011AB28
	public void SetTargetAngle(float angle)
	{
		this.currentAngle = angle;
	}

	// Token: 0x060033D4 RID: 13268 RVA: 0x0011C934 File Offset: 0x0011AB34
	public void UpdateVisuals()
	{
		this.rotationWheel.localRotation = Quaternion.Euler(-this.currentAngle + 7.5f, 0f, 0f);
		float num = this.deltaAngle;
		int count = this.shelfNames.Count;
		float num2 = this.currentAngle / this.deltaAngle;
		for (int i = 0; i < this.shelfNames.Count; i++)
		{
			float num3 = ((float)i - num2) * this.deltaAngle + this.pointerOffsetAngle;
			float num4 = num3 * 3.1415927f / 180f;
			float num5 = Mathf.Cos(num4);
			float num6 = Mathf.Sin(num4);
			Quaternion quaternion = Quaternion.Euler(90f - num3, 180f, 0f);
			Vector3 vector = new Vector3(this.textHorizOffset, num5 * this.wheelTextRadius, num6 * this.wheelTextRadius);
			this.shelfNames[i].transform.rotation = base.transform.TransformRotation(quaternion);
			this.shelfNames[i].transform.position = base.transform.TransformPoint(vector);
			this.shelfNames[i].color = ((Math.Abs(num2 - (float)i) < 0.5f) ? Color.green : Color.white);
		}
	}

	// Token: 0x04004374 RID: 17268
	private List<TMP_Text> shelfNames = new List<TMP_Text>();

	// Token: 0x04004375 RID: 17269
	public TMP_Text templateText;

	// Token: 0x04004376 RID: 17270
	public float deltaAngle;

	// Token: 0x04004377 RID: 17271
	public float pointerOffsetAngle;

	// Token: 0x04004378 RID: 17272
	public float wheelTextRadius;

	// Token: 0x04004379 RID: 17273
	public float textHorizOffset = -0.0375f;

	// Token: 0x0400437A RID: 17274
	public float rotSpeed = 60f;

	// Token: 0x0400437B RID: 17275
	public bool isBeingDrivenRemotely;

	// Token: 0x0400437C RID: 17276
	public AudioSource audioSource;

	// Token: 0x0400437D RID: 17277
	public int lastPlayedAudioTickPage = -1;

	// Token: 0x0400437E RID: 17278
	public float wheelTextPairOffset = 0.0025f;

	// Token: 0x0400437F RID: 17279
	public Transform rotationWheel;

	// Token: 0x04004380 RID: 17280
	public float lastAngle = -1000f;

	// Token: 0x04004382 RID: 17282
	[NonSerialized]
	public int targetPage;

	// Token: 0x04004383 RID: 17283
	[NonSerialized]
	public float currentAngle;

	// Token: 0x04004384 RID: 17284
	private float rotSpeedMult;
}
