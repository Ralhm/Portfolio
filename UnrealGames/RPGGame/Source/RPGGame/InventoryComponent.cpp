// Fill out your copyright notice in the Description page of Project Settings.


#include "InventoryComponent.h"
#include "Item_ConsumableAsset.h"



// Sets default values for this component's properties
UInventoryComponent::UInventoryComponent()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	// ...
}


//looping through the array probably isn't that big of a deal, since most of the time you'll likely be adding my reference instead, through designated item drops or shops
void UInventoryComponent::AddConsumableByID(int ID) {
	for (int i = 0; i < ConsumableContainer.Num(); i++) {
		if (ID == ConsumableContainer[i]->ID) {
			Consumables.Add(ConsumableContainer[i]);
			return;
		}
	}


	
	
}

void UInventoryComponent::RemoveConsumableByID(int ID) {
	for (int i = 0; i < ConsumableContainer.Num(); i++) {
		if (ID == ConsumableContainer[i]->ID) {
			Consumables.RemoveSingle(ConsumableContainer[i]);
			return;
		}
	}
}

void UInventoryComponent::AddConsumableByReference(UItem_ConsumableAsset* ref) {
	Consumables.Add(ref);
	Consumables.Sort([](const UItem_ConsumableAsset& a, const UItem_ConsumableAsset& b) { return a.ID < b.ID; });
}


void UInventoryComponent::RemoveConsumableByReference(UItem_ConsumableAsset* ref) {
	Consumables.RemoveSingle(ref);
}

void UInventoryComponent::OrganizeItems() {
	//Consumables.Sort();
}

void UInventoryComponent::AddConsumableToContainer(UItem_ConsumableAsset* ref) {
	ConsumableContainer.Add(ref);

}

UItem_ConsumableAsset* UInventoryComponent::GetConsumableByID(int ID) {
	for (int i = 0; i < ConsumableContainer.Num(); i++) {
		if (ID == ConsumableContainer[i]->ID) {
			return ConsumableContainer[i];
		}
	}

	GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Red, "FAILED TO FIND ITEM RETURNING DEFAULT ITEM!");


	return ConsumableContainer[0];
}