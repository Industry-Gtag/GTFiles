using System;
using Liv.Lck;
using Liv.Lck.Core.Cosmetics;
using Liv.Lck.Cosmetics;
using Liv.Lck.DependencyInjection;
using UnityEngine;

// Token: 0x020003ED RID: 1005
[DefaultExecutionOrder(-950)]
public class GtLckServiceInitializer : MonoBehaviour
{
	// Token: 0x060017D9 RID: 6105 RVA: 0x00088C98 File Offset: 0x00086E98
	private void Awake()
	{
		LckDiContainer instance = LckDiContainer.Instance;
		if (instance.HasService<ILckService>())
		{
			Debug.LogWarning("LCK: Service already configured. Skipping custom GT initialisation.");
			return;
		}
		Debug.Log("LCK: Initializing with GT-SPECIFIC overrides.");
		LckServiceInitializer.ConfigureServices(instance, this._qualityConfig, delegate(LckDiContainer container)
		{
			container.AddSingleton<ILckCosmeticsFeatureFlagManager, LckCosmeticsFeatureFlagManagerPlayFab>();
			container.AddSingleton<ILckCosmeticsCoordinator, LckCoreCosmeticsCoordinator>();
			container.AddSingleton<ILckCosmeticsManager, LckCosmeticsManager>();
		});
	}

	// Token: 0x04002318 RID: 8984
	[Header("LCK Configuration")]
	[Tooltip("Assign the LCK Quality Config ScriptableObject here.")]
	[SerializeField]
	private LckQualityConfig _qualityConfig;
}
