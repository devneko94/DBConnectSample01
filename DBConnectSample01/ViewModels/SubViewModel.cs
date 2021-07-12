using DBConnectSample01.Common;
using DBConnectSample01.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DBConnectSample01.ViewModels
{
    public class SubViewModel : INotifyPropertyChanged
    {
        public MemberModel Model
        {
            get => _model;
            set
            {
                _model = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand<Window> CancelCommand { get; private set; }

        public DelegateCommand<Window> RegisterCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private MemberModel _model = new MemberModel();

        public SubViewModel()
        {
            CancelCommand = new DelegateCommand<Window>(cancelCommand);
            RegisterCommand = new DelegateCommand<Window>(registerCommand);
        }

        private void cancelCommand(Window param)
        {
            param.Close();
        }

        private void registerCommand(Window param)
        {
            param.Close();
        }


        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
