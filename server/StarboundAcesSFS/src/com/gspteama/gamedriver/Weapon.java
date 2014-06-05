/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.gamedriver;

/**
 *
 * @author Martin
 */
public class Weapon {
    protected float       cooldown    = 0.0f;
    protected Projectile  projectile;
    
    private float         currentCooldown = 0.0f;
    
    public Weapon(float cooldown, Projectile projectile){
        this.cooldown = cooldown;
        this.projectile = projectile;
    }

    public float getCooldown() {
        return cooldown;
    }
    
    public void decrementCooldown(float value){
        currentCooldown -= value;
    }
    
    public bool canFire(){
        return currentCooldown <= 0.0f;
    }
    
    public Projectile onFire(){
        currentCooldown = cooldown;
        return projectile;
    }
    
}
