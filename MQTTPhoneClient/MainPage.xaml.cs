using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using KittyHawk.MqttLib.Client;
using KittyHawk.MqttLib.Net;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MQTTPhoneClient.Resources;
using MQTTPhoneClient.Views;

namespace MQTTPhoneClient
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        private async void ConnectButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (App.Instance.MqttClient != null && App.Instance.MqttClient.IsConnected)
            {
                NavigateToChildPage();
            }

            MqttConnectionInfo connectionInfo = GetConnectionInfo();

            App.Instance.SaveApplicationSetting(App.Instance.MqttConnectionSettingString, connectionInfo);

            if (UseSsl.IsChecked == true)
            {
                App.Instance.MqttClient = MqttClient.CreateSecureClient(SocketEncryption.Ssl);
            }
            else
            {
                App.Instance.MqttClient = MqttClient.CreateClient();
            }

            await App.Instance.ConnectToMqtt(connectionInfo);

            if (App.Instance.MqttClient.IsConnected)
            {
                AppendLogMessage(MessageLevel.Info, "Connection successful.");
                App.Instance.SaveApplicationSetting(App.Instance.MqttConnectedSettingString, true);
                NavigateToChildPage();
            }
            else
            {
                AppendLogMessage(MessageLevel.Error, "ERROR: Connection failed.");
                App.Instance.SaveApplicationSetting(App.Instance.MqttConnectedSettingString, false);
            }
        }

        private async void DisconnectButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await App.Instance.MqttClient.DisconnectAsync();
                AppendLogMessage(MessageLevel.Info, "Successfully disconnected.");
                App.Instance.SaveApplicationSetting(App.Instance.MqttConnectedSettingString, false);
            }
            catch (Exception ex)
            {
                AppendLogMessage(MessageLevel.Error, "ERROR: Disconnect failed with error: " + ex.Message);
            }
        }

        private MqttConnectionInfo GetConnectionInfo()
        {
            var connectionInfo = new MqttConnectionInfo();

            if (HiveMqLocal.IsChecked == true)
            {
                connectionInfo.HostName = "192.168.0.23";
            }
            else if (MosquittoLocal.IsChecked == true)
            {
                connectionInfo.HostName = "192.168.0.16";
            }
            else if (XivelyRemote.IsChecked == true)
            {
                connectionInfo.HostName = App.Instance.XivelyUrl;
                connectionInfo.Username = App.Instance.XivelyApiKey;
            }

            return connectionInfo;
        }

        private void NavigateToChildPage()
        {
            if (HiveMqLocal.IsChecked == true)
            {
                NavigationService.Navigate(new Uri("/Views/LocalFeedView.xaml", UriKind.Relative));
            }
            else if (MosquittoLocal.IsChecked == true)
            {
                NavigationService.Navigate(new Uri("/Views/LocalFeedView.xaml", UriKind.Relative));
            }
            else if (XivelyRemote.IsChecked == true)
            {
                NavigationService.Navigate(new Uri("/Views/XivelyFeedView.xaml", UriKind.Relative));
            }
        }

        private void AppendLogMessage(MessageLevel level, string msg)
        {
            LogViewer.AddMessage(level, msg);
        }
    }
}