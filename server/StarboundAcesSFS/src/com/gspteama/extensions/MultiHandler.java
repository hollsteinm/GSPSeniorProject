/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.extensions;

import com.gspteama.db.DBService;
import com.gspteama.gamedriver.Game;
import com.gspteama.gamedriver.Projectile;
import com.gspteama.gamedriver.Ship;
import com.gspteama.gamedriver.Weapon;
import com.gspteama.main.StarboundAcesExtension;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import com.smartfoxserver.v2.extensions.SFSExtension;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.HashMap;


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
                
            case "gameslist":
                handleGameList(user, params);
                break;
                
            case "spawn":
                handleSpawn(user, params);
                break;
                
            case "fire":
                trace("Player <" + user.getId() + "> is firing at <" +
                        params.getInt("player.hit.id"));
                handleFire(user, params);
                break;
                
            case "shoot":
                handleShoot(user, params);
                break;
                
            case "death":
                trace("Player <" + user.getId() + "> is dead.");
                handleDeath(user, params);
                break;
                
            case "scores":
                trace("Score event requested");
                handleScores(user, params);
                break;
                
            case "gamestart":
                handleGameStart(user, params);
                break;

            default:
                trace("Unrecognized request Id sent... ignoring");
                break;
        }
    }
    
    private void handleGameStart(User user, ISFSObject params){
        try{
            DBService.updateGameStatus(this.getParentExtension().getParentZone().getDBManager().getConnection(),
                    "A", user.getLastJoinedRoom().getName());
                
            ISFSObject data = SFSObject.newInstance();
            data.putUtfString("game", user.getLastJoinedRoom().getName());
            send("gamelist.remove", data, this.getParentExtension().getParentZone().getRoomByName("lobby").getUserList());
                
        } catch(SQLException e){
            onException(e);
        } finally{
            send("game.start", SFSObject.newInstance(), user.getLastJoinedRoom().getUserList());
        }      
        
    }
    
    private void handleGameList(User user, ISFSObject params){
        try{
            ArrayList<String> games = DBService.getQueuedGames(
                this.getParentExtension().getParentZone().getDBManager().getConnection());
            if(games.size() > 0){
                    
                ISFSObject data = SFSObject.newInstance();                    
                data.putUtfStringArray("games", games);
                    
                send("gamelist", data, user);
                trace("Sending gamelist: " + data.toString());
            }         
                
        } catch(Exception e){
            onException(e);
        }
    }
    
    private void handleShoot(User user, ISFSObject params){
        int id = 0;
        Game game = getGame(user);
        Projectile p = null;
        java.util.Random r = new java.util.Random();
        
        trace("new projectile instantiated");
        try{
            p = DBService.selectProjectile(
                    this.getParentExtension().getParentZone().getDBManager().getConnection(),
                    params.getUtfString("type"));
            
            p.setPosition(new float[]{
                params.getFloat("position.x"),
                params.getFloat("position.y"),
                params.getFloat("position.z")
            });
            
            p.setRotation(new float[]{
                params.getFloat("rotation.x"),
                params.getFloat("rotation.y"),
                params.getFloat("rotation.z"),
                params.getFloat("rotation.w")
            });
            
            id = r.nextInt();
            game.addProjectile(id, p);
            
            trace("Properties set");
        } catch(Exception e){
            onException(e);
        } finally{
            ISFSObject response = SFSObject.newInstance();
            
            if(id == 0){
                id = r.nextInt();
            }
            
            try{
                if(p == null){
                    p = DBService.selectProjectile(
                        this.getParentExtension().getParentZone().getDBManager().getConnection(),
                        params.getUtfString("type"));
                }
            } catch(SQLException e){
                    onException(e);
                    p = new Projectile();
            }
            
            response.putInt("playerId", user.getId());
            response.putInt("networkId", id);
            
            paramsIntoResponseTransform(params, response);
            
            response.putFloat("damage", p.getDamage());
            response.putFloat("speed", p.getSpeed());
            response.putFloat("range", p.getRange());
            
            game.addProjectile(id, p);
            trace(response.getDump());
            send("shoot", response, user.getLastJoinedRoom().getUserList());
            trace("data sent");
        }
    }
    
    private void handleProjectileTransform(User user, ISFSObject params){
        int pid = params.getInt("networkId");
        
         try{
            Game game = getGame(user);
            Projectile p = game.getProjectile(pid);
            
            p.setPosition(new float[]{
                params.getFloat("position.x"),
                params.getFloat("position.y"),
                params.getFloat("position.z")
            });
            
            p.setRotation(new float[]{
                params.getFloat("rotation.x"),
                params.getFloat("rotation.y"),
                params.getFloat("rotation.z"),
                params.getFloat("rotation.w")
            });

        } catch (Exception e){
            onException(e);
        } finally {
        
            ISFSObject response = SFSObject.newInstance();            
            
            response.putInt("networkId", pid);
            response.putUtfString("type", "projectile");
            
            paramsIntoResponseTransform(params, response);
            
            //this.send("transform", response, user.getLastJoinedRoom().getPlayersList(), true);
            trace(response.getDump());
            send("transform", response, user.getLastJoinedRoom().getPlayersList());
        }
    }
    
    private void paramsIntoResponseTransform(ISFSObject params, ISFSObject response){
        try{
            response.putFloat("position.x", params.getFloat("position.x"));
            response.putFloat("position.y", params.getFloat("position.y"));
            response.putFloat("position.z", params.getFloat("position.z"));
            response.putFloat("rotation.x", params.getFloat("rotation.x"));
            response.putFloat("rotation.y", params.getFloat("rotation.y"));
            response.putFloat("rotation.z", params.getFloat("rotation.z"));  
            response.putFloat("rotation.w", params.getFloat("rotation.w"));
        }catch(Exception e){
            onException(e);
        }
    }
    
    private void handleScores(User user, ISFSObject params){
        ArrayList results;
        long playerScore = 0L;
        ISFSObject response = SFSObject.newInstance();
        
        try{
            results = DBService.selectTopTenScores(this.getParentExtension().getParentZone().getDBManager().getConnection());
            
            long userid = DBService.userIdFromUsername(this.getParentExtension().getParentZone().getDBManager().getConnection()
                    , user.getName());
            playerScore = DBService.selectUserScore(this.getParentExtension().getParentZone().getDBManager().getConnection()
                    , userid);
            
            trace(results.toString());
            trace(Long.toString(userid));
            trace(Long.toString(playerScore));
            
            //TODO: get out of having to do this loop oddly
            int size = results.size();
            int rank = 0;
            for(int i = 0; i < size; i+=2){
                response.putUtfString("player"+Integer.toString(rank), results.get(i).toString());
                response.putLong("score"+Integer.toString(rank), (long)results.get(i+1));
                rank++;
            }
            
            size /= 2;
            response.putInt("size", size);
            
            response.putLong("my.score", playerScore);
            send("scores", response, user);
            
        }catch(SQLException e){
            onException(e);
        }      
    }
    
    private Game getGame(User user){
        Game game = null;
        try{
            game = ((StarboundAcesExtension)this.getParentExtension()).getGame(user.getLastJoinedRoom().getId());
            return game;
        }catch(Exception e){
            onException(e);
        } finally{
            return game;
        }
    }
    
    private void handleSpawn(User user, ISFSObject params){
        try {
            trace("handling spawn");
            Game game = getGame(user);
            int playerid = user.getId();
            trace(params.getUtfString("weaponType"));
            
            HashMap<String, Object> weaponConfig = 
                    DBService.selectWeaponConfigurations(
                            this.getParentExtension().getParentZone().getDBManager().getConnection(),
                            params.getUtfString("weaponType"));
            
            Weapon w = (Weapon)weaponConfig.get("Weapon");
            game.AddShip(playerid, new Ship(300.0f, w));
            
            Ship ship = game.getShip(playerid);
            
            ship.setPosition( new float[]{ (100.0f * playerid) % 100.0f, (100.0f * playerid) % 100.0f, (100.0f * playerid) % 100.0f } );
            
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
            response.putFloat("cooldown", ship.getWeapon().getCooldown());
            response.putFloat("damage", ship.getWeapon().getDamage());         
                          

            trace(response.getDump());
            send("spawn", response, user.getLastJoinedRoom().getPlayersList());
        } catch (Exception ex) {
            onException(ex);
        }

    }
    
    private void handleTransform(User user, ISFSObject params){
        
        try{
            switch(params.getUtfString("type")){
                case "player":
                    handlePlayerTransform(user, params);
                    break;

                case "projectile":
                    handleProjectileTransform(user, params);
                    break;

                default:
                    trace("Unrecognized type for transform");
                    break;
            }
        }catch(Exception e){
            onException(e);
        }
    }
    
    private void handlePlayerTransform(User user, ISFSObject params){
        try{
            Game game = getGame(user);
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
            onException(e);
        } finally {
        
            ISFSObject response = SFSObject.newInstance();
            response.putInt("player", user.getId());
            response.putUtfString("type", "player");
            paramsIntoResponseTransform(params, response);

            //this.send("transform", response, user.getLastJoinedRoom().getPlayersList(), true);
            send("transform", response, user.getLastJoinedRoom().getPlayersList());
        }
    }
    
    private void handleFire(User user, ISFSObject params){
        try{
            Projectile p = null;
            try{
                if(p == null){
                    p = DBService.selectProjectile(
                        this.getParentExtension().getParentZone().getDBManager().getConnection(),
                        params.getUtfString("type"));
                }
            } catch(SQLException e){
                    onException(e);
                    p = new Projectile();
            }            
            
            ISFSObject response = SFSObject.newInstance();
            response.putFloat("damage", p.getDamage());
            response.putInt("player.hit.id", params.getInt("player.hit.id"));
            response.putFloat("contact.point.x", params.getFloat("contact.point.x"));
            response.putFloat("contact.point.y", params.getFloat("contact.point.y"));
            response.putFloat("contact.point.z", params.getFloat("contact.point.z"));

            trace(response.getDump());

            //Game game = getGame(user);
            //Ship other = game.getShip(params.getInt("player.hit.id"));
            //other.setHealth(other.getHealth() - params.getFloat("damage"));

            //TODO: remove magic numbers for scores
            long score = 25;

            long userid = DBService.userIdFromUsername(this.getParentExtension().getParentZone().getDBManager().getConnection()
                    , user.getName());
            long playerScore = DBService.selectUserScore(this.getParentExtension().getParentZone().getDBManager().getConnection()
                        , userid);
            
            trace(Long.toString(userid));
            trace(Long.toString(score));
            playerScore += score;
            trace(Long.toString(playerScore));

            //Update scores
            if(!user.getName().startsWith("AceGuest#")){
                trace("updating score for: " + user.getName());
                DBService.updatePlayerScore(this.getParentExtension().getParentZone().getDBManager().getConnection(),
                        playerScore, userid);
                trace(Long.toString(playerScore));
            }
            
            send("player.hit", response, user.getLastJoinedRoom().getPlayersList());
            
        }catch(Exception e){
            onException(e);
        }

    }
    
    private void handleDeath(User user, ISFSObject params){
        ISFSObject response = SFSObject.newInstance();
        
        if(!params.containsKey("networkId")){
            response.putInt("id", user.getId());        
            send("death", response, user.getLastJoinedRoom().getPlayersList());
        } else {
            response.putInt("networkId", params.getInt("networkId"));
            send("projectile.expire", response, user.getLastJoinedRoom().getPlayersList());
        }
    }
    
    private void onException(Exception e){
        String error = e.getMessage();
        error += "\n";
        for(StackTraceElement ee : e.getStackTrace()){
            error+=ee.toString();
            error+="\n";
        }
        trace(error);
    }
}
