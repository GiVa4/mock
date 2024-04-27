using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] Image life;
    [SerializeField] float health = 100;
    [SerializeField] float maxHealth = 100;
    [SerializeField] GameObject hitVFX;
    [SerializeField] GameObject ragdoll;
    public AudioSource getLife;
    public AudioSource damage;
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        life.fillAmount = health / maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        damage.Play();
        health -= damageAmount;
        animator.SetTrigger("damage");
        
        if (health <= 0)
        {
            //Die();
            //Carga la escena de Game Over
            SceneManager.LoadScene(4);
        }
    }

    void Die()
    {
        Instantiate(ragdoll, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
    public void HitVFX(Vector3 hitPosition)
    {
        GameObject hit = Instantiate(hitVFX, hitPosition, Quaternion.identity);
        Destroy(hit, 3f);

    }

    //Aumnetar Vida
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("life"))
        {
            getLife.Play();
            health += 2;
            Destroy(other.gameObject);
        }
    }
}
