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
    
    
    public Player getPlayer(int playerId){
        return players.get(playerId);
    }
    
    public Ship getShip(int playerId){
        return ships.get(playerId);
    }
    
    //adds projecitle to game, returns its key in hashmap
    public void addProjectile(int id, Projectile p){
        projectiles.put(id, p);
    }
    
    public Projectile getProjectile(int id){
        return projectiles.get(id);
    }
}
