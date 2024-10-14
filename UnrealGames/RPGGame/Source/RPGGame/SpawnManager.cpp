// Fill out your copyright notice in the Description page of Project Settings.


#include "SpawnManager.h"
#include "RPGGameGameMode.h"
#include "Kismet/GameplayStatics.h"
#include "RPGCharacter.h"

// Sets default values
ASpawnManager::ASpawnManager()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

}

// Called when the game starts or when spawned
void ASpawnManager::BeginPlay()
{
	Super::BeginPlay();
	
	//SpawnEnemies();
	//SpawnPlayers();
}




void ASpawnManager::SpawnEnemies() {
	UWorld* World = GetWorld();

	if (World) {
		ARPGGameGameMode* GameMode = Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()));

		for (int i = 0; i < 3; i++) {
			GameMode->SpawnEnemy(GetWorld()->SpawnActor<ARPGCharacter>(EnemyStorage, EnemySpawnPoints[i]->GetActorLocation(), EnemySpawnPoints[i]->GetActorRotation()), i);
		}
	}



}


void ASpawnManager::SpawnPlayers() {
	UWorld* World = GetWorld();

	if (World) {
		ARPGGameGameMode* GameMode = Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()));

		for (int i = 0; i < 4; i++) {
			GameMode->SpawnPlayer(GetWorld()->SpawnActor<ARPGCharacter>(PlayerStorage[i], PlayerSpawnPoints[i]->GetActorLocation(), PlayerSpawnPoints[i]->GetActorRotation()), i);
		}
	}




}
