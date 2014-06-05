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
    public Movement     movement;
    public float        velocity = 0.0f;
    public float        acceleration = 0.0f;
    
    private long                shipTypeID = 0;
    private String              shipTypeString = "";
    
    protected Weapon            weapon;
    protected Hull              hull;
    
    private long                owningPlayerID;
    
    public Ship(long owningPlayerID, long shipTypeID, String shipTypeString, Weapon weapon, Hull hull, float maxVelocity){
        movement = new Movement(maxVelocity);
        this.hull = hull;
        this.weapon = weapon;
        this.shipTypeID = shipTypeID;
        this.shipTypeString = shipTypeString;
        this.owningPlayerID = owningPlayerID;
    }
    
    public long getOwningPlayerID(){
        return owningPlayerID;
    }
    
    public long getTypeID(){
        return shipTypeID;
    }
    
    public String getShipTypeString(){
        return shipTypeString;
    }

    public Hull getHull(){
        return hull;
    }

    public Weapon getWeapon() {
        return weapon;
    }
    
}
