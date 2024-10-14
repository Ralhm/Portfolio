// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Pawn.h"
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


	UPROPERTY()
	class ARPGGameGameMode* GameMode;

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

};
