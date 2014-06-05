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
    
    public Movement     movement        ;
    
    public Projectile(float damage, float maxVelocity, float range){
        movement = new Movement;
        this.range = range;
        this.damage = damage;
        this.maxVelocity = maxVelocity;
    }

    public float getRange() {
        return range;
    }

    public float getDamage() {
        return damage;
    }
}
