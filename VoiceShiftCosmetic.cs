using System;
using UnityEngine;

// Token: 0x02000303 RID: 771
public class VoiceShiftCosmetic : MonoBehaviour
{
	// Token: 0x170001F3 RID: 499
	// (get) Token: 0x060013A9 RID: 5033 RVA: 0x00067C8C File Offset: 0x00065E8C
	public bool ModifyPitch
	{
		get
		{
			return this.modifyPitch;
		}
	}

	// Token: 0x170001F4 RID: 500
	// (get) Token: 0x060013AA RID: 5034 RVA: 0x00067C94 File Offset: 0x00065E94
	public bool ModifyVolume
	{
		get
		{
			return this.modifyVolume;
		}
	}

	// Token: 0x170001F5 RID: 501
	// (get) Token: 0x060013AB RID: 5035 RVA: 0x00067C9C File Offset: 0x00065E9C
	public bool IsShifted
	{
		get
		{
			return this.isShifted;
		}
	}

	// Token: 0x170001F6 RID: 502
	// (get) Token: 0x060013AC RID: 5036 RVA: 0x00067CA4 File Offset: 0x00065EA4
	// (set) Token: 0x060013AD RID: 5037 RVA: 0x00067CAC File Offset: 0x00065EAC
	public float Pitch
	{
		get
		{
			return this.pitch;
		}
		set
		{
			if (!this.modifyPitch)
			{
				return;
			}
			float num = Mathf.Clamp(value, 0.6666667f, 1.5f);
			this.pitch = num;
			VRRig vrrig = this.myRig;
			if (vrrig == null)
			{
				return;
			}
			vrrig.SetVoiceShiftCosmeticsDirty();
		}
	}

	// Token: 0x170001F7 RID: 503
	// (get) Token: 0x060013AE RID: 5038 RVA: 0x00067CEA File Offset: 0x00065EEA
	// (set) Token: 0x060013AF RID: 5039 RVA: 0x00067CF4 File Offset: 0x00065EF4
	public float Volume
	{
		get
		{
			return this.volume;
		}
		set
		{
			if (!this.modifyVolume)
			{
				return;
			}
			float num = Mathf.Clamp(value, 0f, 1f);
			this.volume = num;
			VRRig vrrig = this.myRig;
			if (vrrig == null)
			{
				return;
			}
			vrrig.SetVoiceShiftCosmeticsDirty();
		}
	}

	// Token: 0x060013B0 RID: 5040 RVA: 0x00067D34 File Offset: 0x00065F34
	private void OnEnable()
	{
		if (this.myRig == null)
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
		if (this.myRig == null)
		{
			return;
		}
		this.myRig.VoiceShiftCosmetics.Add(this);
		this.myRig.SetVoiceShiftCosmeticsDirty();
	}

	// Token: 0x060013B1 RID: 5041 RVA: 0x00067D80 File Offset: 0x00065F80
	private void OnDisable()
	{
		if (this.myRig == null)
		{
			return;
		}
		this.myRig.VoiceShiftCosmetics.Remove(this);
		this.myRig.SetVoiceShiftCosmeticsDirty();
	}

	// Token: 0x060013B2 RID: 5042 RVA: 0x00067DAE File Offset: 0x00065FAE
	public void StartVoiceShift()
	{
		if (this.isShifted)
		{
			return;
		}
		this.isShifted = true;
		if (this.modifyPitch)
		{
			this.Pitch = this.shiftedPitch;
		}
		if (this.modifyVolume)
		{
			this.Volume = this.shiftedVolume;
		}
	}

	// Token: 0x060013B3 RID: 5043 RVA: 0x00067DE8 File Offset: 0x00065FE8
	public void StopVoiceShift()
	{
		if (!this.isShifted)
		{
			return;
		}
		this.isShifted = false;
		VRRig vrrig = this.myRig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.SetVoiceShiftCosmeticsDirty();
	}

	// Token: 0x060013B4 RID: 5044 RVA: 0x00067E0A File Offset: 0x0006600A
	public void ToggleVoiceShift()
	{
		if (this.isShifted)
		{
			this.StopVoiceShift();
			return;
		}
		this.StartVoiceShift();
	}

	// Token: 0x0400181A RID: 6170
	private const float PITCH_MIN = 0.6666667f;

	// Token: 0x0400181B RID: 6171
	private const float PITCH_MAX = 1.5f;

	// Token: 0x0400181C RID: 6172
	private const float VOLUME_MIN = 0f;

	// Token: 0x0400181D RID: 6173
	private const float VOLUME_MAX = 1f;

	// Token: 0x0400181E RID: 6174
	[SerializeField]
	private bool modifyPitch = true;

	// Token: 0x0400181F RID: 6175
	[SerializeField]
	private bool modifyVolume = true;

	// Token: 0x04001820 RID: 6176
	[Range(0.6666667f, 1.5f)]
	[SerializeField]
	private float shiftedPitch = 1.5f;

	// Token: 0x04001821 RID: 6177
	[Range(0f, 1f)]
	[SerializeField]
	private float shiftedVolume = 1f;

	// Token: 0x04001822 RID: 6178
	private float pitch = 1f;

	// Token: 0x04001823 RID: 6179
	private float volume = 1f;

	// Token: 0x04001824 RID: 6180
	private bool isShifted;

	// Token: 0x04001825 RID: 6181
	private VRRig myRig;
}
