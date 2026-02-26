// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"

#include "AC_ParkourMantle.generated.h"



UCLASS(Blueprintable, meta = (BlueprintSpawnableComponent))
class PARKOURROR_API UAC_ParkourMantle : public UActorComponent
{
	GENERATED_BODY()

public:
	// Sets default values for this component's properties
	UAC_ParkourMantle();



protected:
	// Called when the game starts
	virtual void BeginPlay() override;

	UFUNCTION()
		UCharacterMovementComponent* GetMovementComp() const;

	UFUNCTION()
		class AParkourrorCharacter* GetCharacter() const;

private:
	float timeSpentWallClambering = 0.0f;

public:
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

	UPROPERTY(EditAnywhere)
		USoundWave* jumpSound;

	UPROPERTY(EditAnywhere)
		UAnimMontage* ClimbMontage;

	UPROPERTY(EditAnywhere)
		UAnimMontage* VaultMontage;


	FTimerHandle VaultHandle;
	FTimerHandle BoostHandle;
	FTimerHandle ClamberHandle;
	FTimerHandle ClimbHandle;
	FTimerHandle ClimbBoostHandle;

	UFUNCTION()
		void AttemptMantle();

	UFUNCTION()
		void ForwardTrace();

	UFUNCTION()
		void HeightTrace();

	UFUNCTION()
		void FootTrace();

	UFUNCTION()
		void VaultFootTrace();

	UFUNCTION()
		void SetVaultJump();

	UFUNCTION()
		void VaultBoostTimer();

	UFUNCTION()
		void SetMantleStepHeight();

	UFUNCTION()
		void ResetMantleStepHeight();

	UFUNCTION()
		void ClamberUpWall(float deltaTime);

	UFUNCTION()
		void EnterClamber();

	UFUNCTION()
		void StopClambering();

	UFUNCTION()
		void SetCanClamber();

	UFUNCTION()
		bool GetIsClambering();

	UFUNCTION()
		void ExitClimb();


	UFUNCTION()
		void ExitVault();

	UFUNCTION()
		void AttemptBoost();

	UFUNCTION()
		void EndBoostTimer();

	UFUNCTION(BlueprintCallable)
		void BoostLeap(float Power);

	UFUNCTION(BlueprintCallable)
		void Boost(float Power);

	UFUNCTION(BlueprintCallable)
		void BoostLeapClimb();

	UFUNCTION(BlueprintCallable)
		void BoostLeapBuffer();

	UFUNCTION()
		void SlideDownWall();

	UFUNCTION()
		void StopSlidingDownWall();

	UFUNCTION(BlueprintCallable)
		void VaultOverLedge();



	float Timer = 0;

	bool BoostBuffering;

	bool HoldingJump;

	bool SetIsVaulting;

	bool SetIsClimbing;

	bool CanBoost;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	bool isVaulting;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	bool isClimbing;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	bool isClambering;

	bool CanClamber = true;

	bool CanClimb;

	bool isBoosting;

	bool IsAttemptingToVault;

	bool IsWallSliding;


	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
		FVector MantleNormal;

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
		FVector MantleLocation;

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
		FVector MantleHeight;

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
		FVector FootHitLocation;



	UPROPERTY(EditAnywhere, BlueprintReadOnly)
		TArray< AActor* > ActorsToIgnore;

	//How far down does the height trace stretch?
	UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 0.5, UIMax = 215.0))
		float MantleHeightLength = 215.0f;

	//How far down does the Foot trace begin?
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
		float MantleFootPos = 40;

	//How far down does the Forward trace begin?
	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
		float MantleForwardPos = 80.0f;

	//Progress into climb required to properly boost off of ledge. Will buffer players input if they try to boost prematurely.
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
		float MontageBoostLimit = 0.4f;

	//Increased play rate if the player attempts to boost while still climbing
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
		float MontageBoostPlayRate = 2.0f;


	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float MantleForwardTraceDistance = 70.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float MantleHeightTraceDistance = 45.0f;

	//How much time does a player have to Boost after pressing the button?
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float BoosTime = 0.25f;

	//How far do you have to be to boost up the ledge? Higher value = boost from lower on ledge
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float MantleBoostLimit = 103.0f;

	//How close do you have to be to the ledge in order to climb it? (Distance between the ledge and you) Higher value = lower distance (Must be closer to ledge in order to climb). 
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float MantleLimitHigh = 65.0f;

	//Threshold between doing a full on climb and doing a little climb
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float MantleLimitLow = 55.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float MantleStepHeight = 65.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float WallClimbLimit = 70.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float ForwardTraceRadius = 2.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float HeightTraceRadius = 20.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float ClamberForce = 1000.0f;

	//Duration of clambering up a wall
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float ClamberTime = 0.2f;

	//Prevent the player from running up if they're falling too fast
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float MinClamberVelocity = 0.0f;

	//Minimum distance from wall required to clamber up it
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values", meta = (UIMin = 0.0, UIMax = 130.0))
		float MinClamberDistance = 125.0f;

	//Prevent the player from Boosting up a ledge if they're too slow
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float MinBoostVelocity = 500.0f;


	//Affects how far you go when boosting up a ledge (IF PRESSED BEFORE A CLIMB)
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float BoostPowerLaunch = 1500;

	//Affects how far you go when boosting up a ledge (IF VAULTING)
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float BoostPowerVault = 1500;

	//Affects how far you go when boosting up a ledge (WHILE CLIMBING)
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float BoostPowerClimb = 1500;

	//Adjusts trajectory of ledge boosting, higher value = taller arc
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values", meta = (UIMin = 0.0, UIMax = 1.0))
		float BoostAngle = 0.2f;

	//Boosts vertical velocity when boosting up ledge
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float VerticalJumpBoost;

	//Limitation on the angle at which you need to approach the wall to clamber up. Higher value = straighter approach to wall
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values", meta = (UIMin = 0.5, UIMax = 1.0))
		float DesiredWallClimbAngle = 0.85f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Designer Values")
		float SlideGravity = 1.0f;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Designer Values")
		TArray <TEnumAsByte <EObjectTypeQuery> > ObjectTypes;
		

	FHitResult ForwardHit;
	FHitResult FootHit;

	TScriptDelegate <FWeakObjectPtr> ClimbFinished;

};
