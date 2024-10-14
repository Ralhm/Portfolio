// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Skill.h"
#include "Skill_Ailment.generated.h"

/**
 * 
 */
UCLASS()
class RPGGAME_API USkill_Ailment : public USkill
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	TEnumAsByte<FStatus> Type;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	bool DoesDamage;


	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float InflictionChance;

	virtual void SkillEffect(class ARPGCharacter* User, class ARPGCharacter* Target) override;
	
};
