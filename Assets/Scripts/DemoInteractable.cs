using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoInteractable : Interactable {

    public PlayerController player;
    public SpriteRenderer interactIcon;

    void Start() {
        interactIcon.enabled = false;
    }

    public override bool CanInteract() {
        return player.grounded;
    }

    public override void GainFocus() {
        interactIcon.enabled = true;
    }

    public override void LoseFocus() {
        interactIcon.enabled = false;
    }

    public override void Interact() {
        player.health.heal(10);
    }
}
