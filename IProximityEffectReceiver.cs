using System;

// Token: 0x020002F3 RID: 755
public interface IProximityEffectReceiver
{
	// Token: 0x0600133D RID: 4925
	void OnProximityCalculated(float distance, float alignment, float parallel);
}
