// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "SpawnManager.generated.h"

UCLASS()
class RPGGAME_API ASpawnManager : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	ASpawnManager();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	


	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TArray<AActor*> PlayerSpawnPoints;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TArray<AActor*> EnemySpawnPoints;


	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TArray<TSubclassOf<AActor>> PlayerStorage;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TSubclassOf<AActor> EnemyStorage;


	UFUNCTION()
	void SpawnEnemies();

	UFUNCTION()
	void SpawnPlayers();

};
