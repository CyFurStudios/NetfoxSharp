# Netfox Sharp

[![License](https://img.shields.io/badge/License-MIT-green)](https://github.com/CyFurStudios/NetfoxSharp/blob/main/LICENSE)
[![Documentation](https://img.shields.io/badge/Docs-github.io-blue)](https://foxssake.github.io/netfox/latest/)
[![Netfox](https://img.shields.io/badge/View-netfox-orange)](https://github.com/foxssake/netfox/)
[![Supported Version](https://img.shields.io/badge/Supports_Netfox-v1.14-blue)](https://github.com/foxssake/netfox/)
[![Godot_Version](https://img.shields.io/badge/Supports_Godot-4.X-blue)](https://godotengine.org/download/archive/)

A C# wrapper for the netfox addon in Godot.

## Features

* ⏲️  Consistent timing across multiple machines
* 🖥️ Supports client-server architecture
* 🧈 Smooth motion with easy-to-use interpolation
* 💨 Lag compensation with Client-side Prediction and Server-side Reconciliation

## Overview

This repo is an experimental release allowing the use of C# to more easily interact
with the [netfox](https://github.com/foxssake/netfox/) addon.

## Install

* Download the repo, and move the netfox_sharp and netfox_sharp_internals folders into the addons of a C#-enabled Godot project in the .NET version of Godot 4.x.
* Install the netfox addon, which can be found at the link above.
* Build your project, then enable netfox and Netfox Sharp in your project settings.
* Restart Godot, and you're done!

## Supported versions

Godot 4.x is supported by Netfox Sharp. If you find any issue using any supported
version, please [open an issue](https://github.com/CyFurStudios/NetfoxSharp/issues).

## License

Netfox Sharp is under the [MIT license](LICENSE). Note that the netfox dependency
may differ, see its [license](https://github.com/foxssake/netfox/blob/main/LICENSE).
