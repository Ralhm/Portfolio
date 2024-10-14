// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Skill.h"
#include "Skill_Random.generated.h"

/**
 * 
 */
UCLASS()
class RPGGAME_API USkill_Random : public USkill
{
	GENERATED_BODY()
	
public:

	virtual void OnActivate(class ARPGCharacter* User);

	virtual void UseSkillOnMultiple(class ARPGCharacter* User, TArray<class ARPGCharacter*> Targets);

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	int NumHitsBottom;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	int NumHitsTop;

};
