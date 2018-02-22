using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace fyp1_prototype
{
	class ItemDrop
	{
		public int ItemType { get; set; }
		public int DropSpeed { get; set; }
		public int ItemWidth { get; set; }
		public int ItemHeight { get; set; }
		public int VerticalMaxLength { get; set; }
		public ImageSource ImageSource { get; set; }

		public Image ItemImage { get; set; }
		public Image BlueBin { get; set; }
		public Image OrangeBin { get; set; }
		public Image BrownBin { get; set; }
		public Canvas CanvasGame { get; set; }
		public ItemGripped ItemGrippedObject { get; set; }

		//	Value is -1, 0 or 1
		private int CurrentScore = 0;
		private int CurrentLives = 0;

		//	Game Mode
		private int GameMode;

		DispatcherTimer timerPushImage = new DispatcherTimer();

		public ItemDrop()
		{
			//	Start pushing images down
			timerPushImage.Tick += new EventHandler(Tick_PushImage);
			timerPushImage.Interval = TimeSpan.FromMilliseconds(50);
			timerPushImage.Start();
		}

		private void Tick_PushImage(object source, EventArgs e)
		{
			//	Record the image index in canvas children
			int imageIndex = -1;

			//	Index might be out of range after the image is removed
			try
			{
				imageIndex = CanvasGame.Children.IndexOf(ItemImage);
			}
			catch (Exception)
			{

			}

			//	Skip if this item is being gripped or is removed
			if (imageIndex == ItemGrippedObject.HandCursorOn || imageIndex == -1)
				return;
			
			//	Get the points
			Point itemPoint = ItemImage.TranslatePoint(new Point(0, 0), CanvasGame);
			Point blueBinPoint = BlueBin.TranslatePoint(new Point(0, 0), CanvasGame);
			Point orangeBinPoint = OrangeBin.TranslatePoint(new Point(0, 0), CanvasGame);
			Point brownBinPoint = BrownBin.TranslatePoint(new Point(0, 0), CanvasGame);

			//	Push the image down
			Canvas.SetTop(ItemImage, itemPoint.Y + DropSpeed);

			//	Update the item point
			itemPoint = ItemImage.TranslatePoint(new Point(0, 0), CanvasGame);

			//	If the image passed the max vertical length then stop it
			if (itemPoint.Y >= VerticalMaxLength && imageIndex != ItemGrippedObject.HandCursorOn)
			{
				timerPushImage.Stop();

				CanvasGame.Children.Remove(ItemImage);

				//	Check if item touches any bins
				if (itemPoint.X + ItemWidth * 0.75 >= blueBinPoint.X && itemPoint.X + ItemWidth * 0.25 <= blueBinPoint.X + BlueBin.ActualWidth)
				{
					//	Blue bin. Tag = 0
					if (ItemType == 0)
					{
						CurrentScore++;
					}
					else if (GameMode == 0)
					{
						CurrentLives--;
					}
				}
				else if (itemPoint.X + ItemWidth * 0.75 >= orangeBinPoint.X && itemPoint.X + ItemWidth * 0.25 <= orangeBinPoint.X + OrangeBin.ActualWidth)
				{
					//	Orange bin. Tag = 1
					if (ItemType == 1)
					{
						CurrentScore++;
					}
					else if (GameMode == 0)
					{
						CurrentLives--;
					}
				}
				else if (itemPoint.X + ItemWidth * 0.75 >= brownBinPoint.X && itemPoint.X + ItemWidth * 0.25 <= brownBinPoint.X + BrownBin.ActualWidth)
				{
					//	Brown bin. Tag = 2
					if (ItemType == 2)
					{
						CurrentScore++;
					}
					else if (GameMode == 0)
					{
						CurrentLives--;
					}
				}
				else if (GameMode == 0)
				{
					//	When the item never touches the bin but on the floor, easier to lose if uncommented
					//currentLives--;
				}

				ItemGrippedObject.CurrentLives += CurrentLives;
				ItemGrippedObject.CurrentScore += CurrentScore;
			}
		}
	}
}
