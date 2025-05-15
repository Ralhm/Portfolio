// Fill out your copyright notice in the Description page of Project Settings.


#include "RPGAlly.h"
#include "RPGEnemy.h"

#include "Item_ConsumableAsset.h"

#include "Kismet/GameplayStatics.h"

#include "RPGGameGameMode.h"

#include "ViewPawn.h"

// Called when the game starts or when spawned
void ARPGAlly::BeginPlay()
{
	Super::BeginPlay();

	CurrentDrawRate = BaseDrawRate;
}

void ARPGAlly::SetDrawRate(int min, int max) {
	DrawRateMin = min;
	DrawRateMax = max;
}

void ARPGAlly::SetCurrentDrawRate(int val) {
	CurrentDrawRate = val;

	Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()))->CalculateDrawRate();


}

void ARPGAlly::LoseHealth(int Amount) {
	Super::LoseHealth(Amount);
}

void ARPGAlly::Die() {
	Super::Die();
	HP = 0;
	CurrentDrawRate = 0;

	ARPGGameGameMode* GameMode = Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()));
	GameMode->CalculateDrawRate();

	UAnimInstance* instance = GetMesh()->GetAnimInstance();
	float MontageLength = instance->Montage_Play(DeathMontage, 1);
	GetWorld()->GetTimerManager().SetTimer(ActionHandle, this, &ARPGAlly::DeathEnd, MontageLength);
	GameMode->SetDeathDelayTime(MontageLength);

}


void ARPGAlly::DeathEnd() {

}

void ARPGAlly::Revive(int Amount) {
	GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Purple, "Player getting revived!");

	SetCurrentStatus(Normal);
	GetMesh()->GetAnimInstance()->Montage_Stop(1, DeathMontage);
	CurrentDrawRate = BaseDrawRate;
	RestoreHealth(Amount);

	Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()))->CalculateDrawRate();
}


//1. Either we have skills and items contain a piece of data saying what the camera should do
//2. Or we put if statements in here that parse info from the skill/item and then decide what the camera should do
//3. We pass in data from the skill/item to the viewPawn and let that decide 

//Option 1 means more data stored in each skill and one more thing to assign
//option 2 would probably be awful to maintain, and would require re-compiling and shit
//Option 3 since we know certain actions will always result in certain camera movements, but would still limit
//Multi skills/items will always result in a field view
//Single target Friendly skills will always result in looking at the person who got healed
//Single target Enemy skills will always result in

//But what if we don't want certain action types to always result in the same camera movement?
//What if we want two different single target skills to result in two different camera actions?

void ARPGAlly::BeginAction() {
	if (!CheckForAilments()) {
		EndAction();
		return;
	}

	ARPGGameGameMode* GameMode = Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()));


	//Have the action itself (skill/item) decide what the camera does
	if (QueuedAction == UseSkill) {
		if (!SkillComponent->QueuedSkill->IsFriendly) {

			CheckNewEnemyTarget();
		}
		BeginAnimationSkill(SkillComponent->QueuedSkill->SkillType);
		if (!SkillComponent->QueuedSkill->IsMultiTarget) {
			LookAtTarget();
		}
		
		GameMode->TellCameraWhatToDo(this, SkillComponent->QueuedSkill->CamType, SkillComponent->QueuedSkill->Snap, SkillComponent->QueuedSkill->ZoomOut, true);


		return;
	}
	else if (QueuedAction == NormalAttack) {
		CheckNewEnemyTarget();
		LookAtTarget();

		GameMode->TellCameraWhatToDo(this, NormalAttackCameraAngle, true, NormalAttackCameraZoom, true);


	}
	else if (QueuedAction == UseItem) {
		if (QueuedItem->IsMultiTarget) {
			GameMode->TellCameraWhatToDo(this, Field, true, false, true);

		}
		else {
			LookAtTarget();
			GameMode->TellCameraWhatToDo(StoredTarget, Front, true, false, true);
		}




	}
	else if (QueuedAction == Defend) {
		GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Yellow, FString::Printf(TEXT("Defending!!")));
	}


	BeginAnimationAction(QueuedAction);
}