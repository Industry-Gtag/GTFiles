using System;
using UnityEngine;

// Token: 0x020002F6 RID: 758
public class ProximityEffectScoreCurvesSO : ScriptableObject
{
	// Token: 0x04001797 RID: 6039
	[Tooltip("How far apart the transforms are. A distance. Contributes 'red' to the debug line. Y value should be in the range 0-1.")]
	public AnimationCurve distanceModifierCurve;

	// Token: 0x04001798 RID: 6040
	[Tooltip("How closely the transforms' Z vectors are pointed towards each other. A dot product. Contributes 'green' to the debug line. Y value should be in the range 0-1.")]
	public AnimationCurve alignmentModifierCurve;

	// Token: 0x04001799 RID: 6041
	[Tooltip("Whether each transform is in front of the other transform. The average of two dot products. Contributes 'blue' to the debug line. Y value should be in the range 0-1.")]
	public AnimationCurve parallelModifierCurve;
}
