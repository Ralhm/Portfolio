// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Item_ConsumableAsset.h"
#include "Item_Offensive.generated.h"

/**
 * 
 */
UCLASS()
class RPGGAME_API UItem_Offensive : public UItem_ConsumableAsset
{
	GENERATED_BODY()
	
public:

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	TEnumAsByte<FType> Type;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	TEnumAsByte<FAttribute> Property;


	virtual void ItemEffect(class ARPGCharacter* Target) override;

};
