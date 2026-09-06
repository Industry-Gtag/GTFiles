using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001353 RID: 4947
	public class ParticleModifierCosmetic : MonoBehaviour
	{
		// Token: 0x06007BEB RID: 31723 RVA: 0x00287D34 File Offset: 0x00285F34
		private void Awake()
		{
			this.StoreOriginalValues();
			this.currentIndex = -1;
		}

		// Token: 0x06007BEC RID: 31724 RVA: 0x00287D43 File Offset: 0x00285F43
		private void OnValidate()
		{
			this.StoreOriginalValues();
		}

		// Token: 0x06007BED RID: 31725 RVA: 0x00287D43 File Offset: 0x00285F43
		private void OnEnable()
		{
			this.StoreOriginalValues();
		}

		// Token: 0x06007BEE RID: 31726 RVA: 0x00287D4B File Offset: 0x00285F4B
		private void OnDisable()
		{
			this.ResetToOriginal();
		}

		// Token: 0x06007BEF RID: 31727 RVA: 0x00287D54 File Offset: 0x00285F54
		private void StoreOriginalValues()
		{
			if (this.ps == null)
			{
				return;
			}
			ParticleSystem.MainModule main = this.ps.main;
			this.originalStartSize = main.startSize.constant;
			this.originalStartColor = main.startColor.color;
		}

		// Token: 0x06007BF0 RID: 31728 RVA: 0x00287DA6 File Offset: 0x00285FA6
		public void ApplySetting(ParticleSettingsSO setting)
		{
			this.SetStartSize(setting.startSize);
			this.SetStartColor(setting.startColor);
		}

		// Token: 0x06007BF1 RID: 31729 RVA: 0x00287DC0 File Offset: 0x00285FC0
		public void ApplySettingLerp(ParticleSettingsSO setting)
		{
			this.LerpStartSize(setting.startSize);
			this.LerpStartColor(setting.startColor);
		}

		// Token: 0x06007BF2 RID: 31730 RVA: 0x00287DDC File Offset: 0x00285FDC
		public void MoveToNextSetting()
		{
			this.currentIndex++;
			if (this.currentIndex > -1 && this.currentIndex < this.particleSettings.Length)
			{
				ParticleSettingsSO particleSettingsSO = this.particleSettings[this.currentIndex];
				this.ApplySetting(particleSettingsSO);
			}
		}

		// Token: 0x06007BF3 RID: 31731 RVA: 0x00287E28 File Offset: 0x00286028
		public void MoveToNextSettingLerp()
		{
			this.currentIndex++;
			if (this.currentIndex > -1 && this.currentIndex < this.particleSettings.Length)
			{
				ParticleSettingsSO particleSettingsSO = this.particleSettings[this.currentIndex];
				this.ApplySettingLerp(particleSettingsSO);
			}
		}

		// Token: 0x06007BF4 RID: 31732 RVA: 0x00287E71 File Offset: 0x00286071
		public void ResetSettings()
		{
			this.currentIndex = -1;
			this.ResetToOriginal();
		}

		// Token: 0x06007BF5 RID: 31733 RVA: 0x00287E80 File Offset: 0x00286080
		public void MoveToSettingIndex(int index)
		{
			if (index > -1 && index < this.particleSettings.Length)
			{
				ParticleSettingsSO particleSettingsSO = this.particleSettings[index];
				this.ApplySetting(particleSettingsSO);
			}
		}

		// Token: 0x06007BF6 RID: 31734 RVA: 0x00287EAC File Offset: 0x002860AC
		public void MoveToSettingIndexLerp(int index)
		{
			if (index > -1 && index < this.particleSettings.Length)
			{
				ParticleSettingsSO particleSettingsSO = this.particleSettings[index];
				this.ApplySettingLerp(particleSettingsSO);
			}
		}

		// Token: 0x06007BF7 RID: 31735 RVA: 0x00287ED8 File Offset: 0x002860D8
		public void SetStartSize(float size)
		{
			if (this.ps == null)
			{
				return;
			}
			this.ps.main.startSize = size;
			this.targetSize = null;
		}

		// Token: 0x06007BF8 RID: 31736 RVA: 0x00287F1C File Offset: 0x0028611C
		public void IncreaseStartSize(float delta)
		{
			if (this.ps == null)
			{
				return;
			}
			ParticleSystem.MainModule main = this.ps.main;
			float constant = main.startSize.constant;
			main.startSize = constant + delta;
			this.targetSize = null;
		}

		// Token: 0x06007BF9 RID: 31737 RVA: 0x00287F70 File Offset: 0x00286170
		public void LerpStartSize(float size)
		{
			if (this.ps == null)
			{
				return;
			}
			if (Mathf.Abs(this.ps.main.startSize.constant - size) < 0.01f)
			{
				return;
			}
			this.targetSize = new float?(size);
		}

		// Token: 0x06007BFA RID: 31738 RVA: 0x00287FC4 File Offset: 0x002861C4
		public void SetStartColor(Color color)
		{
			if (this.ps == null)
			{
				return;
			}
			this.ps.main.startColor = color;
			this.targetColor = null;
		}

		// Token: 0x06007BFB RID: 31739 RVA: 0x00288008 File Offset: 0x00286208
		public void LerpStartColor(Color color)
		{
			if (this.ps == null)
			{
				return;
			}
			Color color2 = this.ps.main.startColor.color;
			if (this.IsColorApproximatelyEqual(color2, color, 0.0001f))
			{
				return;
			}
			this.targetColor = new Color?(color);
		}

		// Token: 0x06007BFC RID: 31740 RVA: 0x0028805C File Offset: 0x0028625C
		public void SetStartValues(float size, Color color)
		{
			this.SetStartSize(size);
			this.SetStartColor(color);
		}

		// Token: 0x06007BFD RID: 31741 RVA: 0x0028806C File Offset: 0x0028626C
		public void LerpStartValues(float size, Color color)
		{
			this.LerpStartSize(size);
			this.LerpStartColor(color);
		}

		// Token: 0x06007BFE RID: 31742 RVA: 0x0028807C File Offset: 0x0028627C
		private void Update()
		{
			if (this.ps == null)
			{
				return;
			}
			ParticleSystem.MainModule main = this.ps.main;
			if (this.targetSize != null)
			{
				float num = Mathf.Lerp(main.startSize.constant, this.targetSize.Value, Time.deltaTime * this.transitionSpeed);
				main.startSize = num;
				if (Mathf.Abs(num - this.targetSize.Value) < 0.01f)
				{
					main.startSize = this.targetSize.Value;
					this.targetSize = null;
				}
			}
			if (this.targetColor != null)
			{
				Color color = Color.Lerp(main.startColor.color, this.targetColor.Value, Time.deltaTime * this.transitionSpeed);
				main.startColor = color;
				if (this.IsColorApproximatelyEqual(color, this.targetColor.Value, 0.0001f))
				{
					main.startColor = this.targetColor.Value;
					this.targetColor = null;
				}
			}
		}

		// Token: 0x06007BFF RID: 31743 RVA: 0x002881AC File Offset: 0x002863AC
		[ContextMenu("Reset To Original")]
		public void ResetToOriginal()
		{
			if (this.ps == null)
			{
				return;
			}
			this.targetSize = null;
			this.targetColor = null;
			ParticleSystem.MainModule main = this.ps.main;
			main.startSize = this.originalStartSize;
			main.startColor = this.originalStartColor;
		}

		// Token: 0x06007C00 RID: 31744 RVA: 0x00288210 File Offset: 0x00286410
		private bool IsColorApproximatelyEqual(Color a, Color b, float threshold = 0.0001f)
		{
			float num = a.r - b.r;
			float num2 = a.g - b.g;
			float num3 = a.b - b.b;
			float num4 = a.a - b.a;
			return num * num + num2 * num2 + num3 * num3 + num4 * num4 < threshold;
		}

		// Token: 0x04008E11 RID: 36369
		[SerializeField]
		private ParticleSystem ps;

		// Token: 0x04008E12 RID: 36370
		[Tooltip("For calling gradual functions only")]
		[SerializeField]
		private float transitionSpeed = 5f;

		// Token: 0x04008E13 RID: 36371
		public ParticleSettingsSO[] particleSettings = new ParticleSettingsSO[0];

		// Token: 0x04008E14 RID: 36372
		private float originalStartSize;

		// Token: 0x04008E15 RID: 36373
		private Color originalStartColor;

		// Token: 0x04008E16 RID: 36374
		private float? targetSize;

		// Token: 0x04008E17 RID: 36375
		private Color? targetColor;

		// Token: 0x04008E18 RID: 36376
		private int currentIndex;
	}
}
