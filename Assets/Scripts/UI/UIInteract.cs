using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UIInteract : MonoBehaviour {

    public TMPro.TMP_Text description;
    public GameObject root;

    public GameObject longTextRoot;


    PlayerInteractions player;


    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteractions>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        root.SetActive(false);
        longTextRoot.SetActive(false);

        player.OnInteractionStart += OnStartInteraction;
        player.OnInteractionEnd += OnEndInteraction;


        player.OnInteractionEvent += OnEndInteraction;
        player.OnInteractionClose += OnStartInteraction;

        player.OnInteractionClose += OnClose;
        player.OnInteractionEnd += OnClose;
    }


    public void OnStartInteraction(System.Object source, IInteractable target) {
        root.SetActive(true);
        description.text = target.GetDescription();
    }

    public void OnEndInteraction(System.Object source, IInteractable target) {
        root.SetActive(false);
        longTextRoot.SetActive(true); //display the long text when we do InteractionEvent
    }

    public void OnClose(System.Object source, IInteractable target) {
        longTextRoot.SetActive(false);
    }

}
