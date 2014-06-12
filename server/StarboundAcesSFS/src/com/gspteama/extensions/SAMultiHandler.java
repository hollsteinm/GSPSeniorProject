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
import com.smartfoxserver.v2.SmartFoxServer;
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
import java.util.concurrent.TimeUnit;


/**
*
* @author Martin
*/
@com.smartfoxserver.v2.annotations.MultiHandler
public class SAMultiHandler extends BaseClientRequestHandler implements IEventListener{
    
    @Override
    public void handleClientRequest(User user, ISFSObject params){
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
    
    User getUser(int id){
        return this.getParentExtension().getParentZone().getUserById(id);
    }
    //End Helpers
    
    private void handleGameStartRequest(User user, ISFSObject params){
        try{
            DBService.updateGameStatus(getConnection(),
                    "A", user.getLastJoinedRoom().getName());
                
            ISFSObject data = SFSObject.newInstance();
            data.putUtfString("game", user.getLastJoinedRoom().getName());
            
            getGame(user).Register(this);
            
            List<User> users = user.getLastJoinedRoom().getUserList();
            for(User u : users){
                HashMap<String, Object> shiphull = DBService.selectShipConfiguration(
                        getConnection(),
                        (long)u.getVariable("shipid").getIntValue());
                
                Weapon w = DBService.selectWeaponConfigurations(getConnection(), 
                        u.getVariable("weapon").getStringValue(), 
                        (long)u.getId());
                
                Ship s = new Ship( (long)u.getId(),
                        (long)u.getVariable("shipid").getIntValue(),
                        shiphull.get("name").toString(),
                        w,
                        (Hull)shiphull.get("hull"),
                        (float)shiphull.get("maxVelocity"),
                        (float)shiphull.get("maxEnergy")
                );
                
                getGame(user).AddPlayer(new Player((long)u.getId(), u.getName(),s));
                trace("Player ready!\nPlayer Name [" + u.getName() + "]\nShip[" + 
                        s.toString() + "]\n");
            }
            
            getGame(user).initialize();
            send("gamelist.remove", data, this.getParentExtension().getParentZone().getRoomByName("lobby").getUserList());
            
        } catch(Exception e){
            onException(e);
        } finally{
            sendToAllInGame("game.start", SFSObject.newInstance(), user, false);
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
        switch(event){
            case "projectile.spawn":
                Projectile ref = (Projectile)data;
                ISFSObject sfs = SFSObject.newInstance();
                sfs.putInt("networkid", ref.hashCode());
                User user = getUser(ref.getOwningPlayerId());
                
                /*debugging*/trace(sfs.getDump());
                sendToAllInGame("projectile.spawn", sfs, user, false);                
                break;
                
            case "projectile.update":
                HashMap<String, Object> jdata = (HashMap<String, Object>)data;
                
                ISFSObject sfs2 = SFSObject.newInstance();
                sfs2.putInt("networkid", (int)jdata.get("projectile.id"));
                User user2 = getUser((int)jdata.get("projectile.owner"));
                
                sfs2.putFloat("projectile.position.x", ((float[])jdata.get("projectile.position"))[0]);
                sfs2.putFloat("projectile.position.y", ((float[])jdata.get("projectile.position"))[1]);
                sfs2.putFloat("projectile.position.z", ((float[])jdata.get("projectile.position"))[2]);
                
                sfs2.putFloat("projectile.rotation.x", ((float[])jdata.get("projectile.rotation"))[0]);
                sfs2.putFloat("projectile.rotation.y", ((float[])jdata.get("projectile.rotation"))[1]);
                sfs2.putFloat("projectile.rotation.z", ((float[])jdata.get("projectile.rotation"))[2]);
                sfs2.putFloat("projectile.rotation.w", ((float[])jdata.get("projectile.rotation"))[3]);
                
                /*testing*/trace(sfs2.getDump());
                sendToAllInGame("projectile.update", sfs2, user2, false);
                
                break;
                    
            case "player.update":
                break;
                        
            case "collision":
                break;
                
            case "game.over":
                break;
                    
            case "player.death":
                break;
                
            case "projectile.death":
                break;
            
            default:
                trace("Unrecognized Event [ event: " + event + " data: " + data.toString());
                break;
        }
    }
}
