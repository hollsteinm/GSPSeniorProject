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
            case "transform":
                handleTransform(user, params);
                break;

            default:
                trace("Unrecognized request Id sent... ignoring");
                break;
        }
    }    
    
    private void handleTransform(User user, ISFSObject params){
        try{
            Game game = ((StarboundAcesExtension)this.getParentExtension()).getGame(user.getLastJoinedRoom().getId());
            Ship ship = game.getShip(user.getId());

            ship.setPosition(new float[]{
                params.getFloat("position.x"), 
                params.getFloat("position.y"), 
                params.getFloat("position.z")
            });
            
            ship.setRotation(new float[]{
                params.getFloat("rotation.x"), 
                params.getFloat("position.y"), 
                params.getFloat("position.z"),
                params.getFloat("position.w")
            });


            ISFSObject response = SFSObject.newInstance();
            response.putInt("player", user.getId());
            response.putFloat("position.x", ship.getPosition()[0]);
            response.putFloat("position.y", ship.getPosition()[1]);
            response.putFloat("position.z", ship.getPosition()[2]);
            response.putFloat("rotation.x", ship.getRotation()[0]);
            response.putFloat("rotation.y", ship.getRotation()[1]);
            response.putFloat("rotation.z", ship.getRotation()[2]);  
            response.putFloat("rotation.w", ship.getRotation()[3]);
        
            //this.send("transform", response, user.getLastJoinedRoom().getPlayersList(), true);
            this.send("transform", response, user.getLastJoinedRoom().getPlayersList());
        }catch(Exception e){
            trace(e.toString());
        }
    }
}
