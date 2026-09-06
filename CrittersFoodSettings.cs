using System;
using UnityEngine;

// Token: 0x02000062 RID: 98
public class CrittersFoodSettings : CrittersActorSettings
{
	// Token: 0x060001EB RID: 491 RVA: 0x0000B6B0 File Offset: 0x000098B0
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersFood crittersFood = (CrittersFood)this.parentActor;
		crittersFood.maxFood = this._maxFood;
		crittersFood.currentFood = this._currentFood;
		crittersFood.startingSize = this._startingSize;
		crittersFood.currentSize = this._currentSize;
		crittersFood.food = this._food;
		crittersFood.disableWhenEmpty = this._disableWhenEmpty;
		crittersFood.SpawnData(this._maxFood, this._currentFood, this._startingSize);
	}

	// Token: 0x0400022A RID: 554
	public float _maxFood;

	// Token: 0x0400022B RID: 555
	public float _currentFood;

	// Token: 0x0400022C RID: 556
	public float _startingSize;

	// Token: 0x0400022D RID: 557
	public float _currentSize;

	// Token: 0x0400022E RID: 558
	public Transform _food;

	// Token: 0x0400022F RID: 559
	public bool _disableWhenEmpty;
}
