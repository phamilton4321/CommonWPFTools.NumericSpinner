using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CommonWPFTools.UserControls
{
    /// <summary>
    /// Interaction logic for NumericSpinner.xaml
    /// </summary>
    public partial class NumericSpinner : UserControl
    {
        /// <summary>
        /// Delay after which the error message will disapear.
        /// </summary>
        private const int ErrorMessageTimoutDelay = 3000;
        private readonly Brush ControlsBorderBrush;
        private readonly Thickness ControlBorderThickness;
        
        public NumericSpinner()
        {
            InitializeComponent();
            //Save chosen border color and thickness for later. We'll be using it for a blinking effect in case of out of manual entered range values.
            ControlsBorderBrush = MyBorder.BorderBrush;
            ControlBorderThickness = MyBorder.BorderThickness;
            DataContext = this;
        }

        /// <summary>
        /// Define your error message string here
        /// </summary>
        private string ErrorMessageInPopup
        {
            /// <summary>
            /// Message which pops up if value is out of range
            /// </summary>
            get => string.Concat
                (
                    "Ungültiger Wert",
                    "\r\n",
                    "Werte müssen zwischen ",
                        MinValue.ToString(),
                    " und ",
                        MaxValue.ToString(),
                    " sein!"
                );
        }
        
        #region ValueProperty

        public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register(
                "Value",
                typeof(int),
                typeof(NumericSpinner),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set
            {
                var newValue = Validatenewvalue(value);
                SetValue(ValueProperty, newValue);
                if (newValue != value)
                    TextboxMain.Text = newValue.ToString();
            }
        }

        private int Validatenewvalue(object newvalue)
        {
            if (newvalue is int _newData)
            {
                if (_newData < MinValue)
                {
                    ShowErrorPopup();
                    _newData = MinValue;
                }
                if (_newData > MaxValue)
                {
                    ShowErrorPopup();
                    _newData = MaxValue;
                }
                return _newData;
            }
            else
            {
                ShowErrorPopup();
                return MinValue;
            }
        }

        #endregion

        #region StepProperty

        public readonly static DependencyProperty StepProperty = DependencyProperty.Register(
            "Step",
            typeof(int),
            typeof(NumericSpinner),
            new PropertyMetadata(1));

        public int Step
        {
            get { return (int)GetValue(StepProperty); }
            set
            {
                SetValue(StepProperty, value);
            }
        }

        #endregion

        #region MinValueProperty

        public readonly static DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue",
            typeof(int),
            typeof(NumericSpinner),
            new PropertyMetadata(0));

        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set
            {
                if (value > MaxValue)
                    MaxValue = value;
                SetValue(MinValueProperty, value);
            }
        }

        #endregion

        #region MaxValueProperty

        public readonly static DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue",
            typeof(int),
            typeof(NumericSpinner),
            new PropertyMetadata(int.MaxValue));
        

        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set
            {
                if (value < MinValue)
                    value = MinValue;
                SetValue(MaxValueProperty, value);
            }
        }

        #endregion


        #region Buttons
        RelayCommand _StepDownButton; public ICommand StepDownButton
        {
            get
            {
                if (_StepDownButton == null)
                {
                    _StepDownButton = new RelayCommand(
                        param => StepDownButton_Executed(),
                        param => StepDownButton_CanExecute);
                }
                return _StepDownButton;
            }
        }
        private bool StepDownButton_CanExecute => Value > MinValue;
        private void StepDownButton_Executed() => Value -= Step;

        RelayCommand _StepUpButton; public ICommand StepUpButton
        {
            get
            {
                if (_StepUpButton == null)
                {
                    _StepUpButton = new RelayCommand(
                        param => StepUpButton_Executed(),
                        param => StepUpButton_CanExecute);
                }
                return _StepUpButton;
            }
        }
        private bool StepUpButton_CanExecute => Value < MaxValue;
        private void StepUpButton_Executed() => Value += Step;

        RelayCommand _MouseWheelUpAction; public ICommand MouseWheelUpAction
        {
            get
            {
                if (_MouseWheelUpAction == null)
                {
                    _MouseWheelUpAction = new RelayCommand(
                        param => MouseWheelUpAction_Executed(),
                        param => MouseWheelUpAction_CanExecute);
                }
                return _MouseWheelUpAction;
            }
        }
        private bool MouseWheelUpAction_CanExecute => Value < MaxValue;
        private void MouseWheelUpAction_Executed()
        {
            Value += Step;
            CommandManager.InvalidateRequerySuggested();
        }

        RelayCommand _MouseWheelDownAction; public ICommand MouseWheelDownAction
        {
            get
            {
                if (_MouseWheelDownAction == null)
                {
                    _MouseWheelDownAction = new RelayCommand(
                        param => MouseWheelDownAction_Executed(),
                        param => MouseWheelDownAction_CanExecute);
                }
                return _MouseWheelDownAction;
            }
        }
        private bool MouseWheelDownAction_CanExecute => Value > MinValue;
        private void MouseWheelDownAction_Executed()
        {
            Value -= Step;
            CommandManager.InvalidateRequerySuggested();
        }


        #endregion

        private void TextBoxMain_TextChanged(object sender, TextChangedEventArgs e)
        {
            var Error = false;
            
            if (string.IsNullOrEmpty(TextboxMain.Text))
                return;
                
            if (!int.TryParse(TextboxMain.Text, out _))
            {
                e.Handled = true;
                TextboxMain.Text = Value.ToString();
                ShowErrorPopup();
                return;
            }
            if (Value < MinValue) { Value = MinValue; Error = true; }
            if (Value > MaxValue) { Value = MaxValue; Error = true; }

            if (Error)
            {
                ShowErrorPopup();
            }
        }

        private async void ShowErrorPopup()
        {
            var tt = new ToolTip()
            {
                Content = ErrorMessageInPopup,
                HasDropShadow = true,
                Placement = System.Windows.Controls.Primitives.PlacementMode.Right,
                PlacementTarget =MyBorder,

            };
            tt.IsOpen = true;
            for (var i = 0; i < 5; i++)
            {
                await System.Threading.Tasks.Task.Delay(125);
                MyBorder.BorderBrush = Brushes.Red;
                MyBorder.BorderThickness = new Thickness(2);
                await System.Threading.Tasks.Task.Delay(125);
                MyBorder.BorderBrush = ControlsBorderBrush;
                MyBorder.BorderThickness = ControlBorderThickness;
            }
            await System.Threading.Tasks.Task.Delay(ErrorMessageTimoutDelay);
            tt.IsOpen = false;
        }

    }


    public class MouseWheelUp : MouseGesture
    {
        public MouseWheelUp() : base(MouseAction.WheelClick)
        {
        }

        public MouseWheelUp(ModifierKeys modifiers) : base(MouseAction.WheelClick, modifiers)
        {
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (!base.Matches(targetElement, inputEventArgs)) return false;
            if (!(inputEventArgs is MouseWheelEventArgs)) return false;
            var args = (MouseWheelEventArgs)inputEventArgs;
            return args.Delta > 0;
        }
    }
    public class MouseWheelDown : MouseGesture
    {
        public MouseWheelDown() : base(MouseAction.WheelClick)
        {
        }

        public MouseWheelDown(ModifierKeys modifiers) : base(MouseAction.WheelClick, modifiers)
        {
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (!base.Matches(targetElement, inputEventArgs)) return false;
            if (!(inputEventArgs is MouseWheelEventArgs)) return false;
            var args = (MouseWheelEventArgs)inputEventArgs;
            return args.Delta < 0;
        }
    }

}
