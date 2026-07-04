using UnityEngine;

public class EscManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            if (TryGetComponent<Setting>(out Setting settingComp)) {
                settingComp.SaveBtnClick();
            }
        }
    }
}