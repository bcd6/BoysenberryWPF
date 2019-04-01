using System;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Data.Xml.Dom;
using System.Diagnostics;
using ToastNotifications;
using ToastNotifications.Position;
using System.Windows;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;

namespace Boysenberry.Services
{
    public class ToastNotificationUtil
    {
        //public static ToastNotification GetToast(string title, string content, string image = @"", string logo = @"C:\A\P\C\BoysenberryWPF\Assets\Logo.png")
        //{
        //    AdaptiveText _title = new AdaptiveText();
        //    AdaptiveText _content = new AdaptiveText();
        //    AdaptiveImage _image = new AdaptiveImage();
        //    ToastGenericAppLogo _logo = new ToastGenericAppLogo();
        //    ToastVisual visual = new ToastVisual();
        //    ToastBindingGeneric bindingGeneric = new ToastBindingGeneric();


        //    _title.Text = title;
        //    _content.Text = content;
        //    _image.Source = image;
        //    _logo.Source = logo;
        //    _logo.HintCrop = ToastGenericAppLogoCrop.Circle;
        //    bindingGeneric.Children.Add(_title);
        //    bindingGeneric.Children.Add(_content);
        //    bindingGeneric.Children.Add(_image);
        //    bindingGeneric.AppLogoOverride = _logo;
        //    visual.BindingGeneric = bindingGeneric;

        //    ToastContent toastContent = new ToastContent()
        //    {
        //        Visual = visual,
        //        //Actions = actions,
        //        // Arguments when the user taps body of toast
        //        //Launch = new QueryString() { { "action", "viewConversation" }, { "conversationId", "1231231" } }.ToString()
        //    };

        //    // And create the toast notification
        //    XmlDocument xml = new XmlDocument();
        //    xml.LoadXml(toastContent.GetContent());
        //    Debug.WriteLine(toastContent.GetContent());
        //    ToastNotification toast = new ToastNotification(xml);
        //    toast.ExpirationTime = DateTime.Now.AddDays(1);
        //    return toast;
        //}

        //public static void Show(string title, string content, string image = @"", string logo = @"C:\A\P\C\BoysenberryWPF\Assets\Logo.png")
        //{
        //    ToastNotificationManager.CreateToastNotifier("Boysenberry").Show(GetToast(title, content, image, logo));
        //}

        public static Notifier GetNotifier()
        {
            Notifier notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.TopRight,
                    offsetX: 0,
                    offsetY: 0);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
            return notifier;
        }

        public static void Show(string msg)
        {
            Show(msg, "info");
        }
        public static void Show(string msg,string level)
        {
            if (level == "info")
            {
                GetNotifier().ShowInformation(msg);
            }
            if (level == "succ")
            {
                GetNotifier().ShowSuccess(msg);
            }
            if (level == "warn")
            {
                GetNotifier().ShowWarning(msg);
            }
            if (level == "error")
            {
                GetNotifier().ShowError(msg);
            }
        }
    }
}
