// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"


UENUM(BlueprintType)
enum FAttribute {
	NoAttribute UMETA(DisplayName = "NoAttribute"),
	Slash UMETA(DisplayName = "Slash"),
	Stab UMETA(DisplayName = "Stab"),
	Bash UMETA(DisplayName = "Bash"),
	Fire UMETA(DisplayName = "Fire"),
	Ice UMETA(DisplayName = "Ice"),
	Electric UMETA(DisplayName = "Electric")



};

UENUM(BlueprintType)
enum FType {
	NoType UMETA(DisplayName = "NoType"),
	Physical UMETA(DisplayName = "Physical"),
	Elemental UMETA(DisplayName = "Elemental"),
	Status UMETA(DisplayName = "Status"),
	Buff UMETA(DisplayName = "Buff"),
	Health UMETA(DisplayName = "Health"),
	Revival UMETA(DisplayName = "Revival"),
	Special UMETA(DisplayName = "Special"),
	Item UMETA(DisplayName = "Item"),
	Defensive UMETA(DisplayName = "Defensive")

};

UENUM(BlueprintType)
enum FStatus {
	Normal	UMETA(DisplayName = "Normal"),
	Dead UMETA(DisplayName = "Dead"),
	Poisoned UMETA(DisplayName = "Poisoned"),
	Stun UMETA(DisplayName = "Stun"),
	Confuse UMETA(DisplayName = "Confuse"),
	Paralyze UMETA(DisplayName = "Paralyze"),
	Sleep UMETA(DisplayName = "Sleep"),
	Blind UMETA(DisplayName = "Blind")

};

UENUM(BlueprintType)
enum FActionType {
	NoAction UMETA(DisplayName = "NoAction"),
	NormalAttack UMETA(DisplayName = "NormalAttack"),
	UseSkill UMETA(DisplayName = "UseSkill"),
	UseItem UMETA(DisplayName = "UseItem"),
	Defend UMETA(DisplayName = "Defend")

};

UENUM(BlueprintType)
enum EBuffType {
	NoBuff UMETA(DisplayName = "NoBuff"),
	Attack UMETA(DisplayName = "Attack"),
	PhysicalAttack UMETA(DisplayName = "PhysicalAttack"),
	MagicAttack UMETA(DisplayName = "MagicalAttack"),
	Defense UMETA(DisplayName = "Defense"),
	PhysicalDefense UMETA(DisplayName = "PhysicalDefense"),
	MagicDefense UMETA(DisplayName = "MagicDefense"),
	Speed UMETA(DisplayName = "Speed"),
	RegenerationHP UMETA(DisplayName = "RegenerationHP"),
	RegenerationSP UMETA(DisplayName = "RegenerationSP"),
	Evasion UMETA(DisplayName = "Evasion"),
	DrawRate UMETA(DisplayName = "DrawRate"),
	Accuracy UMETA(DisplayName = "Accuracy"),
	Luck UMETA(DisplayName = "Luck")


};


UENUM(BlueprintType)
enum FResourceType {
	SkillPoints UMETA(DisplayName = "SkillPoints"),
	HealthPoints UMETA(DisplayName = "HealthPoints")


};

UENUM(BlueprintType)
enum ETargetType {
	Target_Enemy UMETA(DisplayName = "Target_Enemy"),
	Target_Ally UMETA(DisplayName = "Target_Ally"),
	Target_Self UMETA(DisplayName = "Target_Self")

};

UENUM(BlueprintType)
enum EAnimType {
	SearchByName UMETA(DisplayName = "SearchByName"),
	Emote_Item UMETA(DisplayName = "Item"),
	Emote_Defend UMETA(DisplayName = "Defend"),
	Emote_Shock UMETA(DisplayName = "Shock"),
	Emote_Idle UMETA(DisplayName = "Idle"),
	Emote_Special UMETA(DisplayName = "Special")
};

UENUM(BlueprintType)
enum EEquipType {
	Armor UMETA(DisplayName = "Armor"),
	Weapon UMETA(DisplayName = "Weapon"),
	Accessory UMETA(DisplayName = "Accessory"),

};


UENUM(BlueprintType)
enum ERestorativeType {
	Item_None UMETA(DisplayName = "None"),
	Item_Health UMETA(DisplayName = "Health"),
	Item_Mana UMETA(DisplayName = "Mana"),
	Item_Revival UMETA(DisplayName = "Revival")

};

UENUM(BlueprintType)
enum ECamMovementType {
	NoMovement UMETA(DisplayName = "NoMovement"),
	Front UMETA(DisplayName = "Front"), //Look at the character from the front straight on.
	FrontSide UMETA(DisplayName = "FrontSide"), //Look at the character from the front, with an offset to the side (Used when selecting actions
	Behind UMETA(DisplayName = "Behind"),
	OverheadFront UMETA(DisplayName = "OverheadFront"), //Look down at target from above
	OverheadBehind UMETA(DisplayName = "OverheadBehind"),
	OverheadBehindTarget UMETA(DisplayName = "OverheadBehindTarget"), // Same as OverheadBehind, but zoomout to behind the target enemy instead of behind user
	Between UMETA(DisplayName = "Between"), //Move to a point between to characters with an offset and look at the point between them
	Shoulder UMETA(DisplayName = "Shoulder"), //Look over the characters shoulder at target (Used primarily for single target actions)
	LowAngle UMETA(DisplayName = "LowAngle"), //Look beyond character from behind low angle at target (Used primarily for single target actions)
	Field UMETA(DisplayName = "Field"),
	SpecialCam UMETA(DisplayName = "SpecialCam") //For special attacks that doe multiple cam movements, dictated by anim events
 
};



/**
 * 
 */
class RPGGAME_API EnumContainer
{
public:

};
