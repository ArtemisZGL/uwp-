using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using tally.model;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace tally
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;
        public MainPage()
        {
            this.InitializeComponent();
            lv.DataContext = App.Listview.Allitems;
            Current = this;
            Tile();
        }
        internal static void Tile()
        {
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueueForSquare150x150(true);
            updater.EnableNotificationQueueForSquare310x310(true);
            updater.EnableNotificationQueueForWide310x150(true);
            updater.EnableNotificationQueue(true);
            updater.Clear();
            foreach (var n in App.Listview.Allitems)
            {
                Windows.Data.Xml.Dom.XmlDocument doc = new Windows.Data.Xml.Dom.XmlDocument();
                doc = TileService.CreateTiles(n);
                TileNotification tileNotification = new TileNotification(doc);
                updater.Update(tileNotification);
            }

        }
        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            paytype.SelectedIndex = 0;
            intype.SelectedIndex = 0;
            inf.Text = "";
            money.Text = "";
            create.Content = "create";
            date.Date = DateTimeOffset.Now;
        }
        public void todoitem_click(object sender, ItemClickEventArgs e)
        {
            model.tallyitems s = e.ClickedItem as model.tallyitems;
            create.Content = "update";
            inf.Text = s.detail;
            money.Text = s.money;
            date.Date = s.date;
            if(s.first_label == "收入")
            {
                paytype.SelectedIndex = 0;
                ItemCollection items = intype.Items;
                int count = 0;
                foreach(ComboBoxItem i in items)
                {
                    if (i.Content.ToString() == s.second_label)
                    {
                        intype.SelectedIndex = count;
                        break;
                    }
                    count++;
                }
            }
            else
            {
                paytype.SelectedIndex = 1;
                ItemCollection items = costtype.Items;
                int count = 0;
                foreach (ComboBoxItem i in items)
                {
                    if (i.Content.ToString() == s.second_label)
                    {
                        costtype.SelectedIndex = count;
                        break;
                    }
                    count++;
                }
            }
            
        }
        private void createClick(object sender, RoutedEventArgs e)
        {
            ComboBoxItem first_item = paytype.SelectedItem as ComboBoxItem;
            ComboBoxItem second_item = null;
            if (costtype.Visibility == Visibility.Visible)
                second_item = costtype.SelectedItem as ComboBoxItem;
            else if(intype.Visibility == Visibility.Visible)
                second_item = intype.SelectedItem as ComboBoxItem;
            else
            {
                showDialog();
            }


            if ((string)create.Content == "update")
            {
                var db = App.connect;
                string sql = @"UPDATE Tally SET date = ?, first_label = ?, second_label = ?, money = ?, detail = ? WHERE ID = ?";
                using (var res = db.Prepare(sql))
                {
                    res.Bind(1, date.Date.DateTime.ToString());
                    res.Bind(2, first_item.Content.ToString().Trim());
                    res.Bind(3, second_item.Content.ToString().Trim());
                    res.Bind(4, money.Text.Trim());
                    res.Bind(5, inf.Text.Trim());
                    res.Bind(6, App.Listview.Allitems[lv.SelectedIndex].id);
                    res.Step();

                }

                create.Content = "create";
                App.Listview.Updateitem(lv.SelectedIndex.ToString(), first_item.Content.ToString(), second_item.Content.ToString(), money.Text, inf.Text, date.Date);
                paytype.SelectedIndex = 0;
                intype.SelectedIndex = 0;
                inf.Text = "";
                money.Text = "";
                date.Date = DateTimeOffset.Now;
                Tile();
            }
            else
            {
                if (inf.Text == "" || date.Date < DateTimeOffset.Now)
                {
                    showDialog();
                }
                else
                {
                    var db = App.connect;
                    App.Listview.Additem(db.LastInsertRowId(), first_item.Content.ToString(), second_item.Content.ToString(), money.Text, inf.Text, date.Date);

                    string sql = @"INSERT INTO Tally (date, first_label, second_label, money, detail) VALUES (?,?,?,?,?)";
                    using (var res = db.Prepare(sql))
                    {
                        res.Bind(1, date.Date.DateTime.ToString());
                        res.Bind(2, first_item.Content.ToString().Trim());
                        res.Bind(3, second_item.Content.ToString().Trim());
                        res.Bind(4, money.Text.Trim());
                        res.Bind(5, inf.Text.Trim());
                        res.Step();
                    }

                    paytype.SelectedIndex = 0;
                    intype.SelectedIndex = 0;
                    inf.Text = "";
                    money.Text = "";
                    date.Date = DateTimeOffset.Now;
                    Tile();
                }
            }
        }
        private async void showDialog()
        {
            string inf = "请将上述内容填满";
            var msgDialog = new Windows.UI.Popups.MessageDialog(inf) { Title = "创建失败" };
            await msgDialog.ShowAsync();
        }
        private void cancelClick(object sender, RoutedEventArgs e)
        {
            paytype.SelectedIndex = 0;
            intype.SelectedIndex = 0;
            inf.Text = "";
            money.Text = "";
            create.Content = "create";
            date.Date = DateTimeOffset.Now;
        }
        private void deleteAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string)create.Content == "update")
            {
                var db = App.connect;
                string sql = @"DELETE FROM Tally WHERE ID = ?";
                using (var res = db.Prepare(sql))
                {
                    res.Bind(1, App.Listview.Allitems[MainPage.Current.lv.SelectedIndex].id);
                    res.Step();
                }
                App.Listview.Removeitem(MainPage.Current.lv.SelectedIndex.ToString());
                paytype.SelectedIndex = 0;
                intype.SelectedIndex = 0;
                inf.Text = "";
                money.Text = "";
                date.Date = DateTimeOffset.Now;
                Tile();
            }
        }

        private void paytype_select(object sender, SelectionChangedEventArgs e)
        {
            if(paytype.SelectedIndex == 1)
            {
                costlabel.Visibility = Visibility.Visible;
                costtype.Visibility = Visibility.Visible;
                inlabel.Visibility = Visibility.Collapsed;
                intype.Visibility = Visibility.Collapsed;
            }
            else if(paytype.SelectedIndex == 0)
            {
                costlabel.Visibility = Visibility.Collapsed;
                costtype.Visibility = Visibility.Collapsed;
                inlabel.Visibility = Visibility.Visible;
                intype.Visibility = Visibility.Visible;
            }
        }

        private void time_select(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = timetype.SelectedItem as ComboBoxItem;
            in_text.Text = item.Content.ToString() + "收入";
            out_text.Text = item.Content.ToString() + "支出";
        }

        private void showToast(string first, string second, DateTimeOffset date, string money)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = first
                            },
                            new AdaptiveText()
                            {
                                Text = second
                            },
                            new AdaptiveText()
                            {
                                Text = date.ToString()
                            },
                            new AdaptiveText()
                            {
                                Text = money
                            }
    }
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Inputs =
                    {
                        new ToastSelectionBox("snoozeTime")
                        {
                            DefaultSelectionBoxItemId = "15",
                            Items =
                            {
                                new ToastSelectionBoxItem("1", "1 minute"),
                                new ToastSelectionBoxItem("15", "15 minutes"),
                                new ToastSelectionBoxItem("60", "1 hour"),
                                new ToastSelectionBoxItem("240", "4 hours"),
                                new ToastSelectionBoxItem("1440", "1 day")
                            }
                        }
                    },
                    Buttons =
                    {
                        new ToastButtonSnooze()
                        {
                            SelectionBoxId = "snoozeTime"
                        },
                        new ToastButtonDismiss()
                    }
                },
                Launch = "action=viewEvent&eventId=1983",
                Scenario = ToastScenario.Reminder
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        private void remind_Click(object sender, RoutedEventArgs e)
        {
            dynamic ori = e.OriginalSource;
            tallyitems item = (tallyitems)ori.DataContext;
            showToast(item.first_label, item.second_label, item.date, item.money);
        }
    }


}
