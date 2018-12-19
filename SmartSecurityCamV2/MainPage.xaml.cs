using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SmartSecurityCamV2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int SENSOR_PIN = 4;
        private GpioPin sensorPin;

        public MainPage()
        {
            this.InitializeComponent();
            InitGpio();
            Unloaded += MainPageUnloaded;
        }

        private void InitGpio()
        {
            var gpio = GpioController.GetDefault();
            sensorPin = gpio.OpenPin(SENSOR_PIN);   
            sensorPin.SetDriveMode(GpioPinDriveMode.Input);
            sensorPin.ValueChanged += SensorPinValueChanged;
        }

        private void SensorPinValueChanged(GpioPin sender, GpioPinValueChangedEventArgs arg)
        {
            bool isOn = (arg.Edge == GpioPinEdge.RisingEdge);

            if (isOn)
            {
                // take picture
            }
        }

        private void MainPageUnloaded(object sender, object arg)
        {
            sensorPin.Dispose();
        }
    }
}
