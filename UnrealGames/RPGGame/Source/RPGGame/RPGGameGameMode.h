// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/GameModeBase.h"
#include "RPGGameGameMode.generated.h"

DECLARE_DYNAMIC_MULTICAST_DELEGATE(FEndTurn);

UENUM(BlueprintType)
enum EBattleStatus {
	None UMETA(DisplayName = "None"),
	Player_Select UMETA(DisplayName = "Player_Select"),
	Enemy_Select UMETA(DisplayName = "Enemy_Select"),
	Action_Select UMETA(DisplayName = "Action_Select"),
	Skill_Select UMETA(DisplayName = "Skill_Select"),
	Item_Select UMETA(DisplayName = "Item_Select"),
	Execute_Turn UMETA(DisplayName = "Execute_Turn"),
	Enemy_Select_Multi UMETA(DisplayName = "Enemy_Select_Multi"),
	Player_Select_Multi UMETA(DisplayName = "Player_Select_Multi"),
	Confirmation UMETA(DisplayName = "Confirmation")



};


UCLASS(minimalapi)
class ARPGGameGameMode : public AGameModeBase
{
	GENERATED_BODY()

public:
	ARPGGameGameMode();


public:

	virtual void BeginPlay() override;


	//The widget class we will use as our menu when the game starts
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	class TSubclassOf<UUserWidget> StartingWidgetClass;

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
	class UInventoryComponent* InventoryComponent;


	//The widget instance we are using as our game menu
	UPROPERTY()
	class UUserWidget* CurrentWidget;

	UPROPERTY()
	class UUserWidget* UIDisplay;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	class TSubclassOf<UUserWidget> ListWidget;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	class TSubclassOf<UUserWidget> UIWidget;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	class TSubclassOf<UUserWidget> ConfirmationWidget;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	class TSubclassOf<UUserWidget> ItemButton;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	class TSubclassOf<UUserWidget> SkillButton;



	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
	TArray<class ARPGCharacter*> Players;

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
	TArray<class ARPGCharacter*> Enemies;

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
	TArray<class ARPGCharacter*> AllEntities;

	UPROPERTY(BlueprintReadWrite)
	class AViewPawn* PawnController;



	//The widget instance we are using as our game menu
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	TSubclassOf<AActor> SpawnManagerStorage;

	//The widget instance we are using as our game menu
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	TSubclassOf<AActor> PawnControllerStorage;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	TEnumAsByte<EBattleStatus> State = Action_Select;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	TEnumAsByte<EBattleStatus> PreviousState;

	UPROPERTY()
	AActor* SpawnManager;


	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	int TargetSum;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	int PlayerIndex;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	int PlayerSelectIndex;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	int EnemyIndex;

	void AdjustEnemyIndex(int val);



	void IncrementPlayerActionIndex();
	void DecrementPlayerActionIndex();

	void AdjustPlayerSelectIndex(int val);

	void AdjustListIndex(int val);

	UFUNCTION(BlueprintCallable)
	void ChangeMenuWidget(TSubclassOf<UUserWidget> NewWidgetClass);

	UFUNCTION(BlueprintCallable)
	void DisplaySkills();

	UFUNCTION(BlueprintCallable)
	void DisplayItems();

	UFUNCTION(BlueprintCallable)
	void DisplayEnemies();

	UFUNCTION(BlueprintCallable)
	void DisplayEnemiesSingleTarget();

	UFUNCTION(BlueprintCallable)
	void DisplayEnemiesMultiTarget();

	UFUNCTION(BlueprintCallable)
	void DisplayFriendly();

	UFUNCTION(BlueprintCallable)
	void DisplayFriendlySingleTarget();

	UFUNCTION(BlueprintCallable)
	void DisplayFriendlyMultiTarget();

	UFUNCTION(BlueprintCallable)
	void UndoMenu();

	UFUNCTION(BlueprintCallable)
	void ConfirmAction();

	UFUNCTION(BlueprintCallable)
	void RemoveTargetArrows();

	UFUNCTION(BlueprintCallable)
	void RemoveEnemyTargetArrows();

	UFUNCTION(BlueprintCallable)
	void RemoveAllyTargetArrows();

	UFUNCTION()
	void CalculateDrawRate();

	UFUNCTION()
	class ARPGCharacter* GetAllyToTarget();




	UFUNCTION()
	class ARPGCharacter* GetNewEnemyTarget();

	class ARPGCharacter* GetNewAllyTarget();

	UFUNCTION(BlueprintCallable)
	void DetermineTurnOrder();

	UFUNCTION(BlueprintCallable)
	void DetermineEnemyActions();

	int Partition(TArray<class ARPGCharacter*>& Entities, int Low, int High);

	UFUNCTION(BlueprintCallable)
	void QuickSort(TArray<class ARPGCharacter*>& Entities, int Low, int High);

	UFUNCTION()
	void SpawnPlayer(class ARPGCharacter* PlayerRef, int index);

	UFUNCTION()
	void SpawnEnemy(class ARPGCharacter* EnemyRef, int index);

	UFUNCTION()
	void RemoveEnemy(class ARPGCharacter* EnemyRef);


	UFUNCTION()
	void EnemyTurn();


	UFUNCTION(BlueprintCallable)
	void ExecuteTurn();

	UFUNCTION()
	void StartNextTurn();


	UFUNCTION()
	void AskConfirmTurn();


	UFUNCTION(BlueprintCallable)
	FString GetSkillName(int index);

	UFUNCTION(BlueprintCallable)
	FString GetSkillDescription(int index);

	UFUNCTION(BlueprintCallable)
	FString GetItemName(int index);

	UFUNCTION(BlueprintCallable)
	FString GetItemDescription(int index);

	UFUNCTION(BlueprintCallable)
	void SetHoveredSkillDescription(int index);

	UFUNCTION(BlueprintCallable)
	void SetHoveredItemDescription(int index);

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
	FString HoveredDescription;

	UFUNCTION(BlueprintCallable)
	void SetQueuedAction(int type);

	UFUNCTION(BlueprintCallable)
	void SetQueuedActionAttack();

	UFUNCTION(BlueprintCallable)
	void SetQueuedActionSkill();

	UFUNCTION(BlueprintCallable)
	void SetQueuedActionItem();

	UFUNCTION(BlueprintCallable)
	void SetQueuedActionDefend();

	UFUNCTION(BlueprintCallable)
	void SetQueuedSkill(int index);

	UFUNCTION(BlueprintCallable)
	void SetQueuedItem(int index);


	FORCEINLINE void RemoveConsumable(class UItem_ConsumableAsset* ItemRef);
	FORCEINLINE void AddConsumable(class UItem_ConsumableAsset* ItemRef);

	FORCEINLINE void RemoveConsumableByID(int ID);
	FORCEINLINE void AddConsumableByID(int ID);

	FTimerHandle ActionDelayHandle;

	FTimerHandle ActionAnimHandle;

	UPROPERTY(EditAnywhere)
	FEndTurn EndOfTurnDelegate;


};



