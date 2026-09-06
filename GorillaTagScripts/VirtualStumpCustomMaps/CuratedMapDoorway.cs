using System;
using Modio.Mods;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000FC6 RID: 4038
	public class CuratedMapDoorway : MonoBehaviour
	{
		// Token: 0x1700098F RID: 2447
		// (get) Token: 0x06006462 RID: 25698 RVA: 0x00204916 File Offset: 0x00202B16
		// (set) Token: 0x06006463 RID: 25699 RVA: 0x0020491E File Offset: 0x00202B1E
		private Mod CuratedMod { get; set; }

		// Token: 0x17000990 RID: 2448
		// (get) Token: 0x06006464 RID: 25700 RVA: 0x00204927 File Offset: 0x00202B27
		public bool HasCuratedMap
		{
			get
			{
				return this.CuratedMod != null;
			}
		}

		// Token: 0x06006465 RID: 25701 RVA: 0x00204932 File Offset: 0x00202B32
		public void OnEnable()
		{
			CuratedDestinationsManager.OnCuratedMapsUpdated.AddListener(new UnityAction(this.OnCuratedMapsUpdated));
			if (CuratedDestinationsManager.HasRetrievedCuratedMaps)
			{
				this.OnCuratedMapsUpdated();
				return;
			}
			CuratedDestinationsManager.RetrieveCuratedMaps(false);
		}

		// Token: 0x06006466 RID: 25702 RVA: 0x0020495E File Offset: 0x00202B5E
		public void OnDisable()
		{
			CuratedDestinationsManager.OnCuratedMapsUpdated.RemoveListener(new UnityAction(this.OnCuratedMapsUpdated));
		}

		// Token: 0x06006467 RID: 25703 RVA: 0x00204978 File Offset: 0x00202B78
		private void OnCuratedMapsUpdated()
		{
			Mod mod;
			CuratedDestinationsManager.TryGetCuratedMod(this.doorway, out mod);
			this.CuratedMod = mod;
			UnityEvent<Mod> unityEvent = this.onCuratedMapResolved;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(mod);
		}

		// Token: 0x04007327 RID: 29479
		[Tooltip("Which slot in the DestinationsCurated TitleData ID list this doorway loads")]
		[SerializeField]
		private CuratedDestinationsManager.CuratedDoorway doorway;

		// Token: 0x04007328 RID: 29480
		public UnityEvent<Mod> onCuratedMapResolved;
	}
}
