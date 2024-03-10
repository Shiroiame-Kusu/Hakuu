using System;
using Microsoft.Win32;

Console.Title = "SwitchToEdge for Hakuu";
Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", "Hakuu-Winform.exe", 6001, RegistryValueKind.DWord);
Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_ENABLE_CLIPCHILDREN_OPTIMIZATION", "Hakuu-Winform.exe", 1, RegistryValueKind.DWord);
Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", "Hakuu-WPF.exe", 6001, RegistryValueKind.DWord);
Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_ENABLE_CLIPCHILDREN_OPTIMIZATION", "Hakuu-WPF.exe", 1, RegistryValueKind.DWord);
Console.WriteLine("写入注册表成功。按任意键退出");
Console.ReadKey();