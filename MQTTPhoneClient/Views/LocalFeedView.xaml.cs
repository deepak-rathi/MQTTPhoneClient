using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using KittyHawk.MqttLib.Client;
using KittyHawk.MqttLib.Collections;
using KittyHawk.MqttLib.Messages;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MQTTPhoneClient.Views
{
    public partial class LocalFeedView : PhoneApplicationPage
    {
        readonly Dictionary<string, SubscriptionClient> _subscriptions = new Dictionary<string, SubscriptionClient>();

        public LocalFeedView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            foreach (var subClient in _subscriptions.Values)
            {
                subClient.Close();
            }
            _subscriptions.Clear();
        }

        private void MqttClientOnPublishReceived(MqttPublishMessage msg)
        {
            AppendLogMessage(MessageLevel.Info, string.Format("Publish received for topic: {0}, Data={1}", msg.TopicName, msg.StringPayload));
        }

        private async void PublishButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await App.Instance.MqttClient.PublishStringAsync(PublishTopic.Text, PublishData.Text);
                AppendLogMessage(MessageLevel.Info, "Successfully published data to topic.");
            }
            catch (Exception ex)
            {
                AppendLogMessage(MessageLevel.Error, "ERROR: Publish failed with error: " + ex.Message);
            }
        }

        private void SubscribeButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_subscriptions.ContainsKey(PublishTopic.Text))
                {
                    var subscription = new SubscriptionItem
                    {
                        QualityOfService = QualityOfService.ExactlyOnce,
                        TopicName = PublishTopic.Text
                    };
                    SubscriptionClient subClient = App.Instance.MqttClient.CreateSubscription(subscription);
                    _subscriptions.Add(PublishTopic.Text, subClient);
                    subClient.OnMessage(MqttClientOnPublishReceived);
                }

                AppendLogMessage(MessageLevel.Info, "Successfully subscribed to topic.");
            }
            catch (Exception ex)
            {
                AppendLogMessage(MessageLevel.Error, "ERROR: Subscribe failed with error: " + ex.Message);
            }
        }

        private void UnsubscribetButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_subscriptions.ContainsKey(PublishTopic.Text))
                {
                    SubscriptionClient subClient = _subscriptions[PublishTopic.Text];
                    subClient.Close();
                    _subscriptions.Remove(PublishTopic.Text);
                }
                AppendLogMessage(MessageLevel.Info, "Successfully unsubscribed to topic.");
            }
            catch (Exception ex)
            {
                AppendLogMessage(MessageLevel.Error, "ERROR: Unsubscribe failed with error: " + ex.Message);
            }
        }

        private void AppendLogMessage(MessageLevel level, string msg)
        {
            LogViewer.AddMessage(level, msg);
        }
    }
}