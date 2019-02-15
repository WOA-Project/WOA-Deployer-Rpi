# WoA Deployer for Raspberry Pi 3

The application to install Full Windows 10 into your Raspberry Pi!

![image](https://user-images.githubusercontent.com/3109851/43066047-e7134552-8e63-11e8-8ac7-895e601b60e3.png)

# **Super easy to use. No-hassle.**

Please keep reading carefully. All you need is here.

# Requirements for WOA
- Raspberry Pi 3 Model B (or B+)
- MicroSD card. Recommended with A1 rating.
- A Windows 10 ARM64 Image (.wim). Please, check [this link](Docs/GettingWOA.md) to get it.

## Requirements for running this application
- A recent version of Windows 10 (please, use the latest to ensure it'll run correctly, don't open issues otherwise)
- .NET Framework 4.6.1 (should come included in recent versions of Windows 10)

# About Core Packages
Please, notice the WoA Installer is only a tool with helps you with the deployment. WoA Installer needs a set of binaries, AKA the **Core Package**, to do its job. **These binaries are not not mine** and are bundled and offered just for convenience to make your life easier, since this tool is focused on simplicity. 

Find them below.

# Downloads

## 1. WOA Deployer

Download the **[latest version](https://github.com/SuperJMN/WoA-Installer/releases/download/v1.2/WoA.Installer.for.Raspberry.Pi.zip)** 

# Donations are welcome!

If you find this useful, feel free to [buy me a coffee ☕](http://paypal.me/superjmn). Thanks in advance!!

## Donate to the contributors of this project
Please, don't forget that the RaspberryPi WOA Project is supported by other individuals and companies (see the [credits and acknowledgements section](#credits-and-acknowledgements
)).
 - Donate to MCCI. Why? [Read this 🗒](Docs/mcci_donate.md) 

# Need help?
Then visit our projects website at https://pi64.win, the one-stop solution for all your questions 😊

# Credits and Acknowledgements

This WoA Installer is possible because the great community behind it. I would like to thank the brilliant minds behind this technical wonder. If you think you should be listed, please, contact me using the e-mail address on my profile.

- [Andrei Warkentin](https://github.com/andreiw) for the **64-bit Pi UEFI**, UEFI Pi (HDMI, USB, SD/MMC) drivers, improved ATF and Windows boot/runtime support.
- [MCCI](https://mcci.com/) for their great contribution to the RaspberryPI WOA project:
  - for porting their **TrueTask USB stack** to Windows 10 ARM64, and allowing non-commercial use with this project ([see license](Docs/mcci_license.md))
  - for funding the site of the project http://pi64.win and the discourse site http://discourse.pi64.win
  - Special thanks to Terry Moore for all the great support and commitment, and for setting up the online presence for the project and its insfrastructure.
- Ard Bisheuvel for initial ATF and UEFI ports
- [Googulator](https://github.com/Googulator) for his method to install WoA in the Raspberry Pi
- Mario Bălănică for his [awesome tool](https://www.worproject.ml), and for tips and support :)
	- daveb77
    - thchi12
    - falkor2k15
    - driver1998
    - XperfectTR
    - woachk
    - novaspirit
    - zlockard 
     
    ...for everything from ACPI/driver work to installation procedures, testing and so on.
- Microsoft for the 32-bit IoT firmware.

In addition to:

- [Eric Zimmerman](https://github.com/EricZimmerman) for [The Registry Project](https://github.com/EricZimmerman/Registry)
- [Jan Karger](https://github.com/punker76) [MahApps.Metro](https://mahapps.com)
- [ReactiveUI](https://reactiveui.net)
- [Adam Hathcock](https://github.com/adamhathcock) for [SharpCompress](https://github.com/adamhathcock/sharpcompress)

And our wonderful group at Telegram for their testing and support!
- [RaspberryPiWOA](https://t.me/raspberrypiwoa)

## Related projects
These are the related projects. The Core Packages comes from them. Big thanks!

- [RaspberryPiPkg](https://github.com/andreiw/RaspberryPiPkg)
- [Microsoft IoT-BSP](https://github.com/ms-iot/bsp)
- [Raspberry Pi ATF](https://github.com/andreiw/raspberry-pi3-atf)
- [WOR Project](https://www.worproject.ml) by [Mario Bălănică](https://github.com/mariobalanica)
