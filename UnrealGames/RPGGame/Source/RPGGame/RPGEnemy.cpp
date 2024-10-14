// Fill out your copyright notice in the Description page of Project Settings.


#include "RPGEnemy.h"

#include "Kismet/GameplayStatics.h"


#include "RPGGameGameMode.h"


void ARPGEnemy::ChooseAction() {
	QueuedAction = NormalAttack;

}

void ARPGEnemy::BeginAction() {
	if (!CheckForAilments()) {
		EndAction();
		return;
	}



	StoredTarget = Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()))->GetAllyToTarget();


	if (QueuedAction == UseSkill) {
		BeginAnimationSkill(SkillComponent->QueuedSkill->SkillType);
	}
	else {
		BeginAnimationAction(QueuedAction);
	}

}

void ARPGEnemy::Die() {
	GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Magenta, "ENEMY DEATH!!");
	SetCurrentStatus(Dead);

	UAnimInstance* instance = GetMesh()->GetAnimInstance();
	float MontageLength = instance->Montage_Play(DeathMontage, 1);


	Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()))->RemoveEnemy(this);


	GetWorld()->GetTimerManager().SetTimer(DeathHandle, this, &ARPGEnemy::DeathEnd, MontageLength);


}

void ARPGEnemy::DeathEnd() {
	GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Magenta, "Enemy going into hidden!!");
	SetActorHiddenInGame(true);
}