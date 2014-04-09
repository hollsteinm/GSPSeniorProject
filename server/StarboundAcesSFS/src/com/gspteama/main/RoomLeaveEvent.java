/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.main;

import com.gspteama.db.DBService;
import com.gspteama.gamedriver.Game;
import com.gspteama.gamedriver.Player;
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
public class RoomLeaveEvent extends BaseServerEventHandler{
    @Override
    public void handleServerEvent(ISFSEvent isfse) throws SFSException {
        Room room = (Room)(isfse.getParameter(SFSEventParam.ROOM));
        User user = (User)(isfse.getParameter(SFSEventParam.USER));
        
        if(room.isGame()){
            try{
                Game game = ((StarboundAcesExtension)this.getParentExtension()).getGame(user.getLastJoinedRoom().getId());
                Player player = game.getPlayer(user.getId());
                DBService.updatePlayerScore(
                        this.getParentExtension().getParentZone().getDBManager().getConnection(), 
                        player);

            } catch (Exception e){
                trace(e.toString());
                Logger.getLogger(DBService.class.getName()).log(Level.SEVERE, null, e);
            }
        }
    }
    
}
