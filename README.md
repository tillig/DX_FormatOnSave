# DX_FormatOnSave

Visual Studio allows you to specify different code formatting rules for different languages it understands, but in many cases the formatting only applies to new code you're writing. For example, C# lines generally get formatted when you hit the end of the line and type the semicolon... but the only line that gets formatted is the one you just completed.

Wouldn't it be nice if the whole document would adhere to the same formatting without you having to pay attention to it?

With this plugin, you can automatically have Visual Studio apply code formatting to the entire document you're working on when you save. That way you don't have to think about it - your documents will always be formatted.

## Requirements
This plugin requires **DXCore for Visual Studio .NET 11.2 or later.**

##Installation
In Visual Studio, go to `Tools -> Extension Manager` and search for `DX_FormatOnSave`. Click the handy button to install.

##Configuration
While usage is transparent - documents automatically get formatted when you save them - you can configure which documents the formatting will get applied to.

From the DevExpress menu, select `Options`. In the tree on the left, go to `Editor/Code Style` and select the `Format On Save` options window. There you will be able to select which document types get formatted.
