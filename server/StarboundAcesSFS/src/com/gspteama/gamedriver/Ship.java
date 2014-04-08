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
    
    protected float             health      = 0.0f;
    protected ArrayList<Weapon> weapons     = new ArrayList<Weapon>();
    
    public Ship(float health, ArrayList<Weapon> weapons){
        this.health = health;
        this.weapons = weapons;
    }
    
    public Ship(float health){
        this.health = health;
    }
    
    public Ship(){
        
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

    public float getHealth() {
        return health;
    }

    public void setHealth(float health) {
        this.health = health;
    }

    public ArrayList<Weapon> getWeapons() {
        return weapons;
    }
    
}
