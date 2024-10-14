// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Engine/DataAsset.h"
#include "EnumContainer.h"
#include "ItemDataAsset.generated.h"

/**
 * 
 */
UCLASS()
class RPGGAME_API UItemDataAsset : public UPrimaryDataAsset
{
	GENERATED_BODY()
	
public:

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	int ID;



	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	FName Name;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	FName Description;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TSoftObjectPtr<UTexture> Icon;


	UFUNCTION(BlueprintCallable)
	virtual void OnActivate(class ARPGCharacter* User);


};
