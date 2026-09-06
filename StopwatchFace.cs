using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020009F7 RID: 2551
public class StopwatchFace : MonoBehaviour
{
	// Token: 0x1700062D RID: 1581
	// (get) Token: 0x06004170 RID: 16752 RVA: 0x0015C37D File Offset: 0x0015A57D
	public bool watchActive
	{
		get
		{
			return this._watchActive;
		}
	}

	// Token: 0x1700062E RID: 1582
	// (get) Token: 0x06004171 RID: 16753 RVA: 0x0015C385 File Offset: 0x0015A585
	public int millisElapsed
	{
		get
		{
			return this._millisElapsed;
		}
	}

	// Token: 0x1700062F RID: 1583
	// (get) Token: 0x06004172 RID: 16754 RVA: 0x0015C38D File Offset: 0x0015A58D
	public Vector3Int digitsMmSsMs
	{
		get
		{
			return StopwatchFace.ParseDigits(TimeSpan.FromMilliseconds((double)this._millisElapsed));
		}
	}

	// Token: 0x06004173 RID: 16755 RVA: 0x0015C3A0 File Offset: 0x0015A5A0
	public void SetMillisElapsed(int millis, bool updateFace = true)
	{
		this._millisElapsed = millis;
		if (!updateFace)
		{
			return;
		}
		this.UpdateText();
		this.UpdateHand();
	}

	// Token: 0x06004174 RID: 16756 RVA: 0x0015C3B9 File Offset: 0x0015A5B9
	private void Awake()
	{
		this._lerpToZero = new LerpTask<int>();
		this._lerpToZero.onLerp = new Action<int, int, float>(this.OnLerpToZero);
		this._lerpToZero.onLerpEnd = new Action(this.OnLerpEnd);
	}

	// Token: 0x06004175 RID: 16757 RVA: 0x0015C3F4 File Offset: 0x0015A5F4
	private void OnLerpToZero(int a, int b, float t)
	{
		this._millisElapsed = Mathf.FloorToInt(Mathf.Lerp((float)a, (float)b, t * t));
		this.UpdateText();
		this.UpdateHand();
	}

	// Token: 0x06004176 RID: 16758 RVA: 0x0015C419 File Offset: 0x0015A619
	private void OnLerpEnd()
	{
		this.WatchReset(false);
	}

	// Token: 0x06004177 RID: 16759 RVA: 0x0015C419 File Offset: 0x0015A619
	private void OnEnable()
	{
		this.WatchReset(false);
	}

	// Token: 0x06004178 RID: 16760 RVA: 0x0015C419 File Offset: 0x0015A619
	private void OnDisable()
	{
		this.WatchReset(false);
	}

	// Token: 0x06004179 RID: 16761 RVA: 0x0015C424 File Offset: 0x0015A624
	private void Update()
	{
		if (this._lerpToZero.active)
		{
			this._lerpToZero.Update();
			return;
		}
		if (this._watchActive)
		{
			this._millisElapsed += Mathf.FloorToInt(Time.deltaTime * 1000f);
			this.UpdateText();
			this.UpdateHand();
		}
	}

	// Token: 0x0600417A RID: 16762 RVA: 0x0015C47C File Offset: 0x0015A67C
	private static Vector3Int ParseDigits(TimeSpan time)
	{
		int num = (int)time.TotalMinutes % 100;
		double num2 = 60.0 * (time.TotalMinutes - (double)num);
		int num3 = (int)num2;
		int num4 = (int)(100.0 * (num2 - (double)num3));
		num = Math.Clamp(num, 0, 99);
		num3 = Math.Clamp(num3, 0, 59);
		num4 = Math.Clamp(num4, 0, 99);
		return new Vector3Int(num, num3, num4);
	}

	// Token: 0x0600417B RID: 16763 RVA: 0x0015C4E4 File Offset: 0x0015A6E4
	private void UpdateText()
	{
		Vector3Int vector3Int = StopwatchFace.ParseDigits(TimeSpan.FromMilliseconds((double)this._millisElapsed));
		string text = vector3Int.x.ToString("D2");
		string text2 = vector3Int.y.ToString("D2");
		string text3 = vector3Int.z.ToString("D2");
		this._text.text = string.Concat(new string[] { text, ":", text2, ":", text3 });
	}

	// Token: 0x0600417C RID: 16764 RVA: 0x0015C578 File Offset: 0x0015A778
	private void UpdateHand()
	{
		float num = (float)(this._millisElapsed % 60000) / 60000f * 360f;
		this._hand.localEulerAngles = new Vector3(0f, 0f, num);
	}

	// Token: 0x0600417D RID: 16765 RVA: 0x0015C5BA File Offset: 0x0015A7BA
	public void WatchToggle()
	{
		if (!this._watchActive)
		{
			this.WatchStart();
			return;
		}
		this.WatchStop();
	}

	// Token: 0x0600417E RID: 16766 RVA: 0x0015C5D1 File Offset: 0x0015A7D1
	public void WatchStart()
	{
		if (this._lerpToZero.active)
		{
			return;
		}
		this._watchActive = true;
	}

	// Token: 0x0600417F RID: 16767 RVA: 0x0015C5E8 File Offset: 0x0015A7E8
	public void WatchStop()
	{
		if (this._lerpToZero.active)
		{
			return;
		}
		this._watchActive = false;
	}

	// Token: 0x06004180 RID: 16768 RVA: 0x0015C5FF File Offset: 0x0015A7FF
	public void WatchReset()
	{
		this.WatchReset(true);
	}

	// Token: 0x06004181 RID: 16769 RVA: 0x0015C608 File Offset: 0x0015A808
	public void WatchReset(bool doLerp)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (doLerp)
		{
			if (!this._lerpToZero.active)
			{
				this._lerpToZero.Start(this._millisElapsed % 60000, 0, 0.36f);
				return;
			}
		}
		else
		{
			this._watchActive = false;
			this._millisElapsed = 0;
			this.UpdateText();
			this.UpdateHand();
		}
	}

	// Token: 0x0400521C RID: 21020
	[SerializeField]
	private Transform _hand;

	// Token: 0x0400521D RID: 21021
	[SerializeField]
	private Text _text;

	// Token: 0x0400521E RID: 21022
	[Space]
	[SerializeField]
	private StopwatchCosmetic _cosmetic;

	// Token: 0x0400521F RID: 21023
	[Space]
	[SerializeField]
	private AudioClip _audioClick;

	// Token: 0x04005220 RID: 21024
	[SerializeField]
	private AudioClip _audioReset;

	// Token: 0x04005221 RID: 21025
	[SerializeField]
	private AudioClip _audioTick;

	// Token: 0x04005222 RID: 21026
	[Space]
	[NonSerialized]
	private int _millisElapsed;

	// Token: 0x04005223 RID: 21027
	[NonSerialized]
	private bool _watchActive;

	// Token: 0x04005224 RID: 21028
	[NonSerialized]
	private LerpTask<int> _lerpToZero;
}
