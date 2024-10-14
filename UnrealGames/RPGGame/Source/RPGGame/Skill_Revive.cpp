// Fill out your copyright notice in the Description page of Project Settings.


#include "Skill_Revive.h"
#include "RPGCharacter.h"
#include "RPGGameGameMode.h"
#include "Kismet/GameplayStatics.h"




void USkill_Revive::SkillEffect(ARPGCharacter* User, ARPGCharacter* Target) {
	Revive(User, Target);
}
