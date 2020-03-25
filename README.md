# CommonWPFTools.NumericSpinner
Numeric Up Down Spinner for WPF .Net Core

![Image of Spinner](https://github.com/phamilton4321/CommonWPFTools.NumericSpinner/raw/master/NumericSpinner_default.png "Example view of the control")

This Usercontrol will remind me and help you to implement a Numeric Spinner into an WPF XAML file.

It offers:
- MinValue:          Minimum integer value the control will accept
- MaxValue:          Maximum integer value the control will accept
- Value:             Value to start with at the beginning
- CornerRadiusValue: Corner Radius of the control

## Sample usage
In you xaml file add
`xmlns:usercontrols="clr-namespace:CommonWPFTools.UserControls"` to your Window. And then insert the control like this `<usercontrols:NumericSpinner MinValue="1" MaxValue="15" Value="5" CornerRadiusValue="3"/>`.


---

*An example is also in the solution*

---
Credits to Yassine for the base, that unfortunatley doesn't run for me.
See his [Repository](https://stopbyte.com/t/free-wpf-numeric-spinner-numericupdown/499) as well.
