// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "EnumContainer.h"
#include "AC_Buff.generated.h"

USTRUCT(BlueprintType)
struct FBuffEffect {
	GENERATED_BODY()
	//This decides if it's a buff or debuff
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float Power;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TEnumAsByte<EBuffType> BuffType;
};

USTRUCT(BlueprintType)
struct FBuffData {

	GENERATED_BODY()

	//How many turns does the buff last?
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FName Name;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	bool IsDebuff = false;

	//How many turns does the buff last?
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	int Duration;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TArray<FBuffEffect> Effects;

	//How many turns are left on the current buff duration?
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	int CurrentDuration = 0;

	FBuffData()
	{

	}




	bool operator==(const FBuffData& obj) {
		return Name == obj.Name;
	}

};


UCLASS( ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class RPGGAME_API UAC_Buff : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UAC_Buff();

protected:


	const int MaxBuffs = 3;

public:	

	
	UPROPERTY(VisibleAnywhere)
	float Buff_Attack = 1.0f;

	UPROPERTY(VisibleAnywhere)
	float Buff_PhysAttack = 1.0f;

	UPROPERTY(VisibleAnywhere)
	float Buff_MagAttack = 1.0f;

	UPROPERTY(VisibleAnywhere)
	float Buff_Defense = 1.0f;

	UPROPERTY(VisibleAnywhere)
	float Buff_PhysDefense = 1.0f;

	UPROPERTY(VisibleAnywhere)
	float Buff_MagDefense = 1.0f;

	UPROPERTY(VisibleAnywhere)
	float Buff_Speed = 1.0f;

	UPROPERTY(VisibleAnywhere)
	float Buff_DrawRate = 1.0f;

	UPROPERTY(VisibleAnywhere)
	float Buff_Evasion = 1.0f;

	UPROPERTY(VisibleAnywhere)
	float Buff_Accuracy = 1.0f;

	UPROPERTY(VisibleAnywhere)
	float Buff_Luck = 1.0f;

	UPROPERTY(VisibleAnywhere)
	float Buff_RegenerateHP = 0.0f;

	UPROPERTY(VisibleAnywhere)
	float Buff_RegenerateSP = 0.0f;

	UPROPERTY(VisibleAnywhere)
	bool Buff_IsRegeneratingHP = false;

	UPROPERTY(VisibleAnywhere)
	bool Buff_IsRegeneratingSP = false;

	UPROPERTY(VisibleAnywhere)
	TArray<FBuffData> Buffs;

	UPROPERTY(VisibleAnywhere)
	TArray<FBuffData> DeBuffs;


	float GetBuff_Attack();
	float GetBuff_PhysAttack();
	float GetBuff_MagAttack();
	float GetBuff_Defense();
	float GetBuff_PhysDefense();
	float GetBuff_MagDefense();
	float GetBuff_Speed();
	float GetBuff_Evasion();
	float GetBuff_Accuracy();
	float GetBuff_Luck();
	float GetBuff_RegenerateHP();
	float GetBuff_RegenerateSP();
	bool GetBuff_IsRegeneratingHP();
	bool GetBuff_IsRegeneratingSP();


	void SetBuff_Attack(float val);
	void SetBuff_PhysAttack(float val);
	void SetBuff_MagAttack(float val);
	void SetBuff_Defense(float val);
	void SetBuff_PhysDefense(float val);
	void SetBuff_MagDefense(float val);
	void SetBuff_Speed(float val);
	void SetBuff_Evasion(float val);
	void SetBuff_Accuracy(float val);
	void SetBuff_Luck(float val);
	void SetBuff_DrawRate(float val);
	void SetBuff_RegenerateHP(float val);
	void SetBuff_RegenerateSP(float val);


	//Decide if this buff is a buff or debuff
	void ReceiveBuff(FBuffData Buff);

	void ApplyBuff(FBuffData Buff);
	void ApplyDeBuff(FBuffData Buff);

	void AddBuff(FBuffData Buff);
	void AddDeBuff(FBuffData DeBuff);

	void RemoveBuff(FBuffData Buff);
	void RemoveDeBuff(FBuffData DeBuff);

	void ExtendBuff(int Duration, int index);
	void ExtendDeBuff(int Duration, int index);
	bool AttemptExtendBuff(FBuffData Buff);
	bool AttemptExtendDeBuff(FBuffData Buff);
	void DecrementBuffTime();

	void ApplyBuffEffect(FBuffEffect Effect);
	void RemoveBuffEffect(FBuffEffect Effect);

	void RegenerateHP();
	void RegenerateSP();

};
