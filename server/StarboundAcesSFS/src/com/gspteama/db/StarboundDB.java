/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.db;

import java.sql.Connection;

/**
 *
 * @author Martin
 */
public class StarboundDB {
    private String      username    = "";
    private String      password    = "";
    private String      server      = "";
    private String      jdbc        = "";
    private int         port        = 0;
    private Connection  con     = null;
    
    //when passing this information, don't share with the world on github - load
    //from an uncommited config file
    public StarboundDB(String username, String password, String server, int port){
        this.username = username;
        this.password = password;
        this.server = server;
        this.port = port;        
    }

    public String getUsername() {
        return username;
    }

    public String getServer() {
        return server;
    }

    public int getPort() {
        return port;
    }
    
    public Connection getConnection(){
        return con;
    }
    
}
