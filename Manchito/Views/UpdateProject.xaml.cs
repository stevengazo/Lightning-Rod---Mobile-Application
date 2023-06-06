using Manchito.ViewModel;
namespace Manchito.Views;

public partial class UpdateProject : ContentPage
{
    public int ProjectId { get; set; }
    public UpdateProject()
    {
        InitializeComponent();
        BindingContext = new UpdateProjectViewModel(this);
    }

    public UpdateProject(int id)
    {
        ProjectId = id;
        InitializeComponent();
        BindingContext = new UpdateProjectViewModel(this);
    }
}