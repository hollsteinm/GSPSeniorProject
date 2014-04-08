/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.extensions;

import com.gspteama.gamedriver.Game;
import com.gspteama.gamedriver.Player;
import com.gspteama.gamedriver.Ship;
import com.gspteama.gamedriver.Weapon;
import com.gspteama.main.StarboundAcesExtension;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import com.smartfoxserver.v2.extensions.SFSExtension;
import java.util.ArrayList;
import java.util.logging.Level;
import java.util.logging.Logger;


/**
 *
 * @author Martin
 */
@com.smartfoxserver.v2.annotations.MultiHandler
public class MultiHandler extends BaseClientRequestHandler{
    
    @Override
    public void handleClientRequest(User user, ISFSObject params){
        String reqId = params.getUtfString(SFSExtension.MULTIHANDLER_REQUEST_ID);
        
        switch(reqId){
            case "transform":
                handleTransform(user, params);
                break;
                
            case "spawn":
                handleSpawn(user, params);
                break;
                
            case "fire":
                trace("Player <" + user.getId() + "> is firing at <" +
                        params.getInt("player.hit.id"));
                handleFire(user, params);
                break;
                
            case "death":
                trace("Player <" + user.getId() + "> is dead.");
                handleDeath(user, params);
                break;

            default:
                trace("Unrecognized request Id sent... ignoring");
                break;
        }
    }
    
    private void handleSpawn(User user, ISFSObject params){
        try {
            trace("handling spawn");
            Game game = ((StarboundAcesExtension)this.getParentExtension()).getGame(user.getLastJoinedRoom().getId());
            int playerid = user.getId();
            
            //TODO: load ship data from database of ship configurations
            ArrayList<Weapon> weapons = new ArrayList<Weapon>();
            weapons.add(new Weapon(25.0f, 1.0f));
            game.AddShip(playerid, new Ship(100.0f, weapons));
            
            Ship ship = game.getShip(playerid);
            //trace(ship.toString());
            
            int size = user.getLastJoinedRoom().getUserList().size();
            ship.setPosition( new float[]{ 2.0f * size, 2.0f * size, 0.0f } );
            ISFSObject response = SFSObject.newInstance();
            response.putInt("player", playerid);
            response.putFloat("position.x", ship.getPosition()[0]);
            response.putFloat("position.y", ship.getPosition()[1]);
            response.putFloat("position.z", ship.getPosition()[2]);
            response.putFloat("rotation.x", ship.getRotation()[0]);
            response.putFloat("rotation.y", ship.getRotation()[1]);
            response.putFloat("rotation.z", ship.getRotation()[2]);  
            response.putFloat("rotation.w", ship.getRotation()[3]);
            response.putFloat("health", ship.getHealth());
            
            int numWeapons = ship.getWeapons().size();
            ArrayList<Float> cooldowns = new ArrayList<Float>();
            ArrayList<Float> damages = new ArrayList<Float>();
            for(int i = 0; i < numWeapons; ++i){
                cooldowns.add(ship.getWeapons().get(i).getCooldown());
                damages.add(ship.getWeapons().get(i).getDamage());
            }
            
            response.putInt("numWeapons", numWeapons);
            response.putFloatArray("cooldowns", cooldowns);
            response.putFloatArray("damages", damages);               

            //this.send("transform", response, user.getLastJoinedRoom().getPlayersList(), true);
            send("spawn", response, user.getLastJoinedRoom().getPlayersList());
        } catch (Exception ex) {
            Logger.getLogger(MultiHandler.class.getName()).log(Level.SEVERE, null, ex);
        }

    }
    
    private void handleTransform(User user, ISFSObject params){
        
        try{
            Game game = ((StarboundAcesExtension)this.getParentExtension()).getGame(user.getLastJoinedRoom().getId());
            Ship ship = game.getShip(user.getId());
            
            ship.setPosition(new float[]{
                params.getFloat("position.x"),
                params.getFloat("position.y"),
                params.getFloat("position.z")
            });
            
            ship.setRotation(new float[]{
                params.getFloat("rotation.x"),
                params.getFloat("rotation.y"),
                params.getFloat("rotation.z"),
                params.getFloat("rotation.w")
            });

        } catch (Exception e){
            e.printStackTrace();
        } finally {
        
            ISFSObject response = SFSObject.newInstance();
            response.putInt("player", user.getId());
            response.putFloat("position.x", params.getFloat("position.x"));
            response.putFloat("position.y", params.getFloat("position.y"));
            response.putFloat("position.z", params.getFloat("position.z"));
            response.putFloat("rotation.x", params.getFloat("rotation.x"));
            response.putFloat("rotation.y", params.getFloat("rotation.y"));
            response.putFloat("rotation.z", params.getFloat("rotation.z"));  
            response.putFloat("rotation.w", params.getFloat("rotation.w"));

            //this.send("transform", response, user.getLastJoinedRoom().getPlayersList(), true);
            send("transform", response, user.getLastJoinedRoom().getPlayersList());
        }
    }
    
    private void handleFire(User user, ISFSObject params){

        //TODO: verify the damage, in fact - just pull it from the ship instance
        //for the player in question
        ISFSObject response = SFSObject.newInstance();
        response.putFloat("damage", params.getFloat("damage"));
        response.putInt("player.hit.id", params.getInt("player.hit.id"));
        response.putFloat("contact.point.x", params.getFloat("contact.point.x"));
        response.putFloat("contact.point.y", params.getFloat("contact.point.y"));
        response.putFloat("contact.point.z", params.getFloat("contact.point.z"));
        
        send("player.hit", response, user.getLastJoinedRoom().getPlayersList());
        
        //Update scores
        try {
            Game game = ((StarboundAcesExtension)this.getParentExtension()).getGame(user.getLastJoinedRoom().getId());
            Player player = game.getPlayer(user.getId());
            Ship other = game.getShip(params.getInt("player.hit.id"));
            other.setHealth(other.getHealth() - params.getFloat("damage"));
            
            //TODO: remove magic numbers for scores
            float score;
            if(game.getShip(params.getInt("player.hit.id")).getHealth() > 0.0f){
                score= player.getScore() + 5.0f;
            } else {
                score = player.getScore() + 25.0f;
            }
            player.setScore(score);
        } catch (Exception ex) {
            Logger.getLogger(MultiHandler.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
    
    private void handleDeath(User user, ISFSObject params){
        ISFSObject response = SFSObject.newInstance();
        response.putInt("id", user.getId());
        
        send("death", response, user.getLastJoinedRoom().getPlayersList());
    }
}
