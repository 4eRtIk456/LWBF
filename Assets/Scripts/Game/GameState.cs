using System;
using UnityEngine;

public enum QuestStage
{
    Intro,
    CheckMailbox,
    CheckRecipeBook,
    FindLantern,
    FindFirstIngredient,
    ExploreForest,
    BrewPotion
}

public static class GameState
{
    public static bool HasFinishedMorningIntro { get; private set; }
    public static bool HasReadLetter { get; private set; }
    public static bool HasCheckedRecipeBook { get; private set; }

    public static bool HasLantern { get; private set; }

    public static bool HasFirstIngredient { get; private set; }
    public static bool HasSecondIngredient { get; private set; }
    public static bool HasThirdIngredient { get; private set; }

    public static int IngredientCount
    {
        get
        {
            int count = 0;

            if (HasFirstIngredient)
                count++;

            if (HasSecondIngredient)
                count++;

            if (HasThirdIngredient)
                count++;

            return count;
        }
    }

    public static QuestStage CurrentQuest { get; private set; }

    public static event Action OnStateChanged;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetState()
    {
        HasFinishedMorningIntro = false;
        HasReadLetter = false;
        HasCheckedRecipeBook = false;

        HasLantern = false;

        HasFirstIngredient = false;
        HasSecondIngredient = false;
        HasThirdIngredient = false;

        CurrentQuest = QuestStage.Intro;

        OnStateChanged = null;
    }

    public static void CompleteMorningIntro()
    {
        if (HasFinishedMorningIntro)
            return;

        HasFinishedMorningIntro = true;
        UpdateQuest();
    }

    public static void ReadLetter()
    {
        if (HasReadLetter)
            return;

        HasReadLetter = true;
        UpdateQuest();
    }

    public static void CheckRecipeBook()
    {
        if (HasCheckedRecipeBook)
            return;

        HasCheckedRecipeBook = true;
        UpdateQuest();
    }

    public static void CollectLantern()
    {
        if (HasLantern)
            return;

        HasLantern = true;
        UpdateQuest();
    }

    public static void CollectFirstIngredient()
    {
        if (HasFirstIngredient)
            return;

        HasFirstIngredient = true;
        UpdateQuest();
    }

    public static void CollectSecondIngredient()
    {
        if (HasSecondIngredient)
            return;

        HasSecondIngredient = true;
        UpdateQuest();
    }

    public static void CollectThirdIngredient()
    {
        if (HasThirdIngredient)
            return;

        HasThirdIngredient = true;
        UpdateQuest();
    }

    private static void UpdateQuest()
    {
        if (!HasFinishedMorningIntro)
        {
            CurrentQuest = QuestStage.Intro;
        }
        else if (!HasReadLetter)
        {
            CurrentQuest = QuestStage.CheckMailbox;
        }
        else if (!HasCheckedRecipeBook)
        {
            CurrentQuest = QuestStage.CheckRecipeBook;
        }
        else if (!HasLantern)
        {
            CurrentQuest = QuestStage.FindLantern;
        }
        else if (IngredientCount == 0)
        {
            CurrentQuest = QuestStage.FindFirstIngredient;
        }
        else if (IngredientCount < 3)
        {
            CurrentQuest = QuestStage.ExploreForest;
        }
        else
        {
            CurrentQuest = QuestStage.BrewPotion;
        }

        OnStateChanged?.Invoke();
    }

    public static string GetQuestDescription()
    {
        switch (CurrentQuest)
        {
            case QuestStage.Intro:
                return "";

            case QuestStage.CheckMailbox:
                return "Check the mailbox";

            case QuestStage.CheckRecipeBook:
                return "Check the recipe book";

            case QuestStage.FindLantern:
                return "Pick up the lantern";

            case QuestStage.FindFirstIngredient:
            case QuestStage.ExploreForest:
                return "Collect ingredients (" +
                    IngredientCount + "/3)";

            case QuestStage.BrewPotion:
                return "Brew the potion";

            default:
                return "";
        }
    }
}
