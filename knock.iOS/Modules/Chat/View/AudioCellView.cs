using System;

using Xamarin.Forms;

namespace knock
{
	public class AudioCellView : ContentView
	{
		public Label label;
		public AudioCellView()
		{
			label = new Label { Text = "Test text", WidthRequest=100, HeightRequest=50 };
			var buttPlay = new Button { Text="play"};
			Content = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Children = {label, buttPlay, new BoxView { HeightRequest=20, Color=Color.Red} }
			};
		}

	}
}


