using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Manchito.Messages
{
	public class MaintenanceViewMessage : ValueChangedMessage<int>
	{
		public MaintenanceViewMessage(int value) : base(value)
		{
		}
	}
}
