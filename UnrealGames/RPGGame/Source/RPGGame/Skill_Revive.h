// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Skill_Friendly.h"
#include "Skill_Revive.generated.h"

/**
 * 
 */
UCLASS()
class RPGGAME_API USkill_Revive : public USkill_Friendly
{
	GENERATED_BODY()
	

	virtual void SkillEffect(class ARPGCharacter* User, class ARPGCharacter* Target) override;

	
};
