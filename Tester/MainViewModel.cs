using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using CinchExtended.BusinessObjects;
using CinchExtended.Commands;
using CinchExtended.Services.Implementation;
using CinchExtended.Services.Interfaces;
using CinchExtended.Utilities;
using CinchExtended.ViewModels;
using MEFedMVVM.ViewModelLocator;

namespace Tester
{
    [ExportViewModel("MainViewModel")]
    public class MainViewModel : ViewModelBase
    {
        private readonly IExtendedUIVisualizerService visualizerService;

        [ImportingConstructor]
        public MainViewModel(IExtendedUIVisualizerService visualizerService)
        {
            this.visualizerService = visualizerService;
            var parentPropertyChangeArgs = ObservableHelper.CreateArgs<MainViewModel>(viewModel => viewModel.Fabuloso);
            Fabuloso = new DataWrapper<string>(this, parentPropertyChangeArgs);
            Fabuloso.PropertyChanged += FabulosoOnPropertyChanged;
            var name = parentPropertyChangeArgs.PropertyName;

            ShowPopupCommand = new SimpleCommand<object, object>(o => ShowPopup());
        }

        private void ShowPopup()
        {            
            Application.Current.MainWindow.Close();
            visualizerService.ShowDialog("Popup", (object) null, OwnerOption.None);
        }

        private void FabulosoOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var dataWrapper = (DataWrapperBase) sender;
            var myName = dataWrapper.ParentPropertyChangeArgs.PropertyName;
        }

        public DataWrapper<string> Fabuloso { get; set; }

        public ICommand ShowPopupCommand { get; set; }
    }
}