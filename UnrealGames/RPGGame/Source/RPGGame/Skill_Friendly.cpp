// Fill out your copyright notice in the Description page of Project Settings.


#include "Skill_Friendly.h"
#include "RPGCharacter.h"
#include "RPGGameGameMode.h"
#include "Kismet/GameplayStatics.h"


void USkill_Friendly::SkillEffect(ARPGCharacter* User, ARPGCharacter* Target) {
	HealTarget(User, Target);
}
