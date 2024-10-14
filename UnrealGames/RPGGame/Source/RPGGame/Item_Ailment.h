// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Item_ConsumableAsset.h"
#include "Item_Ailment.generated.h"

/**
 * 
 */
UCLASS()
class RPGGAME_API UItem_Ailment : public UItem_ConsumableAsset
{
	GENERATED_BODY()

public:


	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	TEnumAsByte<FStatus> Type;
	
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float InflictionChance;

	virtual void ItemEffect(class ARPGCharacter* Target) override;

};
