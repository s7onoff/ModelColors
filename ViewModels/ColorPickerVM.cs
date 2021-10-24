using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCModelColors.ViewModels
{
    class ColorPickerVM
    {
        #region Props

        private int red = 150;

        public int Red
        {
            get { return red; }
            set { red = value; }
        }

        private int green = 100;

        public int Green
        {
            get { return green; }
            set { green = value; }
        }

        private int blue = 13;

        public int Blue
        {
            get { return blue; }
            set { blue = value; }
        }

        private string redLineLeftColor;

        public string RedLineLeftColor
        {
            get { return "#" + 0.ToString("X2") + Green.ToString("X2") + Blue.ToString("X2") ; }
            set { redLineLeftColor = value; }
        }

        private string greenLineLeftColor;

        public string GreenLineLeftColor
        {
            get { return "#" + Red.ToString("X2") + 0.ToString("X2") + Blue.ToString("X2"); }
            set { greenLineLeftColor = value; }
        }

        private string blueLineLeftColor;

        public string BlueLineLeftColor
        {
            get { return "#" + Red.ToString("X2") + Green.ToString("X2") + 0.ToString("X2"); }
            set { blueLineLeftColor = value; }
        }

        private string redLineRightColor;

        public string RedLineRightColor
        {
            get { return "#" + 255.ToString("X2") + Green.ToString("X2") + Blue.ToString("X2"); }
            set { redLineRightColor = value; }
        }

        private string greenLineRightColor;

        public string GreenLineRightColor
        {
            get { return "#" + Red.ToString("X2") + 255.ToString("X2") + Blue.ToString("X2"); }
            set { greenLineRightColor = value; }
        }

        private string blueLineRightColor;

        public string BlueLineRightColor
        {
            get { return "#" + Red.ToString("X2") + Green.ToString("X2") + 255.ToString("X2"); }
            set { blueLineRightColor = value; }
        }


        private string rgbColor;

        public string RGBColor
        {
            get
            {
                return "#" + Red.ToString("X2") + Green.ToString("X2") + Blue.ToString("X2");
            }
            set { rgbColor = value; }
        }

        private string rgbAntiColor;

        public string RGBAntiColor
        {
            get
            {
                return "#" + (255-Red).ToString("X2") + (255-Green).ToString("X2") + (255-Blue).ToString("X2");
            }
            set { rgbAntiColor = value; }
        }
        #endregion


    }
}
