using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CellProperties;

public class Cell : MonoBehaviour
{
    public bool isActive = false;
    public CellProperties cellProperties;
    public GameObject childObject;

    [SerializeField]
    private GameObject frogPrefab, berryPrefab, arrowPrefab;

    [SerializeField]
    private MeshRenderer cellBase;

    [SerializeField]
    private TextureData textureData;

    // Start is called before the first frame update
    void Start()
    {
        GetBaseTextureForCellTypeAndColor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetClicked(bool isGood)
    {
        if (isActive) 
        {
            if(isGood)
                StartCoroutine(TriggerItem());
            else
                StartCoroutine(ChangeMaterialColor());
        }
    }
    private IEnumerator OpenAnimation()
    {
        float timer = 0f;
        float duration = 0.25f;
        Vector3 originalScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;

        // Determine the correct prefab based on cell type
        GameObject prefabToInstantiate = null;
        switch (cellProperties.cellType)
        {
            case CellProperties.CellType.Frog:
                prefabToInstantiate = frogPrefab;
                break;
            case CellProperties.CellType.Berry:
                prefabToInstantiate = berryPrefab;
                break;
            case CellProperties.CellType.Arrow:
                prefabToInstantiate = arrowPrefab;
                break;
            default:
                break;
        }

        // Instantiate the prefab and assign it to childObject
        if (prefabToInstantiate != null)
        {
            Vector3 rotationEuler;
            if(cellProperties.cellType == CellType.Berry)
            {
                rotationEuler = Vector3.zero;
            }
            else
            {
                switch (cellProperties.direction)
                {
                    case CellProperties.Direction.Up:
                        rotationEuler = Vector3.zero;
                        break;
                    case CellProperties.Direction.Left:
                        rotationEuler = new Vector3(0f, 270f, 0f);
                        break;
                    case CellProperties.Direction.Down:
                        rotationEuler = new Vector3(0f, 180f, 0f);
                        break;
                    case CellProperties.Direction.Right:
                        rotationEuler = new Vector3(0f, 90f, 0f);
                        break;
                    default:
                        rotationEuler = Vector3.zero;
                        break;
                }
            }
            if(cellProperties.cellType == CellProperties.CellType.Berry)
            {
                childObject = Instantiate(prefabToInstantiate, transform.position, Quaternion.Euler(rotationEuler), transform.parent);
            }
            else
            {
                childObject = Instantiate(prefabToInstantiate, transform.position, Quaternion.Euler(rotationEuler), cellBase.transform);
            }
            foreach (CellTypeTexture typeTexture in textureData.typeTextures)
            {
                if (typeTexture.cellType == cellProperties.cellType)
                {
                    childObject.GetComponentInChildren<Renderer>().material.mainTexture = typeTexture.textures[(int)cellProperties.color];
                }
            }
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float scaleFactor = Mathf.Lerp(0f, 1f, timer / duration);
                childObject.transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleFactor);
                yield return null;
            }
            // Ensure scale is exactly 1 at the end of animation
            childObject.transform.localScale = targetScale;
        }        
    }
    private IEnumerator CloseAnimation(float interval)
    {
        yield return new WaitForSeconds(interval);
        float timer = 0f;
        float duration = 0.2f;
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = Vector3.zero;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float scaleFactor = Mathf.Lerp(0f, 1f, timer / duration);
            cellBase.transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleFactor);
            yield return null;
        }
        // Ensure scale is exactly 1 at the end of animation
        cellBase.transform.localScale = targetScale;
        cellBase.gameObject.SetActive(false);
        isActive = false;
        GetComponentInParent<GridStack>().ActivateTopCell();
    }
    public void ActivateCell()
    {
        StartCoroutine(OpenAnimation());
        isActive = true;
    }
    public void DeactivateCell(float interval)
    {
        StopAllCoroutines();
        StartCoroutine(CloseAnimation(interval));
    }
    private void GetBaseTextureForCellTypeAndColor()
    {
        switch (cellProperties.cellType)
        {
            case CellProperties.CellType.Frog:
                cellBase.materials[0].mainTexture = textureData.baseTextures[(int)cellProperties.color];
                break;
            case CellProperties.CellType.Berry:
                cellBase.materials[0].mainTexture = textureData.baseTextures[(int)cellProperties.color];
                break;
            default:
                break;
        }
    }

    private IEnumerator TriggerItem()
    {
        if (childObject != null)
        {
            float timer = 0f;
            float duration = 0.1f;
            Vector3 originalScale = Vector3.one;
            Vector3 targetScale = originalScale * 1.4f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float scaleFactor = Mathf.Lerp(0f, 1f, timer / duration);
                childObject.transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleFactor);
                yield return null;
            }

            timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float scaleFactor = Mathf.Lerp(0f, 1f, timer / duration);
                childObject.transform.localScale = Vector3.Lerp(targetScale, originalScale, scaleFactor);
                yield return null;
            }

            // Ensure scale is exactly original scale at the end
            childObject.transform.localScale = originalScale;
        }
    }

    // IEnumerator to change material color animation
    private IEnumerator ChangeMaterialColor()
    {
        if (childObject != null)
        {
            float duration = 0.2f;
            Renderer renderer = childObject.GetComponentInChildren<Renderer>();
            Material material = renderer.material;

            UnityEngine.Color originalColor = UnityEngine.Color.black;
            UnityEngine.Color targetColor = UnityEngine.Color.white;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                material.color = UnityEngine.Color.Lerp(originalColor, targetColor, t);
                yield return null;
            }

            // Ensure color is exactly target color at the end
            material.color = targetColor;
        }
    }
    public LineRenderer TriggerFrog()
    {
        return childObject.GetComponentInChildren<LineRenderer>();
    }
}
