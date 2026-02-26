// Fill out your copyright notice in the Description page of Project Settings.


#include "Skill.h"
#include "RPGCharacter.h"
#include "RPGGameGameMode.h"
#include "Kismet/GameplayStatics.h"

//player stores current target
//player/skill component is responsible for calculating damage to target
//casts target to enemy, gets weaknesses and resistances, uses their queued skills properties and own stats to determine damage
//this way the target does not need a reference to the skill and user attacking them.
//if multitarget skill, get array of enemies/allies and execute for each of them




void USkill::OnActivate(ARPGCharacter* User) {
	//Cycle through list of enemies in game mode, deal damage to them
	if (IsMultiTarget) {
		UseSkillOnMultiple(User, *User->StoredTargetMultiple);

	}
	else { //Do single target attack on stored enemy
		SkillEffect(User, User->StoredTarget);
	}
	
}

void USkill::UseSkillOnMultiple(ARPGCharacter* User, TArray<ARPGCharacter*> Targets) {
	for (int i = 0; i < Targets.Num(); i++) {
		SkillEffect(User, Targets[i]);
	}
}


void USkill::SkillEffect(ARPGCharacter* User, ARPGCharacter* Target) {
	AttemptDamageTarget(User, Target);
}

void USkill::AttemptDamageTarget(ARPGCharacter* User, ARPGCharacter* Target) {
	if (!Target->AttemptDodge(User->CalculateAccuracy(Accuracy))) {
		Target->TakeDamage((CalculateDamage(Target, User)));
	}
}

void USkill::UseResource(ARPGCharacter* User) {
	if (ResourceType == HealthPoints) {
		User->LoseHealth(Cost);
	}
	else {
		User->LoseSP(Cost);
	}
}

void USkill::HealTarget(ARPGCharacter* User, ARPGCharacter* Target) {
	Target->RestoreHealth(User->MagicPower * SkillPower);

}

void USkill::Revive(ARPGCharacter* User, ARPGCharacter* Target) {
	//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Green, FString::Printf(TEXT("ACTIVATING REVIVIAL SKILL")));
	//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Green, Target->GetName());
	Target->Revive(User->MagicPower * SkillPower);
}





void USkill::OnBegin() {

}

//broadcast message to game mode or whatever to do whatever should be done next
void USkill::OnEnd() {

}



//if the skill calculates damage, then you won't need if statements for what type it is
//if the user/target calculates damage, they'll need to check what type and decide to draw on physical or magical power/defense


//The skill should have full control over what it does and how it calculates damage. these should NOT be passed into the character to do for it
//otherwise we risk having to do more specific shit for the character
//this way we can modify damage calculations and outputs for each skill if necessary

//if we calculate damage output for skills and have the enmy receive the damage, then calculate against it's own values, you'll have to pass in skill properties and such
float USkill::CalculateDamage(ARPGCharacter* Target, ARPGCharacter* User) const {
	float Damage = User->CalculateDealtDamage(SkillType, SkillPower);

	Damage = Target->CalculateTakenDamage(Damage, SkillType, SkillProperty);
	Damage *= Target->GetResistanceMultiplier(SubProperty);

	GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Yellow, FString::Printf(TEXT("Damage Output: %f"), Damage));


	return Damage;
}