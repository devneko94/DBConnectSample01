using DBConnectSample01.Common;
using DBConnectSample01.Models;
using DBConnectSample01.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace DBConnectSample01.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<MemberModel> MemberList { get; set; }

        public DelegateCommand CreateCommand { get; private set; }

        public DelegateCommand<object> UpdateCommand { get; private set; }

        public DelegateCommand<object> DeleteCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            MemberList = new ObservableCollection<MemberModel>();
            MemberList.Add(new MemberModel { MemberID = "001", MemberName = "テスト０１", MemberAddress = "東京" });
            MemberList.Add(new MemberModel { MemberID = "002", MemberName = "テスト０２", MemberAddress = "大阪" });
            MemberList.Add(new MemberModel { MemberID = "003", MemberName = "テスト０３", MemberAddress = "京都" });
            MemberList.Add(new MemberModel { MemberID = "004", MemberName = "テスト０４", MemberAddress = "北海道" });
            MemberList.Add(new MemberModel { MemberID = "005", MemberName = "テスト０５", MemberAddress = "岡山" });

            CreateCommand = new DelegateCommand(createCommand);
            UpdateCommand = new DelegateCommand<object>(updateCommand);
            DeleteCommand = new DelegateCommand<object>(deleteCommand);
        }

        private void createCommand()
        {
            SubWindow sub = new SubWindow()
            {
                DataContext = new SubViewModel()
            };
            
            sub.ShowDialog();

            SubViewModel context = (SubViewModel)sub.DataContext;
            MemberModel newModel = (MemberModel)context.Model;
            MemberList.Add(newModel);
        }

        private void updateCommand(object param)
        {
            MemberModel selectedModel = ((MemberModel)param).Clone();

            SubWindow sub = new SubWindow
            {
                DataContext = new SubViewModel 
                {
                    Model = selectedModel
                }
            };

            sub.ShowDialog();

            // TODO: 更新時の処理
        }

        private void deleteCommand(object param)
        {
            MemberModel selectedModel = (MemberModel)param;

            MemberList.Remove(selectedModel);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

// TODO: DB作成
// TODO: DB接続
// TODO: DB操作