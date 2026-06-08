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
        Instantiate(prefab, transform.position, transform.rotation);
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
