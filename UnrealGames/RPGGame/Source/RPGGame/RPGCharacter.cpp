// Fill out your copyright notice in the Description page of Project Settings.


#include "RPGCharacter.h"

#include "RPGGameGameMode.h"
#include "RPGAlly.h"
#include "RPGEnemy.h"


#include "AC_Status.h"
#include "AC_Buff.h"

#include "Blueprint/WidgetBlueprintLibrary.h"
#include "EnumContainer.h"
#include "Item_ConsumableAsset.h"

#include "Components/WidgetComponent.h"
#include "Components/Widget.h"

#include "Kismet/GameplayStatics.h"

#include "ViewPawn.h"



// Sets default values
ARPGCharacter::ARPGCharacter()
{
 	// Set this character to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	SkillComponent = CreateDefaultSubobject<USkillComponent>(TEXT("SkillComponent"));
	BuffComponent = CreateDefaultSubobject<UAC_Buff>(TEXT("BuffComponent"));
	StatusComponent = CreateDefaultSubobject<UAC_Status>(TEXT("StatusComponent"));
	SelectArrow = CreateDefaultSubobject<UWidgetComponent>(TEXT("TargetArrow"));
	SelectArrow->SetVisibility(true);
	SelectArrow->SetupAttachment(GetMesh());


}

// Called when the game starts or when spawned
void ARPGCharacter::BeginPlay()
{
	Super::BeginPlay();

	SelectArrow->SetHiddenInGame(true);
	HP = MaxHealth;
	SP = MaxSP;
	HealthPercent = HP / MaxHealth;
	SPPercent = SP / MaxSP;

	ARPGGameGameMode* GameMode = Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()));
	GameMode->EndOfTurnDelegate.AddDynamic(this, &ARPGCharacter::EndTurn);
}

// Called every frame
void ARPGCharacter::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}



void ARPGCharacter::LoseHealth(int Amount) {
	HP -= Amount;

	if (HP <= 0) {
		Die();
	}

	

	HealthPercent = HP / MaxHealth;

	//Enter tired state
	if (HealthPercent <= 0.3f) {
		IsLowOnHealth = true;
	}
}

void ARPGCharacter::LoseSP(int Amount) {
	SP -= Amount;

	SPPercent = SP / MaxSP;
}

void ARPGCharacter::RestoreHealth(int Amount) {
	GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Cyan, FString::Printf(TEXT("Restoring Health: %d"), Amount));
	HP += Amount;

	if (HP > MaxHealth) {
		HP = MaxHealth;
	}

	HealthPercent = HP / MaxHealth;

	//Enter Normal state
	if (HealthPercent > 0.3f) {
		IsLowOnHealth = false;
	}
}

void ARPGCharacter::RestoreSP(int Amount) {
	SP += Amount;

	SPPercent = SP / MaxSP;
}

void ARPGCharacter::Revive(int Amount) {
	GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Red, "GENERIC REVIVAL!");
}

bool ARPGCharacter::MeetsSPRequirement(int SkillIndex) {
	return (SP >= SkillComponent->Skills[SkillIndex]->Cost);
}

bool ARPGCharacter::MeetsHPRequirement(int SkillIndex) {
	return (HP > SkillComponent->Skills[SkillIndex]->Cost);
}

bool ARPGCharacter::MeetsRequirement(int SkillIndex) {
	if (SkillComponent->Skills[SkillIndex]->ResourceType == SkillPoints) {
		return MeetsSPRequirement(SkillIndex);
	}
	else if (SkillComponent->Skills[SkillIndex]->ResourceType == HealthPoints) {
		return MeetsHPRequirement(SkillIndex);
	}

	return false;
}

void ARPGCharacter::Die() {
	GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Magenta, "Dying!");
	SetCurrentStatus(Dead);
}




void ARPGCharacter::AttackTarget(ARPGCharacter* Target) {
	if (Target->AttemptDodge(CalculateAccuracy())) {
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Orange, "Target Dodged!");
	}
	else {
		DealDamage(Target, Target->CalculateTakenDamage(CalculateDealtDamage(Physical, 1.0f), Physical, NormalAttackAttribute));
	}
}

void ARPGCharacter::SetStoredTarget(ARPGCharacter* Target) {
	StoredTarget = Target;
}


void ARPGCharacter::SetStoredTargetMultiple(TArray<ARPGCharacter*> *Targets) {
	StoredTargetMultiple = Targets;
}


void ARPGCharacter::DealDamage(ARPGCharacter* Target, int Damage) {

	Target->TakeDamage(Damage);
	GEngine->AddOnScreenDebugMessage(-1, 2.0f, FColor::Orange, "Dealing Damage!");

}

void ARPGCharacter::SetQueuedAction(FActionType type) {
	QueuedAction = type;
}

void ARPGCharacter::SetQueuedSkill(int SkillIndex) {
	SkillComponent->SetQueuedSkill(SkillIndex);
	ActionSpeed = (Speed + SkillComponent->QueuedSkill->Priority) * BuffComponent->GetBuff_Speed();

}

void ARPGCharacter::SetQueuedItem(UItem_ConsumableAsset* Item) {
	QueuedItem = Item;

}

void ARPGCharacter::DisplayTargetArrow() {
	SelectArrow->SetHiddenInGame(false);
	IsSelected = true;
}

void ARPGCharacter::RemoveTargetArrow() {
	SelectArrow->SetHiddenInGame(true);
	IsSelected = false;
}

//what if instead of holding an array of weaknesses and resistances, each character has a dedicated weakness/resistance value for each attribute and status?
float ARPGCharacter::GetResistanceMultiplier(FAttribute Property) {
	switch (Property) {
	case Fire:
		return GetFireRes();
		break;

	case Ice:
		return GetIceRes();
		break;

	case Electric:
		return GetElecRes();
		break;

	case Slash:
		return GetSlashRes();
		break;

	case Stab:
		return GetStabRes();
		break;

	case Bash:
		return GetBashRes();
		break;

	}

	return 1;
}

//return false if ailment prevents player from acting
bool ARPGCharacter::CheckForAilments() {

	//Look I know we coudl compress these into one statement, but for now this makes for a good debug checker and maybe we can print specific action statements off these later
	if (QueuedAction == NoAction) {
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Magenta, "NO ACTION SET!");
		return false;
	}
	else if (GetCurrentStatus() == Dead) {
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, "I'M DEAD!");
		return false;
	}
	else if (GetCurrentStatus() == Stun) {
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Cyan, "I'M STUNNED!");
		return false;
	}
	else if (GetCurrentStatus() == Sleep) {
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Cyan, "HONK SHOO HONK SHOO HONK SHOO!");
		return false;
	}

	if (GetCurrentStatus() == Paralyze) {
		if (StatusComponent->Paralyzed()) {
			GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Yellow, "YOU'RE PARALYYYYYZED!");
			return false;
		}
	}
	else if (GetCurrentStatus() == Confuse) {
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Magenta, "YOU'RE CONFUSED!");
		StatusComponent->Confusion();
	}

	return true;
}

void ARPGCharacter::BeginAction() {

}

void ARPGCharacter::BeginAnimationAction(FActionType Type) {
	UAnimInstance* instance = GetMesh()->GetAnimInstance();
	if (!instance) {
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, "FAILED TO GET ANIM INSTANCE!");
		return;
	}


	float animSpeed = 1.0f;
	float MontageLength = 0;
	if (Type == Physical) {
		MontageLength = instance->Montage_Play(AttackMontage, animSpeed);
	}
	else if (Type == Defend) {
		MontageLength = instance->Montage_Play(DefenseMontage, animSpeed);
	}
	else if (Type == UseItem) {
		MontageLength = instance->Montage_Play(ItemMontage, animSpeed);
	}
	
	//Instead of timing till the end of the montage, we should time it till the end of specific actions
	//For example, damage actions 
	//Item actions wait until an effect or effect timer ends 
	GetWorld()->GetTimerManager().SetTimer(ActionHandle, this, &ARPGCharacter::EndAction, MontageLength);
}

void ARPGCharacter::BeginAnimationSkill(FType Type) {


	UAnimInstance* instance = GetMesh()->GetAnimInstance();
	UAnimMontage* MontageToPlay = AttackMontage;

	if (!instance) {
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, "FAILED TO GET ANIM INSTANCE!");
		return;
	}
	int i;

	//Search for animation by name
	if (SkillComponent->QueuedSkill->UseSpecialAnimation) {
		for (i = 0; i < UniqueAnims.Num(); i++) {
			if (SkillComponent->QueuedSkill->SkillName == (FName)UniqueAnims[i].Montage->GetName()) {
				MontageToPlay = UniqueAnims[i].Montage;
				break;
			}
		}
	}
	else { //Otherwise use some default Animation
		for (i = 0; i < SkillAnims.Num(); i++) {
			if (Type == SkillAnims[i].MontageType) {
				MontageToPlay = SkillAnims[i].Montage;
				break;
			}
		}

	}

	if (MontageToPlay) {
		float animSpeed = 1.0f;
		float MontageLength = instance->Montage_Play(MontageToPlay, animSpeed);



		GetWorld()->GetTimerManager().SetTimer(ActionHandle, this, &ARPGCharacter::EndAction, MontageLength);
	}


}

void ARPGCharacter::BeginAnimationEmote(EAnimType Type) {
	UAnimInstance* instance = GetMesh()->GetAnimInstance();

	if (!instance) {
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, "FAILED TO GET ANIM INSTANCE!");
		return;
	}
	int i;

	if (Type == SearchByName) {
		for (i = 0; i < EmoteAnims.Num(); i++) {
			if (SkillComponent->QueuedSkill->SkillName == EmoteAnims[i].MontageName) {
				break;
			}
		}
	}
	else {
		for (i = 0; i < EmoteAnims.Num(); i++) {
			if (Type == EmoteAnims[i].MontageType) {
				break;
			}
		}

	}

	float animSpeed = 1.0f;
	instance->Montage_Play(EmoteAnims[i].Montage, animSpeed);

}

void ARPGCharacter::EndDamage() {
	if (QueuedAction == Defend) {
		GetMesh()->GetAnimInstance()->Montage_Play(DefenseMontage, 1.0f);
	}


}


void ARPGCharacter::EndAction() {


	GetWorld()->GetTimerManager().ClearTimer(ActionHandle);


	if (!NextCharacterToAct) {
		Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()))->StartNextTurn();
		return;
	}

	GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Green, "Executing Next Action!");

	NextCharacterToAct->BeginAction();
	NextCharacterToAct = nullptr;
	StoredTarget = nullptr;
	StoredTargetMultiple = nullptr;

	//Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()))->NextAction();
}


void ARPGCharacter::EndTurn() {
	if (QueuedAction == Defend) {
		GetMesh()->GetAnimInstance()->Montage_Stop(1, DefenseMontage);
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Green, "Ending Defense!");
	}
	QueuedAction = NoAction;

	if (BuffComponent->GetBuff_IsRegeneratingHP()) {
		BuffComponent->RegenerateHP();
	}
	if (BuffComponent->GetBuff_IsRegeneratingSP()) {
		BuffComponent->RegenerateSP();
	}

	BuffComponent->DecrementBuffTime();

	if (GetCurrentStatus() == Poisoned) {
		StatusComponent->Poison();
	}



	if (GetCurrentStatus() != Normal || GetCurrentStatus() != Dead) {
		StatusComponent->AttemptRecoverAilment();
	}

	

}

void ARPGCharacter::CheckNewEnemyTarget() { //Check for if the stored target has died, and we need to target a random new enemy
	if (StoredTarget) {
		if (StoredTarget->GetCurrentStatus() == Dead) {
			StoredTarget = Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()))->GetNewEnemyTarget();
		}
	}

}


void ARPGCharacter::CheckNewPlayerTarget() {
	if (StoredTarget) {
		if (StoredTarget->GetCurrentStatus() == Dead) {
			StoredTarget = Cast<ARPGGameGameMode>(UGameplayStatics::GetGameMode(GetWorld()))->GetNewAllyTarget();
		}
	}

}

void ARPGCharacter::SetNextCharacterActor(ARPGCharacter* Character) {
	NextCharacterToAct = Character;
}

void ARPGCharacter::ChooseTarget(TArray<ARPGCharacter*> Targets) {
	int Rand = FMath::RandRange(0, Targets.Num() - 1);


	if (Targets[Rand]->GetCurrentStatus() == Dead) { //Account for if target is dead
		while (true) {
			Rand = FMath::RandRange(0, Targets.Num() - 1);
			if (Targets[Rand]->GetCurrentStatus() != Dead) {
				break;
			}
		}
	}

	SetStoredTarget(Targets[Rand]);

}

void ARPGCharacter::ExecuteAction() {


	if (QueuedAction == NormalAttack) {
		ExecuteAttack();
	}
	else if (QueuedAction == UseSkill) {
		ExecuteSkill();
	}
	else if (QueuedAction == UseItem) {
		ExecuteItem();
	}
	else if (QueuedAction == Defend) {
		ExecuteDefend();
	}

}



void ARPGCharacter::ExecuteSkill() {
	GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Blue, "Using Skill!");
	SkillComponent->Execute();
	SkillComponent->QueuedSkill = nullptr;
	//LoseSP(SkillComponent->QueuedSkill->Cost);
}



void ARPGCharacter::ExecuteAttack() {
	AttackTarget(StoredTarget);
	GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Green, "Using Normal Attack!");
}




void ARPGCharacter::ExecuteItem() {


	if (QueuedItem) {
		GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Magenta, QueuedItem->Name.ToString());
		QueuedItem->OnActivate(this);
		//QueuedItem = nullptr;
	}
	else {
		GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Red, "FAILED TO USE ITEM!");
	}



}

void ARPGCharacter::ExecuteDefend() {
	GEngine->AddOnScreenDebugMessage(-1, 2.0f, FColor::Black, "Defending!");


}

void ARPGCharacter::CalculateActionSpeed() {
	ActionSpeed = Speed;

	if (SkillComponent->QueuedSkill) {
		ActionSpeed += SkillComponent->QueuedSkill->Priority;
	}
	else if (QueuedAction == Defend) {
		ActionSpeed += 900;
	}
}

float ARPGCharacter::CalculateDealtDamage(FType Type, float Power) {
	float Damage = 0;

	if (Type == Elemental) {
		Damage = MagicPower * Power;
		Damage *= BuffComponent->GetBuff_MagAttack();
	}
	else if (Type == Physical) {
		Damage = PhysicalPower * Power;
		Damage *= BuffComponent->GetBuff_PhysAttack();
	}

	Damage = Damage * BuffComponent->GetBuff_Attack();

	return Damage;
}


float ARPGCharacter::CalculateTakenDamage(float Power, FType Type, FAttribute Property) {
	float Damage = Power;

	if (Type == Physical) {
		Damage -= PhysicalDefense;
		Damage /= BuffComponent->GetBuff_PhysDefense();

	}
	else if (Type == Elemental) {
		Damage -= MagicDefense;
		Damage /= BuffComponent->GetBuff_MagDefense();
	}

	Damage *= GetResistanceMultiplier(Property);
	Damage /= BuffComponent->GetBuff_Defense();


	if (QueuedAction == Defend) {
		Damage *= DefendMultiplier;
	}

	return Damage;

}


bool ARPGCharacter::AttemptDodge(float AttackerAccuracy) {
	int DodgeChance = FMath::RandRange(0, 100);
	GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Emerald, FString::Printf(TEXT("Supplied Accuracy: %f"), AttackerAccuracy));


	if (GetCurrentStatus() == Sleep) {
		AttackerAccuracy += 100;
	}
	else {
		AttackerAccuracy -= (Evasion * BuffComponent->GetBuff_Evasion()) - (Speed * 2) - Luck;
	}
	


	GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Emerald, FString::Printf(TEXT("Dodge Chance: %d"), DodgeChance));
	GEngine->AddOnScreenDebugMessage(-1, 10.0f, FColor::Emerald, FString::Printf(TEXT("Final Calculated Chance To Hit: %f"), AttackerAccuracy));

	if (DodgeChance > AttackerAccuracy) {
		return true;
	}

	return false;

}

float ARPGCharacter::CalculateAccuracy(float AccuracyModifier) {
	float UserAccuracy = ((Accuracy + AccuracyModifier) * BuffComponent->GetBuff_Accuracy()) + (Speed * 2);

	if (GetCurrentStatus() == Blind) {
		Accuracy *= 0.33f;
	}

	return UserAccuracy;
}

void ARPGCharacter::TakeDamage(int Damage) {
	UAnimInstance* instance = GetMesh()->GetAnimInstance();
	float MontageLength = 0;

	//Play a special defend damage montage?
	if (QueuedAction == Defend) {
		MontageLength = instance->Montage_Play(DamageMontage, 1);
	}
	else {
		MontageLength = instance->Montage_Play(DamageMontage, 1);
	}

	if (GetCurrentStatus() == Sleep) {
		Damage *= 1.5f;
		SetCurrentStatus(Normal);
	}


	LoseHealth(Damage);

	if (GetCurrentStatus() != Dead) {
		GetWorld()->GetTimerManager().SetTimer(DamageHandle, this, &ARPGCharacter::EndDamage, MontageLength);
	}
	


}

void ARPGCharacter::GetBuff(EBuffType Type) {
	
}

void ARPGCharacter::RestoreStatus() {
	SetCurrentStatus(Normal);
}

float ARPGCharacter::GetBashRes() {
	return BashRes;
}
float ARPGCharacter::GetStabRes() {
	return StabRes;
}
float ARPGCharacter::GetSlashRes() {
	return SlashRes;
}



float ARPGCharacter::GetFireRes() {
	return FireRes;
}

float ARPGCharacter::GetIceRes() {
	return IceRes;
}
float ARPGCharacter::GetElecRes() {
	return ElecRes;
}


TEnumAsByte<FStatus> ARPGCharacter::GetCurrentStatus() {
	return StatusComponent->CurrentStatus;
}

void ARPGCharacter::SetCurrentStatus(FStatus Status) {
	StatusComponent->CurrentStatus = Status;
}

//Let this receive the ailment type and chance of infliction
//Then use players own properties to determine if it lands or not
//We can modify this so that users have a bonus chance to inflict on targets but maybe later
void ARPGCharacter::AttemptInflictAilment(FStatus Status, int BaseChance) {
	StatusComponent->AttemptInflictAilment(Status, BaseChance);
}