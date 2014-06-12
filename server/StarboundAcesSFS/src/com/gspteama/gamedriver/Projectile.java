/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.gamedriver;

/**
 *
 * @author Martin
 */
public class Projectile {
    private float       damage          ;
    private float       range           ;
    private long        projectileID    ;
    private String      projectileIDString;
    private long        owningPlayerID  ;
    
    public Movement     movement        ;
    
    public int getOwningPlayerId(){
        return (int)owningPlayerID;
    }
    
    public Projectile(long owningPlayerID, long projectileID, String projectileIDString, float damage, float maxVelocity, float range){
        movement = new Movement(maxVelocity);
        movement.Acceleration = 0.0f;
        movement.Velocity = maxVelocity;
        this.range = range;
        this.damage = damage;
        this.projectileID = projectileID;
        this.projectileIDString = projectileIDString;
        this.owningPlayerID = owningPlayerID;
    }
    
    public long getProjectileID(){
        return projectileID;
    }
    
    public String getProjectileStringID(){
        return projectileIDString;
    }

    public float getRange() {
        return range;
    }

    public float getDamage() {
        return damage;
    }
    
    public void update(float deltaTime){
        movement.onUpdate(deltaTime);
        
    }
    
    public String toString(){
        String value = "";
        value += "[Projectile[\""+ this.projectileIDString + "\"](";
        value += "Damage(" + this.damage + ")";
        value += "Range(" + this.range + "))]\n";
        return value;
    }
}
