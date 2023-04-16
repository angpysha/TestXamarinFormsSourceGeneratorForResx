using System.ComponentModel;
using TestXamarinFormsSourceGeneratorForResx.ViewModels;
using Xamarin.Forms;

namespace TestXamarinFormsSourceGeneratorForResx.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}