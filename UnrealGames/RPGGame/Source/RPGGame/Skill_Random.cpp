// Fill out your copyright notice in the Description page of Project Settings.


#include "Skill_Random.h"
#include "RPGCharacter.h"
#include "RPGGameGameMode.h"
#include "Kismet/GameplayStatics.h"

void USkill_Random::OnActivate(ARPGCharacter* User) {
	UseSkillOnMultiple(User, *User->StoredTargetMultiple);
}

void USkill_Random::UseSkillOnMultiple(ARPGCharacter* User, TArray<ARPGCharacter*> Targets) {
	int NumHits = FMath::RandRange(NumHitsBottom, NumHitsTop);
	int Rand = 0;
	for (int i = 0; i < NumHits; i++) {
		Rand = FMath::RandRange(0, Targets.Num() - 1);
		Targets[Rand]->TakeDamage(CalculateDamage(User, Targets[Rand]));
		GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Purple, Targets[Rand]->GetName());
	}



}