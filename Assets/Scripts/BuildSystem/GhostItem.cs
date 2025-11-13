using UnityEngine;

public class GhostItem : MonoBehaviour
{
    public BoxCollider solidCollider; 
    public Renderer mRenderer;
    private Material semiTransparentMat; 
    private Material fullTransparentnMat;
    private Material selectedMaterial;

    public bool isPlaced;
    public bool hasSamePosition = false;

    private void Start()
    {
        mRenderer = GetComponent<Renderer>();
        
        if (ConstructionManager.Instance == null)
        {
            Debug.LogError("ConstructionManager Instance is null!");
            return;
        }

        semiTransparentMat = ConstructionManager.Instance.ghostSemiTransparentMat;
        fullTransparentnMat = ConstructionManager.Instance.ghostFullTransparentMat;
        selectedMaterial = ConstructionManager.Instance.ghostSelectedMat;

        mRenderer.material = semiTransparentMat; 
        
        if(solidCollider != null)
        {
            solidCollider.enabled = false;
        }
    }

    private void Update()
    {
        if (ConstructionManager.Instance == null) return;

        if (solidCollider != null)
        {
            solidCollider.enabled = ConstructionManager.Instance.inConstructionMode && isPlaced;
        }

        if (mRenderer != null)
        {
            if(ConstructionManager.Instance.selectedGhost == this.gameObject)
            {
                mRenderer.material = selectedMaterial;
            }
            else
            {
                mRenderer.material = semiTransparentMat; 
            }
        }
    }
}