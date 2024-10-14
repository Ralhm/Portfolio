// Fill out your copyright notice in the Description page of Project Settings.


#include "Item_Buff.h"
#include "RPGCharacter.h"


void UItem_Buff::ItemEffect(ARPGCharacter* Target) {
	Target->BuffComponent->ReceiveBuff(BuffData);
}