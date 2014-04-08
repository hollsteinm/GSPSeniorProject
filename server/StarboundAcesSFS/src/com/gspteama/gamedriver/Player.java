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
    private float       score      = 0.0f;
    
    public Player(){
    }
    
    public Player(String username){
        this.username = username;
    }

    public String getUsername() {
        return username;
    }

    public void setUsername(String username) {
        this.username = username;
    }
    
    public void setScore(float score){
        this.score = score;
    }
    
    public float getScore(){
        return score;
    }
}