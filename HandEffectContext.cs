using System;
using System.Collections.Generic;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x020004FC RID: 1276
[Serializable]
internal class HandEffectContext : IFXEffectContextObject
{
	// Token: 0x17000367 RID: 871
	// (get) Token: 0x06001FED RID: 8173 RVA: 0x000AC623 File Offset: 0x000AA823
	public List<int> PrefabPoolIds
	{
		get
		{
			return this.prefabHashes;
		}
	}

	// Token: 0x17000368 RID: 872
	// (get) Token: 0x06001FEE RID: 8174 RVA: 0x000AC62B File Offset: 0x000AA82B
	public Vector3 Position
	{
		get
		{
			return this.position;
		}
	}

	// Token: 0x17000369 RID: 873
	// (get) Token: 0x06001FEF RID: 8175 RVA: 0x000AC633 File Offset: 0x000AA833
	public Quaternion Rotation
	{
		get
		{
			return this.rotation;
		}
	}

	// Token: 0x1700036A RID: 874
	// (get) Token: 0x06001FF0 RID: 8176 RVA: 0x000AC63B File Offset: 0x000AA83B
	public float Speed
	{
		get
		{
			return this.speed;
		}
	}

	// Token: 0x1700036B RID: 875
	// (get) Token: 0x06001FF1 RID: 8177 RVA: 0x000AC643 File Offset: 0x000AA843
	public Color Color
	{
		get
		{
			return this.color;
		}
	}

	// Token: 0x1700036C RID: 876
	// (get) Token: 0x06001FF2 RID: 8178 RVA: 0x000AC64B File Offset: 0x000AA84B
	public AudioSource SoundSource
	{
		get
		{
			return this.handSoundSource;
		}
	}

	// Token: 0x1700036D RID: 877
	// (get) Token: 0x06001FF3 RID: 8179 RVA: 0x000AC653 File Offset: 0x000AA853
	public AudioClip Sound
	{
		get
		{
			return this.soundFX;
		}
	}

	// Token: 0x1700036E RID: 878
	// (get) Token: 0x06001FF4 RID: 8180 RVA: 0x000AC65B File Offset: 0x000AA85B
	public float Volume
	{
		get
		{
			return this.soundVolume;
		}
	}

	// Token: 0x1700036F RID: 879
	// (get) Token: 0x06001FF5 RID: 8181 RVA: 0x000AC663 File Offset: 0x000AA863
	public float Pitch
	{
		get
		{
			return this.soundPitch;
		}
	}

	// Token: 0x06001FF6 RID: 8182 RVA: 0x000AC66B File Offset: 0x000AA86B
	public void AddFXPrefab(int hash)
	{
		this.prefabHashes.Add(hash);
	}

	// Token: 0x06001FF7 RID: 8183 RVA: 0x000AC67C File Offset: 0x000AA87C
	public void RemoveFXPrefab(int hash)
	{
		int num = this.prefabHashes.IndexOf(hash, 2);
		if (num >= 2)
		{
			this.prefabHashes.RemoveAt(num);
		}
	}

	// Token: 0x17000370 RID: 880
	// (get) Token: 0x06001FF8 RID: 8184 RVA: 0x000AC6A7 File Offset: 0x000AA8A7
	// (set) Token: 0x06001FF9 RID: 8185 RVA: 0x000AC6B2 File Offset: 0x000AA8B2
	public bool SeparateUpTapCooldown
	{
		get
		{
			return this.separateUpTapCooldownCount > 0;
		}
		set
		{
			this.separateUpTapCooldownCount = Mathf.Max(this.separateUpTapCooldownCount + (value ? 1 : (-1)), 0);
		}
	}

	// Token: 0x17000371 RID: 881
	// (get) Token: 0x06001FFA RID: 8186 RVA: 0x000AC6CE File Offset: 0x000AA8CE
	// (set) Token: 0x06001FFB RID: 8187 RVA: 0x000AC6E0 File Offset: 0x000AA8E0
	public HandTapOverrides DownTapOverrides
	{
		get
		{
			return this.downTapOverrides ?? this.defaultDownTapOverrides;
		}
		set
		{
			this.downTapOverrides = value;
		}
	}

	// Token: 0x17000372 RID: 882
	// (get) Token: 0x06001FFC RID: 8188 RVA: 0x000AC6E9 File Offset: 0x000AA8E9
	// (set) Token: 0x06001FFD RID: 8189 RVA: 0x000AC6FB File Offset: 0x000AA8FB
	public HandTapOverrides UpTapOverrides
	{
		get
		{
			return this.upTapOverrides ?? this.defaultUpTapOverrides;
		}
		set
		{
			this.upTapOverrides = value;
		}
	}

	// Token: 0x14000048 RID: 72
	// (add) Token: 0x06001FFE RID: 8190 RVA: 0x000AC704 File Offset: 0x000AA904
	// (remove) Token: 0x06001FFF RID: 8191 RVA: 0x000AC73C File Offset: 0x000AA93C
	public event Action<HandEffectContext> handTapDown;

	// Token: 0x14000049 RID: 73
	// (add) Token: 0x06002000 RID: 8192 RVA: 0x000AC774 File Offset: 0x000AA974
	// (remove) Token: 0x06002001 RID: 8193 RVA: 0x000AC7AC File Offset: 0x000AA9AC
	public event Action<HandEffectContext> handTapUp;

	// Token: 0x06002002 RID: 8194 RVA: 0x000AC7E1 File Offset: 0x000AA9E1
	public void OnTriggerActions()
	{
		if (this.isDownTap)
		{
			Action<HandEffectContext> action = this.handTapDown;
			if (action == null)
			{
				return;
			}
			action(this);
			return;
		}
		else
		{
			Action<HandEffectContext> action2 = this.handTapUp;
			if (action2 == null)
			{
				return;
			}
			action2(this);
			return;
		}
	}

	// Token: 0x06002003 RID: 8195 RVA: 0x000AC810 File Offset: 0x000AAA10
	public void OnPlayVisualFX(int fxID, GameObject fx)
	{
		FXModifier fxmodifier;
		if (fx.TryGetComponent<FXModifier>(out fxmodifier))
		{
			fxmodifier.UpdateScale(this.soundVolume * ((fxID == GorillaAmbushManager.HandEffectHash) ? GorillaAmbushManager.HandFXScaleModifier : 1f), this.color);
		}
	}

	// Token: 0x06002004 RID: 8196 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPlaySoundFX(AudioSource audioSource)
	{
	}

	// Token: 0x04002ABB RID: 10939
	internal List<int> prefabHashes = new List<int> { -1, -1 };

	// Token: 0x04002ABC RID: 10940
	internal Vector3 position;

	// Token: 0x04002ABD RID: 10941
	internal Quaternion rotation;

	// Token: 0x04002ABE RID: 10942
	internal float speed;

	// Token: 0x04002ABF RID: 10943
	internal Color color = Color.white;

	// Token: 0x04002AC0 RID: 10944
	[SerializeField]
	internal AudioSource handSoundSource;

	// Token: 0x04002AC1 RID: 10945
	internal AudioClip soundFX;

	// Token: 0x04002AC2 RID: 10946
	internal float soundVolume;

	// Token: 0x04002AC3 RID: 10947
	internal float soundPitch;

	// Token: 0x04002AC4 RID: 10948
	internal int separateUpTapCooldownCount;

	// Token: 0x04002AC5 RID: 10949
	[SerializeField]
	internal HandTapOverrides defaultDownTapOverrides;

	// Token: 0x04002AC6 RID: 10950
	internal HandTapOverrides downTapOverrides;

	// Token: 0x04002AC7 RID: 10951
	[SerializeField]
	internal HandTapOverrides defaultUpTapOverrides;

	// Token: 0x04002AC8 RID: 10952
	internal HandTapOverrides upTapOverrides;

	// Token: 0x04002ACB RID: 10955
	internal bool isDownTap;

	// Token: 0x04002ACC RID: 10956
	internal bool isLeftHand;
}
