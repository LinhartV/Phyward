using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// Class for managing exits from a particular level.
/// </summary>
[Serializable]
public class Base : Item, IInteractable
{
    public Base() { }

    public Base(Vector2 pos, Tilemap map = null) : base(pos, GameObjects.baseHouse, false, map)
    {
        this.Prefab.transform.position = new Vector3(pos.x, pos.y, 1);
    }

    public void Interact()
    {
        GCon.game.SavedLevelId = GCon.game.CurLevel.Id;
        ToolsUI.baseInventory.OpenInventory();
        if (GCon.game.TutorialPhase == 4)
        {
            Tutorial.ShowTutorial(new Tutorial.TutorialBlock("Tvá základna", "Napravo vidíš tvou výbavu, kterou si vezmeš na cesty.\nVlevo zas předměty na tvé základně.", "Veličiny se automaticky uskladní na základně - zbraně a jinou výbavu můžeš libovolně přetahovat.", null,
                () =>
                {
                    Tutorial.ShowTutorial(new Tutorial.TutorialBlock("Po levé straně si můžeš vyrobit výbavu. Zobrazuje se jen ta, ke které jsi objevil příslušné veličiny. Vyrobenou výbavu si nezapomeň vzít do batohu. To - co chceš právě používat, přetáhni na horní lištu.", null, () =>
                    {
                        Tutorial.ShowTutorial(new Tutorial.TutorialBlock("Vyrob si nejprve lékárničku a vyleč si své zdraví. Jednorázové bonusy si vybavíš tak, že je přetáhneš do kolonek E nebo Q - pomocí dané klávesy je pak aktivuješ. Pokud umřeš, objevíš se na základně a celá hra se zlehčí - tvůj skill se počítá podle toho, jak dokážeš... neumírat.", null, null, 25));
                    }, 25));
                }
                , 25));
            GCon.game.TutorialPhase = 5;
            GCon.game.CurLevel.DestroyAllItemsOfType<InvisibleBlock>();
        }
        if (GCon.game.TutorialPhase == 10)
        {
            GCon.game.TutorialPhase = 11;
            GCon.game.CurLevel.DestroyAllItemsOfType<InvisibleBlock>();
        }

    }
}

