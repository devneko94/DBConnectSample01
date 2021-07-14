using DBConnectSample01.Common;
using DBConnectSample01.Models;
using DBConnectSample01.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
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
        #region 定数
        /// <summary>
        /// 接続文字列
        /// </summary>
        private static readonly string CONNECT_STRING
            = @"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\temp\SampleDB01.accdb";

        /// <summary>
        /// テーブル取得SQL文
        /// </summary>
        private static readonly string SQL_SELECT = "SELECT * FROM MemberListTable";
        #endregion

        #region プロパティ
        /// <summary>
        /// メンバーリスト
        /// </summary>
        public ObservableCollection<MemberModel> MemberList { get; set; } = new ObservableCollection<MemberModel>();

        /// <summary>
        /// 新規作成コマンド
        /// </summary>
        public DelegateCommand CreateCommand { get; }

        /// <summary>
        /// 更新コマンド
        /// </summary>
        public DelegateCommand<object> UpdateCommand { get; }

        /// <summary>
        /// 削除コマンド
        /// </summary>
        public DelegateCommand<object> DeleteCommand { get; }
        #endregion

        #region イベント
        /// <summary>
        /// 変更通知イベント
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region フィールド
        /// <summary>
        /// データテーブル
        /// </summary>
        private DataTable _dataTable = new DataTable();

        /// <summary>
        /// DBコマンド
        /// </summary>
        private OleDbCommand _dbCommand = new OleDbCommand();

        /// <summary>
        /// DBデータアダプター
        /// </summary>
        private OleDbDataAdapter _dataAdapter = new OleDbDataAdapter();

        /// <summary>
        /// DBコネクション
        /// </summary>
        private OleDbConnection _dbConnection = new OleDbConnection();
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainViewModel()
        {
            CreateCommand = new DelegateCommand(CreateRecord);
            UpdateCommand = new DelegateCommand<object>(UpdateRecord);
            DeleteCommand = new DelegateCommand<object>(DeleteRecord);

            InitDB();
        }
        #endregion

        #region メソッド
        /// <summary>
        /// DB初期化
        /// </summary>
        private void InitDB()
        {
            if (OpenDB())
            {
                ShowTable();
            }
        }

        /// <summary>
        /// DB接続
        /// </summary>
        private bool OpenDB()
        {
            try
            {
                _dbConnection.ConnectionString = CONNECT_STRING;
                _dbConnection.Open();
                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        /// <summary>
        /// テーブル内容表示
        /// </summary>
        private void ShowTable()
        {
            MemberList.Clear();

            try
            {
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = SQL_SELECT;
                _dataAdapter.SelectCommand = _dbCommand;
                _dataAdapter.Fill(_dataTable);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                CloseDB();
            }

            foreach (DataRow dataRow in _dataTable.Rows )
            {
                var model = new MemberModel
                {
                    MemberID = dataRow[0].ToString(),
                    MemberName = dataRow[1].ToString(),
                    MemberAddress = dataRow[2].ToString()
                };

                MemberList.Add(model);
            }
        }

        /// <summary>
        /// DB切断
        /// </summary>
        private void CloseDB()
        {
            _dbCommand?.Dispose();
            _dataAdapter?.Dispose();
            _dbConnection?.Close();
        }

        /// <summary>
        /// 新規作成
        /// </summary>
        private void CreateRecord()
        {
            SubWindow sub = new SubWindow()
            {
                DataContext = new SubViewModel(),
            };
            
            sub.ShowDialog();

            SubViewModel context = (SubViewModel)sub.DataContext;
            MemberList.Add(context.Model);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="record">選択レコード</param>
        private void UpdateRecord(object record)
        {
            MemberModel oldModel = (MemberModel)record;

            SubWindow sub = new SubWindow
            {
                DataContext = new SubViewModel 
                {
                    Model = new MemberModel
                    {
                        MemberID = oldModel.MemberID,
                        MemberName = oldModel.MemberName,
                        MemberAddress = oldModel.MemberAddress
                    },
                    CanEdit = false
                }
            };

            sub.ShowDialog();

            // 更新時の処理
            MemberModel newModel = ((SubViewModel)sub.DataContext).Model;
            int targetIndex = MemberList.IndexOf(oldModel);
            MemberList.RemoveAt(targetIndex);
            MemberList.Insert(targetIndex, newModel);
        }

        /// <summary>
        /// レコード削除
        /// </summary>
        /// <param name="record">選択レコード</param>
        private void DeleteRecord(object record)
        {
            MemberModel selectedModel = (MemberModel)record;

            MemberList.Remove(selectedModel);
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

// TODO: DB操作