// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "InventoryComponent.generated.h"




UCLASS(Blueprintable,  ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class RPGGAME_API UInventoryComponent : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UInventoryComponent();

protected:


public:	



	UFUNCTION()
	void AddConsumableByID(int ID);

	UFUNCTION()
	void RemoveConsumableByID(int ID);

	UFUNCTION()
	void AddConsumableByReference(class UItem_ConsumableAsset* ref);

	UFUNCTION()
	void RemoveConsumableByReference(class UItem_ConsumableAsset* ref);

	UFUNCTION()
	class UItem_ConsumableAsset* GetConsumableByID(int ID);

	UFUNCTION()
	void OrganizeItems();

	UFUNCTION()
	void AddConsumableToContainer(class UItem_ConsumableAsset* ref);


	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	int MaxInventorySize;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	int CurrentInventorySpace;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	TArray <class UItem_ConsumableAsset*> Consumables;




	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TArray<class UItem_ConsumableAsset*> ConsumableContainer;


		
};
