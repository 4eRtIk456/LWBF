
using UnityEngine;

public static class SceneEntryState
{
    public static bool EnteringHouse { get; private set; }

    public static void SetEnteringHouse()
    {
        EnteringHouse = true;
    }

    public static bool ConsumeEnteringHouse()
    {
        bool enteringHouse = EnteringHouse;
        EnteringHouse = false;

        return enteringHouse;
    }

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetState()
    {
        EnteringHouse = false;
    }
}
