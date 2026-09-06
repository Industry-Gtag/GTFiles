using System;
using UnityEngine;

namespace com.AnotherAxiom.MonkeArcade.Joust
{
	// Token: 0x02001176 RID: 4470
	public class JoustPlayer : MonoBehaviour
	{
		// Token: 0x17000AB0 RID: 2736
		// (get) Token: 0x0600700F RID: 28687 RVA: 0x00242735 File Offset: 0x00240935
		// (set) Token: 0x06007010 RID: 28688 RVA: 0x0024273D File Offset: 0x0024093D
		public float HorizontalSpeed
		{
			get
			{
				return this.HSpeed;
			}
			set
			{
				this.HSpeed = value;
			}
		}

		// Token: 0x06007011 RID: 28689 RVA: 0x00242748 File Offset: 0x00240948
		private void LateUpdate()
		{
			this.velocity.x = this.HSpeed * 0.001f;
			if (this.flap)
			{
				this.velocity.y = Mathf.Min(this.velocity.y + 0.0005f, 0.0005f);
				this.flap = false;
			}
			else
			{
				this.velocity.y = Mathf.Max(this.velocity.y - Time.deltaTime * 0.0001f, -0.001f);
				int i = 0;
				while (i < Physics2D.RaycastNonAlloc(base.transform.position, this.velocity.normalized, this.raycastHitResults, this.velocity.magnitude))
				{
					JoustTerrain joustTerrain;
					if (this.raycastHitResults[i].collider.TryGetComponent<JoustTerrain>(out joustTerrain))
					{
						this.velocity.y = 0f;
						if (joustTerrain.transform.localPosition.y < base.transform.localPosition.y)
						{
							base.transform.localPosition = new Vector2(base.transform.localPosition.x, joustTerrain.transform.localPosition.y + this.raycastHitResults[i].collider.bounds.size.y);
							break;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			base.transform.Translate(this.velocity);
			if ((double)Mathf.Abs(base.transform.localPosition.x) > 4.5)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x * -0.95f, base.transform.localPosition.y);
			}
		}

		// Token: 0x06007012 RID: 28690 RVA: 0x0024292E File Offset: 0x00240B2E
		public void Flap()
		{
			this.flap = true;
		}

		// Token: 0x04008015 RID: 32789
		private Vector2 velocity;

		// Token: 0x04008016 RID: 32790
		private RaycastHit2D[] raycastHitResults = new RaycastHit2D[8];

		// Token: 0x04008017 RID: 32791
		private float HSpeed;

		// Token: 0x04008018 RID: 32792
		private bool flap;
	}
}
