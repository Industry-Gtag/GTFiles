using System;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x02001244 RID: 4676
	public class SplinePlanetZone : PlanetZone
	{
		// Token: 0x06007692 RID: 30354 RVA: 0x0026724C File Offset: 0x0026544C
		protected override Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			Vector3 vector;
			float closestEvaluationOnSpline = this.spline.GetClosestEvaluationOnSpline(worldPosition, out vector);
			Vector3 vector2 = this.spline.Evaluate(closestEvaluationOnSpline);
			return worldPosition - vector2;
		}

		// Token: 0x04008610 RID: 34320
		[SerializeField]
		private CatmullRomSpline spline;
	}
}
