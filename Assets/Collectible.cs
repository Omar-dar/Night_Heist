using TMPro;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int value = 10000;
    public TMP_Text valueLabel;

    [Header("Show label when player is close")]
    public float showDistance = 4f;

    private Transform player;

    private void Awake()
    {
        if (valueLabel != null)
        {
            valueLabel.text = value.ToString();
            valueLabel.gameObject.SetActive(false); // start hidden
        }
    }

    private void Start()
    {
        // Find player once
        var cc = FindObjectOfType<CharacterController>();
        if (cc != null) player = cc.transform;
    }

    private void Update()
    {
        if (valueLabel == null || player == null) return;

        float d = Vector3.Distance(player.position, transform.position);
        bool shouldShow = d <= showDistance;

        if (valueLabel.gameObject.activeSelf != shouldShow)
            valueLabel.gameObject.SetActive(shouldShow);
    }

    public void Collect()
    {
        Destroy(gameObject);
    }
}
