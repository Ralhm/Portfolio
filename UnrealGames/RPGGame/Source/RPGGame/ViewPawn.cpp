// Fill out your copyright notice in the Description page of Project Settings.


#include "ViewPawn.h"

#include "Camera/CameraComponent.h"

#include "DrawDebugHelpers.h"

#include "Item_ConsumableAsset.h"

#include "Kismet/GameplayStatics.h"


#include "RPGCharacter.h"
#include "RPGGameGameMode.h"



// Sets default values
AViewPawn::AViewPawn()
{
 	// Set this pawn to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	Camera = CreateDefaultSubobject<UCameraComponent>(TEXT("ActualCamera"));
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

	if (IsMoving) {
		ShiftToFinalPosition(DeltaTime);
	}

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
	if (IsMoving) {
		EndCameraMovement();
		SnapToFinalPosition();

	}


	if (GameMode->State == Enemy_Select || GameMode->State == Enemy_Select_Multi || GameMode->State == Player_Select || GameMode->State == Player_Select_Multi) {
		GameMode->ConfirmAction();
	}
	

}


void AViewPawn::MoveToCharacterFront(ARPGCharacter* Character) {
	FinalPosition = Character->GetActorLocation() + (Character->GetActorForwardVector() * CharacterFrontLookDistance);

	//The first value is where you want to look
	FinalRotation = (Character->GetActorLocation() - FinalPosition).Rotation();


}

void AViewPawn::MoveToCharacterFrontSide(ARPGCharacter* Character) {
	MoveToCharacterFront(Character);
	
	FinalPosition += Character->GetActorRightVector() * CharacterFrontLookDeviation;

}

void AViewPawn::MoveToCharacterBehind(ARPGCharacter* Character) {
	FinalPosition = Character->GetActorLocation() - (Character->GetActorForwardVector() * CharacterFrontLookDistance);
	FinalRotation = (Character->GetActorLocation() - FinalPosition).Rotation();

}

void AViewPawn::MoveBetweenCharacters(ARPGCharacter* Character) {
	CalculateCharacterMidpoint(Character);

	FVector LineVector = Character->GetActorLocation() - Character->StoredTarget->GetActorLocation();



	FinalPosition = CharacterMidpoint +
		(FVector::CrossProduct(LineVector.GetSafeNormal(), Character->GetActorUpVector()) * LineVector.Size() * CharacterBetweenLookFactor) +
		(GetActorUpVector() * CharacterAboveLookDistance) +
		(GetActorForwardVector() * FMath::RandRange(-LookVariance, LookVariance));

	FinalRotation = (CharacterMidpoint - FinalPosition).Rotation();


}


//Essentially just MoveToCharacter, but there's a minor delay before doing so
void AViewPawn::MoveToCharacterDelay(ARPGCharacter* Character, ECamMovementType Type, bool Snap, bool Zoom) {
	CurrentLookCharacter = Character;
	StoredMovementType = Type;
	StoredSnap = Snap;
	StoredZoom = Zoom;
	GetWorldTimerManager().SetTimer(ShiftDelayHandle, this, &AViewPawn::ActivateStoredMovement, CamMovementDelayTime);
}


// --During the turn phase, cut to overhead of acting character, lerp to character front/back, activate their action, then do a certain dynamic camera move like lerping out or switching angles
void AViewPawn::MoveToCharacter(ARPGCharacter* Character, ECamMovementType Type, bool Snap, bool Zoom) {
	ZoomOut = Zoom;
	CurrentLookCharacter = Character;



	switch (Type) {
	case Front:
		MoveToCharacterFront(Character);
		break;
	case FrontSide:
		MoveToCharacterFrontSide(Character);
		break;
	case Behind:
		MoveToCharacterBehind(Character);
		break;
	case OverheadFront:
		MoveToOverhead(Character);
		break;
	case OverheadBehind:
		MoveToOverheadBehind(Character);
		break;
	case OverheadBehindTarget:
		MoveToOverheadBehindTarget(Character);
		break;
	case Between:
		MoveBetweenCharacters(Character);
		break;
	case Shoulder:
		MoveToCharacterOverShoulder(Character);
		break;
	case LowAngle:
		MoveToCharacterLowAngle(Character);
		break;
	case Field:
		MoveToFieldOverhead(Snap);
		break;
	}


	if (Snap) {
		SnapToFinalPosition();
		return;
	}
	else {
		




		SetMoving();
	}



}


void AViewPawn::MoveToFieldOverhead(bool Snap) {

	AActor* FieldActor = GameMode->GetRandomOverheadPoint();

	FinalPosition = FieldActor->GetActorLocation();
	//FinalRotation = (CenterLookPoint - FinalPosition).Rotation();

	FinalRotation = FieldActor->GetActorRotation();

	if (Snap) {
		SnapToFinalPosition();
		return;
	}
	else {
		ZoomOut = true;
		SetMoving();
	}


}

//Look over characters shoulder at midpoint between them and their target
void AViewPawn::MoveToCharacterOverShoulder(ARPGCharacter* Character) {
	FinalPosition = Character->GetActorLocation() +
		(Character->GetActorForwardVector() * ShoulderVector.X) +
		(Character->GetActorRightVector() * ShoulderVector.Y) +
		(Character->GetActorUpVector() * ShoulderVector.Z);




	CalculateCharacterMidpoint(Character);



	FinalRotation = (CharacterMidpoint - FinalPosition).Rotation();
}

//Go above and directly behind a character, used primarily for multi-hit skills (optional, I might just use field views for that instead, but more design options is never a bad thing)
void AViewPawn::MoveToOverheadBehind(ARPGCharacter* Character) {
	FinalPosition = Character->GetActorLocation() +
		(Character->GetActorForwardVector() * OverheadBehindVector.X) +
		(Character->GetActorUpVector() * OverheadBehindVector.Z);




	CalculateCharacterMidpoint(Character);



	FinalRotation = (CharacterMidpoint - FinalPosition).Rotation();
}

void AViewPawn::MoveToOverheadBehindTarget(ARPGCharacter* Character) {
	//Don't go directly behind the target, go in the direction of the vectro from user to target
	FVector BehindDir = (Character->StoredTarget->GetActorLocation() - Character->GetActorLocation());

	FinalPosition = (BehindDir + (BehindDir.GetSafeNormal() * OverheadBehindTargetVector.X)) +
		(Character->StoredTarget->GetActorUpVector() * OverheadBehindTargetVector.Z);

	//ALT METHOD
	//FVector BehindDir = (Character->StoredTarget->GetActorLocation() - Character->GetActorLocation()).GetSafeNormal;
	//FinalPosition = Character->StoredTarget->GetActorLocation() + 
	//(BehindDir * OverheadBehindVector.X) +
	//(Character->StoredTarget->GetActorUpVector() * OverheadBehindVector.Z);

	CalculateCharacterMidpoint(Character);


	FinalRotation = (CharacterMidpoint - FinalPosition).Rotation();
}


void AViewPawn::MoveToCharacterLowAngle(ARPGCharacter* Character) {
	FinalPosition = Character->GetActorLocation() +
		(Character->GetActorForwardVector() * LowAngleVector.X) +
		(Character->GetActorRightVector() * LowAngleVector.Y) +
		(Character->GetActorUpVector() * LowAngleVector.Z);

	CalculateCharacterMidpoint(Character);

	FinalRotation = (CharacterMidpoint - FinalPosition).Rotation();
}



void AViewPawn::MoveToOverhead(ARPGCharacter* Character) {
	//Randomize the circular quadrant the camera ends up in around the character
	int YFactor = 1;
	int XFactor = 1;

	if (FMath::RandRange(0, 1) == 1) {
		YFactor = -1;
	}
	if (FMath::RandRange(0, 1) == 1) {
		XFactor = -1;
	}

	FVector LookPosition = FVector((FMath::RandRange(OverheadDistMin.X, OverheadDistMax.X)) * XFactor, 
		(FMath::RandRange(OverheadDistMin.Y, OverheadDistMax.Y)) * YFactor, 
		(FMath::RandRange(OverheadDistMin.Z, OverheadDistMax.Z)));


	FinalPosition = Character->GetActorLocation() + LookPosition;



	//Slightly adjust rotation not always look directly at target
	LookPos = Character->GetActorLocation() +
		FVector(FMath::RandRange(-OverheadLookDeviation, OverheadLookDeviation), FMath::RandRange(-OverheadLookDeviation, OverheadLookDeviation), 0);
	


	FinalRotation = (LookPos - FinalPosition).Rotation();

	//DrawDebugLine(GetWorld(), GetActorLocation(), LookDeviation, FColor::Green, false, 10.0f, 0, 10.0f);

	


}



void AViewPawn::ActivateStoredMovement() {
	MoveToCharacter(CurrentLookCharacter, StoredMovementType, StoredSnap, StoredZoom);
}


void AViewPawn::SpinAroundCharacter() {

}



void AViewPawn::SetMoving() {
	StartPosition = GetActorLocation();
	StartRotation = GetActorRotation();


	JourneyLength = (StartPosition - FinalPosition).Size();	


	CalculateMidPositions();

	if (DrawDebugLines) {
		DrawDebugLine(GetWorld(), StartPosition, MidPoint1, FColor::Green, false, 5.0f, 0, 10.0f);
		DrawDebugLine(GetWorld(), MidPoint1, MidPoint2, FColor::Green, false, 5.0f, 0, 10.0f);
		DrawDebugLine(GetWorld(), MidPoint2, FinalPosition, FColor::Green, false, 5.0f, 0, 10.0f);
	}

	StartTime = UGameplayStatics::GetTimeSeconds((GetWorld()));




	IsMoving = true;
}

//Instead of lerping rotation to final rotation, always have the camera looking at it's target or current direction or something like that
void AViewPawn::ShiftToFinalPosition(float dt) {
	//FMath::Lerp(StartPosition, FinalPosition, 0);

	float DistCovered = (UGameplayStatics::GetTimeSeconds(GetWorld()) - StartTime) * Speed;
	float Alpha = DistCovered / JourneyLength;
	
	Alpha = FMath::Sin(Alpha);
	//GEngine->AddOnScreenDebugMessage(-1, 2.0f, FColor::Cyan, FString::Printf(TEXT("Alpha: %f"), Alpha));

	SetActorLocation(CubicBezierCurve(StartPosition, MidPoint1, MidPoint2, FinalPosition, Alpha));

	//Consider splitting these into different functions to get rid of the if statement
	if (ZoomOut) {
		SetActorRotation(FQuat::Slerp(StartRotation.Quaternion(), FinalRotation.Quaternion(), Alpha));
	}
	else {
		//This works for when you're moving toward the thing you're looking at
		FVector RotDir = (FQuat::Slerp(StartRotation.Quaternion(), FinalRotation.Quaternion(), Alpha)).Vector() * Alpha;
		FVector LookAtTargetDir = (((LookPos - GetActorLocation()).GetSafeNormal() * (1.0f - Alpha)));
		SetActorRotation((LookAtTargetDir + RotDir).Rotation());






		//DrawDebugLine(GetWorld(), GetActorLocation(), GetActorLocation() + (GetActorRotation().Vector() * 200.0f), FColor::Red, false, 5.0f, 0, 10.0f);

		//DrawDebugLine(GetWorld(), GetActorLocation(), FinalPosition, FColor::Red, false, 5.0f, 0, 10.0f);
		//DrawDebugLine(GetWorld(), GetActorLocation(), GetActorLocation() + ((LookAtTargetDir + RotDir) * 50.0f), FColor::Green, false, 5.0f, 0, 10.0f);

	}
	



	//(GetActorLocation() - FinalPosition).Rotation() + (GetActorLocation() - FinalPosition).Rotation();
	if (DrawDebugLines) {
		DrawDebugLine(GetWorld(), GetActorLocation(), GetActorLocation() + 1.0f, FColor::Red, false, 5.0f, 0, 20.0f);

	}

	
	if (Alpha >= 0.995f) {
		
		EndCameraMovement();
		//Snap because the speed of the camera often results in the camera overcshooting its target
		SnapToFinalPosition();
		

	}
	
}

//Cut straight to characters overhead, then lerp in on their front
void AViewPawn::SwoopInFromOverhead(ARPGCharacter* Character) {
	MoveToCharacter(Character, OverheadFront);


	MoveToCharacterDelay(Character, Front, false, ZoomOut);
}

void AViewPawn::EndCameraMovement() {
	IsMoving = false;
	JourneyLength = 0;

	//You need to differentiate between shifting the camera during actions and during menuing
	GameMode->TriggerEndOfCameraMovement();
}

void AViewPawn::SnapToFinalPosition() {
	SetActorLocation(FinalPosition);
	SetActorRotation(FinalRotation);
}

void AViewPawn::ReceiveCamInstruction(bool IsFriendly, bool IsMultiTarget) {

	if (IsFriendly && IsMultiTarget) {
		//MoveToCharacter(Character, Field);
	}
}

//Try multiplying curve adjustment by a random range of -1 to 1
void AViewPawn::CalculateMidPositions() {
	//float RandX = FMath::RandRange(-XRandRange, XRandRange) / 100.0f;
	//float RandY = FMath::RandRange(-XRandRange, XRandRange) / 100.0f;
	//float RandZ = FMath::RandRange(-XRandRange, XRandRange) / 100.0f;
	//FVector(CurveAdjustment.X, CurveAdjustment.Y, CurveAdjustment.Z);

	float Rand = FMath::RandRange(-100, 100) / 100.0f;

	/* Cubic method
	float Dist = (FinalPosition - StartPosition).Size();
	FVector Dir = (FinalPosition - StartPosition).GetSafeNormal();


	MidPoint1 = (StartPosition + (Dir * (Dist * 0.25f))) + (CurveAdjustment);
	MidPoint2 = (StartPosition + (Dir * (Dist * 0.75f))) + (CurveAdjustment);
	*/
	
	MidPoint2 = ((StartPosition + FinalPosition) * 0.5f);

	if (!ZoomOut) {
		GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Red, "Not set to zoom out!!");
		MidPoint2 += CurveAdjustment;
	}
	else {
		GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Green, "Set to zoom out!!");
	}

	MidPoint1 = MidPoint2;

}

void AViewPawn::CalculateCharacterMidpoint(ARPGCharacter* Character) {

	if (Character->QueuedAction == UseSkill) {
		if (Character->SkillComponent->QueuedSkill->IsMultiTarget) {
			CalculateCharacterMidpointMultiple(Character);

			return;
		}
	}
	else if (Character->QueuedAction == UseItem) {
		if (Character->QueuedItem->IsMultiTarget) {
			CalculateCharacterMidpointMultiple(Character);

			return;
		}
	}

	CharacterMidpoint = (Character->GetActorLocation() + Character->StoredTarget->GetActorLocation()) / 2.0f;


}

void AViewPawn::CalculateCharacterMidpointMultiple(ARPGCharacter* Character) {
	CharacterMidpoint = Character->GetActorLocation();
	TArray<ARPGCharacter*> Targets = *Character->StoredTargetMultiple;


	for (int i = 0; i < Targets.Num(); i++) {
		CharacterMidpoint += Targets[i]->GetActorLocation();
	}

	CharacterMidpoint /= (Targets.Num() + 1);

}

FVector AViewPawn::CubicBezierCurve(FVector P0, FVector P1, FVector P2, FVector P3, float alpha) {


	FVector Point = P0 +
		(alpha * ((-3 * P0) + (3 * P1))) +
		(FMath::Pow(alpha, 2) * ((3 * P0) - (6 * P1) + (3 * P2))) +
		(FMath::Pow(alpha, 3) * ((-P0) + (3 * P1) - (3 * P2) + P3));


	return Point;
}