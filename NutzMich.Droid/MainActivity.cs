using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Views;

[assembly: UsesFeature("android.hardware.camera", Required = false)]
[assembly: UsesFeature("android.hardware.camera.autofocus", Required = false)]
namespace NutzMich.Droid
{
	[Activity(
			MainLauncher = true,
			ConfigurationChanges = Uno.UI.ActivityHelper.AllConfigChanges,
			WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
		)]
	public class MainActivity : Windows.UI.Xaml.ApplicationActivity
	{
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
			Xamarin.Essentials.Platform.Init(this, bundle);
			//ToastNotification
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}

