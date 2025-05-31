using UnityEngine;

public class CharacterSoundFXManager : MonoBehaviour
{
    private AudioSource _audioSource;
    
    [Header("Damage Grunts")]
    [SerializeField] protected AudioClip[] damageGrunts;
    
    [Header("Attack Whoosh")]
    [SerializeField] protected AudioClip[] attackWhoosh;
    
    //[Header("Block Sounds")]

    protected virtual void Awake()
    { 
        _audioSource = GetComponent<AudioSource>();
    }

    public void Start()
    {
       
    }

    public void PlaySoundFX(AudioClip soundFX, float volume = 1.0f, bool randomizePitch = true, float pitchRandom = 0.1f)
    {
        _audioSource.PlayOneShot(soundFX, volume);
        _audioSource.pitch = 1;

        if (randomizePitch)
        {
            _audioSource.pitch += Random.Range(-pitchRandom, pitchRandom);
        }
    }
    public void PlayRollSoundFX()
    {
        _audioSource.PlayOneShot(WorldSoundFXManager.instance.rollingSFX);
    }

    public void PlayDamageGruntSoundFX()
    {
        PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSfxFromArray(damageGrunts));
    }

    public void PlayAttackWhooshSoundFX()
    {
        PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSfxFromArray(attackWhoosh));
    }

    public void PlayStanceBreakSoundFX()
    {
        _audioSource.PlayOneShot(WorldSoundFXManager.instance.stanceBreakSFX);
    }
    
    public virtual void PlayCriticallyStrikeSoundFX()
    {
        _audioSource.PlayOneShot(WorldSoundFXManager.instance.criticalStrikeSFX);
    }
    public virtual void PlayBlockSFX()
    {
        
    }

    
}
