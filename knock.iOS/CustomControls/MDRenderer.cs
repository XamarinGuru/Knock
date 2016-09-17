using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Visual1993;
using Visual1993.iOS;
using System.Linq;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using knock.CustomControls;

[assembly: ExportRenderer (typeof(MDPage), typeof(MDRenderer))]
namespace Visual1993.iOS
{
	public class MDRenderer: PhoneMasterDetailRenderer
	{
		public MDRenderer ()
		{
		}
	}
}

