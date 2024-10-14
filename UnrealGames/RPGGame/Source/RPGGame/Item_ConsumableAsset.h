// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "ItemDataAsset.h"
#include "Item_ConsumableAsset.generated.h"

/**
 * 
 */
UCLASS()
class RPGGAME_API UItem_ConsumableAsset : public UItemDataAsset
{
	GENERATED_BODY()
	
public:

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	int Amount;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	bool IsOffensive;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	bool IsMultiTarget;


	virtual void OnActivate(class ARPGCharacter* User) override;

	void UseItemOnMultiple(TArray<class ARPGCharacter*> Targets);

	virtual void ItemEffect(class ARPGCharacter* Target);
};
