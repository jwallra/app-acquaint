﻿using FormsToolkit;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;

//[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Acquaint.XForms
{
	public partial class App : Application
	{
        public App()
        {
            InitializeComponent();

            SubscribeToDisplayAlertMessages();

            // The navigation logic startup needs to diverge per platform in order to meet the UX design requirements
            if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
            {
                // if this is an Android device, set the MainPage to a new SplashPage
                MainPage = new SplashPage();
            }
            else
            {
                // create a new NavigationPage, with a new AcquaintanceListPage set as the Root
                var navPage =
                    new NavigationPage(
                        new AcquaintanceListPage()
                        {
                            BindingContext = new AcquaintanceListViewModel(),
                            Title = "Acquaintances"
                        })
                    {
                        BarBackgroundColor = Color.FromHex("547799")
                    };

                navPage.BarTextColor = Color.White;

                // set the MainPage of the app to the navPage
                MainPage = navPage;
            }

            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start("android=34d5ed40-4ff1-4db4-9ef6-0eefbf97e8ab;uwp=34d5ed40-4ff1-4db4-9ef6-0eefbf97e8ab;ios=7056d0a8-3a01-49e4-8fca-f5eff47839df", typeof(Analytics));
            Analytics.TrackEvent("StartCalled"+ System.DateTime.Now.ToString("HH:MM"));


        }

        /// <summary>
        /// Subscribes to messages for displaying alerts.
        /// </summary>
        static void SubscribeToDisplayAlertMessages()
		{
			MessagingService.Current.Subscribe<MessagingServiceAlert>(MessageKeys.DisplayAlert, async (service, info) => {
				var task = Current?.MainPage?.DisplayAlert(info.Title, info.Message, info.Cancel);
				if (task != null)
				{
					await task;
					info?.OnCompleted?.Invoke();
				}
			});

			MessagingService.Current.Subscribe<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, async (service, info) => {
				var task = Current?.MainPage?.DisplayAlert(info.Title, info.Question, info.Positive, info.Negative);
				if (task != null)
				{
					var result = await task;
					info?.OnCompleted?.Invoke(result);
				}
			});
		}
	}
}

