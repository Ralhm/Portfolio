// Fill out your copyright notice in the Description page of Project Settings.


#include "AC_Buff.h"
#include "RPGAlly.h"

// Sets default values for this component's properties
UAC_Buff::UAC_Buff()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	// ...
}




float UAC_Buff::GetBuff_Attack() {
	return Buff_Attack;
}
float UAC_Buff::GetBuff_PhysAttack() {
	return Buff_PhysAttack;
}
float UAC_Buff::GetBuff_MagAttack() {
	return Buff_MagAttack;
}
float UAC_Buff::GetBuff_Defense() {
	return Buff_Defense;
}
float UAC_Buff::GetBuff_PhysDefense() {
	return Buff_PhysDefense;
}
float UAC_Buff::GetBuff_MagDefense() {
	return Buff_MagDefense;
}
float UAC_Buff::GetBuff_Speed() {
	return Buff_Speed;
}
float UAC_Buff::GetBuff_Luck() {
	return Buff_Luck;
}
float UAC_Buff::GetBuff_Evasion() {
	return Buff_Evasion;
}
float UAC_Buff::GetBuff_Accuracy() {
	return Buff_Accuracy;
}

float UAC_Buff::GetBuff_RegenerateHP() {
	return Buff_RegenerateHP;
}
float UAC_Buff::GetBuff_RegenerateSP() {
	return Buff_RegenerateSP;
}
bool UAC_Buff::GetBuff_IsRegeneratingHP() {
	return Buff_IsRegeneratingHP;
}
bool UAC_Buff::GetBuff_IsRegeneratingSP() {
	return Buff_IsRegeneratingSP;
}





void UAC_Buff::SetBuff_Attack(float val) {
	Buff_Attack *= val;
}
void UAC_Buff::SetBuff_PhysAttack(float val) {
	Buff_PhysAttack *= val;
}
void UAC_Buff::SetBuff_MagAttack(float val) {
	Buff_MagAttack *= val;
}
void UAC_Buff::SetBuff_Defense(float val) {
	Buff_Defense *= val;
}
void UAC_Buff::SetBuff_PhysDefense(float val) {
	Buff_PhysDefense *= val;
}
void UAC_Buff::SetBuff_MagDefense(float val) {
	Buff_MagDefense *= val;
}
void UAC_Buff::SetBuff_Speed(float val) {
	Buff_Speed *= val;
}
void UAC_Buff::SetBuff_Luck(float val) {
	Buff_Luck *= val;
}
void UAC_Buff::SetBuff_Evasion(float val) {
	Buff_Evasion *= val;
}

void UAC_Buff::SetBuff_Accuracy(float val) {
	Buff_Accuracy *= val;
}

void UAC_Buff::SetBuff_DrawRate(float val) {
	Buff_DrawRate *= val;

	ARPGAlly* Owner = Cast<ARPGAlly>(GetOwner());
	Owner->SetCurrentDrawRate(Owner->CurrentDrawRate * Buff_DrawRate);
}

void UAC_Buff::SetBuff_RegenerateHP(float val) {
	Buff_RegenerateHP += val;
	if (Buff_RegenerateHP > 0) {
		Buff_IsRegeneratingHP = true;
	}
	else if (Buff_RegenerateHP == 0) {
		Buff_IsRegeneratingHP = false;
	}

}
void UAC_Buff::SetBuff_RegenerateSP(float val) {
	Buff_RegenerateSP += val;
	if (Buff_RegenerateHP > 0) {
		Buff_IsRegeneratingSP = true;
	}
	else if (Buff_RegenerateHP == 0) {
		Buff_IsRegeneratingSP = false;
	}
}



//Decide if this buff is a buff or debuff
void UAC_Buff::ReceiveBuff(FBuffData Buff) {
	if (Buff.IsDebuff) {
		ApplyDeBuff(Buff);
	}
	else {
		ApplyBuff(Buff);
	}
}

void UAC_Buff::AddBuff(FBuffData Buff) {
	Buff.CurrentDuration = Buff.Duration;
	Buffs.Add(Buff);
	for (int i = 0; i < Buff.Effects.Num(); i++) {
		ApplyBuffEffect(Buff.Effects[i]);
	}
}
void UAC_Buff::AddDeBuff(FBuffData DeBuff) {
	DeBuff.CurrentDuration = DeBuff.Duration;
	DeBuffs.Add(DeBuff);
	for (int i = 0; i < DeBuff.Effects.Num(); i++) {
		ApplyBuffEffect(DeBuff.Effects[i]);
	}
}

//Decide what to do with the buff (extend it or add it to list of buffs)
void UAC_Buff::ApplyBuff(FBuffData Buff) {
	if (Buffs.Num() > 0) {
		if (AttemptExtendBuff(Buff)) {
			return;
		}
	}


	if (Buffs.Num() == MaxBuffs) {
		RemoveBuff(Buffs[0]);
	}

	AddBuff(Buff);



}


void UAC_Buff::ApplyDeBuff(FBuffData Buff) {
	if (DeBuffs.Num() > 0) {
		if (AttemptExtendDeBuff(Buff)) {
			return;
		}
	}


	DeBuffs.Add(Buff);
	for (int i = 0; i < Buff.Effects.Num(); i++) {
		ApplyBuffEffect(Buff.Effects[i]);
	}

}



void UAC_Buff::RemoveBuff(FBuffData Buff) {
	Buffs.Remove(Buff);
	for (int i = 0; i < Buff.Effects.Num(); i++) {
		RemoveBuffEffect(Buff.Effects[i]);
	}
}

void UAC_Buff::RemoveDeBuff(FBuffData DeBuff) {
	DeBuffs.Remove(DeBuff);
	for (int i = 0; i < DeBuff.Effects.Num(); i++) {
		RemoveBuffEffect(DeBuff.Effects[i]);
	}
}

//Keep this as a separate function, what if we want a special skill or item that extends buff durations?
void UAC_Buff::ExtendBuff(int Duration, int index) {
	Buffs[index].CurrentDuration += Duration;
	if (Buffs[index].CurrentDuration > 9) {
		Buffs[index].CurrentDuration = 9;
	}
}

void UAC_Buff::ExtendDeBuff(int Duration, int index) {
	DeBuffs[index].CurrentDuration += index;
	if (DeBuffs[index].CurrentDuration > 9) {
		DeBuffs[index].CurrentDuration = 9;
	}
}

bool UAC_Buff::AttemptExtendBuff(FBuffData Buff) {
	for (int i = 0; i < Buffs.Num(); i++) {
		if (Buffs[i] == Buff) {
			ExtendBuff(Buffs[i].Duration, i);

			return true;
		}
	}

	return false;
}

bool UAC_Buff::AttemptExtendDeBuff(FBuffData Buff) {
	for (int i = 0; i < Buffs.Num(); i++) {
		if (DeBuffs[i] == Buff) {
			ExtendBuff(DeBuffs[i].Duration, i);

			return true;
		}
	}

	return false;
}


void UAC_Buff::DecrementBuffTime() {
	for (int i = 0; i < Buffs.Num(); i++) {
		Buffs[i].CurrentDuration--;
		if (Buffs[i].CurrentDuration == 0) {
			RemoveBuff(Buffs[i]);
		}

	}
	for (int i = 0; i < DeBuffs.Num(); i++) {
		DeBuffs[i].CurrentDuration--;
		if (DeBuffs[i].CurrentDuration == 0) {
			RemoveBuff(DeBuffs[i]);
		}
	}
}

void UAC_Buff::RegenerateHP() {
	ARPGCharacter* Owner = Cast<ARPGCharacter>(GetOwner());
	Owner->RestoreHealth(Buff_RegenerateHP * Owner->MaxHealth);
	
}


void UAC_Buff::RegenerateSP() {
	ARPGCharacter* Owner = Cast<ARPGCharacter>(GetOwner());
	Owner->RestoreSP(Buff_RegenerateSP * Owner->MaxSP);
}


//Either we set all these values here and now on buff applications, 
//or every time we calculate damage and whatnot, we cycle through our list of buffs and calculate there. no thanks



//Maybe instead of getting the buff and deciding what to do with the received information
//We have the buff itself tell the buff receiver what to do with it's information?
//But how would the buff know what value to modify when applying itself
void UAC_Buff::ApplyBuffEffect(FBuffEffect Effect) {	
	switch (Effect.BuffType) {
	case Attack:
		SetBuff_Attack(Effect.Power);
		break;
	case PhysicalAttack:
		SetBuff_PhysAttack(Effect.Power);
		break;
	case MagicAttack:
		SetBuff_MagAttack(Effect.Power);
		break;
	case Defense:
		SetBuff_Defense(Effect.Power);
		break;
	case PhysicalDefense:
		SetBuff_PhysDefense(Effect.Power);
		break;
	case MagicDefense:
		SetBuff_MagDefense(Effect.Power);
		break;
	case Speed:
		SetBuff_Speed(Effect.Power);
		break;
	case Luck:
		SetBuff_Luck(Effect.Power);
		break;
	case Accuracy:
		SetBuff_Accuracy(Effect.Power);
		break;
	case Evasion:
		SetBuff_Evasion(Effect.Power);
		break;
	case DrawRate:
		SetBuff_DrawRate(Effect.Power);
		break;
	case RegenerationHP:
		SetBuff_RegenerateHP(Effect.Power);
		break;
	case RegenerationSP:
		SetBuff_RegenerateSP(Effect.Power);
		break;
	}
	
}

void UAC_Buff::RemoveBuffEffect(FBuffEffect Effect) {
	switch (Effect.BuffType) {
	case Attack:
		SetBuff_Attack(1.0f / Effect.Power);
		break;
	case PhysicalAttack:
		SetBuff_PhysAttack(1.0f / Effect.Power);
		break;
	case MagicAttack:
		SetBuff_MagAttack(1.0f / Effect.Power);
		break;
	case Defense:
		SetBuff_Defense(1.0f / Effect.Power);
		break;
	case PhysicalDefense:
		SetBuff_PhysDefense(1.0f / Effect.Power);
		break;
	case MagicDefense:
		SetBuff_MagDefense(1.0f / Effect.Power);
		break;
	case Speed:
		SetBuff_Speed(1.0f / Effect.Power);
		break;
	case Evasion:
		SetBuff_Evasion(1.0f / Effect.Power);
		break;
	case DrawRate:
		SetBuff_DrawRate(1.0f / Effect.Power);
		break;
	case RegenerationHP:
		SetBuff_RegenerateHP(-Effect.Power);
		break;
	case RegenerationSP:
		SetBuff_RegenerateSP(-Effect.Power);
		break;
	}
}