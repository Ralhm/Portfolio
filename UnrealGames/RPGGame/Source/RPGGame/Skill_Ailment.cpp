// Fill out your copyright notice in the Description page of Project Settings.


#include "Skill_Ailment.h"

#include "RPGCharacter.h"

void USkill_Ailment::SkillEffect(ARPGCharacter* User, ARPGCharacter* Target) {
	if (DoesDamage) {
		Target->TakeDamage((CalculateDamage(Target, User)));
	}


	Target->AttemptInflictAilment(Type, InflictionChance);

}
