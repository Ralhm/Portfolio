// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Item_ConsumableAsset.h"
#include "Item_Restorative.generated.h"

/**
 * 
 */
UCLASS()
class RPGGAME_API UItem_Restorative : public UItem_ConsumableAsset
{
	GENERATED_BODY()

public:

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	TEnumAsByte<ERestorativeType> Type;


	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	int SubAmount;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	TEnumAsByte<ERestorativeType> SubType;



	virtual void ItemEffect(class ARPGCharacter* Target) override;
	
};
