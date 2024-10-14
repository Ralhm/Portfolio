// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Item_ConsumableAsset.h"
#include "AC_Buff.h"
#include "Item_Buff.generated.h"

/**
 * 
 */
UCLASS()
class RPGGAME_API UItem_Buff : public UItem_ConsumableAsset
{
	GENERATED_BODY()
	
public:

	virtual void ItemEffect(class ARPGCharacter* Target) override;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FBuffData BuffData;
};
