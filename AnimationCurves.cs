using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AE4 RID: 2788
public static class AnimationCurves
{
	// Token: 0x170006AE RID: 1710
	// (get) Token: 0x06004785 RID: 18309 RVA: 0x00181EA4 File Offset: 0x001800A4
	public static AnimationCurve EaseInQuad
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 2.000003f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x170006AF RID: 1711
	// (get) Token: 0x06004786 RID: 18310 RVA: 0x00181F10 File Offset: 0x00180110
	public static AnimationCurve EaseOutQuad
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 2.000003f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x170006B0 RID: 1712
	// (get) Token: 0x06004787 RID: 18311 RVA: 0x00181F7C File Offset: 0x0018017C
	public static AnimationCurve EaseInOutQuad
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333334f),
				new Keyframe(0.5f, 0.5f, 1.999994f, 1.999994f, 0.333334f, 0.333334f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333334f, 0f)
			});
		}
	}

	// Token: 0x170006B1 RID: 1713
	// (get) Token: 0x06004788 RID: 18312 RVA: 0x00182014 File Offset: 0x00180214
	public static AnimationCurve EaseInCubic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 3.000003f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x170006B2 RID: 1714
	// (get) Token: 0x06004789 RID: 18313 RVA: 0x00182080 File Offset: 0x00180280
	public static AnimationCurve EaseOutCubic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 3.000003f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x170006B3 RID: 1715
	// (get) Token: 0x0600478A RID: 18314 RVA: 0x001820EC File Offset: 0x001802EC
	public static AnimationCurve EaseInOutCubic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333334f),
				new Keyframe(0.5f, 0.5f, 2.999994f, 2.999994f, 0.333334f, 0.333334f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333334f, 0f)
			});
		}
	}

	// Token: 0x170006B4 RID: 1716
	// (get) Token: 0x0600478B RID: 18315 RVA: 0x00182184 File Offset: 0x00180384
	public static AnimationCurve EaseInQuart
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.0139424f, 0f, 0.434789f),
				new Keyframe(1f, 1f, 3.985819f, 0f, 0.269099f, 0f)
			});
		}
	}

	// Token: 0x170006B5 RID: 1717
	// (get) Token: 0x0600478C RID: 18316 RVA: 0x001821F0 File Offset: 0x001803F0
	public static AnimationCurve EaseOutQuart
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 3.985823f, 0f, 0.269099f),
				new Keyframe(1f, 1f, 0.01394233f, 0f, 0.434789f, 0f)
			});
		}
	}

	// Token: 0x170006B6 RID: 1718
	// (get) Token: 0x0600478D RID: 18317 RVA: 0x0018225C File Offset: 0x0018045C
	public static AnimationCurve EaseInOutQuart
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.01394243f, 0f, 0.434788f),
				new Keyframe(0.5f, 0.5f, 3.985842f, 3.985834f, 0.269098f, 0.269098f),
				new Keyframe(1f, 1f, 0.0139425f, 0f, 0.434788f, 0f)
			});
		}
	}

	// Token: 0x170006B7 RID: 1719
	// (get) Token: 0x0600478E RID: 18318 RVA: 0x001822F4 File Offset: 0x001804F4
	public static AnimationCurve EaseInQuint
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.02411811f, 0f, 0.519568f),
				new Keyframe(1f, 1f, 4.951815f, 0f, 0.225963f, 0f)
			});
		}
	}

	// Token: 0x170006B8 RID: 1720
	// (get) Token: 0x0600478F RID: 18319 RVA: 0x00182360 File Offset: 0x00180560
	public static AnimationCurve EaseOutQuint
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 4.953289f, 0f, 0.225963f),
				new Keyframe(1f, 1f, 0.02414908f, 0f, 0.518901f, 0f)
			});
		}
	}

	// Token: 0x170006B9 RID: 1721
	// (get) Token: 0x06004790 RID: 18320 RVA: 0x001823CC File Offset: 0x001805CC
	public static AnimationCurve EaseInOutQuint
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.02412004f, 0f, 0.519568f),
				new Keyframe(0.5f, 0.5f, 4.951789f, 4.953269f, 0.225964f, 0.225964f),
				new Keyframe(1f, 1f, 0.02415099f, 0f, 0.5189019f, 0f)
			});
		}
	}

	// Token: 0x170006BA RID: 1722
	// (get) Token: 0x06004791 RID: 18321 RVA: 0x00182464 File Offset: 0x00180664
	public static AnimationCurve EaseInSine
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, -0.001208493f, 0f, 0.36078f),
				new Keyframe(1f, 1f, 1.572508f, 0f, 0.326514f, 0f)
			});
		}
	}

	// Token: 0x170006BB RID: 1723
	// (get) Token: 0x06004792 RID: 18322 RVA: 0x001824D0 File Offset: 0x001806D0
	public static AnimationCurve EaseOutSine
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 1.573552f, 0f, 0.330931f),
				new Keyframe(1f, 1f, -0.0009282457f, 0f, 0.358689f, 0f)
			});
		}
	}

	// Token: 0x170006BC RID: 1724
	// (get) Token: 0x06004793 RID: 18323 RVA: 0x0018253C File Offset: 0x0018073C
	public static AnimationCurve EaseInOutSine
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, -0.001202949f, 0f, 0.36078f),
				new Keyframe(0.5f, 0.5f, 1.572508f, 1.573372f, 0.326514f, 0.33093f),
				new Keyframe(1f, 1f, -0.0009312395f, 0f, 0.358688f, 0f)
			});
		}
	}

	// Token: 0x170006BD RID: 1725
	// (get) Token: 0x06004794 RID: 18324 RVA: 0x001825D4 File Offset: 0x001807D4
	public static AnimationCurve EaseInExpo
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.03124388f, 0f, 0.636963f),
				new Keyframe(1f, 1f, 6.815432f, 0f, 0.155667f, 0f)
			});
		}
	}

	// Token: 0x170006BE RID: 1726
	// (get) Token: 0x06004795 RID: 18325 RVA: 0x00182640 File Offset: 0x00180840
	public static AnimationCurve EaseOutExpo
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 6.815433f, 0f, 0.155667f),
				new Keyframe(1f, 1f, 0.03124354f, 0f, 0.636963f, 0f)
			});
		}
	}

	// Token: 0x170006BF RID: 1727
	// (get) Token: 0x06004796 RID: 18326 RVA: 0x001826AC File Offset: 0x001808AC
	public static AnimationCurve EaseInOutExpo
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.03124509f, 0f, 0.636964f),
				new Keyframe(0.5f, 0.5f, 6.815477f, 6.815476f, 0.155666f, 0.155666f),
				new Keyframe(1f, 1f, 0.03124377f, 0f, 0.636964f, 0f)
			});
		}
	}

	// Token: 0x170006C0 RID: 1728
	// (get) Token: 0x06004797 RID: 18327 RVA: 0x00182744 File Offset: 0x00180944
	public static AnimationCurve EaseInCirc
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.002162338f, 0f, 0.55403f),
				new Keyframe(1f, 1f, 459.267f, 0f, 0.001197994f, 0f)
			});
		}
	}

	// Token: 0x170006C1 RID: 1729
	// (get) Token: 0x06004798 RID: 18328 RVA: 0x001827B0 File Offset: 0x001809B0
	public static AnimationCurve EaseOutCirc
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 461.7679f, 0f, 0.001198f),
				new Keyframe(1f, 1f, 0.00216235f, 0f, 0.554024f, 0f)
			});
		}
	}

	// Token: 0x170006C2 RID: 1730
	// (get) Token: 0x06004799 RID: 18329 RVA: 0x0018281C File Offset: 0x00180A1C
	public static AnimationCurve EaseInOutCirc
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.002162353f, 0f, 0.554026f),
				new Keyframe(0.5f, 0.5f, 461.7703f, 461.7474f, 0.001197994f, 0.001198053f),
				new Keyframe(1f, 1f, 0.00216245f, 0f, 0.554026f, 0f)
			});
		}
	}

	// Token: 0x170006C3 RID: 1731
	// (get) Token: 0x0600479A RID: 18330 RVA: 0x001828B4 File Offset: 0x00180AB4
	public static AnimationCurve EaseInBounce
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.6874897f, 0f, 0.3333663f),
				new Keyframe(0.0909f, 0f, -0.687694f, 1.374792f, 0.3332673f, 0.3334159f),
				new Keyframe(0.2727f, 0f, -1.375608f, 2.749388f, 0.3332179f, 0.3333489f),
				new Keyframe(0.6364f, 0f, -2.749183f, 5.501642f, 0.3333737f, 0.3332673f),
				new Keyframe(1f, 1f, 0f, 0f, 0.3333663f, 0f)
			});
		}
	}

	// Token: 0x170006C4 RID: 1732
	// (get) Token: 0x0600479B RID: 18331 RVA: 0x001829A0 File Offset: 0x00180BA0
	public static AnimationCurve EaseOutBounce
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.3333663f),
				new Keyframe(0.3636f, 1f, 5.501643f, -2.749183f, 0.3332673f, 0.3333737f),
				new Keyframe(0.7273f, 1f, 2.749366f, -1.375609f, 0.3333516f, 0.3332178f),
				new Keyframe(0.9091f, 1f, 1.374792f, -0.6877043f, 0.3334158f, 0.3332673f),
				new Keyframe(1f, 1f, 0.6875f, 0f, 0.3333663f, 0f)
			});
		}
	}

	// Token: 0x170006C5 RID: 1733
	// (get) Token: 0x0600479C RID: 18332 RVA: 0x00182A8C File Offset: 0x00180C8C
	public static AnimationCurve EaseInOutBounce
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.6875001f, 0f, 0.333011f),
				new Keyframe(0.0455f, 0f, -0.6854643f, 1.377057f, 0.334f, 0.3328713f),
				new Keyframe(0.1364f, 0f, -1.373381f, 2.751643f, 0.3337624f, 0.3331683f),
				new Keyframe(0.3182f, 0f, -2.749192f, 5.501634f, 0.3334654f, 0.3332673f),
				new Keyframe(0.5f, 0.5f, 0f, 0f, 0.3333663f, 0.3333663f),
				new Keyframe(0.6818f, 1f, 5.501634f, -2.749191f, 0.3332673f, 0.3334653f),
				new Keyframe(0.8636f, 1f, 2.751642f, -1.37338f, 0.3331683f, 0.3319367f),
				new Keyframe(0.955f, 1f, 1.354673f, -0.7087823f, 0.3365205f, 0.3266002f),
				new Keyframe(1f, 1f, 0.6875f, 0f, 0.3367105f, 0f)
			});
		}
	}

	// Token: 0x170006C6 RID: 1734
	// (get) Token: 0x0600479D RID: 18333 RVA: 0x00182C20 File Offset: 0x00180E20
	public static AnimationCurve EaseInBack
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 4.701583f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x170006C7 RID: 1735
	// (get) Token: 0x0600479E RID: 18334 RVA: 0x00182C8C File Offset: 0x00180E8C
	public static AnimationCurve EaseOutBack
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 4.701584f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x170006C8 RID: 1736
	// (get) Token: 0x0600479F RID: 18335 RVA: 0x00182CF8 File Offset: 0x00180EF8
	public static AnimationCurve EaseInOutBack
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333334f),
				new Keyframe(0.5f, 0.5f, 5.594898f, 5.594899f, 0.333334f, 0.333334f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333334f, 0f)
			});
		}
	}

	// Token: 0x170006C9 RID: 1737
	// (get) Token: 0x060047A0 RID: 18336 RVA: 0x00182D90 File Offset: 0x00180F90
	public static AnimationCurve EaseInElastic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.0143284f, 0f, 1f),
				new Keyframe(0.175f, 0f, 0f, -0.06879552f, 0.008331452f, 0.8916667f),
				new Keyframe(0.475f, 0f, -0.4081632f, -0.5503653f, 0.4083333f, 0.8666668f),
				new Keyframe(0.775f, 0f, -3.26241f, -4.402922f, 0.3916665f, 0.5916666f),
				new Keyframe(1f, 1f, 12.51956f, 0f, 0.5916666f, 0f)
			});
		}
	}

	// Token: 0x170006CA RID: 1738
	// (get) Token: 0x060047A1 RID: 18337 RVA: 0x00182E7C File Offset: 0x0018107C
	public static AnimationCurve EaseOutElastic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 12.51956f, 0f, 0.5916667f),
				new Keyframe(0.225f, 1f, -4.402922f, -3.262408f, 0.5916666f, 0.3916667f),
				new Keyframe(0.525f, 1f, -0.5503654f, -0.4081634f, 0.8666667f, 0.4083333f),
				new Keyframe(0.825f, 1f, -0.06879558f, 0f, 0.8916666f, 0.008331367f),
				new Keyframe(1f, 1f, 0.01432861f, 0f, 1f, 0f)
			});
		}
	}

	// Token: 0x170006CB RID: 1739
	// (get) Token: 0x060047A2 RID: 18338 RVA: 0x00182F68 File Offset: 0x00181168
	public static AnimationCurve EaseInOutElastic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.01433143f, 0f, 1f),
				new Keyframe(0.0875f, 0f, 0f, -0.06879253f, 0.008331452f, 0.8916667f),
				new Keyframe(0.2375f, 0f, -0.4081632f, -0.5503692f, 0.4083333f, 0.8666668f),
				new Keyframe(0.3875f, 0f, -3.262419f, -4.402895f, 0.3916665f, 0.5916712f),
				new Keyframe(0.5f, 0.5f, 12.51967f, 12.51958f, 0.5916621f, 0.5916664f),
				new Keyframe(0.6125f, 1f, -4.402927f, -3.262402f, 0.5916669f, 0.3916666f),
				new Keyframe(0.7625f, 1f, -0.5503691f, -0.4081627f, 0.8666668f, 0.4083335f),
				new Keyframe(0.9125f, 1f, -0.06879289f, 0f, 0.8916666f, 0.008331029f),
				new Keyframe(1f, 1f, 0.01432828f, 0f, 1f, 0f)
			});
		}
	}

	// Token: 0x170006CC RID: 1740
	// (get) Token: 0x060047A3 RID: 18339 RVA: 0x001830FC File Offset: 0x001812FC
	public static AnimationCurve Spring
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 3.582263f, 0f, 0.2385296f),
				new Keyframe(0.336583f, 0.828268f, 1.767519f, 1.767491f, 0.4374225f, 0.2215123f),
				new Keyframe(0.550666f, 1.079651f, 0.3095257f, 0.3095275f, 0.4695607f, 0.4154884f),
				new Keyframe(0.779498f, 0.974607f, -0.2321364f, -0.2321428f, 0.3585643f, 0.3623514f),
				new Keyframe(0.897999f, 1.003668f, 0.2797853f, 0.2797431f, 0.3331026f, 0.3306926f),
				new Keyframe(1f, 1f, -0.2023914f, 0f, 0.3296829f, 0f)
			});
		}
	}

	// Token: 0x170006CD RID: 1741
	// (get) Token: 0x060047A4 RID: 18340 RVA: 0x00183210 File Offset: 0x00181410
	public static AnimationCurve Linear
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 1f, 0f, 0f),
				new Keyframe(1f, 1f, 1f, 0f, 0f, 0f)
			});
		}
	}

	// Token: 0x170006CE RID: 1742
	// (get) Token: 0x060047A5 RID: 18341 RVA: 0x0018327C File Offset: 0x0018147C
	public static AnimationCurve Step
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0f),
				new Keyframe(0.5f, 0f, 0f, 0f, 0f, 0f),
				new Keyframe(0.5f, 1f, 0f, 0f, 0f, 0f),
				new Keyframe(1f, 1f, 0f, 0f, 0f, 0f)
			});
		}
	}

	// Token: 0x060047A6 RID: 18342 RVA: 0x0018333C File Offset: 0x0018153C
	static AnimationCurves()
	{
		Dictionary<AnimationCurves.EaseType, AnimationCurve> dictionary = new Dictionary<AnimationCurves.EaseType, AnimationCurve>();
		dictionary[AnimationCurves.EaseType.EaseInQuad] = AnimationCurves.EaseInQuad;
		dictionary[AnimationCurves.EaseType.EaseOutQuad] = AnimationCurves.EaseOutQuad;
		dictionary[AnimationCurves.EaseType.EaseInOutQuad] = AnimationCurves.EaseInOutQuad;
		dictionary[AnimationCurves.EaseType.EaseInCubic] = AnimationCurves.EaseInCubic;
		dictionary[AnimationCurves.EaseType.EaseOutCubic] = AnimationCurves.EaseOutCubic;
		dictionary[AnimationCurves.EaseType.EaseInOutCubic] = AnimationCurves.EaseInOutCubic;
		dictionary[AnimationCurves.EaseType.EaseInQuart] = AnimationCurves.EaseInQuart;
		dictionary[AnimationCurves.EaseType.EaseOutQuart] = AnimationCurves.EaseOutQuart;
		dictionary[AnimationCurves.EaseType.EaseInOutQuart] = AnimationCurves.EaseInOutQuart;
		dictionary[AnimationCurves.EaseType.EaseInQuint] = AnimationCurves.EaseInQuint;
		dictionary[AnimationCurves.EaseType.EaseOutQuint] = AnimationCurves.EaseOutQuint;
		dictionary[AnimationCurves.EaseType.EaseInOutQuint] = AnimationCurves.EaseInOutQuint;
		dictionary[AnimationCurves.EaseType.EaseInSine] = AnimationCurves.EaseInSine;
		dictionary[AnimationCurves.EaseType.EaseOutSine] = AnimationCurves.EaseOutSine;
		dictionary[AnimationCurves.EaseType.EaseInOutSine] = AnimationCurves.EaseInOutSine;
		dictionary[AnimationCurves.EaseType.EaseInExpo] = AnimationCurves.EaseInExpo;
		dictionary[AnimationCurves.EaseType.EaseOutExpo] = AnimationCurves.EaseOutExpo;
		dictionary[AnimationCurves.EaseType.EaseInOutExpo] = AnimationCurves.EaseInOutExpo;
		dictionary[AnimationCurves.EaseType.EaseInCirc] = AnimationCurves.EaseInCirc;
		dictionary[AnimationCurves.EaseType.EaseOutCirc] = AnimationCurves.EaseOutCirc;
		dictionary[AnimationCurves.EaseType.EaseInOutCirc] = AnimationCurves.EaseInOutCirc;
		dictionary[AnimationCurves.EaseType.EaseInBounce] = AnimationCurves.EaseInBounce;
		dictionary[AnimationCurves.EaseType.EaseOutBounce] = AnimationCurves.EaseOutBounce;
		dictionary[AnimationCurves.EaseType.EaseInOutBounce] = AnimationCurves.EaseInOutBounce;
		dictionary[AnimationCurves.EaseType.EaseInBack] = AnimationCurves.EaseInBack;
		dictionary[AnimationCurves.EaseType.EaseOutBack] = AnimationCurves.EaseOutBack;
		dictionary[AnimationCurves.EaseType.EaseInOutBack] = AnimationCurves.EaseInOutBack;
		dictionary[AnimationCurves.EaseType.EaseInElastic] = AnimationCurves.EaseInElastic;
		dictionary[AnimationCurves.EaseType.EaseOutElastic] = AnimationCurves.EaseOutElastic;
		dictionary[AnimationCurves.EaseType.EaseInOutElastic] = AnimationCurves.EaseInOutElastic;
		dictionary[AnimationCurves.EaseType.Spring] = AnimationCurves.Spring;
		dictionary[AnimationCurves.EaseType.Linear] = AnimationCurves.Linear;
		dictionary[AnimationCurves.EaseType.Step] = AnimationCurves.Step;
		AnimationCurves.gEaseTypeToCurve = dictionary;
	}

	// Token: 0x060047A7 RID: 18343 RVA: 0x001834F8 File Offset: 0x001816F8
	public static AnimationCurve GetCurveForEase(AnimationCurves.EaseType ease)
	{
		return AnimationCurves.gEaseTypeToCurve[ease];
	}

	// Token: 0x04005A0F RID: 23055
	private static Dictionary<AnimationCurves.EaseType, AnimationCurve> gEaseTypeToCurve;

	// Token: 0x02000AE5 RID: 2789
	public enum EaseType
	{
		// Token: 0x04005A11 RID: 23057
		EaseInQuad = 1,
		// Token: 0x04005A12 RID: 23058
		EaseOutQuad,
		// Token: 0x04005A13 RID: 23059
		EaseInOutQuad,
		// Token: 0x04005A14 RID: 23060
		EaseInCubic,
		// Token: 0x04005A15 RID: 23061
		EaseOutCubic,
		// Token: 0x04005A16 RID: 23062
		EaseInOutCubic,
		// Token: 0x04005A17 RID: 23063
		EaseInQuart,
		// Token: 0x04005A18 RID: 23064
		EaseOutQuart,
		// Token: 0x04005A19 RID: 23065
		EaseInOutQuart,
		// Token: 0x04005A1A RID: 23066
		EaseInQuint,
		// Token: 0x04005A1B RID: 23067
		EaseOutQuint,
		// Token: 0x04005A1C RID: 23068
		EaseInOutQuint,
		// Token: 0x04005A1D RID: 23069
		EaseInSine,
		// Token: 0x04005A1E RID: 23070
		EaseOutSine,
		// Token: 0x04005A1F RID: 23071
		EaseInOutSine,
		// Token: 0x04005A20 RID: 23072
		EaseInExpo,
		// Token: 0x04005A21 RID: 23073
		EaseOutExpo,
		// Token: 0x04005A22 RID: 23074
		EaseInOutExpo,
		// Token: 0x04005A23 RID: 23075
		EaseInCirc,
		// Token: 0x04005A24 RID: 23076
		EaseOutCirc,
		// Token: 0x04005A25 RID: 23077
		EaseInOutCirc,
		// Token: 0x04005A26 RID: 23078
		EaseInBounce,
		// Token: 0x04005A27 RID: 23079
		EaseOutBounce,
		// Token: 0x04005A28 RID: 23080
		EaseInOutBounce,
		// Token: 0x04005A29 RID: 23081
		EaseInBack,
		// Token: 0x04005A2A RID: 23082
		EaseOutBack,
		// Token: 0x04005A2B RID: 23083
		EaseInOutBack,
		// Token: 0x04005A2C RID: 23084
		EaseInElastic,
		// Token: 0x04005A2D RID: 23085
		EaseOutElastic,
		// Token: 0x04005A2E RID: 23086
		EaseInOutElastic,
		// Token: 0x04005A2F RID: 23087
		Spring,
		// Token: 0x04005A30 RID: 23088
		Linear,
		// Token: 0x04005A31 RID: 23089
		Step
	}
}
