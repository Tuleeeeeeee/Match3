using UnityEngine;

public class GemGridVisual : MonoBehaviour
{
    private Transform gemTransform;
    private GemGrid gemGrid;

    public GemGridVisual(Transform gemTransform, GemGrid gemGrid)
    {
        this.gemTransform = gemTransform;
        this.gemGrid = gemGrid;

        gemGrid.OnDestroyed += GemGrid_OnDestroyed;
    }

    private void GemGrid_OnDestroyed(object sender, System.EventArgs e)
    {
        gemTransform.GetComponent<Animation>().Play();
        Destroy(gemTransform.gameObject, 1f);
    }

    public void Update()
    {
        Vector3 targetPosition = gemGrid.GetWorldPosition();
        Vector3 moveDir = (targetPosition - gemTransform.position);
        float moveSpeed = 10f;
        gemTransform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}