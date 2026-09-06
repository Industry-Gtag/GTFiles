using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x0200000C RID: 12
public class ActivateGO : MonoBehaviour
{
	// Token: 0x06000025 RID: 37 RVA: 0x000025D0 File Offset: 0x000007D0
	private void OnEnable()
	{
		this.active = PlayerPrefFlags.Check(this.flag) != this.invertFlag;
		this.SetGOsActive(0);
		PlayerPrefFlags.OnFlagChange = (Action<PlayerPrefFlags.Flag, bool>)Delegate.Combine(PlayerPrefFlags.OnFlagChange, new Action<PlayerPrefFlags.Flag, bool>(this.OnFlagChange));
	}

	// Token: 0x06000026 RID: 38 RVA: 0x00002620 File Offset: 0x00000820
	private void OnDisable()
	{
		PlayerPrefFlags.OnFlagChange = (Action<PlayerPrefFlags.Flag, bool>)Delegate.Remove(PlayerPrefFlags.OnFlagChange, new Action<PlayerPrefFlags.Flag, bool>(this.OnFlagChange));
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00002620 File Offset: 0x00000820
	private void OnDestroy()
	{
		PlayerPrefFlags.OnFlagChange = (Action<PlayerPrefFlags.Flag, bool>)Delegate.Remove(PlayerPrefFlags.OnFlagChange, new Action<PlayerPrefFlags.Flag, bool>(this.OnFlagChange));
	}

	// Token: 0x06000028 RID: 40 RVA: 0x00002642 File Offset: 0x00000842
	public void OnFlagChange(PlayerPrefFlags.Flag f, bool value)
	{
		if (f != this.flag)
		{
			return;
		}
		if (this.invertFlag)
		{
			this.active = !value;
		}
		else
		{
			this.active = value;
		}
		this.SetGOsActive(this.flashes);
	}

	// Token: 0x06000029 RID: 41 RVA: 0x00002678 File Offset: 0x00000878
	private async void SetGOsActive(int fls)
	{
		if (!this.flashing)
		{
			if (this.mode == ActivateGO.ActivateGOMode.EnableRenderers)
			{
				List<Renderer> renderers = new List<Renderer>();
				renderers.AddRange(this.targetGO.GetComponentsInChildren<MeshRenderer>(true));
				renderers.AddRange(this.targetGO.GetComponentsInChildren<SkinnedMeshRenderer>(true));
				FirstPersonToggleOverride[] componentsInChildren = this.targetGO.GetComponentsInChildren<FirstPersonToggleOverride>(true);
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					if (componentsInChildren[j].Toggle && !renderers.Contains(componentsInChildren[j].Renderer))
					{
						renderers.Add(componentsInChildren[j].Renderer);
					}
					else if (!componentsInChildren[j].Toggle && renderers.Contains(componentsInChildren[j].Renderer))
					{
						renderers.Remove(componentsInChildren[j].Renderer);
					}
				}
				for (int i = 0; i < fls; i++)
				{
					this.flashing = true;
					this.toggle(renderers, this.active);
					await Task.Delay(150);
					this.toggle(renderers, !this.active);
					await Task.Delay(100);
				}
				this.toggle(renderers, this.active);
				this.flashing = false;
				renderers = null;
			}
			else
			{
				for (int i = 0; i < fls; i++)
				{
					this.flashing = true;
					this.targetGO.SetActive(this.active);
					await Task.Delay(150);
					this.targetGO.SetActive(!this.active);
					await Task.Delay(100);
				}
				this.targetGO.SetActive(this.active);
				this.flashing = false;
			}
		}
	}

	// Token: 0x0600002A RID: 42 RVA: 0x000026B8 File Offset: 0x000008B8
	private void toggle(List<Renderer> renderers, bool state)
	{
		for (int i = 0; i < renderers.Count; i++)
		{
			if ((this.layerMask.value & (1 << renderers[i].gameObject.layer)) != 0)
			{
				renderers[i].forceRenderingOff = !state;
			}
		}
	}

	// Token: 0x0400000D RID: 13
	[SerializeField]
	private GameObject targetGO;

	// Token: 0x0400000E RID: 14
	[SerializeField]
	private PlayerPrefFlags.Flag flag;

	// Token: 0x0400000F RID: 15
	[SerializeField]
	private bool invertFlag;

	// Token: 0x04000010 RID: 16
	[SerializeField]
	private int flashes;

	// Token: 0x04000011 RID: 17
	[SerializeField]
	private LayerMask layerMask;

	// Token: 0x04000012 RID: 18
	[SerializeField]
	private ActivateGO.ActivateGOMode mode;

	// Token: 0x04000013 RID: 19
	private bool active;

	// Token: 0x04000014 RID: 20
	private bool flashing;

	// Token: 0x0200000D RID: 13
	public enum ActivateGOMode
	{
		// Token: 0x04000016 RID: 22
		EnableRenderers,
		// Token: 0x04000017 RID: 23
		ActivateGameObjects
	}
}
