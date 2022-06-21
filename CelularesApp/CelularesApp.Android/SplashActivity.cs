using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CelularesApp.Droid
{
    [Activity(Theme = "@style/Theme.Splash", NoHistory = true, MainLauncher = true, Icon = "@drawable/Icon")]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                
                base.OnCreate(savedInstanceState);
                System.Threading.Thread.Sleep(500);
                StartActivity(typeof(MainActivity));
            }
            catch (Exception)
            {

            }
            // Create your application here
        }
    }
}