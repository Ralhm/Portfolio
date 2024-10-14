// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "Skill.h"
#include "Engine/DataTable.h"
#include "SkillComponent.generated.h"



USTRUCT(BlueprintType)
struct FSkillInfo : public FTableRowBase {

	GENERATED_BODY()

	FSkillInfo()
	{

	}

	FSkillInfo(int cost, FString skillname, FString skilldescription, FType type, FAttribute property, FAttribute Subproperty, bool multitarget = false, bool offensive = true)
		: Cost(cost), SkillName(skillname), SkillDescription(skilldescription), SkillType(type), SkillProperty(property), SubProperty(Subproperty),
		IsMultiTarget(multitarget), IsOffensive(offensive)
	{

	}

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FString SkillName;


	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TEnumAsByte<FType> SkillType;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TEnumAsByte<FAttribute> SkillProperty;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TEnumAsByte<FAttribute> SubProperty;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	int Cost;


	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FString SkillDescription;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	bool IsMultiTarget;

	UPROPERTY(EditAnywhere, BlueprintReadWrite) //Is this used on an enemy or an ally
	bool IsOffensive;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TSoftObjectPtr<UTexture> SkillIcon;

};


UCLASS(Blueprintable, ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class RPGGAME_API USkillComponent : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	USkillComponent();

protected:


	FORCEINLINE class ARPGCharacter* GetCharacter();


public:


	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	UDataTable* SkillTable;


	UPROPERTY(Instanced, EditAnywhere, BlueprintReadWrite)
	TArray <USkill*> Skills;



	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
	USkill* QueuedSkill;


	UFUNCTION()
	void Execute();

	UFUNCTION()
	void SetQueuedSkill(int SkillIndex);

	UFUNCTION()
	void AddSkill(const FName SkillName);

};
