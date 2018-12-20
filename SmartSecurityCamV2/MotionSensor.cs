using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace SmartSecurityCamV2
{
    class MotionSensor
    {
        private const int SENSOR_PIN = 4;
        private GpioPin sensorPin;

        public MotionSensor()
        {
            var gpio = GpioController.GetDefault();
            sensorPin = gpio.OpenPin(SENSOR_PIN);
            sensorPin.SetDriveMode(GpioPinDriveMode.Input);
            sensorPin.ValueChanged += SensorPinValue_Changed;
        }

        public void Dispose()
        {
            sensorPin.Dispose();
        }

        private void SensorPinValue_Changed(GpioPin sender, GpioPinValueChangedEventArgs arg)
        {
            if (arg.Edge == GpioPinEdge.RisingEdge)
            {
                Triggered();
            }
        }

        public event Action Triggered;
    }
}
