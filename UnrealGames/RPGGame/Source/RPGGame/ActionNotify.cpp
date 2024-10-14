// Fill out your copyright notice in the Description page of Project Settings.


#include "ActionNotify.h"
#include "RPGCharacter.h"

void UActionNotify::Notify(USkeletalMeshComponent* MeshComp, UAnimSequenceBase* Animation) {
	if (MeshComp != NULL && MeshComp->GetOwner() != NULL) {
		ARPGCharacter* User = Cast<ARPGCharacter>(MeshComp->GetOwner());

		if (User) {
			User->ExecuteAction();
		}

	}
}