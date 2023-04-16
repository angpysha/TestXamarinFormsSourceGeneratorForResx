using System;
using System.Collections.Generic;
using TestXamarinFormsSourceGeneratorForResx.ViewModels;
using TestXamarinFormsSourceGeneratorForResx.Views;
using Xamarin.Forms;

namespace TestXamarinFormsSourceGeneratorForResx
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
