using Iot.Device.Ssd13xx;
using nanoFramework.MagicBit;
using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Diagnostics;
using System.Threading;

byte[] _heart = new byte[] {
            0b0100_0010,
            0b0110_0110,
            0b1111_1111,
            0b1111_1111,
            0b1111_1111,
            0b0011_1100,
            0b0011_1100,
            0b0001_1000,
        };

Debug.WriteLine("Hello MagicBit from nanoFramework!");
// Screen MUST be initialized if you want to use it
MagicBit.InitializeScreen();
CancellationTokenSource cancellationTokenSource;

demo:
// You can use the Screen primitives like this:
Screen.Clear();
Screen.Write(2, 0, "MagicBit", 2);
Screen.Write(2, 26, "loves .NET", 1, true);
Screen.Write(2, 40, "nanoFramework", 1, true);
Screen.Write(2, 50, "Clk right button", 1, false);
Screen.DrawBitmap(0, 26, 8, _heart);
Screen.DrawBitmap(Screen.Width - 9, 26, 8, _heart);
// You should make sure that you call Display
Screen.Display();
// You can get more primitives with Screen.Device.

// Simple way of getting the button status
while (!MagicBit.ButtonRight.IsPressed)
{
    Thread.Sleep(20);
}

cancellationTokenSource = new CancellationTokenSource();
// If you don't use the motors, you can comment this block and
// uncomment the leds
Console.Clear();
Console.WriteLine("Motors");
Console.CursorTop = 2;
Console.WriteLine("Motors will run reverse then rotate both direction");
var motor1 = MagicBit.Motor1;
var motor2 = MagicBit.Motor2;
motor1.Speed = -0.5;
motor2.Speed = -0.5;
Thread.Sleep(2000);
motor1.Speed = +0.5;
motor2.Speed = -0.5;
Thread.Sleep(2000);
motor1.Speed = -0.5;
motor2.Speed = +0.5;
Thread.Sleep(2000);
motor1.Speed = 0;
motor2.Speed = 0;

// You can use Console as well to display text on the screen
Console.Clear();
Console.WriteLine("Buzzer");
Console.CursorTop = 2;
Console.WriteLine("Clk left button to stop");
// You have events on the button and can register them
MagicBit.ButtonLeft.Press += (sender, e) =>
{
    cancellationTokenSource.Cancel();
};

var buzz = MagicBit.Buzzer;
for (int i = 0; i < 10; i++)
{
    buzz.PlayTone(500 + i * 25, 1000);
    if (cancellationTokenSource.Token.IsCancellationRequested)
    {
        break;
    }
}

// Uncomment to use the leds
// Important: this is not compatible with motor usage
//MagicBit.LedBlue.Write(PinValue.High);
//MagicBit.LedRed.Write(PinValue.Low);
//MagicBit.LedYellow.Write(PinValue.High);
//MagicBit.LedRed.Write(PinValue.High);
//MagicBit.LedGreen.Write(PinValue.High);

Console.Clear();
Console.WriteLine("Sensors");
Console.CursorTop = 2;
Console.WriteLine("Clk left button to stop");
cancellationTokenSource = new CancellationTokenSource();

while (!cancellationTokenSource.Token.IsCancellationRequested)
{
    var ratio = MagicBit.Potentiometer.ReadRatio();
    var lumi = MagicBit.Luminosity.ReadRatio();
    Console.CursorTop = 4;
    Console.CursorLeft = 0;
    Console.WriteLine($"Pot: {ratio * 100:N2}%  ");
    Console.CursorTop = 5;
    Console.CursorLeft = 0;
    Console.WriteLine($"lum: {lumi * 100:N2}%  ");
    cancellationTokenSource.Token.WaitHandle.WaitOne(200, true);
}

// Un comment to use the right black pin
//var blackRight = MagicBit.GetPinBlackRight(PinMode.Output);
//blackRight.Write(PinValue.High);

Console.Clear();
Console.WriteLine("Servo");
Console.CursorTop = 2;
Console.WriteLine("Clk left button to stop");
cancellationTokenSource = new CancellationTokenSource();
// The servo can be different and you can adjust the values as needed
var servo = MagicBit.GetServoPinBlue(180, 500, 2400);
servo.Start();
int angle = 0;
while (!cancellationTokenSource.Token.IsCancellationRequested)
{
    servo.WriteAngle(angle);
    angle = angle == 0 ? 180 : 0;
    Console.CursorTop = 4;
    Console.CursorLeft = 0;
    Console.WriteLine($"Angle: {angle}deg  ");
    cancellationTokenSource.Token.WaitHandle.WaitOne(3000, true);
}

servo.Stop();

goto demo;

