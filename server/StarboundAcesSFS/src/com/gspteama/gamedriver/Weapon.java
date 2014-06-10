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
    
    private int maxClipSize = 0;
    private int totalAmmo = 0;
    
    private float         currentCooldown = 0.0f;
    
    private int currentAmmo = 0;
    
    public Weapon(float cooldown, Projectile projectile, int maxClipSize, int totalAmmo){
        this.cooldown = cooldown;
        this.projectile = projectile;
        this.maxClipSize = maxClipSize;
        this.totalAmmo = totalAmmo;
    }

    public float getCooldown() {
        return cooldown;
    }
    
    public void decrementCooldown(float value){
        currentCooldown -= value;
    }
    
    public void OnUpdate(float deltaTime){
        decrementCooldown(deltaTime);
    }
    
    public void onReload(){
        if(canReload()){
            if(totalAmmo > (maxClipSize - currentAmmo)){
                totalAmmo -= (maxClipSize - currentAmmo);
                currentAmmo = maxClipSize;
            } else {
                currentAmmo += totalAmmo;
                totalAmmo = 0;
            }
        }
    }
    
    private boolean canReload(){
        return currentAmmo < maxClipSize && totalAmmo > 0;
    }
    
    public boolean canFire(){
        return currentCooldown <= 0.0f && currentAmmo > 0;
    }
    
    public Projectile onFire() throws Exception{
        currentCooldown = cooldown;
        currentAmmo--;
        return (Projectile)projectile.getClass().newInstance();
    }
    
}
