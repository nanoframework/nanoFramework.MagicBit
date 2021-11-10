using Iot.Device.Buzzer;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Adc;
using System.Device.Gpio;
using Iot.Device.Button;
using System.Device.I2c;
using System.Device.Pwm;
using Iot.Device.ServoMotor;
using Iot.Device.DCMotor;

namespace nanoFramework.MagicBit
{
    /// <summary>
    /// MagicBit board package
    /// </summary>
    public static class MagicBit
    {
        private const int PinMotor1A = 16;
        private const int PinMotor1B = 17;
        private const int PinMotor2A = 27;
        private const int PinMotor2B = 18;

        private static GpioController _gpio;
        private static GpioPin _ledRed;
        private static GpioPin _ledBlue;
        private static GpioPin _ledGreen;
        private static GpioPin _ledYellow;
        private static PwmChannel _bluePin;
        private static GpioPin _blackLeftPin;
        private static GpioPin _blackRightPin;
        private static ServoMotor _servoPinBlue;
        private static Buzzer _buzzer;
        private static AdcChannel _poten;
        private static AdcChannel _lumi;
        private static AdcController _adc;
        private static GpioButton _buttonLeft;
        private static GpioButton _buttonRight;
        private static DCMotor _motor1;
        private static DCMotor _motor2;
        private static Screen _screen;

        /// <summary>
        /// Sets the red led.
        /// </summary>
        public static GpioPin LedRed
        {
            get
            {
                if (_ledRed == null)
                {
                    _ledRed = _gpio.OpenPin(27, PinMode.Output);
                }

                return _ledRed;
            }
        }

        /// <summary>
        /// Sets the red led.
        /// </summary>
        public static GpioPin LedBlue
        {
            get
            {
                if (_ledBlue == null)
                {
                    _ledBlue = _gpio.OpenPin(17, PinMode.Output);
                }

                return _ledBlue;
            }
        }

        /// <summary>
        /// Sets the red led.
        /// </summary>
        public static GpioPin LedGreen
        {
            get
            {
                if (_ledGreen == null)
                {
                    _ledGreen = _gpio.OpenPin(16, PinMode.Output);
                }

                return _ledGreen;
            }
        }

        /// <summary>
        /// Sets the red led.
        /// </summary>
        public static GpioPin LedYellow
        {
            get
            {
                if (_ledYellow == null)
                {
                    _ledYellow = _gpio.OpenPin(18, PinMode.Output);
                }

                return _ledYellow;
            }
        }

        /// <summary>
        /// Gets the buzzer.
        /// </summary>
        public static Buzzer Buzzer
        {
            get
            {
                if (_buzzer == null)
                {
                    Configuration.SetPinFunction(25, DeviceFunction.PWM1);
                    _buzzer = new(25);
                }

                return _buzzer;
            }
        }

        /// <summary>
        /// Gets the potentiometer.
        /// </summary>
        public static AdcChannel Potentiometer
        {
            get
            {
                if (_poten == null)
                {
                    Configuration.SetPinFunction(39, DeviceFunction.ADC1_CH3);
                    _adc = _adc ?? new AdcController();
                    _poten = _adc.OpenChannel(3);
                }

                return _poten;
            }
        }

        /// <summary>
        /// Gets the potentiometer.
        /// </summary>
        public static AdcChannel Luminosity
        {
            get
            {
                if (_lumi == null)
                {
                    Configuration.SetPinFunction(36, DeviceFunction.ADC1_CH0);
                    _adc = _adc ?? new AdcController();
                    _lumi = _adc.OpenChannel(0);
                }

                return _lumi;
            }
        }

        /// <summary>
        /// Gets the left button.
        /// </summary>
        public static GpioButton ButtonLeft
        {
            get
            {
                if (_buttonLeft == null)
                {
                    _buttonLeft = new(35, _gpio, false, PinMode.InputPullUp);
                }

                return _buttonLeft;
            }
        }

        /// <summary>
        /// Gets the left button.
        /// </summary>
        public static GpioButton ButtonRight
        {
            get
            {
                if (_buttonRight == null)
                {
                    _buttonRight = new(34, _gpio, false, PinMode.InputPullUp);
                }

                return _buttonRight;
            }
        }

        /// <summary>
        /// Gets the motor 1.
        /// </summary>
        public static DCMotor Motor1
        {
            get
            {
                if (_motor1 == null)
                {
                    Configuration.SetPinFunction(PinMotor1B, DeviceFunction.PWM5);
                    _motor1 = DCMotor.Create(PwmChannel.CreateFromPin(PinMotor1B, 50, 0), PinMotor1A, _gpio, false, false);
                }

                return _motor1;
            }
        }

        /// <summary>
        /// Gets the motor 2.
        /// </summary>
        public static DCMotor Motor2
        {
            get
            {
                if (_motor2 == null)
                {
                    Configuration.SetPinFunction(PinMotor2B, DeviceFunction.PWM7);
                    _motor2 = DCMotor.Create(PwmChannel.CreateFromPin(PinMotor2B, 50, 0), PinMotor2A, _gpio, false, false);
                }

                return _motor2;
            }
        }

        /// <summary>
        /// Gets the GPIO controller.
        /// </summary>
        public static GpioController GpioController { get => _gpio; }

        static MagicBit()
        {
            // setup Gpio controller and make sure the motor are stopped
            _gpio = new();
            _gpio.OpenPin(PinMotor1A, PinMode.Output);
            _gpio.Write(PinMotor1A, PinValue.Low);
            _gpio.ClosePin(PinMotor1A);
            _gpio.OpenPin(PinMotor1B, PinMode.Output);
            _gpio.Write(PinMotor1B, PinValue.Low);
            _gpio.ClosePin(PinMotor1B);
            _gpio.OpenPin(PinMotor2A, PinMode.Output);
            _gpio.Write(PinMotor2A, PinValue.Low);
            _gpio.ClosePin(PinMotor2A);
            _gpio.OpenPin(PinMotor2B, PinMode.Output);
            _gpio.Write(PinMotor2B, PinValue.Low);
            _gpio.ClosePin(PinMotor2B);

            // Default I2C
            Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(22, DeviceFunction.I2C1_CLOCK);
        }

        /// <summary>
        /// Gets the screen.
        /// </summary>
        /// <remarks>The screen initialization takes a little bit of time, if you need the screen consider using it as early as possible in your code.</remarks>
        public static void InitializeScreen()
        {
            // If the screen is not needed, it's not going to be created
            // Note: initialization may take a little bit of time
            if (_screen == null)
            {
                _screen = new();                
            }
        }

        /// <summary>
        /// Gets the I2C device. Same as GetRed.
        /// </summary>
        /// <param name="deviceAddress">The device I2C address.</param>
        /// <returns>An I2C device.</returns>
        static public I2cDevice GetI2cDevice(int deviceAddress) => I2cDevice.Create(new I2cConnectionSettings(1, deviceAddress));

        /// <summary>
        /// Gets the I2C device. Same as GetI2cDevice.
        /// </summary>
        /// <param name="deviceAddress">The device I2C address.</param>
        /// <returns>An I2C device.</returns>
        static public I2cDevice GetRed(int deviceAddress) => I2cDevice.Create(new I2cConnectionSettings(1, deviceAddress));

        /// <summary>
        /// Gets the Blue pin and open it as desired.
        /// </summary>
        /// <param name="pinMode">The desired pin mode.</param>
        /// <returns>A GpioPin.</returns>
        static public PwmChannel GetPinBlue()
        {
            if (_bluePin == null)
            {
                Configuration.SetPinFunction(26, DeviceFunction.PWM13);
                _bluePin = PwmChannel.CreateFromPin(26);
            }

            return _bluePin;
        }

        /// <summary>
        /// Gets a servo motor on the blue pin.
        /// </summary>
        /// <param name="maxAngle">The maximum angle</param>
        /// <param name="minPuls">The minimum pulse.</param>
        /// <param name="maxPuls">The maximum pulse.</param>
        /// <returns></returns>
        static public ServoMotor GetServoPinBlue(int maxAngle = 180, int minPuls = 1000, int maxPuls = 2000)
        {
            if (_servoPinBlue == null)
            {
                _servoPinBlue = new(GetPinBlue(), maxAngle, minPuls, maxPuls);
            }

            return _servoPinBlue;
        }

        /// <summary>
        /// Gets the Black left pin and open it as desired.
        /// </summary>
        /// <param name="pinMode">The desired pin mode.</param>
        /// <returns>A GpioPin.</returns>
        /// 
        static public GpioPin GetPinBlackLeft(PinMode pinMode)
        {
            if (_blackLeftPin == null)
            {
                _blackLeftPin = _gpio.OpenPin(32, pinMode);
            }

            return _blackLeftPin;
        }

        /// <summary>
        /// Gets the black right pin and open it as desired.
        /// </summary>
        /// <param name="pinMode">The desired pin mode.</param>
        /// <returns>A GpioPin.</returns>
        static public GpioPin GetPinBlackRight(PinMode pinMode)
        {
            if (_blackRightPin == null)
            {
                _blackRightPin = _gpio.OpenPin(33, pinMode);
            }

            return _blackRightPin;
        }

    }
}
