// Fill out your copyright notice in the Description page of Project Settings.


#include "Item_Ailment.h"
#include "RPGCharacter.h"



void UItem_Ailment::ItemEffect(ARPGCharacter* Target) {
	Target->AttemptInflictAilment(Type, InflictionChance);
}