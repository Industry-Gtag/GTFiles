using System;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005A1 RID: 1441
[RequireComponent(typeof(OnTriggerEventsCosmetic))]
public class SeedPacketTriggerHandler : MonoBehaviour
{
	// Token: 0x06002480 RID: 9344 RVA: 0x000C3F03 File Offset: 0x000C2103
	public void OnTriggerEntered()
	{
		if (this.toggleOnceOnly && this.triggerEntered)
		{
			return;
		}
		this.triggerEntered = true;
		UnityEvent<SeedPacketTriggerHandler> unityEvent = this.onTriggerEntered;
		if (unityEvent != null)
		{
			unityEvent.Invoke(this);
		}
		this.ToggleEffects();
	}

	// Token: 0x06002481 RID: 9345 RVA: 0x000C3F38 File Offset: 0x000C2138
	public void ToggleEffects()
	{
		if (this.particleToPlay)
		{
			this.particleToPlay.Play();
		}
		if (this.soundBankPlayer)
		{
			this.soundBankPlayer.Play();
		}
		if (this.destroyOnTriggerEnter)
		{
			if (this.destroyDelay > 0f)
			{
				base.Invoke("Destroy", this.destroyDelay);
				return;
			}
			this.Destroy();
		}
	}

	// Token: 0x06002482 RID: 9346 RVA: 0x000C3FA2 File Offset: 0x000C21A2
	private void Destroy()
	{
		this.triggerEntered = false;
		if (ObjectPools.instance.DoesPoolExist(base.gameObject))
		{
			ObjectPools.instance.Destroy(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04002FE0 RID: 12256
	[SerializeField]
	private ParticleSystem particleToPlay;

	// Token: 0x04002FE1 RID: 12257
	[SerializeField]
	private SoundBankPlayer soundBankPlayer;

	// Token: 0x04002FE2 RID: 12258
	[SerializeField]
	private bool destroyOnTriggerEnter;

	// Token: 0x04002FE3 RID: 12259
	[SerializeField]
	private float destroyDelay = 1f;

	// Token: 0x04002FE4 RID: 12260
	[SerializeField]
	private bool toggleOnceOnly;

	// Token: 0x04002FE5 RID: 12261
	[HideInInspector]
	public UnityEvent<SeedPacketTriggerHandler> onTriggerEntered;

	// Token: 0x04002FE6 RID: 12262
	private bool triggerEntered;
}
