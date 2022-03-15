using System.ComponentModel;

namespace com.services.telegram.youtrack
{
	public enum YouTrackNotificationType
	{
		[Description("Стал исполнителем")]
		Assigned,

		[Description("Исполнителем стал другой пользователь")]
		Reassigned,

		[Description("У задачи изменили заголовок")]
		UpdatedSummary,

		[Description("У задачи изменили описание")]
		UpdatedDescription,

		[Description("У задачи изменили состояние")]
		UpdatedState,

		[Description("У задачи изменили приоритет")]
		UpdatedPriority,

		[Description("У задачи изменили какое-то поле")]
		UpdatedOtherField,

		[Description("На тебя была создана задача")]
		Created,

		[Description("К задаче добавили сссылку")]
		LinkAdded,

		[Description("У задачи удалили ссылку")]
		LinkRemoved,

		[Description("К задаче добавили коментарий")]
		Commented,

		[Description("Тебя упомянули в сообщении к задаче")]
		Mentioned
	}
}