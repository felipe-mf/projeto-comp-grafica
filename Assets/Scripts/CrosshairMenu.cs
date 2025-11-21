using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CrosshairMenu : MonoBehaviour
{
    [Header("Referência do Dropdown")]
    public TMP_Dropdown crosshairDropdown;

    private string dropdownTag = "CrosshairDropdown";
    private bool dropdownFound = false;

    private void Start()
    {
        FindAndSetupDropdown();
    }

    private void OnEnable()
    {
        // Sempre que voltar para o menu, reconecta o dropdown
        dropdownFound = false;
        FindAndSetupDropdown();
    }

    private void Update()
    {
        // Se ainda não encontrou o dropdown, continua procurando
        if ((!dropdownFound || crosshairDropdown == null) && IsInMenuScene())
        {
            FindAndSetupDropdown();
        }
    }

    private void FindAndSetupDropdown()
    {
        if (crosshairDropdown != null)
        {
            SetupDropdown();
            dropdownFound = true;
            Debug.Log("Dropdown CONFIGURADO com sucesso!");
        }

        // Tenta encontrar o dropdown
        if (crosshairDropdown == null)
        {
            FindDropdownByTag();
        }

        if (crosshairDropdown == null)
        {
            FindDropdownByName();
        }

        if (crosshairDropdown == null)
        {
            FindAnyDropdown();
        }

        // Se encontrou, configura
        if (crosshairDropdown != null)
        {
            SetupDropdown();
            dropdownFound = true;
            Debug.Log("Dropdown CONFIGURADO com sucesso!");
        }
        else
        {
            Debug.LogWarning("Ainda procurando pelo dropdown...");
        }
    }

    private void FindDropdownByTag()
    {
        GameObject dropdownObj = GameObject.FindGameObjectWithTag(dropdownTag);
        if (dropdownObj != null)
        {
            crosshairDropdown = dropdownObj.GetComponent<TMP_Dropdown>();
            if (crosshairDropdown != null)
            {
                Debug.Log("Dropdown encontrado por TAG: " + dropdownObj.name);
            }
        }
    }

    private void FindDropdownByName()
    {
        GameObject dropdownObj = GameObject.Find("CrosshairDropdown");
        if (dropdownObj != null)
        {
            crosshairDropdown = dropdownObj.GetComponent<TMP_Dropdown>();
            if (crosshairDropdown != null)
            {
                Debug.Log("Dropdown encontrado por NOME: " + dropdownObj.name);
            }
        }
    }

    private void FindAnyDropdown()
    {
        TMP_Dropdown[] allDropdowns = FindObjectsOfType<TMP_Dropdown>();
        if (allDropdowns.Length > 0)
        {
            crosshairDropdown = allDropdowns[0];
            Debug.Log("Dropdown encontrado (qualquer um): " + crosshairDropdown.name);
        }
    }

    private void SetupDropdown()
    {
        if (crosshairDropdown == null) return;

        // Remove listeners antigos
        crosshairDropdown.onValueChanged.RemoveAllListeners();

        // Limpa e configura as opções
        crosshairDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>
        {
            "Dot", "Cross", "Brackets"
        };
        crosshairDropdown.AddOptions(options);

        // Atualiza a seleção baseada no Crosshair atual
        UpdateDropdownSelection();

        // Adiciona o listener
        crosshairDropdown.onValueChanged.AddListener(OnCrosshairChanged);

        Debug.Log("Dropdown totalmente configurado! Valor: " + crosshairDropdown.value);
    }

    private void UpdateDropdownSelection()
    {
        if (Crosshair.Instance != null && crosshairDropdown != null)
        {
            int currentValue = (int)Crosshair.Instance.crosshairType;

            // Remove listener temporariamente para não triggerar evento
            crosshairDropdown.onValueChanged.RemoveListener(OnCrosshairChanged);

            crosshairDropdown.value = currentValue;
            crosshairDropdown.captionText.text = crosshairDropdown.options[currentValue].text;

            // Readiciona o listener
            crosshairDropdown.onValueChanged.AddListener(OnCrosshairChanged);

            Debug.Log("Dropdown selecionado: " + currentValue + " - " + Crosshair.Instance.crosshairType);
        }
    }

    private void OnCrosshairChanged(int typeIndex)
    {
        if (Crosshair.Instance != null)
        {
            Crosshair.CrosshairType newType = (Crosshair.CrosshairType)typeIndex;

            if (Crosshair.Instance.crosshairType != newType)
            {
                Crosshair.Instance.SetCrosshairType(newType);
                Debug.Log("Mira alterada para: " + newType);
            }
        }
    }

    // forçar reconexão
    public void ForceReconnectDropdown()
    {
        dropdownFound = false;
        crosshairDropdown = null;
        FindAndSetupDropdown();
    }

    bool IsInMenuScene()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        return sceneName.ToLower().Contains("menu");
    }
}