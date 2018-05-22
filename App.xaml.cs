using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace tally
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public static SQLiteConnection connect;
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            try
            {
                LoadDatabase();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void LoadDatabase()
        { // Get a reference to the SQLite database
            connect = new SQLiteConnection("tally.db");
            string sql = @"CREATE TABLE IF NOT EXISTS [Tally](
                    [ID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    [date] VARCHAR(30),
                    [first_label] VARCHAR(140), 
                    [second_label] VARCHAR(140),
                    [money] VARCHAR(140), 
                    [detail] VARCHAR(140)
                  );";
            using (var statement = connect.Prepare(sql)) { statement.Step(); }
            using (var statement = connect.Prepare("SELECT ID, date, first_label, second_label, money, detail FROM Tally"))
            {
                while (SQLiteResult.ROW == statement.Step())
                {
                    var s = statement[1].ToString();
                    s = s.Substring(0, s.IndexOf(' '));
                    long ID = (long)statement[0];
                    DateTime date = new DateTime(int.Parse(s.Split('/')[0]), int.Parse(s.Split('/')[1]), int.Parse(s.Split('/')[2]));
                    string first_lable = (string)statement[2];
                    string second_lable = (string)statement[3];
                    string money = (string)statement[4];
                    string detail = (string)statement[5];
                    listview.Additem(ID, first_lable, second_lable, money, detail, date);
                }

            }
        }

        private static model.tallyitemViewModel listview = new model.tallyitemViewModel();

        internal static model.tallyitemViewModel Listview { get => listview; set => listview = value; }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }
    }
}
