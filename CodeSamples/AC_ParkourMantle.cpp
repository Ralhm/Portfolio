// Fill out your copyright notice in the Description page of Project Settings.


#include "AC_ParkourMantle.h"
#include "DrawDebugHelpers.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "Kismet/GameplayStatics.h"
#include "Kismet/KismetSystemLibrary.h"
#include "Kismet/KismetMathLibrary.h"
#include "ParkourrorCharacter.h"

#include "Components/AudioComponent.h"

// Sets default values for this component's properties
UAC_ParkourMantle::UAC_ParkourMantle()
{



	// ...
}


// Called when the game starts
void UAC_ParkourMantle::BeginPlay()
{
	Super::BeginPlay();



}


// Called every frame
void UAC_ParkourMantle::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);



}


void UAC_ParkourMantle::ForwardTrace() {
	FVector StartPos = (GetOwner()->GetActorLocation() + FVector(0, 0, MantleForwardPos));
	FVector EndPos = StartPos + (GetOwner()->GetActorForwardVector() * MantleForwardTraceDistance);

	UKismetSystemLibrary::SphereTraceSingleForObjects(GetWorld(), StartPos, EndPos, ForwardTraceRadius, ObjectTypes, false, ActorsToIgnore, EDrawDebugTrace::None, ForwardHit, true, FLinearColor::Red, FLinearColor::Green, 5.0f);

	MantleNormal = ForwardHit.Normal;
	MantleLocation = ForwardHit.Location;


	if (ForwardHit.bBlockingHit) {
		CanBoost = true;
	}
	else {
		CanBoost = false;
		isClambering = false;
		if (IsWallSliding) {
			StopSlidingDownWall();
		}
		if (isClambering) {
			GetMovementComp()->Velocity = FVector(0, 0, 0);
			GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Magenta, "SPECIAL CLAMBER STOP!!!");
			StopClambering();
		}
		
	}

}

//I elected to keep some of this logic separate from Boost so that Boost could be called on its own if deemed necessary
void UAC_ParkourMantle::BoostLeap(float Power) {
	GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Emerald, "Boosting from high up!!!");
	StopClambering();
	isBoosting = true;	
	Boost(Power);
}


void UAC_ParkourMantle::Boost(float Power) {
	AParkourrorCharacter* player = GetCharacter();

	FVector Start = GetOwner()->GetActorLocation();
	FVector BoostVel = player->GetActorForwardVector() + (player->GetActorUpVector() * BoostAngle);
	BoostVel *= Power;

	DrawDebugLine(GetWorld(), Start, Start + BoostVel.GetSafeNormal() * 100, FColor::Green, false, 20.0f, 0, 10.0f);


	float JumpPower = BoostVel.Z + VerticalJumpBoost;
	FVector vel = FVector(BoostVel.X, BoostVel.Y, JumpPower);

	player->Boost(vel);
	if (jumpSound) {
		UGameplayStatics::CreateSound2D(GetWorld(), jumpSound)->Play();
	}
	else {
		GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Red, "JUMPSOUND NOT SET!!!");
	}
}

void UAC_ParkourMantle::BoostLeapClimb() {
	UAnimInstance* instance = GetCharacter()->GetMesh()->GetAnimInstance();
	if (instance->Montage_IsPlaying(ClimbMontage)) {
		float pos = instance->Montage_GetPosition(ClimbMontage);
		if (pos < MontageBoostLimit) {
			if (!BoostBuffering) { //Queue user desired input to boost
				instance->Montage_SetPlayRate(ClimbMontage, MontageBoostPlayRate);
				GetWorld()->GetTimerManager().SetTimer(ClimbBoostHandle, this,
					&UAC_ParkourMantle::BoostLeapBuffer, 0.1f, true);
				BoostBuffering = true;
			}
		}
		else { //Boost out of climb if close enough to end of montage
			ExitClimb();
			instance->Montage_Stop(1, ClimbMontage);
			BoostLeap(BoostPowerClimb);
		}

	}
	else { //Montage Not Playing failsafe
		ExitClimb();
		BoostLeap(BoostPowerClimb);
	}

}

//Activated Queued input
void UAC_ParkourMantle::BoostLeapBuffer() {
	UAnimInstance* instance = GetCharacter()->GetMesh()->GetAnimInstance();
	float pos = instance->Montage_GetPosition(ClimbMontage);


	if (pos > MontageBoostLimit) {
		GetWorld()->GetTimerManager().ClearTimer(ClimbBoostHandle);
		ExitClimb();
		BoostBuffering = false;
		instance->Montage_Stop(1, ClimbMontage);
		BoostLeap(BoostPowerClimb);
	}
}



void UAC_ParkourMantle::AttemptBoost() {
	IsAttemptingToVault = true;
	//GEngine->AddOnScreenDebugMessage(-1, 0.1f, FColor::Orange, "Attempting Boost!");
	GetWorld()->GetTimerManager().SetTimer(BoostHandle, this, &UAC_ParkourMantle::EndBoostTimer, BoosTime);
}

void UAC_ParkourMantle::EndBoostTimer() {
	//GEngine->AddOnScreenDebugMessage(-1, 3.0f, FColor::Orange, "No Longer Attempting Vault!");
	GetWorld()->GetTimerManager().ClearTimer(BoostHandle);
	IsAttemptingToVault = false;
}

void UAC_ParkourMantle::FootTrace() {

	FVector FootStartPos = (GetOwner()->GetActorLocation() - FVector(0, 0, -MantleFootPos));
	FVector FootEndPos = FootStartPos + (GetOwner()->GetActorForwardVector() * MantleForwardTraceDistance);
	UKismetSystemLibrary::LineTraceSingleForObjects(GetWorld(), FootStartPos, FootEndPos, ObjectTypes, false, ActorsToIgnore, EDrawDebugTrace::ForDuration, FootHit, true, FLinearColor::Red, FLinearColor::Green, 10.0f);

	FootHitLocation = FootHit.Location;

}

void UAC_ParkourMantle::VaultFootTrace() {

}

void UAC_ParkourMantle::HeightTrace() {



	const FVector StartPos = (GetOwner()->GetActorLocation() + FVector(0, 0, 150.0f) + (GetOwner()->GetActorForwardVector() * MantleHeightTraceDistance));
	const FVector EndPos = StartPos - FVector(0, 0, MantleHeightLength);
	FHitResult HeightHit;
	UKismetSystemLibrary::SphereTraceSingleForObjects(GetWorld(), StartPos, EndPos, HeightTraceRadius, ObjectTypes, false, ActorsToIgnore, EDrawDebugTrace::None, HeightHit, true, FLinearColor::Red, FLinearColor::Green, 10.0f);


	//If we're in front of a wall
	if (HeightHit.bBlockingHit) {
		MantleHeight = HeightHit.Location;

		float HeightDiff = MantleHeight.Z - MantleLocation.Z;
		float FootDiff = MantleHeight.Z - FootHitLocation.Z;
		//GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Yellow, FString::Printf(TEXT("HeightDiff: %f"), HeightDiff));
		//GEngine->AddOnScreenDebugMessage(-1, 1.0f, FColor::Yellow, FString::Printf(TEXT("FootDiff: %f"), FootDiff));


		FVector forwardVel = GetMovementComp()->Velocity;
		forwardVel = FVector(forwardVel.X, forwardVel.Y, 0);
		float forwardSpeed = forwardVel.Size();

		float ForwardDot = FVector::DotProduct(GetOwner()->GetActorForwardVector(), forwardVel.GetSafeNormal());


		//Check for if there's a ledge for us to climb up
		if (!GetMovementComp()->IsMovingOnGround() && HeightDiff < MantleLimitHigh && HeightDiff > MantleLimitLow && !isVaulting && HeightHit.bBlockingHit) {
			
			if (!isClimbing) {
				GEngine->AddOnScreenDebugMessage(-1, 7.0f, FColor::Cyan, FString::Printf(TEXT("HeightDiff: %f"), HeightDiff));
				GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Yellow, "Entering Climb!");
				StopClambering();
				isClimbing = true;


				FRotator rot = ForwardHit.ImpactNormal.Rotation();
				rot.Pitch = 0;

				GetOwner()->SetActorRotation(rot);

				UAnimInstance* instance = GetCharacter()->GetMesh()->GetAnimInstance();
				if (instance) {
					float animSpeed = 1.0f; 

					float MontageLength = instance->Montage_Play(ClimbMontage, animSpeed);
					MontageLength /= animSpeed;
					GetCharacter()->SetCapsuleGravity(false);
					GetMovementComp()->SetMovementMode(EMovementMode::MOVE_Flying);
					GetWorld()->GetTimerManager().SetTimer(ClimbHandle, this, &UAC_ParkourMantle::ExitClimb, MontageLength);
					if (IsAttemptingToVault) {
						BoostLeapClimb();
					}
				}
			}

			return;

		}



		float MantleDot = FVector::DotProduct(MantleNormal, GetOwner()->GetActorForwardVector());
		MantleDot = abs(MantleDot);


		//If there's no ledge, either clamber up the wall, or begin sliding down
		if (HeightDiff > WallClimbLimit && ForwardHit.bBlockingHit && MantleDot > DesiredWallClimbAngle && HeightHit.bBlockingHit) {
			//GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Magenta, "Clambering!!");
			float DistanceDiff = (GetOwner()->GetActorLocation() - ForwardHit.Location).Size();
			//GEngine->AddOnScreenDebugMessage(-1, 8.0f, FColor::Yellow, FString::Printf(TEXT("Distance Diff: %f"), DistanceDiff));
			if (!isClambering && GetMovementComp()->IsFalling() && CanClamber && GetMovementComp()->Velocity.Z > MinClamberVelocity && !isClimbing && DistanceDiff < MinClamberDistance) {
				

				GEngine->AddOnScreenDebugMessage(-1, 6.0f, FColor::Cyan, FString::Printf(TEXT("HeightDiff: %f"), HeightDiff));
				//GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Green, FString::Printf(TEXT("Dot: %f"), Dot));

				if (HoldingJump) {			
					//GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Green, FString::Printf(TEXT("Entering from hold")));
					EnterClamber();
				}
				else if (IsAttemptingToVault) {
					//GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Green, FString::Printf(TEXT("Entering from Vaulting buffer!")));
					EnterClamber();
				}
				else {
					//GEngine->AddOnScreenDebugMessage(-1, 8.0f, FColor::Yellow, FString::Printf(TEXT("Distance Diff: %f"), DistanceDiff));
					SlideDownWall();
				}

			}

			return;
		}

		//If the player jumps at just the right moment in front of a small ledge, do a little boost
		if (!GetMovementComp()->IsMovingOnGround() && FootDiff < MantleBoostLimit && FootDiff > 0) {
			if (!isClimbing && !isBoosting && ForwardDot > -0.2f && IsAttemptingToVault) {
				//Do an extra trace because in build it gets fucky and thinks it's still hitting something because you don't trace all the time
				//So if you hit something, quit tracing, then try to jump again it will think it's hitting because last time it checked, it was hitting something
				FootTrace();
				//Prevent the player from boosting or vaulting when walking up a slope
				float vertDot = FVector::DotProduct(FootHit.ImpactNormal, FVector(0, 0, 1));

				//Dumb check to stop boosting on floors in build
				float PlayerDot = FVector::DotProduct(FootHit.ImpactNormal, (GetOwner()->GetActorLocation() - FootHitLocation).GetSafeNormal());

				if (FootHit.bBlockingHit && vertDot == 0 && PlayerDot != 0) {
					//GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Cyan, FString::Printf(TEXT("HeightDiff: %f"), HeightDiff));
					GEngine->AddOnScreenDebugMessage(-1, 6.0f, FColor::Yellow, FString::Printf(TEXT("FootDiff: %f"), FootDiff));
					GEngine->AddOnScreenDebugMessage(-1, 6.0f, FColor::Yellow, FString::Printf(TEXT("VertDot: %f"), vertDot));
					GEngine->AddOnScreenDebugMessage(-1, 6.0f, FColor::Yellow, FString::Printf(TEXT("PlayerDot: %f"), PlayerDot));
					GEngine->AddOnScreenDebugMessage(-1, 6.0f, FColor::Yellow, FString::Printf(TEXT("FootHit.ImpactNormal: %f %f %f"), FootHit.ImpactNormal.X, FootHit.ImpactNormal.Y, FootHit.ImpactNormal.Z));
					DrawDebugLine(GetWorld(), FootHitLocation, FootHitLocation + FootHit.ImpactNormal * 100, FColor::Orange, false, 12.0f, 0, 100.0f);
					DrawDebugLine(GetWorld(), FootHitLocation, GetOwner()->GetActorLocation(), FColor::Magenta, false, 12.0f, 0, 20.0f);
					BoostLeap(BoostPowerLaunch);
				}



			}
			return;

		}

	}
	else {
		MantleHeight = FVector(0, 0, 0);
		//if (isVaulting) { SetVaultJump(); }
	}


}

void UAC_ParkourMantle::SlideDownWall() {
	if (!IsWallSliding) {
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Orange, FString::Printf(TEXT("Sliding!")));
		GetMovementComp()->BrakingDecelerationFalling = 0;
		GetMovementComp()->AirControl = 0;
		GetMovementComp()->GravityScale = SlideGravity;
		GetMovementComp()->FallingLateralFriction = 20;
		GetMovementComp()->SetPlaneConstraintNormal(FVector(0, 0, 1));
		GetMovementComp()->Velocity = FVector(0, 0, 0);
		GetCharacter()->StopJumping();
		IsWallSliding = true;
	}

	if (GetCharacter()->jumpStored)
	{
		EnterClamber();
	}
}


void UAC_ParkourMantle::VaultOverLedge() {
	UAnimInstance* instance = GetCharacter()->GetMesh()->GetAnimInstance();
	if (instance) {
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Blue, "Vaulting!!");

		isVaulting = true;
		float MontageLength = instance->Montage_Play(VaultMontage);
		GetCharacter()->InstaShrinkCapsule(MontageLength);
		GetCharacter()->SetCapsuleGravity(false);
		GetMovementComp()->SetMovementMode(EMovementMode::MOVE_Flying);
		GetWorld()->GetTimerManager().SetTimer(VaultHandle, this, &UAC_ParkourMantle::ExitVault, MontageLength);
		
	}
}

void UAC_ParkourMantle::ExitVault() {
	isVaulting = false;

	UAnimInstance* instance = GetCharacter()->GetMesh()->GetAnimInstance();

	if (instance) {
		instance->Montage_Stop(0.5f, VaultMontage);
	}

	GetWorld()->GetTimerManager().ClearTimer(VaultHandle);
	GetMovementComp()->SetMovementMode(EMovementMode::MOVE_Walking);
	GetCharacter()->SetCapsuleGravity(true);
	GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Green, "Exited Vaulting!");
}


void UAC_ParkourMantle::StopSlidingDownWall() {
	GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, FString::Printf(TEXT("Stopping Sliding!")));
	IsWallSliding = false;
	GetCharacter()->ResetMovementVals();
}

void UAC_ParkourMantle::SetVaultJump() {

	GetWorld()->GetTimerManager().SetTimer(VaultHandle, this, &UAC_ParkourMantle::VaultBoostTimer, 0.2f);

}


void UAC_ParkourMantle::VaultBoostTimer() {
	GetCharacter()->ResetJumpPower();
	//isVaulting = false;
	GetWorld()->GetTimerManager().ClearTimer(VaultHandle);
	//GEngine->AddOnScreenDebugMessage(-1, 3.0f, FColor::Red, "No Longer Vaulting!!");
}


void UAC_ParkourMantle::SetMantleStepHeight() {
	GetMovementComp()->MaxStepHeight = MantleStepHeight;
}


void UAC_ParkourMantle::ResetMantleStepHeight() {
	GetCharacter()->ResetStepHeight();
}


void UAC_ParkourMantle::ClamberUpWall(float deltaTime) {
	if (isClambering) {
		//GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Green, "Clambering!!");
		if (timeSpentWallClambering + deltaTime <= ClamberTime)
		{
			GetMovementComp()->AddForce(FVector(0, 0, ClamberForce));
			
		}
		else if (timeSpentWallClambering < ClamberTime)
		{
			GetMovementComp()->AddImpulse(FVector(0, 0, ClamberForce) * (ClamberTime - timeSpentWallClambering));
		}
		else
		{
			if (GetCharacter()->GetVelocity().Z < 10)
			{
				StopClambering();
			}
		}

		timeSpentWallClambering += deltaTime;

	}
}

//Begin the animation montage and set state values
void UAC_ParkourMantle::EnterClamber() {
	GetMovementComp()->BrakingDecelerationFalling = 0;
	GetMovementComp()->AirControl = 0;
	GetMovementComp()->GravityScale = 1;
	GetMovementComp()->FallingLateralFriction = 20;
	GetMovementComp()->SetPlaneConstraintNormal(FVector(0, 0, 1));
	//GetMovementComp()->Velocity = FVector(0, 0, GetMovementComp()->Velocity.Z);
	GetMovementComp()->Velocity = FVector(0, 0, 0);
	GetCharacter()->StopJumping();

	isClambering = true;
	CanClamber = 0;
	//GetWorld()->GetTimerManager().SetTimer(ClamberHandle, this, &UAC_ParkourMantle::StopClambering, ClamberTime);

	timeSpentWallClambering = 0.0f;

	GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Magenta, "Entering Clamber!!");
}


void UAC_ParkourMantle::StopClambering() {
	GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, "Stopped Clamber!!");
	isClambering = false;
	GetCharacter()->ResetMovementVals();
}


void UAC_ParkourMantle::SetCanClamber() {
	CanClamber = true;
	isClimbing = false;
}


bool UAC_ParkourMantle::GetIsClambering() {

	return isClambering;
}

void UAC_ParkourMantle::ExitClimb() {
	GetWorld()->GetTimerManager().ClearTimer(ClimbHandle);
	GetMovementComp()->SetMovementMode(EMovementMode::MOVE_Walking);
	GetCharacter()->SetCapsuleGravity(true);
	SetCanClamber();
	GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Green, "Exited Climbing!");

}

void UAC_ParkourMantle::AttemptMantle() {
	ForwardTrace();
	HeightTrace();
}


UCharacterMovementComponent* UAC_ParkourMantle::GetMovementComp() const {

	return Cast<AParkourrorCharacter>(GetOwner())->GetCharacterMovement();
}

AParkourrorCharacter* UAC_ParkourMantle::GetCharacter() const {
	return Cast<AParkourrorCharacter>(GetOwner());
}

