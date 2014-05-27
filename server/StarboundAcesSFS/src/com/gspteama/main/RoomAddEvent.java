/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.main;

import com.gspteama.db.DBService;
import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.SFSRoomRemoveMode;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;
import java.sql.Connection;
import java.sql.SQLException;

/**
 *
 * @author Martin
 */
public class RoomAddEvent extends BaseServerEventHandler{

    @Override
    public void handleServerEvent(ISFSEvent isfse) throws SFSException {
       Room room  = (Room)(isfse.getParameter(SFSEventParam.ROOM));
       User user = room.getOwner();
       
       if(room.isGame()){
            trace("Room created:  " + room.getName() + " by " + user.getName());
            room.setAutoRemoveMode(SFSRoomRemoveMode.WHEN_EMPTY);
            
            //add game to database
            try{
                //TODO: fix database to store array as bigint
                Connection conref = this.getParentExtension().getParentZone().getDBManager().getConnection();
                long userid = DBService.userIdFromUsername(conref, user.getName());
                
                conref = this.getParentExtension().getParentZone().getDBManager().getConnection();
                DBService.insertNewGame(conref,
                        room.getName(), (int)userid);
                
            }catch(SQLException e){
                for(StackTraceElement ste : e.getStackTrace()){
                    trace(e.toString());
                }
            }           
            
            try{
                ((StarboundAcesExtension)this.getParentExtension()).createGame(room.getId());
            } catch (Exception e){
                trace("RoomAddEvent: " + e.getMessage());
            }
       }
    }    
}
