using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    private Canvas canvas;
    private EventSystem eventSystem;
    private bool contextMenuOpen = false;
    public List<Button> ContextMenu;

	void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        eventSystem = FindObjectOfType<EventSystem>();
	}
	
	void Update()
    {
        var lmb = Input.GetMouseButtonDown(0);
        var rmb = Input.GetMouseButtonDown(1);

        if (lmb || rmb)
        {
            if (contextMenuOpen)
            {
                if (eventSystem.currentSelectedGameObject != null && ContextMenu.Contains(eventSystem.currentSelectedGameObject.GetComponent<Button>()))
                    return; // UI should handle this, not us.
                else
                {
                    hideContextMenu();
                    if (lmb)
                        return;
                }
            }

            var mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            var mouseGrid = Grid.Get().GetCellFromRay(Camera.main.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y)));
            
            var crt = (canvas.transform as RectTransform);
            Vector2 mouseUi = new Vector2(mousePos.x / crt.localScale.x, mousePos.y / crt.localScale.y);

            if (lmb)
                GetComponent<Actor>().MoveTo(mouseGrid);
            else if (rmb)
            {
                if (ContextMenu == null || ContextMenu.Count < 2)
                    throw new Exception("PlayerInput.ContextMenu not bound // < 2 elements available!");

                // just some test actions
                List<Tuple<string, Action>> actions = new List<Tuple<string, Action>>();
                actions.Add(Tuple.Create<string, Action>("Test action 1", () => { Debug.Log("TA1 fired"); }));
                actions.Add(Tuple.Create<string, Action>("Test action 2", () => Debug.Log("TA2 fired")));
                actions.Add(Tuple.Create<string, Action>("Test action 3", () => Debug.Log("TA3 fired")));
                
                // put actions in UI elements
                if (actions.Count > ContextMenu.Count)
                    Debug.LogWarningFormat("PlayerInput.Update: can't show all context menu actions; need {0} but have {1} UI elements", actions.Count, ContextMenu.Count);

                Vector2 pos = mouseUi; // new Vector2(mousePos.x, mousePos.y);

                for (int i = 0; i < (actions.Count > ContextMenu.Count ? ContextMenu.Count - 1 : actions.Count); i++)
                {
                    ContextMenu[i].gameObject.SetActive(true);
                    ContextMenu[i].GetComponentInChildren<Text>().text = actions[i].Item1;
                    ContextMenu[i].onClick.AddListener(new UnityEngine.Events.UnityAction(hideContextMenu));
                    ContextMenu[i].onClick.AddListener(new UnityEngine.Events.UnityAction(actions[i].Item2));
                    ((RectTransform)ContextMenu[i].transform).anchoredPosition = pos;
                    pos.y -= ((RectTransform)ContextMenu[i].transform).sizeDelta.y;
                }

                if (actions.Count > ContextMenu.Count)
                {
                    var last = ContextMenu[ContextMenu.Count - 1];
                    last.gameObject.SetActive(true);
                    last.GetComponentInChildren<Text>().text = "More... (TODO)";
                    last.onClick.AddListener(new UnityEngine.Events.UnityAction(hideContextMenu));
                    ((RectTransform)last.transform).anchoredPosition = pos;
                }

                contextMenuOpen = true;
            }
        }
	}

    private void hideContextMenu()
    {
        if (ContextMenu == null)
            return;

        foreach (Button b in ContextMenu)
        {
            b.onClick.RemoveAllListeners();
            b.gameObject.SetActive(false);
        }

        contextMenuOpen = false;
    }
}
