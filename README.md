[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_nanoFramework.MagicBit&metric=alert_status)](https://sonarcloud.io/dashboard?id=nanoframework_nanoFramework.MagicBit) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_nanoFramework.MagicBit&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=nanoframework_nanoFramework.MagicBit) [![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE) [![NuGet](https://img.shields.io/nuget/dt/nanoFramework.MagicBit.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.MagicBit/) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/nanoframework/Home/blob/main/CONTRIBUTING.md) [![Discord](https://img.shields.io/discord/478725473862549535.svg?logo=discord&logoColor=white&label=Discord&color=7289DA)](https://discord.gg/gCyBu8T)

![nanoFramework logo](https://raw.githubusercontent.com/nanoframework/Home/main/resources/logo/nanoFramework-repo-logo.png)

-----

# Welcome to the .NET **nanoFramework** MagicBit Library repository

## Build status

| Component | Build Status | NuGet Package |
|:---|---|---|
| MagicBit | [![Build Status](https://dev.azure.com/nanoframework/nanoFramework.MagicBit/_apis/build/status/nanoFramework.MagicBit?repoName=nanoframework%2FnanoFramework.MagicBit&branchName=main)](https://dev.azure.com/nanoframework/nanoFramework.MagicBit/_build/latest?definitionId=84&repoName=nanoframework%2FnanoFramework.MagicBit&branchName=main) | [![NuGet](https://img.shields.io/nuget/v/nanoFramework.MagicBit.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.MagicBit/) |

## Usage

This nuget can be used with the great [MagicBit](https://magicbit.cc/) board.

It does bring support for almost all the sensors and the robot elements. Still, some are not natively in this nuget as they are part of the existing [IoT.Device bindings](https://github.com/nanoframework/nanoFramework.IoT.Device). See the [known limitations](#known-limitations) as well.

You just need to make sure your MagicBit is flashed like this:

```shell
# Replace the com port number by your COM port
nanoff --platform esp32 --update --preview --serialport COM3
```

A detailed example is available in the [Test application](Tests/MagicBitTestApp/Program.cs) as well.

### Screen

The only thing you need to do to access the screen is to initialize it:

```csharp
MagicBit.InitializeScreen();
```

Once you've initialized it, you can access both a `Screen` static class and a `Console` static class.

The `Screen` one brings primitives to write directly on the screen points, select colors as well as writing a text.

For example, you can write a small buffer square of 8x8 at the position 0, 26 with a width of 8 like this:

```csharp
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
Screen.DrawBitmap(0, 26, 8, _heart);
```

Note that only multiple of 8 are possible for the width, the buffer should be a multiple of the width / 8. Each bit is representing a pixel. This is the way you can display images. 

The screen provides as well other primitives, here is a quick example:

```csharp
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
```

And you will get even more thru `Screen.Device`.

The Console class works in a similar way as the classic `System.Console`:

```csharp
Console.Clear();
Console.WriteLine("Motors");
Console.CursorTop = 2;
Console.WriteLine("Motors will run reverse then rotate both direction");
```

> Note: You can change the default font as well, you need to provide it as a property. The Cursor positions are calculated with the width of the font.

### Buttons

The 2 buttons are exposed.

They are called `ButtonLeft` and `ButtonRight`. You can get access to the events as well. For example:

```csharp
MagicBit.ButtonLeft.Press += (sender, e) =>
{
    Console.CursorLeft = 0;
    Console.CursorTop = 0;
    Console.Write($"Left Pressed  ");
};

// Simple way of getting the button status
while (!MagicBit.ButtonRight.IsPressed)
{
    Thread.Sleep(20);
}
```

Another sample with events:


```csharp
MagicBit.ButtonRight.IsHoldingEnabled = true;
MagicBit.ButtonRight.Holding += (sender, e) =>
{
    Console.Write("ButtonRight hold long.");
};
```

### Motors

The MagicBit has a driver allowing to control 2 DC motors. If you have the robot kit, you'll be able to control them. If you don't have the robot kit, you still can use your own if you plug them on the correct pins.

```csharp
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
```

### Buzzer

It's of course possible to use the buzzer. Here is an example playing tones:

```csharp
var buzz = MagicBit.Buzzer;
for (int i = 0; i < 10; i++)
{
    buzz.PlayTone(500 + i * 25, 1000);
}
```

### Potentiometer and Luminosity

Those 2 embedded sensors can be directly accessed and used. Here is a complete example reading them, displaying the value on the screen and interrupting them when the left button is pressed:

```csharp
CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
MagicBit.ButtonLeft.Press += (sender, e) =>
{
    cancellationTokenSource.Cancel();
};

Console.Clear();
Console.WriteLine("Sensors");
Console.CursorTop = 2;
Console.WriteLine("Clk left button to stop");

while (!cancellationTokenSource.Token.IsCancellationRequested)
{
    // Read the potentiometer ratio, from 0.0 to 1.0
    var ratio = MagicBit.Potentiometer.ReadRatio();
    // Read the luminosity sensor ratio from 0.0 (full dark) to 1.0 (full light)
    var lumi = MagicBit.Luminosity.ReadRatio();
    Console.CursorTop = 4;
    Console.CursorLeft = 0;
    Console.WriteLine($"Pot: {ratio * 100:N2}%  ");
    Console.CursorTop = 5;
    Console.CursorLeft = 0;
    Console.WriteLine($"lum: {lumi * 100:N2}%  ");
    cancellationTokenSource.Token.WaitHandle.WaitOne(200, true);
}
```

### Servo motor

Servo motor can be attached to. So far, you have a direct and easy way on the blue pin. This full sample shows how to change the angle from 0 to 180 degrees, displays the next angle and wait for the left button to be pressed to stop:

```csharp
CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
MagicBit.ButtonLeft.Press += (sender, e) =>
{
    cancellationTokenSource.Cancel();
};

Console.Clear();
Console.WriteLine("Servo");
Console.CursorTop = 2;
Console.WriteLine("Clk left button to stop");

// The servo can be different and you can adjust the values as needed
var servo = MagicBit.GetServoPinBlue(180, 500, 2400);
// This is needed to start the servo
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

// Don't forget to stop it at the end.
servo.Stop();
```

> Note: it is technically possible to use any available pin to plug a servo motor. The board package will make your life easy if you are using the blue pin. Otherwise, you'll have to set up the Servo motor yourself.

### Leds

The 4 embedded leds are available.

```csharp
MagicBit.LedBlue.Write(PinValue.High);
```

> Important: You **cannot** use them with the motors as they are using the same pins.

### I2C Device

By default you can get an I2C Device thorough the red pins. You can either use `GetRed` or `GetI2cDevice`. For example if you wan to create an I2C Device with the address 0x42, just do:

```csharp
var myI2cDevice = MagicBit.GetI2cDevice(0x42);
```

### Black left and right pins

You can get a GPIO Pin, so a pin you can use a single output or input from the Black pins directly. Both `GetPinBlackLeft` and `GetPinBlackRight` will give it to you:

```csharp
// This will create an output mode pint, you can for example attach a led
var myPin = MagicBit.GetPinBlackLeft(PinMode.Output);
// This will change the pin to high
myPin.Write(PinValue.High);
```

### Blue pin

The blue pin is setup by default to be used as PWM. When getting it, you'll get a PWM Channel:

```csharp
var myPwm = MagicBit.GetPinBlue();
```

### Changing default pin behavior

This is a more advance scenario but you can change the function of any pin if you did not use the default function before. For example, you can from the black left pin create a PWM chanel as well:

```csharp
Configuration.SetPinFunction(32, DeviceFunction.PWM11);
var myBlackPwm = PwmChannel.CreateFromPin(32);
```

Even if the blue pin default behavior is PWM, if you do not ask for it, you can use it in a different way. For example as a simple input:

```csharp
var myBluePin = MagicBit.GpioController.Open(26, PinMode.Input);
```

## Known limitations

There are few sensors that will not work, see the list below, the reasons and possible mitigation:

- DHT sensor is not supported yet on ESP32 .NET managed code. You can use one of the other supported temperature and humidity sensor. [See here](https://github.com/nanoframework/nanoFramework.IoT.Device/tree/develop/devices#thermometers).
- The HCSR04 which is on the robot is not yet supported as it's using the same pin for emission and reception of the signal. This is work in progress to find a solution. The one sold separately, when used with the red port will perfectly work.
- The QRT Sensors are not yet supported as a group, this is work in progress. In the mean time, you can read them as analog sensors individually.

## Feedback and documentation

For documentation, providing feedback, issues and finding out how to contribute please refer to the [Home repo](https://github.com/nanoframework/Home).

Join our Discord community [here](https://discord.gg/gCyBu8T).

## Credits

The list of contributors to this project can be found at [CONTRIBUTORS](https://github.com/nanoframework/Home/blob/main/CONTRIBUTORS.md).

## License

The **nanoFramework** Class Libraries are licensed under the [MIT license](LICENSE.md).

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behaviour in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

### .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).
