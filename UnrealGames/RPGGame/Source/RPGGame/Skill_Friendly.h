// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Skill.h"
#include "Skill_Friendly.generated.h"

/**
 * 
 */
UCLASS()
class RPGGAME_API USkill_Friendly : public USkill
{
	GENERATED_BODY()

public:

	virtual void SkillEffect(class ARPGCharacter* User, class ARPGCharacter* Target) override;
	
};
