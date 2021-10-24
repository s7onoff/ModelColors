using RCModelColors.Classes;
using RCModelColors.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RCModelColors.Controls
{
    /// <summary>
    /// Interaction logic for PropItemControl.xaml
    /// </summary>
    public partial class PropItemControl : UserControl
    {


        public PropItem PropItem
        {
            get { return (PropItem)GetValue(PropItemProperty); }
            set { SetValue(PropItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PropItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropItemProperty =
            DependencyProperty.Register("PropItem", typeof(PropItem), typeof(PropItemControl), new PropertyMetadata(new PropItem { Name = "Property Item Name", Hue = 255, Saturation = 255, Lightness = 100 }, SetValues));

        private static void SetValues(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropItemControl control = d as PropItemControl;
            if (control != null)
            {
                control.nameTextBlock.Text = (e.NewValue as PropItem).Name;
                control.hueTextBlock.Text = (e.NewValue as PropItem).Hue.ToString();
                control.saturationTextBlock.Text = (e.NewValue as PropItem).Saturation.ToString();
                control.luminanceTextBlock.Text = (e.NewValue as PropItem).Lightness.ToString();
                control.colorButton.Background = new BrushConverter().ConvertFromString((e.NewValue as PropItem).GetColorString()) as SolidColorBrush;
            }

        }

        public PropItemControl()
        {
            InitializeComponent();
        }

        private void colorButton_Click(object sender, RoutedEventArgs e)
        {
            //ColorPicker colorPicker = new ColorPicker((e.Source as PropItem).Red, (e.Source as PropItem).Green, (e.Source as PropItem).Blue);

            //colorPicker.ShowDialog();            
        }
    }
}
