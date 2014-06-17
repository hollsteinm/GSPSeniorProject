package com.gspteama.extensions;

import com.gspteama.db.DBService;
import com.gspteama.gamedriver.Game;
import com.gspteama.gamedriver.Hull;
import com.gspteama.gamedriver.IEventListener;
import com.gspteama.gamedriver.Player;
import com.gspteama.gamedriver.Projectile;
import com.gspteama.gamedriver.Ship;
import com.gspteama.gamedriver.Weapon;
import com.gspteama.main.StarboundAcesExtension;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import com.smartfoxserver.v2.extensions.SFSExtension;
import java.sql.Connection;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;


/**
*
* @author Martin
*/
@com.smartfoxserver.v2.annotations.MultiHandler
public class SAMultiHandler extends BaseClientRequestHandler implements IEventListener{
    private int calls = 0;
    @Override
    public void handleClientRequest(User user, ISFSObject params){
        calls++;
        trace("Calls: "+ calls);
      String reqId = params.getUtfString(SFSExtension.MULTIHANDLER_REQUEST_ID);
        
        /*debugging*/trace(reqId);
        /*debugging*/trace(params.getDump());
        switch(reqId){
          
          case "sa_input":
            handleInputRequest(user, params);
            break;
            
          case "sa_collision":
            handleCollisionRequest(user, params);
            break;
            
          case "scores":
            handleScoresRequest(user, params);
            break;
            
          case "gameslist":
            handleGameListRequest(user, params);
            break;
            
          case "gamestart":
            handleGameStartRequest(user, params);
            break;
          
          default:
              trace("Unhandled RequestID: " + reqId);
              break;
        }
    }
    
    //Helpers
    private Connection getConnection() throws SQLException{
        return this.getParentExtension().getParentZone().getDBManager().getConnection();
    }
    
    private void sendToAllInGame(String event, ISFSObject response, User user, boolean udp){
        trace(response.getDump());
        send(event, response, user.getLastJoinedRoom().getPlayersList(), udp);
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
    
    private Game getGame(User user) throws Exception{
        return ((StarboundAcesExtension)this.getParentExtension()).getGame(user.getLastJoinedRoom().getId());
    }
    
    User getUser(int id) throws Exception{
        User user = this.getParentExtension().getParentZone().getUserById(id);
        if(user == null){
            throw new Exception("Null User.");
        } else {
            return user;
        }
    }
    //End Helpers
    
    private void handleGameStartRequest(User user, ISFSObject params){
        try{
            DBService.updateGameStatus(getConnection(),
                    "A", user.getLastJoinedRoom().getName());
                
            ISFSObject data = SFSObject.newInstance();
            data.putUtfString("game", user.getLastJoinedRoom().getName());
            
            List<User> users = user.getLastJoinedRoom().getUserList();
            for(User u : users){
                long uid = (long)u.getId();
                trace("User Variables ["+uid+"]["+u.getName()+"]\n " + u.getUserVariablesData().getDump());
                
                HashMap<String, Object> shiphull = DBService.selectShipConfiguration(
                        getConnection(),
                        (long)u.getVariable("shipid").getIntValue());
                
                Weapon w = DBService.selectWeaponConfigurations(getConnection(), 
                        u.getVariable("weapon").getStringValue(), 
                        uid);
                
                //ship parameters
                long shipid = (long)u.getVariable("shipid").getIntValue();
                String name = (String)shiphull.get("name");
                Hull shipHull = (Hull)shiphull.get("hull");
                float maxVelocity = new Long((long)shiphull.get("maxVelocity")).floatValue();
                float maxEnergy = new Long((long)shiphull.get("maxEnergy")).floatValue();
                Ship s = new Ship( uid,
                        shipid,
                        name,
                        w,
                        shipHull,
                        maxVelocity,
                        maxEnergy
                );
                
                //getGame(user).AddPlayer(new Player(uid, u.getName(),s));
                trace("Player ready!\nPlayer Name [" + u.getName() + "]\nShip[" + 
                        s.toString() + "]\n");
            }
            
            getGame(user).initialize();
            send("gamelist.remove", data, this.getParentExtension().getParentZone().getRoomByName("lobby").getUserList());
            
        } catch(Exception e){
            onException(e);
        } finally{
            sendToAllInGame("game.start", SFSObject.newInstance(), user, false);
            
            try{
                ((StarboundAcesExtension)this.getParentExtension()).startGame(user.getLastJoinedRoom().getId());
                getGame(user).Register(this);
            } catch (Exception e){
                onException(e);
            }
        } 
    }
    
    private void handleScoresRequest(User user, ISFSObject params){
        ArrayList results;
        long playerScore = 0L;
        ISFSObject response = SFSObject.newInstance();
        
        try{
            results = DBService.selectTopTenScores(getConnection());
            
            long userid = DBService.userIdFromUsername(getConnection(),
                    user.getName());
            playerScore = DBService.selectUserScore(getConnection(),
                    userid);
            
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
    
    private void handleGameListRequest(User user, ISFSObject params){
        try{
            ArrayList<String> games = DBService.getQueuedGames(
                getConnection());
            if(games.size() > 0){
                    
                ISFSObject data = SFSObject.newInstance();
                data.putUtfStringArray("games", games);
                    
                send("gamelist", data, user);
            }
                
        } catch(Exception e){
            onException(e);
        }
        
    }
    
    private void handleInputRequest(User user, ISFSObject params){
        try{            
            getGame(user).registerInputEvent(
                    getGame(user).getPlayer(user.getId()),
                    params.getUtfString("command"),
                    params.getFloat("value"));
        }catch(Exception e){
            onException(e);
        }        
    }
    
    private void handleCollisionRequest(User user, ISFSObject params){
        try{
            
        }catch(Exception e){
            onException(e);
        }
    }

    @Override
    public void Notify(String event, Object data) {
        trace("Game Event: " + event);
        
        try{
            User user = null;
            ISFSObject sfs = SFSObject.newInstance();
            
            switch(event){
                case "projectile.spawn":
                    Projectile ref = (Projectile)data;
                    sfs.putInt("networkid", ref.hashCode());
                    sfs.putUtfString("typename", ref.getProjectileStringID());
                    user = getUser(ref.getOwningPlayerId());

                    sendToAllInGame("projectile.spawn", sfs, user, false);                
                    break;

                case "projectile.update":
                    HashMap<String, Object> jdata = (HashMap<String, Object>)data;
                    user = getUser((int)jdata.get("projectile.owner"));

                    sfs.putInt("networkid", (int)jdata.get("projectile.id"));

                    sfs.putFloat("projectile.position.x", ((float[])jdata.get("projectile.position"))[0]);
                    sfs.putFloat("projectile.position.y", ((float[])jdata.get("projectile.position"))[1]);
                    sfs.putFloat("projectile.position.z", ((float[])jdata.get("projectile.position"))[2]);

                    sfs.putFloat("projectile.rotation.x", ((float[])jdata.get("projectile.rotation"))[0]);
                    sfs.putFloat("projectile.rotation.y", ((float[])jdata.get("projectile.rotation"))[1]);
                    sfs.putFloat("projectile.rotation.z", ((float[])jdata.get("projectile.rotation"))[2]);
                    sfs.putFloat("projectile.rotation.w", ((float[])jdata.get("projectile.rotation"))[3]);

                    sendToAllInGame("projectile.update", sfs, user, false);

                    break;

                case "player.update":
                    HashMap<String, Object> jdata2 = (HashMap<String, Object>)data;
                    user = getUser(new Long((long)jdata2.get("player.id")).intValue());
                    
                    sfs.putInt("networkid", user.getId());
                    sfs.putFloat("player.position.x", ((float[])jdata2.get("player.position"))[0]);
                    sfs.putFloat("player.position.y", ((float[])jdata2.get("player.position"))[1]);
                    sfs.putFloat("player.position.z", ((float[])jdata2.get("player.position"))[2]);

                    sfs.putFloat("player.rotation.x", ((float[])jdata2.get("player.rotation"))[0]);
                    sfs.putFloat("player.rotation.y", ((float[])jdata2.get("player.rotation"))[1]);
                    sfs.putFloat("player.rotation.z", ((float[])jdata2.get("player.rotation"))[2]);
                    sfs.putFloat("player.rotation.w", ((float[])jdata2.get("player.rotation"))[3]);
                    
                    sendToAllInGame("player.update", sfs, user, false);
                    break;

                case "collision":
                    break;

                case "game.over":
                    break;

                case "player.death":
                    break;

                case "projectile.expire":
                    HashMap<String, Object> jdata3 = (HashMap<String, Object>)data;
                    user = getUser((int)jdata3.get("projectile.owner"));
                    
                    sfs.putInt("networkid", (int)jdata3.get("projectile.id"));
                    
                    sendToAllInGame("projectile.expire", sfs, user, false);
                    break;

                default:
                    trace("Unrecognized Event [ event: " + event + " data: " + data.toString());
                    break;
            }
        } catch(Exception e){
            onException(e);
        }
    }
}
