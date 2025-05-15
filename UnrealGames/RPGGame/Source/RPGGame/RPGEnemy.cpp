// Fill out your copyright notice in the Description page of Project Settings.


#include "RPGEnemy.h"

#include "Kismet/GameplayStatics.h"


#include "RPGGameGameMode.h"

void ARPGEnemy::BeginPlay()
{
	Super::BeginPlay();


}

void ARPGEnemy::ChooseAction() {
	QueuedAction = NormalAttack;

}

void ARPGEnemy::BeginAction() {
	if (!CheckForAilments()) {
		EndAction();
		return;
	}

	ChooseAction();

	ARPGGameGameMode* GameMode = Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()));


	StoredTarget = GameMode->GetAllyToTarget();


	if (QueuedAction == UseSkill) {
		BeginAnimationSkill(SkillComponent->QueuedSkill->SkillType);
		if (!SkillComponent->QueuedSkill->IsMultiTarget) {
			LookAtTarget();
		}
	}
	else if (QueuedAction == Defend) {
		GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Yellow, FString::Printf(TEXT("Defending!!")));
	}
	else {
		BeginAnimationAction(QueuedAction);
		LookAtTarget();

		GameMode->TellCameraWhatToDo(this, NormalAttackCameraAngle, true, false, true);

	}


}

void ARPGEnemy::Die() {
	GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Magenta, "ENEMY DEATH!!");
	Super::Die();


	UAnimInstance* instance = GetMesh()->GetAnimInstance();
	float MontageLength = instance->Montage_Play(DeathMontage, 1);


	ARPGGameGameMode* GameMode = Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()));
	GameMode->RemoveEnemy(this);

	GetWorld()->GetTimerManager().SetTimer(DeathHandle, this, &ARPGEnemy::DeathEnd, MontageLength);
	GameMode->SetDeathDelayTime(MontageLength);

}

void ARPGEnemy::DeathEnd() {
	GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Magenta, "Enemy going into hidden!!");
	SetActorHiddenInGame(true);
}

