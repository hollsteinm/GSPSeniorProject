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
    private long       score      = 0L;
    
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
    
    public void setScore(long score){
        this.score = score;
    }
    
    public long getScore(){
        return score;
    }
}