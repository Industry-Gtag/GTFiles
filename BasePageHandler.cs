using System;
using UnityEngine;

// Token: 0x02000D84 RID: 3460
public abstract class BasePageHandler : MonoBehaviour
{
	// Token: 0x1700082A RID: 2090
	// (get) Token: 0x06005541 RID: 21825 RVA: 0x001BE890 File Offset: 0x001BCA90
	// (set) Token: 0x06005542 RID: 21826 RVA: 0x001BE898 File Offset: 0x001BCA98
	private protected int selectedIndex { protected get; private set; }

	// Token: 0x1700082B RID: 2091
	// (get) Token: 0x06005543 RID: 21827 RVA: 0x001BE8A1 File Offset: 0x001BCAA1
	// (set) Token: 0x06005544 RID: 21828 RVA: 0x001BE8A9 File Offset: 0x001BCAA9
	private protected int currentPage { protected get; private set; }

	// Token: 0x1700082C RID: 2092
	// (get) Token: 0x06005545 RID: 21829 RVA: 0x001BE8B2 File Offset: 0x001BCAB2
	// (set) Token: 0x06005546 RID: 21830 RVA: 0x001BE8BA File Offset: 0x001BCABA
	private protected int pages { protected get; private set; }

	// Token: 0x1700082D RID: 2093
	// (get) Token: 0x06005547 RID: 21831 RVA: 0x001BE8C3 File Offset: 0x001BCAC3
	// (set) Token: 0x06005548 RID: 21832 RVA: 0x001BE8CB File Offset: 0x001BCACB
	private protected int maxEntires { protected get; private set; }

	// Token: 0x1700082E RID: 2094
	// (get) Token: 0x06005549 RID: 21833
	protected abstract int pageSize { get; }

	// Token: 0x1700082F RID: 2095
	// (get) Token: 0x0600554A RID: 21834
	protected abstract int entriesCount { get; }

	// Token: 0x0600554B RID: 21835 RVA: 0x001BE8D4 File Offset: 0x001BCAD4
	protected virtual void Start()
	{
		Debug.Log("base page handler " + this.entriesCount.ToString() + " " + this.pageSize.ToString());
		this.pages = this.entriesCount / this.pageSize + 1;
		this.maxEntires = this.pages * this.pageSize;
	}

	// Token: 0x0600554C RID: 21836 RVA: 0x001BE93C File Offset: 0x001BCB3C
	public void SelectEntryOnPage(int entryIndex)
	{
		int num = entryIndex + this.pageSize * this.currentPage;
		if (num > this.entriesCount)
		{
			return;
		}
		this.selectedIndex = num;
		this.PageEntrySelected(entryIndex, this.selectedIndex);
	}

	// Token: 0x0600554D RID: 21837 RVA: 0x001BE978 File Offset: 0x001BCB78
	public void SelectEntryFromIndex(int index)
	{
		this.selectedIndex = index;
		this.currentPage = this.selectedIndex / this.pageSize;
		int num = index - this.pageSize * this.currentPage;
		this.PageEntrySelected(num, index);
		this.SetPage(this.currentPage);
	}

	// Token: 0x0600554E RID: 21838 RVA: 0x001BE9C4 File Offset: 0x001BCBC4
	public void ChangePage(bool left)
	{
		int num = (left ? (-1) : 1);
		this.SetPage(Mathf.Abs((this.currentPage + num) % this.pages));
	}

	// Token: 0x0600554F RID: 21839 RVA: 0x001BE9F4 File Offset: 0x001BCBF4
	public void SetPage(int page)
	{
		if (page > this.pages)
		{
			return;
		}
		this.currentPage = page;
		int num = this.pageSize * page;
		this.ShowPage(this.currentPage, num, Mathf.Min(num + this.pageSize, this.entriesCount));
	}

	// Token: 0x06005550 RID: 21840
	protected abstract void ShowPage(int selectedPage, int startIndex, int endIndex);

	// Token: 0x06005551 RID: 21841
	protected abstract void PageEntrySelected(int pageEntry, int selectionIndex);
}
