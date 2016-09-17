using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.Threading.Tasks;
using knock.Droid;
using Visual1993;

[assembly: ExportRenderer(typeof(Views.NoAnimationNavigationPage),typeof(NoAnimationNavigationRenderer))]
namespace knock.Droid
{
	public class NoAnimationNavigationRenderer : NavigationRenderer
	{

		public NoAnimationNavigationRenderer() :base()
		{
		}

		protected override Task<bool> OnPopToRootAsync (Page page, bool animated)
		{
			return base.OnPopToRootAsync (page, false);
		}

		protected override Task<bool> OnPopViewAsync(Page page, bool animated)
		{
			return base.OnPopViewAsync(page, false);
		}

		protected override Task<bool> OnPushAsync(Page view, bool animated)
		{
			return base.OnPushAsync(view, false);
		}
	}
}