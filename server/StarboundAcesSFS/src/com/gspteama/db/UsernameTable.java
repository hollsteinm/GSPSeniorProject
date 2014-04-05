/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.db;

import java.util.Date;

/**
 *
 * @author Martin
 */
public class UsernameTable {
    public static final String  TABLE_NAME                  = "sa_user";
    public static final String  ROW_USER_NAME               = "user_name";
    public static final String  ROW_USER_ID                 = "user_id";
    public static final String  ROW_USER_PASSWORD           = "user_password";
    public static final String  ROW_USER_ACTIVATION_CODE    = "user_activation_code";
    public static final String  ROW_USER_JOIN_DATE          = "user_join_date";
    public static final String  ROW_USER_EMAIL              = "user_email";
    public static final String  ROW_USER_STATE              = "user_state";
    
    private String  username    = "";
    private String  password    = "";
    private String  email       = "";
    private String  state       = "";
    private String  activation  = "";
    private Date    joinDate    = new Date();
    private int     id          ;
    
    public UsernameTable(String username,
            String password,
            String email,
            String state,
            Date joinDate,
            int id){
        this.username = username;
        this.password = password;
        this.email = email;
        this.state = state;
        this.joinDate = joinDate;
        this.id = id;
    }
    
    public String getUsername() {
        return username;
    }

    public void setUsername(String username) {
        this.username = username;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public String getState() {
        return state;
    }

    public void setState(String state) {
        this.state = state;
    }

    public Date getJoinDate() {
        return joinDate;
    }

    public void setJoinDate(Date joinDate) {
        this.joinDate = joinDate;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getActivation() {
        return activation;
    }

    public void setActivation(String activation) {
        this.activation = activation;
    }
    
}
