/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.gamedriver;

import java.util.ArrayList;

/**
 *
 * @author Martin
 */
public class Ship {
    private float       position[]  = new float[]{0.0f, 0.0f, 0.0f};
    private float       rotation[]  = new float[]{0.0f, 0.0f, 0.0f, 0.0f};
    private float       eulerRotation[] = new float[]{0.0f, 0.0f, 0.0f};
    
    public float        velocity = 0.0f;
    public float        acceleration = 0.0f;
    
    protected static final float       maxVelocity = 1000.0f;
    
    private long                shipTypeID = 0;
    private String              shipTypeString = "";
    
    protected Weapon            weapon;
    protected Hull              hull;
    
    private long                otherShipIDIamCollidingWith;
    private long                otherProjectileIamCollidingWith;
    
    public Ship(long shipTypeID, String shipTypeString, Weapon weapon, Hull hull){
        weapon = new Weapon();
        position = new float[3];
        rotation = new float[4];
        eulerRotation = new float [3];
        this.health = health;
        this.weapon = weapon;
    }
    
    public boolean verifyCollisionShip(long otherShipID){
        return otherShipID == otherShipIDIamCollidingWith;
    }
    
    public boolean verifyCollisionProjectile(long projectileID){
        return otherShipID == projectileID;
    }
    
    public void setCollisionHandshakeShip(long otherShipID){
        otherSHipIDIamCollidingWith = otherShipIDIamCollidingWith;
    }
    
    public void setCollisionHandshakeProjectile(long otherProjectileID){
        otherProjectileIamCollidingWith = otherProjectileID;
    }
    
    public long getTypeID(){
        return shipTypeID;
    }
    
    public String getShipTypeString(){
        return shipTypeString;
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
    
    public float[] getEulerRotation(){
        return eulerRotation;
    }

    public void setRotation(float[] rotation) {
        this.rotation = rotation;
    }
    
    public void setEulerRotation(float[] eulerRotation){
        this.eulerRotation = eulerRotation;
    }
    
    public float setPosition(float x, float y, float z){
        this.position = new float[]{x, y, z};
    }
    
    public float setRotation(float x, float y, float z, float w){
        this.rotation = new float[]{x, y, z, w};
    }
    
    public float setEulerRotation(float x, float y, float z){
        this.eulerRotation = new float[]{x, y, z};
    }

    public Hull getHull(){
        return hull;
    }

    public Weapon getWeapon() {
        return weapon;
    }
    
}
