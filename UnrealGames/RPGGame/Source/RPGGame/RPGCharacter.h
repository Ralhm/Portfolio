// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Character.h"
#include "SkillComponent.h"
#include "RPGCharacter.generated.h"


USTRUCT(BlueprintType)
struct FActionAnims  {

	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	UAnimMontage* Montage;

	//Enum used typically for actions
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	TEnumAsByte<FType> MontageType;



};

USTRUCT(BlueprintType)
struct FEmoteAnims {

	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	UAnimMontage* Montage;

	//Enum used for emotes
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	TEnumAsByte<EAnimType> MontageType;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	FName MontageName;


};





UCLASS()
class RPGGAME_API ARPGCharacter : public ACharacter
{
	GENERATED_BODY()

public:
	// Sets default values for this character's properties
	ARPGCharacter();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;


	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	class UWidgetComponent* SelectArrow;

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
	USkillComponent* SkillComponent;

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
	class UAC_Buff* BuffComponent;

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
	class UAC_Status* StatusComponent;

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
	ARPGCharacter* StoredTarget;


	TArray<ARPGCharacter*> *StoredTargetMultiple;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	ARPGCharacter* NextCharacterToAct;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	class UItem_ConsumableAsset* QueuedItem;





	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	TEnumAsByte<FAttribute> NormalAttackAttribute;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	TEnumAsByte<FActionType> QueuedAction;


	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Animation")
	TArray<FActionAnims> UniqueAnims;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Animation")
	TArray<FActionAnims> SkillAnims;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Animation")
	TArray<FEmoteAnims> EmoteAnims;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Animation")
	UAnimMontage* AttackMontage;


	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Animation")
	UAnimMontage* DefenseMontage;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Animation")
	UAnimMontage* ItemMontage;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Animation")
	UAnimMontage* DamageMontage;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Animation")
	UAnimMontage* DeathMontage;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	int index;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	bool IsSelected;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	bool IsLowOnHealth;

	//What is the order in which this character acts?
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	int ActionSpeed;


	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Properties")
	int PhysicalPower;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Properties")
	int MagicPower;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Properties")
	int PhysicalDefense;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Properties")
	int MagicDefense;


	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Properties")
	float MaxHealth;


	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	float HP;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	float HealthPercent;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Properties")
	float MaxSP;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	float SP;


	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	float SPPercent;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Properties")
	float Speed;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Properties")
	float Evasion = 10.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Properties")
	int Luck = 5;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Properties")
	float Accuracy = 85.0f;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Properties")
	float FireRes = 1.0f;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Properties")
	float IceRes = 1.0f;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Properties")
	float ElecRes = 1.0f;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Properties")
	float BashRes = 1.0f;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Properties")
	float StabRes = 1.0f;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Properties")
	float SlashRes = 1.0f;


	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Properties")
	float DefendMultiplier = 0.3f;

	UFUNCTION()
	void CalculateActionSpeed();


	UFUNCTION()
	void DisplayTargetArrow();

	UFUNCTION()
	void CheckNewEnemyTarget();

	UFUNCTION()
	void CheckNewPlayerTarget();


	UFUNCTION()
	bool MeetsSPRequirement(int SkillIndex);

	UFUNCTION()
	bool MeetsHPRequirement(int SkillIndex);

	UFUNCTION()
	bool MeetsRequirement(int SkillIndex);

	UFUNCTION()
	void RemoveTargetArrow();

	UFUNCTION()
	virtual void TakeDamage(int Damage);


	void LoseHealth(int Amount);

	void LoseSP(int Amount);

	void RestoreHealth(int Amount);

	void RestoreSP(int Amount);

	UFUNCTION()
	void RestoreStatus();

	UFUNCTION()
	void GetBuff(EBuffType Type);

	UFUNCTION()
	virtual void Revive(int Amount);



	virtual void Die();

	UFUNCTION()
	void DealDamage(ARPGCharacter* Target, int Damage);

	UFUNCTION()
	float GetResistanceMultiplier(FAttribute Property);


	UFUNCTION()
	void SetNextCharacterActor(ARPGCharacter* Character);


	FORCEINLINE float GetBashRes();
	FORCEINLINE float GetStabRes();
	FORCEINLINE float GetSlashRes();


	FORCEINLINE float GetFireRes();
	FORCEINLINE float GetIceRes();
	FORCEINLINE float GetElecRes();


	void AttackTarget(ARPGCharacter* Target);

	UFUNCTION()
	void SetStoredTarget(ARPGCharacter* Target);


	void SetStoredTargetMultiple(TArray<ARPGCharacter*> *Targets);


	UFUNCTION()
	float CalculateTakenDamage(float Power, FType Type, FAttribute Property);

	UFUNCTION()
	float CalculateDealtDamage(FType Type, float Power);

	UFUNCTION()
	virtual void BeginAction();

	UFUNCTION()
	bool CheckForAilments();

	UFUNCTION()
	void ChooseTarget(TArray<ARPGCharacter*> Targets);

	UFUNCTION()
	void BeginAnimationEmote(EAnimType Type);

	UFUNCTION()
	void BeginAnimationAction(FActionType Type);

	UFUNCTION()
	void BeginAnimationSkill(FType Type);

	UFUNCTION()
	bool AttemptDodge(float AttackerAccuracy);

	UFUNCTION()
	float CalculateAccuracy(float AccuracyModifier = 0.0f);

	UFUNCTION()
	void EndAction();

	UFUNCTION()
	void EndTurn();

	UFUNCTION()
	void EndDamage();

	UFUNCTION()
	void ExecuteSkill();

	UFUNCTION()
	void ExecuteAction();

	UFUNCTION()
	void ExecuteAttack();

	UFUNCTION()
	void ExecuteItem();

	UFUNCTION()
	void ExecuteDefend();


	UFUNCTION()
	TEnumAsByte<FStatus> GetCurrentStatus();

	UFUNCTION()
	void SetCurrentStatus(FStatus Status);

	UFUNCTION()
	void AttemptInflictAilment(FStatus Status, int BaseChance);

	FORCEINLINE void SetQueuedAction(FActionType type);

	FORCEINLINE void SetQueuedSkill(int SkillIndex);

	FORCEINLINE void SetQueuedItem(class UItem_ConsumableAsset* Item);


	FTimerHandle ActionHandle;
	FTimerHandle DeathHandle;
	FTimerHandle DamageHandle;
};
