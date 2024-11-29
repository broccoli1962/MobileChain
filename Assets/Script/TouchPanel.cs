using UnityEngine;

public class TouchPanel : MonoBehaviour
{
    PanelInteract pi;
    PlayerSystem playerSystem;

    private void Start()
    {
        pi = FindAnyObjectByType<PanelInteract>();
        playerSystem = FindAnyObjectByType<PlayerSystem>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int monsterLayerMask = LayerMask.GetMask("Monster");
            
            RaycastHit2D[] rayhit = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(Input.mousePosition));
            RaycastHit2D[] rayhit2 = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, monsterLayerMask);
            
            //확인 순서를 잘 정해야한다. monster > ui > panel
            foreach (RaycastHit2D ray in rayhit2)
            {
                //monster click
                if (ray.transform.CompareTag("Monster") && !playerSystem.turn)
                {
                    Monster monster = ray.transform.GetComponent<Monster>();
                    if (monster != null)
                    {
                        monster.Selected();
                    }
                    return;
                }
            }
            foreach (RaycastHit2D ray in rayhit)
            {
                if (ray.transform.CompareTag("UI"))
                {
                    Debug.Log("ui 클릭함");
                    return;
                }
            }
            foreach (RaycastHit2D ray in rayhit)
            {
                //panel click
                if (ray.transform.CompareTag("Panel") && !playerSystem.turn || ray.transform.CompareTag("CLPanel") && !playerSystem.turn)
                {
                    GameObject hitObj = ray.transform.gameObject;
                    pi.click(hitObj);
                    return;
                }
            }
        }
    }
}