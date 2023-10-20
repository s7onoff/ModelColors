using ModelColors.Classes;
using ModelColors.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModelColors.Controls
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

        public PropItemControl()
        {
            InitializeComponent();
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

        private void colorButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (PropItem)(((Button)(e.Source)).DataContext);
            var vm = new ViewModels.ColorPickerVM
            {
                Red = item.Red,
                Blue = item.Blue,
                Green = item.Green
            };

            ColorPicker colorPicker = new ColorPicker();
            colorPicker.Owner = Application.Current.MainWindow;

            colorPicker.MainGrid.DataContext = vm;
            colorPicker.ShowDialog();

            var dbInteraction = new DBInteraction();

            item.SetHSLfromRGB(vm.Red, vm.Green, vm.Blue);

            dbInteraction.Store(item);

            ((ViewModels.MainWindowVM)((MainWindow)Application.Current.MainWindow).MainGrid.DataContext).RefreshTable();
        }

        private void Del_Click(object sender, RoutedEventArgs e)
        {
            var item = (PropItem)(((MenuItem)(e.Source)).DataContext);
            var dbInteraction = new DBInteraction();
            dbInteraction.Delete(item);
            ((ViewModels.MainWindowVM)((MainWindow)Application.Current.MainWindow).MainGrid.DataContext).RefreshTable();
        }
    }
}
