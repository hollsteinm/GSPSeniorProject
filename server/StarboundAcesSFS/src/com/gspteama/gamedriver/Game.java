/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.gamedriver;

import java.util.ArrayList;
import java.util.HashMap;

/**
 *
 * @author Martin
 */
public class Game {
    private HashMap<Integer, Player>    players     = new HashMap<>();
    private HashMap<Integer, Projectile> firedProjectiles = new HashMap<>();

    public void AddPlayer(Player player){
        if(!players.containsKey(player.getPlayerID()){
            players.put(player.getPlayerID, player);
        }
    }
    
    public void registerCollisionEvent(Player requester, Player other){
        
    }
    
    public void registerCollisionEvent(Player requester, Projectile other){
        
    }
    
    public void registerCollisionEvent(Player requester, IPowerup other){
        
    }
    
    public void registerCollisionEvent(Player requester, Obstacle Other){
        
    }
    
    public void registerInputEvent(Player request, String command){
        
    }
    
    
    
    
    public Player getPlayer(int playerId){
        return players.get(playerId);
    }
    
    //adds projecitle to game, returns its key in hashmap
    public void addProjectile(int id, Projectile p){
        firedProjectiles.put(id, p);
    }
    
    public Projectile getProjectile(int id){
        return firedProjectiles.get(id);
    }
}
