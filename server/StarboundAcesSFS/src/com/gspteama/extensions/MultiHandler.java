/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.extensions;

import com.gspteama.gamedriver.Game;
import com.gspteama.gamedriver.Ship;
import com.gspteama.main.StarboundAcesExtension;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import com.smartfoxserver.v2.extensions.SFSExtension;
import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

/**
 *
 * @author Martin
 */
public class MultiHandler extends BaseClientRequestHandler{
    
    @Override
    public void handleClientRequest(User user, ISFSObject params){
        String reqId = params.getUtfString(SFSExtension.MULTIHANDLER_REQUEST_ID);
        
        switch(reqId){
            case "transform":
                handleTransform(user, params);
                break;
            case "charmessage":
                handleCharMessage(user, params);
                break;
            default:
                trace("Unrecognized request Id sent... ignoring");
                break;
        }
    }    
    
    private void handleTransform(User user, ISFSObject params){
        trace("Handling transform");
        Game game = ((StarboundAcesExtension)this.getParentExtension()).getGame(user.getLastJoinedRoom().getId());
        Ship ship = game.getShip(user.getId());
        
        float position[] = new float[3];        
        int i = 0;
        for(float f : params.getFloatArray("position")){
            position[i] = f;
            i++;
        }
        ship.setPosition(position);
        
        float rotation[] = new float[3];
        i = 0;
        for(float f : params.getFloatArray("rotation")){
            rotation[i] = f;
            i++;
        }
        ship.setRotation(rotation);
        
        ISFSObject response = SFSObject.newInstance();
        response.putInt("playerid", user.getId());
        
        //the arrays for SFSObjects require collections... so cannot call get()
        //methods from the ship as they return primitive arrays...
        ArrayList<Float> data = new ArrayList<>();
        for(float f : position){
            data.add(f);
        }
        data.clear();
        response.putFloatArray("position", data);
        for(float f : rotation){
            data.add(f);
        }
        response.putFloatArray("rotation", data);
        this.send("transform", response, user.getLastJoinedRoom().getPlayersList(), true);
    }
    
    private void handleCharMessage(User user, ISFSObject params){
        
    }
}
