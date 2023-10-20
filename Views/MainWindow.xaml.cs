using ModelColors.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ModelColors.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

        }

        private void ColorSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            BindingExpression bindingExpression = ((Slider)sender).GetBindingExpression(Slider.ValueProperty);
            bindingExpression.UpdateSource();
        }
    }
}
