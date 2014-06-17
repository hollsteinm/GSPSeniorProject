/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.gamedriver;

/**
 *
 * @author Martin
 */
public class Player {
    private String      username   = "";
    private long        score      = 0L;
    private int         id         ;
    
    private Ship        ship        ;
    
    public void update(float deltaTime){
        ship.onUpdate(deltaTime);
    }
    
    public Player(int id, String username, Ship ship){
        this.id = id;
        this.username = username;
        this.ship = ship;
    }
    
    public int getPlayerId(){
        return id;
    }

    public String getUsername() {
        return username;
    }
    
    public void updateScore(long score){
        this.score += score;
    }

    public long getScore(){
        return score;
    }
    
    public Ship getShip(){
        return ship;
    }
}
