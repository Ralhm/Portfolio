// Fill out your copyright notice in the Description page of Project Settings.


#include "Skill_Buff.h"
#include "RPGCharacter.h"

void USkill_Buff::SkillEffect(ARPGCharacter* User, ARPGCharacter* Target) {
	Target->BuffComponent->ReceiveBuff(BuffData);

}
