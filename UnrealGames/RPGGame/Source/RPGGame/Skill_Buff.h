// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Skill.h"
#include "AC_Buff.h"
#include "Skill_Buff.generated.h"

/**
 * 
 */
UCLASS()
class RPGGAME_API USkill_Buff : public USkill
{
	GENERATED_BODY()
	
public:

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FBuffData BuffData;

	virtual void SkillEffect(class ARPGCharacter* User, class ARPGCharacter* Target) override;

};
