// Fill out your copyright notice in the Description page of Project Settings.


#include "Item_Restorative.h"
#include "RPGCharacter.h"



void UItem_Restorative::ItemEffect(ARPGCharacter* Target) {
	switch (Type)
	{
	case Item_Health:
		Target->RestoreHealth(Amount);
		break;
	case Item_Mana:
		Target->RestoreSP(Amount);
		break;
	case Item_Revival:
		Target->Revive(Amount);
		break;
	default:
		break;
	}

	switch (SubType)
	{
	case Item_Health:
		Target->RestoreHealth(SubAmount);
		break;
	case Item_Mana:
		Target->RestoreSP(SubAmount);
		break;
	case Item_Revival:
		Target->Revive(SubAmount);
		break;
	default:
		break;
	}
}