// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Pawn.h"
#include "EnumContainer.h"
#include "ViewPawn.generated.h"




UCLASS()
class RPGGAME_API AViewPawn : public APawn
{
	GENERATED_BODY()

public:
	// Sets default values for this pawn's properties
	AViewPawn();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	bool DrawDebugLines;

	//SHould the camera lerp or snap when looking over the shoulder? (Placeholder, needs work)
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	bool LerpOnShoulder;

	//After finishing an action, what is the delay time till the enxt action begins?
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float NextActionDelayTime = 0.5f;

	//After zooming in on a character, what is the delay until they actually begin the action
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float CharacterActionDelayTime = 0.1f;

	//How long does the camera hang before beginning to move to its next point when supplied with a Delay command?
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float CamMovementDelayTime = 0.4f;




	//How far does the camera go when looking at the field for target selection?
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float FieldLookDistance = 500;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float CharacterFrontLookDistance = 10;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float CharacterFrontLookDeviation = 20;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float CharacterAboveLookDistance = 10;

	//How much does the camera pan out when looking between two characters?
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float CharacterBetweenLookFactor = 0.75f;

	//Keep these two variables above 0 and in some range, so that the camera doesn't end up directly overhead 
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	FVector OverheadDistMin;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	FVector OverheadDistMax;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	FVector CurveAdjustment;

	//Let X represent forward, Y be Right, and Z Be vertical
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	FVector ShoulderVector;


	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	FVector LowAngleVector;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	FVector OverheadBehindVector;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	FVector OverheadBehindTargetVector;

	//Slightly adjust rotation not always look directly at target from overheadposition
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float OverheadLookDeviation;


	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float ShoulderLookDeviation;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float Speed = 10;




	//When looking between two caharacters, slighlty adjust position to not always be directly between them
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float LookVariance = 0.0f;




	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	int XRandRange;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	int YRandRange;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	int ZRandRange;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float CurveRandomizerMin;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float CurveRandomizerMax;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	TEnumAsByte<ECamMovementType> StoredMovementType;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	bool StoredSnap = true;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	bool StoredZoom = false;

	//As of writing this this is only for moving to field overhead
	//Set false for when you're looking at the thing you're zooming towards, true when you're zooming out while maintaining eye contact 
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	bool ZoomOut = false;


	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	float StartTime;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	FVector StartPosition;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	FVector MidPoint1;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	FVector MidPoint2;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	FVector FinalPosition;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	FRotator StartRotation;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	FRotator FinalRotation;

	//Center point between character and target(s)
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	FVector CharacterMidpoint;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	FVector LookPos;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	bool IsMoving;



	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	float JourneyLength;



	UPROPERTY()
	class ARPGGameGameMode* GameMode;


	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	class UCameraComponent* Camera;


	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
	AActor* OverheadLookPosition;

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
	class ARPGCharacter* CurrentLookCharacter;

	UFUNCTION()
	void SelectPlayerUp();

	UFUNCTION()
	void SelectPlayerDown();


	UFUNCTION()
	void BackMenu();

	UFUNCTION()
	void Undo();

	UFUNCTION()
	void Execute();

	UFUNCTION()
	void ShiftToFinalPosition(float dt);

	UFUNCTION()
	void SnapToFinalPosition();


	UFUNCTION()
	void MoveToCharacterFront(class ARPGCharacter* Character);

	//Add slight deviation for dynamic effect or for more clear menuing
	UFUNCTION()
	void MoveToCharacterFrontSide(class ARPGCharacter* Character);


	UFUNCTION()
	void MoveToCharacterBehind(class ARPGCharacter* Character);

	UFUNCTION()
	void MoveToCharacterOverShoulder(class ARPGCharacter* Character);


	UFUNCTION()
	void MoveToCharacterLowAngle(class ARPGCharacter* Character);

	UFUNCTION()
	void MoveBetweenCharacters(class ARPGCharacter* Character);

	//Make an enum that decides the type of camera movement instead of making a bunch of similar functions
	UFUNCTION()
	void MoveToCharacter(class ARPGCharacter* Character, ECamMovementType Type = NoMovement, bool Snap = true, bool zoom = false);

	UFUNCTION()
	void MoveToCharacterDelay(class ARPGCharacter* Character, ECamMovementType Type, bool Snap = true, bool zoom = false);


	UFUNCTION()
	void MoveToFieldOverhead(bool Snap = true);



	UFUNCTION()
	void MoveToOverhead(class ARPGCharacter* Character);

	UFUNCTION()
	void MoveToOverheadBehind(class ARPGCharacter* Character);

	UFUNCTION()
	void MoveToOverheadBehindTarget(class ARPGCharacter* Character);

	UFUNCTION()
	void SpinAroundCharacter();

	UFUNCTION()
	void SetMoving();


	UFUNCTION()
	void EndCameraMovement();


	UFUNCTION()
	void CalculateMidPositions();


	UFUNCTION()
	void SwoopInFromOverhead(class ARPGCharacter* Character);



	//Calculate the midpoint between acting character and target(s)
	UFUNCTION()
	void CalculateCharacterMidpoint(class ARPGCharacter* Character);

	UFUNCTION()
	void CalculateCharacterMidpointMultiple(class ARPGCharacter* Character);


	UFUNCTION()
	void ActivateStoredMovement();


	UFUNCTION()
	FVector CubicBezierCurve(FVector P0, FVector P1, FVector P2, FVector P3, float alpha);

	void ReceiveCamInstruction(bool IsFriendly, bool IsMultiTarget);


	FTimerHandle ShiftDelayHandle;
};
