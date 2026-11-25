
using System;
using UnityEngine;

public enum QuestStage
{
    FindLantern,
    FindFirstIngredient,
    ExploreForest
}

public static class GameState
{
    public static bool HasLantern { get; private set; }
    public static bool HasReadLetter { get; private set; }
    public static bool HasFirstIngredient { get; private set; }

    public static QuestStage CurrentQuest { get; private set; }

    public static event Action OnStateChanged;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetState()
    {
        HasLantern = false;
        HasReadLetter = false;
        HasFirstIngredient = false;

        CurrentQuest = QuestStage.FindLantern;

        OnStateChanged = null;
    }

    public static void CollectLantern()
    {
        if (HasLantern)
            return;

        HasLantern = true;

        UpdateQuest();
        OnStateChanged?.Invoke();
    }

    public static void ReadLetter()
    {
        if (HasReadLetter)
            return;

        HasReadLetter = true;
        OnStateChanged?.Invoke();
    }

    public static void CollectFirstIngredient()
    {
        if (HasFirstIngredient)
            return;

        HasFirstIngredient = true;

        UpdateQuest();
        OnStateChanged?.Invoke();
    }

    private static void UpdateQuest()
    {
        if (!HasLantern)
        {
            CurrentQuest = QuestStage.FindLantern;
        }
        else if (!HasFirstIngredient)
        {
            CurrentQuest = QuestStage.FindFirstIngredient;
        }
        else
        {
            CurrentQuest = QuestStage.ExploreForest;
        }
    }

    public static string GetQuestDescription()
    {
        switch (CurrentQuest)
        {
            case QuestStage.FindLantern:
                return "Pick up the lantern in the hallway.";

            case QuestStage.FindFirstIngredient:
                return "Find the first ingredient in the forest.";

            case QuestStage.ExploreForest:
                return "Explore the forest.";

            default:
                return "";
        }
    }
}
