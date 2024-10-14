// Fill out your copyright notice in the Description page of Project Settings.


#include "AC_Status.h"

#include "Kismet/GameplayStatics.h"

#include "RPGGameGameMode.h"
#include "RPGCharacter.h"

// Sets default values for this component's properties
UAC_Status::UAC_Status()
{


}

void UAC_Status::Poison() {
	GetCharacter()->LoseHealth(PoisonDamage);
	GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Purple, "Taking Poison damage!");
}

//You'll want to keep these functions so you can add thigns like sounds or particle effects to when they get applied
void UAC_Status::InflictPoison(int Damage) {
	CurrentStatus = Poisoned;
	PoisonDamage = Damage;
}

void UAC_Status::InflictConfusion() {
	CurrentStatus = Confuse;
}

void UAC_Status::InflictParalysis() {
	CurrentStatus = Paralyze;
}

void UAC_Status::InflictSleep() {
	CurrentStatus = Sleep;
}

void UAC_Status::InflictBlind() {
	CurrentStatus = Blind;
}


//Let this receive the ailment type and chance of infliction
//Then use players own properties to determine if it lands or not
void UAC_Status::AttemptInflictAilment(FStatus Ailment, int BaseChance) {

	if (FMath::RandRange(0, 100) < BaseChance) {

		CurrentStatus = Ailment;


		AilmentRecoveryChance = BaseAilmentRecoveryChance;
	}


}

//Set stored target to either an enemy or ally, then do a normal attack
void UAC_Status::Confusion() {
	int ConfuseChance = FMath::RandRange(0, 100);
	
	GetCharacter()->SetQueuedAction(NormalAttack);

	//Attack ally
	if (ConfuseChance < 50) {
		GetCharacter()->SetStoredTarget(Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()))->GetAllyToTarget());
	}
	else { //Attack Enemy
		GetCharacter()->SetStoredTarget(Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()))->GetNewEnemyTarget());
	}


}


bool UAC_Status::Paralyzed() {
	int ParalyzeChance = FMath::RandRange(0, 99);

	if (ParalyzeChance <= 49) {
		return false;
	}
	else {
		return true;
	}
}

bool UAC_Status::Stun() {
	return true;
}

void UAC_Status::AttemptRecoverAilment() {
	if (FMath::RandRange(0, 100) < AilmentRecoveryChance) {
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Green, "Recovered from Ailment over time!");
		CureAilment();
		return;
	}



	AilmentRecoveryChance += AilmentRecoveryRate;
}

void UAC_Status::CureAilment() {
	if (CurrentStatus == Confuse || CurrentStatus == Sleep) { //Cancel out the player action in case they get cured before acting
		GetCharacter()->SetQueuedAction(NoAction);
	}

	//Do some other stuff here like play a jingle or remove UI elements
	AilmentRecoveryChance = BaseAilmentRecoveryChance;
	SetCurrentStatus(Normal);
}

void UAC_Status::SetCurrentStatus(FStatus Status) {
	CurrentStatus = Status;
}

ARPGCharacter* UAC_Status::GetCharacter() {
	return Cast<ARPGCharacter>(GetOwner());



}

