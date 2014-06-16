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
    
    public Projectile(Projectile other){
        movement = new Movement(other.movement.getMaxVelocity());
        movement.Acceleration = 0.0f;
        movement.Velocity = this.movement.getMaxVelocity();
        range = other.range;
        damage = other.damage;
        projectileID = other.projectileID;
        projectileIDString = other.projectileIDString;
        owningPlayerID = other.owningPlayerID;
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
    
    public boolean expired(){
        float[] current = movement.getPosition();
        float[] spawn = movement.getSpawn();
        return Movement.distance(
                spawn[0], spawn[1], spawn[2], 
                current[0], current[1], current[2]) > range;
    }
    
    @Override
    public String toString(){
        String value = "";
        value += "[Projectile[\""+ this.projectileIDString + "\"](";
        value += "Damage(" + this.damage + ")";
        value += "Range(" + this.range + "))]\n";
        return value;
    }
}
