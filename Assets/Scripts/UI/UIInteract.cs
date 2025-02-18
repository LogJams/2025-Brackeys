using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UIInteract : MonoBehaviour {

    public TMPro.TMP_Text description;
    public GameObject root;

    PlayerInteractions player;

    UIDisplayText showText;


    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteractions>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        root.SetActive(false);
        player.OnInteractionStart += OnStartInteraction;
        player.OnInteractionEnd += OnEndInteraction;


        player.OnInteractionEvent += OnEndInteraction;
        player.OnInteractionClose += OnStartInteraction;
    }


    public void OnStartInteraction(System.Object source, IInteractable target) {
        root.SetActive(true);
        description.text = target.GetDescription();
    }

    public void OnEndInteraction(System.Object source, IInteractable target) {
        root.SetActive(false);
    }

}
