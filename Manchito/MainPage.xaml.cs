using Manchito.Model;

namespace Manchito;

public partial class MainPage : FlyoutPage
{


    public MainPage()
    {
        InitializeComponent();
        FlyoutPage.collectionView.SelectionChanged += OnSelectionChanged;

    }

    void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection.FirstOrDefault() as FlyoutPageItem;
        if (item != null)
        {
            Detail =  (Page)Activator.CreateInstance(item.TargetType);
            NavigationDispatcher.Instance.Initialize(this.Navigation);
            if (!((IFlyoutPageController)this).ShouldShowSplitMode)
                IsPresented = false;
        }
    }


}

