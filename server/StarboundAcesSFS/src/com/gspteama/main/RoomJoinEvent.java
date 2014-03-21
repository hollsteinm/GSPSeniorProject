/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.main;

import com.gspteama.gamedriver.Game;
import com.gspteama.gamedriver.Player;
import com.gspteama.gamedriver.Ship;
import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;

/**
 *
 * @author Martin
 */
public class RoomJoinEvent extends BaseServerEventHandler{

    @Override
    public void handleServerEvent(ISFSEvent isfse) throws SFSException {
        Room room  = (Room)(isfse.getParameter(SFSEventParam.ROOM));
        Game game = ((StarboundAcesExtension)this.getParentExtension()).getGame(room.getId());
        
        String username = (String)((User)isfse.getParameter(SFSEventParam.USER)).getName();
        int playerid = (int)((User)isfse.getParameter(SFSEventParam.USER)).getId();
        
        Ship ship = new Ship();
        
        //initialize spawn (TODO: fixed, its pretty hacked, make real spawns)
        ship.setPosition(new float[]{2 * playerid, 2 * playerid, 0.0f});
        ISFSObject data = SFSObject.newInstance();
        data.putFloat("position.x", ship.getPosition()[0]);
        data.putFloat("position.y", ship.getPosition()[1]);
        data.putFloat("position.z", ship.getPosition()[2]);
        data.putFloat("rotation.x", 0.0f);
        data.putFloat("rotation.y", 0.0f);
        data.putFloat("rotation.z", 0.0f);
        data.putFloat("rotation.w", 0.0f);
        data.putInt("player", playerid);
        this.send("transform", data, room.getPlayersList());        
        
        game.AddPlayer(playerid, new Player(username));
        game.AddShip(playerid, ship);
    }   
}
