using RCModelColors.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tekla.Structures;
using Tekla.Structures.Model;
using Tekla.Structures.Dialog;

namespace RCModelColors
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<PropItem> propItemList;
        public MainWindow()
        {
            InitializeComponent();

            propItemList = new List<PropItem>();

            if(TeklaInteraction.Connect())
            {
                TeklaModelPath.Text = App.teklaModelPath;
                TeklaModelName.Text = App.teklaModelName;
                //ConnectionStatusText.Text = "OK";
                //ConnectionStatusText.Background = (SolidColorBrush)(new BrushConverter().ConvertFromString("Lime"));
            }

            RefreshTable();
        }

        private void RefreshTable()
        {
            DBInteraction.ReadDatabase(propItemList, ref PropItemListView);
        }

        private void ReadWholeModel_Click(object sender, RoutedEventArgs e)
        {
            TeklaInteraction.GetAllProfiles();
            RefreshTable();
        }

        private void ReadSelectedElements_Click(object sender, RoutedEventArgs e)
        {
            TeklaInteraction.GetSelectedProfiles();
            RefreshTable();
        }

        private void ShuffleColors_Click(object sender, RoutedEventArgs e)
        {
            ColorLogic.SetColors((int)SaturationSlider.Value, (int)LightnessSlider.Value); ;
            RefreshTable();
        }

        private void CreateModelFilter_Click(object sender, RoutedEventArgs e)
        {
            TeklaInteraction.CreateFilter(applyFilterCheckBox.IsChecked.Value, RepFileName.Text);
        }

        private void ColorSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            //BindingExpression bindingExpression = ((Slider)sender).GetBindingExpression(Slider.ValueProperty);
            //bindingExpression.UpdateSource();
            ColorLogic.UpdateColors((int)SaturationSlider.Value, (int)LightnessSlider.Value);
            RefreshTable();
        }

        private void PlatesPrefixes_LostFocus(object sender, RoutedEventArgs e)
        {
            App.arrayOfPlatePrefixes = PlatesPrefixes.Text.Replace(" ", "").Replace(";", ",").Split(',');
            Array.ForEach(App.arrayOfPlatePrefixes, Console.WriteLine);
        }

        private void IgnoredPrefixes_LostFocus(object sender, RoutedEventArgs e)
        {
            App.ignoredPrefixes = IgnoredPrefixes.Text.Replace(" ", "").Replace(";", ",").Split(',');
            Array.ForEach(App.ignoredPrefixes, Console.WriteLine);
        }

    }
}
