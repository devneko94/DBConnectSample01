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
        #region プロパティ
        /// <summary>
        /// メンバーモデル
        /// </summary>
        public MemberModel Model { get; set; } = new MemberModel();

        /// <summary>
        /// 変更可否フラグ
        /// </summary>
        public bool CanPKEdit { get; set; } = true;

        /// <summary>
        /// キャンセルコマンド
        /// </summary>
        public DelegateCommand<Window> CancelCommand { get; }

        /// <summary>
        /// 登録コマンド
        /// </summary>
        public DelegateCommand<Window> RegisterCommand { get; }
        #endregion

        #region イベント
        /// <summary>
        /// プロパティ変更通知イベント
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SubViewModel()
        {
            CancelCommand = new DelegateCommand<Window>(Cancel);
            RegisterCommand = new DelegateCommand<Window>(Register);
        }
        #endregion

        #region メソッド
        /// <summary>
        /// キャンセル
        /// </summary>
        /// <param name="window">画面オブジェクト</param>
        private void Cancel(Window window)
        {
            window.Close();
        }

        /// <summary>
        /// 登録
        /// </summary>
        /// <param name="window">画面オブジェクト</param>
        private void Register(Window window)
        {
            window.Close();
        }

        /// <summary>
        /// プロパティ変更通知
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
