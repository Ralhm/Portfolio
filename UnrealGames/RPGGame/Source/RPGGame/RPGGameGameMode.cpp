// Copyright Epic Games, Inc. All Rights Reserved.

#include "RPGGameGameMode.h"
#include "RPGGamePlayerController.h"
#include "RPGGameCharacter.h"

#include "GameFramework/GameModeBase.h"

#include "InventoryComponent.h"
#include "Item_ConsumableAsset.h"


#include "Blueprint/UserWidget.h"
#include "Blueprint/WidgetBlueprintLibrary.h"

#include "Components/Widget.h"
#include "Components/WidgetComponent.h"



#include "Kismet/GameplayStatics.h"
#include "Kismet/KismetMathLibrary.h"


#include "RPGAlly.h"
#include "RPGCharacter.h"
#include "RPGEnemy.h"

#include "Skill.h"
#include "SpawnManager.h"

#include "ViewPawn.h"

#include "UObject/ConstructorHelpers.h"


// --Dynamic Camera that moves around, zooms in on current player
// -During the turn phase, cut to overhead, lerp to character front/back, activate their action, then do a certain dynamic camera move like lerping out or switching angles
// -Camera zooms in/out depending on center point of all entities
// -Camera sets its midpoint to better circle around character when shifting towards front
// 
// When re-doing character textures, be sure to unwrap all the parts onto one or two material/texture
// Shrink the faces so they can fit into the engine
// 
//
// <Wavy>Wavy test</> <Shake>Shaking Test</>
// --Dialogue, characters show up and speak during dialogue like in Persona Q
// -Make talking animations
// let SpeakerName indicate commands for special events? (exit, cutscenes, CGs, etc)
// Change slide out to fade out on exit?
// Consider turning the RenderActors emote array into a map, and then simply passing in an enum as a key to get the desired montage
// 
// 
// --If you want multiple actions per character, create a queue of actions rather than queue of characters
// Have the queued actions store references to the user and targets
// 
// ----New Status Effects:
// --Shocked 
// -Like Paralysis, but can spread to others and grants electric element to physical attacks
// --Burning
// -Like Poison, but grants fire element to physical attacks
// 
// ----Implement face changing in animations
// ---Status Indicator
// ---Mouth talking
// ---Enemy Skills and decision making
// --Adjust EndOfActions to not rely entirely on animations ending, give a little bit more breathing room in-between actions
// --Gear System
// -Add more, custom animations
// -Unique Dodge animations
// Fine tune status effects
// Fine Tune evasion and accuracy
// Fine tune lots of other stuff
// Replace for loops and other reference calls and such with delegate calls
// 
// 
// 
// 
// Consider Moving nextaction from end of character animation to end of damage/some other effect
// Put in some timer functions or whatnot to end of turn to handle things like poison or regen effects
// 
// Consider moving inventory component from gamemode to some other instance, so it can be gotten from the battle and overworld game modes and other stuff
// 
// Consider doing a blend state for health state
// 
// Don't worry about all the items being stored in the inventory container for now, they're just there for quick referencing
// We're not going to store asset references to each and every item in one array, not happening
// instead the asset references will be made where necessary, like item shops chests, or enemy drops
// In other words, every enemy will have an array of items they can drop, shops will have arrays of items they sell, etc.
// Keep damage events on the animation, if something is multi/random hit then just tie the hits to the animation
// 
// 
// Consider changing death montage to anim blueprint state
// ^NO DON'T DO THAT^ because then you'll have to rig up the anim bp states for each and every character
// 
// 


ARPGGameGameMode::ARPGGameGameMode()
{
	InventoryComponent = CreateDefaultSubobject<UInventoryComponent>(TEXT("InventoryComponent"));
}

void ARPGGameGameMode::BeginPlay() {
	State = Action_Select;
	//Players.SetNum(4);
	//Enemies.SetNum(3);
	//AllEntities.SetNum(7);




	SpawnManager = UGameplayStatics::GetActorOfClass(GetWorld(), SpawnManagerStorage);
	PawnController = Cast<AViewPawn>(UGameplayStatics::GetActorOfClass(GetWorld(), PawnControllerStorage));

	Cast<ASpawnManager>(SpawnManager)->SpawnPlayers();
	Cast<ASpawnManager>(SpawnManager)->SpawnEnemies();
	CalculateDrawRate();

	ChangeMenuWidget(StartingWidgetClass);

	UIDisplay = CreateWidget<UUserWidget>(GetWorld(), UIWidget);
	UIDisplay->AddToViewport();

	Players[PlayerIndex]->DisplayTargetArrow();


	PawnController->MoveToCharacter(Players[PlayerIndex], FrontSide);

	AddConsumableByID(0);
	AddConsumableByID(0);
	AddConsumableByID(1);
	AddConsumableByID(1);
	AddConsumableByID(2);
	AddConsumableByID(2);
	AddConsumableByID(3);
	AddConsumableByID(3);
	AddConsumableByID(4);
	AddConsumableByID(4);
	AddConsumableByID(5);
	AddConsumableByID(5);
	AddConsumableByID(6);
	AddConsumableByID(6);
	AddConsumableByID(7);
	AddConsumableByID(7);
	AddConsumableByID(7);
	AddConsumableByID(8);
	AddConsumableByID(8);
	AddConsumableByID(9);
	AddConsumableByID(9);


}



void ARPGGameGameMode::SetArrowsLookAtCamera() {
	//GEngine->AddOnScreenDebugMessage(-1, 0.1f, FColor::Red, "Looking at targets!");
	for (int i = 0; i < AllEntities.Num(); i++) {
		AllEntities[i]->SelectArrow->SetWorldRotation(UKismetMathLibrary::MakeRotFromX(PawnController->GetActorLocation() - AllEntities[i]->GetActorLocation()));
	}
}

void ARPGGameGameMode::CalculateDrawRate() {
	TargetSum = 0;
	for (int i = 0; i < Players.Num(); i++) {
		Cast<ARPGAlly>(Players[i])->SetDrawRate(TargetSum, TargetSum + Cast<ARPGAlly>(Players[i])->CurrentDrawRate - 1);
		TargetSum += Cast<ARPGAlly>(Players[i])->CurrentDrawRate;
	}
}

ARPGCharacter* ARPGGameGameMode::GetAllyToTarget() {
	int Target = FMath::RandRange(0, TargetSum - 1);
	for (int i = 0; i < Players.Num(); i++) {
		if ((Target >= Cast<ARPGAlly>(Players[i])->DrawRateMin) && (Target <= Cast<ARPGAlly>(Players[i])->DrawRateMax)) {
			return Players[i];
		}
	}

	GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Red, "FAILED TO FIND A PLAYER TO TARGET!");
	return Players[0];
}

ARPGCharacter* ARPGGameGameMode::GetNewEnemyTarget() {
	return Enemies[FMath::RandRange(0, Enemies.Num() - 1)];
}

ARPGCharacter* ARPGGameGameMode::GetNewAllyTarget() {
	return Players[FMath::RandRange(0, Players.Num() - 1)];
}

void ARPGGameGameMode::SpawnPlayer(ARPGCharacter* PlayerRef, int index) {
	PlayerRef->index = index;
	Players.Add(PlayerRef);
	AllEntities.Add(PlayerRef);

	Players[index]->SelectArrow->SetWorldRotation(UKismetMathLibrary::MakeRotFromX(PawnController->GetActorLocation() - Players[index]->GetActorLocation()));
}


void ARPGGameGameMode::SpawnEnemy(ARPGCharacter* EnemyRef, int index) {
	EnemyRef->index = index;
	Enemies.Add(EnemyRef);
	AllEntities.Add(EnemyRef);

	Enemies[index]->SelectArrow->SetWorldRotation(UKismetMathLibrary::MakeRotFromX(PawnController->GetActorLocation() - Enemies[index]->GetActorLocation()));
}

void ARPGGameGameMode::RemoveEnemy(ARPGCharacter* EnemyRef) {
	//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Magenta, "Removing Enemy from array!");
	//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Magenta, EnemyRef->GetName());
	Enemies.RemoveSingle(EnemyRef);
	AllEntities.RemoveSingle(EnemyRef);


}

void ARPGGameGameMode::EnemyTurn() {

}

//Very beginning of the turn
void ARPGGameGameMode::ExecuteTurn() {
	DetermineTurnOrder();

	CurrentWidget->RemoveFromViewport();
	State = Execute_Turn;

	NextCharacterToAct = AllEntities[0];

	PawnController->SwoopInFromOverhead(NextCharacterToAct);



}

void ARPGGameGameMode::TellCameraWhatToDo(ARPGCharacter* Character, ECamMovementType Type, bool Snap, bool ZoomOut, bool Delay) {
	if (Delay) {
		PawnController->MoveToCharacterDelay(Character, Type, Snap, ZoomOut);
	}
	else {
		PawnController->MoveToCharacter(Character, Type, Snap, ZoomOut);
	}
	
}

void ARPGGameGameMode::TriggerEndOfCameraMovement() {
	if (State == Execute_Turn) { //If camera movement ends during the action
		GetWorldTimerManager().SetTimer(ActionDelayHandle, this, &ARPGGameGameMode::BeginNextCharacterAction, PawnController->CharacterActionDelayTime);
		//BeginNextCharacterAction();
		return;
	}
	else if (State == CharacterActing) {
		GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Red, "Ending CameraMovement in CHARACTER ACTING!");
	}
	
	//if camera is zooming out to show the field for target selection
	//Right now do nothing, but maybe we want to prevent actions while the camera is still moving
	
}

// --During the turn phase, cut to overhead of acting character, lerp to character front/back, activate their action, then do a certain dynamic camera move like lerping out or switching angles

void ARPGGameGameMode::BeginNextCharacterAction() {
	GetWorldTimerManager().ClearTimer(ActionDelayHandle);
	State = CharacterActing;
	NextCharacterToAct->BeginAction();


}



void ARPGGameGameMode::SetNextCharacterToAct(ARPGCharacter* NextCharacter) {
	if (NextCharacter == nullptr) {
		NextTurnDelay();
		return;
	}

	if (NextCharacter->QueuedAction == NoAction) {
		NextCharacterToAct = NextCharacter;

		while (NextCharacterToAct->QueuedAction == NoAction) {
			NextCharacterToAct = NextCharacterToAct->NextCharacterToAct;
			if (NextCharacterToAct == nullptr) {
				NextTurnDelay();
				return;
			}
			

		}

	}
	else {
		NextCharacterToAct = NextCharacter;
	}

	//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Orange, "Next Character to act is:!");


	//Need to still do a delay if character died at very end of turn
	State = Execute_Turn;
	GetWorldTimerManager().SetTimer(ActionDelayHandle, this, &ARPGGameGameMode::DelayTilNextAction, PawnController->NextActionDelayTime + DeathDelayTime);
	if (DeathDelayTime > 0) {
		DeathDelayTime = 0;
	}



	//PawnController->SwoopInFromOverhead(NextCharacterToAct);
}

void ARPGGameGameMode::SetDeathDelayTime(float duration) {
	DeathDelayTime = duration;
}

//Minor delay in-between end of action and start of next action
void ARPGGameGameMode::DelayTilNextAction() {
	PawnController->SwoopInFromOverhead(NextCharacterToAct);
	
}

void ARPGGameGameMode::DetermineEnemyActions() {
	for (int i = 0; i < Enemies.Num(); i++) {
		Cast<ARPGEnemy>(Enemies[i])->ChooseAction();
	}
}

void ARPGGameGameMode::DetermineTurnOrder() {
	DetermineEnemyActions();

	for (int i = 0; i < AllEntities.Num(); i++) {
		AllEntities[i]->CalculateActionSpeed();
	}
	QuickSort(AllEntities, 0, AllEntities.Num() - 1);


	for (int i = 0; i < AllEntities.Num() - 1; i++) {
		AllEntities[i]->SetNextCharacterActor(AllEntities[i + 1]);
		AllEntities[i]->TurnIndex = i;
	}


}


void ARPGGameGameMode::NextTurnDelay() {
	GetWorldTimerManager().SetTimer(ActionDelayHandle, this, &ARPGGameGameMode::StartNextTurn, PawnController->NextActionDelayTime + DeathDelayTime);
	if (DeathDelayTime > 0) {
		DeathDelayTime = 0;
	}

}

void ARPGGameGameMode::StartNextTurn() {
	GetWorldTimerManager().ClearTimer(ActionDelayHandle);
	PlayerIndex = -1;

	IncrementPlayerActionIndex();

	State = Action_Select;
	ChangeMenuWidget(StartingWidgetClass);
	Players[PlayerIndex]->DisplayTargetArrow();

	EndOfTurnDelegate.Broadcast();



}



FString ARPGGameGameMode::GetSkillName(int index) {
	//HoveredDescription = Players[PlayerIndex]->SkillComponent->Skills[index]->SkillName;
	return Players[PlayerIndex]->SkillComponent->Skills[index]->SkillName.ToString();
}


FString ARPGGameGameMode::GetSkillDescription(int index) {
	//HoveredDescription = Players[PlayerIndex]->SkillComponent->Skills[index]->SkillDescription.ToString();
	return Players[PlayerIndex]->SkillComponent->Skills[index]->SkillDescription.ToString();
}

FString ARPGGameGameMode::GetItemName(int index) {
	return InventoryComponent->Consumables[index]->Name.ToString();
}


FString ARPGGameGameMode::GetItemDescription(int index) {
	//HoveredDescription = InventoryComponent->Consumables[index].Description.ToString();
	return InventoryComponent->Consumables[index]->Description.ToString();
}

void ARPGGameGameMode::SetHoveredSkillDescription(int index) {
	HoveredDescription = Players[PlayerIndex]->SkillComponent->Skills[index]->SkillDescription.ToString();

}

void ARPGGameGameMode::SetHoveredItemDescription(int index) {
	HoveredDescription = InventoryComponent->Consumables[index]->Description.ToString();;

}

void ARPGGameGameMode::SetQueuedAction(int type) {
	FActionType AType = (FActionType)type;


	Players[PlayerIndex]->SetQueuedAction(AType);
	if (AType == NormalAttack) {
		SetQueuedActionAttack();
	}
	else if (AType == UseSkill) {
		SetQueuedActionSkill();

	}
	else if (AType == UseItem) {
		SetQueuedActionItem();
	}
	else if (AType == Defend) {
		SetQueuedActionDefend();
	}


}


void ARPGGameGameMode::SetQueuedActionDefend() {
	PreviousState = Action_Select;
	ConfirmAction();
}



void ARPGGameGameMode::SetQueuedActionAttack() {
	DisplayEnemiesSingleTarget();
	PreviousState = Action_Select;
}


void ARPGGameGameMode::SetQueuedActionSkill() {
	State = Skill_Select;
	PreviousState = Action_Select;
	ChangeMenuWidget(ListWidget);

}

void ARPGGameGameMode::SetQueuedActionItem() {
	PreviousState = Action_Select;
	State = Item_Select;
	ChangeMenuWidget(ListWidget);

}

void ARPGGameGameMode::DisplayTargets(bool IsMultiTarget, bool IsOffensive) {
	if (IsOffensive) {
		if (IsMultiTarget) {
			DisplayEnemiesMultiTarget();
		}
		else {
			DisplayEnemiesSingleTarget();
		}
	}
	else {
		if (IsMultiTarget) {
			DisplayFriendlyMultiTarget();
		}
		else {
			DisplayFriendlySingleTarget();
		}
	}
}

void ARPGGameGameMode::SetQueuedItem(int index) {
	PreviousState = Item_Select;
	Players[PlayerIndex]->SetQueuedItem(InventoryComponent->Consumables[index]);
	//InventoryComponent->RemoveItemFromInventory(InventoryComponent->Consumables[index]);
	
	DisplayTargets(Players[PlayerIndex]->QueuedItem->IsMultiTarget, Players[PlayerIndex]->QueuedItem->IsOffensive);
	//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Orange, "Queueing Up item!");


}




void ARPGGameGameMode::SetQueuedSkill(int index) {
	if (Players[PlayerIndex]->MeetsRequirement(index)) {
		Players[PlayerIndex]->SetQueuedSkill(index);
		PreviousState = Skill_Select;

		if (Players[PlayerIndex]->SkillComponent->QueuedSkill->TargetType == Target_Self) {
			Players[PlayerIndex]->SetStoredTarget(Players[PlayerIndex]);
			IncrementPlayerActionIndex();

			return;
		}


		DisplayTargets(Players[PlayerIndex]->SkillComponent->QueuedSkill->IsMultiTarget, !(bool)Players[PlayerIndex]->SkillComponent->QueuedSkill->TargetType);
		//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Orange, "Queueing Up Skill!");

	}



}

void ARPGGameGameMode::ChangeMenuWidget(TSubclassOf<UUserWidget> NewWidgetClass) {
	if (CurrentWidget != nullptr) {
		CurrentWidget->RemoveFromViewport();
		CurrentWidget = nullptr;
	}

	if (NewWidgetClass != nullptr) {
		CurrentWidget = CreateWidget<UUserWidget>(GetWorld(), NewWidgetClass);
		if (CurrentWidget != nullptr) {
			CurrentWidget->AddToViewport();
		}
	}
}


void ARPGGameGameMode::DisplaySkills() {
	//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Purple, "Displaying Skills!!");
	State = Skill_Select;
	ChangeMenuWidget(ListWidget);
	
}

void ARPGGameGameMode::DisplayItems() {
	//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Purple, "Displaying Items!!");
	ChangeMenuWidget(ListWidget);
	State = Item_Select;
}

void ARPGGameGameMode::AskConfirmTurn() {
	ChangeMenuWidget(ConfirmationWidget);
	State = Confirmation;
}

void ARPGGameGameMode::DisplayEnemies() {





}

void ARPGGameGameMode::DisplayEnemiesSingleTarget() {
	EnemyIndex = 0;
	State = Enemy_Select;
	Enemies[EnemyIndex]->DisplayTargetArrow();
	CurrentWidget->RemoveFromViewport();

	MoveToRandomOverheadPoint();
}


void ARPGGameGameMode::DisplayEnemiesMultiTarget() {
	for (int i = 0; i < Enemies.Num(); i++) {
		State = Enemy_Select_Multi;
		Enemies[i]->DisplayTargetArrow();
	}
	CurrentWidget->RemoveFromViewport();

	MoveToRandomOverheadPoint();
}

void ARPGGameGameMode::DisplayFriendly() {


}

void ARPGGameGameMode::DisplayFriendlySingleTarget() {
	PlayerSelectIndex = 0;
	Players[PlayerIndex]->RemoveTargetArrow();

	State = Player_Select;
	Players[PlayerSelectIndex]->DisplayTargetArrow();

	MoveToRandomOverheadPoint();

	CurrentWidget->RemoveFromViewport();
}

void ARPGGameGameMode::DisplayFriendlyMultiTarget() {
	PlayerSelectIndex = 0;
	Players[PlayerIndex]->RemoveTargetArrow();

	for (int i = 0; i < Players.Num(); i++) {
		State = Player_Select_Multi;
		Players[i]->DisplayTargetArrow();
	}

	MoveToRandomOverheadPoint();

	CurrentWidget->RemoveFromViewport();
}


AActor* ARPGGameGameMode::GetRandomOverheadPoint() {
	return OverheadViewPoints[FMath::RandRange(0, OverheadViewPoints.Num() - 1)];
}

void ARPGGameGameMode::MoveToRandomOverheadPoint() {
	/*
	FVector CenterPoint = FVector(0, 0, 0);
	for (int i = 0; i < AllEntities.Num(); i++) {
		CenterPoint += AllEntities[i]->GetActorLocation();
	}


	CenterPoint /= AllEntities.Num();
	*/

	PawnController->MoveToFieldOverhead(false);
}


void ARPGGameGameMode::UndoMenu() {
	if (State == Action_Select) { //return to previous character
		if (PlayerIndex > 0) {
			DecrementPlayerActionIndex();
			return;
		}
	}
	else if (State == Item_Select || State == Skill_Select) {
		State = Action_Select;
		ChangeMenuWidget(StartingWidgetClass);
	}
	else if (State == Confirmation) {
		//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Purple, "Undoing confirmation!");
	
		PlayerIndex--;
		State = Action_Select;
		ChangeMenuWidget(StartingWidgetClass);
		Players[PlayerIndex]->DisplayTargetArrow();
		
	}
	else if (State == Enemy_Select || State == Enemy_Select_Multi || State == Player_Select || State == Player_Select_Multi) {
		
		if (PreviousState == Skill_Select) {
			//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Purple, "Undoing Skill Target Selection!!");
			RemoveTargetArrows();
			DisplaySkills();
		}
		else if (PreviousState == Item_Select) {
			//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Purple, "Undoing Item Target Selection!!");
			RemoveTargetArrows();
			DisplayItems();
		}
		else if (PreviousState == Action_Select) {
			//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Purple, "Undoing Attack Selection!");
			ChangeMenuWidget(StartingWidgetClass);
			RemoveTargetArrows();
			State = Action_Select;

		}
	}

	if (State != CharacterActing && State != Execute_Turn) {
		PawnController->MoveToCharacter(Players[PlayerIndex], FrontSide);
	}

}


void ARPGGameGameMode::AdjustEnemyIndex(int val) {
	Enemies[EnemyIndex]->RemoveTargetArrow();
	EnemyIndex += val;

	if (EnemyIndex < 0) {
		EnemyIndex = Enemies.Num() - 1;
	}
	else if (EnemyIndex == Enemies.Num()) {
		EnemyIndex = 0;
	}

	Enemies[EnemyIndex]->DisplayTargetArrow();
}

void ARPGGameGameMode::AdjustListIndex(int val) {

}

void ARPGGameGameMode::ConfirmAction() {
	if (Players[PlayerIndex]->QueuedAction == UseItem) {
		InventoryComponent->RemoveConsumableByReference(Players[PlayerIndex]->QueuedItem);
	}

	RemoveTargetArrows();
	IncrementPlayerActionIndex();


	
}

void ARPGGameGameMode::RemoveTargetArrows() {
	if (State == Player_Select) {
		Players[PlayerSelectIndex]->RemoveTargetArrow();
		Players[PlayerIndex]->SetStoredTarget(Players[PlayerSelectIndex]);
		//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Magenta, "Remvoing target from ally!");
	}
	else if (State == Enemy_Select) {
		Enemies[EnemyIndex]->RemoveTargetArrow();
		Players[PlayerIndex]->SetStoredTarget(Enemies[EnemyIndex]);
		//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Magenta, "Remvoing target from enemy!");
	}
	else if (State == Enemy_Select_Multi) {
		Players[PlayerIndex]->SetStoredTargetMultiple(&Enemies);
		RemoveEnemyTargetArrows();
	}
	else if (State == Player_Select_Multi) {
		Players[PlayerIndex]->SetStoredTargetMultiple(&Players);
		RemoveAllyTargetArrows();
	}
}

void ARPGGameGameMode::RemoveEnemyTargetArrows() { //Turn these two functions into delegate calls
	//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Magenta, "Remvoing target from ALL ENMEIS!");
	for (int i = 0; i < Enemies.Num(); i++) {
		Enemies[i]->RemoveTargetArrow();
	}
}

void ARPGGameGameMode::RemoveAllyTargetArrows() { //Turn these two functions into delegate calls
	//GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Magenta, "Remvoing target from ALL ALLIES!");
	for (int i = 0; i < Players.Num(); i++) {
		Players[i]->RemoveTargetArrow();
	}
}



void ARPGGameGameMode::IncrementPlayerActionIndex() {

	if (PlayerIndex >= 0) {
		Players[PlayerIndex]->RemoveTargetArrow();
	}


	PlayerIndex++;

	if (PlayerIndex == Players.Num()) {
		AskConfirmTurn();
		return;
	}

	//Do a check, keep incrementing forward until you find a non-incapacitated party member
	while (Players[PlayerIndex]->GetCurrentStatus() == Dead || Players[PlayerIndex]->GetCurrentStatus() == Confuse || Players[PlayerIndex]->GetCurrentStatus() == Sleep) {
		PlayerIndex++;
		if (PlayerIndex == Players.Num()) {
			AskConfirmTurn();
			return;
		}
	}


	PawnController->MoveToCharacter(Players[PlayerIndex], FrontSide);

	State = Action_Select;
	ChangeMenuWidget(StartingWidgetClass);
	Players[PlayerIndex]->DisplayTargetArrow();
}

void ARPGGameGameMode::DecrementPlayerActionIndex() {

	PlayerIndex--;

	//Re-add their queued item to the item list
	if (Players[PlayerIndex]->QueuedAction == UseItem) {
		AddConsumable(Players[PlayerIndex]->QueuedItem);
	}

	while (Players[PlayerIndex]->GetCurrentStatus() == Dead || Players[PlayerIndex]->GetCurrentStatus() == Confuse || Players[PlayerIndex]->GetCurrentStatus() == Sleep) {
		int StoredIndex = PlayerIndex;
		PlayerIndex--;
		if (PlayerIndex < 0) {
			PlayerIndex = StoredIndex + 1;
			return;
		}

	}
	if (PlayerIndex + 1 < Players.Num()) {
		Players[PlayerIndex + 1]->RemoveTargetArrow();
	}


	PawnController->MoveToCharacter(Players[PlayerIndex], FrontSide);

	State = Action_Select;
	ChangeMenuWidget(StartingWidgetClass);
	Players[PlayerIndex]->DisplayTargetArrow();
}

//Should be used for selecting players as the targets of skills
void ARPGGameGameMode::AdjustPlayerSelectIndex(int val) {
	Players[PlayerSelectIndex]->RemoveTargetArrow();
	PlayerSelectIndex += val;



	if (PlayerSelectIndex < 0) {
		PlayerSelectIndex = Players.Num() - 1;
	}
	else if (PlayerSelectIndex == Players.Num()) {
		PlayerSelectIndex = 0;
	}

	Players[PlayerSelectIndex]->DisplayTargetArrow();

	


}

void ARPGGameGameMode::RemoveConsumable(UItem_ConsumableAsset* ItemRef) {
	InventoryComponent->RemoveConsumableByReference(ItemRef);
}

void ARPGGameGameMode::AddConsumable(UItem_ConsumableAsset* ItemRef) {
	InventoryComponent->AddConsumableByReference(ItemRef);
}

void ARPGGameGameMode::RemoveConsumableByID(int ID) {
	InventoryComponent->RemoveConsumableByID(ID);
}
void ARPGGameGameMode::AddConsumableByID(int ID) {
	InventoryComponent->AddConsumableByID(ID);
}

int ARPGGameGameMode::Partition(TArray<ARPGCharacter*>& Entities, int Low, int High) {
	int Pivot = Entities[High]->ActionSpeed;
	int i = Low - 1;

	for (int j = Low; j <= High - 1; j++) {
		if (Entities[j]->ActionSpeed > Pivot) {
			i++;
			Swap(Entities[i], Entities[j]);
		}
	}

	Swap(Entities[i + 1], Entities[High]);
	return i + 1;
}

void ARPGGameGameMode::QuickSort(TArray<ARPGCharacter*>& Entities, int Low, int High) {
	if (Low >= High) {
		return;
	}

	int pi = Partition(Entities, Low, High);


	QuickSort(Entities, Low, pi - 1);
	QuickSort(Entities, pi + 1, High);


}