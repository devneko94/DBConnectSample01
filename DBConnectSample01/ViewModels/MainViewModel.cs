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
    public class MainViewModel
    {
        #region 定数
        /// <summary>
        /// 接続文字列
        /// </summary>
        private readonly string CONNECT_STRING
            = @"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\temp\SampleDB01.accdb";

        /// <summary>
        /// テーブル取得SQL文
        /// </summary>
        private readonly string SQL_SELECT = "SELECT * FROM MemberListTable ORDER BY MemberID";

        /// <summary>
        /// INSERT文テンプレート
        /// </summary>
        private readonly string SQL_INSERT = "INSERT INTO MemberListTable( MemberID, MemberName, MemberAddress )VALUES( {0}, {1}, {2} )";

        /// <summary>
        /// UPDATE文テンプレート
        /// </summary>
        private readonly string SQL_UPDATE = "UPDATE MemberListTable SET {0} WHERE {1}";

        /// <summary>
        /// DELETE文テンプレート
        /// </summary>
        private readonly string SQL_DELETE = "DELETE FROM MemberListTable WHERE {0}";
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

        #region フィールド
        /// <summary>
        /// データテーブル
        /// </summary>
        private DataTable _dataTable = new DataTable();

        /// <summary>
        /// DBコマンド
        /// </summary>
        private OleDbCommand _dbCommand = null;

        /// <summary>
        /// DBデータアダプター
        /// </summary>
        private OleDbDataAdapter _dataAdapter = null;

        /// <summary>
        /// DBコネクション
        /// </summary>
        private OleDbConnection _dbConnection = null;
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
            else
            {
                CloseDB();
            }
        }

        /// <summary>
        /// DB接続
        /// </summary>
        private bool OpenDB()
        {
            try
            {
                _dbConnection = new OleDbConnection();
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
            _dataTable.Clear();
            MemberList.Clear();

            try
            {
                _dbCommand = new OleDbCommand();
                _dataAdapter = new OleDbDataAdapter();
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

            foreach (DataRow dataRow in _dataTable.Rows)
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

            _dbCommand = null;
            _dataAdapter = null;
            _dataAdapter = null;
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

            MemberModel addModel = ((SubViewModel)sub.DataContext).Model;

            if (!OpenDB())
            {
                MessageBox.Show("DB接続に失敗しました。");
                return;
            }

            OleDbTransaction tran = _dbConnection.BeginTransaction();

            try
            {
                _dbCommand = new OleDbCommand();
                _dataAdapter = new OleDbDataAdapter();

                _dbCommand.Connection = _dbConnection;
                _dbCommand.Transaction = tran;

                _dbCommand.CommandText = string.Format(SQL_INSERT, $"'{addModel.MemberID}'", $"'{addModel.MemberName}'", $"'{addModel.MemberAddress}'");
                _dbCommand.ExecuteNonQuery();

                tran.Commit();

                ShowTable();
            }
            catch(Exception e)
            {
                tran.Rollback();
                MessageBox.Show(e.Message);
            }
            finally
            {
                CloseDB();
            }
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
                    CanPKEdit = false
                }
            };

            sub.ShowDialog();

            MemberModel updateModel = ((SubViewModel)sub.DataContext).Model;

            if (!OpenDB())
            {
                MessageBox.Show("DB接続に失敗しました。");
                return;
            }

            OleDbTransaction tran = _dbConnection.BeginTransaction();

            try
            {
                _dbCommand = new OleDbCommand();
                _dataAdapter = new OleDbDataAdapter();

                _dbCommand.Connection = _dbConnection;
                _dbCommand.Transaction = tran;

                string strSetValue = $"{nameof(updateModel.MemberName)} = '{updateModel.MemberName}', {nameof(updateModel.MemberAddress)} = '{updateModel.MemberAddress}'";
                string strWhere = $"{nameof(updateModel.MemberID)} = '{updateModel.MemberID}'";

                _dbCommand.CommandText = string.Format(SQL_UPDATE, strSetValue, strWhere);
                _dbCommand.ExecuteNonQuery();

                tran.Commit();

                ShowTable();
            }
            catch (Exception e)
            {
                tran.Rollback();
                MessageBox.Show(e.Message);
            }
            finally
            {
                CloseDB();
            }
        }

        /// <summary>
        /// レコード削除
        /// </summary>
        /// <param name="record">選択レコード</param>
        private void DeleteRecord(object record)
        {
            MemberModel deleteModel = (MemberModel)record;

            if (!OpenDB())
            {
                MessageBox.Show("DB接続に失敗しました。");
                return;
            }

            OleDbTransaction tran = _dbConnection.BeginTransaction();

            try
            {
                _dbCommand = new OleDbCommand();
                _dataAdapter = new OleDbDataAdapter();

                _dbCommand.Connection = _dbConnection;
                _dbCommand.Transaction = tran;

                string strWhere = $"{nameof(deleteModel.MemberID)} = '{deleteModel.MemberID}'";

                _dbCommand.CommandText = string.Format(SQL_DELETE, strWhere);
                _dbCommand.ExecuteNonQuery();

                tran.Commit();

                ShowTable();
            }
            catch (Exception e)
            {
                tran.Rollback();
                MessageBox.Show(e.Message);
            }
            finally
            {
                CloseDB();
            }
        }
        #endregion
    }
}