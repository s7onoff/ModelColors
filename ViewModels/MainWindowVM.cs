using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using RCModelColors.Classes;

namespace RCModelColors.ViewModels
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        public TeklaInteraction TeklaInteraction = new TeklaInteraction();
        public DBInteraction DBInteraction = new DBInteraction();
        public ColorLogic ColorLogic = new ColorLogic();

        #region Props

        private double lightness;
        public double Lightness
        {
            get { return lightness; }
            set
            {
                if (value != lightness)
                {
                    lightness = value;
                    OnPropertyChanged(nameof(Lightness));
                    ColorLogic.UpdateColors((int)Saturation, (int)Lightness);
                    RefreshTable();
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
                    OnPropertyChanged(nameof(Saturation));
                    ColorLogic.UpdateColors((int)Saturation, (int)Lightness);
                    RefreshTable();
                }
            }
        }

        private string platesPrefixes;

        public string PlatesPrefixes
        {
            get { return platesPrefixes; }
            set
            {
                platesPrefixes = value;
                TeklaInteraction.PlatePrefixesArray = platesPrefixes.Replace(" ", "").Replace(";", ",").Split(',');
                OnPropertyChanged(nameof(PlatesPrefixes));
            }
        }

        private string ignoredPrefixes;

        public string IgnoredPrefixes
        {
            get { return ignoredPrefixes; }
            set
            {
                ignoredPrefixes = value;
                TeklaInteraction.IgnoredPrefixesArray = ignoredPrefixes.Replace(" ", "").Replace(";", ",").Split(',');
                OnPropertyChanged(nameof(IgnoredPrefixes));
            }
        }

        private string filterName;

        public string FilterName
        {
            get { return filterName; }
            set
            {
                filterName = value;
                OnPropertyChanged(nameof(FilterName));
            }
        }
        
        private bool platesIgnored;

        public bool PlatesIgnored
        {
            get { return platesIgnored; }
            set
            {
                platesIgnored = value;
                TeklaInteraction.PlatesIgnored = value;
                OnPropertyChanged(nameof(PlatesIgnored));
            }
        }

        private bool beamsIgnored;

        public bool BeamsIgnored
        {
            get { return platesIgnored; }
            set
            {
                platesIgnored = value;
                TeklaInteraction.BeamsIgnored = value;
                OnPropertyChanged(nameof(BeamsIgnored));
            }
        }

        private bool filterIsApplied;

        public bool FilterIsApplied
        {
            get { return filterIsApplied; }
            set
            {
                filterIsApplied = value;
                OnPropertyChanged(nameof(FilterIsApplied));
            }
        }

        private List<PropItem> propItems;

        public List<PropItem> PropItems
        {
            get { return propItems; }
            set
            {
                propItems = value;
                OnPropertyChanged(nameof(PropItems));
            }
        }

        public bool ButtonsEnabled { get; set; }

        #endregion

        #region Methods
        public void RefreshTable()
        {
            PropItems = DBInteraction.ReadDatabase();
        }

        #endregion


        public ReadWholeModelCommand ReadWholeModel { get; set; }
        public ReadSelectedElementsCommand ReadSelectedElements { get; set; }
        public ShuffleColorsCommand ShuffleColors { get; set; }
        public CreateModelFilterCommand CreateModelFilter { get; set; }

        #region INotify
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public MainWindowVM()
        {
            FilterName = "RC_Profile_Colors";
            PlatesPrefixes = "BL, BPL, FB, FL, FLT, FPL, PL, PLATE, —, ПВ, Полоса, Риф, ЧРиф";
            IgnoredPrefixes = "NUT, WASHER";

            FilterIsApplied = true;
            PlatesIgnored = false;

            Lightness = 50;
            Saturation = 70;

            ButtonsEnabled = true;

            ReadWholeModel = new ReadWholeModelCommand(this);
            ReadSelectedElements = new ReadSelectedElementsCommand(this);
            ShuffleColors = new ShuffleColorsCommand(this);
            CreateModelFilter = new CreateModelFilterCommand(this);

            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                //Просто чтобы не забыть
            }
        }
    }

    public abstract class ButtonCommand : ICommand
    {
        public MainWindowVM VM;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return true;
            //bool buttonsEnabled = true;
            //if (parameter!=null)
            //{
            //    buttonsEnabled = (bool)parameter;
            //}

            //return buttonsEnabled;
        }

        public abstract void Execute(object parameter);

        public ButtonCommand(MainWindowVM vm)
        {
            VM = vm;
        }
    }

    public class ReadWholeModelCommand : ButtonCommand
    {
        public ReadWholeModelCommand(MainWindowVM vm) : base(vm) { }
        public override void Execute(object parameter)
        {
            VM.TeklaInteraction.GetAllProfiles();
            VM.RefreshTable();
        }
    }
    public class ReadSelectedElementsCommand : ButtonCommand
    {
        public ReadSelectedElementsCommand(MainWindowVM vm) : base(vm) { }
        public override void Execute(object parameter)
        {
            VM.TeklaInteraction.GetSelectedProfiles();
            VM.RefreshTable();
        }
    }
    public class ShuffleColorsCommand : ButtonCommand
    {
        public ShuffleColorsCommand(MainWindowVM vm) : base(vm) { }
        public override void Execute(object parameter)
        {
            VM.ColorLogic.SetColors((int)VM.Saturation, (int)VM.Lightness); ;
            VM.RefreshTable();
        }
    }
    public class CreateModelFilterCommand : ButtonCommand
    {
        public CreateModelFilterCommand(MainWindowVM vm) : base(vm) { }
        public override void Execute(object parameter)
        {
            VM.TeklaInteraction.CreateFilter(VM.FilterIsApplied, VM.FilterName);
        }
    }
}
