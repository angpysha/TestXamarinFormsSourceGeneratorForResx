using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestXamarinFormsSourceGeneratorForResx.Models;
using TestXamarinFormsSourceGeneratorForResx.ViewModels;
using TestXamarinFormsSourceGeneratorForResx.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestXamarinFormsSourceGeneratorForResx.Views
{
    public partial class ItemsPage : ContentPage
    {
        private readonly ItemsViewModel _viewModel;

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new ItemsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}