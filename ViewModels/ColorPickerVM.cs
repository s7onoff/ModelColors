using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelColors.ViewModels
{
    class ColorPickerVM : INotifyPropertyChanged
    {
        #region INotify
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Props

        public String MyID { get; set; }

        private int red;

        public int Red
        {
            get { return red; }
            set
            {
                red = value;
                OnPropertyChanged(nameof(Red));
                UpdateGradients();
            }
        }

        private int green = 100;

        public int Green
        {
            get { return green; }
            set 
            { 
                green = value;
                OnPropertyChanged(nameof(Green));
                UpdateGradients();
            }
        }

        private int blue = 100;

        public int Blue
        {
            get { return blue; }
            set 
            { 
                blue = value;
                OnPropertyChanged(nameof(Blue));
                UpdateGradients();
            }
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

        #region Methods
        private void UpdateGradients()
        {
            OnPropertyChanged(nameof(RedLineLeftColor));
            OnPropertyChanged(nameof(RedLineRightColor));
            OnPropertyChanged(nameof(GreenLineLeftColor));
            OnPropertyChanged(nameof(GreenLineRightColor));
            OnPropertyChanged(nameof(BlueLineLeftColor));
            OnPropertyChanged(nameof(BlueLineRightColor));
            OnPropertyChanged(nameof(RGBColor));
            OnPropertyChanged(nameof(rgbAntiColor));
        }
        #endregion

        #region Ctor
        public ColorPickerVM()
        {
            MyID = Guid.NewGuid().ToString();
        }

        #endregion
    }
}
