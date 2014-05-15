/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

package com.gspteama.main;

import com.gspteama.db.DBService;
import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;
import java.sql.Connection;
import java.sql.SQLException;

/**
 *
 * @author Martin
 */
public class RoomRemoveEvent extends BaseServerEventHandler{

    @Override
    public void handleServerEvent(ISFSEvent isfse) throws SFSException {
        Room room  = (Room)(isfse.getParameter(SFSEventParam.ROOM));
        
        if(room.isGame()){
            try{                
                Connection conref = this.getParentExtension().getParentZone().getDBManager().getConnection();
                DBService.updateGameStatus(conref, "D", room.getName());
                
            }catch(SQLException e){
                for(StackTraceElement ste : e.getStackTrace()){
                    trace(e.toString());
                }
            }
        }
    }    
}