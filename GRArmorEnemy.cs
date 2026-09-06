using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000754 RID: 1876
public class GRArmorEnemy : MonoBehaviour
{
	// Token: 0x06002FA9 RID: 12201 RVA: 0x00103921 File Offset: 0x00101B21
	private void Awake()
	{
		this.SetHp(0);
		this.entity = base.GetComponent<GameEntity>();
	}

	// Token: 0x06002FAA RID: 12202 RVA: 0x00103936 File Offset: 0x00101B36
	public void SetHp(int hp)
	{
		this.hp = hp;
		this.RefreshArmor();
	}

	// Token: 0x06002FAB RID: 12203 RVA: 0x00103948 File Offset: 0x00101B48
	private void RefreshArmor()
	{
		bool flag = this.hp > 0;
		GREnemy.HideRenderers(this.renderers, !flag);
		GREnemy.HideObjects(this.visibleObjects, !flag);
		if (this.armorStateData.Count > 0)
		{
			int num = -1;
			Material material = this.armorStateData[0].mainRendererMaterial;
			for (int i = 0; i < this.armorStateData.Count; i++)
			{
				num = i;
				material = this.armorStateData[i].mainRendererMaterial;
				if (this.hp >= this.armorStateData[i].healthThreshold)
				{
					break;
				}
			}
			if (flag && this.materialSwapRenderer != null && material != this.materialSwapRenderer.material)
			{
				this.materialSwapRenderer.material = material;
				this.SetArmorColor(this.GetArmorColor());
			}
			if (num != -1)
			{
				GREnemy.HideObjects(this.armorStateData[num].visibleObjects, !flag);
				for (int j = 0; j < this.armorStateData[num].hiddenObjects.Count; j++)
				{
					GameObject gameObject = this.armorStateData[num].hiddenObjects[j];
					if (gameObject.activeInHierarchy)
					{
						this.PlayDestroyFx(gameObject.transform.position);
					}
				}
				GREnemy.HideObjects(this.armorStateData[num].hiddenObjects, true);
			}
		}
	}

	// Token: 0x06002FAC RID: 12204 RVA: 0x00103AB3 File Offset: 0x00101CB3
	public void SetArmorColor(Color newColor)
	{
		if (this.renderers != null && this.renderers.Count > 0)
		{
			this.materialSwapRenderer.material.SetColor("_BaseColor", newColor);
		}
	}

	// Token: 0x06002FAD RID: 12205 RVA: 0x00103AE4 File Offset: 0x00101CE4
	public Color GetArmorColor()
	{
		Color color = Color.white;
		if (this.materialSwapRenderer != null)
		{
			color = this.materialSwapRenderer.material.GetColor("_BaseColor");
		}
		return color;
	}

	// Token: 0x06002FAE RID: 12206 RVA: 0x00103B1C File Offset: 0x00101D1C
	public void PlayHitFx(Vector3 position)
	{
		this.PlayFx(this.fxHit, position);
		this.PlaySound(this.hitSound, this.hitSoundVolume, position);
	}

	// Token: 0x06002FAF RID: 12207 RVA: 0x00103B3E File Offset: 0x00101D3E
	public void PlayBlockFx(Vector3 position)
	{
		this.PlayFx(this.fxBlock, position);
		this.PlaySound(this.blockSound, this.blockSoundVolume, position);
	}

	// Token: 0x06002FB0 RID: 12208 RVA: 0x00103B60 File Offset: 0x00101D60
	public void PlayDestroyFx(Vector3 position)
	{
		this.PlayFx(this.fxDestroy, position);
		this.PlaySound(this.destroySound, this.destroySoundVolume, position);
	}

	// Token: 0x06002FB1 RID: 12209 RVA: 0x00103B82 File Offset: 0x00101D82
	private void PlayFx(GameObject fx, Vector3 position)
	{
		if (fx == null)
		{
			return;
		}
		fx.SetActive(false);
		fx.SetActive(true);
	}

	// Token: 0x06002FB2 RID: 12210 RVA: 0x00103B9C File Offset: 0x00101D9C
	private void PlaySound(AudioClip clip, float volume, Vector3 position)
	{
		this.audioSource.clip = clip;
		this.audioSource.volume = volume;
		this.audioSource.Play();
	}

	// Token: 0x06002FB3 RID: 12211 RVA: 0x00103BC4 File Offset: 0x00101DC4
	public void FragmentArmor()
	{
		if (this.entity.IsAuthority())
		{
			float num = 0f;
			for (int i = 0; i < this.numFragmentsWhenShattered; i++)
			{
				num += 360f / (float)this.numFragmentsWhenShattered;
				Quaternion quaternion = Quaternion.Euler(0f, num, this.fragmentLaunchPitch);
				Vector3 vector = quaternion * this.fragmentSpawnOffset;
				this.entity.manager.RequestCreateItem(this.armorFragmentPrefab.name.GetStaticHash(), base.transform.position + vector, quaternion, (long)this.entity.GetNetId());
			}
		}
	}

	// Token: 0x04003CF9 RID: 15609
	[SerializeField]
	private List<Renderer> renderers;

	// Token: 0x04003CFA RID: 15610
	[SerializeField]
	private List<GameObject> visibleObjects;

	// Token: 0x04003CFB RID: 15611
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003CFC RID: 15612
	[SerializeField]
	private GameObject fxHit;

	// Token: 0x04003CFD RID: 15613
	[SerializeField]
	private AudioClip hitSound;

	// Token: 0x04003CFE RID: 15614
	[SerializeField]
	private float hitSoundVolume;

	// Token: 0x04003CFF RID: 15615
	[SerializeField]
	private GameObject fxBlock;

	// Token: 0x04003D00 RID: 15616
	[SerializeField]
	private AudioClip blockSound;

	// Token: 0x04003D01 RID: 15617
	[SerializeField]
	private float blockSoundVolume;

	// Token: 0x04003D02 RID: 15618
	[SerializeField]
	private GameObject fxDestroy;

	// Token: 0x04003D03 RID: 15619
	[SerializeField]
	private AudioClip destroySound;

	// Token: 0x04003D04 RID: 15620
	[SerializeField]
	private float destroySoundVolume;

	// Token: 0x04003D05 RID: 15621
	[SerializeField]
	public List<GRArmorEnemy.GREnemyArmorLevel> armorStateData;

	// Token: 0x04003D06 RID: 15622
	[SerializeField]
	public Renderer materialSwapRenderer;

	// Token: 0x04003D07 RID: 15623
	private GameEntity entity;

	// Token: 0x04003D08 RID: 15624
	public GameObject armorFragmentPrefab;

	// Token: 0x04003D09 RID: 15625
	public Vector3 fragmentSpawnOffset = new Vector3(0f, 0.5f, 0.5f);

	// Token: 0x04003D0A RID: 15626
	public int numFragmentsWhenShattered = 3;

	// Token: 0x04003D0B RID: 15627
	public float fragmentLaunchPitch = 30f;

	// Token: 0x04003D0C RID: 15628
	private int hp;

	// Token: 0x02000755 RID: 1877
	[Serializable]
	public struct GREnemyArmorLevel
	{
		// Token: 0x04003D0D RID: 15629
		public int healthThreshold;

		// Token: 0x04003D0E RID: 15630
		public Material mainRendererMaterial;

		// Token: 0x04003D0F RID: 15631
		public List<GameObject> visibleObjects;

		// Token: 0x04003D10 RID: 15632
		public List<GameObject> hiddenObjects;
	}
}
