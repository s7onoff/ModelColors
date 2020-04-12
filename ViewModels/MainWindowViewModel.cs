using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RCModelColors.Classes;
using RCModelColors;
using System.Windows.Input;

namespace RCModelColors
{
    public class MainWindowViewModel: INotifyPropertyChanged 
    {
        #region Props

        private double lightness;

        public double Lightness
        {
            get { return lightness; }
            set 
            {
                if(value != lightness)
                { 
                    lightness = value;
                    INotifyPropertyChanged(nameof(Lightness));
                }
            }
        }

        private double saturation;

        public double Saturation
        {
            get { return saturation; }
            set 
            {
                if (value != saturation)
                {
                    saturation = value;
                    INotifyPropertyChanged(nameof(Saturation));
                }
            }
        }

        private string[] platesPrefixes;

        public string[] PlatesPrefixes
        {
            get { return platesPrefixes; }
            set 
            {
                platesPrefixes = value;
                INotifyPropertyChanged(nameof(PlatesPrefixes));
            }
        }

        #endregion

        public ICommand ButtonCommand { protected get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        private void INotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
