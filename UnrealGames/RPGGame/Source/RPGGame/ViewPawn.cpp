// Fill out your copyright notice in the Description page of Project Settings.


#include "ViewPawn.h"

#include "Kismet/GameplayStatics.h"

#include "RPGGameGameMode.h"



// Sets default values
AViewPawn::AViewPawn()
{
 	// Set this pawn to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

}

// Called when the game starts or when spawned
void AViewPawn::BeginPlay()
{
	Super::BeginPlay();

	GameMode = Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()));
	
}

// Called every frame
void AViewPawn::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

// Called to bind functionality to input
void AViewPawn::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

	PlayerInputComponent->BindAction("Up", IE_Pressed, this, &AViewPawn::SelectPlayerUp);
	PlayerInputComponent->BindAction("Down", IE_Pressed, this, &AViewPawn::SelectPlayerDown);
	PlayerInputComponent->BindAction("Accept", IE_Pressed, this, &AViewPawn::Execute);
	PlayerInputComponent->BindAction("Undo", IE_Pressed, this, &AViewPawn::Undo);
}

void AViewPawn::SelectPlayerUp() {
	if (GameMode->State == Enemy_Select) {
		GameMode->AdjustEnemyIndex(1);
	}
	else if (GameMode->State == Player_Select) {
		GameMode->AdjustPlayerSelectIndex(1);
	}
}


void AViewPawn::SelectPlayerDown() {
	if (GameMode->State == Enemy_Select) {
		GameMode->AdjustEnemyIndex(-1);
	}
	else if (GameMode->State == Player_Select) {
		GameMode->AdjustPlayerSelectIndex(-1);
	}
}


void AViewPawn::BackMenu() {
	


}

void AViewPawn::Undo() {
	GameMode->UndoMenu();
}

void AViewPawn::Execute() {
	GameMode->ConfirmAction();

}