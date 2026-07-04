using UnityEngine;

public class LocaleButtonHandler : MonoBehaviour
{
    // 오른쪽 버튼
    public void NextLocale()
    {
        if (Localization.instance == null) return;
        Localization.instance.ChangeLocaleByDelta(1);
    }

    // 왼쪽 버튼
    public void PrevLocale()
    {
        if (Localization.instance == null) return;
        Localization.instance.ChangeLocaleByDelta(-1);
    }
}
