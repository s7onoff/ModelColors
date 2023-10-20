using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ModelColors.Classes;

namespace ModelColors.ViewModels
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
            get { return beamsIgnored; }
            set
            {
                beamsIgnored = value;
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

        private bool buttonsEnabled;

        public bool ButtonsEnabled
        {
            get { return buttonsEnabled; }
            set
            {
                buttonsEnabled = value;
                ConnectButtonVisibility=(value ? Visibility.Hidden : Visibility.Visible);
                OnPropertyChanged(nameof(ButtonsEnabled));
            }
        }

        private Visibility connectButtonVisibility;

        public Visibility ConnectButtonVisibility
        {
            get { return connectButtonVisibility; }
            set
            {
                connectButtonVisibility = value;
                OnPropertyChanged(nameof(connectButtonVisibility));
            }
        }



        private string teklaModelPath;

        public string TeklaModelPath
        {
            get { return teklaModelPath; }
            set
            {
                teklaModelPath = value.Replace(System.IO.Path.DirectorySeparatorChar, '/');
                OnPropertyChanged(nameof(TeklaModelPath));
            }
        }

        private string teklaModelName;

        public string TeklaModelName
        {
            get { return teklaModelName; }
            set
            {
                teklaModelName = System.IO.Path.GetFileNameWithoutExtension(value);
                OnPropertyChanged(nameof(TeklaModelName));
            }
        }

        private string encoding;

        public string ThisPCEncoding
        {
            get { return encoding; }
            set 
            { 
                encoding = value; 
                OnPropertyChanged(nameof(ThisPCEncoding));
            }
        }


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
            FilterName = "Colors_by_Profile";
            PlatesPrefixes = "BL, BPL, FB, FL, FLT, FPL, PL, PLATE, —, ПВ, Полоса, Риф, ЧРиф";
            IgnoredPrefixes = "NUT, WASHER";

            FilterIsApplied = true;
            PlatesIgnored = false;

            Lightness = 50;
            Saturation = 70;

            ButtonsEnabled = false;

            ReadWholeModel = new ReadWholeModelCommand(this);
            ReadSelectedElements = new ReadSelectedElementsCommand(this);
            ShuffleColors = new ShuffleColorsCommand(this);
            CreateModelFilter = new CreateModelFilterCommand(this);

            ThisPCEncoding = TeklaInteraction.ThisPCEncoding.CodePage.ToString();

            if (TeklaInteraction.Connect())
            {
                TeklaModelPath = TeklaInteraction.ModelPath;
                TeklaModelName = TeklaInteraction.ModelName;
                ButtonsEnabled = true;
            }
            else
            {
                // Check what version of Tekla needed:
                string teklaFullVersion = typeof(Tekla.Structures.Model.Model).Assembly.GetName().Version.ToString();
                string teklaVersion = teklaFullVersion.Substring(0,4) + ((teklaFullVersion[5] == '1') ? "i" : "");
                TeklaModelName = String.Concat("Cannot connect to Tekla Structures v", teklaVersion.ToString());
            }

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                TeklaModelName = "MODEL NAME";
                TeklaModelPath = "C:/Path/to/your/current/Tekla/Model";
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
            bool buttonsEnabled = true;
            if (parameter != null)
            {
                buttonsEnabled = (bool)parameter;
            }

            return buttonsEnabled;
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
            VM.TeklaInteraction.GetProfiles(false);
            VM.RefreshTable();
        }
    }
    public class ReadSelectedElementsCommand : ButtonCommand
    {
        public ReadSelectedElementsCommand(MainWindowVM vm) : base(vm) { }
        public override void Execute(object parameter)
        {
            VM.TeklaInteraction.GetProfiles(true);
            VM.RefreshTable();
        }
    }
    public class ShuffleColorsCommand : ButtonCommand
    {
        public ShuffleColorsCommand(MainWindowVM vm) : base(vm) { }
        public override void Execute(object parameter)
        {
            VM.ColorLogic.SetColors((int)VM.Saturation, (int)VM.Lightness);
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
