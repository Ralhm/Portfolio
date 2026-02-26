// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "UObject/NoExportTypes.h"
#include "EnumContainer.h"
#include "Skill.generated.h"

/**
 * 
 */





UCLASS(DefaultToInstanced, EditInlineNew, Blueprintable)
class RPGGAME_API USkill : public UObject
{
	GENERATED_BODY()
	
public:


	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FName SkillName;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FName SkillDescription;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TEnumAsByte<FType> SkillType;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TEnumAsByte<FAttribute> SkillProperty;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TEnumAsByte<FAttribute> SubProperty;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TEnumAsByte<FResourceType> ResourceType;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TEnumAsByte<ETargetType> TargetType;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	int Cost;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float SkillPower;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float Accuracy = 0.0f;

	//Does this move come out faster or slower than other moves?
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float Priority;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	bool IsFriendly;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	bool IsMultiTarget;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	bool UseSpecialAnimation;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TSoftObjectPtr<UTexture> SkillIcon;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Camera");
	TEnumAsByte<ECamMovementType> CamType = Shoulder;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Camera");
	bool Snap = true;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Camera");
	bool ZoomOut = false;

	//UPROPERTY(EditAnywhere, BlueprintReadWrite)
	//class UAnimMontage* Animation;

	UFUNCTION()
	virtual void OnActivate(class ARPGCharacter* User);

	UFUNCTION()
	virtual void UseSkillOnMultiple(class ARPGCharacter* User, TArray<class ARPGCharacter*> Targets);


	UFUNCTION()
	virtual void SkillEffect(class ARPGCharacter* User, class ARPGCharacter* Target);

	UFUNCTION()
	virtual void AttemptDamageTarget(class ARPGCharacter* User, class ARPGCharacter* Target);

	UFUNCTION()
	virtual void OnBegin();

	UFUNCTION()
	virtual void OnEnd();

	void HealTarget(class ARPGCharacter* User, class ARPGCharacter* Target);


	void Revive(class ARPGCharacter* User, class ARPGCharacter* Target);

	UFUNCTION()
	virtual void UseResource(class ARPGCharacter* User);

	float CalculateDamage(class ARPGCharacter* Target, class ARPGCharacter* User) const;


};
