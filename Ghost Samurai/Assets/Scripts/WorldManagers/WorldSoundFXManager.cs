using UnityEngine;

public class WorldSoundFXManager : MonoBehaviour
{
   public static WorldSoundFXManager instance;

   [Header("Enemy track")] 
   [SerializeField] private AudioSource enemyIntroPlayer;
   
   
   
   [Header("Damage Sounds")]
   public AudioClip[] physicalDamageSFX;
   
   [Header("Actions Sound")]
   public AudioClip rollingSFX;
   public AudioClip stanceBreakSFX;
   public AudioClip criticalStrikeSFX;

   private void Awake()
   {
      if (instance == null)
      {
         instance = this;
      }
      else
      {
         Destroy(gameObject);
      }
   }

   private void Start()
   {
      DontDestroyOnLoad(this.gameObject);
   }

   public AudioClip ChooseRandomSfxFromArray(AudioClip[] clips)
   {
      int index = Random.Range(0, clips.Length);
      return clips[index];
   }
}
