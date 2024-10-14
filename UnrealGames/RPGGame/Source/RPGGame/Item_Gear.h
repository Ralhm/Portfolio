// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "ItemDataAsset.h"
#include "Item_Gear.generated.h"

/**
 * 
 */
UCLASS()
class RPGGAME_API UItem_Gear : public UItemDataAsset
{
	GENERATED_BODY()
	
public:


	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	TEnumAsByte<EEquipType> EquipType;

};
