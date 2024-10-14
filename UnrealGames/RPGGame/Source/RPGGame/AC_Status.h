// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "EnumContainer.h"
#include "AC_Status.generated.h"


UCLASS( ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class RPGGAME_API UAC_Status : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UAC_Status();

protected:

	FORCEINLINE class ARPGCharacter* GetCharacter();

public:	



	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	TEnumAsByte<FStatus> CurrentStatus;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	int PoisonDamage = 5;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	int BaseAilmentRecoveryChance = 10;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	int AilmentRecoveryChance;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	int AilmentRecoveryRate = 10;



	UFUNCTION()
	void InflictPoison(int Damage);

	UFUNCTION()
	void InflictConfusion();

	UFUNCTION()
	void InflictSleep();

	UFUNCTION()
	void InflictBlind();

	UFUNCTION()
	void AttemptInflictAilment(FStatus Ailment, int BaseChance);


	UFUNCTION()
	void InflictParalysis();

	UFUNCTION()
	void Poison();

	UFUNCTION()
	void Confusion();

	UFUNCTION()
	bool Paralyzed();

	UFUNCTION()
	bool Stun();

	UFUNCTION()
	void CureAilment();

	UFUNCTION()
	void AttemptRecoverAilment();

	UFUNCTION()
	void SetCurrentStatus(FStatus Status);

};
