using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityChan
{
	// Token: 0x0200141A RID: 5146
	public class IdleChanger : MonoBehaviour
	{
		// Token: 0x060081CF RID: 33231 RVA: 0x002A6625 File Offset: 0x002A4825
		private void Start()
		{
			this.currentState = this.UnityChanA.GetCurrentAnimatorStateInfo(0);
			this.previousState = this.currentState;
			base.StartCoroutine("RandomChange");
			this.kb = Keyboard.current;
		}

		// Token: 0x060081D0 RID: 33232 RVA: 0x002A665C File Offset: 0x002A485C
		private void Update()
		{
			if (this.kb.upArrowKey.wasPressedThisFrame || this.kb.spaceKey.wasPressedThisFrame)
			{
				this.UnityChanA.SetBool("Next", true);
				this.UnityChanB.SetBool("Next", true);
			}
			if (this.kb.downArrowKey.wasPressedThisFrame)
			{
				this.UnityChanA.SetBool("Back", true);
				this.UnityChanB.SetBool("Back", true);
			}
			if (this.UnityChanA.GetBool("Next"))
			{
				this.currentState = this.UnityChanA.GetCurrentAnimatorStateInfo(0);
				if (this.previousState.fullPathHash != this.currentState.fullPathHash)
				{
					this.UnityChanA.SetBool("Next", false);
					this.UnityChanB.SetBool("Next", false);
					this.previousState = this.currentState;
				}
			}
			if (this.UnityChanA.GetBool("Back"))
			{
				this.currentState = this.UnityChanA.GetCurrentAnimatorStateInfo(0);
				if (this.previousState.fullPathHash != this.currentState.fullPathHash)
				{
					this.UnityChanA.SetBool("Back", false);
					this.UnityChanB.SetBool("Back", false);
					this.previousState = this.currentState;
				}
			}
		}

		// Token: 0x060081D1 RID: 33233 RVA: 0x002A67B8 File Offset: 0x002A49B8
		private void OnGUI()
		{
			if (this.isGUI)
			{
				GUI.Box(new Rect((float)(Screen.width - 110), 10f, 100f, 90f), "Change Motion");
				if (GUI.Button(new Rect((float)(Screen.width - 100), 40f, 80f, 20f), "Next"))
				{
					this.UnityChanA.SetBool("Next", true);
					this.UnityChanB.SetBool("Next", true);
				}
				if (GUI.Button(new Rect((float)(Screen.width - 100), 70f, 80f, 20f), "Back"))
				{
					this.UnityChanA.SetBool("Back", true);
					this.UnityChanB.SetBool("Back", true);
				}
			}
		}

		// Token: 0x060081D2 RID: 33234 RVA: 0x002A688D File Offset: 0x002A4A8D
		private IEnumerator RandomChange()
		{
			for (;;)
			{
				if (this._random)
				{
					float num = Random.Range(0f, 1f);
					if (num < this._threshold)
					{
						this.UnityChanA.SetBool("Back", true);
						this.UnityChanB.SetBool("Back", true);
					}
					else if (num >= this._threshold)
					{
						this.UnityChanA.SetBool("Next", true);
						this.UnityChanB.SetBool("Next", true);
					}
				}
				yield return new WaitForSeconds(this._interval);
			}
			yield break;
		}

		// Token: 0x0400928C RID: 37516
		private AnimatorStateInfo currentState;

		// Token: 0x0400928D RID: 37517
		private AnimatorStateInfo previousState;

		// Token: 0x0400928E RID: 37518
		public bool _random;

		// Token: 0x0400928F RID: 37519
		public float _threshold = 0.5f;

		// Token: 0x04009290 RID: 37520
		public float _interval = 10f;

		// Token: 0x04009291 RID: 37521
		public bool isGUI = true;

		// Token: 0x04009292 RID: 37522
		public Animator UnityChanA;

		// Token: 0x04009293 RID: 37523
		public Animator UnityChanB;

		// Token: 0x04009294 RID: 37524
		private Keyboard kb;
	}
}
