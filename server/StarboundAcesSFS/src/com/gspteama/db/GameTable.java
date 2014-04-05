/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.db;

import java.util.ArrayList;
import java.util.Date;

/**
 *
 * @author Martin
 */
public class GameTable {
    public static final String      TABLE_NAME          = "sa_game";
    public static final String      ROW_USER_LIST       = "game_user_list";
    public static final String      ROW_GAME_ID         = "game_id";
    public static final String      ROW_GAME_NAME       = "game_name";
    public static final String      ROW_GAME_PASSWORD   = "game_password";
    public static final String      ROW_GAME_STATE      = "game_state";
    public static final String      ROW_GAME_START_TIME = "game_start_time";
    public static final String      ROW_GAME_END_TIME   = "game_end_time";
    
    private ArrayList<Integer>  joinedUserIds   = new ArrayList<Integer>();
    private String              gameName        = "";
    private String              password        = "";
    private String              state           = "";
    private Date                date            = new Date();
    private Date                enddate         = new Date();
    private int                 id              ;
    
    public GameTable(ArrayList<Integer> joinedUserIds,
            String gameName,
            String password,
            String state,
            Date date, 
            int id){
        this.joinedUserIds = joinedUserIds;
        this.gameName = gameName;
        this.password = password;
        this.state = state;
        this.date = date;
        this.id = id;
    }
    
    public void addJoinedUserId(Integer id){
        joinedUserIds.add(id);
    }

    public ArrayList<Integer> getJoinedUserIds() {
        return joinedUserIds;
    }

    public void setJoinedUserIds(ArrayList<Integer> joinedUserIds) {
        this.joinedUserIds = joinedUserIds;
    }

    public String getGameName() {
        return gameName;
    }

    public void setGameName(String gameName) {
        this.gameName = gameName;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public String getState() {
        return state;
    }

    public void setState(String state) {
        this.state = state;
    }

    public Date getDate() {
        return date;
    }

    public void setDate(Date date) {
        this.date = date;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public Date getEnddate() {
        return enddate;
    }

    public void setEnddate(Date enddate) {
        this.enddate = enddate;
    }
    
    
    
}