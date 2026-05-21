// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class PetManager : MonoBehaviour
// {

//     public GameObject petPrefab;
    
//     private void Awake()
//     {
//         // Spawns the pet when a new room is loaded
//        GameObject spawnedPet = Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
        
//         // Unity won't destroy the clone
//         DontDestroyOnLoad(spawnedPet);
        
//         // Destroy the object it created
//         Destroy(gameObject);

//     }

//     private void OnEnable()
//     {
        
//         SceneManager.sceneLoaded += OnSceneLoaded;

//     }

//     private void OnDisable()
//     {
        
//         SceneManager.sceneLoaded -= OnSceneLoaded;

//     }

//     private void OnSceneLoaded (Scene scene, LoadSceneMode mode)
//     {
        
//         // Finds the spawn point placed in the specific room
//         GameObject spawnPoint = GameObject.FindWithTag("PetSpawnPoint");

//         if (spawnPoint != null)
//         {
            
//             // Spawn at the exact spawn Point coordinates and rotation so that it faces the camera
//             transform.position = spawnPoint.transform.position;
//             transform.rotation = spawnPoint.transform.rotation;
//         }

            
//                 else
//         {
//             Debug.LogWarning("No PetSpawnPoint found in thi scene! Deafaulting to 0,0,0");
//             transform.position = Vector3.zero;

//         }

//     }
// }

using UnityEngine;
using UnityEngine.SceneManagement;

public class PetManager : MonoBehaviour
{
    // Scrpits can have access and talk to this script 
    public static PetManager Instance { get; private set; }

    private void Awake()
    {
        // If a copy alr exists, destory it and put a new one 
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        // Unity will load the game object when switching rooms 
        DontDestroyOnLoad(gameObject);

        // Force the scene loaded event to hook up right here in Awake
        SceneManager.sceneLoaded -= OnSceneLoaded; 
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Remove link if the scene gets destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Debug
        Debug.Log("--- SCENE LOADED EVENT FIRED FOR: " + scene.name + " ---");
        
        RepositionPetInNewRoom();
    }

    private void RepositionPetInNewRoom()
    {
        // Looks for the Empty object with the tag
        GameObject spawnPoint = GameObject.FindWithTag("PetSpawnPoint");

        if (spawnPoint != null)
        {
            // Places pet on exact Spawn Point and Debug
            Debug.Log("Spawn point found! Moving pet to: " + spawnPoint.transform.position);
            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
        }
        else
        {
            
            Debug.LogError("CRITICAL: Could not find any object with the tag 'PetSpawnPoint' in this scene!");
            transform.position = Vector3.zero; // Fallback to center so it doesn't float in the void
        }
    }
}


    
