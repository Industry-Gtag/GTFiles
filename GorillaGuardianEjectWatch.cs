using System;
using GorillaGameModes;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000873 RID: 2163
public class GorillaGuardianEjectWatch : MonoBehaviour
{
	// Token: 0x06003833 RID: 14387 RVA: 0x001328BC File Offset: 0x00130ABC
	private void Start()
	{
		if (this.ejectButton != null)
		{
			this.ejectButton.onPressButton.AddListener(new UnityAction(this.OnEjectButtonPressed));
		}
	}

	// Token: 0x06003834 RID: 14388 RVA: 0x001328E8 File Offset: 0x00130AE8
	private void OnDestroy()
	{
		if (this.ejectButton != null)
		{
			this.ejectButton.onPressButton.RemoveListener(new UnityAction(this.OnEjectButtonPressed));
		}
	}

	// Token: 0x06003835 RID: 14389 RVA: 0x00132914 File Offset: 0x00130B14
	private void OnEjectButtonPressed()
	{
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null)
		{
			gorillaGuardianManager.RequestEjectGuardian(NetworkSystem.Instance.LocalPlayer);
		}
	}

	// Token: 0x0400484E RID: 18510
	[SerializeField]
	private HeldButton ejectButton;
}
