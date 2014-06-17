/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.main;

import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author Martin
 */
public class RoomJoinEvent extends BaseServerEventHandler{

    @Override
    public void handleServerEvent(ISFSEvent isfse) throws SFSException {
        Room room = (Room)(isfse.getParameter(SFSEventParam.ROOM));
        User user = (User)(isfse.getParameter(SFSEventParam.USER));
        
        if(room.isGame()){
            try {                
                if(((StarboundAcesExtension)this.getParentExtension()).getGame(user.getLastJoinedRoom().getId()) == null){
                    ((StarboundAcesExtension)this.getParentExtension()).createGame(user.getLastJoinedRoom().getId());
                }
            } catch (Exception ex) {
                trace(ex.toString());
                Logger.getLogger(RoomJoinEvent.class.getName()).log(Level.SEVERE, null, ex);
            }
        }
    }   
}
