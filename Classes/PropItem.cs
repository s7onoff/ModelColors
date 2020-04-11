using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklaColor = Tekla.Structures.Model.UI.Color;
using ColorMine.ColorSpaces;

namespace RCModelColors.Classes
{
	public class PropItem
	{
		private string name;
		
		[PrimaryKey]

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public double Red
		{
			get {
				var hsl = new Hsl
				{
					H = Hue,
					S = Saturation,
					L = Lightness
				};
				return hsl.ToRgb().R; }
		}

		public double Green
		{
			get
			{
				var hsl = new Hsl
				{
					H = Hue,
					S = Saturation,
					L = Lightness
				};
				return hsl.ToRgb().G;
			}
		}

		public double Blue
		{
			get
			{
				var hsl = new Hsl
				{
					H = Hue,
					S = Saturation,
					L = Lightness
				};
				return hsl.ToRgb().B;
			}
		}


		//private double red, green, blue = 0.0;

		#region HSL
		private int hue;

		public int Hue
		{
			get { return hue; }
			set
			{
				if (value < 360)
					hue = value;
				else
					hue = 360;
			}
		}

		private int saturation;

		public int Saturation
		{
			get { return saturation; }
			set
			{
				if (value < 100)
					saturation = value;
				else
					saturation = 100;
			}
		}

		private int lightness;

		public int Lightness
		{
			get { return lightness; }
			set
			{
				if (value < 100)
					lightness = value;
				else
					lightness = 100;
			}
		}
        #endregion

        public string GetColorString()
		{
			string colorString = $"#FF{((int)Red).ToString("X2")}{((int)Green).ToString("X2")}{((int)Blue).ToString("X2")}";
			return colorString;
		}

		public string GetTeklaNumber()
		{
			
			string hexNumber = $"{((int)Red).ToString("X2")}{((int)Green).ToString("X2")}{((int)Blue).ToString("X2")}";
			int intNumber = Int32.Parse(hexNumber, System.Globalization.NumberStyles.HexNumber);
			int intFinalNumber = intNumber + 100;
			return intNumber.ToString();
		}
		//public TeklaColor TeklaColor => new TeklaColor(red, green, blue);

	}
}
