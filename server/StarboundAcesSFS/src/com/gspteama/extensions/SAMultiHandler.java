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
          
          case "input":
            handleInputRequest(user, params);
            break;
            
          case "collision":
            handleCollisionRequest(user, params);
            break;
          
          case "shoot":
            handleeShootRequest(user, params);
            break;
            
          case "scores":
            handleScoresRequest(user, params);
            break;
            
          case "gameslist":
            handleGamesListRequest(user, params);
            break;
            
          case "gamestart":
            handleGameStartRequest(user, params);
            break;
          
          default:
              trace("Unhandled RequestID: " + regId);
              break;
        }
    }
    
    //Helpers
    private Connection getConnection(){
        this.getParentExtension().getParentZone().getDBManager().getConnection();
    }
    
    private void sendToAllInGame(String event, ISFSObject response, User user, boolean udp){
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
    //End Helpers
    
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
            
        }catch(Exception e){
            onException(e);
        }
        
    }
    
    private void handleShootRequest(User user, ISFSObject params){
        try{
            
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
}
