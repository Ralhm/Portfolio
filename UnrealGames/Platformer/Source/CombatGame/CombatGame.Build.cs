// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;

public class CombatGame : ModuleRules
{
	public CombatGame(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

		PublicDependencyModuleNames.AddRange(new string[] { "Core", "CoreUObject", "Engine", "InputCore", "HeadMountedDisplay", "UMG", "NavigationSystem", "AIModule", "Niagara", "PerfCounters" });
	}
}
