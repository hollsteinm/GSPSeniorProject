/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.main;
import com.gspteama.extensions.MultiHandler;
import com.smartfoxserver.v2.extensions.*;
import java.util.concurrent.ConcurrentHashMap;
import com.gspteama.gamedriver.Game;
import com.smartfoxserver.v2.annotations.Instantiation;
import com.smartfoxserver.v2.core.SFSEventType;

/**
 *
 * @author Martin
 */
@Instantiation(Instantiation.InstantiationMode.SINGLE_INSTANCE)
public class StarboundAcesExtension extends SFSExtension{
    //integer is the room id and game is a game instance
    private static ConcurrentHashMap<Integer, Game> gameList = null;
    
    @Override
    public void init(){
        trace("Inside of StarboundAcesExtension Method... let's roll!");
        gameList = new ConcurrentHashMap<>();
        addRequestHandler("server", MultiHandler.class);
        addEventHandler(SFSEventType.ROOM_ADDED, RoomAddEvent.class);
        addEventHandler(SFSEventType.USER_JOIN_ROOM, RoomJoinEvent.class);
    }
    
    @Override
    public void destroy(){
        super.destroy();
    }
    
    public Game getGame(int roomid) throws Exception{
        if(!gameList.containsKey(roomid)){
            throw new Exception("Game does not exist");
        }
        
        try{
            trace("Getting game: " + roomid);
            return gameList.get(roomid);
        } catch (Exception e){
            throw e;
        }
 
    }
    
    public void createGame(int roomid) throws Exception{
        if(gameList.containsKey(roomid)){
            throw new Exception("Game already exists");
        }
        gameList.put(roomid, new Game());
        trace("New game added: " + roomid);
    }
}
