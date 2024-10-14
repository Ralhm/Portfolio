// Fill out your copyright notice in the Description page of Project Settings.


#include "Item_ConsumableAsset.h"
#include "RPGCharacter.h"

void UItem_ConsumableAsset::OnActivate(ARPGCharacter* User) {
	if (IsMultiTarget) {
		UseItemOnMultiple(*User->StoredTargetMultiple);
	}
	else {
		ItemEffect(User->StoredTarget);
	}
}

void UItem_ConsumableAsset::UseItemOnMultiple(TArray<ARPGCharacter*> Targets) {
	for (int i = 0; i < Targets.Num(); i++) {
		ItemEffect(Targets[i]);
	}
}

void UItem_ConsumableAsset::ItemEffect(ARPGCharacter* Target) {

}