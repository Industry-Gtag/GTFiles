using System;
using System.Collections.Generic;

// Token: 0x02000786 RID: 1926
public class GRBehaviors<T> : GRBehaviorsBase where T : Enum
{
	// Token: 0x060030D5 RID: 12501 RVA: 0x00108F30 File Offset: 0x00107130
	public void AddBehavior(T behavior, GRAbilityBase ability)
	{
		GRBehaviors<T>.BehaviorData behaviorData = new GRBehaviors<T>.BehaviorData
		{
			behavior = behavior,
			ability = ability
		};
		this.behaviorData.Add(behaviorData);
	}

	// Token: 0x04003E77 RID: 15991
	public List<GRBehaviors<T>.BehaviorData> behaviorData;

	// Token: 0x02000787 RID: 1927
	public class BehaviorData
	{
		// Token: 0x04003E78 RID: 15992
		public T behavior;

		// Token: 0x04003E79 RID: 15993
		public GRAbilityBase ability;
	}
}
