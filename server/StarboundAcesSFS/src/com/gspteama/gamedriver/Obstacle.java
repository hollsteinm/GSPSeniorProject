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
public class Obstacle {
    private String nameId;
    private long id;
    
    public Movement movement;
    
    public Obstacle(String nameId, long id, float maxVelocity){
        this.nameId = nameId;
        this.id = id;
        movement = new Movement(maxVelocity);        
    }
    
}
