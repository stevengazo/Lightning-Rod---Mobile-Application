using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Manchito.Messages;
public class ProjectViewMessage : ValueChangedMessage<int>
{
    public ProjectViewMessage(int value) : base(value)
    {
    }
}

