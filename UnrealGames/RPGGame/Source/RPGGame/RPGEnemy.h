// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "RPGCharacter.h"
#include "RPGEnemy.generated.h"

/**
 * 
 */
UCLASS()
class RPGGAME_API ARPGEnemy : public ARPGCharacter
{
	GENERATED_BODY()

public:

	virtual void Die() override;

	UFUNCTION()
	void ChooseAction();



	virtual void BeginAction() override;

	UFUNCTION()
	void DeathEnd();
	
};
