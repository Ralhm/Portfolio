// Fill out your copyright notice in the Description page of Project Settings.


#include "SkillComponent.h"
#include "RPGCharacter.h"

// Sets default values for this component's properties
USkillComponent::USkillComponent()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	// ...
}




void USkillComponent::Execute() {
	QueuedSkill->OnActivate(Cast<ARPGCharacter>(GetOwner()));
	QueuedSkill->UseResource(Cast<ARPGCharacter>(GetOwner()));
	//GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, QueuedSkill->SkillName);
	


}

void USkillComponent::AddSkill(FName SkillName) {
	//SkillsData.Add(*SkillTable->FindRow<FSkillInfo>((SkillName), FString(TEXT("Player Skill")), true));
}

void USkillComponent::SetQueuedSkill(int SkillIndex) {
	QueuedSkill = Skills[SkillIndex];

}

ARPGCharacter* USkillComponent::GetCharacter() {
	return Cast<ARPGCharacter>(GetOwner());
}