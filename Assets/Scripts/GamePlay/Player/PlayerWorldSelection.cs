using UnityEngine;
using static BaseEngine.ScreenManager;

public class PlayerWorldSelection : MonoBehaviour
{
    //private PropertyBase selectedProperty;
    private SelectableObject selectedObject;
    void OnEnable()
    {
        PlayerInput.OnSelectionObject += OnSelectionObject;
    }


    void OnDisable()
    {
        PlayerInput.OnSelectionObject -= OnSelectionObject;
    }

    private void OnSelectionObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            TurnOffSelectedObject();
            if (!hit.transform.TryGetComponent(out SelectableObject obj)) return;
            HandleSelectObject(obj);
        }
        else
        {
            TurnOffSelectedObject();
        }
    }
    private void HandleSelectObject(SelectableObject obj)
    {
        selectedObject = obj;
        selectedObject.SetSelected(true);
        string screenName = ScreenName.GamePlayScreen.ToString();
        var gamePlayScreen = ScreenManagerInstance.GetScreen<GamePlayScreen>(screenName);
        if (!gamePlayScreen) return;
        var infoPanel = gamePlayScreen.Container.InfoPanel;
        infoPanel.gameObject.SetActive(true);
        // handle case for person
        if (selectedObject.TryGetComponent(out Person person))
        {
            HandlePersonStatusChanged(person);
            Person.OnPersonStatusChanged += HandlePersonStatusChanged;
        }
    }

    private void TurnOffSelectedObject()
    {
        // turn off UI
        string screenName = ScreenName.GamePlayScreen.ToString();
        var gamePlayScreen = ScreenManagerInstance.GetScreen<GamePlayScreen>(screenName);
        if (!gamePlayScreen) return;
        var infoPanel = gamePlayScreen.Container.InfoPanel;
        infoPanel.gameObject.SetActive(false);


        // turn off selected object
        if (selectedObject == null) return;
        if (selectedObject.TryGetComponent(out Person person))
        {
            Person.OnPersonStatusChanged -= HandlePersonStatusChanged;
        }
        selectedObject.SetSelected(false);
        selectedObject = null;
    }

    private void HandlePersonStatusChanged(Person person)
    {
        string screenName = ScreenName.GamePlayScreen.ToString();
        var gamePlayScreen = ScreenManagerInstance.GetScreen<GamePlayScreen>(screenName);
        if (!gamePlayScreen) return;
        var infoPanel = gamePlayScreen.Container.InfoPanel;
        var personData = person.PersonData;
        var personStatus = person.PersonStatus;
        infoPanel.SetNameText(personData.Name);
        infoPanel.SetStateText(personStatus.CurrentState.ToString());


    }
}
