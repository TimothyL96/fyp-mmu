using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fyp1_prototype
{
	class ItemGripped
	{
		//	Indicate on which item the hand cursor is on by index of canvas.children
		public int HandCursorOn { get; set; }

		//	Flag for setting hand cursor on left & top distant
		public bool HandCursorOnSet { get; set; }

		//	When the hand cursor is gripping the item, record the left and right distance to move the item in the right ratio from the point of grip
		public double HandCursorOnLeftDistant { get; set; }
		public double HandCursorOnTopDistant { get; set; }

		//	Store all the lives and scores
		public int CurrentScore { get; set; }
		public int CurrentLives { get; set; }
		public int GameMode { get; set; }

		//	Constructor
		public ItemGripped()
		{

		}
	}
}
