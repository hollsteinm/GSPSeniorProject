/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

package com.gspteama.gamedriver;

/**
 *
 * @author Martin
 */
public class Hull {
    private float health;
    private float maxHealth;
    private boolean showShield;
    private boolean showFire;
    private boolean showSparks;
    
    public Hull(float maxHealth){
        this.health = maxHealth;
        this.maxHealth = maxHealth;
        showShield = true;
        showFire = false;
        showSparks = false;
    }
    
    public float getHealth(){
        return health;
    }
    
    public void onDamage(float damage){
        health-=damage;
        onHealthChange();
    }
    
    public void onRepair(float repair){
        float estimated = repair + health;
        if(estimated <= maxHealth){
            health += repair;
        } else {
            health = maxHealth;
        }
        onHealthChange();
    }
    
    public boolean showSheild(){
        return showShield;
    }
    
    public boolean showFire(){
        return showFire;
    }
    
    public boolean showSparks(){
        return showSparks;
    }
    
    private void onHealthChange(){
        showShield = health > maxHealth * 0.5f;
        showFire = health < maxHealth * 0.1f;
        showSparks = health < maxHealth * 0.3f;
    }
    
    @Override
    public String toString(){
        String value = "";
        value += "[Hull]("+health+"/"+maxHealth+")\n";
        return value;
    }
    
}
