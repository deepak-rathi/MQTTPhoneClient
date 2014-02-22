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
    public partial class XivelyFeedView : PhoneApplicationPage
    {
        private SubscriptionClient _subClient;

        public XivelyFeedView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var subscription = new SubscriptionItem
            {
                TopicName = App.Instance.XivelyFeedPath,
                QualityOfService = QualityOfService.AtMostOnce
            };

            if (_subClient == null)
            {
                _subClient = App.Instance.MqttClient.CreateSubscription(subscription);
            }

            _subClient.OnMessage(msg =>
            {
                string temp;
                string humid;
                ParseTempAndHumid(msg.StringPayload, out temp, out humid);

                Dispatcher.BeginInvoke(() =>
                {
                    TempTitle.Text = "Temperature (C) " + temp;
                    HumidTitle.Text = "Humidity (%) " + humid;
                    TempSlider.Value = float.Parse(temp);
                    HumidSlider.Value = float.Parse(humid);
                });

            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _subClient.Close();
            base.OnNavigatedFrom(e);
        }

        private void ParseTempAndHumid(string data, out string temp, out string humid)
        {
            temp = "0.0";
            humid = "0.0";

            string[] variables = data.Split(new[] { ',', '\n' });
            int i = 0;
            while (i < variables.Length)
            {
                if (variables[i] == "temperature")
                {
                    i += 2;
                    temp = variables[i];
                }
                if (variables[i] == "humidity")
                {
                    i += 2;
                    humid = variables[i];
                }
                i++;
            }
        }
    }
}