﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour, SpaceShipInterface {

    public void RotateLeft() { _rotateLeftIntention = true; }
    public void RotateRight() { _rotateRightIntention = true; }
    public void Accelerate() { _accelerateIntention = true; }
    public void SlowDown() { _slowDownIntention = true; }
    public void Shot() { _shotIntention = true; }

    Rigidbody rb;
    [SerializeField] Transform gun;
    [SerializeField] Transform aim;
    [SerializeField] Transform center;
    [SerializeField] Weapon weapon;
    [SerializeField] FireEngine engineFire;
    public Projectile pl;

    bool _rotateLeftIntention = false;
    bool _rotateRightIntention = false;
    bool _accelerateIntention = false;
    bool _slowDownIntention = false;
    bool _shotIntention = false;
    int _hp = 100;

    SpaceShipInfo info = new SpaceShipInfo();

    public string Name;
    
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        OnSpawn();
        SetOwnerName(Name);
	}

    public void OnSpawn()
    {
        weapon.type = GameParams.ProjectileType.Basic;
        _hp = GameParams.SPACESHIP_MAX_HP;
        GameContext.Instance.ships.Add(info);
    }

    public int Damage(int dmg)
    {
        _hp -= dmg;

        if (_hp > GameParams.SPACESHIP_MAX_HP)
        {
            _hp = GameParams.SPACESHIP_MAX_HP;
        }

        if (_hp <= 0)
        {
            OnDestroy();
            //Destroy(this.gameObject);
        }
        return _hp;
    }

    void OnDestroy()
    {
        var idx = GameContext.Instance.ships.FindIndex(s => s.ownerName == GetOwner());
        if (idx >= 0)
        {
            GameContext.Instance.ships.RemoveAt(idx);
        }
        GameManager.Instance.InitializeRespawn(this);
        StopAllCoroutines();
        this.gameObject.SetActive(false);
    }

    public void SetOwnerName(string name)
    {
        info.ownerName = name;
        weapon.ownerName = name;
    }

    public int GetHP()
    {
        return _hp;
    }
    Vector3 _lastAcceleration;
	void FixedUpdate ()
    {
        bool enableEngine = false;
        _lastAcceleration = Vector3.zero;
        if (_rotateLeftIntention)
        {
            transform.Rotate(new Vector3(0,-1 * GameParams.SPACESHIP_ROTATION_SPEED,0));
        }

        if (_rotateRightIntention)
        {
            transform.Rotate(new Vector3(0, 1 * GameParams.SPACESHIP_ROTATION_SPEED, 0));
        }

        if (_accelerateIntention)
        {
            var vel = (aim.position - center.position).normalized * GameParams.SPACESHIP_ACCELERATION;
            vel = CheckForce(vel);
            rb.AddForce(vel);
            _lastAcceleration = vel;
            enableEngine = true;
            //rb.velocity.Set(vel.x, vel.y, vel.z);
        }

        if (_shotIntention)
        {
            weapon.Shot();
        }
        ResetIntentions();
        engineFire.SetActive(enableEngine);
    }

    void Update()
    {
        UpdateInfo();
    }

    public string GetOwner()
    {
        return Name;
    }

    void UpdateInfo()
    {
        info.position = transform.position;
        info.rotation = transform.rotation;
        info.acceleration = _lastAcceleration;
        info.weapon = weapon.GetWeaponType();
        info.health = _hp;
    }
    

    public Vector3 CheckForce(Vector3 accel)
    {
        var vel = rb.velocity;
        float limit = GameParams.SPACESHIP_MAX_SPEED;
        //accel.x = Mathf.Abs(vel.x)
        accel.x = Mathf.Clamp((vel.x + accel.x), -limit, limit) - vel.x;
        accel.y = Mathf.Clamp((vel.y + accel.y), -limit, limit) - vel.y;
        accel.z = Mathf.Clamp((vel.z + accel.z), -limit, limit) - vel.z;
        rb.velocity = vel;
        return accel;
    }

    void ResetIntentions()
    {
        _rotateLeftIntention = false;
        _rotateRightIntention = false;
        _accelerateIntention = false;
        _slowDownIntention = false;
        _shotIntention = false;
    }
}
