using Manchito.Model;
using Manchito.Views;

namespace Manchito;

public partial class MainPage : FlyoutPage
{


    public MainPage()
    {
        InitializeComponent();
        FlyoutPage.collectionView.SelectionChanged += OnSelectionChanged;
        var DefaultPage = new ViewProjectList();
        NavigationDispatcher.Instance.Initialize(this.Navigation);
    }

    void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection.FirstOrDefault() as FlyoutPageItem;
        if (item != null)
        {
            Detail =  (Page)Activator.CreateInstance(item.TargetType);
            
            if (!((IFlyoutPageController)this).ShouldShowSplitMode)
                IsPresented = false;
        }
    }


}

