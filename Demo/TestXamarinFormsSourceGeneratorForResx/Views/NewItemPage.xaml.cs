using System;
using System.Collections.Generic;
using System.ComponentModel;
using TestXamarinFormsSourceGeneratorForResx.Models;
using TestXamarinFormsSourceGeneratorForResx.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestXamarinFormsSourceGeneratorForResx.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}