/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.main;

import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.SFSRoomRemoveMode;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;

/**
 *
 * @author Martin
 */
public class RoomAddEvent extends BaseServerEventHandler{

    @Override
    public void handleServerEvent(ISFSEvent isfse) throws SFSException {
       Room room  = (Room)(isfse.getParameter(SFSEventParam.ROOM));
       trace("Room created:  " + room.getName());
       room.setAutoRemoveMode(SFSRoomRemoveMode.WHEN_EMPTY);
       try{
          ((StarboundAcesExtension)this.getParentExtension()).createGame(room.getId());
       } catch (Exception e){
           trace(e.getMessage());
       }
    }    
}
