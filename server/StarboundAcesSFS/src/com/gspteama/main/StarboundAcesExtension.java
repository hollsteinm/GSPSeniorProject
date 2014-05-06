/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.main;
import com.gspteama.db.*;
import com.gspteama.extensions.MultiHandler;
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
import com.smartfoxserver.v2.extensions.*;
import java.util.concurrent.ConcurrentHashMap;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author Martin
 */
//@Instantiation(Instantiation.InstantiationMode.SINGLE_INSTANCE)
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
    
        try{
            signUp();
            login();
        } catch (Exception e){
            trace(e.toString());
        }
    }
    
    private class PostSignUpAssistantPlugin implements ISignUpAssistantPlugin{
        
        @Override
        public void execute(User user, ISFSObject params, SignUpConfiguration config) throws SignUpValidationException{
            trace("Entering postprocessor for  SignUpAssistant.");
            try{
                String user_name = user.getVariable("username").getStringValue();
                trace("Username: " + user_name);
                
                long id = DBService.userIdFromUsername(user.getZone().getDBManager().getConnection(),
                        user_name);
                
                trace("ID: " + Long.toString(id));
                
                DBService.insertScoreTable(
                        user.getZone().getDBManager().getConnection(),
                        id);
            } catch(Exception e){
                trace(e.toString());
                Logger.getLogger(DBService.class.getName()).log(Level.SEVERE, null, e);
            }
            trace("Operation completed successfully");
        }
    }
    
    private class PreSignUpAssistantPlugin implements ISignUpAssistantPlugin{
        @Override
        public void execute(User user, ISFSObject params, SignUpConfiguration config) throws SignUpValidationException{
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
        login.getConfig().userNameField = UsernameTable.COL_USER_NAME;
        login.getConfig().passwordField = UsernameTable.COL_USER_PASSWORD;
        login.getConfig().allowGuests = true;
        login.getConfig().useCaseSensitiveNameChecks = true;
        //login.getConfig().activationField = UsernameTable.COL_USER_ACTIVATION_CODE;
        //login.getConfig().activationErrorMessage =
        //        "Your account is not yet activated. "+
        //        "Please check you inbox for the confirmation email";
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
        signup.getConfig().idField = UsernameTable.COL_USER_ID;
        signup.getConfig().usernameField = UsernameTable.COL_USER_NAME;
        signup.getConfig().passwordField = UsernameTable.COL_USER_PASSWORD;
        signup.getConfig().emailField = UsernameTable.COL_USER_EMAIL;
        //signup.getConfig().userIsActiveField = UsernameTable.COL_USER_STATE;
        //signup.getConfig().activationCodeField = UsernameTable.COL_USER_ACTIVATION_CODE;
        signup.getConfig().logSQL = true;
        
        signup.getConfig().emailResponse.isActive = true;
        signup.getConfig().emailResponse.fromAddress = "starboundaces@gmail.com";
        signup.getConfig().emailResponse.subject = "Starbound Aces Registration";
        signup.getConfig().emailResponse.template = "SignUpEmailTemplates/SignUpConfirmation.html";
        signup.getConfig().postProcessPlugin = new PostSignUpAssistantPlugin();
        
        addRequestHandler(SignUpAssistantComponent.COMMAND_PREFIX, signup);
    }
    
    @Override
    public void destroy(){
        super.destroy();
        login.destroy();
    }
    
    public Game getGame(int roomid) throws Exception{
        return gameList.get(roomid); 
    }
    
    public void createGame(int roomid) throws Exception{
        if(gameList.containsKey(roomid)){
            throw new Exception("Game already exists");
        }
        gameList.put(roomid, new Game());
        trace("New game added: " + roomid);
        
        try{
            //TODO: put game in database
        }catch(Exception e){
            trace(e.toString());
        }
    }
}
