// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "RPGCharacter.h"
#include "RPGAlly.generated.h"





/**
 * 
 */
UCLASS()
class RPGGAME_API ARPGAlly : public ARPGCharacter
{
	GENERATED_BODY()
	
public:

	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

	virtual void BeginAction() override;

	virtual void Die() override;

	virtual void Revive(int Amount) override;

	virtual void LoseHealth(int Amount);

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
	int CurrentDrawRate;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	int BaseDrawRate;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	int DrawRateMin;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	int DrawRateMax;



	UFUNCTION()
	void SetDrawRate(int min, int max);

	UFUNCTION()
	void SetCurrentDrawRate(int val);

	UFUNCTION()
	void DeathEnd();

};
