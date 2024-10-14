// Fill out your copyright notice in the Description page of Project Settings.


#include "RPGAlly.h"
#include "RPGEnemy.h"


#include "Kismet/GameplayStatics.h"

#include "RPGGameGameMode.h"

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

void ARPGAlly::Die() {
	SetCurrentStatus(Dead);
	HP = 0;
	CurrentDrawRate = 0;

	Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()))->CalculateDrawRate();

	UAnimInstance* instance = GetMesh()->GetAnimInstance();
	float MontageLength = instance->Montage_Play(DeathMontage, 1);
	GetWorld()->GetTimerManager().SetTimer(ActionHandle, this, &ARPGAlly::DeathEnd, MontageLength);


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

void ARPGAlly::BeginAction() {
	if (!CheckForAilments()) {
		EndAction();
		return;
	}

	if (QueuedAction == UseSkill) {
		if (!SkillComponent->QueuedSkill->IsFriendly) {
			CheckNewEnemyTarget();
		}
		BeginAnimationSkill(SkillComponent->QueuedSkill->SkillType);
		return;
	}
	else if (QueuedAction == NormalAttack) {
		CheckNewEnemyTarget();

	}
	else if (QueuedAction == UseItem) {
		//Check if item is offensive. if it is, check if enemy target is valid
		//CheckNewEnemyTarget();
	}
	else if (QueuedAction == Defend) {
		GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Yellow, FString::Printf(TEXT("Defending!!")));
	}


	BeginAnimationAction(QueuedAction);
}