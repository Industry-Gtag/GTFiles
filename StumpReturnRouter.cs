using System;
using GorillaTagScripts.VirtualStumpCustomMaps;
using UnityEngine;

// Token: 0x02000AAC RID: 2732
[RequireComponent(typeof(TeleportNode))]
public class StumpReturnRouter : MonoBehaviour
{
	// Token: 0x060045E0 RID: 17888 RVA: 0x001776A2 File Offset: 0x001758A2
	private void Awake()
	{
		this.node = base.GetComponent<TeleportNode>();
	}

	// Token: 0x060045E1 RID: 17889 RVA: 0x001776B0 File Offset: 0x001758B0
	private void Update()
	{
		VirtualStumpActivateMode currentActivateMode = CustomMapManager.CurrentActivateMode;
		if (this.hasApplied && currentActivateMode == this.appliedMode)
		{
			return;
		}
		this.appliedMode = currentActivateMode;
		this.hasApplied = true;
		Transform destination = this.GetDestination(currentActivateMode);
		if (destination == null)
		{
			Debug.LogWarning(string.Format("[StumpReturnRouter] No return destination assigned for mode {0}; the ", currentActivateMode) + "return node will fall back to its serialized teleportToRef.", this);
		}
		Debug.LogWarning(string.Format("[StumpReturnRouter] on node '{0}': mode={1} -> destination=", this.node.gameObject.name, currentActivateMode) + ((destination != null) ? destination.name : "NULL"));
		this.node.SetDestinationOverride(destination);
	}

	// Token: 0x060045E2 RID: 17890 RVA: 0x00177760 File Offset: 0x00175960
	public void OnReturnedToHallway()
	{
		CustomMapManager.Deactivate();
	}

	// Token: 0x060045E3 RID: 17891 RVA: 0x00177767 File Offset: 0x00175967
	private Transform GetDestination(VirtualStumpActivateMode mode)
	{
		if (mode == VirtualStumpActivateMode.FeatureA)
		{
			return this.featureADestination;
		}
		if (mode != VirtualStumpActivateMode.FeatureB)
		{
			return this.customDestination;
		}
		return this.featureBDestination;
	}

	// Token: 0x04005826 RID: 22566
	[Tooltip("Where the return node drops the player back into the Custom hallway.")]
	[SerializeField]
	private Transform customDestination;

	// Token: 0x04005827 RID: 22567
	[Tooltip("Where the return node drops the player back into the Feature A hallway.")]
	[SerializeField]
	private Transform featureADestination;

	// Token: 0x04005828 RID: 22568
	[Tooltip("Where the return node drops the player back into the Feature B hallway.")]
	[SerializeField]
	private Transform featureBDestination;

	// Token: 0x04005829 RID: 22569
	private TeleportNode node;

	// Token: 0x0400582A RID: 22570
	private VirtualStumpActivateMode appliedMode;

	// Token: 0x0400582B RID: 22571
	private bool hasApplied;
}
