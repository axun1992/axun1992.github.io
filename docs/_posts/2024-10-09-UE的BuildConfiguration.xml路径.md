---
title:  "UE的BuildConfiguration.xml路径"
tags: ue
---
UE的BuildConfiguration.xml用于额外的构建配置。那么，它到底放在哪些路径才会生效呢？  
<!--more-->
可参见官方的说明文档：  
[Build Configuration](https://dev.epicgames.com/documentation/en-us/unreal-engine/build-configuration-for-unreal-engine?application_version=5.1)  
里面提到了好几个路径，但实际上并不是全都有效。  

可以直接查看`UnrealBuildTool`的源文件来分析。  
以UE5.1为例，在引擎的`Windows/Engine/Source/Programs/UnrealBuildTool/System/XmlConfig.cs`  
```csharp
public static InputFile[] InputFiles
{
	get
	{
		if (CachedInputFiles != null)
		{
			return CachedInputFiles;
		}

		ILogger Logger = Log.Logger;
		
		// Find all the input file locations
		List<InputFile> InputFilesFound = new List<InputFile>(4);

		// Skip all the config files under the Engine folder if it's an installed build
		if (!Unreal.IsEngineInstalled())
		{
			// Check for the config file under /Engine/Programs/NotForLicensees/UnrealBuildTool
			FileReference NotForLicenseesConfigLocation = FileReference.Combine(Unreal.EngineDirectory, "Restricted", "NotForLicensees", "Programs", "UnrealBuildTool", "BuildConfiguration.xml");
			if (FileReference.Exists(NotForLicenseesConfigLocation))
			{
				InputFilesFound.Add(new InputFile(NotForLicenseesConfigLocation, "NotForLicensees"));
			}
			else
			{
				Logger.LogDebug("No config file at {NotForLicenseesConfigLocation}", NotForLicenseesConfigLocation);
			}

			// Check for the user config file under /Engine/Saved/UnrealBuildTool
			FileReference UserConfigLocation = FileReference.Combine(Unreal.EngineDirectory, "Saved", "UnrealBuildTool", "BuildConfiguration.xml");
			if (!FileReference.Exists(UserConfigLocation))
			{
				Logger.LogDebug("Creating default config file at {UserConfigLocation}", UserConfigLocation);
				CreateDefaultConfigFile(UserConfigLocation);
			}
			InputFilesFound.Add(new InputFile(UserConfigLocation, "User"));
		}

		// Check for the global config file under AppData/Unreal Engine/UnrealBuildTool
		string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		if (!String.IsNullOrEmpty(AppDataFolder))
		{
			FileReference AppDataConfigLocation = FileReference.Combine(new DirectoryReference(AppDataFolder), "Unreal Engine", "UnrealBuildTool", "BuildConfiguration.xml");
			if (!FileReference.Exists(AppDataConfigLocation))
			{
				Logger.LogDebug("Creating default config file at {AppDataConfigLocation}", AppDataConfigLocation);
				CreateDefaultConfigFile(AppDataConfigLocation);
			}
			InputFilesFound.Add(new InputFile(AppDataConfigLocation, "Global (AppData)"));
		}

		// Check for the global config file under My Documents/Unreal Engine/UnrealBuildTool
		string PersonalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		if (!String.IsNullOrEmpty(PersonalFolder))
		{
			FileReference PersonalConfigLocation = FileReference.Combine(new DirectoryReference(PersonalFolder), "Unreal Engine", "UnrealBuildTool", "BuildConfiguration.xml");
			if (FileReference.Exists(PersonalConfigLocation))
			{
				InputFilesFound.Add(new InputFile(PersonalConfigLocation, "Global (Documents)"));
			}
			else
			{
				Logger.LogDebug("No config file at {PersonalConfigLocation}", PersonalConfigLocation);
			}
		}

		CachedInputFiles = InputFilesFound.ToArray();

		Logger.LogDebug("Configuration will be read from:");
		foreach (InputFile InputFile in InputFiles)
		{
			Logger.LogDebug("  {File}", InputFile.Location.FullName);
		}

		return CachedInputFiles;
	}
}
```
可以得知，如果是安装的预构建版本，只有`AppData/Unreal Engine/UnrealBuildTool`和`My Documents/Unreal Engine/UnrealBuildTool`目录的会生效。  
如果是源码版本，则在引擎目录下还有这么几个路径先生效：  
`/Engine/Programs/NotForLicensees/UnrealBuildTool`  
`/Engine/Saved/UnrealBuildTool`  