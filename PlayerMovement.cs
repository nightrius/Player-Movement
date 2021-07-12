using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Parameters")]
    public float speed = 10f;
    public float jumpVelocity = 20f;

    [Header("GameObjects")]
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    GameObject weapon; // Weapon that the player is equipping
    public GameObject firePoint;

    [Header("Scripts")]
    public SwitchWeapon switchWeapon;

    public bool isGrounded = true;
    float movement;
    Vector2 firepointPos; // Position of the shooting fire point
    Vector2 gunPos; // Position of the weapon Equip
    private bool doubleJump = false;
    float offset;
    float rotZ;

    public Vector2[] startingGunPos;
    public Vector2[] startingFirepointPos;

    void Start()
    {
        for (int i = 0; i < switchWeapon.transform.childCount; i++)
        {
            //Set the starting positions for the gun and firepoint position
            GameObject weapon = switchWeapon.weaponList.transform.GetChild(i).gameObject;
            startingGunPos[i] = weapon.transform.localPosition;
            startingFirepointPos[i] = weapon.transform.GetChild(0).localPosition;
        }
        SetWeaponObjects();
    }

    // Update is called once per frame
    void Update()
    {
        movement = Input.GetAxis("Horizontal");
        
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        
        WeaponRotation();
    }

    void FixedUpdate()
    {
        //Player Move
        rb.velocity = new Vector2(movement * speed, rb.velocity.y);
    }

    public void Jump()
    {
        if (isGrounded == true)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            isGrounded = false;
            doubleJump = true;
        }
        else if (doubleJump == true) 
        {
            //Double Jumping
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            doubleJump = false;
        }
    }

    public void SetWeaponObjects()
    {
        weapon = switchWeapon.weaponEquip;
        firePoint = weapon.transform.GetChild(0).gameObject;
        //Set the firepoint and gun position, to enable to rotate the sprite
        firepointPos = startingFirepointPos[switchWeapon.weaponID];
        gunPos = startingGunPos[switchWeapon.weaponID];
    }

    public void WeaponRotation()
    {
        //Rotate the sprite of the weapon to face the direction where mouse is
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        if (rotZ >= 90f || rotZ < -90f)
        {
            // Flip the gun sprite to the direction the mouse is facing
            weapon.GetComponent<SpriteRenderer>().flipY = true; 
            FacingLeft(); // flip the guns and firePoint position 
        }
        else
        {
            weapon.GetComponent<SpriteRenderer>().flipY = false;
            FacingRight();
        }
        //Rotate the weapon according to the mouse position
        weapon.transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);
    }

    public void FacingRight()
    {
        //Change the firepoint and gun position to the front of the gun after the gun sprite has been flip
        firePoint.transform.localPosition = new Vector2(firepointPos.x, firepointPos.y);
        weapon.transform.localPosition = new Vector2(gunPos.x, gunPos.y);
        sr.flipX = false;
    }

    public void FacingLeft()
    {
        //Change the firepoint and gun position to the front of the gun after the gun sprite has been flip
        firePoint.transform.localPosition = new Vector2(firepointPos.x, -firepointPos.y);
        weapon.transform.localPosition = new Vector2(-gunPos.x, gunPos.y);
        sr.flipX = true;
    }

    public IEnumerator Knockback(float duration, float force)
    {
        //Knockback effect of the player when touching a spike
        float timer = 0;
        while(duration > timer)
        {
            timer += Time.deltaTime;
            float x = this.transform.position.x * -100 * movement;
            float y = this.transform.position.y * -force * 2.5f;
            rb.AddForce(new Vector3(x , y, transform.position.z));
        }

        yield return 0;
    }
}
