using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x0200102A RID: 4138
	public class BuilderAnimateOnTap : BuilderPieceTappable
	{
		// Token: 0x060066EE RID: 26350 RVA: 0x00210EB9 File Offset: 0x0020F0B9
		public override void OnTapReplicated()
		{
			base.OnTapReplicated();
			this.anim.Rewind();
			this.anim.Play();
		}

		// Token: 0x040075CF RID: 30159
		[SerializeField]
		private Animation anim;
	}
}
