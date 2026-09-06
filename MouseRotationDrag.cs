using System;
using UnityEngine;

// Token: 0x02000026 RID: 38
public class MouseRotationDrag : MonoBehaviour
{
	// Token: 0x06000093 RID: 147 RVA: 0x00004FF4 File Offset: 0x000031F4
	private void Start()
	{
		this.m_currFrameHasFocus = false;
		this.m_prevFrameHasFocus = false;
	}

	// Token: 0x06000094 RID: 148 RVA: 0x00005004 File Offset: 0x00003204
	private void Update()
	{
		this.m_currFrameHasFocus = Application.isFocused;
		bool prevFrameHasFocus = this.m_prevFrameHasFocus;
		this.m_prevFrameHasFocus = this.m_currFrameHasFocus;
		if (!prevFrameHasFocus && !this.m_currFrameHasFocus)
		{
			return;
		}
		Vector3 mousePosition = Input.mousePosition;
		Vector3 prevMousePosition = this.m_prevMousePosition;
		Vector3 vector = mousePosition - prevMousePosition;
		this.m_prevMousePosition = mousePosition;
		if (!prevFrameHasFocus)
		{
			this.m_euler = base.transform.rotation.eulerAngles;
			return;
		}
		if (Input.GetMouseButton(0))
		{
			this.m_euler.x = this.m_euler.x + vector.y;
			this.m_euler.y = this.m_euler.y + vector.x;
			base.transform.rotation = Quaternion.Euler(this.m_euler);
		}
	}

	// Token: 0x040000AB RID: 171
	private bool m_currFrameHasFocus;

	// Token: 0x040000AC RID: 172
	private bool m_prevFrameHasFocus;

	// Token: 0x040000AD RID: 173
	private Vector3 m_prevMousePosition;

	// Token: 0x040000AE RID: 174
	private Vector3 m_euler;
}
