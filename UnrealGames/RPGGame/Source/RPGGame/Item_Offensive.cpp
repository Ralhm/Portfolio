// Fill out your copyright notice in the Description page of Project Settings.


#include "Item_Offensive.h"
#include "RPGCharacter.h"



void UItem_Offensive::ItemEffect(ARPGCharacter* Target) {
	Target->TakeDamage(Target->CalculateTakenDamage(Amount, Type, Property));
}