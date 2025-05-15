// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Engine/DataTable.h"
#include "RenderActor.generated.h"





UENUM(BlueprintType)
enum EDialogueAnimType {
	Neutral UMETA(DisplayName = "Neutral"),
	Happy UMETA(DisplayName = "Happy"),
	Sad UMETA(DisplayName = "Sad"),
	Angry UMETA(DisplayName = "Angry"),
	Smug UMETA(DisplayName = "Smug"),
	Shocked UMETA(DisplayName = "Shocked"),
	Confused UMETA(DisplayName = "Confused")

};


UENUM(BlueprintType)
enum EDialogueSide {
	Unchanging UMETA(DisplayName = "Unchanging"),
	Left UMETA(DisplayName = "Left"),
	Right UMETA(DisplayName = "Right"),
	MiddleLeft UMETA(DisplayName = "MiddleLeft"),
	MiddleRight UMETA(DisplayName = "MiddleRight"),
	Exit UMETA(DisplayName = "Exit"),
	ExitAll UMETA(DisplayName = "ExitAll")


};


USTRUCT(BlueprintType)
struct FDialogueStruct : public FTableRowBase {

	GENERATED_BODY()

public:

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	FName SpeakerName;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	FString SpeakerText;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	TEnumAsByte<EDialogueAnimType> AnimType;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	TEnumAsByte<EDialogueSide> ScreenSide;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	TArray<FText> ReplyArray;


};

USTRUCT(BlueprintType)
struct FEmoteStruct {

	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	UAnimMontage* EnterMontage;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	UAnimMontage* IdleMontage;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	TEnumAsByte<EDialogueAnimType> AnimType;


};


//Make a table that contains data for each character that can speak
//Make just one render actor bp that spawns in no more than 3 times and swaps its material, skeleton, and anim class accordingly
USTRUCT(BlueprintType)
struct FRenderActorProperties : public FTableRowBase {

	GENERATED_BODY()

public:

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	FName SpeakerName;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	class USkeletalMesh* SkeletalMesh;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	UClass* AnimClass;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	TArray< FEmoteStruct > Emotes;

};





UCLASS()
class RPGGAME_API ARenderActor : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	ARenderActor();

protected:


public:	

	// Called every frame
	virtual void Tick(float DeltaTime) override;

	bool IsMoving;

	UPROPERTY(EditAnywhere)
	USceneComponent* DefaultRoot = nullptr;



	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	class USceneCaptureComponent2D* RenderCamera;


	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	class USkeletalMeshComponent* SkeletalMesh;



	FEmoteStruct*  CurrentEmote;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	UDataTable* RenderActorTable;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	FVector OffScreenPosition;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float LerpSpeed;


	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	float JourneyLength;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	float StartTime;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	FVector StartPos;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	FVector EndPos;

	FRenderActorProperties* Properties;

	//Call this, transition into a talking/emote animation, which then automatically transitions into idle on finish
	UFUNCTION(BlueprintCallable)
	void SetAnimation(EDialogueAnimType AnimType);

	UFUNCTION(BlueprintCallable)
	void SetSkeleton(FName Name);

	UFUNCTION(BlueprintCallable)
	void SetRenderTexture(class UTextureRenderTarget2D* Texture);

	UFUNCTION(BlueprintCallable, BlueprintPure)
	FName GetSpeakerName();

	UFUNCTION()
	void ShiftToPosition(float dt);



	UFUNCTION(BlueprintCallable)
	void SetMoving(bool Exit);


	UFUNCTION(BlueprintCallable)
	void EnterEmote();


	UFUNCTION(BlueprintCallable)
	void EnterIdle();

	FTimerHandle EmoteHandle;


	//Either we make multiple blueprints, one for each dialogue character
	//And then get their class references

	//Or we make just one RenderActor, give it an array of data that contains their skeleton, anim bp, and render material


};
