using System;

// Token: 0x02000266 RID: 614
public interface GorillaQuestManager
{
	// Token: 0x06001076 RID: 4214
	void LoadQuestsFromJson(string jsonString);

	// Token: 0x06001077 RID: 4215
	void LoadQuestProgress();

	// Token: 0x06001078 RID: 4216
	void SaveQuestProgress();

	// Token: 0x06001079 RID: 4217
	void SetupAllQuestEventListeners();

	// Token: 0x0600107A RID: 4218
	void ClearAllQuestEventListeners();

	// Token: 0x0600107B RID: 4219
	void HandleQuestProgressChanged(bool initialLoad);

	// Token: 0x0600107C RID: 4220
	void HandleQuestCompleted(int questID);
}
