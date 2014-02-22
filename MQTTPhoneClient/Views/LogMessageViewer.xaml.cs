using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MQTTPhoneClient.Views
{
    public enum MessageLevel
    {
        Info,
        Warning,
        Error,
        Debug
    }

    internal class MessageType
    {
        public string Level { get; set; }
        public string Message { get; set; }
    }

    public partial class LogMessageViewer : UserControl
    {
        public LogMessageViewer()
        {
            InitializeComponent();
        }

        public void AddMessage(MessageLevel level, string msg)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var item = new MessageType()
                {
                    Level = level.ToString(),
                    Message = msg
                };

                var itemView = new Button()
                {
                    //Template = this.Resources["LogItemControl"] as ControlTemplate,
                    ContentTemplate = this.Resources["LogItemData"] as DataTemplate,
                    Content = item,
                    Width = Viewer.ViewportWidth - Viewer.ScrollableWidth,
                };

                switch (level)
                {
                    case MessageLevel.Info:
                        itemView.Background = this.Resources["InfoBrush"] as Brush;
                        break;

                    default:
                        itemView.Background = this.Resources["ErrorBrush"] as Brush;
                        break;
                }

                ViewerContent.Children.Add(itemView);

                //double scrollAmount = Viewer.Height - Viewer.ScrollableHeight;
                //if (scrollAmount < 0)
                //{
                //    Viewer.ScrollToVerticalOffset(scrollAmount);
                //}
            });
        }
    }
}
