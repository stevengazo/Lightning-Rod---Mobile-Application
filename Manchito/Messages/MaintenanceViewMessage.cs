using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Manchito.Messages
{
	public class NameItemViewMessage : ValueChangedMessage<int>
	{
		public NameItemViewMessage(int value) : base(value)
		{
		}
	}
}
