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
    private float[]     rotation        = new float[4];
    private float       damage          ;
    private float       speed           ;
    private float[]     position        = new float[3];
    private float       range           ;
    
    public Projectile(){
        rotation = new float[4];
        position = new float[3]; 
        range = 2000.0f;
        damage = 10.0f;
        speed = 1000.0f;
    }
    
    public Projectile(float speed, float range, float damage){
        rotation = new float[4];
        position = new float[3]; 
        this.range = range;
        this.speed = speed;
        this.damage = damage;
        
    }

    public float getRange() {
        return range;
    }

    public void setRange(float range) {
        this.range = range;
    }
    
    public float[] getPosition() {
        return position;
    }

    public void setPosition(float[] position) {
        this.position = position;
    }

    public float[] getRotation() {
        return rotation;
    }

    public void setRotation(float[] rotation) {
        this.rotation = rotation;
    }

    public float getDamage() {
        return damage;
    }

    public void setDamage(float damage) {
        this.damage = damage;
    }

    public float getSpeed() {
        return speed;
    }

    public void setSpeed(float speed) {
        this.speed = speed;
    }
}
