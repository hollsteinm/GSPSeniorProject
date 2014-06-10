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
    public Movement             movement;

    private float               maxEnergy;
    private float               currentEnergy;
    private long                shipTypeID = 0;
    private String              shipTypeString = "";
    
    protected Weapon            weapon;
    protected Hull              hull;
    
    private long                owningPlayerID;
    
    public Ship(long owningPlayerID, long shipTypeID, String shipTypeString, 
            Weapon weapon, Hull hull, float maxVelocity, float maxEnergy){
        
        movement = new Movement(maxVelocity);
        this.hull = hull;
        this.weapon = weapon;
        this.shipTypeID = shipTypeID;
        this.shipTypeString = shipTypeString;
        this.owningPlayerID = owningPlayerID;
        this.maxEnergy = maxEnergy;
        this.currentEnergy = maxEnergy;
    }
    
    public void onUpdate(float deltaTime){
        movement.onUpdate(deltaTime);
        weapon.onUpdate(deltaTime);
        restoreEnergy(deltaTime / 2.0f);
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
    
    public void useEnergy(float value){
        currentEnergy -= value;
    }
    
    public void restoreEnergy(float value){
        currentEnergy += value;
        if(currentEnergy >= maxEnergy){
            currentEnergy = maxEnergy;
        }
    }
    
    public boolean canHasEnoughEnergy(float amountToConsume){
        return amountToConsume <= currentEnergy;
    }
    
}
