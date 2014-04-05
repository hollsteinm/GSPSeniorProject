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
import com.smartfoxserver.v2.components.login.ILoginAssistantPlugin;
import com.smartfoxserver.v2.components.login.LoginAssistantComponent;
import com.smartfoxserver.v2.components.login.LoginData;
import com.smartfoxserver.v2.components.signup.ISignUpAssistantPlugin;
import com.smartfoxserver.v2.components.signup.PasswordMode;
import com.smartfoxserver.v2.components.signup.SignUpAssistantComponent;
import com.smartfoxserver.v2.components.signup.SignUpConfiguration;
import com.smartfoxserver.v2.components.signup.SignUpValidationException;
import com.smartfoxserver.v2.core.SFSEventType;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.gspteama.db.*;

/**
 *
 * @author Martin
 */
@Instantiation(Instantiation.InstantiationMode.SINGLE_INSTANCE)
public class StarboundAcesExtension extends SFSExtension{
    //integer is the room id and game is a game instance
    private static ConcurrentHashMap<Integer, Game> gameList = null;
    
    private SignUpAssistantComponent signup;
    private LoginAssistantComponent login;
    
    @Override
    public void init(){
        trace("Inside of StarboundAcesExtension Method... let's roll!");
        gameList = new ConcurrentHashMap<>();
        addRequestHandler("server", MultiHandler.class);
        addEventHandler(SFSEventType.ROOM_ADDED, RoomAddEvent.class);
        addEventHandler(SFSEventType.USER_JOIN_ROOM, RoomJoinEvent.class);
    
        signUp();
        login();
    }
    
    private class SignUpAssistantPlugin implements ISignUpAssistantPlugin{
        @Override
        public void execute(User user, ISFSObject params, SignUpConfiguration config) throws SignUpValidationException{
            //stuff
        }
    }
    
    private class LoginAssistantPlugin implements ILoginAssistantPlugin{
        @Override
        public void execute(LoginData data){
            //stuff            
        }
    }
    
    private void login(){
        login = new LoginAssistantComponent(this);
        login.getConfig().loginTable = UsernameTable.TABLE_NAME;
        login.getConfig().userNameField = UsernameTable.ROW_USER_NAME;
        login.getConfig().passwordField = UsernameTable.ROW_USER_PASSWORD;
        login.getConfig().useCaseSensitiveNameChecks = true;
        login.getConfig().activationField = UsernameTable.ROW_USER_ACTIVATION_CODE;
        login.getConfig().activationErrorMessage =
                "Your account is not yet activated. "+
                "Please check you inbox for the confirmation email";
        login.getConfig().postProcessPlugin = new LoginAssistantPlugin();
    }
    
    private void signUp(){
        signup = new SignUpAssistantComponent();
        signup.getConfig().signUpTable = UsernameTable.TABLE_NAME;
        signup.getConfig().minUserNameLength = 4;
        signup.getConfig().maxUserNameLength = 64;
        signup.getConfig().minPasswordLength = 8;
        signup.getConfig().maxPasswordLength = 64;
        
        signup.getConfig().passwordMode = PasswordMode.MD5;
        signup.getConfig().userIsActiveField = UsernameTable.ROW_USER_STATE;
        signup.getConfig().activationCodeField = UsernameTable.ROW_USER_ACTIVATION_CODE;
        signup.getConfig().logSQL = true;
        
        signup.getConfig().emailResponse.isActive = true;
        signup.getConfig().emailResponse.fromAddress = "signup@starboundaces.com";
        signup.getConfig().emailResponse.subject = "Starbound Aces Registration";
        signup.getConfig().emailResponse.template = "SignUpEmailTemplates/SignUpConfirmation.html";
        signup.getConfig().preProcessPlugin = new SignUpAssistantPlugin();
        addRequestHandler(SignUpAssistantComponent.COMMAND_PREFIX, signup);
    }
    
    @Override
    public void destroy(){
        login.destroy();
        super.destroy();
    }
    
    public Game getGame(int roomid) throws Exception{
        if(!gameList.containsKey(roomid)){
            throw new Exception("Game does not exist: " + roomid);
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
