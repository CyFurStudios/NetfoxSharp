# NetfoxSharp

[![License](https://img.shields.io/badge/License-MIT-green)](https://github.com/CyFurStudios/NetfoxSharp/blob/main/LICENSE)
[![Release](https://img.shields.io/badge/Release-v0.6.6-blue)](https://github.com/CyFurStudios/NetfoxSharp/releases/tag/v0.6.6)
[![Documentation](https://img.shields.io/badge/Docs-github.io-blue)](https://foxssake.github.io/netfox/latest/)
[![Netfox](https://img.shields.io/badge/View-netfox-orange)](https://github.com/foxssake/netfox/)
[![Supported Version](https://img.shields.io/badge/Supports_netfox-v1.25-blue)](https://github.com/foxssake/netfox/)
[![Godot_Version](https://img.shields.io/badge/Supports_Godot-4.1+-blue)](https://godotengine.org/download/archive/)

A C# wrapper for the netfox addon in Godot.

## Features

* ⏲️  Consistent timing across multiple machines
* 🖥️ Supports client-server architecture
* 🧈 Smooth motion with easy-to-use interpolation
* 💨 Lag compensation with Client-side Prediction and Server-side Reconciliation

## Overview

This repo is an **experimental** release allowing the use of C# to more easily interact
with the [netfox](https://github.com/foxssake/netfox/) addon.

## Install

* Download the repo, and move the netfox_sharp and netfox_sharp_internals folders into the addons of a C#-enabled Godot project in the .NET version of Godot 4.1+.
* Install the netfox addon, which can be found at the link above.
* Alternatively, download a [release](https://github.com/CyFurStudios/NetfoxSharp/releases/) which includes both the relevant netfox and NetfoxSharp folders.
* Build your project, then enable netfox and NetfoxSharp in your project settings.
* Restart Godot, and you're done!

## Supported versions

Godot 4.1+ is supported by NetfoxSharp. If you find any issue using any supported
version, please [open an issue](https://github.com/CyFurStudios/NetfoxSharp/issues).

## License

NetfoxSharp is under the [MIT license](LICENSE). Note that the netfox dependency
may differ, see its [license](https://github.com/foxssake/netfox/blob/main/LICENSE).
