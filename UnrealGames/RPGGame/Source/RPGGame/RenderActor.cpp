// Fill out your copyright notice in the Description page of Project Settings.


#include "RenderActor.h"

#include "Components/SceneCaptureComponent2D.h"
#include "Components/SkeletalMeshComponent.h"


#include "Kismet/GameplayStatics.h"

#include "Materials/Material.h"

// Sets default values
ARenderActor::ARenderActor()
{
	PrimaryActorTick.bCanEverTick = true;

	DefaultRoot = CreateDefaultSubobject<USceneComponent>(TEXT("DefaultRoot"));
	SetRootComponent(DefaultRoot);

	SkeletalMesh = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("SkeletalMesh"));


	RenderCamera = CreateDefaultSubobject<USceneCaptureComponent2D>(TEXT("RenderCamera"));


	SkeletalMesh->SetupAttachment(DefaultRoot);
	RenderCamera->SetupAttachment(DefaultRoot);
	//RenderCamera->SetupAttachment(SkeletalMesh);

}


void ARenderActor::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	if (IsMoving) {
		ShiftToPosition(DeltaTime);
	}

}



void ARenderActor::SetAnimation(EDialogueAnimType AnimType) {
	//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Green, "Animating!");
	//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Green, GetSpeakerName().ToString());


	//UNCOMMENT THIS WHEN YOU IMPLEMENT TALKING
	/*
	if (CurrentEmote) {
		if (AnimType == CurrentEmote->AnimType) {
			return;
		}
	}
	*/


	bool FoundEmote = false;

	for (int i = 0; i < Properties->Emotes.Num(); i++) { //Consider converting this to a map and just passing in the enum as a kay
		if (AnimType == Properties->Emotes[i].AnimType) {
			CurrentEmote = &Properties->Emotes[i];
			FoundEmote = true;
		}
	}

	if (FoundEmote) {
		EnterEmote();
	}
	else {
		GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Red, "FAILED TO FIND EMOTE ANIM!");
	}
}


void ARenderActor::SetSkeleton(FName CharacterName) {

	Properties = RenderActorTable->FindRow<FRenderActorProperties>(CharacterName, "");

	SkeletalMesh->SetSkeletalMesh(Properties->SkeletalMesh);
	SkeletalMesh->SetAnimClass(Properties->AnimClass);
}

void ARenderActor::SetRenderTexture(UTextureRenderTarget2D* Texture) {
	RenderCamera->TextureTarget = Texture;
}

FName ARenderActor::GetSpeakerName() {
	return Properties->SpeakerName;
}

void ARenderActor::ShiftToPosition(float dt) {
	float DistCovered = (UGameplayStatics::GetTimeSeconds(GetWorld()) - StartTime) * LerpSpeed;
	float Alpha = DistCovered / JourneyLength;

	SkeletalMesh->SetRelativeLocation(FMath::Lerp(StartPos, EndPos, Alpha));


	if (Alpha >= 0.995f) {
		IsMoving = false;
	}
}



void ARenderActor::SetMoving(bool Exit) {
	if (Exit) {
		StartPos = FVector(0, 0, 0);
		EndPos = OffScreenPosition;
	}
	else {
		EndPos = FVector(0, 0, 0);
		StartPos = OffScreenPosition;
	}


	IsMoving = true;
	StartTime = UGameplayStatics::GetTimeSeconds((GetWorld()));
	SkeletalMesh->SetRelativeLocation(OffScreenPosition);



	JourneyLength = (OffScreenPosition - FVector(0, 0, 0)).Size();

	GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Green, "Moving!");
	GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Green, GetSpeakerName().ToString());
}


void ARenderActor::EnterEmote() {
	UAnimInstance* instance = SkeletalMesh->GetAnimInstance();

	//DELETE THIS WHEN YOU IMPLEMENT TALKING
	if (instance->GetCurrentActiveMontage() == CurrentEmote->EnterMontage) {
		return;
	}

	if (!instance) {
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, "FAILED TO GET ANIM INSTANCE!");
		return;
	}

	

	float MontageLength = instance->Montage_Play(CurrentEmote->EnterMontage);
	//GetWorldTimerManager().SetTimer(EmoteHandle, this, &ARenderActor::EnterIdle, MontageLength);
}



void ARenderActor::EnterIdle() {
	UAnimInstance* instance = SkeletalMesh->GetAnimInstance();
	if (!instance) {
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, "FAILED TO GET ANIM INSTANCE!");
		return;
	}

	instance->Montage_Play(CurrentEmote->IdleMontage);
}