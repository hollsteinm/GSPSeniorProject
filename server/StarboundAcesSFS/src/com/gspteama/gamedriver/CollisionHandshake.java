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

public class CollisionHandshake {
    
    public enum ColliderType{
        PLAYER,
        OBSTACLE,
        PROJECTILE,
    }
    
    private ColliderType typeICollideWith;
    private int idICollideWith;
    
    private ColliderType type;
    private int id;

    public CollisionHandshake(ColliderType type, int id){
        this.type = type;
        this.id = id;
    }
    
    public void onCollide(ColliderType othertype, int otherId){
        typeICollideWith = othertype;
        idICollideWith = otherId;        
    }
    
    public int getId(){
        return id;
    }
    
    public ColliderType getType(){
        return type;
    }
    
    public boolean qualifiedCollision(CollisionHandshake other){
        return other.getType() == typeICollideWith && other.getId() == idICollideWith;
    }
    
    public void clear(){
        //invalidate the data
        type = null;
        id = -1;
    }
}
