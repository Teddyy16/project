using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    public GameObject axelotl;
    public GameObject bunny;
    public GameObject current =null;
    public void Spawn()
    {
        if (current != null)
            Destroy(current);
        GameObject prefab =(PlayerPrefs.GetInt("isRabbitEqip", 0) == 0) ? axelotl : bunny;
       current=Instantiate(prefab, transform.position, transform.rotation);
       current.transform.localScale = transform.localScale;
    }
    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
